using SV21T1020599.DomainModels;
using SV21T1020599.DataLayers;

namespace SV21T1020599.BusinessLayers
{
    public static class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;
        private static readonly IUserAccountDAL customerAccountDB;
        static UserAccountService()
        {
            string connectionString = "Server=DESKTOP-12J6D6C;user id=sa;Password=123;database=PL;TrustServerCertificate=true";
            


            employeeAccountDB = new DataLayers.SQLServer.EmployeeAccountDAL(connectionString);
            customerAccountDB = new DataLayers.SQLServer.CustomerAccountDAL(connectionString);
        }
        public static UserAccount? Authorize(UserTypes userTypes, string username, string password)
        {
            if (userTypes == UserTypes.Employee)
                return employeeAccountDB.Authorize(username, password);
            else
                return customerAccountDB.Authorize(username, password);
        }
        public static bool ChangedPassword(string username, string newpassword)
        {
            var customerAccount = customerAccountDB.GetUserAccountByUsername(username);
            if (customerAccount != null)
            {
                return customerAccountDB.ChangePassword(username, newpassword);
            }

            var employeeAccount = employeeAccountDB.GetUserAccountByUsername(username);
            if (employeeAccount != null)
            {
                return employeeAccountDB.ChangePassword(username, newpassword);
            }

            return false;
        }
        public static bool Register(string email, string password, string customerName, string address, string phone)
        {
            return customerAccountDB.Register(email, password, customerName, address, phone);
        }
    }


    public enum UserTypes
    {
        Employee,
        Customer
    }
}