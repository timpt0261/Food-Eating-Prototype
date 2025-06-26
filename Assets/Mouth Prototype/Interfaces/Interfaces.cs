using UnityEngine;

public interface IInteractable
{
	public void Interact(Interactor interactor);

}

public interface IDrinkable
{
	void Drink();
}
public interface IEdible
{
	void Eaten();
}

public interface IDamageable
{
	void Damage(float damage);
}
