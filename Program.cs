using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PixelPainter
{
    internal static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Form1을 먼저 실행
            Form1 mainForm = new Form1();

            // Form1이 실행된 후, editIMG를 모달로 띄운다.
            editIMG editImageForm = new editIMG();
            editImageForm.ShowDialog();  // 모달 형식으로 띄우기

            Application.Run(new Form1());
        }
    }
}
