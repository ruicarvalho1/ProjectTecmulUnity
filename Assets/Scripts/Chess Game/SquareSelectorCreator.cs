using System;
using System.Collections.Generic;
using UnityEngine;

public class SquareSelectorCreator : MonoBehaviour
{
    [SerializeField] private Material freeSquareMaterial;
    [SerializeField] private Material opponentSquareMaterial;
    [SerializeField] private GameObject selectorPrefab;
    private List<GameObject> instantiatedSelectors = new List<GameObject>();

    public void ShowSelection(Dictionary<Vector3, bool> squareData){
        ClearSelection();
        foreach(var square in squareData)
        {
            GameObject selector = Instantiate(selectorPrefab, square.Key, Quaternion.identity);
            instantiatedSelectors.Add(selector);
            foreach(var setter in selector.GetComponentsInChildren<MaterialSetter>())
            {
                setter.SetSingleMaterial(square.Value ? freeSquareMaterial : opponentSquareMaterial);
            }
        }
    }

    public void ClearSelection()
    {
        foreach (var selector in instantiatedSelectors)
        {
            Destroy(selector.gameObject);
        }
        instantiatedSelectors.Clear();
    }

}
