using System.Collections.Generic;
using UnityEngine;

public class GiantWave : MonoBehaviour
{
	private const float kDamageFrequency = 0.2f;

	private const float kDamageRange = 200f;

	private float mSpeed;

	private float mTimeUntilNextDamage;

	private float mDamageRemaining;

	private float mEndZ;

	private string mAbilityID = string.Empty;

	private void Start()
	{
		mAbilityID = Singleton<PlayModesManager>.instance.selectedModeData["giantWaveID"];
		mSpeed = float.Parse(Singleton<AbilitiesDatabase>.instance.GetAttribute(mAbilityID, "speed"));
		mDamageRemaining = Singleton<AbilitiesDatabase>.instance.Extrapolate<float>(mAbilityID, "infiniteUpgradeDamage", "damage");
		if (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.RightToLeft)
		{
			mEndZ = WeakGlobalSceneBehavior<InGameImpl>.instance.hero.controller.constraintLeft;
		}
		else
		{
			mEndZ = WeakGlobalSceneBehavior<InGameImpl>.instance.hero.controller.constraintRight;
		}
	}

	private void Update()
	{
		Vector3 position = base.transform.position;
		float num = mSpeed * Time.deltaTime;
		if (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.RightToLeft)
		{
			num = 0f - num;
		}
		position.z += num;
		mTimeUntilNextDamage -= Time.deltaTime;
		if (mTimeUntilNextDamage <= 0f)
		{
			mTimeUntilNextDamage = 0.2f;
			List<Character> list = ((Singleton<PlayModesManager>.instance.gameDirection != PlayModesManager.GameDirection.RightToLeft) ? WeakGlobalInstance<CharactersManager>.instance.GetCharactersInRange(position.z, position.z + 200f, true) : WeakGlobalInstance<CharactersManager>.instance.GetCharactersInRange(position.z - 200f, position.z, true));
			foreach (Character item in list)
			{
				if (item != null && mDamageRemaining > 0f && item.health > 0f)
				{
					float health = item.health;
					item.RecievedAttack(EAttackType.Trample, mDamageRemaining);
					mDamageRemaining -= health;
				}
			}
		}
		bool flag = (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.LeftToRight && position.z >= mEndZ) || (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.RightToLeft && position.z <= mEndZ);
		if (mDamageRemaining <= 0f || flag)
		{
			if (Singleton<PlayModesManager>.instance.gameDirection == PlayModesManager.GameDirection.RightToLeft)
			{
				position.z = Mathf.Max(position.z, mEndZ);
			}
			else
			{
				position.z = Mathf.Min(position.z, mEndZ);
			}
			position.y = WeakGlobalInstance<RailManager>.instance.GetY(position.z);
			base.transform.position = position;
			string attribute = Singleton<AbilitiesDatabase>.instance.GetAttribute(mAbilityID, "fxDie");
			if (attribute != string.Empty)
			{
				Object.Instantiate(Resources.Load(attribute) as GameObject, base.transform.position, base.transform.rotation);
			}
			Object.Destroy(base.gameObject);
		}
		else
		{
			position.y = WeakGlobalInstance<RailManager>.instance.GetY(position.z);
			base.transform.position = position;
		}
	}
}
