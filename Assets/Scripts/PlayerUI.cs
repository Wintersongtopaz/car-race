using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    static Dictionary<string, List<Text>> textComponentMap = new Dictionary<string, List<Text>>();
    static Dictionary<string, List<Image>> imageComponentMap = new Dictionary<string, List<Image>>();
    // a text component with name key should display newText
    public static void SetText(string key, string newText)
    {
        if (textComponentMap.ContainsKey(key))
        {
            List<Text> l = textComponentMap[key];
            int i = 0;
            while(i < l.Count)
            {
                if (l[i])
                {
                    l[i].text = newText;
                    i++;
                }
                else
                {
                    l.RemoveAt(i);
                }
            }
        }
    }

    public static void SetImageFill(string key, float fill)
    {
        if (imageComponentMap.ContainsKey(key))
        {
            List<Image> l = imageComponentMap[key];
            int i = 0;
            while (i < l.Count)
            {
                if (l[i])
                {
                    l[i].fillAmount = fill;
                    i++;
                }
                else
                {
                    l.RemoveAt(i);
                }
            }
        }
    }

    private void Awake()
    {
        Text[] t = FindObjectsOfType<Text>();
        foreach (Text text in t) AddTextComponent(text);

        Image[] images = FindObjectsOfType<Image>();
        foreach (Image image in images) AddImageComponent(image);

    }
    // check if text componet matches key & update accordingly
    private void AddTextComponent(Text text)
    {
        string key = text.gameObject.name;
        if (textComponentMap.ContainsKey(key))
        {
            textComponentMap[key].Add(text);
        }
        else
        {
            textComponentMap.Add(key, new List<Text>());
            textComponentMap[key].Add(text);
        }
    }
    // adds image
    private void AddImageComponent(Image image)
    {
        string key = image.gameObject.name;
        if (imageComponentMap.ContainsKey(key))
        {
            imageComponentMap[key].Add(image);
        }
        else
        {
            imageComponentMap.Add(key, new List<Image>());
            imageComponentMap[key].Add(image);
        }
    }

}
