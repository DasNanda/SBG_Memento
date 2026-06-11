using System;
using System.IO;
using UnityEngine;

namespace SBG.Memento
{
	public static class SaveManager
	{
        #region FIELDS

        /// <summary>
        /// Default Name for the Game File
        /// </summary>
        public const string DEFAULT_GAMEFILE = "SaveGame";
        /// <summary>
        /// Default Name for the Settings File
        /// </summary>
        public const string DEFAULT_SETTINGFILE = "Settings";

        /// <summary>
        /// Subscribe to this event to write all your data to the SaveManager before it writes to a file of the given Type
        /// </summary>
        public static Action<SaveType> OnBeforeSave;
        /// <summary>
        /// Subscribe to this event to read out your data after a file with the given Type was loaded successfully
        /// </summary>
        public static Action<SaveType> OnLoadFinished;

        private static SaveData gameData = new SaveData();
        private static SaveData settingData = new SaveData();

        internal static SaveData GetData(SaveType type)
        {
            if (type == SaveType.Settings) return settingData;

            return gameData;
        }

        #endregion

        #region SET VALUES

        public static void SetValue(string key, int value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, long value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, float value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, double value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, string value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, bool value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, int[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, long[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, float[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, double[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, string[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, bool[] value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, SaveData data, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, data);
        public static void SetValue(string key, SaveData[] dataArray, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, dataArray);
        public static void SetValue(string key, Vector2 value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, Vector3 value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, Quaternion value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);
        public static void SetValue(string key, Color value, SaveType target = SaveType.GameFile) => GetData(target).SetValue(key, value);

        #endregion

        #region GET VALUES

        public static int GetInt(string key, int defaultValue=0, SaveType source = SaveType.GameFile) => GetData(source).GetInt(key, defaultValue);
        public static long GetLong(string key, long defaultValue=0, SaveType source = SaveType.GameFile) => GetData(source).GetLong(key, defaultValue);
        public static float GetFloat(string key, float defaultValue=0, SaveType source = SaveType.GameFile) => GetData(source).GetFloat(key, defaultValue);
        public static double GetDouble(string key, double defaultValue=0, SaveType source = SaveType.GameFile) => GetData(source).GetDouble(key, defaultValue);
        public static string GetString(string key, string defaultValue="", SaveType source = SaveType.GameFile) => GetData(source).GetString(key, defaultValue);
        public static bool GetBool(string key, bool defaultValue=false, SaveType source = SaveType.GameFile) => GetData(source).GetBool(key, defaultValue);
        public static int[] GetIntArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetIntArray(key);
        public static long[] GetLongArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetLongArray(key);
        public static float[] GetFloatArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetFloatArray(key);
        public static double[] GetDoubleArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetDoubleArray(key);
        public static string[] GetStringArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetStringArray(key);
        public static bool[] GetBoolArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetBoolArray(key);
        public static SaveData GetSubData(string key, SaveType source = SaveType.GameFile, ushort minVersion=0, SaveDataVersionConverter converter=null) => GetData(source).GetSubData(key, minVersion, converter);
        public static SaveData[] GetSubDataArray(string key, SaveType source = SaveType.GameFile) => GetData(source).GetSubDataArray(key);
        public static Vector2 GetVector2(string key, SaveType source = SaveType.GameFile) => GetData(source).GetVector2(key);
        public static Vector2 GetVector2(string key, Vector2 defaultValue, SaveType source = SaveType.GameFile) => GetData(source).GetVector2(key, defaultValue);
        public static Vector3 GetVector3(string key, SaveType source = SaveType.GameFile) => GetData(source).GetVector3(key);
        public static Vector3 GetVector3(string key, Vector3 defaultValue, SaveType source = SaveType.GameFile) => GetData(source).GetVector3(key, defaultValue);
        public static Quaternion GetQuaternion(string key, SaveType source = SaveType.GameFile) => GetData(source).GetQuaternion(key);
        public static Quaternion GetQuaternion(string key, Quaternion defaultValue, SaveType source = SaveType.GameFile) => GetData(source).GetQuaternion(key, defaultValue);
        public static Color GetColor(string key, SaveType source = SaveType.GameFile) => GetData(source).GetColor(key);
        public static Color GetColor(string key, Color defaultValue, SaveType source = SaveType.GameFile) => GetData(source).GetColor(key, defaultValue);

        #endregion

        #region SAVE FILE FUNCTIONS

        public static string GetPath(string filename, SaveType saveType)
        {
            string basePath = $"{Application.persistentDataPath}/SaveFiles";

            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);

            string end = saveType == SaveType.Settings ? "memset" : "mem";

            return $"{basePath}/{filename}.{end}";
        }

        public static bool IsValidGameFile(string filename=DEFAULT_GAMEFILE)
        {
            return File.Exists(GetPath(filename, SaveType.GameFile));
        }

        public static bool IsValidSettingsFile(string filename = DEFAULT_SETTINGFILE)
        {
            return File.Exists(GetPath(filename, SaveType.Settings));
        }

        /// <summary>
        /// Returns the names of all files in the save folder
        /// </summary>
        public static string[] GetAllSaveFiles(SaveType fileType = SaveType.GameFile)
        {
            string baseDir = $"{Application.persistentDataPath}/SaveFiles/";

            if (Directory.Exists(baseDir) == false) return null;

            string end = fileType == SaveType.Settings ? "memset" : "mem";

            string[] paths = Directory.GetFiles(baseDir, $"*.{end}");

            if (paths == null || paths.Length == 0) return null;

            string[] filenames = new string[paths.Length];

            for (int i = 0; i < filenames.Length; i++)
            {
                filenames[i] = Path.GetFileNameWithoutExtension(paths[i]);
            }

            return filenames;
        }

        /// <summary>
        /// Saves the current Game Data using the given filename
        /// </summary>
        /// <returns>Returns true the Save was successful</returns>
        public static bool SaveGame(string filename = DEFAULT_GAMEFILE, ushort rootVersion = 0) => Save(filename, SaveType.GameFile, rootVersion);
        /// <summary>
        /// Saves the current Settings Data using the given filename
        /// </summary>
        /// <returns>Returns true the Save was successful</returns>
        public static bool SaveSettings(string filename = DEFAULT_SETTINGFILE, ushort rootVersion = 0) => Save(filename, SaveType.Settings, rootVersion);

        private static bool Save(string filename, SaveType saveContent, ushort rootVersion = 0)
        {
            OnBeforeSave?.Invoke(saveContent);

            string path = GetPath(filename, saveContent);

            var data = GetData(saveContent);
            data.Version = rootVersion;

            var fileContent = new SerializedSaveFile(data);

            return SaveToBinary(path, fileContent);
        }


        /// <summary>
        /// Attempts to load Game Data into the cache from the given file.
        /// </summary>
        /// <returns>Returns true the Load was successful</returns>
        public static bool LoadGame(string filename = DEFAULT_GAMEFILE, ushort minRootVersion=0, SaveDataVersionConverter converter = null) => Load(filename, SaveType.GameFile, minRootVersion, converter);
        /// <summary>
        /// Attempts to load Settings Data into the cache from the given file.
        /// </summary>
        /// <returns>Returns true the Load was successful</returns>
        public static bool LoadSettings(string filename = DEFAULT_SETTINGFILE, ushort minRootVersion = 0, SaveDataVersionConverter converter = null) => Load(filename, SaveType.Settings, minRootVersion, converter);

        
        private static bool Load(string filename, SaveType loadContent, ushort minRootVersion = 0, SaveDataVersionConverter converter = null)
        {
            string path = GetPath(filename, loadContent);

            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.LogError("MEMENTO: Invalid Path");
#endif
                return false;
            }

            if (LoadFromBinary(path, loadContent))
            {
                SaveData data = GetData(loadContent);

                if (!SaveData.IsNullOrEmpty(data) && data.Version < minRootVersion)
                {
                    data = converter?.Invoke(data, data.Version, minRootVersion);
                }

                OnLoadFinished?.Invoke(loadContent);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool DeleteGameFile(string filename = DEFAULT_GAMEFILE) => DeleteSaveFile(filename, SaveType.GameFile);
        public static bool DeleteSettingsFile(string filename = DEFAULT_SETTINGFILE) => DeleteSaveFile(filename, SaveType.Settings);

        private static bool DeleteSaveFile(string filename, SaveType fileType)
        {
            string path = GetPath(filename, fileType);

            if (File.Exists(path))
            {
                File.Delete(path);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Clears the current Game Cache. Use this to start a new save file.
        /// </summary>
        public static void ClearGameData() => ClearData(SaveType.GameFile);

        /// <summary>
        /// Clears the current Settings Cache. Use this to start a new save file.
        /// </summary>
        public static void ClearSettingsData() => ClearData(SaveType.Settings);

        private static void ClearData(SaveType fileType)
        {
            if (fileType == SaveType.Settings) settingData = new SaveData();
            else gameData = new SaveData();
        }

        #endregion

        #region BINARY PROCESSING

        private static bool SaveToBinary(string path, SerializedSaveFile file)
        {
            try
            {
                byte[] bytes = SerializedSaveFile.WriteToBytes(file);
                string base64String = Convert.ToBase64String(bytes);
                File.WriteAllText(path, base64String);
                return true;
            }
#if UNITY_EDITOR
            catch (Exception errorMsg)
            {
                Debug.LogError("MEMENTO: Binary Save Failed!");
                Debug.LogError(errorMsg);
#else
            catch
            {
#endif
                return false;
            }
        }

        private static bool LoadFromBinary(string path, SaveType fileType)
        {
            string base64Text = File.ReadAllText(path);
            byte[] byteData = Convert.FromBase64String(base64Text);

            // Wipe data in case anything doesnt load correctly later on
            ClearData(fileType);

            try
            {
                var fileContent = SerializedSaveFile.ReadFromBytes(byteData);

                if (fileType == SaveType.GameFile) gameData = fileContent.RootData.Deserialize();
                else settingData = fileContent.RootData.Deserialize();

                return true;
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
                ClearData(fileType);
                return false;
            }
        }

#endregion
    }
}