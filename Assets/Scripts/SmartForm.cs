using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SmartForm : SmartBehaviour
{


    public Color warningColor = new Color(.7f, 0, 0, 1);

    private UnityEvent valueChange;

    public enum FormType
    {
        InputField,
        Dropdown,
        Toggle
    }
    FormType formType;

    Color initialColor;
    private void Awake()
    {
        base.Awake();
        if (GetComponent<InputField>())
        {
            formType = FormType.InputField;
            GetComponent<InputField>().onValueChanged.AddListener(delegate { InputFieldValueChanged(); });
            initialColor = GetComponent<InputField>().image.color;
        }
        else if (GetComponent<Dropdown>())
        {
            formType = FormType.Dropdown;
            GetComponent<Dropdown>().onValueChanged.AddListener(delegate { DropdownValueChanged(); });
            initialColor = GetComponent<Dropdown>().image.color;
        }
        else if (GetComponent<Toggle>())
        {
            formType = FormType.Toggle;
            GetComponent<Toggle>().onValueChanged.AddListener(delegate { ToggleValueChanged(); });
            initialColor = GetComponent<Toggle>().image.color;
        }
        else
        {
            Debug.LogError("Form type on object " + name + " Unsupported");
        }
        Reset();
    }


    private void InputFieldValueChanged()
    {
        GetComponent<InputField>().image.color = initialColor;
    }
    private void DropdownValueChanged()
    {
        Dropdown d = GetComponent<Dropdown>();
        if (d.value == 0)
        {
            return;
        }

        d.captionText.enabled = true;

        d.GetComponent<Image>().color = initialColor;
        if (d.captionText.text != d.options[d.value].text)
        {
            d.captionText.text = d.options[d.value].text;
        }
    }
    private void ToggleValueChanged()
    {
        transform.GetChild(0).GetComponent<Image>().color = initialColor;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool IsCompleted(ref string label, ref string completedValue, bool highlightIfNotCompleted = true)
    {
        label = gameObject.name;
        completedValue = null;
        switch (formType)
        {
            case FormType.InputField:
                HandleInputFieldForm(ref completedValue, highlightIfNotCompleted);
                break;
            case FormType.Dropdown:
                HandleDropdownFieldForm(ref completedValue, highlightIfNotCompleted);
                break;
            case FormType.Toggle:
                HandleToggleFieldForm(ref completedValue, highlightIfNotCompleted);
                break;
            default:
                break;
        }
        if (completedValue == null)
        {
            return false;
        }
        //for toggle
        if (completedValue == "False")
        {
            return false;
        }
        return true;
    }

    public void Reset()
    {
        switch (formType)
        {
            case FormType.InputField:
                GetComponent<InputField>().text = "";
                GetComponent<Image>().color = initialColor;
                break;
            case FormType.Dropdown:
                Dropdown d = GetComponent<Dropdown>();
                GetComponent<Image>().color = initialColor;
                d.value = 0;
                break;
            case FormType.Toggle:
                GetComponent<Toggle>().isOn = false;
                transform.GetChild(0).GetComponent<Image>().color = initialColor;
                break;
            default:
                break;
        }
    }

    private void HandleDropdownFieldForm(ref string completedValue, bool highlightIfNotCompleted = true)
    {
        Dropdown field = GetComponent<Dropdown>();
        if (field.value != 0)
        {
            if (field.captionText.text.Length != 0)
            {
                completedValue = field.captionText.text;
            }
        }
        else
        {
            if (highlightIfNotCompleted)
            {
                field.GetComponent<Image>().color = warningColor;
            }
        }
    }


    private void HandleInputFieldForm(ref string completedValue, bool highlightIfNotCompleted = true)
    {
        InputField field = GetComponent<InputField>();
        if (field.text.Length != 0)
        {
            completedValue = field.text;
        }
        else
        {
            if (highlightIfNotCompleted)
            {
                field.GetComponent<Image>().color = warningColor;
            }
        }
    }
    private void HandleToggleFieldForm(ref string completedValue, bool highlightIfNotCompleted = true)
    {
        Toggle field = GetComponent<Toggle>();
        if (field.isOn)
        {
            completedValue = "True";
        }
        else
        {
            completedValue = "False";
            if (highlightIfNotCompleted)
            {
                field.transform.GetChild(0).GetComponent<Image>().color = warningColor;
            }
        }
    }
}
