using crudeoperationonazuresqldb.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace crudeoperationonazuresqldb.Repository
{
    public class EmpRepository
    {
        public SqlConnection con;
        //To Handle connection related activities
        private void connection()
        {
           // string constr = ConfigurationManager.ConnectionStrings["SqlConn"].ToString();

            string constr = "Data Source=employeedbserver.database.windows.net;Initial Catalog=employeedb;User ID=*******;Password=********;Connect Timeout=30;Encrypt=True;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            con = new SqlConnection(constr);
        }
        //To Add Employee details
        public void AddEmployee(EmployeeModel objEmp)
        {
            //Additing the employess
            try
            {
                connection();
                con.Open();
                con.Execute("AddNewEmpDetails3", objEmp, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //To view employee details with generic list 
        public List<EmployeeModel> GetAllEmployees()
        {
            try
            {
                connection();
                con.Open();
                IList<EmployeeModel> EmpList = (IList<EmployeeModel>)SqlMapper.Query<EmployeeModel>(
                                  con, "GetEmployees").ToList();
                con.Close();
                return EmpList.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //To Update Employee details
        public void UpdateEmployee(EmployeeModel objUpdate)
        {
            try
            {
                connection();
                con.Open();
                con.Execute("UpdateEmpDetails", objUpdate, commandType: CommandType.StoredProcedure);
                con.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        //To delete Employee details
        public bool DeleteEmployee(int Id)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@EmpId", Id);
                connection();
                con.Open();
                con.Execute("DeleteEmpById", param, commandType: CommandType.StoredProcedure);
                con.Close();
                return true;
            }
            catch (Exception ex)
            {
                //Log error as per your need 
                throw ex;
            }
        }
    }
}