using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Import TextMeshPro namespace

public class DiceNumberTextScript : MonoBehaviour
{
    public TMP_Text text; // Use TMP_Text instead of Text
    public static int? diceNumber = null; // Use nullable int (int?)

    void Update()
    {
        if (text != null) // Avoid NullReferenceException
        {
            // Show empty text if diceNumber is null
            text.text = diceNumber.HasValue ? diceNumber.Value.ToString() : "";
        }
    }
}
