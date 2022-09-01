using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dispensing
{
    #region 单条处方信息获取
    public class PrescItemListItem
    {
        /// <summary>
        /// 处方明细号
        /// </summary>
        public string prescriptionItemCode { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string prescriptionNo { get; set; }
        /// <summary>
        /// 药品编号
        /// </summary>
        public string drugCode { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string drugName { get; set; }
        /// <summary>
        /// 药物规格
        /// </summary>
        public string drugSpec { get; set; }
        /// <summary>
        /// 项目数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 分装容量单位
        /// </summary>
        public string unit { get; set; }
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
        /// 处方状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 处方批注
        /// </summary>
        public string remark { get; set; }
    }

    public class SinglePresc
    {
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string equipNo { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string prescriptionNo { get; set; }
        /// <summary>
        /// 处方条码
        /// </summary>
        public string prescriptionBar { get; set; }
        /// <summary>
        /// 门诊流水号
        /// </summary>
        public string clinicCode { get; set; }
        /// <summary>
        /// 患者编号
        /// </summary>
        public string patCode { get; set; }
#nullable enable
        /// <summary>
        /// 患者名称
        /// </summary>
        public string? patName { get; set; }
        /// <summary>
        /// 患者性别
        /// </summary>
        public string? patGender { get; set; }
        /// <summary>
        /// 患者年龄
        /// </summary>
        public string? patAge { get; set; }
#nullable disable
        /// <summary>
        /// 科室编码
        /// </summary>
        public string deptCode { get; set; }
#nullable enable
        /// <summary>
        /// 科室名称
        /// </summary>
        public string? deptName { get; set; }
        /// <summary>
        /// 医生工号
        /// </summary>
        public string? doctorCode { get; set; }
        /// <summary>
        /// 医生姓名
        /// </summary>
        public string? doctorName { get; set; }
#nullable disable
        /// <summary>
        /// 开立时间
        /// </summary>
        public string enterDateTime { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 处方条目
        /// </summary>
        public List<PrescItemListItem> prescItemList { get; set; }
    }
    #endregion
    #region 处方执行回执
    public class PrescItemList
    {
        /// <summary>
        /// 药槽ID
        /// </summary>
        public string storageNo { get; set; }
        /// <summary>
        /// 处方明细号
        /// </summary>
        public string prescriptionItemCode { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string prescriptionNo { get; set; }
        /// <summary>
        /// 药品批次
        /// </summary>
        public string drugBatchNo { get; set; }
        /// <summary>
        /// 药品编号
        /// </summary>
        public string drugCode { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string drugName { get; set; }
        /// <summary>
        /// 项目数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 处方状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 异常说明
        /// </summary>
        public string remark { get; set; }
    }

    public class GetMedicPullData
    {
        /// <summary>
        /// 设备唯一编号
        /// </summary>
        public string equipNo { get; set; }
        /// <summary>
        /// 处方号
        /// </summary>
        public string prescriptionNo { get; set; }
        /// <summary>
        /// 开立时间
        /// </summary>
        public string createTime { get; set; }
        /// <summary>
        /// 执行状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 处方条目
        /// </summary>
        public List<PrescItemList> prescItemList { get; set; }
    }

    #endregion
    #region 柜机状态定时上报
    public class DeviceInfo
    {
        /// <summary>
        /// 药柜编号
        /// </summary>
        public string cupboardCode { get; set; }
        /// <summary>
        /// 柜机HIS编码
        /// </summary>
        public string cupboardName { get; set; }
        /// <summary>
        /// 柜机型号
        /// </summary>
        public string deviceType { get; set; }
        /// <summary>
        /// 科室编码
        /// </summary>
        public string deptCode { get; set; }
        /// <summary>
        /// 科室名称
        /// </summary>
        public string deptName { get; set; }
        /// <summary>
        /// 柜机MAC地址
        /// </summary>
        public string macAddr { get; set; }
        /// <summary>
        /// 柜机IP地址
        /// </summary>
        public string ip { get; set; }
        /// <summary>
        /// 设备状态
        /// </summary>
        public string status { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
    #endregion
    #region 加药信息上报
    public class AddMedicData
    {
        /// <summary>
        /// 药柜编号
        /// </summary>
        public string cupboardCode { get; set; }
        /// <summary>
        /// 储存单元编号
        /// </summary>
        public string storageNo { get; set; }
        /// <summary>
        /// 分区编号
        /// </summary>
        public int indexNo { get; set; }
        /// <summary>
        /// 容器类型
        /// </summary>
        public string boxType { get; set; }
        /// <summary>
        /// 药品编码
        /// </summary>
        public string drugCode { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string drugBatchNo { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        public DateTime drugValidDate { get; set; }
        /// <summary>
        /// 库存数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 补药人
        /// </summary>
        public string addUser { get; set; }
        /// <summary>
        /// 补药时间
        /// </summary>
        public DateTime addTime { get; set; }
    }
    #endregion
    #region 储位状态定时上报
    public class StorageInfo
    {
        /// <summary>
        /// 药柜编号
        /// </summary>
        public string cupboardCode { get; set; }
        /// <summary>
        /// 药箱单元编号
        /// </summary>
        public string storageNo { get; set; }
        /// <summary>
        /// 药箱类型
        /// </summary>
        public string boxType { get; set; }
        /// <summary>
        /// 药箱坐标位置
        /// </summary>
        public int position { get; set; }
        /// <summary>
        /// 是否占用
        /// </summary>
        public int isUsed { get; set; }
        /// <summary>
        /// 工作状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
    #endregion
    #region 柜机药品库存盘点信息上报
    public class StorageDrug
    {
        /// <summary>
        /// 药柜编号
        /// </summary>
        public string cupboardCode { get; set; }
        /// <summary>
        /// 储存单元编号
        /// </summary>
        public string storageNo { get; set; }
        /// <summary>
        /// 分区编号
        /// </summary>
        public int indexNo { get; set; }
        /// <summary>
        /// 储位状态
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 药品编码
        /// </summary>
        public string drugCode { get; set; }
        /// <summary>
        /// 药品名称
        /// </summary>
        public string drugName { get; set; }
        /// <summary>
        /// 库存总数量
        /// </summary>
        public int quantity { get; set; }
        /// <summary>
        /// 批号
        /// </summary>
        public string drugBatchNo { get; set; }
        /// <summary>
        /// 效期
        /// </summary>
        public DateTime validDate { get; set; }
        /// <summary>
        /// 托盘类型
        /// </summary>
        public string palletType { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime updateTime { get; set; }
    }
    #endregion
}
