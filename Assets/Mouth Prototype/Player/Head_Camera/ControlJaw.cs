using UnityEngine;
using UnityEngine.Rendering;

public class ControlJaw : MonoBehaviour
{

    public float scrollSenstivity = 100f;
    private float min_clamp = -90f;
    private float max_clamp = 90f;

    public HingeJoint hingeJoint;
    private JointSpring spring;

    private Vector2 mouseScroll;
    void Start()
    {
        hingeJoint = GetComponentInChildren<HingeJoint>();
        spring = hingeJoint.spring;

    }

    // Update is called once per frame
    void Update()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        // Debug.Log($"Mouse Scroll:  {mouseScroll}");
        if (Mathf.Abs(mouseScroll) > 0)
        {
            spring.targetPosition -= mouseScroll * scrollSenstivity;
            spring.targetPosition = Mathf.Clamp(spring.targetPosition, min_clamp, max_clamp);

            hingeJoint.spring = spring;
        }

    }
}
