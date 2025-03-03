using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SliderUpdate : MonoBehaviour
{
	//GENERATE BUTTON
	public GenerateButton genButton;
	//MAP DIMENSION INPUT FIELD
	public TMP_InputField inputField;
	//TEXT FOR STEP BY STEP SLIDERS
	public TextMeshProUGUI sbsText;
	//TILEMANAGER EXISTING IN SCENE
	public TileManager manager;
	//TRUE IF THE SLIDER DECIDES MAP DIMENSION
	public bool genSlider;

    // START IS CALLED BEFORE THE FIRST FRAME UPDATE
    void Start()
    {
		if (genSlider)
		{
			genButton.mapSize = 10;
			inputField.SetTextWithoutNotify(genButton.mapSize.ToString());
		}
    }

    // UPDATE IS CALLED ONCE PER FRAME
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
	}

	//UPDATE VALUE FROM SLIDER
	public void UpdateValue()
	{
		int val = GetComponentInChildren<Slider>().value.ConvertTo<int>();
 		genButton.mapSize = val;
		inputField.SetTextWithoutNotify(val.ToString());
	}
	
	//GET VALUE FROM INPUT FIELD
	public void GetTypedValue()
	{
		string valString = inputField.text;
		if (int.TryParse(valString, out int newNum) && newNum > 0 && newNum <= 100)
		{
			genButton.mapSize = newNum;
			inputField.SetTextWithoutNotify(newNum.ToString());
			GetComponent<Slider>().SetValueWithoutNotify(newNum);
		}
		else 
			inputField.SetTextWithoutNotify(genButton.mapSize.ToString());
	}

	//UPDATE STEP BY STEP DELAY TIME BASED ON SLIDER
	public void UpdateSBSDelay()
	{
		manager.sbsStepTime = GetComponentInChildren<Slider>().value;
		sbsText.SetText(manager.sbsStepTime.ToString("F2"));
	}

	//UPDATE AMOUNT OF STEPS PER UPDATE
	public void UpdateSBSSteps()
	{
		manager.tilesPerFrame = (int)GetComponentInChildren<Slider>().value;
		sbsText.SetText(manager.tilesPerFrame.ToString());
	}
}
