using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TaggedAnimPlayer))]
[RequireComponent(typeof(AutoPaperdoll))]
public class CharacterModelController : MonoBehaviour
{
	public enum EPostDeathAction
	{
		vanish = 0,
		melt = 1,
		ascend = 2,
		stay = 3
	}

	[Serializable]
	public class RandomAnimSet
	{
		public string name;

		public AnimationClip[] clips;

		public bool onlyRandomizeOnce;

		[NonSerialized]
		internal int? mLastClipIndexChosen;
	}

	private const float kSpeedDefault = 100f;

	private const float kAimRotationSpeed = 150f;

	private const float kAimResetSpeed = 150f;

	private const float kKnockbackTime = 0.5f;

	private const float kKnockbackSpeed = 200f;

	private const float kKnockbackArcHeight = 32f;

	private const float kSimulatedKnockbackLandTime = 0.25f;

	private const float kMaxBackpedalTime = 1.3f;

	private const float kFacingTowardTurnSpeed = 225f;

	private const float kResetFacingTurnSpeed = 225f;

	public bool attackActive;

	public float animatedWalkSpeed = 100f;

	public RandomAnimSet[] randomAnimSets = new RandomAnimSet[0];

	public GameObject arrowImpactEffect;

	public GameObject bladeImpactEffect;

	public GameObject bladeCriticalImpactEffect;

	public GameObject bluntImpactEffect;

	public GameObject bluntCriticalImpactEffect;

	public bool snapToGround = true;

	public Action onMeleeAttackDelivery;

	public Action onRangedAttackDelivery;

	private Transform mCachedTransform;

	private float mSpeed;

	private float mConstraintLeft;

	private float mConstraintRight;

	private float mSpeedModifier = 1f;

	private float mImpactPauseTime;

	private FacingType mWalkDirection;

	private FacingType mFacing = FacingType.Right;

	private bool mAttackDeliveryTriggered;

	private bool mSnapThisUpdate = true;

	private bool mUseRangedAttackIdle;

	private bool mUseCowerIdle;

	private bool mPlayingHurtAnim;

	private bool mPlayingVictoryAnim;

	private float mStunnedTimer;

	private float mKnockbackTime;

	private TaggedAnimPlayer mAnimPlayer;

	private AutoPaperdoll mAutoPaperdoll;

	private SoundThemePlayer mSoundPlayer;

	private GameObject mProjectileInstance;

	private Action mOnAttackDelivery;

	private EPostDeathAction mPostDeathAction;

	private Dictionary<string, RandomAnimSet> mRandomAnimSets = new Dictionary<string, RandomAnimSet>();

	private string mCurrentAnim = "idle";

	private float? mAimAngle;

	private float? mHeadAngle;

	public AutoPaperdoll autoPaperdoll
	{
		get
		{
			return mAutoPaperdoll;
		}
	}

	public TaggedAnimPlayer animPlayer
	{
		get
		{
			return mAnimPlayer;
		}
	}

	public Vector3 position
	{
		get
		{
			return mCachedTransform.position;
		}
		set
		{
			mCachedTransform.position = value;
		}
	}

	public float speed
	{
		get
		{
			return mSpeed;
		}
		set
		{
			mSpeed = value;
		}
	}

	public float constraintLeft
	{
		get
		{
			return mConstraintLeft;
		}
		set
		{
			mConstraintLeft = value;
		}
	}

	public float constraintRight
	{
		get
		{
			return mConstraintRight;
		}
		set
		{
			mConstraintRight = value;
		}
	}

	public FacingType facing
	{
		get
		{
			return mFacing;
		}
		set
		{
			SetFacing(value);
		}
	}

	public float baseFacingAngle
	{
		get
		{
			return (mFacing == FacingType.Left) ? 180 : 0;
		}
	}

	public float speedModifier
	{
		get
		{
			return mSpeedModifier;
		}
		set
		{
			if (mWalkDirection == FacingType.Unchanged)
			{
				mAnimPlayer.baseAnimSpeed = value;
				if (value < mSpeedModifier)
				{
					mAnimPlayer.currAnimSpeed = value;
				}
			}
			if (value >= 0f)
			{
				mSpeedModifier = value;
			}
			else
			{
				mSpeedModifier = 1f;
			}
		}
	}

