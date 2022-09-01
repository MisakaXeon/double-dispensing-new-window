using dispensing.Expends;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace dispensing.ViewModel.Items
{
    public class LayerContent : INotifyPropertyChanged
    {
        sqlite m_sql = new sqlite();
        public event PropertyChangedEventHandler PropertyChanged;
        //public MedicineManagerModel m_manager;
        /// <summary>
        /// 层名称
        /// </summary>
        public string LayerName { get; set; }
        /// <summary>
        /// 层号
        /// </summary>
        public int LayerIndex { get; set; }
        /// <summary>
        /// 当前被选中的项
        /// </summary>
        public LayerMedicineInfo SelectedItemInfo { get; set; }
        /// <summary>
        /// 左侧药槽
        /// </summary>
        public ObservableCollection<LayerMedicineInfo> LeftMedicineSlot { get; set; } = new ObservableCollection<LayerMedicineInfo>();
        /// <summary>
        /// 右侧药槽
        /// </summary>
        public ObservableCollection<LayerMedicineInfo> RightMedicineSlot { get; set; } = new ObservableCollection<LayerMedicineInfo>();

        public LayerContent()
        {
            //SelectedItemInfo.managerModel = m_manager;
            m_sql.Init(1);
        }

        /// <summary>
        /// 当出发添加的项的时候
        /// </summary>
        /// <param name="item"></param>
        public void AddItemEvent(LayerMedicineInfo item)
        {
            if (item.Machine.Equals(1))
            {
                var info = item.CopyFromOnlyValue();
                LeftMedicineSlot.Add(info);
            }
            else
            {
                var info = item.CopyFromOnlyValue();
                RightMedicineSlot.Add(info);
            }
        }
        /// <summary>
        /// 修改事件
        /// </summary>
        /// <param name="item"></param>
        public void ModifyEvent(LayerMedicineInfo item)
        {
            if (item.Machine.Equals(1))
            {
                //var ret = LeftMedicineSlot.Where(v => v.Id.Trim().Equals(item.Id.Trim()));
                //if (ret.Count() >= 1)
                //{
                //    ret.First().AddMedicineCount = item.AddMedicineCount;
                //    ret.First().Date = item.Date;
                //    ret.First().Height = item.Height;
                //    ret.First().Width = item.Width;
                //    ret.First().Name = item.Name;
                //    ret.First().Long = item.Long;
                //    ret.First().Machine = item.Machine;
                //    ret.First().MedicineNumber = item.MedicineNumber;
                //    ret.First().SerialNumber = item.SerialNumber;
                //    ret.First().Stock = item.Stock;
                //    ret.First().Number = item.Number;
                //    ret.First().Position = item.Position;
                //}
                for (int i = 0; i < LeftMedicineSlot.Count; i++)
                {
                    if (LeftMedicineSlot[i].Id == item.Id)
                    {
                        LeftMedicineSlot[i] = item;
                        break;
                    }
                }
            }
            else
            {
                //var ret = RightMedicineSlot.Where(v => v.Id.Trim().Equals(item.Id.Trim()));
                //if (ret.Count() >= 1)
                //{
                //    ret.First().AddMedicineCount = item.AddMedicineCount;
                //    ret.First().Date = item.Date;
                //    ret.First().Height = item.Height;
                //    ret.First().Width = item.Width;
                //    ret.First().Name = item.Name;
                //    ret.First().Long = item.Long;
                //    ret.First().Machine = item.Machine;
                //    ret.First().MedicineNumber = item.MedicineNumber;
                //    ret.First().SerialNumber = item.SerialNumber;
                //    ret.First().Stock = item.Stock;
                //    ret.First().Number = item.Number;
                //    ret.First().Position = item.Position;
                //}
                for (int i = 0; i < RightMedicineSlot.Count; i++)
                {
                    if (RightMedicineSlot[i].Id == item.Id)
                    {
                        RightMedicineSlot[i] = item;
                        break;
                    }
                }
            }

        }

        /// <summary>
        /// 删除药品
        /// </summary>
        /// <param name="item"></param>
        public void DeleteItemEvent(LayerMedicineInfo item)
        {
            if (item.Machine.Equals(1))
            {
                for (int i = 0; i < LeftMedicineSlot.Count; i++)
                {
                    if (LeftMedicineSlot[i].Id == item.Id)
                    {
                        LeftMedicineSlot.Remove(LeftMedicineSlot[i]);
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < RightMedicineSlot.Count; i++)
                {
                    if (RightMedicineSlot[i].Id == item.Id)
                    {
                        RightMedicineSlot.Remove(RightMedicineSlot[i]);
                        break;
                    }
                }
            }
        }


        public void MockDatas(int i)
        {
            LeftMedicineSlot.Clear();
            RightMedicineSlot.Clear();
            ////测试专用
            //for (int j = 0; j < 10; j++)
            //{
            //    LeftMedicineSlot.Add(new LayerMedicineInfo()
            //    {
            //        Id = j + "_left",
            //        Number = j.ToString(),
            //        Position = LayerName,
            //        Name = "名称",
            //        Stock = 11,
            //        Date = DateTime.Now,
            //        SerialNumber = "111",
            //    });
            //    RightMedicineSlot.Add(new LayerMedicineInfo()
            //    {
            //        Id = j + "_right",
            //        Number = j.ToString(),
            //        Position = LayerName,
            //        Name = "名称",
            //        Stock = 11,
            //        Date = DateTime.Now,
            //        SerialNumber = "111",
            //    });
            //}
            //return;

            if (!m_sql.QueryAllMedicines(out List<MedicModel> model))
            {
                return;
            }
            foreach (var item in model)
            {
                if (item.machine == 1)
                {
                    if (item.level == i + 1)
                    {
                        LeftMedicineSlot.Add(new LayerMedicineInfo()
                        {
                            Id = item.boxid,
                            Number = item.medid,
                            Position = item.level.ToString(),
                            Name = item.name,
                            Stock = item.qty,
                            Date = DateTime.ParseExact(item.validity, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                            SerialNumber = item.batch,
                            Long = item.x,
                            Width = item.z,
                            Height = item.y,
                            Machine = item.machine,
                            AddEventHandle = AddItemEvent,
                            DeleteEventHandle = DeleteItemEvent,
                        });
                    }
                }
                else
                {
                    if (item.level == i + 1)
                    {
                        RightMedicineSlot.Add(new LayerMedicineInfo()
                        {
                            Id = item.boxid,
                            Number = item.medid,
                            Position = item.level.ToString(),
                            Name = item.name,
                            Stock = item.qty,
                            Date = DateTime.ParseExact(item.validity, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture),
                            SerialNumber = item.batch,
                            Long = item.x,
                            Width = item.z,
                            Height = item.y,
                            Machine = item.machine,
                            AddEventHandle = AddItemEvent,
                            DeleteEventHandle = DeleteItemEvent,
                        });
                    }
                }
            }

        }
    }
}
