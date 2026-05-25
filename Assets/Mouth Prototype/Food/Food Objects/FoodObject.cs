using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;


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

	[Header("RigidBody Physics")]
	public Rigidbody Rigidbody
	{
		get => _rigidbody;
		set => _rigidbody = value;
	}
	
	[Header("Gravity")]
	[field:SerializeField] private Vector3 _direction = Vector3.up;
	[Range(-5f, 5f) ]
	[field:SerializeField] private float intensity = 1f;

	// Stats
	[field: SerializeField] private FoodStats stats;
	public FoodStats Stats {
		get => stats;
		
	}
	private List<Effect> _foodEffectsActive = new List<Effect>();
	
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
	[field: SerializeField] internal UnityEvent onGrounded;
	internal UnityEvent onGrab;
	internal UnityEvent onEaten;
	internal UnityEvent onDropped;
	internal UnityEvent onAir;
	

	void Awake()
	{
		RegisterFoodEffectsFromStats();
	}

	

	private void Start()
	{
		_rigidbody = GetComponent<Rigidbody>();
		_rigidbody.freezeRotation  = true;
		_collider = GetComponent<Collider>();
		_foodObjState = FOOD_OBJ_STATE.GROUNDED;

		_grabCounter = 0; 
		_dropCounter = 0;
		_biteCounter = 0;
		
	}

	private void Update()
	{
		//HandleFoodEffectStack();
	}

	private void HandleFoodEffectStack()
	{
		if(this._foodEffectsActive.Count <= 0) return;
		foreach (Effect effect in _foodEffectsActive )
			effect.Tick(Time.deltaTime);
	}


	private void FixedUpdate()
	{
		if(_rigidbody.position.y is < -10 or > 10 ) Destroy(this.gameObject);
		
		if (!_rigidbody.useGravity)
		{
			var targetPosition = _direction;
			_rigidbody.AddForce(_direction * intensity, ForceMode.VelocityChange);
		}

		
		
		HandleFoodObjectState();
	}

	#region RigidBody_Handling
	
	private void HandleFoodObjectState()
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

	private void Float(bool active)
	{
		
	}


	#endregion
	
	private void RegisterFoodEffectsFromStats()
	{
		// get all effects to have a refernce instance of food object and food stats
		if(stats == null)return;
		foreach (Effect effect in stats.effects)
		{
			switch (effect.stateToActivateFoodEffect)
			{
				case Effect.FOOD_EFFECT_ACTIVE.GROUND:
					effect.Intialize(this, this.onGrounded);
					break;
				case Effect.FOOD_EFFECT_ACTIVE.GRABBED:
					effect.Intialize(this, this.onGrab);
					break;
				case Effect.FOOD_EFFECT_ACTIVE.EATEN:
					effect.Intialize(this, this.onEaten);
					break;
				case Effect.FOOD_EFFECT_ACTIVE.DROPPED:
					effect.Intialize(this, this.onDropped);
					break;
				case Effect.FOOD_EFFECT_ACTIVE.AIR:
					effect.Intialize(this, this.onAir);
					break;
				default:
					effect.Intialize(this, this.onGrounded);
					break;
			}
			this._foodEffectsActive.Add(effect);
		}
	}
	

	#region Food Object State
	
	public void UpdateFoodObjectState(FOOD_OBJ_STATE newFoodObjectState)
	{
		if(newFoodObjectState == this._foodObjState) return;
		this._foodObjState = newFoodObjectState;
	
		switch (newFoodObjectState)
		{
			case FOOD_OBJ_STATE.GROUNDED:
				this.onGrounded?.Invoke();
				break;
			case FOOD_OBJ_STATE.GRABBED:
				this.onGrab?.Invoke();
				this._grabCounter++;
				break;
			case FOOD_OBJ_STATE.DROPPED :
				this.onDropped?.Invoke();
				this._dropCounter++;
				break;
			case FOOD_OBJ_STATE.AIR:
				this.onAir?.Invoke();
				break;
			case FOOD_OBJ_STATE.EATEN:
				this.onEaten?.Invoke();
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
		if(!debugMode) return;
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
		GUI.Label(new Rect(_debugGuIposition.x, _debugGuIposition.y + (12 * 10), _debugGuIsize.x, _debugGuIsize.y), $"Effect Details: {this._foodEffectsActive}", _debugGUIStyle); // Effect Detaails
		

	}
	#endregion


	private void OnDestroy()
	{
		onGrounded.RemoveAllListeners();
		// onEaten.RemoveAllListeners();
		// onGrab.RemoveAllListeners();
		// onAir.RemoveAllListeners();
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
