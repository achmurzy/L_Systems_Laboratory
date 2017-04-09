using UnityEngine;
using System.Collections;

[System.Serializable]
public class SystemModule : ScriptableObject
{
	public char Symbol;
	public float Age;
    public float TerminalAge;
    public int Growth;

	public SystemModule(){}
	public SystemModule(char sym, float a, float term, int growth)
	{
		Symbol = sym;
		Age = a;
		TerminalAge = term;
		Growth = growth;
	}

	public virtual SystemModule CopyModule()
	{
		SystemModule sm = new SystemModule(Symbol, Age, TerminalAge, Growth);
		return sm;
	}

	public float GrowthFunction (float trait)
	{
		switch(Growth)
		{
			case GrowthList.NON_DEVELOPMENTAL:
				return NonDevelopmentalGrowth(trait);
				break;
			case GrowthList.LOGISTIC:
				return LogisticGrowth(trait);
				break;
			case GrowthList.EXPONENTIAL:
				return ExponentialGrowth(trait);
				break;
			case GrowthList.LINEAR:
				return LinearGrowth(trait);
				break;
			default:
				Debug.Log("Invalid growth code. Giving constant life.");
				return NonDevelopmentalGrowth(trait);
		}
	}

	//We can't achieve logistic growth without a state variable. This exponential constant is empirically derived for plant growth forms
	public const float EXPONENTIAL_CONSTANT = 0.48f;
	public float LogisticGrowth (float trait) 
	{
		return trait * Age * Mathf.Exp((1f - Mathf.Clamp(TerminalAge - Age, 0f, 1f)) * EXPONENTIAL_CONSTANT);
	}

	public float ExponentialGrowth(float trait)
	{
		return trait * Mathf.Exp(Age * EXPONENTIAL_CONSTANT);
	}

	public float LinearGrowth (float trait)
	{
		return trait * Age;
	}

	public float NonDevelopmentalGrowth (float trait)
	{
		return trait;
	}

	public string Print()
	{
		string output = Symbol.ToString() + " : " + 
						Age.ToString() + " : " + 
						TerminalAge.ToString();
		return output;
	}
}
