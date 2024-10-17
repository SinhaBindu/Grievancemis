using IronBarCode;
using NLog;
using QRCoder;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace Grievancemis.Manager
{
    public class GlobalUtilityManager
    {
        public static Logger logger = LogManager.GetCurrentClassLogger();
        public static void MessageToaster(Controller controller, string MessageTitle, string MessageBody, string MessageType = "success", string OptionalUrl = "")
        {
            ToastMsg toastMsg = new ToastMsg()
            {
                MsgType = MessageType,
                MsgTitle = MessageTitle,
                MsgBody = MessageBody
            };
            controller.TempData["ToastMsg"] = toastMsg.ToJSON();
        }
        public static bool GenerateBarCode(string codeString, string fileName, bool isLogoRequired = false)
        {
            bool isBarCodeGenerated = false;
            try
            {
                //codeString = "RegNo: CR2021080000001, Name: SAnjay Kumar, DOB: 2021-20-12, Add-Motilal Nehru Park, Patliputra-842002, Patna, Bihar";
                string fileTargetPath = HttpContext.Current.Server.MapPath(AppConstants.BarCodeFilePath);
                if (!string.IsNullOrEmpty(fileTargetPath))
                {
                    if (!Directory.Exists(fileTargetPath))
                    {
                        Directory.CreateDirectory(fileTargetPath);
                    }
                }
                //if (!File.Exists(fileTargetPath + fileName + ".png"))
                //{
                //    File.Delete(fileTargetPath + fileName + ".png");
                //}
                if (!isLogoRequired)
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeString, QRCodeGenerator.ECCLevel.Q);
                    BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
                    byte[] qrCodeAsBitmapByteArr = qrCode.GetGraphic(20);

                    //QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    //QRCodeData qrCodeData = qrGenerator.CreateQrCode(codeString, QRCodeGenerator.ECCLevel.Q);
                    //QRCode qrCode = new QRCode(qrCodeData);
                    //Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    //isBarCodeGenerated = true;


                    //byte[] bitmap = GetYourImage();

                    using (Image image = Image.FromStream(new MemoryStream(qrCodeAsBitmapByteArr)))
                    {
                        image.Save(fileTargetPath + "/" + fileName + ".png", ImageFormat.Png);  // Or Png
                    }
                    isBarCodeGenerated = true;
                }
                //else
                //{
                //    QRCodeWriter.CreateQrCode(codeString, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng(fileTargetPath + fileName + ".png");
                //    isBarCodeGenerated = true;
                //}
                return isBarCodeGenerated;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message + ", " + ex.StackTrace);
                return false;
            }
        }
        public static bool CreateBarCode(string codeString, string fileName, bool isLogoRequired = false)
        {
            bool isBarCodeGenerated = false;
            try
            {
                //codeString = "RegNo: CR2021080000001, Name: SAnjay Kumar, DOB: 2021-20-12, Add-Motilal Nehru Park, Patliputra-842002, Patna, Bihar";
                string fileTargetPath = HttpContext.Current.Server.MapPath(AppConstants.BarCodeFilePath);
                if (!string.IsNullOrEmpty(fileTargetPath))
                {
                    if (!Directory.Exists(fileTargetPath))
                    {
                        Directory.CreateDirectory(fileTargetPath);
                    }
                }
                //if (!File.Exists(fileTargetPath + fileName + ".png"))
                //{
                //    File.Delete(fileTargetPath + fileName + ".png");
                //}
                if (isLogoRequired)
                {
                    // Adding a Logo
                    QRCodeWriter.CreateQrCodeWithLogo(codeString, AppConstants.LogoPath, 500).SaveAsPng(fileTargetPath + fileName + ".png");
                    //MyQRWithLogo.ChangeBarCodeColor(System.Drawing.Color.DarkGreen);
                    isBarCodeGenerated = true;
                }
                else
                {
                    QRCodeWriter.CreateQrCode(codeString, 500, QRCodeWriter.QrErrorCorrectionLevel.Medium).SaveAsPng(fileTargetPath + fileName + ".png");
                    isBarCodeGenerated = true;
                }
                return isBarCodeGenerated;
            }
            catch (Exception ex)
            {
                logger.Info(ex.Message + ", " + ex.StackTrace);
                return false;
            }
        }

        #region Email Service
        public static bool SendEmail_LeadCompiledData()
        {
            //ToDo: Write code snippet to send email of lead compiled data
            return false;
        }

        public static bool SendEmail_CPMULeadCompiledData()
        {
            //ToDo: Write code snippet to send email of lead compiled data
            return false;
        }
        #endregion
    }
    public class ToastMsg
    {
        public string MsgType { get; set; }
        public string MsgTitle { get; set; }
        public string MsgBody { get; set; }
    }

}
