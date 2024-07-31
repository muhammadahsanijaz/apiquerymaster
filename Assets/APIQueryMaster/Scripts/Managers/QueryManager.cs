using APIQueryMaster.Structure;
using System;
using UnityEngine;

namespace APIQueryMaster.Managers
{
    public class QueryManager : MonoBehaviour
    {
        // declaring API manager Object
        private APIManager APIManager;

        // declaring Failed callback
        public Action<string> APIFailedCallBack;

        #region Initialization

        private void Start()
        {
            APIManager = new APIManager(this);
        }

        private void OnDestroy()
        {
            APIManager.ClearQueue();
        }
        #endregion

        #region Queries

        public void GetEmployeeAPI(int id, Action<string> ResponseText)
        {
            // Create new API Query Object
            var GetEmployeeQuery = new APIQuery(QueryName.DummyGetEmployee);
            // Create Send Data Object and pass it to Set Query Data with
            // Value false as query does need to send names with but only values
            var sendData = new EmployeeSendData { employeeId = id };
            GetEmployeeQuery.SetQueryData(sendData, false);
            // Register succcess callback with data return type
            GetEmployeeQuery.SetSuccessCallBack(OnResponse);
            // Register API Failed callback
            GetEmployeeQuery.SetFailCallBack(OnAPIFailed);
            // Finally, Enqueue the query to the API manager
            APIManager.EnqueueQuery(GetEmployeeQuery);

            // Response callback process function
            void OnResponse(string data)
            {
                ResponseText.Invoke(data);
            }
        }

        public void CreateEmployeeAPI(CreateEmployeeData sendData, Action<string> ResponseText)
        {
            // Create new API Query Object
            var GetEmployeeQuery = new APIQuery(QueryName.DummyCreateEmployee);
            // Sending data to form data
            GetEmployeeQuery.SetFormData(sendData);
            // Register succcess callback with data return type
            GetEmployeeQuery.SetSuccessCallBack(OnResponse);
            // Register API Failed callback
            GetEmployeeQuery.SetFailCallBack(OnAPIFailed);
            // Finally, Enqueue the query to the API manager
            APIManager.EnqueueQuery(GetEmployeeQuery);

            // Response callback process function
            void OnResponse(string data)
            {
                ResponseText.Invoke(data);
            }
        }

        public void DeleteEmployeeAPI(int id, Action<string> ResponseText)
        {
            // Create new API Query Object
            var GetEmployeeQuery = new APIQuery(QueryName.DummyDeleteEmployee);
            // Create Send Data Object and pass it to Set Query Data with
            // Value false as query does need to send names with but only values
            var sendData = new EmployeeSendData { employeeId = id };
            GetEmployeeQuery.SetQueryData(sendData, false);
            // Register succcess callback with data return type
            GetEmployeeQuery.SetSuccessCallBack(OnResponse);
            // Register API Failed callback
            GetEmployeeQuery.SetFailCallBack(OnAPIFailed);
            // Finally, Enqueue the query to the API manager
            APIManager.EnqueueQuery(GetEmployeeQuery);

            // Response callback process function
            void OnResponse(string data)
            {
                ResponseText.Invoke(data);
            }
        }

        #endregion

        #region APIFailed

        private void OnAPIFailed(string Error)
        {
            // Show Dialog Here
            if (APIGlobal.Settings.EnableAPIdebug)
            {
                Debug.LogError("APIQueryMaster : API Failed \n " + Error);
            }
            APIFailedCallBack?.Invoke(Error);
        }

        #endregion

    }

}