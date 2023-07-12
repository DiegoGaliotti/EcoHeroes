using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    private int XPNow; //El XP que tienes en el momento
    private int Level; //El nivel en el que estas 
    private int xpToNext; //El XP que te falta para pasar de nivel

    [SerializeField] private GameObject levelPanel; //crea un panel de nivel del tipo GameObject(probablemente la va a usar para asociarle un game object que va a ser donde aparece el nivel)
    [SerializeField] private GameObject lvlWindowPrefab; //crea una ventana de nivel del tipo Gamebject (seguro la usa para que aparezca una ventana del tipo prefab con un cambio de nivel)

    private Slider slider;
    private TextMeshProUGUI xpText;
    private TextMeshProUGUI lvlText;
    private Image starImage;




}
