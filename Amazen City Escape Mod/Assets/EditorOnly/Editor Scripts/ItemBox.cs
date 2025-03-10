using Sirenix.OdinInspector;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
	public enum Type
	{
		Default,
		Rings5,
		Rings10,
		Rings20,
		Life,
		SpeedUp,
		GaugeUp,
		Invincible,
		Shield
	}

    [Space]
    [Title("Static title")]
    public const string MyTitle = "Item Box Properties";
    private string MySubtitle = "";
    [Space]
    [Title("$MyTitle", "$MySubtitle", TitleAlignments.Centered)]
    public Type ItemBoxType;

    public enum TypeModel
    {
        Air,
        Ground,
    }
    [Space][Header("Item Box Model (Air/Ground)")]
    public TypeModel ItemBoxModel;

    [HideInInspector] public Renderer BoxRenderer;

	[HideInInspector] public Texture2D[] BoxTextures;

    [HideInInspector] public Texture2D[] LifeBoxTextures;

    [HideInInspector] public GameObject[] MeshType;
    [HideInInspector] public GameObject[] DisplayType;

    private void OnValidate()
    {

        if (ItemBoxModel == TypeModel.Air)
        {
            MeshType[0].SetActive(true);
            MeshType[1].SetActive(false);
            MeshType[2].transform.localPosition = new Vector3(0f, 0.7280273f, 0f);

            switch (ItemBoxType)
            {
                case Type.Default:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings5:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings10:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings20:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Life:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.SpeedUp:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.GaugeUp:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Invincible:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Shield:
                    gameObject.name = "ItemBoxAirSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
            }
        }
        if (ItemBoxModel == TypeModel.Ground)
        {
            MeshType[0].SetActive(false);
            MeshType[1].SetActive(true);
            MeshType[2].transform.localPosition = new Vector3(0f, 0.84f, 0f);
            switch (ItemBoxType)
            {
                case Type.Default:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings5:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings10:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Rings20:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Life:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.SpeedUp:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.GaugeUp:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Invincible:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
                case Type.Shield:
                    gameObject.name = "ItemBoxGroundSpawner/" + (int)ItemBoxType;
                    DisableAllDisplay();
                    break;
            }
        }
    }
    void DisableAllDisplay()
    {
        for (int i = 0; i < DisplayType.Length; i++)
        {
            DisplayType[i].SetActive(false);
            DisplayType[(int)ItemBoxType].SetActive(true);
        }
    }
}	

