using Newtonsoft.Json;

namespace APIQueryMaster.Structure
{

    #region GetEmployee
    public partial class EmployeeSendData
    {
        [JsonProperty("employeeId")]
        public int employeeId { get; set; }
    }

    public partial class GetEmployeeRecieveData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public EmployeeData Data { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public partial class EmployeeData
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("employee_name")]
        public string EmployeeName { get; set; }

        [JsonProperty("employee_salary")]
        public long EmployeeSalary { get; set; }

        [JsonProperty("employee_age")]
        public long EmployeeAge { get; set; }

        [JsonProperty("profile_image")]
        public string ProfileImage { get; set; }
    }

    #endregion
    #region CreateEmployee
    
    public partial class CreateEmployeeData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("salary")]
        public string Salary { get; set; }

        [JsonProperty("age")]
        public int Age { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class CreateEmployeeRecieveData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public CreateEmployeeData Data { get; set; }
    }

    #endregion
    #region DeleteEmployee
    public partial class DeleteEmployeeRecieveData
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
    #endregion
}