	public float impactPauseTime
	{
		get
		{
			return mImpactPauseTime;
		}
		set
		{
			mImpactPauseTime = value;
		}
	}

	public Action animEndedCallback
	{
		set
		{
			TaggedAnimPlayer taggedAnimPlayer = mAnimPlayer;
			taggedAnimPlayer.currAnimChangedCallback = (Action)Delegate.Combine(taggedAnimPlayer.currAnimChangedCallback, value);
		}
	}

	public GameObject projectilePrefab { get; set; }

	public bool startedDieAnim { get; set; }

	public bool readyToVanish
	{
		get
		{
			return startedDieAnim && !isAnimationPlaying && mPostDeathAction == EPostDeathAction.vanish;
		}
	}

	public bool isMoving
	{
		get
		{
			return mWalkDirection != FacingType.Unchanged;
		}
	}

	public bool isEffectivelyIdle
	{
		get
		{
			return !isInHurtState && (mAnimPlayer.actionAnimState == null || mAnimPlayer.actionAnim == "backpedalturn" || mPlayingVictoryAnim);
		}
	}

	public bool isInKnockback
	{
		get
		{
			return mKnockbackTime > 0f;
		}
	}

	public bool isInHurtState
	{
		get
		{
			return mKnockbackTime != 0f || mPlayingHurtAnim || mStunnedTimer > 0f || startedDieAnim;
		}
	}

	public string currentAnimation
	{
		get
		{
			if (mAnimPlayer.actionAnimState == null)
			{
				return mAnimPlayer.baseAnim;
			}
			return mCurrentAnim;
		}
	}

	public bool isAnimationPlaying
	{
		get
		{
			return !mAnimPlayer.IsDone();
		}
	}

	public float currentAnimationLength
	{
		get
		{
			return mAnimPlayer.currAnimState.length;
		}
	}

	public float xPos { get; set; }

	public float stunnedTimer
	{
		get
		{
			return mStunnedTimer;
		}
		set
		{
			mStunnedTimer = value;
		}
	}

	public void Awake()
	{
		mSpeed = 100f;
		mCachedTransform = base.transform;
		mAnimPlayer = GetComponent<TaggedAnimPlayer>();
		if (mAnimPlayer == null)
		{
			Debug.LogWarning("Tagged Animation Player was NOT found on GameObject: " + base.gameObject.name, base.gameObject);
			mAnimPlayer = base.gameObject.AddComponent<TaggedAnimPlayer>();
		}
		mAutoPaperdoll = GetComponent<AutoPaperdoll>();
		mSoundPlayer = GetComponent<SoundThemePlayer>();
		if (randomAnimSets != null)
		{
			RandomAnimSet[] array = randomAnimSets;
			foreach (RandomAnimSet randomAnimSet in array)
			{
				mRandomAnimSets.Add(randomAnimSet.name, randomAnimSet);
			}
		}
	}

	public void Start()
	{
		xPos = mCachedTransform.position.x;
		if (snapToGround && WeakGlobalInstance<RailManager>.instance != null)
		{
			mCachedTransform.position = new Vector3(Mathf.Clamp(xPos, WeakGlobalInstance<RailManager>.instance.GetMinX(mCachedTransform.position.z), WeakGlobalInstance<RailManager>.instance.GetMaxX(mCachedTransform.position.z)), WeakGlobalInstance<RailManager>.instance.GetY(mCachedTransform.position.z), mCachedTransform.position.z);
		}
	}

	public void Update()
	{
		mStunnedTimer = Mathf.Max(0f, mStunnedTimer - Time.deltaTime);
		if (impactPauseTime > 0f)
		{
			mAnimPlayer.paused = true;
			impactPauseTime -= Time.deltaTime;
			if (!(impactPauseTime <= 0f))
			{
				return;
			}
			impactPauseTime = 0f;
		}
		else if (mAnimPlayer.paused)
		{
			mAnimPlayer.paused = false;
		}
		if (startedDieAnim && !isAnimationPlaying && mPostDeathAction != 0 && mPostDeathAction != EPostDeathAction.stay)
		{
			StartPostDeathAction();
		}
		UpdateWalking();
		UpdateAttackDelivery();
	}

