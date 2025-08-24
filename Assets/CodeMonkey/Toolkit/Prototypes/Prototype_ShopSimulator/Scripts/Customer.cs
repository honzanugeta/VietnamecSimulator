using CodeMonkey.Toolkit.TChatBubble3D;
using CodeMonkey.Toolkit.TLookAtCamera;
using CodeMonkey.Toolkit.TRandomData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace CodeMonkey.Toolkit.ShopSimulatorDemo
{
    public class Customer : MonoBehaviour
    {
        private static List<Customer> instanceList = new List<Customer>();
        public static List<Customer> GetInstanceList() => instanceList;

        private CustomerAnimator animator;
        private NavMeshAgent agent;

        private enum State
        {
            GoingToShelf,
            GrabbingItem,
            WaitingForCheckoutToBeFree,
            GoingToCheckout,
            WaitingToPay,
            Leaving,
        }

        // Shopping goal picked at Start
        private int productsToGrab;
        private int RemainingToGrab => Mathf.Max(0, productsToGrab - grabbedObjectTypeList.Count);

        private Shelf shelf; // the shelf we currently reserved/are using
        private State state;
        private float timer;
        private List<ObjectType> grabbedObjectTypeList = new List<ObjectType>();
        private HashSet<Shelf> visitedShelves = new HashSet<Shelf>();

        // Tunables
        [SerializeField] private float shelfStoppingDistance = 0.45f;
        [SerializeField] private float checkoutStoppingDistance = 0.45f;
        [SerializeField] private float leaveStoppingDistance = 0.25f;
        [SerializeField] private float rotationLerpSpeed = 10f;
        [SerializeField] private float moveSpeed = 3.5f; // NavMeshAgent speed
        [SerializeField] private ObstacleAvoidanceType obstacleAvoidanceType;

        private void Awake()
        {
            instanceList.Add(this);

            animator = GetComponent<CustomerAnimator>();
            agent = GetComponent<NavMeshAgent>();

            if (animator == null)
                Debug.LogError("Animator component not found in children of " + gameObject.name);
            if (agent == null)
                Debug.LogError("NavMeshAgent component missing on " + gameObject.name);

            if (agent != null)
            {
                agent.speed = moveSpeed;
                agent.acceleration = 12f;
                agent.angularSpeed = 0f;            // we handle rotation manually
                agent.updateRotation = false;
                agent.autoBraking = true;
                agent.obstacleAvoidanceType = obstacleAvoidanceType;

                // If spawned slightly off the NavMesh, try to snap on
                if (!agent.isOnNavMesh)
                {
                    if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                    else
                    {
                        Debug.LogWarning($"{name} could not find NavMesh nearby; destroying.");
                        DestroySelf();
                        return;
                    }
                }
            }
        }

        private void Start()
        {
            productsToGrab = Random.Range(1, 4); // 1 až 3 produkty
            GoToNextShelfOrQueue();
        }

        private void Update()
        {
            // Kontrola stavu obchodu
            if (StoreManager.Instance != null && !StoreManager.Instance.IsStoreOpen())
            {
                if (grabbedObjectTypeList.Count > 0)
                {
                    // Only change state if not already in checkout/leave flow

                    if (state != State.WaitingForCheckoutToBeFree &&
                        state != State.GoingToCheckout &&
                        state != State.WaitingToPay &&
                        state != State.Leaving)
                    {
                        state = State.WaitingForCheckoutToBeFree;
                    }
                }
                else
                {
                    // No items -> leave immediately
                    LeaveShop();
                }
            }

            if (agent != null)
                animator.SetWalking(!agent.isStopped && agent.velocity.sqrMagnitude > 0.01f);

            HandleRotation();

            switch (state)
            {
                default:
                case State.GoingToShelf:
                    if (ReachedDestination())
                    {
                        // Convert reservation into active shopping (or bail if we lost it)
                        if (shelf != null && shelf.BeginShopping(this))
                        {
                            state = State.GrabbingItem;
                            agent.isStopped = true;
                            timer = Random.Range(1.2f, 2.2f); // slightly snappier
                            animator.SetWalking(false);
                        }
                        else
                        {
                            // Couldn’t begin shopping (edge case) — try again elsewhere
                            if (shelf != null)
                                shelf.CancelReservation(this);
                            GoToNextShelfOrQueue();
                        }
                    }
                    break;

                case State.GrabbingItem:
                    if (shelf != null)
                    {
                        // Face the shelf while "shopping"
                        FacePointOnPlane(shelf.GetLookingPositionWhenShoping());
                    }

                    timer -= Time.deltaTime;
                    if (timer <= 0f)
                    {
                        // Attempt to grab up to RemainingToGrab from this shelf
                            for (int i = 0; i < RemainingToGrab; i++)
                            {
                                ObjectType ot = shelf.PeekRandomItem();
                                if (ot != ObjectType.None)
                                {
                                    var boxData = GameAssetsShopSimulator.Instance.GetObjectTypeBoxData(ot);
                                    float sellPrice = boxData.sellPrice;
                                    float currentPrice = PriceManager.Instance.GetPrice(ot);

                                    // Generate random percentage between 10% and 30%
                                    float randomPercent = UnityEngine.Random.Range(0.10f, 0.30f);

                                    if (currentPrice > sellPrice * (1 + randomPercent))
                                    {
                                        // Too expensive, don't add product
                                        Debug.Log($"TOO EXPENSIVE! Current={currentPrice}, Allowed={sellPrice * (1 + randomPercent)}");

                                        ChatBubble3D.Create(transform, new Vector3(0, 2.5f, 0), ChatBubble3D.IconType.Angry,
                                            "To je moc drahé!", 0.07f, 2f)
                                            .transform.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

                                        // Skip this item and try next
                                        continue;
                                    }
                                    else
                                    {
                                        // Price okay, actually grab the product from shelf
                                        ObjectType grabbedItem = shelf.TryGrabRandomItem();
                                        if (grabbedItem != ObjectType.None)
                                        {
                                            grabbedObjectTypeList.Add(grabbedItem);

                                            string objectName = boxData.objectName;
                                            string[] messageArray = new string[] { $"Ach ano {objectName}!" };
                                            ChatBubble3D.Create(transform, new Vector3(0, 2.5f, 0), ChatBubble3D.IconType.Happy,
                                                messageArray.GetRandomElement(), 0.07f, 2f)
                                                .transform.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

                                            if (RemainingToGrab <= 0)
                                                break; // reached goal
                                        }
                                        else
                                        {
                                            // Couldn't grab item (shelf became empty)
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    // This shelf ran out
                                    break;
                                }
                            }

                        // Free the shelf no matter what
                        if (shelf != null)
                            shelf.EndShopping(this);

                        if (RemainingToGrab <= 0)
                        {
                            // Done shopping — queue for checkout
                            state = State.WaitingForCheckoutToBeFree;
                        }
                        else
                        {
                            // Need more items — try another shelf (or queue/leave if none)
                            GoToNextShelfOrQueue();
                        }
                    }
                    break;

                case State.WaitingForCheckoutToBeFree:
                    agent.isStopped = true;
                    if (Checkout.Instance.IsFree())
                    {
                        Checkout.Instance.SetCustomer(this);
                        state = State.GoingToCheckout;
                        SetDestination(Checkout.Instance.GetCustomerPosition(), checkoutStoppingDistance);
                    }
                    break;

                case State.GoingToCheckout:
                    if (ReachedDestination())
                    {
                        foreach (var grabbedObjectType in grabbedObjectTypeList)
                        {
                            Checkout.Instance.AddObject(grabbedObjectType);
                        }
                        state = State.WaitingToPay;
                        agent.isStopped = true;
                        animator.SetWalking(false);
                    }
                    break;

                case State.WaitingToPay:
                    agent.isStopped = true;
                    break;

                case State.Leaving:
                    if (ReachedDestination())
                    {
                        DestroySelf();
                    }
                    break;
            }
        }

        private void GoToNextShelfOrQueue()
        {
            if (RemainingToGrab <= 0)
            {
                // Goal reached — head to checkout flow
                state = State.WaitingForCheckoutToBeFree;
                return;
            }

            // Gather candidate shelves that are free (not reserved and not being shopped)
            List<Shelf> candidates = new List<Shelf>();
            foreach (var s in Shelf.instanceList)
            {
                if (s.IsFree())
                {
                    candidates.Add(s);
                }
            }

            if (candidates.Count == 0)
            {
                // No shelf is free
                if (grabbedObjectTypeList.Count == 0)
                {
                    ChatBubble3D.Create(transform, new Vector3(0, 2f, 0), ChatBubble3D.IconType.Neutral,
                        "Vypadá to, že je obchod plný. Jdu pryč.", .07f, 3f)
                        .transform.AddLookAtCamera(TLookAtCamera.LookAtCamera.Method.LookAtInverted);

                    // Nahraďte volání LeaveShop() tímto:
                    StartCoroutine(WaitAndLeaveShop(1.5f));
                }
                else
                {
                    // We already have something — queue up
                    state = State.WaitingForCheckoutToBeFree;
                }
                return;
            }

            // Soft-prefer unvisited; then try to reserve one randomly
            List<Shelf> pool = candidates.FindAll(s => !visitedShelves.Contains(s));
            if (pool.Count == 0)
                pool = candidates;

            Shelf chosen = TryReserveRandomShelfFromPool(pool);
            if (chosen == null)
            {
                // Couldn’t reserve anything; fallback
                if (grabbedObjectTypeList.Count == 0)
                    LeaveShop();
                else
                    state = State.WaitingForCheckoutToBeFree;
                return;
            }

            SetTargetShelf(chosen);
        }

        private Shelf TryReserveRandomShelfFromPool(List<Shelf> pool)
        {
            if (pool.Count == 0)
                return null;

            // Randomly probe shelves until one reserves (removing those that fail)
            var temp = new List<Shelf>(pool);
            while (temp.Count > 0)
            {
                int idx = Random.Range(0, temp.Count);
                Shelf candidate = temp[idx];
                temp.RemoveAt(idx);

                if (candidate.TryReserve(this))
                    return candidate;
            }
            return null;
        }

        private void SetTargetShelf(Shelf shelf)
        {
            this.shelf = shelf;
            visitedShelves.Add(shelf);
            state = State.GoingToShelf;
            SetDestination(shelf.GetInteractFromPosition(), shelfStoppingDistance);
        }

        public void LeaveShop()
        {
            // If we were occupying or had reserved a shelf, release it
            if (shelf != null)
            {
                shelf.EndShopping(this);
                shelf.CancelReservation(this);
            }

            state = State.Leaving;
            SetDestination(CustomerManager.Instance.GetCustomerLeavePosition(), leaveStoppingDistance);
        }

        private void DestroySelf()
        {
            instanceList.Remove(this);

            if (shelf != null)
            {
                shelf.EndShopping(this);
                shelf.CancelReservation(this);
            }

            if (agent != null && agent.isOnNavMesh)
                agent.ResetPath();

            Destroy(gameObject);
        }

        private void SetDestination(Vector3 worldPos, float stoppingDistance)
        {
            if (agent == null || !agent.isOnNavMesh)
                return;

            agent.stoppingDistance = stoppingDistance;
            agent.isStopped = false;

            if (NavMesh.SamplePosition(worldPos, out NavMeshHit hit, 1.5f, NavMesh.AllAreas))
                agent.SetDestination(hit.position);
            else
                agent.SetDestination(worldPos);
        }

        private bool ReachedDestination()
        {
            if (agent == null)
                return true;
            if (agent.pathPending)
                return false;

            if (agent.remainingDistance <= agent.stoppingDistance + 0.02f)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude < 0.001f)
                    return true;
            }
            return false;
        }

        private void HandleRotation()
        {
            if (agent == null || agent.isStopped)
                return;

            Vector3 desired = agent.desiredVelocity;
            desired.y = 0f;

            if (desired.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(desired, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationLerpSpeed * Time.deltaTime);
            }
        }

        private void FacePointOnPlane(Vector3 worldPoint)
        {
            Vector3 to = worldPoint - transform.position;
            to.y = 0f;
            if (to.sqrMagnitude > 0.0001f)
            {
                Quaternion target = Quaternion.LookRotation(to, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationLerpSpeed * Time.deltaTime);
            }
        }
        private IEnumerator WaitAndLeaveShop(float waitSeconds)
        {
            yield return new WaitForSeconds(waitSeconds);
            LeaveShop();
        }
    }
}
