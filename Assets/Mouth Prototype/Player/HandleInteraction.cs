using UnityEngine;

public class HandleInteraction : MonoBehaviour
{

	[SerializeField]
	private bool b_isInteracting;
	public bool IsInteracting { get => b_isInteracting; set => value = b_isInteracting; }

	[SerializeField]
	private GameObject go_interacting;
	[SerializeField]
	private Rigidbody rb_intractable;

	public float interactionRadius;

	public LayerMask intractableLayer;

	void Awake()
	{
		b_isInteracting = false;
		interactionRadius = GetComponent<SphereCollider>().radius;
	}
	void OnTriggerEnter(Collider other)
	{

		 if (((1 << other.gameObject.layer) & intractableLayer) != 0 && other.CompareTag("FoodItems"))
		{
			Debug.Log($"{other.gameObject.name} Entering");

			if (go_interacting != null || b_isInteracting == true) return;

			b_isInteracting = true;
			go_interacting = other.gameObject;
			go_interacting.transform.parent = this.transform;

			go_interacting.TryGetComponent<Rigidbody>(out Rigidbody rb);
			if (rb == null)
			{
				return;
			}
			rb_intractable = rb;
			rb_intractable.useGravity = false;
			rb_intractable.linearDamping = 10;
			rb_intractable.constraints = RigidbodyConstraints.FreezeRotation;

		}

	}

	void OnTriggerStay(Collider other)
	{
		// Debug.Log($"{other.gameObject.name} Stay");
	}

	void OnTriggerExit(Collider other)
	{

		if (((1 << other.gameObject.layer) & intractableLayer) != 0 && other.CompareTag("FoodItems"))
		{
			Debug.Log($"{other.gameObject.name} Exit");
			if (other.gameObject != go_interacting.gameObject) return;
			b_isInteracting = false;
			rb_intractable.useGravity = true;
			rb_intractable.linearDamping = 1;
			rb_intractable.constraints = RigidbodyConstraints.None;
			go_interacting.transform.parent = null;


		}
	}

	private void DrawGizmo()
	{
		Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRadius);

	}
}
