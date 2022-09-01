using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Drawing;
using System.Windows;
using dispensingMachine.Infrastracture.Models;

namespace dispensing
{
    public struct MedicModel
    {
        public int level;
        public int qty;
        public int x;
        public int y;
        public int z;
        public int machine;
        public string boxid;
        public string medid;
        public string name;
        public string mark;
        public string unit;
        public string batch;
        public string validity;
    }
    public struct AddRecord
    {
        public string addTime;
        public string doctorName;
        public string medicName;
        public int quantity;
        public string batch;
        public string validity;
        public string status;
        public int machine;
    }
    public struct GetRecord
    {
        public string getTime;
        public string prescription;
        public string medicName;
        public int quantity;
        public string patName;
        public string patGender;
        public string patAge;
        public string status;
    }
    class sqlite
    {
        object sqliteobj = new object();
        SQLiteConnection con = null;
        string sqlFile = System.Environment.CurrentDirectory + @"\data1.db";

        //private delegate void DispMSGDelegate1();
        /// <summary>
        /// 初始化数据库
        /// </summary>
        /// <param name="type">0：终端 1：药师端</param>
        /// <returns></returns>
        public bool Init(int type)
        {
            try
            {
                if (type == 0)
                {
                    string sqlFile = System.Environment.CurrentDirectory + @"\data1.db";
                    string connStr = "Data Source=" + sqlFile + ";Version=3;";
                    con = new SQLiteConnection(connStr);
                    con.Open();
                    return true;
                }
                else
                {
                    DirectoryInfo path = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                    string strPath = path.Parent.FullName;
                    strPath += @"\data1.db";
                    string connStr = "Data Source=" + strPath + ";Version=3;";
                    con = new SQLiteConnection(connStr);
                    con.Open();
                    return true;
                }

            }
            catch (Exception ex)
            {
                log4net.log.Info("数据库报错: " + ex.Message);
                return false;
            }

        }

        public void Close()
        {
            con.Close();
        }
        public bool QueryMedicineInfo(string medicineid, int num, out int level, out int qty, out string boxid, out string medid, out string name)
        {
            level = 0; qty = 0; boxid = ""; medid = ""; name = "";
            try
            {
                SQLiteDataReader msqlReader = null;
                SQLiteCommand cmd = new SQLiteCommand(con);

                int max = 0;

                string str = string.Format("SELECT * FROM medicineBox WHERE medicineid='{0}'", medicineid);
                cmd.CommandText = str;
                msqlReader = cmd.ExecuteReader();
                while (msqlReader.Read())
                {
                    int validity = Convert.ToInt32(msqlReader["validity"].ToString());
                    int today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

                    if (validity >= today)
                    {
                        int temp_level = Convert.ToInt32(msqlReader["level"].ToString());
                        int temp_qty = Convert.ToInt32(msqlReader["quantity"].ToString());

                        if (temp_qty >= num)
                        {
                            if (temp_level > max)
                            {
                                max = temp_level;
                                level = max;
                                qty = temp_qty;
                                boxid = msqlReader["boxid"].ToString();
                                medid = msqlReader["medicineid"].ToString();
                                name = msqlReader["name"].ToString();
                            }
                        }
                    }
                    else
                    {
                        medid = msqlReader["medicineid"].ToString();
                        name = msqlReader["name"].ToString();
                        log4net.log.Info("药品已过期: " + medicineid);
                    }
                }

                msqlReader.Close();
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("数据库报错: " + ex.Message);
                log4net.log.Info("数据库报错: " + ex.Message);
                return false;
            }

        }
        //public bool AddMedicineBox(int level, string strInfo)
        //{
        //    try
        //    {
        //        string[] split = strInfo.Split(',');
        //        SQLiteCommand cmd = new SQLiteCommand(con);
        //        string str = string.Format("INSERT INTO medicineBox VALUES({0},", level);
        //        for (int i = 0; i < 8; i++)
        //        {
        //            if (i < 2 || i > 4)
        //            {
        //                str = str + "'" + split[i] + "'";
        //            }
        //            else
        //            {
        //                str = str + split[i];
        //            }
        //            if (i < 7)
        //            {
        //                str = str + ",";
        //            }
        //            else
        //            {
        //                str = str + ")";
        //            }
        //        }
        //        cmd.CommandText = str;
        //        cmd.ExecuteNonQuery();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        //MessageBox.Show("数据库报错: " + ex.Message);
        //        log4net.log.Info("数据库报错: " + ex.Message);
        //        return false;
        //    }
        //}

