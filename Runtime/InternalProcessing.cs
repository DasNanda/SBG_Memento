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

		public static SaveData LoadFromBinaryFile(string path)
        {
            string base64String = File.ReadAllText(path);
            return LoadFromBase64(base64String);
        }

        public static SaveData LoadFromBase64(string base64String)
        {
            if (string.IsNullOrEmpty(base64String)) return null;

            byte[] byteData = Convert.FromBase64String(base64String);

            try
            {
                var fileContent = SerializedSaveFile.ReadFromBytes(byteData);

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
                return null;
            }
        }
	}
}