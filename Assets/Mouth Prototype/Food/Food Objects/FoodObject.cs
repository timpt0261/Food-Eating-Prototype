using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	
	[Header("Internal Components")]
	private HandPickUp _interactor;
	private Rigidbody _rigidbody;
	private Collider _collider;
	[field: SerializeField] 
	private FoodStats stats;
	// Stats

	// Mesh

	// SFX

	// Effects when PickedUp

	// Effects when Eating
	
	// Collision
	
	private bool _isFoodObjectTouchingSurface = false;
	private bool _isPlayerEatingObject = false;
	
	// Debug Mode
	[SerializeField] private bool debugMode = false;
	private readonly GUIStyle _debugGUIStyle = new GUIStyle();
	private  readonly Vector2 _debugGuIposition = new Vector3(10,10);
	private readonly  Vector2 _debugGuIsize = new Vector2(200, 50);
	
	
	// grab state, eaten state , dropped state

	
	private FOOD_OBJ_STATE _foodObjState = FOOD_OBJ_STATE.GROUNDED;

	public static event Action OnGrab;
	public static event Action OnEaten;
	public static event Action OnDropped;

	void Awake()
	{
		
		// if stats has more than one effect -> for each effect in stats enable
		// if(stats.effects.Count <= 0)
		// 	return;
		// foreach (FoodEffect effect in stats.effects)
		// {
		// 	switch (effect.activation)
		// 	{
		// 		case  FoodEffectActivation.ONGRAB:
		// 			// subscribe
		// 			break;
		// 		case FoodEffectActivation.ONEATEN :
		// 			break;
		// 		case FoodEffectActivation.ONDROPPED:
		// 			break;
		// 		default:
		// 			break;
		// 	}
		// }
	}

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		_foodObjState = FOOD_OBJ_STATE.GROUNDED;
	}
	

	private void FixedUpdate()
	{
		
		// is grounded if on table surface, _interactor == null, and velocity == 0  
		if (this._interactor)
		{
			UpdateState(FOOD_OBJ_STATE.GRABBED); 
			return;
		}
		
		if (_isFoodObjectTouchingSurface)
		{
			UpdateState(FOOD_OBJ_STATE.GROUNDED); 
			return;
		}

		if (this._rigidbody.useGravity)
		{
			UpdateState(FOOD_OBJ_STATE.DROPPED); 
			return;
		}
		
		UpdateState(FOOD_OBJ_STATE.AIR);
		
			
	}

	public void SetInteractor(HandPickUp interactor)
	{
		this._interactor = interactor;
	}

	public void UpdateState(FOOD_OBJ_STATE newState)
	{
		if(newState == this._foodObjState) return;
		this._foodObjState = newState;
	}

	#region Collision Handling

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "Surface" &&  !_isFoodObjectTouchingSurface)
		{
			_isFoodObjectTouchingSurface = true;
		} 
		
		
		if(collision.gameObject.name == "Mouth" &&  !_isPlayerEatingObject)
		{
			_isPlayerEatingObject = true;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.tag == "Surface" && _isFoodObjectTouchingSurface)
		{
			_isFoodObjectTouchingSurface = false;
		}
	}


	#endregion

	#region Debug GUI
	
	[ExecuteInEditMode] 
	private void OnGUI()
	{
		_debugGUIStyle.fontSize = 12;
		_debugGUIStyle.normal.textColor = Color.green;
		float offset = _debugGUIStyle.fontSize;
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y, _debugGuIsize.x, _debugGuIsize.y), $" Postion : {this._rigidbody.position}", _debugGUIStyle); // Position
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 1), _debugGuIsize.x, _debugGuIsize.y), $"Rotation : {this._rigidbody.rotation}", _debugGUIStyle); // Rotation
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 2), _debugGuIsize.x, _debugGuIsize.y), $"Scale : {this.transform.localScale}", _debugGUIStyle); // Scale
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 3), _debugGuIsize.x, _debugGuIsize.y), $"Mass: {this._rigidbody.mass}", _debugGUIStyle); // Mass
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 4), _debugGuIsize.x, _debugGuIsize.y), $"Velocity: {this._rigidbody.angularVelocity}", _debugGUIStyle); // Scale
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 5), _debugGuIsize.x, _debugGuIsize.y), $"Current State: {this._foodObjState}", _debugGUIStyle); // State
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 6), _debugGuIsize.x, _debugGuIsize.y), $"Interactor: {this._interactor}", _debugGUIStyle); // Interactor

	}
	#endregion

}

public enum FOOD_OBJ_STATE
{
	GROUNDED,
	GRABBED,
	DROPPED,
	AIR, 
	EATEN,
		
}
