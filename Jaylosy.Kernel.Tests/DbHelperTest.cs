using System;
using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jaylosy.Kernel.DataBase;
namespace Jaylosy.Kernel.Tests
{
    [TestClass]
    public class DbHelperTest
    {
        [TestMethod]
        public void TestQueryDatatable()
        {
            DbHelper db = new DbHelper("defaultDb");
            db.OpenConnection();
            var trans = db.BeginTransaction();
            try
            {
                for (int i = 0; i < 100; i++)
                {
                    var p_rq = db.CreateDbParameter();
                    p_rq.ParameterName = "@rq";
                    p_rq.DbType = DbType.DateTime;
                    p_rq.Value = DateTime.Now;

                    var p_name = db.CreateDbParameter();
                    p_name.ParameterName = "@name";
                    p_name.DbType = DbType.String;
                    p_name.Value = "admin";
                    var ret = db.ExecuteNonQuery(trans,"insert into t_test(name,rq) values(@name,@rq)",new IDbDataParameter[] {
                        p_name,p_rq
                    } );
                }
                trans.Commit();
            }
            catch (Exception)
            {
                trans.Rollback();
                throw;
            }

            db.CloseConnection();
        }

        [TestMethod]
        public void TestQueryDatatable_Oracle()
        {
            DbHelper db = new DbHelper("cwgl1");
            db.OpenConnection();
            var data = db.QueryDatatable("select * from xt_xtyhry");
            Assert.AreEqual(data.Rows.Count > 0, true);
        }
        [TestMethod]
        public void TestQueryDatatable_ManagedOracle()
        {
            DbHelper db = new DbHelper("cwgl");
            db.OpenConnection();
            var data = db.QueryDatatable("select * from xt_xtyhry");
            Assert.AreEqual(data.Rows.Count > 0, true);
        }
    }
}