	public void StartWalkLeft()
	{
		if (!isInHurtState)
		{
			mWalkDirection = FacingType.Left;
			StartWalkAnim();
		}
	}

	public void StartWalkRight()
	{
		if (!isInHurtState)
		{
			mWalkDirection = FacingType.Right;
			StartWalkAnim();
		}
	}

	private void StartWalkAnim()
	{
		mPlayingVictoryAnim = false;
		if (mWalkDirection != facing && HasAnim("backpedal"))
		{
			if (mAnimPlayer.currAnim == "backpedal" && HasAnim("runback") && mAnimPlayer.currAnimTime >= 1.3f)
			{
				if (mAnimPlayer.baseAnim != "runback")
				{
					SetBaseAnim("runback");
					PlayAnimation("backpedalturn");
				}
			}
			else if (mAnimPlayer.baseAnim != "runback")
			{
				SetBaseAnim("backpedal");
			}
		}
		else
		{
			SetBaseAnim("walk");
			facing = mWalkDirection;
		}
		if (currentAnimation != "backpedalturn")
		{
			mAnimPlayer.RevertToBaseAnim();
		}
	}

	public void StopWalking()
	{
		mWalkDirection = FacingType.Unchanged;
		SetBaseAnimToIdle();
	}

	public void Idle()
	{
		StopWalking();
		if (!isInHurtState)
		{
			mAnimPlayer.RevertToBaseAnim();
		}
	}

	public void PlayVictoryAnim()
	{
		if (mPlayingVictoryAnim || !HasAnim("victory"))
		{
			return;
		}
		bool flag = HasAnim("victoryloop");
		StopWalking();
		mCurrentAnim = "victory";
		mAnimPlayer.PlayAnim(GetRandomAnim("victory"), mAnimPlayer.GetDefaultBlendSpeed("victory"), flag ? WrapMode.Once : WrapMode.Loop);
		if (flag)
		{
			TaggedAnimPlayer taggedAnimPlayer = mAnimPlayer;
			taggedAnimPlayer.currAnimDoneCallback = (TaggedAnimPlayer.TaggedAnimCallback)Delegate.Combine(taggedAnimPlayer.currAnimDoneCallback, (TaggedAnimPlayer.TaggedAnimCallback)delegate
			{
				mAnimPlayer.PlayAnim("victoryloop", mAnimPlayer.GetDefaultBlendSpeed("victoryloop"), WrapMode.Loop);
			});
		}
		mPlayingVictoryAnim = true;
	}

	public void Attack(float inMaxTime)
	{
		StopWalking();
		PlayAnimation("attack", inMaxTime);
		SetBaseAnim("attackidle");
		attackActive = false;
		mAttackDeliveryTriggered = false;
		animEndedCallback = ClearAttackCallback;
		mOnAttackDelivery = onMeleeAttackDelivery;
	}

	public void RangeAttack(float inMaxTime)
	{
		SetArrowVisible(true);
		if (HasAnim("rangedattack"))
		{
			PlayAnimation("rangedattack", inMaxTime);
		}
		else if (HasAnim("cast"))
		{
			PlayAnimation("cast", inMaxTime);
		}
		StopWalking();
		SetBaseAnim("rangedattackidle");
		attackActive = false;
		mAttackDeliveryTriggered = false;
		animEndedCallback = ClearAttackCallback;
		mOnAttackDelivery = onRangedAttackDelivery;
	}

	public void SetUseRangedAttackIdle(bool useRangedIdle)
	{
		mUseRangedAttackIdle = useRangedIdle;
		if (isEffectivelyIdle && !isMoving)
		{
			SetBaseAnimToIdle();
		}
	}

	public void SetUseCowerIdle(bool useCowerIdle)
	{
		if (mUseCowerIdle != useCowerIdle)
		{
			mUseCowerIdle = useCowerIdle;
			if (isEffectivelyIdle && !isMoving)
			{
				SetBaseAnimToIdle();
			}
		}
	}

