using UnityEngine;
using CodeMonkey.Toolkit.TChatBubble3D;
using CodeMonkey.Toolkit.TLookAtCamera;
using CodeMonkey.Toolkit.TRandomData;
using System.Collections.Generic;

public class Zakaznik : MonoBehaviour
{
    private static List<Zakaznik> instanceList = new List<Zakaznik>();


        public static List<Zakaznik> GetInstanceList() {
            return instanceList;
        }


        private enum State {
            GoingToShelf,
            GrabbingItem,
            WaitingForCheckoutToBeFree,
            GoingToCheckout,
            WaitingToPay,
            Leaving,
        }


        private ShelfScript shelf;
        private State state;
        private float timer;
        private Objekttyp grabbedObjectType;


        private void Awake() {
            instanceList.Add(this);
        }

        private void Start() {
            GoToRandomShelf();
        }

        private void GoToRandomShelf() {
            ShelfScript shelf = ShelfScript.instanceList.GetRandomElement();
            SetTargetShelf(shelf);
        }

        private void SetTargetShelf(ShelfScript shelf) {
            this.shelf = shelf;
            state = State.GoingToShelf;
        }

        private void Update() {
            switch (state) {
                default:
                case State.GoingToShelf: 
                    {
                        Vector3 moveDir = (shelf.GetInteractFromPosition() - transform.position).normalized;
                        float moveSpeed = 5f;
                        transform.position += moveDir * Time.deltaTime * moveSpeed;
                        float reachedPositionDistance = .5f;
                        if (Vector3.Distance(transform.position, shelf.GetInteractFromPosition()) < reachedPositionDistance) {
                            state = State.GrabbingItem;
                            timer = Random.Range(1f, 3f);
                        }
                    }
                    break;

                case State.GrabbingItem:
                    timer -= Time.deltaTime;
                    if (timer <= 0f) {
                        Objekttyp grabbedObjectType = shelf.TryGrabRandomItem();
                        if (grabbedObjectType != Objekttyp.None) {
                            // Grabbed something
                            state = State.WaitingForCheckoutToBeFree;
                            this.grabbedObjectType = grabbedObjectType;
                            string[] messageArray = new string[] {
                                "Oh this looks good!",
                                "I like this",
                                "I'll have this",
                                "Yup this is what I need",
                                "Let me grab this"
                            };
                            ChatBubble3D.Create(transform, new Vector3(0, 2f, 0), ChatBubble3D.IconType.Happy, messageArray.GetRandomElement(), .07f, 2f).
                                transform.AddLookAtCamera(CodeMonkey.Toolkit.TLookAtCamera.LookAtCamera.Method.LookAtInverted);
                        } else {
                            // Could not grab anything, shelf is empty
                            GoToRandomShelf();
                        }
                    }
                    break;

                case State.WaitingForCheckoutToBeFree:
                    if (Checkoutscript.Instance.IsFree()) {
                        Checkoutscript.Instance.SetCustomer(this);
                        state = State.GoingToCheckout;
                    }
                    break;

                case State.GoingToCheckout: 
                    {
                        Vector3 moveDir = (Checkoutscript.Instance.GetCustomerPosition() - transform.position).normalized;
                        float moveSpeed = 5f;
                        transform.position += moveDir * Time.deltaTime * moveSpeed;
                        float reachedPositionDistance = .5f;
                        if (Vector3.Distance(transform.position, Checkoutscript.Instance.GetCustomerPosition()) < reachedPositionDistance) {
                            Checkoutscript.Instance.AddObject(grabbedObjectType);
                            state = State.WaitingToPay;
                        }
                    }
                    break;

                case State.WaitingToPay:
                    break;

                case State.Leaving: 
                    {
                        Vector3 moveDir = (Zakaznikmanazer.Instance.GetCustomerLeavePosition() - transform.position).normalized;
                        float moveSpeed = 5f;
                        transform.position += moveDir * Time.deltaTime * moveSpeed;
                        float reachedPositionDistance = .5f;
                        if (Vector3.Distance(transform.position, Zakaznikmanazer.Instance.GetCustomerLeavePosition()) < reachedPositionDistance) {
                            DestroySelf();
                        }
                    }
                    break;
            }
        }

        public void LeaveShop() {
            state = State.Leaving;
        }

        private void DestroySelf() {
            instanceList.Remove(this);
            Destroy(gameObject);
        }

    }

