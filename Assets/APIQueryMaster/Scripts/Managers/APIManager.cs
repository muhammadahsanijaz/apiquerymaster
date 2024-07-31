using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Text;
using APIQueryMaster.Structure;

namespace APIQueryMaster.Managers
{
    /// <summary>
    /// API Manager : handle API calls process with prority Queue system and return data with callbacks
    /// </summary>
    public class APIManager
    {
        // CallBackActions

        private int currentQueryProcesses = 0; // Flag to track if a query is currently being processed

        private readonly PriorityQueue<APIQuery, int> queryQueue; 
        private readonly MonoBehaviour manager;

        /// <summary>
        /// API manager Constructor 
        /// </summary>
        /// <param name="queryManager"></param>
        public APIManager(MonoBehaviour manager)
        {
            this.manager = manager;
            queryQueue = new PriorityQueue<APIQuery, int>();
            ClearQueue();
        }


        #region Queue
        /// <summary>
        /// Clear Queue : Clear the queue when you need to discard all the remaining API
        /// </summary>
        public void ClearQueue()
        {
            queryQueue.Clear();
        }

        /// <summary>
        /// Enqueue Query : Enqueue the API query to the prority queue which will be process at its turn
        /// </summary>
        /// <param name="query"></param>
        public void EnqueueQuery(APIQuery query)
        {
            queryQueue.Enqueue(query, query.Priority);
            if (currentQueryProcesses < APIGlobal.Settings.MaxQueryProcesses)
            {
                ProcessNextQuery(); // Start processing the query if no query is currently being processed
            }
        }

        /// <summary>
        /// Processing Next Query when one gets finished
        /// </summary>
        private void ProcessNextQuery()
        {
            if (queryQueue.Count > 0)
            {
                currentQueryProcesses++; // Set the flag to indicate that a query is being processed
                APIQuery query = queryQueue.Dequeue();
                if (APIGlobal.Settings.EnableAPIdebug)
                {
                    Debug.Log($"Processing {query.GetCompleteURL()}  \n {query.QueryType}");
                }
                manager.StartCoroutine(SendRequest(query, OnResponse));
            }
        }

        /// <summary>
        /// Query Callback response and parsing it and then invoke to response data
        /// </summary>
        /// <param name="query"></param>
        /// <param name="downloadedData"></param>
        void OnResponse(APIQuery query, string downloadedData)
        {
            currentQueryProcesses--; // Reset the flag after processing is complete
            query.RecieveDataCallback?.Invoke(downloadedData);
            // Starting to process next query
            ProcessNextQuery();
        }

        #endregion

        #region Generic SendRequest
        /// <summary>
        /// Make Send Request Coroutine Will take API query and process it
        /// </summary>
        /// <param name="APIQuery"></param>
        /// <param name="actionToInvoke"></param>
        /// <returns></returns>
        private IEnumerator SendRequest(APIQuery APIQuery, Action<APIQuery, string> actionToInvoke)
        {
            using (UnityWebRequest request = new UnityWebRequest(APIQuery.GetCompleteURL().AbsoluteUri, APIQuery.QueryType.ToString()))
            {
                if (APIQuery.FormData != null && APIQuery.IsSendingDataByForm)
                {
                    byte[] bodyRaw = Encoding.UTF8.GetBytes(APIQuery.Body);
                    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                    request.SetRequestHeader("Content-Type", "application/json");
                    foreach (var header in APIQuery.Headers)
                    {
                        request.SetRequestHeader(header.headerName,header.headerName);
                    }
                    if (APIGlobal.Settings.EnableAPIdebug)
                    {
                        Debug.LogError($"Body {APIQuery.GetCompleteURL()}  \n {APIQuery.Body}");
                    }
                }

                request.downloadHandler = new DownloadHandlerBuffer();
                if (APIQuery.IsTimeoutSet)
                {
                    request.timeout = APIQuery.Timeout;
                }
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string responseBody = request.downloadHandler.text;
                    if (APIGlobal.Settings.EnableAPIdebug)
                    {
                        Debug.LogError($" Request to {APIQuery.GetCompleteURL()} - {APIQuery.QueryType}was successful\n Response: {responseBody}");
                    }
                    actionToInvoke?.Invoke(APIQuery, responseBody);
                }
                else
                {
                    string responseError = (!string.IsNullOrEmpty(request.downloadHandler.error))? request.downloadHandler.error : request.downloadHandler.text;
                    if (APIGlobal.Settings.EnableAPIdebug)
                    {
                        Debug.LogError($"{APIQuery.QueryType}  Request to {APIQuery.GetCompleteURL()} \n failed with status code: {request.responseCode}");

                    }

                    APIQuery.IncrementRetries();
                    if (APIQuery.Retries < APIQuery.AvailableRetries)
                    {
                        manager.StartCoroutine(SendRequest(APIQuery, OnResponse));
                    }
                    else
                    {
                        APIQuery.OnFailedCallback?.Invoke(responseError);
                        currentQueryProcesses--;
                    }
                }
            }


        }
        #endregion

    }


}