	public void PlaySoundEvent(string theSoundEvent)
	{
		if ((bool)mSoundPlayer)
		{
			mSoundPlayer.PlaySoundEvent(theSoundEvent);
		}
	}

	public GameObject SpawnEffectAtJoint(GameObject effectPrefab, string theJointLabel, bool andAttach)
	{
		return mAutoPaperdoll.InstantiateObjectOnJoint(effectPrefab, theJointLabel, andAttach);
	}

	public GameObject InstantiateObjectOnJoint(GameObject prefab, string theJointLabel)
	{
		return SpawnEffectAtJoint(prefab, theJointLabel, true);
	}

	public void PerformSpecialAction(string theAnimName, Action onAttackDelivery)
	{
		StopWalking();
		PlayAnimation(theAnimName);
		attackActive = false;
		mAttackDeliveryTriggered = false;
		animEndedCallback = ClearAttackCallback;
		mOnAttackDelivery = onAttackDelivery;
	}

	public void PerformSpecialAction(string theAnimName, Action onAttackDelivery, float inMaxTime)
	{
		StopWalking();
		PlayAnimation(theAnimName, inMaxTime);
		attackActive = false;
		mAttackDeliveryTriggered = false;
		animEndedCallback = ClearAttackCallback;
		mOnAttackDelivery = onAttackDelivery;
	}

	public void PlayHurtAnim(string theAnimName)
	{
		if (HasAnim(theAnimName))
		{
			StopWalking();
			float num = speedModifier;
			mSpeedModifier = 1f;
			PlayAnimation(theAnimName);
			mSpeedModifier = num;
			mPlayingHurtAnim = true;
			animEndedCallback = ClearPlayingHurtAnim;
		}
	}

	public void Knockback()
	{
		StopWalking();
		if (HasAnim("knockback"))
		{
			float num = speedModifier;
			mSpeedModifier = 1f;
			PlayAnimation("knockback");
			mSpeedModifier = num;
		}
		else
		{
			mAnimPlayer.RevertToBaseAnim();
		}
		mKnockbackTime = 0.5f;
	}

	public void DeliverAttack()
	{
		mAttackDeliveryTriggered = true;
		if (mOnAttackDelivery != null)
		{
			mOnAttackDelivery();
		}
	}

	public void Die()
	{
		Die("die", EPostDeathAction.vanish);
	}

	public void Die(string animName, EPostDeathAction postDeathAction)
	{
		if (startedDieAnim || mPlayingHurtAnim)
		{
			return;
		}
		startedDieAnim = true;
		StopWalking();
		mAnimPlayer.ClearBaseAnim();
		speedModifier = 1f;
		if (HasAnim(animName))
		{
			PlayAnimation(animName);
			switch (animName)
			{
			case "dieexplode":
				postDeathAction = EPostDeathAction.vanish;
				break;
			}
		}
		else
		{
			PlayAnimation("die");
		}
		mPostDeathAction = postDeathAction;
	}

	public void StartPostDeathAction()
	{
		switch (mPostDeathAction)
		{
		case EPostDeathAction.melt:
			mPostDeathAction = EPostDeathAction.stay;
			ModelMelter.MeltGameObject(base.gameObject, OnDieMeltDone);
			break;
		case EPostDeathAction.ascend:
			mPostDeathAction = EPostDeathAction.stay;
			ModelMelter.AscendGameObject(base.gameObject, OnDieMeltDone);
			break;
		}
	}

	private void OnDieMeltDone()
	{
		mPostDeathAction = EPostDeathAction.vanish;
	}

	public void ShowArrow()
	{
		SetArrowVisible(true);
	}

	public void HideArrow()
	{
		SetArrowVisible(false);
	}

	public void SetArrowVisible(bool v, Projectile.EProjectileType type)
	{
		if (ProjectileManager.ProjectileShownWhileAiming(type))
		{
			if (projectilePrefab == null)
			{
				projectilePrefab = WeakGlobalInstance<ProjectileManager>.instance.PrefabForProjectile(type);
			}
			SetArrowVisible(v);
		}
	}

