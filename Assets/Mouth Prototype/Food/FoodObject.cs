using UnityEngine;
using System.Threading.Tasks;
using System.Collections;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour, IInteractable, IEdible
{
    private HandPickUp interactor;
    private Rigidbody rb;

    public Rigidbody Rb => rb;

    [SerializeField]
    private AudioSource eatSFX = null;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Interact(HandPickUp interactor)
    {
        this.interactor = interactor;
    }




    public void Eaten()
    {
        StartCoroutine(HandleEating());
    }

    IEnumerator HandleEating()
    {
        eatSFX.Play();
        yield return new WaitForSeconds(1);
        Destroy(this);
    }
}
