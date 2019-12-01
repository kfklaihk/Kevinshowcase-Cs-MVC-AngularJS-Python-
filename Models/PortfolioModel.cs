using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class StkHoldingModel
    {
        [Key,DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid stkhldgGUID { get; set; }
        public  ApplicationUser User { get; set; }
        public string datetme { get; set; }
        public int shares { get; set; }

       
      //  public  ICollection<StklistModel> StklistModels { get; set; }
       public  StklistModel Stklist { get; set; }

        public string remark { get; set; }
        
    }

    public class CashholdingModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid cashhldgGUID { get; set; }
        public  ApplicationUser User { get; set; }
        
        public string datetme { get; set; }
        public int freecash { get; set; }
        public int margin { get; set; }
        public string remark { get; set; }

    }

    public class StkModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid stkGUID { get; set; }
        public string stkcode { get; set; }
        public string name { get; set; }
        public string datetme { get; set; }
        public decimal oprice { get; set; }
        public decimal hprice { get; set; }
        public decimal lprice { get; set; }
        public decimal cprice { get; set; }
        public decimal dividend { get; set; }
 //       public string currency { get; set; }
        public decimal xrate { get; set; }
        public string remark { get; set; }

     
      //  public Guid StklistGUID;
   //     public  ICollection<StklistModel> StklistModels { get; set; }
        public  StklistModel Stklist { get; set; }
    }

    public class txModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid txGUID { get; set; }
        public  ApplicationUser User { get; set; }
        public string datetme { get; set; }
        public string hrmmtme { get; set; }
        public string buysell { get; set; }
        [Required(ErrorMessage="Number of Shares cannot be blank")]
        public int shares { get; set; }

     
    //    public  ICollection<StklistModel> StklistModels { get; set; }
       public  StklistModel Stklist { get; set; }
      [Required(ErrorMessage = "Price cannot be blank")]
      [Range(0, 100000, ErrorMessage ="Price must be greater than 0")]
        public decimal price { get; set; }
  //    public string currency { get; set; }
        public decimal xrate { get; set; }
        public string remark { get; set; }

    }

    public class Pending_txModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid txGUID { get; set; }
        public  ApplicationUser User { get; set; }
        public string datetme { get; set; }
        public string hrmmtme { get; set; }
        public string buysell { get; set; }
        public int shares { get; set; }

       
        
      //  public  ICollection<StklistModel> StklistModels { get; set; }
          public  StklistModel Stklist { get; set; }
        public decimal price { get; set; }
   //     public string currency { get; set; }
         public string remark { get; set; }

    }


    public class StklistModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid StklistGUID { get; set; }
        public string stkcode { get; set; }
        public string name { get; set; }
        public string market { get; set; }
        public string currency { get; set; }
        public string remark { get; set; }

       

        public  ICollection<StkHoldingModel> StkholdingModels { get; set; }
        
        public  ICollection<StkModel> StkModels { get; set; }
       
        public  ICollection<txModel> txModels { get; set; }

        public  ICollection<Pending_txModel> Pending_txModels { get; set; }

        
        public Recommend_Model Recommendlist { get; set; }
    }

    public class MM_Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid MMGUID { get; set; }
        public ApplicationUser User { get; set; }

        public string datetme { get; set; }
        public int stk_val { get; set; }
        public int stk_val_h { get; set; }
        public int stk_val_l { get; set; }
        public int freecash { get; set; }
        public int margin { get; set; }
        public int HSI { get; set; }
        public string remark { get; set; }

    }

    public class Recommend_Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
         public Guid RMGUID { get; set; }

        
        

        public string remark { get; set; }
        public ICollection<details_Model> details_Models { get; set; }


        [Required]
        public StklistModel Stklist { get; set; }
    }

    public class Analyst_Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AnaGUID { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public string remark { get; set; }
        public ICollection<details_Model> details_Models { get; set; }

    }


    public class details_Model
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DetailsGUID { get; set; }
        public string datetme { get; set; }
        public Recommend_Model Recommendlist { get; set; }
        public Analyst_Model Analyst_list { get; set; }
        public string recommendation { get; set; }
        public string remark { get; set; }

    }

    public class M6_Model
    {
        [Key]
        public string id { get; set; }
        public string datetme { get; set; }
        public string Y1 { get; set; }
        public string Y2 { get; set; }
        public string M1 { get; set; }
        public string M2 { get; set; }
        public string D1 { get; set; }
        public string D2 { get; set; }
        public  int no1  { get; set; }
         public int no2 { get; set; }
        public int no3 { get; set; }
        public int no4 { get; set; }
        public int no5 { get; set; }
        public int no6 { get; set; }
        public int sno { get; set; }
        public string remark { get; set; }
        public string c1 { get; set; }
        public string c2 { get; set; }
        public string c3 { get; set; }
        public string c4 { get; set; }
        public string c5 { get; set; }
        public string c6 { get; set; }
        public string c7 { get; set; }
    }



}