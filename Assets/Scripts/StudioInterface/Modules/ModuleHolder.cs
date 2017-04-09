using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ModuleHolder : ScriptableObject 
{
	[SerializeField] private SystemModule[] ModuleArray;
	[SerializeField] private SystemModule[] ProductionSymbols;
	[SerializeField] private char[] ProductionLHS;
	[SerializeField] private int[] ProductionLengths;

	public const string SAVE_PATH = "Prefabs/L_Systems/Jointed";
	public const string MODULE_FOLDER = "Modules";
	public const string PRODUCTION_FOLDER = "Productions";
	public const string PATH_TAIL = "_Holder";
	public const string FILE_TYPE = ".asset";
	public string Root { get { return "Assets/Resources/" + SAVE_PATH; } }
	public string Path;

	public void StoreLSystem (Parametric_L_System pls)
	{
		Path = Root + "/" + pls.name;
		StoreModules(pls);
		StoreProductions(pls);
		AssetDatabase.CreateAsset(this, Path + "/" + pls.name + PATH_TAIL + FILE_TYPE);
		AssetDatabase.SaveAssets();
		EditorUtility.SetDirty(this);
	}

	private void StoreModules(Parametric_L_System pls)
	{
		AssetDatabase.CreateFolder(Root, pls.name);
		AssetDatabase.CreateFolder(Path, MODULE_FOLDER);

		ModuleArray = new SystemModule[pls.returnList.Count];
		for(int i = 0; i < pls.returnList.Count; i++)
		{
			ModuleArray[i] = pls.returnList[i].CopyModule();
			AssetDatabase.CreateAsset(ModuleArray[i], Path + "/" + MODULE_FOLDER + "/" + i + FILE_TYPE);
		}

		AssetDatabase.SaveAssets();
		EditorUtility.SetDirty(this);
	}

	private void StoreProductions (Parametric_L_System pls)
	{
		AssetDatabase.CreateFolder(Path, PRODUCTION_FOLDER);

		KeyValuePair<char, SystemModule[]> ProductionModules;
		List<SystemModule> symbols = new List<SystemModule>();
		ProductionLengths = new int[pls.Productions.Count];
		ProductionLHS = new char[pls.Productions.Count];
		int count = 0;
		foreach(KeyValuePair<char, List<SystemModule>> kvp in pls.Productions)
		{
			AssetDatabase.CreateFolder(Path+"/"+PRODUCTION_FOLDER, kvp.Key.ToString());
			ProductionLengths[count] = kvp.Value.Count;
			ProductionLHS[count] = kvp.Key;

			for(int i = 0; i < kvp.Value.Count; i++)
			{
				symbols.Add(kvp.Value[i].CopyModule());
				kvp.Value[i].Print();
			}
			count++;
		}

		ProductionSymbols = symbols.ToArray();
		count = 0;
		for(int i = 0; i < ProductionLengths.Length; i++)
		{
			for(int j = count; j < count + ProductionLengths[i]; j++)
			{
				AssetDatabase.CreateAsset(ProductionSymbols[j], Path + "/" + PRODUCTION_FOLDER + "/" + ProductionLHS[i] + "/" + (j - count) + FILE_TYPE);
				Debug.Log(ProductionSymbols[j].Print());
			}

			count += ProductionLengths[i];
		}

		AssetDatabase.SaveAssets();
		EditorUtility.SetDirty(this);
	}

	public void LoadModules(Parametric_L_System pls)
	{
		pls.returnList.Clear();
		for(int i = 0; i < ModuleArray.Length; i++)
		{
			pls.returnList.Add(ModuleArray[i].CopyModule());
		}
	}

	public void LoadProductions(Parametric_L_System pls)
	{
		pls.Productions = new Dictionary<char, List<SystemModule>>();
		int count = 0;
		for(int i = 0; i < ProductionLengths.Length; i++)
		{
			List<SystemModule> newProd = new List<SystemModule>();
			for(int j = count; j < count + ProductionLengths[i]; j++)
			{
				newProd.Add(ProductionSymbols[j].CopyModule());
				Debug.Log(ProductionSymbols[j].Print());
			}
			pls.Productions.Add(ProductionLHS[i], newProd);
			count += ProductionLengths[i];
		}
	}
}
