using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxSpawner : MonoBehaviour
{
    [Space][Header("Choose a box Type")]
    public BoxSpawner.Type BoxType;
    [Space]
    [HideInInspector]public GameObject[] BoxReferences;
    public enum Type
    {
        WoodBox,
        ReinforcedWoodBox,
        FlashBox,
        BombBox,
        IronBox
    }

    private void OnValidate()
    {
        switch (BoxType)
        {
            case (Type)4:
                DisableAllBoxes();
                BoxReferences[4].SetActive(true);
                BoxReferences[4].gameObject.tag = "Untagged";
                break;
            case (Type)3:
                DisableAllBoxes();
                BoxReferences[3].SetActive(true);
                BoxReferences[3].gameObject.tag = "Untagged";
                break;
            case (Type)2:
                DisableAllBoxes();
                BoxReferences[2].SetActive(true);
                BoxReferences[2].gameObject.tag = "Untagged";
                break;
            case (Type)1:
                DisableAllBoxes();
                BoxReferences[1].SetActive(true);
                BoxReferences[1].gameObject.tag = "Untagged";
                break;
            case (Type)0:
                DisableAllBoxes();
                BoxReferences[0].SetActive(true);
                BoxReferences[0].gameObject.tag = "Untagged";
                break;
            default:
                DisableAllBoxes();
                BoxReferences[0].SetActive(true);
                BoxReferences[0].gameObject.tag = "Untagged";
                break;
        }
    }


    void DisableAllBoxes()
    {
        for (int i = 0; i < BoxReferences.Length; i++)
        {
            BoxReferences[i].SetActive(false);
            foreach (Transform child in transform)
            {
                child.gameObject.tag = "EditorOnly";
            }
        }
    }
}
