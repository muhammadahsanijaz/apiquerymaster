using APIQueryMaster.Managers;
using APIQueryMaster.Structure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace APIQueryMaster.Demo
{
    public class Demo : MonoBehaviour
    {
        [Header("GetEmployee")]
        [SerializeField] private Button GetEmployeeButton;
        [SerializeField] private GameObject GetEmployeePanel;
        [SerializeField] private TMP_InputField GetEmployeeIdInputField;
        [SerializeField] private Button GetEmployeeSendQueryButton;

        [Header("CreateEmployee")]
        [SerializeField] private Button CreateEmployeeButton;
        [SerializeField] private GameObject CreateEmployeePanel;
        [SerializeField] private TMP_InputField CreateEmployeeNameInputField;
        [SerializeField] private TMP_InputField CreateEmployeeSalaryInputField;
        [SerializeField] private TMP_InputField CreateEmployeeAgeInputField;
        [SerializeField] private Button CreateEmployeeSendQueryButton;

        [Header("DeleteEmployee")]
        [SerializeField] private Button DeleteEmployeeButton;
        [SerializeField] private GameObject DeleteEmployeePanel;
        [SerializeField] private TMP_InputField DeleteEmployeeIdInputField;
        [SerializeField] private Button DeleteEmployeeSendQueryButton;

        [Header("Response")]
        [SerializeField] private TextMeshProUGUI Response;

        [Header("APIFailed")]
        [SerializeField] private GameObject APIFailedPanel;
        [SerializeField] private TextMeshProUGUI APIFailedText;
        [SerializeField] private Button APIFailedCloseButton;


        private QueryManager queryManager;
        // Start is called before the first frame update
        void Start()
        {

            queryManager = GetComponent<QueryManager>();
            // Registering API Failed Callback
            queryManager.APIFailedCallBack += OnAPIFailed;
            APIFailedCloseButton.onClick.AddListener(() => APIFailedPanel.SetActive(false));
            APIFailedPanel.SetActive(false);

            // Registering Buttons Callback
            GetEmployeeButton.onClick.AddListener(ToggleGetEmployeePanel);
            CreateEmployeeButton.onClick.AddListener(ToggleCreateEmployeePanel);
            DeleteEmployeeButton.onClick.AddListener(ToggleDeleteEmployeePanel);

            GetEmployeeSendQueryButton.onClick.AddListener(SendGetEmployeeQuery);
            CreateEmployeeSendQueryButton.onClick.AddListener(SendCreateEmployeeQuery);
            DeleteEmployeeSendQueryButton.onClick.AddListener(SendDeleteEmployeeQuery);

            // Initializing GetEmployee at start
            ToggleGetEmployeePanel();
        }

        /// <summary>
        /// Event When API Failed
        /// </summary>
        /// <param name="error"></param>
        private void OnAPIFailed(string error)
        {
            APIFailedText.text = error;
            APIFailedPanel.SetActive(true);
        }

        private void ToggleGetEmployeePanel()
        {
            GetEmployeePanel.SetActive(true);
            CreateEmployeePanel.SetActive(false);
            DeleteEmployeePanel.SetActive(false);
            GetEmployeeIdInputField.text = "1";
        }

        private void ToggleCreateEmployeePanel()
        {
            GetEmployeePanel.SetActive(false);
            CreateEmployeePanel.SetActive(true);
            DeleteEmployeePanel.SetActive(false);
            CreateEmployeeNameInputField.text = "Luffy";
            CreateEmployeeSalaryInputField.text = "1000$";
            CreateEmployeeAgeInputField.text = "19";
        }

        private void ToggleDeleteEmployeePanel()
        {
            GetEmployeePanel.SetActive(false);
            CreateEmployeePanel.SetActive(false);
            DeleteEmployeePanel.SetActive(true);
            DeleteEmployeeIdInputField.text = "1";
        }

        private void SendGetEmployeeQuery()
        {
            queryManager.GetEmployeeAPI(int.Parse(GetEmployeeIdInputField.text), ShowResponse);
        }

        private void SendCreateEmployeeQuery()
        {
            var sendData = new CreateEmployeeData
            {
                Name = CreateEmployeeNameInputField.text,
                Salary = CreateEmployeeSalaryInputField.text,
                Age = int.Parse(CreateEmployeeAgeInputField.text)
            };
            queryManager.CreateEmployeeAPI(sendData, ShowResponse);
        }

        private void SendDeleteEmployeeQuery()
        {
            queryManager.DeleteEmployeeAPI(int.Parse(DeleteEmployeeIdInputField.text), ShowResponse);
        }
        private void ShowResponse(string response)
        {
            Response.text = response;
            LayoutRebuilder.ForceRebuildLayoutImmediate(Response.transform.parent as RectTransform);
        }
    }
}
