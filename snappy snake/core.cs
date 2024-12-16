using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;


namespace snappy_snake
{
    class core
    {
        private int blockwidth = 40;
        private int blockheight = 30;
        private Color backgound;
        private Graphics paintgraph;
        private List<Snake> snakeblock;
        private Direction snakedirection=Direction.Up;
        private System.Timers.Timer refresh;
        private Snake food;
        private int nsize = 20;
        private int gamelevel = 1;
        private bool gameover = false;
        private int[] gamespeed = new int[] { 500, 400, 300, 200, 100 ,50};

        public core(int width,int height,int size, Color backgroundcolor,Graphics paintg,int lev)
        {
            this.blockheight = height;
            this.blockwidth = width;
            this.backgound = backgroundcolor;
            this.nsize = size;
            this.paintgraph = paintg;
            this.gamelevel = lev;
            this.snakeblock = new List<Snake>();
            this.snakeblock.Insert(0, new Snake(Color.AliceBlue, this.nsize, new Point(this.blockwidth / 2, this.blockheight / 2)));//在中心生成一个蛇
            this.snakeblock.Insert(0, new Snake(Color.AliceBlue, this.nsize, new Point(this.blockwidth / 2 - 1, this.blockheight / 2)));
            this.snakedirection = Direction.Right;
        }//初始化整个画面

        public void gamestart()
        {
            this.food = Newfood();
            refresh = new System.Timers.Timer(gamespeed[this.gamelevel]-1);
            refresh.Elapsed += new System.Timers.ElapsedEventHandler(OnBlockTimedEvent);
            refresh.AutoReset = true;
            refresh.Start();

        }//计时器和初代食物

        private void OnBlockTimedEvent(object source,ElapsedEventArgs e)
        {
            this.move();

            if (this.checkgameover())//时间开始时检查游戏结束
            {
                this.refresh.Stop();
                this.refresh.Dispose();
                MessageBox.Show(
                    "You Scored: " + (this.snakeblock.Count()-2),
                    "GameOver",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

            }
        }

        private bool checkgameover()
        {
            Snake head = (Snake)(this.snakeblock[0]);
            if(head.GetPoint.X<0||head.GetPoint.Y<0|| head.GetPoint.X >this.blockwidth || head.GetPoint.Y > this.blockheight)
            {
                this.gameover = true;
                return true;

            }//检查撞墙

            for(int i = 2; i < this.snakeblock.Count(); i++)
            {
                Snake body = (Snake)(this.snakeblock[i]);
                if(head.GetPoint.X==body.GetPoint.X&& head.GetPoint.Y == body.GetPoint.Y)
                {
                    this.gameover = true;
                    return true;
                }
            }//检查装自己

            this.gameover = false;
            return false;

        }

        private void move()
        {
            Snake head = this.snakeblock[0];
            Point newpoint = new Point(head.GetPoint.X, head.GetPoint.Y + 1);
            
            switch (this.snakedirection)
            {
                case Direction.Down:
                    newpoint = new Point(head.GetPoint.X, head.GetPoint.Y + 1);
                    break;
                case Direction.Left:
                    newpoint = new Point(head.GetPoint.X - 1, head.GetPoint.Y);
                    break;
                case Direction.Up:
                    newpoint = new Point(head.GetPoint.X, head.GetPoint.Y - 1);
                    break;
                case Direction.Right:
                    newpoint = new Point(head.GetPoint.X + 1, head.GetPoint.Y);
                    break;
            }
            Snake newhead = new Snake(Color.AliceBlue, this.nsize, newpoint);//找准方向生成一个头部

            if (newpoint != food.GetPoint)
            {
                this.snakeblock.RemoveAt(this.snakeblock.Count() - 1);
                //不是食物就删去尾巴，是食物就新建一个食物
            }
            else
            {
                this.food=Newfood();
            }

            this.snakeblock.Insert(0, newhead);
            this.UpdateGraph();

        }

        private Snake Newfood()
        {
            Snake foodblock = null;
            Random randpoint = new Random();
            bool redo = false;
            while (true)
            {
                redo = false;
                int x = randpoint.Next(this.blockwidth);
                int y = randpoint.Next(this.blockheight);
                for (int i = 0; i < this.snakeblock.Count(); i++)
                {
                    if (x == this.snakeblock[i].GetPoint.X && y == this.snakeblock[i].GetPoint.Y)
                    {
                        redo = true;
                    }
                    //检查冲突
                }
                if (redo == false)
                {
                    foodblock = new Snake(Color.OrangeRed, this.nsize, new Point(x, y));
                    return foodblock;
                }
            }
        }//创建了新的食物

        public void UpdateGraph()
        {
            this.paintgraph.Clear(this.backgound);
            this.food.PaintBlock(this.paintgraph);
            foreach(Snake snakeelement in this.snakeblock)
            {
                snakeelement.PaintBlock(this.paintgraph);
            }
        }

        public Direction GetDirection()
        {
            Direction direc = new Direction();
            direc = this.snakedirection;
            return direc;
        }

        public void UpdateDirection(Direction newdirec)
        {
            this.snakedirection = newdirec;
        }











        public enum Direction //这是一个枚举
        {
            Left,
            Right,
            Up,
            Down
        }
        
    }
}
