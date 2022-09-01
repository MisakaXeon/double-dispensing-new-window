using dispensing.CustomCommand;
using dispensing.CustomControls;
using dispensing.Expends;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using dispensingMachine.Infrastracture.Models;
using System.Windows;

namespace dispensing.ViewModel.Items
{

    /// <summary>
    /// 层 药槽中的药品详细信息
    /// </summary>
    public class LayerMedicineInfo : INotifyPropertyChanged
    {
        //public MedicineManagerModel managerModel;
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 添加事件
        /// </summary>
        public Action<LayerMedicineInfo> AddEventHandle { get; set; }
        /// <summary>
        /// 删除事件
        /// </summary>
        public Action<LayerMedicineInfo> DeleteEventHandle { get; set; }
        /// <summary>
        /// 修改事件
        /// </summary>
        public Action<LayerMedicineInfo> ModifyEventHandle { get; set; }
        /// <summary>
        /// id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public string Number { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 库存
        /// </summary>
        public int Stock { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime Date { get; set; }
        /// <summary>
        /// 长
        /// </summary>
        public int Long { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// 机器号
        /// </summary>
        public int Machine { get; set; }
        /// <summary>
        /// 药品号
        /// </summary>
        public string MedicineNumber { get; set; } = "1231231231";
        /// <summary>
        /// 厂商
        /// </summary>
        public string Manufacturers { get; set; } = "强生制药";
        /// <summary>
        /// 加药数量
        /// </summary>
        public int AddMedicineCount { get; set; } = 0;
        /// <summary>
        /// 保存命令
        /// </summary>
        public ICommand SaveCommand { get; set; }
        /// <summary>
        /// 删除命令
        /// </summary>
        public ICommand DeleteCommand { get; set; }
        IniFile m_ini = new IniFile("");
        sqlite m_sql = new sqlite();
        bool medicAddResultRecived = false;
        TaskList recvTaskList = null;
        public LayerMedicineInfo()
        {
            m_sql.Init(1);
            SaveCommand = new SampleCommand(o => true, o =>
            {
                //AddEventHandle?.Invoke(this);
                //DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "加药成功", null, null, "wait"));
                //return;

                // 处理保存的逻辑
                LayerContent layerContent = o as LayerContent;

                m_sql.QueryBox(Machine, Id, out bool exists);
                MedicModel existsModel = new MedicModel();
                if (exists)
                {
                    m_sql.QueryOneMedicines(Id, out existsModel);
                }
                //AddEventHandle?.Invoke(this);
                // 修改和保存数据处理

                //Console.WriteLine(layerContent);
                if (Id == "" || Id == String.Empty || Number == "" || Number == String.Empty || SerialNumber == "" || SerialNumber == String.Empty || Position == "" || Position == String.Empty || Name == "" || Name == String.Empty || Date == null)
                {
                    //this.ShowMessageAsync(new DispensingDialog("知道了!", "请到柜台取药!", null, null, "wait"));
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "有空参数，请检查！！", null, null, "wait"));
                    return;
                }
                if (Date <= DateTime.Now)
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "有效期小于等于当前日期\n请重新选则日期！", null, null, "wait"));
                    Console.WriteLine(Date);
                    return;
                }
                TaskList taskList = new TaskList
                {
                    code = "",
                    taskTime = DateTime.Now,
                    type = 9,
                    getEnd = false,
                    getSuc = false,
                    strState = "",
                    endTime = null,
                    confirmState = true,
                    patName = DosingAdministration.doctorName
                };
                taskList.drugTaskDetailList = new List<DrugTaskDetail>();
                taskList.drugTaskDetailList.Add(new DrugTaskDetail
                {
                    level = Convert.ToInt32(Position),
                    boxId = Id,
                    medicineId = Number,
                    x = Long,
                    y = Height,
                    z = Width,
                    name = Name,
                    quantity = Stock,
                    mark = "",
                    unit = "盒",
                    batch = SerialNumber,
                    validity = Date,
                    machine = Machine
                });
                DosingAdministration.m_socket.MsgReceived += SocketOperate;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
                string socketStr = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                DosingAdministration.m_socket.Send(socketStr);

                DateTime dtNow = DateTime.Now;
                while (true)
                {
                    if (DateTime.Now.Subtract(dtNow).TotalMilliseconds > 15000)
                    {
                        DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "终端返回加药信息超时\n请重试", null, null, "wait"));
                        DosingAdministration.m_socket.MsgReceived -= SocketOperate;

                        if (exists)
                        {
                            //DataRollBack(existsModel);
                        }
                        //DataRollBack(oldLayerMedicineInfo);

