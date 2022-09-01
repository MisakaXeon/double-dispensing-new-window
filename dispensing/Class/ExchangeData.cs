using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing
{

    public class TaskList
    {
        /// <summary>
        /// 条码
        /// </summary>
        public string code { get; set; }
        /// <summary>
        /// 任务创建时间
        /// </summary>
        public DateTime taskTime { get; set; }
        /// <summary>
        /// 传输类型 0：分配前台任务 1：分配后台任务 2:前台确认 3：前台等待 4：条码回传 5：确认回传 6：任务出错 7：接收另一台机器的发药结果兼结束标志 8:单机发药结束标志 9：传送加药信息 10：电机回零 11：Y轴控制 12:流程最终结束
        /// </summary>
        public int type { get; set; }
        /// <summary>
        /// 任务是否结束
        /// </summary>
        public bool getEnd { get; set; }
        /// <summary>
        /// 是否发药成功
        /// </summary>
        public bool getSuc { get; set; }
        /// <summary>
        /// 发药状态详情(回调)
        /// </summary>
        public string strState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? endTime { get; set; }
        /// <summary>
        /// 确认状态
        /// </summary>
        public bool confirmState { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
#nullable enable
        public string? patName { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string? patAge { get; set; }
        /// <summary>
        /// 患者姓名
        /// </summary>
        public string? patGender { get; set; }
        /// <summary>
        /// Y轴位置
        /// </summary>
#nullable disable
        public int yPosition { get; set; }
        /// <summary>
        /// 处方开立时间
        /// </summary>
        public DateTime createTime { get; set; }
        /// <summary>
        /// 加药明细(null)
        /// </summary>
        public List<DrugTaskDetail> drugTaskDetailList { get; set; }
    }
    /// <summary>
    /// 发药明细列表
    /// </summary>
    public class DrugTaskDetail
    {
        /// <summary>
        /// 层
        /// </summary>
        public int level { get; set; }
        /// <summary>
        /// 药槽ID
        /// </summary>
        public string boxId { get; set; }
        /// <summary>
        /// 药盒ID
        /// </summary>
        public string medicineId { get; set; }
        /// <summary>
        /// X
        /// </summary>
        public int x { get; set; }
        /// <summary>
        /// Y
        /// </summary>
        public int y { get; set; }
        /// <summary>
        /// Z
        /// </summary>
        public int z { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string mark { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public string unit { get; set; }
        /// <summary>
        /// 批次
        /// </summary>
        public string batch { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime validity { get; set; }
        /// <summary>
        /// 频次
        /// </summary>
        public string frequency { get; set; }
        /// <summary>
        /// 用法
        /// </summary>
        public string administration { get; set; }
        /// <summary>
        /// 单次剂量
        /// </summary>
        public string dosage { get; set; }
        /// <summary>
        /// 单次剂量单位
        /// </summary>
        public string dosageUnit { get; set; }
        /// <summary>
        /// 机器号
        /// </summary>
        public int machine { get; set; }
        /// <summary>
        /// 发药成功的数量
        /// </summary>
        public int provideSucQty { get; set; } = 0;
    }

}
