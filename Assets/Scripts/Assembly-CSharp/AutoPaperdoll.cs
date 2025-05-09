using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Framework/Auto Paperdoll")]
public class AutoPaperdoll : MonoBehaviour
{
	[Serializable]
	public class Offset
	{
		public Vector3 position;

		public Vector3 rotation;

		public Vector3 scale;
	}

	[Serializable]
	public class LabeledJoint
	{
		public string label = string.Empty;

		public Transform joint;

		public GameObject autoAttachPrefab;

		public Offset transformOffset = new Offset();
	}

	public LabeledJoint[] joints = new LabeledJoint[0];

	private Dictionary<string, LabeledJoint> mJointsDict;

	public void AttachObjectToJoint(GameObject theObject, string theJointLabel)
	{
		AttachObjectToJoint(theObject, theJointLabel, true);
	}

	public void AttachObjectToJoint(GameObject theObject, string theJointLabel, bool ignoreCurrentTransform)
	{
		LabeledJoint labeledJoint = GetJointData(theJointLabel);
		if (labeledJoint == null)
		{
			labeledJoint = new LabeledJoint();
			labeledJoint.joint = base.transform;
		}
		AttachObjectToJoint(theObject, labeledJoint, ignoreCurrentTransform);
	}

	public void AttachObjectToJoint(GameObject theObject, LabeledJoint theJointData, bool ignoreCurrentTransform)
	{
		if (ignoreCurrentTransform)
		{
			AlignObjectWithJoint(theObject, theJointData);
		}
		theObject.transform.parent = theJointData.joint;
		BroadcastMessage("OnAutoPaperdollAdded", theObject, SendMessageOptions.DontRequireReceiver);
	}

	public void AlignObjectWithJoint(GameObject theObject, string theJointLabel)
	{
		LabeledJoint labeledJoint = GetJointData(theJointLabel);
		if (labeledJoint == null)
		{
			labeledJoint = new LabeledJoint();
			labeledJoint.joint = base.transform;
		}
		AlignObjectWithJoint(theObject, labeledJoint);
	}

	public void AlignObjectWithJoint(GameObject theObject, LabeledJoint theJointData)
	{
		theObject.transform.position = theJointData.joint.position;
		theObject.transform.rotation = theJointData.joint.rotation;
		theObject.transform.localScale = theJointData.joint.lossyScale;
		ApplyJointOffset(theObject, theJointData.transformOffset);
	}

	public bool HasJoint(string theJointLabel)
	{
		return mJointsDict != null && mJointsDict.ContainsKey(theJointLabel);
	}

	public LabeledJoint GetJointData(string theJointLabel)
	{
		if (mJointsDict == null)
		{
			if (joints == null || joints.Length == 0)
			{
				return null;
			}
			Start();
		}
		LabeledJoint value;
		if (mJointsDict.TryGetValue(theJointLabel, out value))
		{
			return value;
		}
		return null;
	}

	public GameObject InstantiateObjectOnJoint(GameObject thePrefab, string theJointLabel)
	{
		return InstantiateObjectOnJoint(thePrefab, theJointLabel, true, true);
	}

	public GameObject InstantiateObjectOnJoint(GameObject thePrefab, string theJointLabel, bool keepAttachedToJoint)
	{
		return InstantiateObjectOnJoint(thePrefab, theJointLabel, keepAttachedToJoint, keepAttachedToJoint);
	}

	public GameObject InstantiateObjectOnJoint(GameObject thePrefab, string theJointLabel, bool keepAttachedToJoint, bool applyJointScale)
	{
		GameObject result = null;
		if (thePrefab == null)
		{
			Debug.LogError("Tried to instantiate null prefab on joint " + theJointLabel + " of " + base.name);
			return result;
		}
		LabeledJoint labeledJoint = GetJointData(theJointLabel);
		if (labeledJoint == null)
		{
			labeledJoint = new LabeledJoint();
			labeledJoint.joint = base.transform;
		}
		result = UnityEngine.Object.Instantiate(thePrefab) as GameObject;
		if (result == null)
		{
			return result;
		}
		Vector3 localScale = result.transform.localScale;
		if (keepAttachedToJoint)
		{
			AttachObjectToJoint(result, labeledJoint, true);
		}
		else
		{
			AlignObjectWithJoint(result, labeledJoint);
		}
		if (!applyJointScale)
		{
			result.transform.localScale = localScale;
		}
		return result;
	}

	public Vector3 GetJointPosition(string theJointLabel)
	{
		return GetRelativeJointPosition(theJointLabel) + base.transform.position;
	}