                        return;
                    }
                    if (!medicAddResultRecived)
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        medicAddResultRecived = false;
                        break;
                    }
                }
                if (!recvTaskList.getSuc)
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "终端返回加药失败，请重试\n错误信息: " + recvTaskList.strState, null, null, "wait"));
                    DosingAdministration.m_socket.MsgReceived -= SocketOperate;
                    //DataRollBack(oldLayerMedicineInfo);
                    if (exists)
                    {
                        //DataRollBack(existsModel);
                    }
                    return;
                }
                MedicineInBox medc = new MedicineInBox
                {
                    ItemId = Number,
                    BoxId = Id,
                    Id = Number,
                    BatchNumber = SerialNumber,
                    Level = Convert.ToInt32(Position),
                    ExpireDate = Date,
                    Name = Name,
                    Unit = "盒",
                    Quantity = Stock,//Convert.ToInt32(Quantity.Text),
                    X = Long,
                    Y = Height,
                    Z = Width,
                    Machine = Machine
                };
                DosingAdministration.m_socket.MsgReceived -= SocketOperate;

                if (exists)
                {
                    ModifyEventHandle?.Invoke(this);
                    m_sql.UpdateMedicine(medc);
                }
                else
                {
                    AddEventHandle?.Invoke(this);
                    m_sql.AddMedicineBox(medc);
                    //DataRollBack(existsModel);
                }
                DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "加药成功", null, null, "wait"));
                
                //managerModel.MockDatas();
            });

            DeleteCommand = new SampleCommand(o => true, o =>
            {
                //DeleteEventHandle?.Invoke(this);
                //return;
                //return;
                // 处理删除的逻辑
                LayerContent layerContent = o as LayerContent;
                if (Id == "" || Id == String.Empty || Number == "" || Number == String.Empty || SerialNumber == "" || SerialNumber == String.Empty || Position == "" || Position == String.Empty || Name == "" || Name == String.Empty || Date == null)
                {
                    //this.ShowMessageAsync(new DispensingDialog("知道了!", "请到柜台取药!", null, null, "wait"));
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "请选择一个药品", null, null, "wait"));
                    return;
                }
                MedicModel existsModel = new MedicModel();
                m_sql.QueryOneMedicines(Id, out existsModel);

                TaskList taskList = new TaskList
                {
                    code = "",
                    taskTime = DateTime.Now,
                    type = 9,
                    getEnd = false,
                    getSuc = false,
                    strState = "",
                    endTime = null,
                    confirmState = true,
                    patName = DosingAdministration.doctorName
                };
                taskList.drugTaskDetailList = new List<DrugTaskDetail>();
                taskList.drugTaskDetailList.Add(new DrugTaskDetail
                {
                    level = Convert.ToInt32(Position),
                    boxId = Id,
                    medicineId = Number,
                    x = Long,
                    y = Height,
                    z = Width,
                    name = Name,
                    quantity = 0,
                    mark = "",
                    unit = "盒",
                    batch = SerialNumber,
                    validity = Date,
                    machine = Machine
                });
                DosingAdministration.m_socket.MsgReceived += SocketOperate;
                IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy-MM-dd HH:mm:ss.fff" };
                string socketStr = JsonConvert.SerializeObject(taskList, Formatting.Indented, timeConverter);
                DosingAdministration.m_socket.Send(socketStr);
                DateTime dtNow = DateTime.Now;
                while (true)
                {
                    if (DateTime.Now.Subtract(dtNow).TotalMilliseconds > 15000)
                    {
                        DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "终端返回加药信息超时，请重试", null, null, "wait"));
                        DosingAdministration.m_socket.MsgReceived -= SocketOperate;
                        //DataRollBack(existsModel);
                        //DataRollBack(oldLayerMedicineInfo);

                        return;
                    }
                    if (!medicAddResultRecived)
                    {
                        Thread.Sleep(100);
                    }
                    else
                    {
                        medicAddResultRecived = false;
                        break;
                    }
                }

                if (m_sql.DeleteMedic(existsModel.boxid))
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "删除成功", null, null, "wait"));
                    //managerModel.MockDatas();
                    DeleteEventHandle?.Invoke(this);
                }
                else
                {
                    DosingAdministration.dosingAdministration.ShowMessageAsync(new DispensingDialog("确定", "删除失败", null, null, "wait"));
                    
                }
                DosingAdministration.m_socket.MsgReceived -= SocketOperate;
            });
        }
        public void DataRollBack(MedicModel existsModel)
        {
            MessageBox.Show(existsModel.validity);
            SerialNumber = existsModel.batch;
            Name = existsModel.name;
            Date = DateTime.ParseExact(existsModel.validity, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            Long = existsModel.x;
            Width = existsModel.z;
            Height = existsModel.y;
            Stock = existsModel.qty;
            Number = existsModel.medid;
            Id = existsModel.boxid;
            Position = Convert.ToString(existsModel.level);
        }
        public void SocketOperate(string msg)
        {
            TaskList taskList = new TaskList();
            taskList = JsonConvert.DeserializeObject<TaskList>(msg);
            if (taskList.getEnd == true)
            {
                medicAddResultRecived = true;
                recvTaskList = taskList;
            }
        }

    }
}
