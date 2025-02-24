using System.Collections;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour
{
	public GenerateButton genButton;
	public TMP_InputField inputField;
	public TextMeshProUGUI sbsText;
	public TileManager manager;
	public bool genSlider;
    // Start is called before the first frame update
    void Start()
    {
		if (genSlider)
		{
			genButton.mapSize = 10;
			inputField.SetTextWithoutNotify(genButton.mapSize.ToString());
		}
    }

    // Update is called once per frame
    void Update()
    {
		if (genSlider)
		{
			if (genButton.mapSize <= 1)
			{
				genButton.mapSize = 2;
				inputField.SetTextWithoutNotify(genButton.mapSize.ToString());
			}
		}
		// else if (!genSlider)
		// {
		// 	if (manager.sbsStepTime < 0.01)
		// 	{
		// 		manager.sbsStepTime = 0.01f;
		// 		inputField.SetTextWithoutNotify(manager.sbsStepTime.ToString("F2"));
		// 	}
		// 	else if (manager.sbsStepTime > 1.0f)
		// 	{
		// 		manager.sbsStepTime = 1.0f;
		// 		inputField.SetTextWithoutNotify(manager.sbsStepTime.ToString("F2"));
		// 	}
		// }
	}

	//
	public void UpdateValue()
	{
		int val = GetComponentInChildren<Slider>().value.ConvertTo<int>();
 		genButton.mapSize = val;
		inputField.SetTextWithoutNotify(val.ToString());
	}
	
	//
	public void GetTypedValue()
	{
		string valString = inputField.text;
		if (int.TryParse(valString, out int newNum))
		{
			genButton.mapSize = newNum;
			inputField.SetTextWithoutNotify(newNum.ToString());
			GetComponent<Slider>().SetValueWithoutNotify(newNum);
		}
		else 
			inputField.SetTextWithoutNotify(genButton.mapSize.ToString());
	}

	//
	public void UpdateSBSDelay()
	{
		manager.sbsStepTime = GetComponentInChildren<Slider>().value;
		sbsText.SetText(manager.sbsStepTime.ToString("F2"));
		// inputField.SetTextWithoutNotify(manager.sbsStepTime.ToString("F2"));
	}

	//
	// public void GetTypedSBSDelayValue()
	// {
	// 	string valString = inputField.text;
	// 	if (int.TryParse(valString, out int newNum))
	// 	{
	// 		manager.sbsStepTime = newNum;
	// 		inputField.SetTextWithoutNotify(newNum.ToString());
	// 		GetComponent<Slider>().SetValueWithoutNotify(newNum);
	// 	}
	// 	else 
	// 		inputField.SetTextWithoutNotify(manager.sbsStepTime.ToString("F2"));
	// }
}
