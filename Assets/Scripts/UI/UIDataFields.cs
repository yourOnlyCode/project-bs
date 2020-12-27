using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIDataFields : MonoBehaviour
{

    [SerializeField] private List<TMP_Text> _textFields;

    public List<TMP_Text> GetTextFields()
    {
        return _textFields;
    }

}
