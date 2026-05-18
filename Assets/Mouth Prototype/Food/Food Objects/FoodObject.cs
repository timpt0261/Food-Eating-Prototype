using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;


[RequireComponent(typeof(Rigidbody))]
public class FoodObject : MonoBehaviour
{
	[Header("Internal Components")]
	private HandPickUp _interactor;
	private Rigidbody _rigidbody;
	private Collider _collider;
	
	
	public HandPickUp Interactor { 
		get => _interactor;
		set => _interactor = value;
	}
	
	
	// Stats
	[field: SerializeField] private FoodStats stats;
	public FoodStats Stats {
		get => stats;
		
	}

	// Mesh

	// SFX

	// Effects when PickedUp
	[field: SerializeField] private List<FoodEffect> groundEffects;
	// [field: SerializeField] private List<FoodEffect> grabEffects;
	// [field: SerializeField] private List<FoodEffect> dropEffects;
	// [field: SerializeField] private List<FoodEffect> eatEffects;

	// Effects when Eating
	
	// Collision
	private int _grabCounter = 0;
	private int _dropCounter = 0;
	private int _biteCounter = 0;

	private bool _isFoodObjectTouchingSurface = false;
	private bool _isPlayerEatingObject = false;
	
	// Debug Mode
	[field: SerializeField] private bool debugMode = false;
	private readonly GUIStyle _debugGUIStyle = new GUIStyle();
	private  readonly Vector2 _debugGuIposition = new Vector3(10,10);
	private readonly  Vector2 _debugGuIsize = new Vector2(200, 50);
	
	private FOOD_OBJ_STATE _foodObjState = FOOD_OBJ_STATE.GROUNDED;

	[Header ("Event Handling")]
	[field: SerializeField] UnityEvent onGrounded;
	private UnityEvent onGrab;
	private UnityEvent onEaten;
	private UnityEvent onDropped;
	private UnityEvent onAir;
	

	void Awake()
	{
		// get all effects to have a refernce instance of food object and food stats
		foreach (FoodEffect effect in groundEffects)
		{
			effect.Intialize(this);
			onGrounded.AddListener(effect.Activate);
		}
	}

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_collider = GetComponent<Collider>();
		_foodObjState = FOOD_OBJ_STATE.GROUNDED;

		_grabCounter = 0; 
		_dropCounter = 0;
		_biteCounter = 0;
	}
	

	private void FixedUpdate()
	{
		
		if (this._interactor)
		{
			UpdateFoodObjectState(FOOD_OBJ_STATE.GRABBED); 
			return;
		}
		
		if (_isFoodObjectTouchingSurface)
		{
			UpdateFoodObjectState(FOOD_OBJ_STATE.GROUNDED); 
			return;
		}

		if (_isPlayerEatingObject)
		{	
			UpdateFoodObjectState(FOOD_OBJ_STATE.EATEN);
			return;
		}

		if (this._rigidbody.useGravity)
		{
			UpdateFoodObjectState(FOOD_OBJ_STATE.DROPPED); 
			return;
		}
		
		UpdateFoodObjectState(FOOD_OBJ_STATE.AIR);
		
			
	}
	
	

	#region Food Object State
	
	public void UpdateFoodObjectState(FOOD_OBJ_STATE newFoodObjectState)
	{
		if(newFoodObjectState == this._foodObjState) return;
		this._foodObjState = newFoodObjectState;
	
		switch (newFoodObjectState)
		{
			case FOOD_OBJ_STATE.GROUNDED:
				this.onGrounded.Invoke();
				break;
			case FOOD_OBJ_STATE.GRABBED:
			//	this.onGrab.Invoke();
				this._grabCounter++;
				break;
			case FOOD_OBJ_STATE.DROPPED :
			//	this.onDropped.Invoke();
				this._dropCounter++;
				break;
			case FOOD_OBJ_STATE.AIR:
			//	this.onAir.Invoke();
				break;
			case FOOD_OBJ_STATE.EATEN:
			//	this.onEaten.Invoke();
				this._biteCounter++;
				break;
			default:
				this.onGrounded.Invoke();
				break;
		
		}
	}
	#endregion
	
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
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 7), _debugGuIsize.x, _debugGuIsize.y), $"Grab Counter: {this._grabCounter}", _debugGUIStyle); // Grab Counter
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 8), _debugGuIsize.x, _debugGuIsize.y), $"Drop Counter: {this._dropCounter}", _debugGUIStyle); // Drop Counter
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 9), _debugGuIsize.x, _debugGuIsize.y), $"Bite Counter: {this._biteCounter}", _debugGUIStyle); // Bite Counter
		

	}
	#endregion


	private void OnDestroy()
	{
		onGrounded.RemoveAllListeners();
	}
}

public enum FOOD_OBJ_STATE
{
	GROUNDED,
	GRABBED,
	DROPPED,
	AIR, 
	EATEN,
		
}
