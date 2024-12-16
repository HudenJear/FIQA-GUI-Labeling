using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace snappy_snake
{
    class Snake
    {
        private Color snakecolor;
        private int snakesize;
        private Point snakepoint;

        public Snake(Color color, int size ,Point point)
        {
            this.snakecolor = color;
            this.snakepoint = point;
            this.snakesize = size;
        }

        public Point GetPoint
        {
            get { return this.snakepoint; }
        }

        public virtual void PaintBlock(Graphics snakegraph)
        {
            SolidBrush snakebrush = new SolidBrush(snakecolor);
            lock (snakegraph)
            {
                try
                {
                    snakegraph.FillRectangle(snakebrush,
                        this.GetPoint.X * this.snakesize,
                        this.GetPoint.Y * this.snakesize,
                        this.snakesize - 1,
                        this.snakesize - 1);

                }
                catch
                {
                    MessageBox.Show("出错！", "Snake Tips", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
        }
    }
}
