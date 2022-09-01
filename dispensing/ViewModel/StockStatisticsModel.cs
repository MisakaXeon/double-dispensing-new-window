using dispensing.CustomCommand;
using dispensing.ViewModel.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace dispensing.ViewModel
{
    /// <summary>
    /// 库存统计模型
    /// </summary>
    public class StockStatisticsModel : INotifyPropertyChanged
    {
        sqlite m_sql = new sqlite();
        IniFile m_ini = new IniFile("");
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 查询命令
        /// </summary>
        public ICommand SearchCommand { get; set; }
        /// <summary>
        /// 根据输入框内容进行查询
        /// </summary>
        public ICommand ContentSearchCommand { get; set; }
        /// <summary>
        /// 查询类型方式
        /// </summary>
        public string SearchTypeName { get; set; } = "全部";
        /// <summary>
        /// 查询内容
        /// </summary>
        public string SearchContent { get; set; }
        /// <summary>
        ///  药品名称下拉列表
        /// </summary>
        public string[] DrugNames { get; set; } = new string[] { "药品ID", "药品名", "药槽ID", "拼音码" };
        /// <summary>
        /// 被选中的药品名称
        /// </summary>
        public string SelectedDrugName { get; set; } = "药品ID";
        /// <summary>
        /// 药品列表
        /// </summary>
        public ObservableCollection<LayerMedicineInfo> LayerMedicines { get; set; } = new ObservableCollection<LayerMedicineInfo>();

        public StockStatisticsModel()
        {
            m_sql.Init(1);
            SearchCommand = new SampleCommand(o => true, o =>
            {
                // 查询命令 代码
                String content = o as string;
                Console.WriteLine("选中的内容!" + content);
                if (content == "全部")
                {
                    LoadAllDatas();
                }
                else if (content == "近期效")
                {
                    LoadValityDatas();
                }
                else if (content == "低库存")
                {
                    LoadStockDatas();
                }
            });

            ContentSearchCommand = new SampleCommand(o => true, o =>
            {
                string searchContent = o as string;
                Console.WriteLine("查询内容 : " + searchContent);
                LayerMedicines.Clear();
                if (SelectedDrugName == "药品ID")
                {
                    SearchMedic(0, searchContent);
                }
                else if (SelectedDrugName == "药品名")
                {
                    SearchMedic(1, searchContent);
                }
                else if (SelectedDrugName == "药槽ID")
                {
                    SearchMedic(2, searchContent);
                }
                else if (SelectedDrugName == "拼音码")
                {
                    SearchMedic(3, searchContent);
                }
            });
            LoadAllDatas();
        }
        /// <summary>
        /// 搜索药品
        /// </summary>
        /// <param name="type">0：药品ID 1：药品名 2：药槽ID 3：拼音码</param>
        public void SearchMedic(int type, string searchContent)
        {
            if (!m_sql.QueryAllMedicines(out List<MedicModel> model))
            {
                return;
            }
            List<MedicModel> searchData = new List<MedicModel>();
            foreach (var item in model)
            {
                switch (type)
                {
                    case 0:
                        //药品ID筛选
                        if (item.medid == searchContent.Trim()) searchData.Add(item);
                        break;
                    case 1:
                        //药品名筛选
                        if (item.name.Contains(searchContent.Trim())) searchData.Add(item);
                        break;
                    case 2:
                        //药槽ID筛选
                        if (item.boxid == searchContent.Trim()) searchData.Add(item);
                        break;
                    case 3:
                        //拼音码筛选
                        string upSearchContent = searchContent.Trim().ToUpper();
                        string upMedicSpellStr = GetSpellCode(item.name).Trim().ToUpper();
                        if (upMedicSpellStr.Contains(upSearchContent)) searchData.Add(item);
                        break;
                    default:
                        break;
                }
            }
            RefreshSearchData(searchData);
        }
        public void RefreshSearchData(List<MedicModel> searchData)
        {
            foreach (var item in searchData)
            {
                LayerMedicines.Add(new LayerMedicineInfo()
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
                    Machine = item.machine
                });
            }
        }
        public void LoadAllDatas()
        {
            if (!m_sql.QueryAllMedicines(out List<MedicModel> model))
            {
                return;
            }
            LayerMedicines.Clear();
            foreach (var item in model)
            {
                LayerMedicines.Add(new LayerMedicineInfo()
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
                    Machine = item.machine
                });
            }
        }
        public void LoadStockDatas()
        {
            if (!m_sql.QueryAllMedicines(out List<MedicModel> model))
            {
                return;
            }
            LayerMedicines.Clear();
            foreach (var item in model)
            {
                if (item.qty <= m_ini.IniReadIntValue("MedicAdd", "StockThreshold"))
                {
                    LayerMedicines.Add(new LayerMedicineInfo()
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
                        Machine = item.machine
                    });
                }
            }
        }
        public void LoadValityDatas()
        {
            if (!m_sql.QueryAllMedicines(out List<MedicModel> model))
            {
                return;
            }
            LayerMedicines.Clear();
            foreach (var item in model)
            {
                int dateNow = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
                int dateVality = Convert.ToInt32(item.validity);
                if (dateVality - dateNow <= m_ini.IniReadIntValue("MedicAdd", "ValityThreshold"))
                {
                    LayerMedicines.Add(new LayerMedicineInfo()
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
                        Machine = item.machine
                    });
                }
            }
        }
        /// <summary>
        /// 在指定的字符串列表CnStr中检索符合拼音索引字符串
        /// </summary>
        /// <param name="CnStr">汉字字符串</param>
        /// <returns>相对应的汉语拼音首字母串</returns>
        public static string GetSpellCode(string CnStr)
        {
            string strTemp = "";
            int iLen = CnStr.Length;
            for (int i = 0; i <= iLen - 1; i++)
            {
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1));
                //break;
            }
            return strTemp;
        }
        /// <summary>
        /// 得到一个汉字的拼音第一个字母，如果是一个英文字母则直接返回大写字母
        /// </summary>
        /// <param name="CnChar">单个汉字</param>
        /// <returns>单个大写字母</returns>
        private static string GetCharSpellCode(string CnChar)
        {
            long iCnChar;
            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);
            //如果是字母，则直接返回
            if (ZW.Length == 1)
            {
                return CnChar.ToUpper();
            }
            else
            {
                // get the array of byte from the single char
                int i1 = (short)(ZW[0]);
                int i2 = (short)(ZW[1]);
                iCnChar = i1 * 256 + i2;
            }
            // iCnChar match the constant
            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {
                return "A";
            }
            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {
                return "B";
            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {
                return "C";
            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {
                return "D";
            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {
                return "E";
            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {
                return "F";
            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {
                return "G";
            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {
                return "H";
            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {
                return "J";
            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {
                return "K";
            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {
                return "L";
            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {
                return "M";
            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {
                return "N";
            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {
                return "O";
            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {
                return "P";
            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {
                return "Q";
            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {
                return "R";
            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {
                return "S";
            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {
                return "T";
            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {
                return "W";
            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {
                return "X";
            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {
                return "Y";
            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {
                return "Z";
            }
            else
                return ("?");
        }
    }
}
