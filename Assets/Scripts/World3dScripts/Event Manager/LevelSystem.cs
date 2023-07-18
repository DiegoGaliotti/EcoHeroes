using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelSystem : MonoBehaviour
{
    private int XPNow; //El XP que tienes en el momento -- BBDD SAVE --
    private int Level; //El nivel en el que estas  -- BBDD SAVE --
    private int xpToNext; //El XP que te falta para pasar de nivel

    [SerializeField] private GameObject canvas; //crea un panel de nivel del tipo GameObject(probablemente la va a usar para asociarle un game object que va a ser donde aparece el nivel)
    [SerializeField] private GameObject lvlWindowPrefab; //crea una ventana de nivel del tipo Gamebject (seguro la usa para que aparezca una ventana del tipo prefab con un cambio de nivel)

    private Slider slider; //UI Slider
    private TextMeshProUGUI xpText; //UI XP Text
    private TextMeshProUGUI lvlText; // UI LEVEL TEXT
    private Image lvlImage; // UI strella

    private static bool initialized;
    private static Dictionary<int, int> xpToNextLevel = new Dictionary<int, int>(); // Cuando pasar de nivel
    private static Dictionary<int, int[]> lvlReward = new Dictionary<int, int[]>(); // Que te da cuando pasas de nivel

    private void Awake() 
    {
        slider = canvas.transform.Find("Slider").GetComponent<Slider>();
        xpText = slider.transform.Find("XP Text").GetComponent<TextMeshProUGUI>();
        lvlImage = canvas.transform.Find("LevelImage").GetComponent<Image>(); 
        lvlText = lvlImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 

        if (!initialized)
        {
            Inicialize();
        }

        xpToNextLevel.TryGetValue(Level, out xpToNext); 
    }

    private void Start()
    {
        EventManager.Instance.AddListener<XPAddedGameEvent>(OnXPAdded);
        EventManager.Instance.AddListener<LevelChangedGameEvent>(OnLevelChanged);
        
        UpdateUI();
    }

    private static void Inicialize()
    {
        try
        {
            string path = "levelXP"; //Cargar la ruta al directorio

            TextAsset textAsset = Resources.Load<TextAsset>(path);
            string[] lines = textAsset.text.Split('\n');
                
            xpToNextLevel = new Dictionary<int, int>(capacity: lines.Length - 1);

            for(int i = 1; i < lines.Length -1; i++)
            {
                string[] columns = lines[i].Split(';');

                int lvl = -1;
                int xp = -1;
                int Ecocoin = -1;
                int Ecogold = -1;

                int.TryParse(columns[0], out lvl);
                int.TryParse(columns[1], out xp);
                int.TryParse(columns[2], out Ecocoin);
                int.TryParse(columns[3], out Ecogold);

                if (lvl >= 0 && xp > 0)
                {
                    if (!xpToNextLevel.ContainsKey(lvl))
                    {
                        xpToNextLevel.Add(lvl, xp);
                        lvlReward.Add(lvl, new []{Ecocoin, Ecogold});
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);        
        }

        initialized = true;
    }

    private void UpdateUI()
    {
        float fill = (float)XPNow / xpToNext; 
        slider.value = fill;
        xpText.text = XPNow + "/" + xpToNext;
    }

    private void OnXPAdded(XPAddedGameEvent info)
    {
        XPNow += info.amount;

        UpdateUI();

        if (XPNow >= xpToNext)
        {
            Level++;
            LevelChangedGameEvent levelChange = new LevelChangedGameEvent(Level);
            EventManager.Instance.QueueEvent(levelChange);
        }
    }

    private void OnLevelChanged(LevelChangedGameEvent info)
    {
        XPNow -= xpToNext; //Ac? te deja en el XP now el valor de lo que se pas?. 
        xpToNext = xpToNextLevel[info.newLvl]; // Ac? busca el valor del diccionario y lo agrega en el valor para pasar. 
        lvlText.text = (info.newLvl + 1).ToString(); // Te actualiza la UI
        UpdateUI(); // Actualiza la UI

        GameObject window = Instantiate(lvlWindowPrefab, GameManager.current.canvas.transform); //Instancia algo

        //initialize text and images here

        window.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate
        {
            Destroy(window);
        });

        CurrencyChangeGameEvent currencyInfo =
            new CurrencyChangeGameEvent(lvlReward[info.newLvl][0], CurrencyType.Ecocoin);
        EventManager.Instance.QueueEvent(currencyInfo);

        currencyInfo =
            new CurrencyChangeGameEvent(lvlReward[info.newLvl][1], CurrencyType.Ecogold);
        EventManager.Instance.QueueEvent(currencyInfo);

    }

}
