using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SD = System.Drawing;

namespace NDO_Crop_SaveImageIn_MVC.Controllers
{
    public class JcropController : Controller
    {
        string path = ConfigurationManager.AppSettings["path"] + "\\images";

        // GET: Jcrop
        public ActionResult Index()
        {
            return PartialView();
        }
        public ActionResult tutorial1()
        {
            return PartialView();
        }
        public ActionResult tutorial2()
        {
            return PartialView();
        }
        public ActionResult tutorial3()
        {
            return PartialView();
        }
        public ActionResult tutorial4()
        {
            return PartialView();
        }
        public ActionResult tutorial5()
        {
            return PartialView();
        }

        public ActionResult ImgFileUpload(HttpPostedFileBase ImageFile)
        {
            string img_upload = "";
            if (ImageFile != null)
            {
                string Path = Server.MapPath("~/images");
                DateTime myDate = DateTime.Now;
                string fName = myDate.ToString("yyyyMMddHHmmss") + "-p";
                img_upload = upFile(ImageFile, Path, fName);
            }
            img imgs = new img();
            imgs.ImageFile = img_upload;
            return Json(imgs);
        }
        #region 檔案上傳
        private string upFile(HttpPostedFileBase fileUp, string Path, string fileName)
        {
            string upFilePath = Path;
            if (fileUp != null)
            {
                //重新命名，存入檔案
                string fileSubName = fileUp.FileName.Substring(fileUp.FileName.IndexOf("."), fileUp.FileName.Length - fileUp.FileName.IndexOf("."));
                int cupfilelength = fileUp.ContentLength;//檔案大小
                upFilePath += @"\" + fileName + fileSubName;
                fileUp.SaveAs(upFilePath);
                upFilePath = fileName + fileSubName;
            }
            else
            {
                upFilePath = "";
            }
            return upFilePath;
        }
        #endregion

        public ActionResult ImgCrop(int w, int h, int x, int y, string ImageName = "pool.jpg")
        {
            
            #region 切圖

            byte[] CropImage = Crop(path + "\\" + ImageName, w, h, x, y);// 切圖
            using (MemoryStream ms = new MemoryStream(CropImage, 0, CropImage.Length))
            {
                ms.Write(CropImage, 0, CropImage.Length);
                using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
                {
                    string SaveTo = path + "\\crop\\" + ImageName;
                    CroppedImage.Save(SaveTo, CroppedImage.RawFormat);


                    ViewBag.imgCropped = "images/crop/" + ImageName;
                }
            }

            #endregion

            return PartialView();
        }



        static byte[] Crop(string Img, int Width, int Height, int X, int Y)
        {
            try
            {
                using (SD.Image OriginalImage = SD.Image.FromFile(Img))
                {
                    using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                    {
                        bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                        using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                        {
                            Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                            Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                            MemoryStream ms = new MemoryStream();
                            bmp.Save(ms, OriginalImage.RawFormat);
                            return ms.GetBuffer();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }
    }

    internal class img
    {
        public string ImageFile = "";
    }
}