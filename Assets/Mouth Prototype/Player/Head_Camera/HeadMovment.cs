using System;
using System.IO.Compression;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class HeadMovment : MonoBehaviour
{
	private Vector3 input;
	public Rigidbody rb;
	public float speed = .5f;
	public float clampValue = 3f;

	void Awake()
	{

	}
	void Start()
	{
		rb = GetComponent<Rigidbody>();

	}

	// Update is called once per frame
	void Update()
	{
		// input = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal"));
		input = new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y"));

		if (input != Vector3.zero)
		{
			float x_axis = Mathf.Clamp(input.x, -clampValue, clampValue);
			float z_axis = Mathf.Clamp(input.z, -clampValue, clampValue);
			var new_pos = new Vector3(x_axis, 0, z_axis) * speed * Time.deltaTime;

			transform.Translate(new_pos);
		}

	}

	void FixedUpdate()
	{
		// HeadSway();
	}

	private void HeadSway()
	{

		// if (input == Vector3.zero) return;
		var next_pos = transform.position + input.normalized * speed * Time.fixedDeltaTime;
		Debug.Log($"{next_pos}");
		rb.MovePosition(next_pos);



	}
}
