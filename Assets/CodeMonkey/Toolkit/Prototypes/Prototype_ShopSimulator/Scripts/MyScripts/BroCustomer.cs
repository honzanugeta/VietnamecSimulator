using System.Collections.Generic;
using UnityEngine;

public class BroCustomer : MonoBehaviour
{
    [SerializeField] private List<GameObject> broModels = new List<GameObject>();

    private void Start()
    {
        SelectRandomModel();
    }

    private void SelectRandomModel()
    {
        foreach (GameObject broModel in broModels)
        {
            broModel.SetActive(false);
        }
        int randomIndex = Random.Range(0, broModels.Count);
        broModels[randomIndex].SetActive(true);
    }
}
