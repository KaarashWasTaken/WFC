using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManualStepToggle : MonoBehaviour
{
	// public Toggle sbsToggle;
	public GameObject stepByStep;
	public GameObject stepDelay;
	public Toggle manualToggle;
	public Button manualStep;
    // Start is called before the first frame update
    void Awake()
    {
		stepByStep.SetActive(GetComponent<Toggle>().isOn);
		SetStepMode();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public bool GetManualMode()
	{
		return manualToggle.isOn;
	}

	public void SetStepMode()
	{
		stepDelay.SetActive(!manualToggle.isOn);
		manualStep.gameObject.SetActive(manualToggle.isOn);
	}

	public void SetStepByStep()
	{
		stepByStep.SetActive(GetComponent<Toggle>().isOn);
	}
}
