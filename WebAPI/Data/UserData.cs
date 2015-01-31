using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
namespace WebAPI.Data
{
    public class UserData
    {
        private static UserData instance;
        public static UserData Instance 
        {
            get 
            {
                if (instance == null) { instance = new UserData(); }
                return instance;
            }
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool RegistUser(Models.UserModel userInfo,ref string reason)
        {
            try
            {
                if(string.IsNullOrEmpty(userInfo.phone))
                {
                    reason = "手机号为空";
                    return false;
                }
                if (string.IsNullOrEmpty(userInfo.pwd))
                {
                    reason = "密码为空";
                    return false;
                }
                if (string.IsNullOrEmpty(userInfo.zhifubao))
                {
                    reason = "支付宝为空";
                    return false;
                }
                if (string.IsNullOrEmpty(userInfo.qq))
                {
                    reason = "qq号为空";
                    return false;
                }
                if (string.IsNullOrEmpty(userInfo.deviceid))
                {
                    reason = "deviceID 为空";
                    return false;
                }

                if (string.IsNullOrEmpty(userInfo.fatherphone))
                {
                    userInfo.fatherphone = "";
                }
                if (userInfo.id == 0) { userInfo.id = -1; }


                SqlParameter[] param = new SqlParameter[] { 
                    new SqlParameter("@id",SqlDbType.Int,4),
                    new SqlParameter("@phone",SqlDbType.VarChar,11),
                    new SqlParameter("@pwd",SqlDbType.VarChar,30),
                    new SqlParameter("@fatherphone",SqlDbType.VarChar,11),
                    new SqlParameter("@zhifubao",SqlDbType.VarChar,120),
                    new SqlParameter("@qq",SqlDbType.VarChar,15),
                    new SqlParameter("@deviceid",SqlDbType.VarChar,50),
                    new SqlParameter("@out_flag",SqlDbType.Int,4),
                    new SqlParameter("@out_reason",SqlDbType.VarChar,400)

                };
                param[0].Value = userInfo.id;
                param[1].Value = userInfo.phone;
                param[2].Value = userInfo.pwd;
                param[3].Value = userInfo.fatherphone;
                param[4].Value = userInfo.zhifubao;
                param[5].Value = userInfo.qq;
                param[6].Value = userInfo.deviceid;

                param[7].Direction = ParameterDirection.Output;
                param[8].Direction = ParameterDirection.Output;

                SqlHelper.ExecteNonQuery(CommandType.StoredProcedure, "sp_user_regist", param);
                if (param[7].Value.ToString() == "0")
                {
                    reason = param[8].Value.ToString();
                    return false;
                }
                return true;
            }
            catch (Exception e)
            {
                reason = "系统错误"+e.Message;
                return false;
            }
        }

        /// <summary>
        /// 用户登陆
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool Login(Models.UserModel userInfo, ref string reason,ref int type)
        {
            try
            {
                string sql = "select phone,pwd,deviceid from userinfo where phone=@phone" ;
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@phone",userInfo.phone)
                };
                DataSet ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, param);
                if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1)
                {
                    reason = "用户不存在";
                    return false;
                }
                string password = string.Empty;
                if (ds.Tables[0].Rows[0]["pwd"] != null)
                {
                    password = ds.Tables[0].Rows[0]["pwd"].ToString();
                }
                if (password != userInfo.pwd)
                {
                    reason = "密码错误";
                    return false;
                }

                if (userInfo.allmoney > 20)
                {
                    reason = "用户数据异常，请联系客服";
                    return false;
                }

