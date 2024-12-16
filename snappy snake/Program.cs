using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

/// <summary>
/// 该程序由HudenJear创建，并用于无偿的眼底图像评分工作，如有疑问请联系hudenjear@foxmail.com
/// 本程序可用于任何科研目的图像标注
/// 因本程序滥用或操作不当造成的数据财产损失作者概不负责
/// 
/// </summary>

namespace FundusScore
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
