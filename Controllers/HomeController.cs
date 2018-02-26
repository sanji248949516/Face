using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Face.Models;

namespace Face.Controllers
{
    public class HomeController : Controller
    {

        public Entity entity = new Entity();
        public CtrixDBEntities db = new CtrixDBEntities();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Contact(string id)
        {
            Face en = db.Face.Find(id);
            string URL = "http://*.*.*.*:8080/score/" + en.entityid;
            string Result = "";
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(URL);
            myRequest.Method = "Get";
            try
            {
                HttpWebResponse HttpWResp = (HttpWebResponse)myRequest.GetResponse();

                Stream myStream = HttpWResp.GetResponseStream();
                StreamReader sr = new StreamReader(myStream, Encoding.UTF8);
                StringBuilder strBuilder = new StringBuilder();
                while (-1 != sr.Peek())
                {
                    strBuilder.Append(sr.ReadLine());
                }
                strBuilder = strBuilder.Replace("\\", "");
                Result = strBuilder.ToString();
                Result = Result.Substring(1, Result.Length - 2).Replace("class", "classs");
            }
            catch (Exception exp)
            {
                exp.ToString();
            }
            List<scores> scolist = JsonHelper.ParseFromJson<List<scores>>(Result);
            List<scores> result = new List<scores>();
            if (scolist.Count>2)
            {
                result.Add(scolist.OrderByDescending(s=>s.score).First());
                result.Add(scolist.OrderBy(s => s.score).First());
            }
            else
            {
				result.AddRange(scolist);
            }
            ViewBag.entity = en;
            ViewBag.scolist = result;
            return View();
        }


        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UploadImages(string imgbase)
        {
            if (imgbase != null && imgbase != "")
            {
                try
                {
                    var gid = Guid.NewGuid().ToString();
                    Entity temp = this.FileUpload(imgbase);
                    Face bf = new Face()
                    {
                        entityid = temp.id,
                        file = temp.file,
                        imgsrc = temp.imgsrc,
                        success = temp.success,
                        id = gid
                    };
                    db.Face.Add(bf);
                    db.SaveChanges();

                    if (!bf.success)
                    {
                        return Json(new { Success = true, BFID = gid }, JsonRequestBehavior.AllowGet);
                        //return RedirectToAction("Contact", temp);
                    }
                    else
                    {
                        return Json(new { Success = false, Message = "上传了非法文件:false" }, JsonRequestBehavior.AllowGet);
                    }
                }
                catch (Exception ex)
                {
                    WriteTxt(ex.ToString(), "UploadImages");
                    return Json(new { Success = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {

                return Json(new { Success = false, Message = "请选择要上传的文件！" }, JsonRequestBehavior.AllowGet);
            }
        }

        public Entity FileUpload(string imgbase)
        {
            try
            {
                if (!string.IsNullOrEmpty(imgbase) && imgbase.IndexOf("data:image/") > -1)
                {
                    imgbase = Base64StringToImage(Server.UrlDecode(imgbase).Replace("data:image/png;base64,", "").Replace("data:image/jpeg;base64,", ""));

                }
                string url = "http://*.*.*.*:8080/upload";
                string filePath = Server.MapPath(imgbase);
                //string filePath = Server.MapPath("/img/1.jpg");
                string responseContent;
                var memStream = new MemoryStream();
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                // 边界符  
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                // 边界符  
                var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                //var fileStream = fileData;
                //var 1 = fileData.InputStream.
                // 最后的结束符  
                var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                // 设置属性  
                webRequest.Method = "POST";
                webRequest.Timeout = 5000;
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                // 写入文件  
                const string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type: application/octet-stream\r\n\r\n";
                var header = string.Format(filePartHeader, "file", filePath);
                var headerbytes = Encoding.UTF8.GetBytes(header);
                memStream.Write(beginBoundary, 0, beginBoundary.Length);
                memStream.Write(headerbytes, 0, headerbytes.Length);
                var buffer = new byte[1024];
                int bytesRead; // =0  
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                // 写入字符串的Key  
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}\r\n";
                // 写入最后的结束边界符  
                memStream.Write(endBoundary, 0, endBoundary.Length);
                webRequest.ContentLength = memStream.Length;
                var requestStream = webRequest.GetRequestStream();
                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();

                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    responseContent = httpStreamReader.ReadToEnd();
                }
                Entity entemp = JsonHelper.ParseFromJson<Entity>(responseContent);
                entemp.imgsrc = imgbase;
                fileStream.Close();
                httpWebResponse.Close();
                webRequest.Abort();
                return entemp;
            }
            catch (Exception ex)
            {
                WriteTxt(ex.ToString(), "FileUpload");
                return null;
            }
        }

        private string Base64StringToImage(string base64String)
        {
            string filename = System.DateTime.Now.ToString("MMddHHmmssfff");
            byte[] arr = Convert.FromBase64String(base64String.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream(arr);
            //空白画图
            Bitmap MyImage = new Bitmap(ms);
            MyImage.Save(Server.MapPath("~/uploads/" + filename + ".jpg"), ImageFormat.Png);
            ms.Close();
            return "/uploads/" + filename + ".jpg";
        }

        public void WriteTxt(string text, string textname)
        {
            Guid g = Guid.NewGuid();
            FileStream fs = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + "\\uploads\\" + textname + "-" + g + ".txt", FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs);
            //开始写入
            sw.WriteLine(text);
            //清空缓冲区

            sw.Flush();
            //关闭流
            sw.Close();
            fs.Close();
        }
    }
}