                string deviceId  = string.Empty;
                if(ds.Tables[0].Rows[0]["deviceid"]!=null)
                { deviceId = ds.Tables[0].Rows[0]["deviceid"].ToString(); }
                if(deviceId != userInfo.deviceid)
                {
                    type = 2;
                }
                else
                {
                    type = 1;
                }
                return true;
            }
            catch (Exception e)
            {
                reason = "系统错误" + e.Message;
                return false;
            }
        }

        /// <summary>
        /// 更新密码
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="pwd"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public  bool UpdatePwd(string phone, string pwd, ref string reason)
        {
            try
            {
                string sql = "update userinfo set pwd=@pwd where phone=@phone";
                SqlParameter[] param = new SqlParameter[] { 
                    new SqlParameter("@pwd",pwd),
                    new SqlParameter("@phone",phone)
                };
                SqlHelper.ExecuteScalar(CommandType.Text, sql, param);
                return true;
            }
            catch(Exception e)
            {
                reason = e.Message;
                return false;
            }
        }

        /// <summary>
        /// 根据电话号码获取UserModel
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public Models.UserModel GetUser(string phone, ref string errMsg)
        {
            try
            {
                string sql = @"select  id,phone,pwd,fatherphone,zhifubao,allmoney,nowmoney,
                allsonmoney,nowsonmoney,sonnums,qq,deviceid,remark1,remark2 from userinfo where Phone = @phone";
                SqlParameter[] param = new SqlParameter[] { 
                    new SqlParameter("@phone",phone)
                };
                DataSet ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, param);
                if (ds == null || ds.Tables.Count < 1 || ds.Tables[0].Rows.Count<1)
                {
                    errMsg = "查无数据";
                    return null ;
                }
                List<Models.UserModel> userList = ConvertToList(ds.Tables[0]);
                return userList[0];
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return null;
            }
        }

        /// <summary>
        ///  增加流量比
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool AddMoney(Models.UserModel userInfo, ref string reason)
        {
            try
            {
                Models.UserModel currentModel = this.GetUser(userInfo.phone, ref reason);
                if (!string.IsNullOrEmpty(reason)) { return false;}

                string myKey = MD5Helper.GetMD5(MD5Helper.GetMD5(currentModel.pwd + "ai1zhuan2", 2),2);
                if (myKey != userInfo.key)
                {
                    reason = "数据验证失败";
                    return false;
                }

                if (string.IsNullOrEmpty(userInfo.phone)) { reason = "手机号为空"; return false; }
                if (userInfo.id == 0) { userInfo.id = -1; }
                if (string.IsNullOrEmpty(userInfo.fatherphone)) { userInfo.fatherphone = ""; }

                SqlParameter[] param = new SqlParameter[] { 
                    new SqlParameter("@id",SqlDbType.Int,4),
                    new SqlParameter("@phone",SqlDbType.VarChar,11),
                    new SqlParameter("@fatherphone",SqlDbType.VarChar,11),
                    new SqlParameter("@mymoney",SqlDbType.Float,18),

                    new SqlParameter("@out_flag",SqlDbType.Int,4),
                    new SqlParameter("@out_reason",SqlDbType.VarChar,400)

                };
                param[0].Value = userInfo.id;
                param[1].Value = userInfo.phone;
                param[2].Value = userInfo.fatherphone;
                param[3].Value = userInfo.mymoney;

                param[4].Direction = ParameterDirection.Output;
                param[5].Direction = ParameterDirection.Output;

                SqlHelper.ExecteNonQuery(CommandType.StoredProcedure, "sp_user_addmoney", param);
                if (param[4].Value.ToString() == "0")
                {
                    reason = param[5].Value.ToString();
                    return false;
                }
                return true;

            }
            catch(Exception e)
            {
                reason = e.Message;
                return false;
            }
        }
        private List<Models.UserModel> ConvertToList(DataTable dt)
        {
            if (dt == null || dt.Rows.Count < 1) { return new List<Models.UserModel>(); }
            List<Models.UserModel> userList = new List<Models.UserModel>();
            foreach (DataRow row in dt.Rows)
            {
                Models.UserModel model = new Models.UserModel();
                if(row["id"]!=null && !string.IsNullOrEmpty(row["id"].ToString()))
                {
                    model.id = Convert.ToInt32(row["id"].ToString());
                }
                if (row["pwd"] != null && !string.IsNullOrEmpty(row["pwd"].ToString()))
                {
                    model.pwd = row["pwd"].ToString();
                }
                if (row["phone"] != null && !string.IsNullOrEmpty(row["phone"].ToString()))
                {
                    model.phone = row["phone"].ToString();
                }
                if (row["fatherphone"] != null && !string.IsNullOrEmpty(row["fatherphone"].ToString()))
                {
                    model.fatherphone = row["fatherphone"].ToString();
                }
                if (row["zhifubao"] != null && !string.IsNullOrEmpty(row["zhifubao"].ToString()))
                {
                    model.zhifubao = row["zhifubao"].ToString();
                }
                if (row["allmoney"] != null && !string.IsNullOrEmpty(row["allmoney"].ToString()))
                {
                    model.allmoney = Convert.ToDouble(row["allmoney"].ToString());
                }
                if (row["nowmoney"] != null && !string.IsNullOrEmpty(row["nowmoney"].ToString()))
                {
                    model.nowmoney = Convert.ToDouble(row["nowmoney"].ToString());
                }
                if (row["allsonmoney"] != null && !string.IsNullOrEmpty(row["allsonmoney"].ToString()))
                {
                    model.allsonmoney = Convert.ToDouble(row["allsonmoney"].ToString());
                }
                if (row["nowsonmoney"] != null && !string.IsNullOrEmpty(row["nowsonmoney"].ToString()))
                {
                    model.nowsonmoney = Convert.ToDouble(row["nowsonmoney"].ToString());
                }
                if (row["sonnums"] != null && !string.IsNullOrEmpty(row["sonnums"].ToString()))
                {
                    model.sonnums = Convert.ToDouble(row["sonnums"].ToString());
                }
                if (row["qq"] != null && !string.IsNullOrEmpty(row["qq"].ToString()))
                {
                    model.qq = row["qq"].ToString();
                }
                if (row["deviceid"] != null && !string.IsNullOrEmpty(row["deviceid"].ToString()))
                {
                    model.deviceid = row["deviceid"].ToString();
                }
                userList.Add(model);
            }
            return userList;
        }


        #region adminLogin
        public Models.AdminModel AdminLogin(Models.AdminModel adminInfo, ref string reason)
        {
            try
            {
                string sql = "select user_id,user_account,user_pwd,mobile,real_name from SystemUser where user_account='" + adminInfo.user_account + "'";
                DataSet ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, null);
                if(ds==null || ds.Tables.Count<1)
                {
                    reason = "用户不存在";
                    return null;
                }

                List<WebAPI.Models.AdminModel> AdminList = ConvertToAdminList(ds.Tables[0]);
                if (AdminList == null || AdminList.Count < 1)
                {
                    reason = "用户不存在";
                    return null;
                }

                if(adminInfo.user_pwd != AdminList[0].user_pwd)
                {
                    reason = "密码不正确";
                    return null;
                }

                return AdminList[0];
            }
            catch(Exception e)
            {
                reason = e.Message;
                return null;
            }
        }

        private List<Models.AdminModel> ConvertToAdminList(DataTable dataTable)
        {
            if (dataTable == null || dataTable.Rows.Count < 1) { return null; }
            List<Models.AdminModel> modelList = new List<Models.AdminModel>();
            foreach(DataRow row in dataTable.Rows)
            {
                Models.AdminModel admin = new Models.AdminModel();
                if (row["user_id"] != null && row["user_id"].ToString() != "")
                {
                    admin.user_id = row["user_id"].ToString();
                }
                if (row["user_account"] != null && row["user_account"].ToString() != "")
                {
                    admin.user_account = row["user_account"].ToString();
                }
                if (row["user_pwd"] != null && row["user_pwd"].ToString() != "")
                {
                    admin.user_pwd = row["user_pwd"].ToString();
                }
                if (row["mobile"] != null && row["mobile"].ToString() != "")
                {
                    admin.mobile = row["mobile"].ToString();
                }
                if (row["real_name"] != null && row["real_name"].ToString() != "")
                {
                    admin.real_name = row["real_name"].ToString();
                }
                modelList.Add(admin);
            }
            return modelList;
        }
        #endregion 
    
        public List<Models.UserModel> GetUserList(Dictionary<string, string> param, ref int count, ref string errMsg)
        {
            try
            {
                string pageWhere = " where 1=1";
                string sqlWhere = " where 1=1";
                CreateSqlWhere(param, ref pageWhere, ref sqlWhere);


                string countSql = "select Count(phone) from userInfo" + sqlWhere;
                object objCount = SqlHelper.ExecuteScalar(CommandType.Text, countSql, null);
                count = Convert.ToInt32(objCount.ToString());

                string sql = string.Format(@"with temp as (select id,phone,pwd,fatherphone,zhifubao,allmoney,nowmoney,
                allsonmoney,nowsonmoney,sonnums,qq,deviceid,remark1,remark2 from UserInfo {0}),
                list as (select ROW_NUMBER() over (order by id asc) as RowNumber,* from temp)
                select * from list {1} ", sqlWhere, pageWhere);

                DataSet ds = SqlHelper.ExecuteDataSet(CommandType.Text, sql, null);
                if (ds == null || ds.Tables.Count < 1) { count = 0; return null; }
                return ConvertToList(ds.Tables[0]);
            }
            catch (Exception e)
            {
                errMsg = e.Message;
                return null;
            }
        }

        private void CreateSqlWhere(Dictionary<string, string> param, ref string pageWhere, ref string sqlWhere)
        {
            foreach (KeyValuePair<string, string> values in param)
            {
                if (values.Key == "startRow")
                {
                    pageWhere += string.Format(" and RowNumber >{0}", values.Value);
                }
                else if(values.Key=="endRow")
                {
                    pageWhere += string.Format(" and RowNumber <= {0}", values.Value);

                }
                else
                {
                    sqlWhere += string.Format(" and {0}='{1}'", values.Key, values.Value);
                }
            }
        }

        /// <summary>
        /// 更新货币
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="nowMoney"></param>
        /// <param name="nowSonMoney"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool SetMoney(string phone, double nowMoney, double nowSonMoney, ref string reason)
        {
            try
            {
                string sql = "update UserInfo set nowmoney=nowmoney-@nowMoney,nowsonmoney=nowsonmoney-@nowSonMoney where phone = @phone";
                SqlParameter[] param = new SqlParameter[] {
                    new SqlParameter("@nowMoney",nowMoney),
                    new SqlParameter("@nowSonMoney",nowSonMoney),
                    new SqlParameter("@phone",phone)
                };

                SqlHelper.ExecteNonQuery(CommandType.Text, sql, param);
                return true;           
            }
            catch (Exception e)
            {
                reason = e.Message;
                return false;
            }
        }
    }
}