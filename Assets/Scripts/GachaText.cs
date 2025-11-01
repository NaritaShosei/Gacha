using TMPro;
using UnityEngine;
using static GachaSystem;

public class GachaText : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    public void Initialize(GachaItem item)
    {
        _text.text = item.ItemName;
    }
}
