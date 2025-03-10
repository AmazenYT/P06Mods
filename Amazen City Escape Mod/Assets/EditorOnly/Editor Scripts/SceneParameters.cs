using System.Globalization;
using System.Threading;
using UnityEngine;

public class SceneParameters : MonoBehaviour
{
	public enum Setting
	{
		Normal,
		FoggyNoise,
		Thick
	}

	[Header("Framework")]
	public TextAsset SceneFile;

	public Flare LensFlare;

	public float BloomThreshold;

	public float BloomMaxThreshold;

	public float BloomSpread;

	public bool UseShadows = true;

	public bool UseVolumetricLight;

	public Setting VolumetricSetting;

	public Cubemap StageCubemap;

	public LightPreset[] LightPresets;

	public LightAnimationPreset[] LightAnimationPresets;

	private LensFlare MainLensFlare;

    private VolumetricLight MainVLight;

	private Light LightMain;

	private Light LightSub;

	private Color LightMainColor;

	private Color LightSubColor;

	private Color AmbientColor;

	private Vector3 LightMainDir;

	private Vector3 LightSubDir;

	private float AmbientIntensity;

	internal bool LightChange;

	internal bool LightAnimationChange;

	private Light MainLightComp;

	private Light SubLightComp;

	private string MainLight;

	private string SubLight;

	private string Ambient;

	private Color MainChangeColor;

	private float MainChangeInt;

	private Color SubChangeColor;

	private float SubChangeInt;

	private Color AmbChangeColor;

	private float AmbChangeMult;

	private string AnimMainLight;

	private string AnimSubLight;

	private string AnimAmbient;

	private Gradient MainAnimColor;

	private float MainAnimSpeed;

	private Gradient SubAnimColor;

	private float SubAnimSpeed;

	private Gradient AmbAnimColor;

	private float AmbAnimSpeed;

	private Color OrigMainColor;

	private float OrigMainInt;

	private Color OrigSubColor;

	private float OrigSubInt;

	private Color OrigAmbColor;

	private Vector3 MainChangeDir;

	private Vector3 SubChangeDir;

	private Vector3 OrigMainDir;

	private Vector3 OrigSubDir;


}
