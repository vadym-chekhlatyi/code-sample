using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Event = Modules.CustomEvent;

namespace DigimightEngine
{
    public class BoostersController : MonoBehaviour
    {
        static BoostersController instance;
        public static BoostersController Instance
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
        List<Event> events;
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

        List<string> activeBoosters = new List<string>();
        public List<string> ActiveBoosters
        {
            get
            {
                return activeBoosters;
            }
            set
            {
                activeBoosters = value;
            }
        }

        public void Initialize()
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public static void SwitchBooster(string name, int value)
        {
            if (!Instance.ActiveBoosters.Contains(name) && value > 0)
            {
                Instance.ActiveBoosters.Add(name);

                foreach(BoosterItem booster in FindObjectsOfType<BoosterItem>())
                {
                    if(booster.Name == name)
                    {
                        booster.ActivateBooster();
                        Debug.Log(name + ": " + value);
                    }
                }

                InvokeEvent("Activate Booster");
            }
            else
            {
                Instance.ActiveBoosters.Remove(name);

                foreach (BoosterItem booster in FindObjectsOfType<BoosterItem>())
                {
                    if (booster.Name == name)
                    {
                        booster.DeactivateBooster();
                        Debug.Log(name + ": " + value);
                    }
                }

                InvokeEvent("Deactivate Booster");
            }
        }

        public void EnableBoosters(List<GameObject> boosterList, UnityEngine.Events.UnityAction action)
        {
            StartCoroutine(ApplyBoosters(boosterList, action));
        }

        private IEnumerator ApplyBoosters(List<GameObject> boosterList, UnityEngine.Events.UnityAction action)
        {
            yield return new WaitForSeconds(2f);
            for (int i = 0; i < boosterList.Count; i++)
            {
                if (ActiveBoosters.Contains(boosterList[i].name))
                {

                    boosterList[i].SetActive(true);

                    int decreasedValue = Manipulation.ObjectToInt(Player.GetObjectValueFieldWithName(boosterList[i].name, "Booster")) - 1;

                    Player.SetValueFieldWithName(boosterList[i].name, "Booster", decreasedValue);

                    GameAnalyticsSDK.GameAnalytics.NewResourceEvent(GameAnalyticsSDK.GAResourceFlowType.Sink, boosterList[i].name, 1, "Products", "-");

                    yield return new WaitForSeconds(6f);
                }
            }

            ActiveBoosters.Clear();

            action.Invoke();
        }

        private void CheckBooster(GameObject booster, bool result)
        {
            if (!booster.activeInHierarchy)
            {
                result = true;
            }
        }

        public static void ChangeValue(BoosterItem booster, int value)
        {
            Player.SetValueFieldWithName(booster.Name, "Booster", booster.BoosterValue + value);
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
    }
}