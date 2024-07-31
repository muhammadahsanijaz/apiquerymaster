using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;


namespace APIQueryMaster.Structure
{
    /// <summary>
    /// All HTTP Method type
    /// </summary>
    public enum QueryType
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD,
        PATCH,
        TRACE,
        OPTIONS,
        CONNECT,
    }

    /// <summary>
    /// Data structure using get header from Inpector and feed to the request in API manager
    /// </summary>
    [Serializable]
    public class APIHeader
    {
        public string headerName;
        public string headerValue;
    }

    /// <summary>
    /// API Query: This is a DataStructure class designed to organize the sending and receiving of data before posting it to the APIManager.
    /// </summary>
    [Serializable]
    public class APIQuery
    {
        [Tooltip("Name of query : Add this in the Query Name Script")]
        [SerializeField] private QueryName queryName;
        [Tooltip("Select type of the HTTP Method")]
        [SerializeField] private QueryType queryType;
        [Tooltip("Add The endpoint without start slash")]
        [SerializeField] private string endPoint;
        [Tooltip("Set default prority with accending order of execution")]
        [SerializeField] private int defaultPriority;

        internal string BaseURL { get; private set; }
        internal bool IsSendingDataByQuery { get; private set; }
        internal bool IncludeNameForQuery { get; private set; }
        internal object QueryData { get; private set; }
        internal bool IsSendingDataByForm { get; private set; }
        internal object FormData { get; private set; }
        internal Action<string> RecieveDataCallback { get; private set; }
        internal Action<string> OnFailedCallback { get; private set; }
        internal List<APIHeader> Headers { get; private set; }
        internal int Retries { get; private set; }
        internal int AvailableRetries { get; private set; }
        internal int Priority { get; private set; }
        internal int Timeout { get; private set; }
        internal bool IsTimeoutSet { get; private set; }

        public QueryName QueryName => queryName;
        public QueryType QueryType => queryType;
        public string EndPoint => endPoint;
        public int DefaultPriority => defaultPriority;

        /// <summary>
        /// Basic consturctor to initialize API Query with default values
        /// </summary>
        /// <param name="queryName"> Get URL Query Name </param>
        /// 

        public APIQuery(QueryName queryName)
        {
            var urlQueryTemplate = APIGlobal.Settings.APIQueries.Find(x => x.QueryName == queryName);

            this.queryName = urlQueryTemplate.queryName;
            endPoint = urlQueryTemplate.endPoint;
            queryType = urlQueryTemplate.queryType;
            Priority = urlQueryTemplate.defaultPriority;
            BaseURL = APIGlobal.Settings.BaseURL;
            AvailableRetries = APIGlobal.Settings.MaxRetries;
            Headers = APIGlobal.Settings.DefaultHeaders;
            Retries = 0;
        }

        /// <summary>
        /// Override Consturctor to initailize API Query with custom values
        /// Use this to send custom API with custom Base URL and endpoint
        /// Note : You have to initailize Headers too as Default headers not include in this
        /// </summary>
        /// <param name="URL"></param>
        /// <param name="queryType"></param>
        public APIQuery(QueryType queryType, string URL, string endPoint = "", int priority = 0)
        {
            this.queryType = queryType;
            BaseURL = URL;
            this.endPoint = endPoint;
            AvailableRetries = 1;
            Retries = 0;
            Priority = priority;
        }


        /// <summary>
        /// Get Complete URL : It will return base url attached with end point and add query params if sendData by query is set.
        /// </summary>
        /// <returns></returns>
        public virtual Uri GetCompleteURL()
        {
            UriBuilder uriBuilder = new UriBuilder(BaseURL + endPoint);
            if (IsSendingDataByQuery)
            {
                var queryString = BuildQueryString();
                if (IncludeNameForQuery)
                {
                    // Encode query parameters
                    uriBuilder.Query = queryString;
                }
                else
                {
                    uriBuilder = new UriBuilder(uriBuilder.Uri + "/" + queryString);
                }
            }
            // Get the final URI
            Uri uri = uriBuilder.Uri;
            return uri;
        }

        internal string Body => JsonConvert.SerializeObject(FormData);

        /// <summary>
        /// Set Query Data : Assign to add params in the Query
        /// </summary>
        /// <param name="QuerySendData">object of params</param>
        /// <param name="IncludeName">Default : true, Use if params aren't meant to include name in query</param>
        public void SetQueryData(object QuerySendData, bool IncludeName = true)
        {
            this.QueryData = QuerySendData;
            IsSendingDataByQuery = true;
            IncludeNameForQuery = IncludeName;
        }

        /// <summary>
        /// Set Query Data : Assign to add params in the Form/Body
        /// </summary>
        /// <param name="FormSendData">object of params</param>
        public void SetFormData(object FormSendData)
        {
            this.FormData = FormSendData;
            IsSendingDataByForm = true;
        }

        /// <summary>
        /// Set Recieve Action callback with custom dataReturnType to get proper deserialized element ready to use
        /// </summary>
        /// <param name="recieveDataCallback">Action Callback to invoke with data</param>
        /// <param name="dataReturnType">Returntype of data to be deserialized</param>
        public void AddHeader(string headerName, string headerValue)
        {
            this.Headers.Add(new APIHeader { headerName = headerName, headerValue = headerValue });
        }

        /// <summary>
        /// Set Recieve Action callback : Register Success Callback to process data afterwards
        /// </summary>
        /// <param name="recieveDataCallback">Action Callback to invoke with data</param>
        public void SetSuccessCallBack(Action<string> recieveDataCallback)
        {
            this.RecieveDataCallback += recieveDataCallback;
        }

        /// <summary>
        /// Set Fail API Callback : Register Action Callback to invoke error message
        /// </summary>
        /// <param name="failedCallback">Action Callback to invoke with error message</param>
        public void SetFailCallBack(Action<string> failedCallback)
        {
            this.OnFailedCallback += failedCallback;
        }

        /// <summary>
        /// Set Timeout : Set API timeout if get response takes too long
        /// </summary>
        /// <param name="timeout">timeout value</param>
        public void SetTimeOut(int timeout)
        {
            Timeout = timeout;
            IsTimeoutSet = true;
        }

        /// <summary>
        /// Set Retries : Set API retires how much we will have to retires if API get failed
        /// </summary>
        /// <param name="retries">retries value</param>
        public void SetRetries(int retries)
        {
            AvailableRetries = retries;
        }

        internal void IncrementRetries()
        {
            Retries++;
        }

        private string BuildQueryString()
        {
            var properties = QueryData.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var queryParameters = properties
                .Select(p => (IncludeNameForQuery ? UnityWebRequest.EscapeURL(p.Name) + "=" : "") + UnityWebRequest.EscapeURL(p.GetValue(QueryData)?.ToString() ?? string.Empty));


            return queryParameters.Any() ? string.Join((IncludeNameForQuery)?"/" : "&", queryParameters) : string.Empty;
        }
    }

}