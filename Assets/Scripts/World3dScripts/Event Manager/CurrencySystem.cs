using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CurrencySystem : MonoBehaviour
{
    private static Dictionary<CurrencyType, int> CurrencyAmounts = 
        new Dictionary<CurrencyType, int>(); //Creamos una variable CurrencyAmount que es un diccionario, del tipo de currency (coin o gold) y la cantidad

    [SerializeField] private List<GameObject> texts; //Creo una lista de textos
   
    private Dictionary<CurrencyType, TextMeshProUGUI> currencyTexts = 
        new Dictionary<CurrencyType, TextMeshProUGUI>(); //Creamos una variable CurrencyAmount que es un diccionario, del tipo de currency (coin o gold) y el texto

    private void Awake() //Cuando arranque el juego va a pasar lo siguiente:
    {
        for (int i = 0; i < texts.Count; i++) //Para todos los gameobject dentro de la lista textos
        {
            CurrencyAmounts.Add((CurrencyType)i, 0); //Va a agregar un el tipo de currency y un valor puesto a mano que es 0
            currencyTexts.Add((CurrencyType)i, texts[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>()); //en el currency text va a poner el tecto de la lista
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener<CurrencyChangeGameEvent>(OnCurrencyChange);
        EventManager.Instance.AddListener<NotEnoughCurrencyGameEvent>(OnNotEnough); 
    }

    private void OnCurrencyChange(CurrencyChangeGameEvent info)
    {
        //To do save the currency
        CurrencyAmounts[info.currencyType] += info.amount;
        currencyTexts[info.currencyType].text = CurrencyAmounts[info.currencyType].ToString();
    }

    private void OnNotEnough(NotEnoughCurrencyGameEvent info) // Cuando no tenga más currency
    {
        Debug.Log(message: $"You don´t have enough of {info.amount} {info.currencyType}"); //Te tira el error, de la cantidad y del tipo
    }

}

public enum CurrencyType  //se crea una variable del tipo CurrencyType que está compuesta por ecocoins y ecogolds
{ 
    Ecocoins, Ecogold
}