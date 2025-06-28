//using System.Diagnostics;
//using UnityEngine;
//using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

//public class ManualTextAssigner : MonoBehaviour
//{
//    [Header("Exactly 25 GameObjects with Text Components")]
//    public GameObject[] textObjects = new GameObject[25];

//    [Header("Exactly 25 Strings to Assign")]
//    public string[] targetTexts = new string[25];

//    [ContextMenu("Apply Texts")]
//    public void ApplyTexts()
//    {
//        if (textObjects.Length != 25 || targetTexts.Length != 25)
//        {
//            Debug.LogError("❌ You must have exactly 25 GameObjects and 25 strings.");
//            return;
//        }

//        for (int i = 0; i < 25; i++)
//        {
//            if (textObjects[i] == null)
//            {
//                Debug.LogWarning($"Text object at index {i} is missing.");
//                continue;
//            }

//            Text textComponent = textObjects[i].GetComponent<Text>();
//            if (textComponent == null)
//            {
//                Debug.LogWarning($"No Text component on GameObject at index {i}: {textObjects[i].name}");
//                continue;
//            }

//            textComponent.text = targetTexts[i];
//        }

//        Debug.Log("✅ Texts assigned successfully.");
//    }
//}
