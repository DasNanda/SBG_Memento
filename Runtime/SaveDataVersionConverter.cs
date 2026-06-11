namespace SBG.Memento
{
	public delegate SaveData SaveDataVersionConverter(SaveData oldData, ushort oldVersion, ushort targetVersion);
}