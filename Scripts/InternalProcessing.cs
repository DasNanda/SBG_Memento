using System;
using System.IO;
using UnityEngine;

namespace SBG.Memento
{
	public static class InternalProcessing
	{
		public static object GetRawDataEntry(SaveData saveData, string key)
        {
			object entry = saveData.GetValueFromTable(key, null);

            if (entry is SerializedData) return saveData.GetSubData(key);
            if (entry is SerializedDataArray) return saveData.GetSubDataArray(key);
            return entry;
        }

        public static SaveData GetData(SaveType type)
        {
            return SaveManager.GetData(type);
        }

		public static SaveData LoadFromBinaryFile(string path, out int versionNr)
        {
            string base64String = File.ReadAllText(path);
            byte[] byteData = Convert.FromBase64String(base64String);

            try
            {
                var fileContent = SerializedSaveFile.ReadFromBytes(byteData); 

                versionNr = fileContent.VersionNr;

                //Check File Version Compatibility
                if (fileContent.VersionNr < VersionControl.MIN_FILE_VERSION)
                {
#if UNITY_EDITOR
                    Debug.Log("MEMENTO: Save File is outdated!");
#endif
                    return null;
                }

                return fileContent.RootData.Deserialize();
            }
#if UNITY_EDITOR
            catch (Exception errorMsg)
            {
                Debug.LogError("MEMENTO: Binary Load failed! Printing Error Message:");
                Debug.LogError(errorMsg);
#else
            catch
            {
#endif
                versionNr = -1;
                return null;
            }
        }
	}
}