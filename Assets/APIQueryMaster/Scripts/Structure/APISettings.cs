using System;
using System.Collections.Generic;
using UnityEngine;

namespace APIQueryMaster.Structure
{
    [Serializable]
    [CreateAssetMenu(fileName = "APISettings", menuName = "APIQueryMaster/API Settings")]
    public class APISettings : ScriptableObject
    {
        [Header("Add base URL end with slash '/' so that endpoints can be attached to it")]
        [SerializeField] private string baseURL;
        [Header("Set Default max retries of API, can be override while creating query for every API")]
        [SerializeField] private int maxRetries;
        [Header("Set Default max queries processing at a samestime")]
        [Range(0, 100)]
        [SerializeField] private int maxQueryProcesses;
        [Header("Enable API debuging value to Debug Logs in console")]
        [SerializeField] private bool enableAPIdebug;
        [Header("Add default headers here ")]
        [SerializeField] private List<APIHeader> defaultHeaders;
        [Space]
        [Header("Queries")]
        [Header("Create all query templates here")]
        [SerializeField] private List<APIQuery> aPIQueries;

        public string BaseURL => baseURL;
        public int MaxRetries => maxRetries;
        public int MaxQueryProcesses => maxQueryProcesses;
        public bool EnableAPIdebug => enableAPIdebug;
        public List<APIHeader> DefaultHeaders => defaultHeaders;
        public List<APIQuery> APIQueries => aPIQueries;

        


    }

}
