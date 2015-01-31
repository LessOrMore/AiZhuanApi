using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAPI.Controllers
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/
        public void VertifyCode()
        {
            int i;
            Color clr;
            int codeW = 80;
            int codeH = 0x16;
            int fontSize = 0x10;
            string chkCode = string.Empty;
            Color[] color = new Color[] { Color.Black, Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Brown, Color.Brown, Color.DarkBlue };
            string[] font = new string[] { "Times New Roman", "Verdana", "Arial", "Gungsuh", "Impact" };
            char[] character = new char[] { 
                                                '2', '3', '4', '5', '6', '8', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 
                                                'r', 'x', 'y', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 
                                                'P', 'R', 'S', 'T', 'W', 'X', 'Y'
                                             };
            Random rnd = new Random();
            for (i = 0; i < 4; i++)
            {
                chkCode = chkCode + character[rnd.Next(character.Length)];
            }
            //验证码存储session 
            Session["SessionVerifyCode"] = chkCode;
            Bitmap bmp = new Bitmap(codeW, codeH);
            Graphics g = Graphics.FromImage(bmp);
            g.Clear(Color.White);
            for (i = 0; i < 1; i++)
            {
                int x1 = rnd.Next(codeW);
                int y1 = rnd.Next(codeH);
                int x2 = rnd.Next(codeW);
                int y2 = rnd.Next(codeH);
                clr = color[rnd.Next(color.Length)];
                g.DrawLine(new Pen(clr), x1, y1, x2, y2);
            }
            for (i = 0; i < chkCode.Length; i++)
            {
                string fnt = font[rnd.Next(font.Length)];
                Font ft = new Font(fnt, (float)fontSize);
                clr = color[rnd.Next(color.Length)];
                g.DrawString(chkCode[i].ToString(), ft, new SolidBrush(clr), (float)((i * 18f) + 2f), (float)0f);
            }
            Response.Buffer = true;
            Response.ExpiresAbsolute = DateTime.Now.AddMilliseconds(0.0);
            Response.Expires = 0;
            Response.CacheControl = "no-cache";
            Response.AppendHeader("Pragma", "No-Cache");

            MemoryStream ms = new MemoryStream();
            try
            {
                bmp.Save(ms, ImageFormat.Png);
                Response.ClearContent();
                Response.ContentType = "image/Gif";
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception)
            {

            }
            finally
            {
                g.Dispose();
                bmp.Dispose();
            }
        }

    }
}
