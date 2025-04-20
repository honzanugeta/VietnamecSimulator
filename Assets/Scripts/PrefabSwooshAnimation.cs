using System.Collections;
using UnityEngine;

public class PrefabSwooshAnimation : MonoBehaviour
{
    public GameObject prefab;
    public Transform playerTransform;
    public float hiddenDistance = 2.0f; // Distance behind the player
    public float visibleDistance = 1.0f; // Distance in front of the player
    public float animationDuration = 1.0f; // Duration of the animation

    private Vector3 hiddenPosition;
    private Vector3 visiblePosition;
    private GameObject currentInstance;


    private void Start()
    {
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
        Debug.Log("PrefabSwooshAnimation initialized.");
    }

    private void UpdatePositions()
    {
        hiddenPosition = playerTransform.position - playerTransform.forward * hiddenDistance;
        visiblePosition = playerTransform.position + playerTransform.forward * visibleDistance;
    }

    public void AnimatePrefab(bool show)
    {
        UpdatePositions();
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            Debug.Log("Previous prefab instance destroyed.");
        }
        currentInstance = Instantiate(prefab, hiddenPosition, Quaternion.identity);
        Debug.Log("Prefab instantiated at hidden position: " + hiddenPosition);
        StartCoroutine(AnimatePrefabCoroutine(currentInstance, show));
    }

    private IEnumerator AnimatePrefabCoroutine(GameObject instance, bool show)
    {
        float time = 0;
        Vector3 startPosition = show ? hiddenPosition : visiblePosition;
        Vector3 endPosition = show ? visiblePosition : hiddenPosition;
        Debug.Log("Animating prefab from " + startPosition + " to " + endPosition);

        while (time < animationDuration)
        {
            time += Time.deltaTime;
            instance.transform.position = Vector3.Lerp(startPosition, endPosition, time / animationDuration);
            yield return null;
        }
        instance.transform.position = endPosition;
        Debug.Log("Animation completed. Prefab position: " + endPosition);
        
        if (!show)
        {
            Destroy(instance);
            Debug.Log("Prefab destroyed.");
        }
    }
}