	public Vector3 GetJointPosition(LabeledJoint theJointData)
	{
		return GetRelativeJointPosition(theJointData) + base.transform.position;
	}

	public Vector3 GetRelativeJointPosition(string theJointLabel)
	{
		LabeledJoint jointData = GetJointData(theJointLabel);
		if (jointData == null)
		{
			return Vector3.zero;
		}
		return GetRelativeJointPosition(jointData);
	}

	public Vector3 GetRelativeJointPosition(LabeledJoint theJointData)
	{
		Vector3 position = theJointData.transformOffset.position;
		position.Scale(theJointData.joint.lossyScale);
		position = theJointData.joint.rotation * position;
		return theJointData.joint.position + position - base.transform.position;
	}

	public Quaternion GetJointRotation(string theJointLabel)
	{
		LabeledJoint jointData = GetJointData(theJointLabel);
		if (jointData == null)
		{
			return base.transform.rotation;
		}
		return GetJointRotation(jointData);
	}

	public Quaternion GetJointRotation(LabeledJoint theJointData)
	{
		Vector3 eulerAngles = theJointData.joint.eulerAngles;
		eulerAngles += theJointData.transformOffset.rotation;
		return Quaternion.Euler(eulerAngles);
	}

	public Quaternion GetJointLocalRotation(string theJointLabel)
	{
		LabeledJoint jointData = GetJointData(theJointLabel);
		if (jointData == null)
		{
			return Quaternion.identity;
		}
		return GetJointLocalRotation(jointData);
	}

	public Quaternion GetJointLocalRotation(LabeledJoint theJointData)
	{
		Vector3 localEulerAngles = theJointData.joint.localEulerAngles;
		localEulerAngles += theJointData.transformOffset.rotation;
		return Quaternion.Euler(localEulerAngles);
	}

	public Vector3 GetJointScale(string theJointLabel)
	{
		LabeledJoint jointData = GetJointData(theJointLabel);
		if (jointData == null)
		{
			return base.transform.lossyScale;
		}
		return GetJointScale(jointData);
	}

	public Vector3 GetJointScale(LabeledJoint theJointData)
	{
		Vector3 lossyScale = theJointData.joint.lossyScale;
		Vector3 localScale = theJointData.joint.localScale;
		Vector3 vector = localScale + theJointData.transformOffset.scale;
		lossyScale.x = lossyScale.x / localScale.x * vector.x;
		lossyScale.y = lossyScale.x / localScale.y * vector.y;
		lossyScale.z = lossyScale.x / localScale.z * vector.z;
		return lossyScale;
	}

	public Vector3 GetJointLocalScale(string theJointLabel)
	{
		LabeledJoint jointData = GetJointData(theJointLabel);
		if (jointData == null)
		{
			return new Vector3(1f, 1f, 1f);
		}
		return GetJointLocalScale(jointData);
	}

	public Vector3 GetJointLocalScale(LabeledJoint theJointData)
	{
		return theJointData.joint.localScale + theJointData.transformOffset.scale;
	}

	private void Start()
	{
		if (joints == null || joints.Length == 0)
		{
			return;
		}
		mJointsDict = new Dictionary<string, LabeledJoint>();
		LabeledJoint[] array = joints;
		foreach (LabeledJoint labeledJoint in array)
		{
			if (labeledJoint == null)
			{
				continue;
			}
			if (labeledJoint.joint == null)
			{
				labeledJoint.joint = base.transform;
			}
			if (!string.IsNullOrEmpty(labeledJoint.label))
			{
				mJointsDict.Add(labeledJoint.label, labeledJoint);
			}
			if (!(labeledJoint.autoAttachPrefab != null))
			{
				continue;
			}
			GameObject autoAttachPrefab = labeledJoint.autoAttachPrefab;
			if (autoAttachPrefab == null)
			{
				Debug.LogWarning(labeledJoint.autoAttachPrefab.name + " is not a prefab, could not attach!");
				continue;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(autoAttachPrefab) as GameObject;
			if ((bool)gameObject)
			{
				AttachObjectToJoint(gameObject, labeledJoint, true);
			}
		}
	}

	private void ApplyJointOffset(GameObject theObject, Offset theOffset)
	{
		Transform transform = theObject.transform;
		transform.localScale += theOffset.scale;
		transform.localEulerAngles += theOffset.rotation;
		Vector3 position = theOffset.position;
		position.Scale(transform.lossyScale);
		position = transform.rotation * position;
		transform.localPosition += position;
	}
}