	public void SetArrowVisible(bool v)
	{
		if (!(projectilePrefab == null))
		{
			if (!v && mProjectileInstance != null)
			{
				UnityEngine.Object.Destroy(mProjectileInstance);
				mProjectileInstance = null;
			}
			else if (v && mProjectileInstance == null && (bool)projectilePrefab && mAutoPaperdoll.HasJoint("arrow"))
			{
				mProjectileInstance = mAutoPaperdoll.InstantiateObjectOnJoint(projectilePrefab, "arrow");
			}
		}
	}

	public void AimTowards(Projectile.EProjectileType projectileType, Vector3 targetPos)
	{
		AutoPaperdoll.LabeledJoint jointData = autoPaperdoll.GetJointData("aim_angle");
		if (jointData == null || jointData.joint == null)
		{
			return;
		}
		Transform joint = jointData.joint;
		Vector3 vector = ProjectileManager.ProjectileAimPosForTarget(projectileType, autoPaperdoll.GetJointPosition("projectile_spawn"), targetPos);
		Vector3 vector2 = vector - joint.position;
		Vector3 from = vector2;
		from.y = 0f;
		float num = Vector3.Angle(from, vector2);
		if (vector2.y < 0f)
		{
			num = 0f - num;
		}
		if (!mAimAngle.HasValue)
		{
			mAimAngle = 0f;
		}
		mAimAngle = Mathf.MoveTowardsAngle(mAimAngle.Value, num, 150f * Time.deltaTime);
		jointData = autoPaperdoll.GetJointData("aim_angle_2");
		if (jointData != null && !(jointData.joint == null))
		{
			Transform joint2 = jointData.joint;
			vector2 = targetPos - joint2.position;
			from = vector2;
			from.y = 0f;
			float num2 = Vector3.Angle(from, vector2);
			if (vector2.y < 0f)
			{
				num2 = 0f - num2;
			}
			if (joint2.IsChildOf(joint))
			{
				num2 -= num;
			}
			if (!mHeadAngle.HasValue)
			{
				mHeadAngle = 0f;
			}
			mHeadAngle = Mathf.MoveTowardsAngle(mHeadAngle.Value, num2, 150f * Time.deltaTime);
		}
	}

	public void ResetAimAngle()
	{
		if (mAimAngle.HasValue)
		{
			mAimAngle = Mathf.MoveTowardsAngle(mAimAngle.Value, 0f, 150f * Time.deltaTime);
			if (mAimAngle == 0f)
			{
				mAimAngle = null;
			}
		}
		if (mHeadAngle.HasValue)
		{
			mHeadAngle = Mathf.MoveTowardsAngle(mHeadAngle.Value, 0f, 150f * Time.deltaTime);
			if (mHeadAngle == 0f)
			{
				mHeadAngle = null;
			}
		}
	}

