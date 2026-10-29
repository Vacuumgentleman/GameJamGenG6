#region copyright
// -------------------------------------------------------
// Copyright (C) Dmitry Yuhanov [https://codestage.net]
// -------------------------------------------------------
#endregion

namespace CodeStage.Maintainer.Tools
{
	using Issues;
	using UnityEditor;
	using UnityEngine;

	public static class CSSerializedPropertyTools
	{
		public static bool IsPropertyHasMissingReference(SerializedProperty currentProperty)
		{
			if (currentProperty.propertyType != SerializedPropertyType.ObjectReference ||
				currentProperty.objectReferenceValue != null)
				return false;

			if (CSInstanceIdTools.GetObjectReferenceId(currentProperty) != 0)
				return true;

			var fileId = currentProperty.FindPropertyRelative("m_FileID");
			if (fileId != null)
			{
				if (fileId.intValue != 0) 
					return true;
			}
			else
			{
				Debug.LogError(Maintainer.ErrorForSupport("Property seems to be missing reference but m_FileID could not be found!",
					IssuesFinder.ModuleName));
			}

			return false;
		}
		
		public static uint GetPropertyHash(SerializedProperty sp)
		{
			return sp.contentHash;
		}
	}
}