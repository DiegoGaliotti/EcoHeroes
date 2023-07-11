using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencySystem : MonoBehaviour
{
   private static Dictionary<CurrencyType, int> currencyAmount = new Dictionary<CurrencyType, int>();

    [SerializeField] private List<GameObject> text;
    
    private Dictionary<CurrencyType, TextMeshProUGUI> currencyText = new Dictionary<CurrencyType, TextMeshProUGUI>();
    



}

public enum CurrencyType  //se crea una variable del tipo CurrencyType que está compuesta por ecocoins y ecogolds
{ 
    Ecocoins, Ecogold
}