        public bool AddMedicineBox(MedicineInBox medc)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    string str = string.Format("INSERT INTO medicineBox VALUES(1,'{0}','M000',1,1,1,'b',1,'b','b','1','2021',{1})", medc.BoxId, medc.Machine);

                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();

                    UpdateMedicine(medc);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据库报错: " + ex.Message);
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine(ex.ToString());
                    return false;
                }
            }
        }
        public bool UpdateMedicine(MedicineInBox medc)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    string str = string.Format("UPDATE medicineBox SET level={0},x={1},y={2},z={3},quantity={4},medicineid='{5}',name='{6}',mark='{7}'," +
                        "unit='{8}',batch='{9}',validity='{10}',machine={11} WHERE boxid='{12}'", medc.Level, medc.X, medc.Y, medc.Z, medc.Quantity, medc.Id, medc.Name, medc.Mark,
                        medc.Unit, medc.BatchNumber, medc.expireDate, medc.Machine, medc.BoxId);

                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据库报错: " + ex.ToString());
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
        public bool UpdateMedicineNumber(string boxid, string medid, int qty, int machine)
        {
            try
            {

                SQLiteCommand cmd = new SQLiteCommand(con);
                string str = string.Format("UPDATE medicineBox SET quantity={0} WHERE boxid='{1}' AND medicineid='{2}' AND machine={3}",
                    qty, boxid, medid, machine);

                cmd.CommandText = str;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                //MessageBox.Show("数据库报错: " + ex.Message);
                log4net.log.Info("数据库报错: " + ex.Message);
                return false;
            }
        }

        public bool InsertGetMedicinesRecord(string prescription, string medicine, int qty, string status)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(con);
                string str = string.Format("INSERT INTO getmedicine VALUES('{0}','{1}','{2}',{3},'{4}','{5}',{6},'{7}')",
                    DateTime.Now.ToString("yyyyMMdd HH:mm"), prescription, medicine, qty, "张三", "男", 24, status);

                cmd.CommandText = str;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库报错: " + ex.Message);
                //log4net.log.Info("数据库报错: " + ex.Message);
                //Console.WriteLine("数据库报错: " + ex.Message);
                return false;
            }
        }

        public bool InsertAbnormalRecord(string prescription, string reason, string status)
        {
            try
            {
                SQLiteCommand cmd = new SQLiteCommand(con);
                string str = string.Format("INSERT INTO abnormal VALUES('{0}','{1}','{2}','{3}','{4}')",
                    DateTime.Now.ToString("yyyyMMdd HH:mm"), prescription, "张三", reason, status);

                cmd.CommandText = str;
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("数据库报错: " + ex.Message);
                //log4net.log.Info("数据库报错: " + ex.Message);
                //Console.WriteLine("数据库报错: " + ex.Message);
                return false;
            }
        }

        public bool QueryAllMedicines(out List<MedicModel> model)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteDataReader msqlReader = null;
                    SQLiteCommand cmd = new SQLiteCommand(con);

                    string str = "SELECT * FROM medicineBox";
                    cmd.CommandText = str;
                    msqlReader = cmd.ExecuteReader();
                    model = new List<MedicModel>();
                    while (msqlReader.Read())
                    {
                        model.Add(
                            new MedicModel()
                            {
                                level = Convert.ToInt32(msqlReader["level"].ToString()),
                                qty = Convert.ToInt32(msqlReader["quantity"].ToString()),
                                x = Convert.ToInt32(msqlReader["x"].ToString()),
                                y = Convert.ToInt32(msqlReader["y"].ToString()),
                                z = Convert.ToInt32(msqlReader["z"].ToString()),
                                machine = Convert.ToInt32(msqlReader["machine"].ToString()),

                                boxid = msqlReader["boxid"].ToString(),
                                medid = msqlReader["medicineid"].ToString(),
                                name = msqlReader["name"].ToString(),
                                mark = msqlReader["mark"].ToString(),
                                unit = msqlReader["unit"].ToString(),
                                batch = msqlReader["batch"].ToString(),
                                validity = msqlReader["validity"].ToString()
                            });
                    }

                    msqlReader.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    model = null;
                    MessageBox.Show("数据库报错: " + ex.Message);
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
        public bool QueryOneMedicines(string boxId, out MedicModel model)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteDataReader msqlReader = null;
                    SQLiteCommand cmd = new SQLiteCommand(con);

                    string str = String.Format("SELECT * FROM medicineBox WHERE boxid='{0}'", boxId);
                    cmd.CommandText = str;
                    msqlReader = cmd.ExecuteReader();
                    model = new MedicModel();
                    while (msqlReader.Read())
                    {

                        model = new MedicModel()
                        {
                            level = Convert.ToInt32(msqlReader["level"].ToString()),
                            qty = Convert.ToInt32(msqlReader["quantity"].ToString()),
                            x = Convert.ToInt32(msqlReader["x"].ToString()),
                            y = Convert.ToInt32(msqlReader["y"].ToString()),
                            z = Convert.ToInt32(msqlReader["z"].ToString()),
                            machine = Convert.ToInt32(msqlReader["machine"].ToString()),

                            boxid = msqlReader["boxid"].ToString(),
                            medid = msqlReader["medicineid"].ToString(),
                            name = msqlReader["name"].ToString(),
                            mark = msqlReader["mark"].ToString(),
                            unit = msqlReader["unit"].ToString(),
                            batch = msqlReader["batch"].ToString(),
                            validity = msqlReader["validity"].ToString()
                        };
                        break;
                    }

                    msqlReader.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    model = new MedicModel();
                    MessageBox.Show("数据库报错: " + ex.Message);
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
        public bool QueryBox(int machine, string boxId, out bool exists)
        {
            lock (sqliteobj)
            {
                exists = false;
                try
                {
                    SQLiteDataReader msqlReader = null;
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    //string str = "SELECT * FROM medicineBox WHERE boxid='{0}', machine={1}";
                    string str = string.Format("SELECT * FROM medicineBox WHERE boxid='{0}' AND machine={1}", boxId, machine.ToString());
                    cmd.CommandText = str;
                    msqlReader = cmd.ExecuteReader();
                    if (msqlReader.Read())
                    {
                        exists = true;
                    }
                    return true;
                }
                catch (Exception ex)
                {

                    MessageBox.Show("数据库报错" + ex.Message);
                    return false;
                }
            }
        }
        public bool DeleteMedic(string boxId)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteCommand cmd = new SQLiteCommand(con);
                    string str = string.Format("DELETE FROM medicineBox WHERE boxid='{0}'", boxId);

                    cmd.CommandText = str;
                    cmd.ExecuteNonQuery();
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
        public bool QueryMedicAddRecord(out List<AddRecord> addRecords)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteDataReader msqlReader = null;
                    SQLiteCommand cmd = new SQLiteCommand(con);

                    string str = "SELECT * FROM addmedicine";
                    cmd.CommandText = str;
                    msqlReader = cmd.ExecuteReader();
                    addRecords = new List<AddRecord>();
                    while (msqlReader.Read())
                    {
                        addRecords.Add(
                            new AddRecord()
                            {
                                quantity = Convert.ToInt32(msqlReader["quantity"].ToString()),
                                machine = Convert.ToInt32(msqlReader["machine"].ToString()),

                                addTime = msqlReader["addTime"].ToString(),
                                doctorName = msqlReader["doctor"].ToString(),
                                medicName = msqlReader["medicine"].ToString(),
                                batch = msqlReader["batch"].ToString(),
                                validity = msqlReader["validity"].ToString(),
                                status = msqlReader["status"].ToString(),
                            });
                    }
                    msqlReader.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    addRecords = null;
                    MessageBox.Show("数据库报错: " + ex.Message);
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
        public bool QueryMedicGetRecord(out List<GetRecord> getRecords)
        {
            lock (sqliteobj)
            {
                try
                {
                    SQLiteDataReader msqlReader = null;
                    SQLiteCommand cmd = new SQLiteCommand(con);

                    string str = "SELECT * FROM getmedicine";
                    cmd.CommandText = str;
                    msqlReader = cmd.ExecuteReader();
                    getRecords = new List<GetRecord>();
                    while (msqlReader.Read())
                    {
                        getRecords.Add(
                            new GetRecord()
                            {
                                quantity = Convert.ToInt32(msqlReader["quantity"].ToString()),
                                patAge = msqlReader["age"].ToString(),

                                getTime = msqlReader["gettime"].ToString(),
                                prescription = msqlReader["prescription"].ToString(),
                                medicName = msqlReader["medicine"].ToString(),
                                patName = msqlReader["patient"].ToString(),
                                patGender = msqlReader["sex"].ToString(),
                                status = msqlReader["status"].ToString(),
                            });
                    }
                    msqlReader.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    getRecords = null;
                    MessageBox.Show("数据库报错: " + ex.Message);
                    //log4net.log.Info("数据库报错: " + ex.Message);
                    //Console.WriteLine("数据库报错: " + ex.Message);
                    return false;
                }
            }
        }
    }
}
