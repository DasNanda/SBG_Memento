using System;
using UnityEngine;
using System.IO;
using System.Text;

namespace SBG.Memento
{
	[Serializable]
	internal struct SerializedSaveFile
	{
		public SerializedData RootData;

		public SerializedSaveFile(SaveData root)
		{
			RootData = new SerializedData(root);
		}

        public SerializedSaveFile(SerializedData root)
        {
            RootData = root;
        }

        public static byte[] WriteToBytes(SerializedSaveFile file)
		{
            byte[] bytes = Array.Empty<byte>();

            using (var stream = new MemoryStream())
			{
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, false))
                {
                    WriteSubData(writer, file.RootData);
                }

				bytes = stream.ToArray();
            }

			return bytes;
		}

		public static SerializedSaveFile ReadFromBytes(byte[] bytes)
		{
			SerializedData rootData = new();

			using (var stream = new MemoryStream(bytes))
			{
				using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, false))
				{
                    rootData = ReadSubData(reader);
				}
			}

			return new SerializedSaveFile(rootData);
		}

		private static void WriteSubData(BinaryWriter writer, SerializedData subData)
		{
            writer.Write(subData.Version);
			writer.Write(subData.Data.Length);

			foreach (var data in subData.Data)
			{
				writer.Write(data.Key);
                if (data.Value != null)
                {
                    writer.Write(data.Value.GetType().ToString());
                    WriteDataValue(writer, data.Value);
                }
                else
                {
                    writer.Write("null");
                }
            }
		}

        private static SerializedData ReadSubData(BinaryReader reader)
        {
            SerializedData subData = new SerializedData();
            ushort version = reader.ReadUInt16();
			int length = reader.ReadInt32();

            subData.Version = version;
            subData.Data = new SerializedDataEntry[length];

			for (int i = 0; i < length; i++)
			{
				subData.Data[i].Key = reader.ReadString();
                string typeString = reader.ReadString();

                if (typeString == "null") subData.Data[i].Value = null;
                else subData.Data[i].Value = ReadDataValue(reader, Type.GetType(typeString));
            }

            return subData;
        }

        private static void WriteDataValue(BinaryWriter writer, object value)
		{
			if (value is SerializedData) WriteSubData(writer, (SerializedData)value);
			else if (value is SerializedDataArray) WriteArrayValue(writer, ((SerializedDataArray)value).DataArray);
			else if (value is Array) WriteArrayValue(writer, (Array)value);
            else if (value is int) writer.Write((int)value);
            else if (value is long) writer.Write((long)value);
            else if (value is float) writer.Write((float)value);
            else if (value is double) writer.Write((double)value);
            else if (value is bool) writer.Write((bool)value);
            else if (value is string) writer.Write((string)value);
			else Debug.LogError($"MEMENTO: Could not write data value of type \"{value.GetType()}\" because it is not supported!");
        }

        private static object ReadDataValue(BinaryReader reader, Type type)
        {
            if (type == typeof(SerializedData)) return ReadSubData(reader);
            else if (type == typeof(SerializedDataArray)) return new SerializedDataArray((SerializedData[])ReadArrayValue(reader, typeof(SerializedData)));
            else if (type.IsArray) return ReadArrayValue(reader, type.GetElementType());
            else if (type == typeof(int)) return reader.ReadInt32();
            else if (type == typeof(long)) return reader.ReadInt64();
            else if (type == typeof(float)) return reader.ReadSingle();
            else if (type == typeof(double)) return reader.ReadDouble();
            else if (type == typeof(bool)) return reader.ReadBoolean();
            else if (type == typeof(string)) return reader.ReadString();
            
			Debug.LogError($"MEMENTO: Could not read data value of type \"{type}\" because it is not supported!");
			return null;
        }

        private static void WriteArrayValue(BinaryWriter writer, Array array)
		{
			writer.Write(array.Length);
			for (int i = 0; i < array.Length; i++) WriteDataValue(writer, array.GetValue(i));
        }

        private static object ReadArrayValue(BinaryReader reader, Type elementType)
        {
			var array = Array.CreateInstance(elementType, reader.ReadInt32());
            for (int i = 0; i < array.Length; i++)
			{
				object value = ReadDataValue(reader, elementType);
                array.SetValue(value, i);
            }

			return array;
        }
    }
}