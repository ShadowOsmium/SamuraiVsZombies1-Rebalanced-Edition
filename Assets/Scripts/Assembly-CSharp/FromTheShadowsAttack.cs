using UnityEngine;

public class FromTheShadowsAttack : MonoBehaviour
{
	public Character target { get; set; }

	private void DeliverAttack()
	{
		if (target != null)
		{
			target.RecievedAttack(EAttackType.Stealth, (!target.isBoss) ? target.maxHealth : (target.maxHealth * 0.25f));
			base.enabled = false;
		}
	}

	private void Update()
	{
		if (target != null)
		{
			base.transform.position = target.position;
		}
	}
}
