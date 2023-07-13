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

    [SerializeField] private GameObject levelPanel; //crea un panel de nivel del tipo GameObject(probablemente la va a usar para asociarle un game object que va a ser donde aparece el nivel)
    [SerializeField] private GameObject lvlWindowPrefab; //crea una ventana de nivel del tipo Gamebject (seguro la usa para que aparezca una ventana del tipo prefab con un cambio de nivel)

    private Slider slider; //UI Slider
    private TextMeshProUGUI xpText; //UI XP Text
    private TextMeshProUGUI lvlText; // UI LEVEL TEXT
    private Image starImage; // UI strella

    private static bool initialized;
    private static Dictionary<int, int> xpToNextLevel = new Dictionary<int, int>(); // Cuando pasar de nivel
    private static Dictionary<int, int[]> lvlReward = new Dictionary<int, int[]>(); // Que te da cuando pasas de nivel

    private void Awake() 
    {
        slider = levelPanel.GetComponent<Slider>();
        xpText = levelPanel.transform.Find("XP Text").GetComponent<TextMeshProUGUI>(); 
        starImage = levelPanel.transform.Find("Star").GetComponent<Image>(); 
        lvlText = starImage.transform.GetChild(0).GetComponent<TextMeshProUGUI>(); 

        if(!initialized)
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
                string[] columns = lines[i].Split(',');

                int lvl = -1;
                int xp = -1;
                int curr1 = -1;
                int curr2 = -1;

                int.TryParse(columns[0], out lvl);
                int.TryParse(columns[1], out xp);
                int.TryParse(columns[2], out curr1);
                int.TryParse(columns[3], out curr2);

                if (lvl >= 0 && xp > 0)
                {
                    if (!xpToNextLevel.ContainsKey(lvl))
                    {
                        xpToNextLevel.Add(lvl, xp);
                        lvlReward.Add(lvl, new []{curr1, curr2});
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
        XPNow -= xpToNext; //Acá te deja en el XP now el valor de lo que se pasó. 
        xpToNext = xpToNextLevel[info.newLvl]; // Acá busca el valor del diccionario y lo agrega en el valor para pasar. 
        lvlText.text = (info.newLvl + 1).ToString(); // Te actualiza la UI
        UpdateUI(); // Actualiza la UI

        GameObject window = Instantiate(lvlWindowPrefab, GameManager.current.canvas.transform); //Instancia algo

        //initialize text and images here

        window.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(call: delegate
        {
            Destroy(window);
        });

        CurrencyChangeGameEvent currencyInfo =
            new CurrencyChangeGameEvent(lvlReward[info.newLvl][0], CurrencyType.Ecocoins);
        EventManager.Instance.QueueEvent(currencyInfo);

        currencyInfo =
            new CurrencyChangeGameEvent(lvlReward[info.newLvl][1], CurrencyType.Ecogold);
        EventManager.Instance.QueueEvent(currencyInfo);

    }

}
