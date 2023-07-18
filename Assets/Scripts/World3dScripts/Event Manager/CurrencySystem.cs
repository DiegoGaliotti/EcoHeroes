using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum CurrencyType  //se crea una variable del tipo CurrencyType que está compuesta por ecocoins y ecogolds
{
    Ecocoin, Ecogold
}
public class CurrencySystem : MonoBehaviour
{
    private static Dictionary<CurrencyType, int> CurrencyAmounts = 
    new Dictionary<CurrencyType, int>(); //Creamos una variable CurrencyAmount que es un diccionario, del tipo de currency (coin o gold) y la cantidad

    [SerializeField] private List<GameObject> texts; //Creo una lista de GameObjects
   
    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = 
        new Dictionary<CurrencyType, TextMeshProUGUI>(); //Creamos una variable CurrencyAmount que es un diccionario, del tipo de currency (coin o gold) y el texto

    private void Awake() //Cuando arranque el juego va a pasar lo siguiente:
    {
        for (int i = 0; i < texts.Count; i++) //Para todos los gameobject dentro de la lista textos
        {
            CurrencyAmounts.Add((CurrencyType)i, 0); //Va a agregar un el tipo de currency y un valor puesto a mano que es 0
            currencyTexts.Add((CurrencyType)i, texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>()); //Va a cargar en el dic la currency con el textpro del hijo del gameobject que le pones a la lista de text
        }
    }
    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough); 
    }
    private void OnCurrencyChange(CurrencyChangeGameEvent info)
    {
        //Todo save the currency
        CurrencyAmounts[info.currencyType] += info.amount;
        currencyTexts[info.currencyType].text = CurrencyAmounts[info.currencyType].ToString();
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info) // Cuando no tenga más currency
    {
        Debug.Log(message: $"You don´t have enough of {info.amount} {info.currencyType}"); //Te tira el error, de la cantidad y del tipo
    }

}

