using UnityEngine;

[ExecuteInEditMode]
public class WindCollisionBox : MonoBehaviour
{
	public enum Type
	{
		Wind,
		Leaves,
		Underflow
	}

	[Header("Framework")]
	public float Power;

	public float FloatHeight;

	public Type Particle;

	[Header("Prefab")]
	public BoxCollider Collider;

	public ParticleSystem[] Particles;

	[Header("Editor References")]
	public GameObject PreviewModel;

	private void Update()
    {
		switch (Particle)
		{
			case Type.Wind:
				Particles[1].gameObject.SetActive(value: false);
				Particles[2].gameObject.SetActive(value: false);
				break;
			case Type.Leaves:
				Particles[2].gameObject.SetActive(value: false);
				break;
			case Type.Underflow:
				Particles[1].gameObject.SetActive(value: false);
				break;
		}
		for (int i = 0; i < Particles.Length; i++)
		{
			ParticleSystem.ShapeModule shape = Particles[i].shape;
			shape.position = Vector3.up * Collider.size.y * 0.5f;
			shape.scale = new Vector3(Collider.size.x, Collider.size.y, Collider.size.z);
		}
		PreviewModel.transform.localScale = Collider.size;
		transform.localScale = new Vector3(1f, 1f, 1f);
	}

}
