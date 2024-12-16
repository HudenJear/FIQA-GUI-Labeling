using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using Renci.SshNet;

namespace snappy_snake
{
    //食物类
    public class Bean
    {
        //用于画食物的顶端坐标
        private Point _origin;

        public Point Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }

        //显示食物
        public void ShowBean(Graphics g)
        {
            //定义红色的画笔
            SolidBrush brush = new SolidBrush(Color.Red);
            //画实心矩形表示食物
            g.FillRectangle(brush, Origin.X, Origin.Y, 15, 15);
        }

        public void UnShowBean(Graphics g)
        {
            //定义系统背景颜色的画笔
            SolidBrush brush = new SolidBrush(Color.Silver);
            //画实心矩形颜色为系统背景颜色，相当于食物被吃掉了
            g.FillRectangle(brush, Origin.X, Origin.Y, 15, 15);
        }
    }
}
