using System;
using System.Reflection;
using UnityEngine;

public class Utilities
{
	private static bool IsUnityReference(Type type)
	{
		if (!type.Assembly.FullName.Contains("UnityEngine"))
		{
			return false;
		}
		if (type == typeof(AnimationCurve))
		{
			return false;
		}
		return true;
	}

	public static object DeepCopy(object original)
	{
		if (original == null)
		{
			return null;
		}
		Type type = original.GetType();
		if (type.IsValueType || IsUnityReference(type))
		{
			return original;
		}
		if (type.IsArray)
		{
			object[] array = (object[])original;
			Array array2 = Array.CreateInstance(type.GetElementType(), array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				array2.SetValue(DeepCopy(array[i]), i);
			}
			return array2;
		}
		object obj = Activator.CreateInstance(type);
		MemberInfo[] members = type.GetMembers();
		foreach (MemberInfo memberInfo in members)
		{
			if (memberInfo.MemberType == MemberTypes.Field)
			{
				FieldInfo fieldInfo = (FieldInfo)memberInfo;
				object value = fieldInfo.GetValue(original);
				if (value is ICloneable)
				{
					fieldInfo.SetValue(obj, (value as ICloneable).Clone());
				}
				else
				{
					fieldInfo.SetValue(obj, DeepCopy(value));
				}
			}
			else
			{
				if (memberInfo.MemberType != MemberTypes.Property)
				{
					continue;
				}
				PropertyInfo propertyInfo = (PropertyInfo)memberInfo;
				if (propertyInfo.CanRead && propertyInfo.CanWrite)
				{
					object value2 = propertyInfo.GetValue(original, null);
					if (value2 is ICloneable)
					{
						propertyInfo.SetValue(obj, (value2 as ICloneable).Clone(), null);
					}
					else
					{
						propertyInfo.SetValue(obj, DeepCopy(value2), null);
					}
				}
			}
		}
		return obj;
	}
}
