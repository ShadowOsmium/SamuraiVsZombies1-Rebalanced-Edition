using System.Collections.Generic;
using UnityEngine;

public class GroundShockAttack : MonoBehaviour
{
	private GameRange mDamageRange = new GameRange(0f, 0f);

	private float mDamage;

	private void Start()
	{
		float num = float.Parse(Singleton<AbilitiesDatabase>.instance.GetAttribute("GroundShock", "damageRadius"));
		mDamageRange.left = base.transform.position.z - num;
		mDamageRange.right = mDamageRange.left + num * 2f;
		mDamage = Singleton<AbilitiesDatabase>.instance.Extrapolate<float>("GroundShock", "infiniteUpgradeDamage", "damage");
	}

	private void DeliverAttack()
	{
		List<Character> charactersInRange = WeakGlobalInstance<CharactersManager>.instance.GetCharactersInRange(mDamageRange, true);
		foreach (Character item in charactersInRange)
		{
			if (item != null && item.health > 0f)
			{
				item.RecievedAttack(EAttackType.Stomp, mDamage);
			}
		}
		charactersInRange = WeakGlobalInstance<CharactersManager>.instance.GetCharactersInRange(mDamageRange, false);
		foreach (Character item2 in charactersInRange)
		{
			if (item2 != null && item2.health > 0f)
			{
				item2.RecievedAttack(EAttackType.Stomp, mDamage);
			}
		}
	}
}
