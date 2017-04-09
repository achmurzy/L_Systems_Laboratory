using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TransformDOL
{
	private const float MODULE_LIFETIME = 1f;
	public delegate void BuildProductions(Szilard_L_System pls, RawDOLSystem rawDOL);
	public static BuildProductions ProductionStrategy = SzilardRules;

	public static Szilard_L_System BuildWBESystem (int generations, int branchOrder, float branchRadii, float xylemRadii)
	{
		GameObject blankSystem = GameObject.Instantiate(Resources.Load ("Prefabs/L_Systems/BlankSystem")) as GameObject;
		Szilard_L_System generatedSystem = blankSystem.AddComponent<Szilard_L_System>();

		generatedSystem.maturity = generations;
		generatedSystem.segmentGrowth = 0.5f;
		generatedSystem.axlGrowth = 10.5f;

		//Add trunk axiom
		int charCode = char.ConvertToUtf32("a", 0);
		Debug.Log(charCode);
		generatedSystem.returnList.Add(new SzilardModule('a', 0, 1, MODULE_LIFETIME));

		for(int i = 0; i < generations; i++)
		{
			List<SzilardModule> RHSSzilardModules = new List<SzilardModule>();
			RHSSzilardModules.Add(new SzilardModule('F', 0, 1, generatedSystem.segmentGrowth));
			int branches = branchOrder;
			while(branches > 0)
			{
				RHSSzilardModules.Add(new SzilardModule('[', 0, 1, 0));
				if(branches % 2 == 0)
					RHSSzilardModules.Add(new SzilardModule('^', 0, 1, generatedSystem.axlGrowth));
				else
					RHSSzilardModules.Add(new SzilardModule('^', 0, 1, -generatedSystem.axlGrowth));
				RHSSzilardModules.Add(new SzilardModule(char.ConvertFromUtf32(charCode+1)[0], 0, 1, generatedSystem.segmentGrowth));
				RHSSzilardModules.Add(new SzilardModule(']', 0, 1, 0));

				branches--;
			}

			generatedSystem.productions.Add(char.ConvertFromUtf32(charCode)[0], RHSSzilardModules);
			charCode++;
		}

		return generatedSystem;
	}

	public static Szilard_L_System ConvertRawSystem(RawDOLSystem rawDOL)
	{
		GameObject blankSystem = GameObject.Instantiate(Resources.Load ("Prefabs/L_Systems/BlankSystem")) as GameObject;
		Szilard_L_System generatedSystem = blankSystem.AddComponent<Szilard_L_System>();

		generatedSystem.maturity = 10f;
		generatedSystem.segmentGrowth = 0.05f;
		generatedSystem.axlGrowth = 22.5f;

		foreach (char c in rawDOL.Axiom)
		{
			if(c == rawDOL.Alphabet[rawDOL.Alphabet.Length-1])
				generatedSystem.returnList.Add (new SzilardModule ('F', 0, 1, generatedSystem.segmentGrowth));
			else
				generatedSystem.returnList.Add (new SzilardModule (c, 0, 1, MODULE_LIFETIME));
		}

		ProductionStrategy (generatedSystem, rawDOL);
		ProductionStrategy = SzilardRules;

		return generatedSystem;
	}

	public static void SzilardRules (Szilard_L_System pls, RawDOLSystem rawDOL)
	{}

	public static void BuildOneRuleProductions(Szilard_L_System pls, RawDOLSystem rawDOL)
	{
		List<SzilardModule> RHSSzilardModules = new List<SzilardModule>();
		pls.returnList.Clear ();
		pls.returnList.Add(new SzilardModule ('F', 0, 1, pls.segmentGrowth));
		foreach(KeyValuePair<char, List<char>> kvp in rawDOL.Productions)
		{
			for(int i = 0; i < kvp.Value.Count; i++)
				RHSSzilardModules.Add(new SzilardModule('F', 0, 1, pls.segmentGrowth));
		}

		pls.productions.Add ('F', RHSSzilardModules);
	}

	//No angular or bracketed notations - Only indexing the first k - 1 members of the alphabet
	//The kth symbol is made to be the write-rule
	public static void BuildIndexedRules(Szilard_L_System pls, RawDOLSystem rawDOL)
	{
		foreach(KeyValuePair<char, List<char>> kvp in rawDOL.Productions)
		{
			List<SzilardModule> RHSSzilardModules = new List<SzilardModule>();

			RHSSzilardModules.Add(new SzilardModule(kvp.Value[0], 0, 1, MODULE_LIFETIME));
			if(kvp.Value.Count > 1)
			{
				RHSSzilardModules.Add(new SzilardModule(kvp.Value[1], 0, 1, MODULE_LIFETIME));
				if(kvp.Value.Count > 2)
				{
					for(int j = 2; j < kvp.Value.Count; j++)
					{
						RHSSzilardModules.Add(new SzilardModule('F', 0, 1, pls.segmentGrowth));
					}
				}
			}

			pls.productions.Add(kvp.Key, RHSSzilardModules);
		}
	}

	//Here we place the alphabet symbol corresponding to the left-hand side of a rule at the tail end
	//of the right-hand side to simulate apical growth
	//Importantly, we choose to include angle symbols and brackets
	public static void ReverseIndexedRules(Szilard_L_System pls, RawDOLSystem rawDOL)
	{
		foreach(KeyValuePair<char, List<char>> kvp in rawDOL.Productions)
		{
			List<SzilardModule> RHSSzilardModules = new List<SzilardModule>();

			if(kvp.Value.Count > 1)
			{
				RHSSzilardModules.Add(new SzilardModule('[', 0, 1, 0));
				RHSSzilardModules.Add(new SzilardModule('^', 0, 1, pls.axlGrowth));
				RHSSzilardModules.Add(new SzilardModule(kvp.Value[1], 0, 1, MODULE_LIFETIME));
				RHSSzilardModules.Add(new SzilardModule(']', 0, 1, 0));

				if(kvp.Value.Count > 2)
				{
					for(int j = 2; j < kvp.Value.Count; j++)
					{
						RHSSzilardModules.Add(new SzilardModule('F', 0, 1, pls.segmentGrowth));
					}
				}
			}

			RHSSzilardModules.Add(new SzilardModule(kvp.Value[0], 0, 1, MODULE_LIFETIME));
			pls.productions.Add(kvp.Key, RHSSzilardModules);
		}

	}

	public class RawDOLSystem
	{
		public char[] Alphabet;
		public List<char> Axiom;
		public Dictionary<char, List<char>> Productions;

		public RawDOLSystem(int k)
		{
			Alphabet = new char[k+1];
			Axiom = new List<char>();
			Productions = new Dictionary<char, List<char>>();
		}

		public void GetAlphabet(int k)
		{
			char symbol = 'a';
			for(int i = 0; i <= k; i++)
			{
				Alphabet[i] = symbol;
				symbol++;
			}
		}
		
		public void GetAxiom(int q_0)
		{
			int t = q_0 - 1;
			Axiom.Add (Alphabet [0]);
			while(t > 0)
			{
				Axiom.Add(Alphabet[Alphabet.Length-1]);
				t--;
			}
		}
		
		public void GetProductions(double[] qVals)
		{
			for(int i = 0; i < Alphabet.Length; i++)
			{
				if(i == Alphabet.Length - 1)
				{
					Productions[Alphabet[i]] = new List<char>();
					Productions[Alphabet[i]].Add(Alphabet[i]);
				}
				else
				{
					Productions[Alphabet[i]] = new List<char>();
					Productions[Alphabet[i]].Add(Alphabet[i]);
					Productions[Alphabet[i]].Add(Alphabet[i+1]);
					int t = (int)qVals[i+1] - 1;
					while(t > 0)
					{
						Productions[Alphabet[i]].Add(Alphabet[Alphabet.Length-1]);
						t--;
					}
				}
			}
		}

		public string PrintSystem()
		{
			string output = "Alphabet: ";
			foreach (char c in Alphabet)
				output += c + " ";

			output += "\n" + "Axiom: ";
			foreach (char c in Axiom)
				output += c + " ";

			output += "\n" + "Productions:\n";
			foreach (KeyValuePair<char, List<char>> production in Productions)
			{
				string RHS = "";
				foreach(char c in production.Value)
					RHS += c + " ";
				output += production.Key.ToString () + "-->" + RHS + "\n";
			}
			return output;
		}
	}
}
