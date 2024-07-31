using APIQueryMaster.Structure;
using UnityEngine;

namespace APIQueryMaster
{
    public static class APIGlobal
    {
        public static APISettings Settings { get; private set; }

        /// <summary>
        ///Callback invoked when the first scene's objects are loaded into memory but before Awake has been called.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeBeforeSceneLoad()
        {
            Initialize();
        }

        private static void Initialize()
        {
            APISettings[] settings = Resources.LoadAll<APISettings>(""); // All Setting From resources
            Settings = settings.Length > 0 ? Object.Instantiate(settings[0]) : null;
        }
    }
}