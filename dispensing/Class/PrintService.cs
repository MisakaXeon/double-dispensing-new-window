using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Drawing;

namespace dispensing
{
    class PrintService
    {
        public PrintService()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
            this.docToPrint.PrintPage += new PrintPageEventHandler(docToPrint_PrintPage);
        }//将事件处理函数添加到PrintDocument的PrintPage中

        // Declare the PrintDocument object.
        private System.Drawing.Printing.PrintDocument docToPrint = new System.Drawing.Printing.PrintDocument();//创建一个PrintDocument的实例

        private string streamType;
        private string streamtxt;
        private Image streamima;

        // This method will set properties on the PrintDialog object and
        // then display the dialog.
        public void StartPrint(string txt, string streamType)
        {
            this.streamType = streamType;
            this.streamtxt = txt;
            // Allow the user to choose the page range he or she would
            // like to print.
            //System.Windows.Forms.PrintDialog PrintDialog1 = new PrintDialog();//创建一个PrintDialog的实例。
            //PrintDialog1.AllowSomePages = true;

            // Show the help button.
            //PrintDialog1.ShowHelp = true;

            docToPrint.DefaultPageSettings.Margins.Top = 10;
            docToPrint.DefaultPageSettings.Margins.Left = 0;

            // Set the Document property to the PrintDocument for 
            // which the PrintPage Event has been handled. To display the
            // dialog, either this property or the PrinterSettings property 
            // must be set 
            //PrintDialog1.Document = docToPrint;//把PrintDialog的Document属性设为上面配置好的PrintDocument的实例

            bool show = false;
            if (show == true)
            {
                //DialogResult result = PrintDialog1.ShowDialog();//调用PrintDialog的ShowDialog函数显示打印对话框,如果不要注释即可，直接调用docToPrint.Print()
                                                                // If the result is OK then print the document.
                //if (result == DialogResult.OK)
                {
                    docToPrint.Print();//开始打印
                }
            }
            else
            {
                docToPrint.Print();//开始打印
            }

        }
        public void StartPrint(Image ima, string streamType)
        {
            this.streamType = streamType;
            this.streamima = ima;
            // Allow the user to choose the page range he or she would
            // like to print.
            //System.Windows.Forms.PrintDialog PrintDialog1 = new PrintDialog();//创建一个PrintDialog的实例。
            //PrintDialog1.AllowSomePages = true;

            // Show the help button.
            //PrintDialog1.ShowHelp = true;

            // Set the Document property to the PrintDocument for 
            // which the PrintPage Event has been handled. To display the
            // dialog, either this property or the PrinterSettings property 
            // must be set 
            //PrintDialog1.Document = docToPrint;//把PrintDialog的Document属性设为上面配置好的PrintDocument的实例

            //DialogResult result = PrintDialog1.ShowDialog();//调用PrintDialog的ShowDialog函数显示打印对话框,如果不要注释即可，直接调用docToPrint.Print()
                                                            // If the result is OK then print the document.
            //if (result == DialogResult.OK)
            {
                docToPrint.Print();//开始打印
            }
        }
        // The PrintDialog will print the document
        // by handling the document's PrintPage event.
        private void docToPrint_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)//设置打印机开始打印的事件处理函数
        {

            // Insert code to render the page here.
            // This code will be called when the control is drawn.

            // The following code will render a simple
            // message on the printed document
            switch (this.streamType)
            {
                case "txt":
                    string text = null;
                    System.Drawing.Font printFont = new System.Drawing.Font
                     ("Arial", 10, System.Drawing.FontStyle.Regular);

                    // Draw the content.

                    text = streamtxt;
                    e.Graphics.DrawString(text, printFont, System.Drawing.Brushes.Black, e.MarginBounds.X, e.MarginBounds.Y);
                    break;
                case "image":
                    System.Drawing.Image image = streamima;
                    int x = e.MarginBounds.X;
                    int y = e.MarginBounds.Y;
                    int width = image.Width;
                    int height = image.Height;
                    if ((width / e.MarginBounds.Width) > (height / e.MarginBounds.Height))
                    {
                        width = e.MarginBounds.Width;
                        height = image.Height * e.MarginBounds.Width / image.Width;
                    }
                    else
                    {
                        height = e.MarginBounds.Height;
                        width = image.Width * e.MarginBounds.Height / image.Height;
                    }
                    System.Drawing.Rectangle destRect = new System.Drawing.Rectangle(x, y, width, height);
                    e.Graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, System.Drawing.GraphicsUnit.Pixel);
                    break;
                default:
                    break;
            }
        }
    }
}
