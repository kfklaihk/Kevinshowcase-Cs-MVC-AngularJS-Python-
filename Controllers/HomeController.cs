using System;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using WebApplication1.Models;
using WebApplication1.ViewModels;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
using System.Data.Entity.Core.Objects;
using System.Data.Entity;
using System.Diagnostics;
using System.Globalization;
using System.Web.Helpers;

namespace WebApplication1.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        private ApplicationDbContext db = ApplicationDbContext.Create();
        private ViewModel_1 vmodel = new ViewModel_1();

        [Authorize]
        public ActionResult portfolio(string datme,Boolean first=false)
        {
            var id = User.Identity.GetUserId();
            var userid = db.Users.FirstOrDefault(x => x.Id==id ).userid;
            IQueryable<StkHoldingModel> stkhldg , p_w_stklist;
            IQueryable<txModel> tx, t_w_stklist;
            IQueryable<Pending_txModel> pending_tx, pend_w_stklist;
            bool load_success = true;
            string dt = (DateTime.ParseExact(datme, "yyyyMMdd", CultureInfo.InvariantCulture)).AddDays(0).ToString("yyyyMMdd");
            TempData["dt"] = dt;

            if (first==true)
            {
                db.CashHoldingModels.Add(new CashholdingModel { datetme = DateTime.Now.ToString("yyyyMMdd"), freecash = 10000000, margin = 0, User = db.Users.FirstOrDefault(x => x.Id == id) });
                db.SaveChanges();
            }

            var p_query = from x in db.StkHoldingModels
                           where x.User.userid == userid && x.datetme.CompareTo(datme) <= 0
                           group x by x.Stklist into a
                           select a.OrderByDescending(y => y.datetme).FirstOrDefault() ;

            try
            {

                 stkhldg = p_query.Include(p => p.Stklist);

            }
            catch  (Exception e)
            {
               Debug.WriteLine(e.Message);
                stkhldg = p_query;
                load_success = false;
            }

            if (load_success == true)
            {
                p_w_stklist = stkhldg;
                try
                {
                    
                    stkhldg = from y in p_query.Include(p => p.Stklist).Include(u => u.Stklist.StkModels)//.Where(u => u.Stklist.StkModels.Any(f => f.datetme == dt))
                              select y;
                    
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    stkhldg = p_w_stklist;
                    load_success = false;
                }
            }

            load_success = true;

            var cashhldg = (from x in db.CashHoldingModels
                            where x.User.userid == userid && x.datetme.CompareTo(datme) <= 0
                            orderby x.datetme descending
                            select x).Take(1);

            var t_query = from x in db.txModels
                          where x.User.userid == userid && x.datetme == datme
                          select x;


            try
            {

                tx = (t_query ).Include(p => p.Stklist);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                tx = t_query;
                load_success = false;
            }


            load_success = true;

            var pend_query = from x in db.Pending_txModels
                             where x.User.userid == userid && x.datetme == datme
                             select x;

            try
            {

                pending_tx = (pend_query ).Include(p => p.Stklist);

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                pending_tx = pend_query;
                load_success = false;
            }
     
            ViewModel_1 mymodel = vmodel;
            mymodel.StkHoldingModels = stkhldg;
            mymodel.CashHoldingModels = cashhldg;
            mymodel.txModels = tx;
            mymodel.Pending_txModels = pending_tx;
          
            mymodel.CurrentUser = userid;
            mymodel.datme = datme;
          
          
                return View(mymodel);
            
        }


        [Authorize]
       
        public ActionResult portfolio_partial(string datme)
        {
            var id = User.Identity.GetUserId();
            var userid = db.Users.FirstOrDefault(x => x.Id == id).userid;
            string dt = (DateTime.ParseExact(datme, "yyyyMMdd", CultureInfo.InvariantCulture)).AddDays(0).ToString("yyyyMMdd");
            TempData["dt"] = dt;
            var stkhldg = from y in ((from x in db.StkHoldingModels
                            where x.User.userid == userid && x.datetme.CompareTo(datme) <= 0
                            group x by x.Stklist into a
                            select a.OrderByDescending(y => y.datetme).FirstOrDefault()) ).Include( p => p.Stklist).Include(u => u.Stklist.StkModels)//.Where(u => u.Stklist.StkModels.Any(f => f.datetme == dt))
                          select y;
                       

            var cashhldg = (from x in db.CashHoldingModels
                            where x.User.userid == userid && x.datetme.CompareTo(datme) <= 0
                            orderby x.datetme descending
                            select x).Take(1);

            var tx = ((from x in db.txModels
                       where x.User.userid == userid && x.datetme == datme
                       select x)).Include(p => p.Stklist);



            var pending_tx = ((from x in db.Pending_txModels
                               where x.User.userid == userid && x.datetme == datme
                               select x)).Include(p => p.Stklist);


            ViewModel_1 mymodel = vmodel;
            mymodel.StkHoldingModels = stkhldg;
            mymodel.CashHoldingModels = cashhldg;
            mymodel.txModels = tx;
            mymodel.Pending_txModels = pending_tx;
        mymodel.CurrentUser = userid;
            mymodel.datme = datme;

            return PartialView("~/views/Portfolio_partial.cshtml", mymodel);

        }

        public StkModel getprice(string stk_code, string xchange)
        {
            string stkcode;
            StkModel mymodel = new StkModel();
            String astring = "";
            HttpWebRequest myWebRequest;
            HttpWebResponse myWebResponse;
            String URL = "", userRequest = "";


            if (xchange == "HK")
            {
                stkcode = stk_code.Replace(".HK","").PadLeft(5, '0');
          
                userRequest = @"{
                 'url': 'http://money18.on.cc/info/hk/liveinfo_quote_" + stkcode + @".html',
        'requestSettings': {
		   'maxwait' : 50000
		},
		'renderType': 'plaintext',
'outputAsJson':false
						}";



                URL = "http://api.phantomjscloud.com/api/browser/v2/ak-moh6m-5qdy2-cmmmm-fvcfj-65ew1/";
            }
            else if (xchange == "CN")
            {
                stkcode = stk_code.Replace(".SS", "").Replace(".SZ", "").PadLeft(6, '0');
                userRequest = @"{
                 'url': 'http://www.etnet.com.hk/www/tc/ashares/quote.php?code=" + stkcode + @"',
        'requestSettings': {
		   'maxwait' : 35000
		},
		'renderType': 'plaintext',
		'outputAsJson':false,
       'scripts': {
                    'domReady': [

                    'https://cdnjs.cloudflare.com/ajax/libs/jquery/3.3.1/jquery.min.js',
                   
                    '$(""#refresh"").click();',
                 	'_pjscMeta.manualWait=true;'

],
                    'loadFinished': [ 
                    'setInterval(function(){ var txt=$(""#content.data"").first().text(); if(txt!=null){_pjscMeta.manualWait=false;} },200)'
                    ]
                 }
				}";
                URL = "http://api.phantomjscloud.com/api/browser/v2/ak-moh6m-5qdy2-cmmmm-fvcfj-65ew1/";
            }
            else if (xchange == "US")
            {
                stkcode = stk_code;
                userRequest = @"{
                 'url': 'http://www.aastocks.com/en/usq/quote/quote.aspx?symbol=" + stkcode + @"',
        'requestSettings': {
		   'maxwait' : 35000
		},
		'renderType': 'plaintext',
		'outputAsJson':false
       				}";
                URL = "http://api.phantomjscloud.com/api/browser/v2/ak-moh6m-5qdy2-cmmmm-fvcfj-65ew1/";

            }


            myWebRequest = (HttpWebRequest)WebRequest.Create(URL);
            if (xchange == "CN" || xchange == "HK" || xchange == "US")
            {
                myWebRequest.ContentType = "application/json";
                myWebRequest.Method = "POST";
                myWebRequest.Timeout = 40000; //60 seconds
                myWebRequest.KeepAlive = false;
                myWebRequest.MediaType = "application/json";
                myWebRequest.ServicePoint.Expect100Continue = false;
                userRequest = userRequest.Replace("\r\n", "").Replace(" ", "").Replace("\t", "");


                using (var streamWriter = new System.IO.StreamWriter(myWebRequest.GetRequestStream()))
                {

                    string pageRequestJson = userRequest;
                    streamWriter.Write(pageRequestJson);
                    streamWriter.Flush();
                    streamWriter.Close();
                }
            }

            try
            {
                myWebResponse = (HttpWebResponse)myWebRequest.GetResponse();
            }
            catch (Exception Ex)
            {
                mymodel.cprice = -1;
                mymodel.name = Ex.Message;
                mymodel.stkcode = stk_code;
                return  mymodel;
            }
            if (myWebResponse.StatusCode != HttpStatusCode.OK)
            {
                mymodel.cprice = -1;
                mymodel.name = "Unsuccessful Load";
                mymodel.stkcode = stk_code;
                return mymodel;
            }


            Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader sreader = new StreamReader(streamResponse);
            astring = sreader.ReadToEnd();



            String inner_text, substr, subsubstr;


            var doc = new HtmlDocument();
            doc.LoadHtml(astring);
            inner_text = doc.DocumentNode.InnerHtml;

            if (xchange == "HK")
            {

				try
				{
                    int i = 0;
                    while (!astring.Split('\n')[i].Trim().Contains("現價"))
                    { i++; }

                    substr = astring.Split('\n')[i-3];


                    int pFrom = astring.Split('\n')[i].IndexOf("現價") + "現價".Length;
                    int pTo = astring.Split('\n')[i].LastIndexOf("最高價");

                    mymodel.cprice = Convert.ToDecimal(astring.Split('\n')[i].Substring(pFrom, pTo - pFrom).Trim());

                    mymodel.name = substr.Trim();
					mymodel.stkcode = stk_code;
					return mymodel;
				}
				catch (Exception Ex)
				{
					mymodel.cprice = -1;
					mymodel.name = Ex.Message;
					mymodel.stkcode = stk_code;
					return mymodel;
				}


			}


            if (xchange == "CN")
            {
                string str;

                if (stk_code.Substring(stk_code.Length - 3) == ".SS")
                {
                    str = "滬股通股票";
                }
                else
                {

                    str = "深股通股票";
                }
				try
				{
                    int i = 0;
                    while (!astring.Split('\n')[i].Trim().Contains(str))
                    { i++; }

                    substr = astring.Split('\n')[i-2];


					mymodel.cprice = Convert.ToDecimal(astring.Split('\n')[i+1].Replace("&nbsp;", "").Trim());

					mymodel.name = substr.Substring(8).Trim();
					mymodel.stkcode = stk_code;
					return mymodel;
				}
				catch (Exception Ex)
				{
					mymodel.cprice = -1;
					mymodel.name = Ex.Message;
					mymodel.stkcode = stk_code;
					return mymodel;
				}

			}

			try
			{
                int i = 0;
                while (astring.Split('\n')[i].Trim()!="Symbol")
                { i++; }
				substr = astring.Split('\n')[i+1];
				subsubstr = substr.Split('(')[1].Trim();

				mymodel.cprice = Convert.ToDecimal(astring.Split('\n')[i+6].Replace("&nbsp;", "").Trim());

				mymodel.name = substr.Split('(')[0].Trim();
				mymodel.stkcode = stk_code;
				return mymodel;
			}
			catch (Exception Ex)
			{
				mymodel.cprice = -1;
				mymodel.name = Ex.Message;
				mymodel.stkcode = stk_code;
				return mymodel;
			}


		}


        [Authorize]
        public ActionResult m6(string yr)
        {
          
            var q = from x in db.M6_Models
                    where x.datetme.Substring(0,4)==yr
                    orderby x.datetme descending
                    select x;

            return View(q.ToList()); 


        }

        [Authorize]
        public ActionResult stkquote_partial(string stkcode, string xchange)
        {
            StkModel mymodel = new StkModel();
            mymodel = getprice(stkcode, xchange);
            return PartialView("~/views/Stkquote_partial.cshtml", mymodel);


        }

        [Authorize]
        public JsonResult initial_load_Ana()
        {
            
        


           

            var q = from x in db.Analyst_Models
                    select x.name;


            return Json(q.ToList(), JsonRequestBehavior.AllowGet); 


        }

        [Authorize]
        public JsonResult initial_load_Rec()
        {
           
            


            var q = from x in db.Recommend_Models.Include(y => y.Stklist)
                    orderby x.Stklist.stkcode
                    select x.Stklist.stkcode;




            return Json(q.ToList(), JsonRequestBehavior.AllowGet);


        }

        [Authorize]
        public JsonResult load_chart(string stkcode)
        {
            


            var q = from x in db.StkModels
                    where x.stkcode==stkcode && x.cprice <= x.oprice
                    select new { datetme=x.datetme, S1=x.hprice, S2=x.oprice, S3=x.cprice,S4=x.lprice };


            var p = from x in db.StkModels
                    where x.stkcode == stkcode && x.cprice > x.oprice
                    select new { datetme = x.datetme, S1 = x.lprice, S2 = x.oprice, S3 = x.cprice, S4 = x.hprice };

            var r = (from x in q select x).Concat(from x in p select x);
            var s = (from x in r orderby x.datetme select x);

            return Json(s.ToList(), JsonRequestBehavior.AllowGet);


        }



        [Authorize]
        public JsonResult load_recommend(int typ,string arg)
        {

            if (typ==0)
            {
                var q = from x in db.details_Models.Include(p=>p.Analyst_list).Include(u=>u.Recommendlist)
                        from y in db.StklistModels
                        where x.Analyst_list.name==arg && x.Recommendlist.RMGUID == y.StklistGUID
                        orderby x.datetme descending
                        select new { datetme = x.datetme, name=x.Analyst_list.name,stk=y.stkcode,stkname= y.name,rec=x.recommendation, rmk = x.remark };
                return Json(q.ToList(), JsonRequestBehavior.AllowGet);

            }
            else
            {
                var q = from x in db.details_Models.Include(p => p.Analyst_list).Include(u => u.Recommendlist)
                        from y in db.StklistModels
                        where y.stkcode == arg && x.Recommendlist.RMGUID == y.StklistGUID
                        orderby x.datetme descending
                        select new { datetme = x.datetme, name = x.Analyst_list.name, stk =y.stkcode, stkname = y.name, rec = x.recommendation, rmk = x.remark };

                return Json(q.ToList(), JsonRequestBehavior.AllowGet);


            }






           


        }


        [Authorize]
        public ActionResult portana()
        {
            IQueryable<MM_Model> mymodel ;
            var id = User.Identity.GetUserId();
            var userid = db.Users.FirstOrDefault(x => x.Id == id).userid;
            string dt = DateTime.Now.ToString("yyyyMMdd");
            mymodel = db.MM_Models.Where(x=>x.User.userid==userid && x.datetme.CompareTo(dt)<0).OrderBy(x=>x.datetme);
            return View( mymodel);


        }

        [Authorize]
        [HttpPost]
        public ActionResult Order(string xrate, string curr, string xchange, string stk_code, string stk_name, string stk_price, string shares, string bs, string otype)
        {
            bool s_new = false;
            bool c_new = false;
            string dt = DateTime.Now.ToString("yyyyMMdd");
            StklistModel stklist;
            txModel m;
            Pending_txModel n;
            StkHoldingModel stkhldg;
            var id = User.Identity.GetUserId();
            var curr_user = db.Users.FirstOrDefault(y => y.Id == id);
            // CashholdingModel cs = db.CashHoldingModels.Where(x => x.User == User).OrderByDescending(x => x.datetme).FirstOrDefault();
            CashholdingModel cs = (CashholdingModel)(from x in db.CashHoldingModels
                                                     where x.User.userid == curr_user.userid
                                                     orderby x.datetme descending
                                                     select x).ToList().FirstOrDefault();

            CashholdingModel cs_today;
            ViewModel_1 mymodel = vmodel;

            try {
                // cs_today= db.CashHoldingModels.Where(x => x.User == User && x.datetme == dt).FirstOrDefault();
                cs_today = (CashholdingModel)(from x in db.CashHoldingModels
                                              where x.User.userid == curr_user.userid && x.datetme == dt
                                              select x).ToList().FirstOrDefault();
            }
            catch (Exception e)
            { cs_today = null;
            }
            StkHoldingModel stkhldg_today;
            try
            {
                //  stkhldg_today = db.StkHoldingModels.Where(x2 => x2.User == User && x2.Stklist.stkcode == stk_code && x2.Stklist.market == xchange && x2.datetme == dt).OrderByDescending(x3 => x3.datetme).First();
                stkhldg_today = (StkHoldingModel)(from x in db.StkHoldingModels.Include(p => p.Stklist)
                                                  where x.User.userid == curr_user.userid && x.Stklist.stkcode == stk_code && x.Stklist.market == xchange && x.datetme == dt
                                                  select x).FirstOrDefault();

            }
            catch (Exception e)
            {
                stkhldg_today = null;
            }
            try
            {
                // stkhldg = db.StkHoldingModels.Where(x2 => x2.User == User && x2.Stklist.stkcode == stk_code && x2.Stklist.market == xchange).OrderByDescending(x3 => x3.datetme).First();
                stkhldg = (StkHoldingModel)(from x in db.StkHoldingModels.Include(p => p.Stklist)
                                            where x.User.userid == curr_user.userid && x.Stklist.stkcode == stk_code && x.Stklist.market == xchange
                                            orderby x.datetme descending
                                            select x).FirstOrDefault();

            }
            catch (Exception e)
            {
                stkhldg = null;
            }

            int cash = cs.freecash;
            int margin = cs.margin;
            int old_margin, free_up_margin;
            decimal last_stk_price;
            decimal last_xrate;
            Decimal stockprice = Convert.ToDecimal(stk_price);
            Decimal exrate = Convert.ToDecimal(xrate);
            int num_shares = Convert.ToInt32(shares);
            int new_shares;
          

            try
			{
              
                stklist = (StklistModel)(from g in db.StklistModels
                                         where g.stkcode == stk_code && g.market == xchange && g.currency == curr
                                         select g).FirstOrDefault();

            }
			catch (Exception e)
			{
				stklist = null;
			}
            
			if (stklist == null)

			{
				stklist = new StklistModel() { stkcode = stk_code, name = stk_name, market = xchange, currency = curr };
               


			}

			if (otype == "mkt")
            {
                    



                        switch (bs)
                         {
                                              case "Buy":

                                                  if (stkhldg == null)  // no stock

                                                  {
                                                      if (cash >= stockprice * num_shares * exrate)  //enough cash to buy
                                                      {
                                                          stkhldg_today = new StkHoldingModel() { User = curr_user, shares = num_shares, datetme = dt, Stklist = stklist };
                                                        s_new = true;
                                                          if (cs_today == null)
                                                          {
                                                         c_new = true;
                                                      cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate), margin = margin };

                                                          }
                                                          else
                                                          {
                                                              cs_today.freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate);
                                                                cs_today.margin = Convert.ToInt32(margin);
                                                          }
                                                      }
                                                      else
                                                      {
                                            m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate,remark= "Insufficient Cash to buy stock", User = curr_user, Stklist = stklist };
                                         db.txModels.Add(m);
                               
                                        db.SaveChanges();
                                        TempData["order_status"] = "Insufficient Cash to buy stock";
                                                          return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                                      }

                                                  }
                                                 else if (stkhldg.shares >= 0)  //net long
                                                  {
                                                      if (cash >= stockprice * num_shares * exrate)
                                                      {

                                                          if (stkhldg_today == null)
                                                          {
                                                         s_new = true;
                                                              stkhldg_today = new StkHoldingModel() { User = curr_user, shares = num_shares + stkhldg.shares, datetme = dt, Stklist = stklist };
                                                          }
                                                          else
                                                          {
                                                              stkhldg_today.shares = num_shares + stkhldg.shares;
                                                          }


                                                          if (cs_today == null)
                                                          {
                                                              cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate), margin = margin };
                                    c_new = true;
                                }
                                                          else
                                                          {
                                                              cs_today.freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate);
                                                            cs_today.margin = Convert.ToInt32(margin);

                                                          }


                                                      }
                                                      else
                                                      {

                                m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark = "Insufficient Cash to buy stock", User = curr_user, Stklist = stklist };
                                db.txModels.Add(m);

                                db.SaveChanges();

                                TempData["order_status"] =  "Insufficient Cash to buy stock";
                                                          return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                                      }

                                                  }
                                                  else  // net short 
                                                  {
                                                      if (-stkhldg.shares > num_shares) // still short pos
                                                      {
                                                   //       old_margin = Convert.ToInt32(-stkhldg.shares * last_stk_price * last_xrate / 2);
                                                          new_shares = stkhldg.shares + num_shares;
                                //         free_up_margin = Convert.ToInt32(old_margin + new_shares * stockprice * exrate / 2);
                                old_margin = 0;  free_up_margin = 0;
                                                              if (stkhldg_today == null)
                                                              {
                                    s_new = true;
                                    stkhldg_today = new StkHoldingModel() { User = curr_user, shares = new_shares, datetme = dt, Stklist = stklist };
                                                              }
                                                              else
                                                              {
                                                                  stkhldg_today.shares = new_shares;
                                                              }
                                                          

                                                          if (cs_today == null)
                                                          {
                                    c_new = true;
                                    cs_today = new CashholdingModel { User = curr_user, datetme = dt,  freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate + free_up_margin), margin = Convert.ToInt32(margin - free_up_margin*3) };

                                                          }
                                                          else
                                                          {
                                                              cs_today.freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate + free_up_margin);
                                                              cs_today.margin = Convert.ToInt32(margin - free_up_margin*3);
                                                          }


                                                      }
                                                      else     // net short to net long
                                                      {

                                //  free_up_margin = Convert.ToInt32(stkhldg.shares * last_stk_price * exrate / 2);
                                free_up_margin = 0;
                                                          margin = margin - free_up_margin;
                                                          new_shares = stkhldg.shares + num_shares;
                                                            cash = Convert.ToInt32(cash  + free_up_margin);
                                                        if (cash >= new_shares * stockprice * exrate)
                                                          {
                                    if (stkhldg_today == null)
                                    {
                                        s_new = true;
                                        stkhldg_today = new StkHoldingModel() { User = curr_user, shares = new_shares, datetme = dt, Stklist = stklist };
                                    }
                                    else
                                    {
                                        stkhldg_today.shares = new_shares;
                                    }

                                    if (cs_today == null)
                                                              {
                                                                  cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash + free_up_margin - Convert.ToInt32(num_shares * stockprice * exrate)), margin = margin - free_up_margin*3 };
                                        c_new = true;
                                    }
                                                              else
                                                              {
                                                                  cs_today.freecash = Convert.ToInt32(cash + free_up_margin - Convert.ToInt32(num_shares * stockprice * exrate));
                                                                  cs_today.margin = Convert.ToInt32(margin - free_up_margin*3);
                                                              }
                                                          }
                                                          else
                                                          {

                                    m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark = "Insufficient Cash to buy stock", User = curr_user, Stklist = stklist };
                                    db.txModels.Add(m);

                                    db.SaveChanges();


                                    TempData["order_status"] =  "Insufficient Cash to buy stock";
                                                              return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                                          }
                                                      }

                                                  }

                      m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark="Complete Order",User = curr_user, Stklist = stklist };
                                                  db.txModels.Add(m);
                        if (s_new == true)
                        {
                            db.StkHoldingModels.Add(stkhldg_today);
                        }
                        if (c_new == true)
                        {
                            db.CashHoldingModels.Add(cs_today);
                        }
                                                  db.SaveChanges();


                        TempData["order_status"] =  "Complete Market Order to buy stock";
                                                  return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                
                //---------------------------------------------------------------------------
                            case "Sell":
                                 if (stkhldg == null)  // no stock

                                 {
                                     if (cash >= stockprice * num_shares * exrate / 2) //cash > margin
                                     {
                                if (stkhldg_today == null)
                                {
                                    s_new = true;
                                    stkhldg_today = new StkHoldingModel() { User = curr_user, shares = -num_shares , datetme = dt, Stklist = stklist };
                                }
                                else
                                {
                                    stkhldg_today.shares = -num_shares ;
                                }

                               
                                         if (cs_today == null)
                                         {
                                             cs_today = new CashholdingModel() { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate / 2), margin = Convert.ToInt32(margin + stockprice * num_shares * exrate * 3/ 2) };
                                    c_new = true;
                                }
                                         else
                                         {
                                             cs_today.freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate / 2);
                                             cs_today.margin = Convert.ToInt32(margin + stockprice * num_shares * exrate *3/ 2);
                                         }
                                     }
                                     else  //not enough cash 
                                     {

                                m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark = "Insufficient Cash to sell stock", User = curr_user, Stklist = stklist };
                                db.txModels.Add(m);

                                db.SaveChanges();
                                TempData["order_status"] =  "Insufficient Cash to sell stock";
                                         return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                     }

                                 }
                                 else if (stkhldg.shares <= 0)   // net short
                                 {
                                     if (cash >= stockprice * num_shares * exrate / 2)
                                     {

                                         if (stkhldg_today == null)
                                         {
                                    s_new = true;
                                    stkhldg_today = new StkHoldingModel() { User = curr_user, shares = -num_shares + stkhldg.shares, datetme = dt, Stklist = stklist };
                                         }
                                         else
                                         {
                                             stkhldg_today.shares = -num_shares + stkhldg.shares;
                                         }


                                         if (cs_today == null)
                                         {
                                    c_new = true;
                                    cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate / 2), margin = Convert.ToInt32(margin + stockprice * num_shares * exrate *3/ 2) };

                                         }
                                         else
                                         {
                                             cs_today.freecash = Convert.ToInt32(cash - stockprice * num_shares * exrate / 2);
                                             cs_today.margin = Convert.ToInt32(margin + stockprice * num_shares * exrate *3/ 2);
                                         }


                                     }
                                     else //not enough cash 
                                     {

                                m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark = "Insufficient Cash to sell stock", User = curr_user, Stklist = stklist };
                                db.txModels.Add(m);

                                db.SaveChanges();
                                TempData["order_status"] = "Insufficient Cash to sell stock";
                                         return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                     }

                                 }
                                 else
                                 {
                                     if (stkhldg.shares >= num_shares)  // net long
                                     {
                                         
                                         new_shares = stkhldg.shares - num_shares;
                                cash = cash + Convert.ToInt32(stockprice * num_shares * exrate);


                                             if (stkhldg_today == null)
                                             {
                                    s_new = true;
                                    stkhldg_today = new StkHoldingModel() { User = curr_user, shares = new_shares, datetme = dt, Stklist = stklist };
                                             }
                                             else
                                             {
                                                 stkhldg_today.shares = new_shares;
                                             }
                                        

                                         if (cs_today == null)
                                         {
                                    c_new = true;
                                    cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash ), margin = margin  };

                                         }
                                         else
                                         {
                                             cs_today.freecash = Convert.ToInt32(cash );
                                             cs_today.margin = Convert.ToInt32(margin );
                                         }


                                     }
                                     else //net long to net short
                                     {
                                new_shares = stkhldg.shares - num_shares;
                                free_up_margin = Convert.ToInt32(stkhldg.shares * stockprice * exrate );
                                         cash = cash + free_up_margin;
                            


                                         if (cash >= -new_shares * stockprice * exrate/2)
                                         {
                                    if (stkhldg_today == null)
                                    {
                                        s_new = true;
                                        stkhldg_today = new StkHoldingModel() { User = curr_user, shares = new_shares, datetme = dt, Stklist = stklist };
                                    }
                                    else
                                    {
                                        stkhldg_today.shares = new_shares;
                                    }




                                    

                                             if (cs_today == null)
                                             {
                                                 cs_today = new CashholdingModel { User = curr_user, datetme = dt, freecash = Convert.ToInt32(cash + new_shares * stockprice * exrate / 2), margin = margin - Convert.ToInt32( new_shares * stockprice * exrate*3 / 2) };
                                        c_new = true;
                                    }
                                             else
                                             {
                                                 cs_today.freecash = Convert.ToInt32(cash + new_shares * stockprice * exrate / 2);
                                                 cs_today.margin = Convert.ToInt32(margin - new_shares * stockprice * exrate *3/ 2);
                                             }
                                         }
                                         else
                                         {

                                    m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate,remark= "Insufficient Cash to sell stock", User = curr_user, Stklist = stklist };
                                    db.txModels.Add(m);
                                   
                                    db.SaveChanges();



                                    TempData["order_status"]  = "Insufficient Cash to sell stock";
                                             return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
                                         }
                                     }

                                 }
                                 m = new txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, xrate = exrate, remark = "Complete Order" ,User = curr_user, Stklist = stklist };
                                 db.txModels.Add(m);
                        if (s_new == true)
                        {
                            db.StkHoldingModels.Add(stkhldg_today);
                        }
                        if (c_new == true)
                        {
                            db.CashHoldingModels.Add(cs_today);
                        }

                        db.SaveChanges();



                        TempData["order_status"] = "Complete Market Order to sell stock";
                                 return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });


                         }

                TempData["order_status"]  = "Unexpected";
                return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });

            }

            else
            {
				n = new Pending_txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice,  User = curr_user, Stklist = stklist };
				db.Pending_txModels.Add(n);
				
				db.SaveChanges();

				if (bs == "Buy")
				{

                    TempData["order_status"] = "Complete Limit Order to buy stock";
				}
				else
				{
                    TempData["order_status"] = "Complete Limit Order to sell stock";
				}

				return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });


			}

		}


        [Authorize]
        [HttpPost]
        public ActionResult SQL_Order(string xrate, string curr, string xchange, string stk_code, string stk_name, string stk_price, string shares, string bs, string otype)
        {
            Pending_txModel n;
            string dt = DateTime.Now.ToString("yyyyMMdd");
            string hrmmtme = DateTime.Now.ToString("HHmmss");
            var id = User.Identity.GetUserId();
            string curr_user_id = db.Users.FirstOrDefault(y => y.Id == id).userid;
            var curr_user = db.Users.FirstOrDefault(y => y.Id == id);
            ViewModel_1 mymodel = vmodel;
            Decimal stockprice = Convert.ToDecimal(stk_price);
            Decimal exrate = Convert.ToDecimal(xrate);
            int num_shares = Convert.ToInt32(shares);
            StklistModel stklist;

            try
            {
                
                stklist = (StklistModel)(from g in db.StklistModels
                                         where g.stkcode == stk_code && g.market == xchange && g.currency == curr
                                         select g).FirstOrDefault();

            }
            catch (Exception e)
            {
                stklist = null;
            }

            if (stklist == null)

            {
                stklist = new StklistModel() { stkcode = stk_code, name = stk_name, market = xchange, currency = curr };



            }

            if (otype == "mkt")
            {
               int ret = db.Database.ExecuteSqlCommand(@"[dbo].[Mkt_Order] {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", id , stk_code , dt , hrmmtme, curr , xchange , xrate , stk_price , stk_name , shares , bs);
                if (ret==-1)
                    TempData["order_status"] = "Insufficient Cash to buy stock";
                else if (ret==-2)
                    TempData["order_status"] = "Insufficient Cash to sell stock";
                else
                    TempData["order_status"] = "Complete Market Order";

                return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });
            }
            else
            {
                n = new Pending_txModel { datetme = dt, hrmmtme = DateTime.Now.ToString("HHmmss"), buysell = bs, shares = num_shares, price = stockprice, User = curr_user, Stklist = stklist };
                db.Pending_txModels.Add(n);

                db.SaveChanges();

                if (bs == "Buy")
                {

                    TempData["order_status"] = "Complete Limit Order to buy stock";
                }
                else
                {
                    TempData["order_status"] = "Complete Limit Order to sell stock";
                }

                return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });


            }

        }

            public ActionResult Index()
        {
            if (Request.IsAuthenticated)
                return RedirectToAction("portfolio", "Home", new { datme = DateTime.Now.ToString("yyyyMMdd") });

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult stkana(string datme)
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
