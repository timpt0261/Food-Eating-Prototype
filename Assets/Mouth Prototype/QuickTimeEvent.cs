using UnityEngine;
using UnityEngine.UI;
public class QuickTimeEvent : MonoBehaviour
{

    [SerializeField] private Slider qte_slider;
    [SerializeField] private float speed = 1f;

    [SerializeField] private KeyCode key = KeyCode.E;
    [SerializeField] private bool pressedOnFrame;

    

    // Update is called once per frame
    void Update()
    {


        pressedOnFrame = Input.GetKey(key);
        // Debug.Log($"{pressedOnFrame}");
    }

    private void ActivateQTE()
    {
        qte_slider.value = 0;
    }


    private void UpdateQTE()
    {
        qte_slider.value = Mathf.PingPong(Time.time * speed, qte_slider.maxValue);

    }
}
