using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Modules.CustomEvent;

namespace DigimightEngine
{
    public class BoosterItem : MonoBehaviour
    {
        static BoosterItem instance;
        public static BoosterItem Instance
        {
            get
            {
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        [Space]
        [SerializeField]
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        private bool opened;
        public bool Opened
        {
            get
            {
                return opened;
            }
            set
            {
                opened = value;
            }
        }

        [SerializeField]
        private int levelToUnlock;
        public int LevelToUnlock
        {
            get
            {
                return levelToUnlock;
            }
            set
            {
                levelToUnlock = value;
            }
        }

        private int boosterValue;
        public int BoosterValue
        {
            get
            {
                return boosterValue;
            }
            set
            {
                boosterValue = value;
            }
        }

        [Space]
        [SerializeField]
        private int boosterPrice;
        public int BoosterPrice
        {
            get
            {
                return boosterPrice;
            }
            set
            {
                boosterPrice = value;
            }
        }

        [Space]
        [SerializeField]
        private List<GameObject> stages = new List<GameObject>();
        public List<GameObject> Stages
        {
            get
            {
                return stages;
            }
            set
            {
                stages = value;
            }
        }

        [Space]
        [SerializeField]
        private List<Event> events;
        public List<Event> Events
        {
            get
            {
                return events;
            }
            set
            {
                events = value;
            }
        }

        private void Awake()
        {
            Instance = this;
            BoosterValue = Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(Name, "Booster"));

            if(Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName("Level", "Progress")) < LevelToUnlock)
            {
                Stages[0].SetActive(true); // Locked state
                gameObject.SetActive(false);
            }
            else
            {
                Stages[0].SetActive(false); // Locked state
                Stages[1].SetActive(Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(Name, "Booster")) <= 0);  // Price state
                Stages[2].SetActive(Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(Name, "Booster")) > 0);  // Count state
            }
        }

        public void ActivateBooster()
        {
            Stages[1].SetActive(false); // Price state
            Stages[2].SetActive(false); // Count state
            Stages[3].SetActive(true);  // Active state
        }

        public void DeactivateBooster()
        {
            Stages[1].SetActive(Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(Name, "Booster")) <= 0);  // Price state
            Stages[2].SetActive(Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(Name, "Booster")) > 0);  // Count state
            Stages[3].SetActive(false); // Active state
        }

        public void RequestBuy(bool result)
        {
            if (!result)
            {
                int coins = Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName("Currency Type 1", "Currencies"));

                if(coins >= BoosterPrice)
                {
                    BuyBooster(true, Name, BoosterPrice);
                }
            }
        }

        public void BuyBooster(bool result, string Name, int BoosterPrice)
        {
            if (result)
            {
                GameAnalyticsSDK.GameAnalytics.NewResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, "Coins", BoosterPrice, "Products", "-");
                GameAnalyticsSDK.GameAnalytics.NewResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Source, Name, 1, "Products", "-");

                int coinsLeft = Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName("Currency Type 1", "Currencies")) - BoosterPrice;

                Player.SetValueFieldWithName("Currency Type 1", "Currencies", coinsLeft);

                BoosterValue++;

                Player.SetValueFieldWithName(Name, "Booster", BoosterValue);

                BoostersController.SwitchBooster(Name, BoosterValue);

                InvokeEvent("Actualize Booster");
            }
        }

        public void RequestFail()
        {
            InvokeEvent("Request Fail");
        }

        public static void InvokeEvent(string name)
        {
            for (int i = 0; i < Instance?.Events.Count; i++)
            {
                if (Instance?.Events[i].Name == name)
                {
                    Instance?.Events[i].Invoke();

                    return;
                }
            }
        }

        public static void InvokeIf(bool result, string name)
        {
            if (result)
            {
                for (int i = 0; i < Instance?.Events.Count; i++)
                {
                    if (Instance?.Events[i].Name == name)
                    {
                        Instance?.Events[i].Invoke();

                        return;
                    }
                }
            }
        }

        public static void InvokeElse(bool result, string name)
        {
            if (!result)
            {
                for (int i = 0; i < Instance?.Events.Count; i++)
                {
                    if (Instance?.Events[i].Name == name)
                    {
                        Instance?.Events[i].Invoke();

                        return;
                    }
                }
            }
        }
    }
}
