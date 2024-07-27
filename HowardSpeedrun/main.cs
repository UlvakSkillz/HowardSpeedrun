using HarmonyLib;
using MelonLoader;
using Il2CppRUMBLE.Environment.Howard;
using RumbleModdingAPI;
using System.Diagnostics;
using Il2CppTMPro;
using UnityEngine;

namespace HowardSpeedrun
{
    public class main : MelonMod
    {
        [HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Howard.Howard), "OnActivationLeverChanged")]
        public static class Patch0
        {
            private static void Postfix(int step)
            {
                if (step == 0)
                {
                    howardActive = true;
                    howard.currentHp = howard.CurrentSelectedLogic.maxHealth;
                    howard.UpdateHealthBarPercentage(100, false);
                    timer.Restart();
                }
                else if (step == 1)
                {
                    howardActive = false;
                    timer.Stop();
                    timerTimeComponent.text = timer.Elapsed.ToString();
                }
            }
        }

        [HarmonyPatch(typeof(Il2CppRUMBLE.Environment.Howard.Howard), "DealDamage")]
        public static class Patch1 { private static void Postfix() { if (howard.currentHp <= 0) { howard.OnActivationLeverChanged(1); } } }

        private string currentScene = "Loader";
        private bool sceneChanged = false;
        private bool loaded = false;
        private GameObject timerGameObject, timerTextStatic, timerTime;
        private static TextMeshPro timerTextComponent, timerTimeComponent;
        private static Howard howard;
        private static bool howardActive = false;
        private static Stopwatch timer = new Stopwatch();

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            currentScene = sceneName;
            sceneChanged = true;
            howardActive = false;
        }

        public override void OnFixedUpdate()
        {
            if (currentScene != "Gym") { return; }
            if (sceneChanged)
            {
                try
                {
                    howard = Calls.GameObjects.Gym.Logic.HeinhouserProducts.HowardRoot.GetGameObject().GetComponent<Howard>();
                    timerGameObject = new GameObject();
                    timerGameObject.name = "HowardTimer";
                    timerTextStatic = GameObject.Instantiate(Calls.GameObjects.Gym.Logic.HeinhouserProducts.Leaderboard.PlayerTags.HighscoreTag0.Nr.GetGameObject());
                    timerTextStatic.transform.parent = timerGameObject.transform;
                    timerTextStatic.name = "TimerText";
                    timerTextComponent = timerTextStatic.GetComponent<TextMeshPro>();
                    timerTextComponent.text = "Time: ";
                    timerTextComponent.fontSize = 5f;
                    timerTextComponent.color = new Color(0, 0, 0, 1);
                    timerTextComponent.outlineColor = new Color(0, 0, 0, 0.5f);
                    timerTextComponent.alignment = TextAlignmentOptions.Left;
                    timerTextComponent.enableWordWrapping = false;
                    timerTextComponent.outlineWidth = 0.1f;
                    timerTime = GameObject.Instantiate(timerTextStatic);
                    timerTime.transform.parent = timerGameObject.transform;
                    timerTime.name = "TimerTime";
                    timerTimeComponent = timerTime.GetComponent<TextMeshPro>();
                    timerTimeComponent.text = timer.Elapsed.ToString();
                    timerTextStatic.transform.position = new Vector3(-2f, 1f, 0);
                    timerTime.transform.position = new Vector3(-1f, 1f, 0);
                    timerGameObject.transform.position = new Vector3(10.5091f, -0.9382f, -22.3673f);
                    timerGameObject.transform.rotation = Quaternion.Euler(0, 133.5728f, 0);
                    loaded = true;
                    sceneChanged = false;
                } catch { return; }
            }
            if (loaded && howardActive) { timerTimeComponent.text = timer.Elapsed.ToString(); }
        }
    }
}
