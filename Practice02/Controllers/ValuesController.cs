using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Newtonsoft.Json;
using Practice02.Models;
using Microsoft.Extensions.Configuration;

namespace Practice02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ValuesController(IConfiguration ic)
        {
            _config = ic;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Selects a specific Acoount.
        /// </summary>
        /// <param name="account"></param> 

        // GET api/values/user1 -> SELECT
        [HttpGet("{account}")]
        public ActionResult Get(string account)
        {
            string queryString = "select * from [dbo].[Membership] " +
                "where Account=@Account";
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(queryString, conn);
                try
                {
                    cmd.Parameters.AddWithValue("@Account", account);
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter
                    {
                        SelectCommand = cmd
                    };
                    da.Fill(ds);
                    DataTable dt = ds.Tables[0]; // assign to Member obj -> not return json array                    
                    return new JsonResult(dt); // json:　Base 
                }
                catch (Exception ex)
                {
                    Console.WriteLine(queryString);
                    Console.WriteLine(ex.Message);
                    var result = "wrong";
                    return new JsonResult(result); // ex.Message;
                }
            }
        }



        // POST api/values -> INSERT
        [HttpPost]
        public ActionResult Post([FromBody] Member memberAdd) // without DeserializeObject
        {
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string queryString = "insert into [dbo].[membership] (Account, Phone, Birthday, " +
                    "Address, TimeInsert, TimeLastUpdate) values ( @Account, @Phone, @Birthday, " +
                    "@Address, @TimeInsert, @TimeLastUpdate)";
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(queryString, conn))
                    {
                        cmd.Parameters.Add("@Account", SqlDbType.VarChar, 10).Value = memberAdd.Account;
                        cmd.Parameters.Add("@Phone", SqlDbType.VarChar, 10).Value = memberAdd.Phone;
                        cmd.Parameters.Add("@Birthday", SqlDbType.Date).Value = memberAdd.Birthday;
                        cmd.Parameters.Add("@Address", SqlDbType.NText).Value = memberAdd.Address;
                        cmd.Parameters.Add("@TimeInsert", SqlDbType.Date).Value = memberAdd.TimeInsert;
                        cmd.Parameters.Add("@TimelastUpdate", SqlDbType.Date).Value = memberAdd.TimeLastUpdate;

                        var result = cmd.ExecuteNonQuery() > 0 ?
                            new { result = "success" } : new { result = "failed" };
                        return new JsonResult(result);
                    }
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var result = new { result = "wrong" };
                    return new JsonResult(result);
                }
            }
        
        }

        /// <summary>
        /// Modifys a specific Acoount, with updated data in body.
        /// </summary>
        /// <param name="account"></param> 
        
        // PUT api/values/user2 -> UPDATE
        [HttpPut("{account}")]
        public ActionResult Put(string account, [FromBody] Member member)
        {
            string queryString = "update [dbo].[membership] set ";
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                bool[] queryNum = { false, false, false };
                bool flag = false;
                if(member.GetType().GetProperty("Phone").GetValue(member) != null)
                {
                    queryString += "Phone=@Phone ";
                    queryNum[0] = true;
                    flag = true;
                }
                if (member.GetType().GetProperty("Birthday").GetValue(member) != null)
                {
                    if (flag) queryString += ", ";
                    queryString += "Birthday=@Birthday";
                    queryNum[1] = true;
                    flag = true;
                }
                if (member.GetType().GetProperty("Address").GetValue(member) != null)
                {
                    if (flag) queryString += ", ";
                    queryString += "Address=@Address";
                    queryNum[2] = true;
                    flag = true;
                }
                if (flag) queryString += ", ";
                queryString += "TimeLastUpdate=@TimeLastUpdate where Account=@Account";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                try
                {
                    conn.Open();
                    if (queryNum[0]) cmd.Parameters.AddWithValue("@Phone", member.Phone);
                    if (queryNum[1]) cmd.Parameters.AddWithValue("@Birthday", member.Birthday);
                    if (queryNum[2]) cmd.Parameters.AddWithValue("@Address", member.Address);
                    cmd.Parameters.AddWithValue("@Account", account);
                    cmd.Parameters.AddWithValue("@TimelastUpdate", member.TimeLastUpdate);
                    var result = cmd.ExecuteNonQuery() > 0 ?
                        new { result = "success" } : new { result = "failed" };
                    return new JsonResult(result);
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(cmd);
                    var result = new { result = "wrong" };
                    return new JsonResult(result);
                }
            }
            
        }

        /// <summary>
        /// Deletes a specific Acoount.
        /// </summary>
        /// <param name="account"></param> 

        // DELETE api/values/user3 -> DELETE
        [HttpDelete("{account}")]
        public ActionResult Delete(string account)
        {
            string queryString = "delete from [dbo].[Membership] " +
                "where Account=@Account";
            string connectionString = _config["ConnectionStrings:DefaultConnection"];
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand(queryString, conn);
                try
                {
                    conn.Open();
                    cmd.Parameters.AddWithValue("@Account", account);
                    var result = cmd.ExecuteNonQuery() > 0 ? 
                        new { result = "success" } : new { result = "failed" };
                    return new JsonResult(result);
                }   
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    var result = new { result = "wrong" };
                    return new JsonResult(result);
                }
            }
        }
    }
}
