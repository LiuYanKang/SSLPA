using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace SSLPA.BaseInfo.DTO
{
    [DataContract]
    public class MProduct
    {
        [DataMember]
        public int? ProdID { get; set; }
        [DataMember]
        public int CustID { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string CustCode { get; set; }
        [DataMember]
        public double Weight { get; set; }
        [DataMember]
        public bool IsDSA { get; set; }
        [DataMember]
        public bool IsSLM { get; set; }
        [DataMember]
        public string BtwFile { get; set; }
        [DataMember]
        public Nullable<bool> BtwIsConst { get; set; }
        [DataMember]
        public string Remark { get; set; }
        [DataMember]
        public string CustomerName { get; set; }
        [DataMember]
        public string CustomerCode { get; set; }

        [DataMember]
        public string LocCode { get; set; }
        [DataMember]
        public MSemiProduct[] SemiProduct { get; set; }
        [DataMember]
        public int[] SemiProdID { get; set; }

        /// <summary>
        /// 工装关联
        /// </summary>
        [DataMember]
        public MProdAbrMap[] Abrasives { get; set; }
        [DataMember]
        public int[] AbrasivesID { get; set; }
    }
}
