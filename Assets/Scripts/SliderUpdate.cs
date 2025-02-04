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
	public GenerateButton GenButton;
	public TMP_InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
		// GenButton = GetComponentInParent<GenerateButton>();
		// textBox = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	public void UpdateValue()
	{
		int val = GetComponentInChildren<Slider>().value.ConvertTo<int>();
 		GenButton.MapSize = val;
		inputField.SetTextWithoutNotify(val.ToString());
	}
	public void GetTypedValue()
	{
		// string valString = GetComponentInChildren<InputField>().text;
		string valString = inputField.text;
		if (int.TryParse(valString, out int newNum))
		{
			GenButton.MapSize = newNum;
			inputField.SetTextWithoutNotify(newNum.ToString());
			GetComponent<Slider>().SetValueWithoutNotify(newNum);
		}
		else 
			inputField.SetTextWithoutNotify(GenButton.MapSize.ToString());
	}
}
