using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Production : MonoBehaviour 
{
	public char LHS { get; private set; }
	private bool changeCaret;
	public int LastCaretPosition { get; private set; }
	public List<SystemModule> RHS { get; private set; }

	public Dropdown currentLHS;
	public InputField currentRHS;
	public Button RemoveButton;

	void Awake ()
	{
		RHS = new List<SystemModule>();

		//currentRHS = this.gameObject.transform.GetChild(1).gameObject.AddComponent<InputField>();
		//currentRHS.textComponent = currentRHS.transform.GetChild(1).GetComponent<Text>();
		//currentRHS.placeholder = currentRHS.transform.GetChild(0).gameObject.GetComponent<Text>();
		//currentRHS.readOnly = true;
		LastCaretPosition = currentRHS.caretPosition;
	}

	// Use this for initialization
	void Start () 
	{
		ModuleBuilder.FillSymbolDropdown(ref currentLHS, typeof(Parametric_Turtle));


		SetSymbol();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(currentRHS.isFocused)
		{
			LastCaretPosition = currentRHS.caretPosition;
			SetProduction();
		}
	}

	public void AppendModule(SystemModule m)
	{
		SystemModule newMod = m.CopyModule();
		RHS.Insert(LastCaretPosition, newMod);

		if(LastCaretPosition == 0)
			currentRHS.text = m.Symbol + currentRHS.text;
		else
		{
			currentRHS.text = currentRHS.text.Insert(LastCaretPosition, m.Symbol.ToString());
		}
		LastCaretPosition++;
	}

	public void RemoveSystemModule ()
	{		
		if(RHS.Count > 0 && LastCaretPosition > 0)
		{
			SystemModule m = RHS[LastCaretPosition - 1];
			RHS.Remove(m);
			currentRHS.text = currentRHS.text.Remove(LastCaretPosition - 1, 1);
			LastCaretPosition--;
		}
	}

	public void SetProduction()
	{
		FindObjectOfType<ProductionBuilder>().currentProduction = this;
	}

	public void SetSymbol()
	{
		LHS = ModuleBuilder.dropdownDictionary[currentLHS.value][0];
	}

	public static int SymbolIndexConverter(char c)
	{
		switch(c)
		{
			case '1':
				return Parametric_Turtle.PRODUCTION_ONE - 1;
			break;
			case '2':
				return Parametric_Turtle.PRODUCTION_TWO - 1;
			break;
			case '3':
				return Parametric_Turtle.PRODUCTION_THREE - 1;
			break;
			case '4':
				return Parametric_Turtle.PRODUCTION_FOUR - 1;
			break;
			case '5':
				return Parametric_Turtle.PRODUCTION_FIVE - 1;
			break;
			case Parametric_Turtle.DRAW:
				return 5;
			break;
			case Parametric_Turtle.OBJECT:
				return 6;
			break;
			case Parametric_Turtle.JOINT_OPEN:
				return 7;
			break;
			case Parametric_Turtle.JOINT_CLOSE:
				return 8;
			break;
			case Parametric_Turtle.BRANCH_OPEN:
				return 9;
			break;
			case Parametric_Turtle.BRANCH_CLOSE:
				return 10;
			break;
			case Parametric_Turtle.ROTATE:
				return 11;
			break;
			default:
				Debug.Log("Symbol not found in Turtle Alphabet");
				return -1;
		}
	}
}