	public void FaceTowards(Vector3 targetPos)
	{
		Vector2 to = new Vector2(targetPos.z - mCachedTransform.position.z, targetPos.x - mCachedTransform.position.x);
		float num = Mathf.Abs(Vector2.Angle(Vector2.right, to));
		Vector3 eulerAngles = mCachedTransform.eulerAngles;
		if (num != eulerAngles.y)
		{
			if (targetPos.x < mCachedTransform.position.x)
			{
				num = 0f - num;
			}
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, num, 225f * Time.deltaTime);
			mCachedTransform.eulerAngles = eulerAngles;
		}
	}

	public void ResetFacingAngle()
	{
		float num = baseFacingAngle;
		Vector3 eulerAngles = mCachedTransform.eulerAngles;
		if (eulerAngles.y != num)
		{
			eulerAngles.y = Mathf.MoveTowardsAngle(eulerAngles.y, num, 225f * Time.deltaTime);
			if (isInHurtState)
			{
				eulerAngles.y = num;
			}
			mCachedTransform.eulerAngles = eulerAngles;
		}
	}

	public void Destroy()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public bool HasAnim(string theAnimName)
	{
		return mRandomAnimSets.ContainsKey(theAnimName) || mAnimPlayer[theAnimName] != null || mAnimPlayer[theAnimName + "01"] != null;
	}

	private void UpdateWalking()
	{
		if (impactPauseTime > 0f)
		{
			return;
		}
		if (mKnockbackTime != 0f)
		{
			UpdateKnockback();
		}
		else if (mWalkDirection != 0)
		{
			float num = (float)mWalkDirection * mSpeed * mSpeedModifier;
			if (mAnimPlayer.baseAnim == "runback")
			{
				num *= float.Parse(Singleton<Config>.instance.root.to("Game")["runbackspeedmult"]);
			}
			MoveBy(num * Time.deltaTime);
			mAnimPlayer.baseAnimSpeed = Mathf.Abs(num / animatedWalkSpeed);
		}
	}

	private void UpdateAttackDelivery()
	{
		if (mOnAttackDelivery == null)
		{
			return;
		}
		if (mAttackDeliveryTriggered || impactPauseTime > 0f)
		{
			if (!attackActive)
			{
				mAttackDeliveryTriggered = false;
			}
		}
		else if (attackActive)
		{
			DeliverAttack();
		}
	}

	private void MoveBy(float zDelta)
	{
		if (impactPauseTime > 0f || zDelta == 0f)
		{
			return;
		}
		float z = Mathf.Clamp(mCachedTransform.position.z + zDelta, mConstraintLeft, mConstraintRight);
		if (snapToGround && mSnapThisUpdate && WeakGlobalInstance<RailManager>.instance != null)
		{
			mCachedTransform.position = new Vector3(Mathf.Clamp(xPos, WeakGlobalInstance<RailManager>.instance.GetMinX(z), WeakGlobalInstance<RailManager>.instance.GetMaxX(z)), WeakGlobalInstance<RailManager>.instance.GetY(z), z);
		}
		else
		{
			mCachedTransform.position = new Vector3(mCachedTransform.position.x, mCachedTransform.position.y, z);
		}
		mSnapThisUpdate = !mSnapThisUpdate;
		if (facing == FacingType.Unchanged)
		{
			if (zDelta > 0f)
			{
				facing = FacingType.Right;
			}
			else if (zDelta < 0f)
			{
				facing = FacingType.Left;
			}
		}
	}

	private void UpdateKnockback()
	{
		if (mKnockbackTime < 0f)
		{
			mKnockbackTime += Time.deltaTime;
			if (mKnockbackTime > 0f)
			{
				mKnockbackTime = 0f;
			}
			return;
		}
		mKnockbackTime -= Time.deltaTime;
		float z = Mathf.Clamp(mCachedTransform.position.z - 200f * Time.deltaTime * (float)facing, mConstraintLeft, mConstraintRight);
		float num = ((!snapToGround || WeakGlobalInstance<RailManager>.instance == null) ? mCachedTransform.position.y : WeakGlobalInstance<RailManager>.instance.GetY(z));
		if (mKnockbackTime <= 0f)
		{
			if (HasAnim("knockbackland") && !startedDieAnim && !mPlayingHurtAnim)
			{
				PlayAnimation("knockbackland");
				mKnockbackTime = 0f;
			}
			else
			{
				mKnockbackTime = -0.25f;
			}
		}
		else
		{
			float num2 = mKnockbackTime / 0.5f - 0.5f;
			num += 32f - 128f * (num2 * num2);
		}
		float x = Mathf.Clamp(xPos, WeakGlobalInstance<RailManager>.instance.GetMinX(z), WeakGlobalInstance<RailManager>.instance.GetMaxX(z));
		mCachedTransform.position = new Vector3(x, num, z);
	}

	private void SetFacing(FacingType newFacing)
	{
		if (mFacing != newFacing)
		{
			Vector3 eulerAngles = mCachedTransform.eulerAngles;
			eulerAngles.y = ((newFacing == FacingType.Left) ? 180 : 0);
			mCachedTransform.eulerAngles = eulerAngles;
			mFacing = newFacing;
		}
	}

	private string GetRandomAnim(string anim)
	{
		RandomAnimSet value;
		if (mRandomAnimSets != null && mRandomAnimSets.TryGetValue(anim, out value))
		{
			if (value.onlyRandomizeOnce)
			{
				int? mLastClipIndexChosen = value.mLastClipIndexChosen;
				if (mLastClipIndexChosen.HasValue)
				{
					return value.clips[value.mLastClipIndexChosen.Value].name;
				}
			}
			int num = RandomRangeInt.between(0, value.clips.Length - 1);
			int? mLastClipIndexChosen2 = value.mLastClipIndexChosen;
			if (mLastClipIndexChosen2.HasValue && value.clips.Length >= 3)
			{
				while (value.mLastClipIndexChosen == num)
				{
					num = RandomRangeInt.between(0, value.clips.Length - 1);
				}
			}
			value.mLastClipIndexChosen = num;
			AnimationClip animationClip = value.clips[num];
			if (animationClip != null)
			{
				return animationClip.name;
			}
		}
		if ((bool)mAnimPlayer[anim])
		{
			return anim;
		}
		if ((bool)mAnimPlayer[anim + "01"])
		{
			return anim + "01";
		}
		return string.Empty;
	}

	private string LastRandomAnim(string anim)
	{
		RandomAnimSet value;
		if (mRandomAnimSets != null && mRandomAnimSets.TryGetValue(anim, out value))
		{
			int? mLastClipIndexChosen = value.mLastClipIndexChosen;
			if (mLastClipIndexChosen.HasValue)
			{
				return value.clips[value.mLastClipIndexChosen.Value].name;
			}
			return string.Empty;
		}
		if ((bool)mAnimPlayer[anim])
		{
			return anim;
		}
		if ((bool)mAnimPlayer[anim + "01"])
		{
			return anim + "01";
		}
		return string.Empty;
	}

	private void PlayAnimation(string anim)
	{
		mPlayingVictoryAnim = false;
		mCurrentAnim = anim;
		mAnimPlayer.PlayAnim(GetRandomAnim(anim), mAnimPlayer.GetDefaultBlendSpeed(anim), WrapMode.Once, speedModifier);
	}

	private void PlayAnimation(string anim, float inMaxTime)
	{
		PlayAnimation(anim);
		if (mAnimPlayer.currAnimState.length > inMaxTime)
		{
			mAnimPlayer.currAnimSpeed *= mAnimPlayer.currAnimState.length / inMaxTime;
		}
	}

	private void SetBaseAnim(string anim)
	{
		if (!(mAnimPlayer.baseAnim == LastRandomAnim(anim)))
		{
			mAnimPlayer.SetBaseAnim(GetRandomAnim(anim));
			mAnimPlayer.baseAnimSpeed = speedModifier;
		}
	}

	private void SetBaseAnimToIdle()
	{
		if (mUseCowerIdle && HasAnim("cower"))
		{
			SetBaseAnim("cower");
		}
		else if (mUseRangedAttackIdle && HasAnim("rangedattackidle"))
		{
			SetBaseAnim("rangedattackidle");
		}
		else
		{
			SetBaseAnim("idle");
		}
	}

	private void LateUpdate()
	{
		if (!mAimAngle.HasValue || mAnimPlayer.paused)
		{
			return;
		}
		AutoPaperdoll.LabeledJoint jointData = autoPaperdoll.GetJointData("aim_angle");
		if (jointData != null && !(jointData.joint == null))
		{
			Vector3 axis = mCachedTransform.rotation * Vector3.right;
			Quaternion quaternion = Quaternion.AngleAxis(0f - mAimAngle.Value, axis);
			Transform joint = jointData.joint;
			joint.rotation = quaternion * joint.rotation;
			jointData = autoPaperdoll.GetJointData("aim_angle_2");
			if (jointData != null && !(jointData.joint == null) && mHeadAngle.HasValue)
			{
				quaternion = Quaternion.AngleAxis(0f - mHeadAngle.Value, axis);
				joint = jointData.joint;
				joint.rotation = quaternion * joint.rotation;
			}
		}
	}

	private void ClearAttackCallback()
	{
		mOnAttackDelivery = null;
	}

	private void ClearPlayingHurtAnim()
	{
		mPlayingHurtAnim = false;
	}
}
