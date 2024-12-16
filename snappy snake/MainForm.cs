using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;


namespace FundusScore
{
    public partial class MainForm : Form
    {
        //private bool newlight = false;
        public bool stopw = false;
        private string work_space = System.IO.Directory.GetCurrentDirectory()+"\\";
        private string csv_name = "score_list_new.csv";
        private string csv_name_saving = "score_list_new.csv";
        private DataTable indeks= new DataTable();
        private DataTable refers= new DataTable();
        private DataTable collec= new DataTable();
        private int ind = 0;
        private int max_ind = 1;
        private int max_ref_ind = 1;
        private int ref_ind = 50;

        public MainForm()
        {
            InitializeComponent();
            //initialize the datatable
            // 寻找所谓参考的数据
            if (File.Exists(work_space + "refs\\score_list_ref.csv")){
                try
                {
                    this.refers = CsvOperator.OpenCSV(work_space + "refs\\score_list_ref.csv");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("没有找到参照图片，将在没有参照的情况下继续. no referrence image found!");
                this.Width = 850;
                
            }
            if (File.Exists(work_space + "data\\collection.csv"))
            {
                try
                {
                    this.collec = CsvOperator.OpenCSV(work_space + "data\\collection.csv");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("没有找到参照分数，将在没有参照的情况下继续. no referrence image found!");
                this.Width = 850;

            }

            var last_row1 = refers.AsEnumerable().Last<DataRow>()[0]; // 读取最大行数
            try
            {
                int max_inde = int.Parse(last_row1.ToString());
                this.max_ref_ind = max_inde + 1;
                this.ref_ind = (int)(this.max_ref_ind / 2);
            }
            catch (Exception ex)
            {
                this.sshinfo.Text = (ex.Message);
            }

            // 寻找需要评分的数据
            FolderBrowserDialog find_path = new FolderBrowserDialog();
            find_path.SelectedPath = work_space;
             // 检查默认位置是否有数据文件，能找到数据文件则跳过手动寻找
            if (File.Exists(work_space + "data\\score_list_new.csv"))
            {
                work_space = work_space + "data\\";
                csv_name = "score_list_new.csv";
            }
            else if (File.Exists(work_space + "data\\score_list.csv"))
            {
                csv_name = "score_list.csv";
                work_space = work_space + "data\\";

            }
            else
            {
                find_path.Description = "默认位置没有文件，请手动选择文件所在位置";
                if (find_path.ShowDialog() == DialogResult.OK) // 没有找到默认文件就手动寻找文件夹位置
                {
                    work_space = find_path.SelectedPath + "\\";
                    if (File.Exists(work_space + "score_list_new.csv"))
                    {
                        csv_name = "score_list_new.csv";
                    }
                    else if (File.Exists(work_space + "score_list.csv"))
                    {
                        csv_name = "score_list.csv";
                    }
                    else
                    {
                        MessageBox.Show("Opinion Score list does not exsit! 请先生成空白分数表格");
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
            // 关闭寻找的窗口，开始导入csv，进行初始化
            find_path.Dispose();
            try
            {
                this.indeks = CsvOperator.OpenCSV(work_space + csv_name);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            var last_row = indeks.AsEnumerable().Last<DataRow>()[0]; // 读取最大行数
            try
            {
                int max_inde = int.Parse(last_row.ToString());
                max_ind = max_inde + 1;
            }
            catch (Exception ex)
            {
                this.sshinfo.Text = (ex.Message);
            }

            // initialize the img box
            this.ind = 0;
            this.ShowImage();
            this.ShowRefImage();
            // initialize the side boxes of ref img


            this.sshinfo.Text = ("initialized...");



        }

        private void ShowImage()
        {
            if (ind < max_ind)
            {
                string img_name = indeks.Rows[ind]["file_name"].ToString();
                Bitmap now_img = ReadImageFile(work_space + img_name);
                // update img
                if (now_img != null)
                {
                    now_img = resizeImage(now_img, 1);

                    this.pictureBox1.Image = now_img;
                    this.pictureBox1.Show();
                    this.pictureBox1.Refresh();
                }
                // update existing score
                this.update_score_text();
                this.update_level_box();

            }
            else
            {
                this.pictureBox1.Image = null;
                MessageBox.Show("Out of index!!!");
            }
            
        }

        private void ShowRefImage() // 因为区域大小变为了560，所有比率调整为了7/8
        {
            if (this.ref_ind < this.max_ref_ind)
            {
                string img_name = this.refers.Rows[ref_ind]["file_name"].ToString();
                Bitmap now_img = ReadImageFile(work_space + img_name);
                // update img
                if (now_img != null)
                {
                    now_img = resizeImage(now_img, (float)0.875);

                    this.pictureBox2.Image = now_img;
                    this.pictureBox2.Show();
                    this.pictureBox2.Refresh();
                }
                // update existing score
                string img_score = refers.Rows[ref_ind]["opinion_score"].ToString();
                this.score_label.Text = img_score;
                string img_com = refers.Rows[ref_ind]["comment"].ToString();
                this.comment_label.Text = img_com;
                // update side imgs
                this.ShowSideImage();

            }
            else
            {
                this.pictureBox1.Image = null;
                MessageBox.Show("Out of index!!!");
            }
        }


        private void ShowSideImage() //用来更新旁边五个小窗
        {
            string img_name = this.refers.Rows[this.ref_ind]["file_name"].ToString();
            string img_score = this.refers.Rows[this.ref_ind]["opinion_score"].ToString();
            Bitmap img3 = ReadImageFile(work_space + img_name);
            if(img3 != null)
            {
                img3 = resizeImage(img3, (float)0.2);
                this.sideBox3.Image = img3;
                this.sidelabel3.Text = img_score;
                this.sideBox3.Show();
                this.sideBox3.Refresh();
            }
            //更新上方两个（last ref）
            if (this.ref_ind > 0)
            {
                img_name = this.refers.Rows[this.ref_ind -1]["file_name"].ToString();
                img_score = this.refers.Rows[this.ref_ind -1]["opinion_score"].ToString();
                Bitmap img2 = ReadImageFile(work_space + img_name);
                if (img2 != null)
                {
                    img2 = resizeImage(img2, (float)0.2);
                    this.sideBox2.Image = img2;
                    this.sidelabel2.Text = img_score;
                    this.sideBox2.Show();
                    this.sideBox2.Refresh();
                }
                if (this.ref_ind > 1)
                {
                    img_name = this.refers.Rows[this.ref_ind - 2]["file_name"].ToString();
                    img_score = this.refers.Rows[this.ref_ind - 2]["opinion_score"].ToString();
                    Bitmap img1 = ReadImageFile(work_space + img_name);
                    if (img1 != null)
                    {
                        img1 = resizeImage(img1, (float)0.2);
                        this.sideBox1.Image = img1;
                        this.sidelabel1.Text = img_score;
                        this.sideBox1.Show();
                        this.sideBox1.Refresh();
                    }
                }
                else
                {
                    
                    this.sideBox1.Image = null;
                    this.sidelabel1.Text = null;
                    this.sideBox1.Show();
                    this.sideBox1.Refresh();
                }
            }
            else
            {
                this.sideBox1.Image = null;
                this.sidelabel1.Text = null;
                this.sideBox1.Show();
                this.sideBox1.Refresh();
                this.sideBox2.Image = null;
                this.sidelabel2.Text = null;
                this.sideBox2.Show();
                this.sideBox2.Refresh();
            }
            //更新下方两个（next ref）
            if (this.ref_ind <this.max_ref_ind-1)
            {
                img_name = this.refers.Rows[this.ref_ind + 1]["file_name"].ToString();
                img_score = this.refers.Rows[this.ref_ind + 1]["opinion_score"].ToString();
                Bitmap img4 = ReadImageFile(work_space + img_name);
                if (img4 != null)
                {
                    img4 = resizeImage(img4, (float)0.2);
                    this.sideBox4.Image = img4;
                    this.sidelabel4.Text = img_score;
                    this.sideBox4.Show();
                    this.sideBox4.Refresh();
                }
                if (this.ref_ind < this.max_ref_ind-2)
                {
                    img_name = this.refers.Rows[this.ref_ind + 2]["file_name"].ToString();
                    img_score = this.refers.Rows[this.ref_ind + 2]["opinion_score"].ToString();
                    Bitmap img5 = ReadImageFile(work_space + img_name);
                    if (img5 != null)
                    {
                        img5 = resizeImage(img5, (float)0.2);
                        this.sideBox5.Image = img5;
                        this.sidelabel5.Text = img_score;
                        this.sideBox5.Show();
                        this.sideBox5.Refresh();
                    }
                }
                else
                {

                    this.sideBox5.Image = null;
                    this.sidelabel5.Text = null;
                    this.sideBox5.Show();
                    this.sideBox5.Refresh();
                }
            }
            else
            {
                this.sideBox5.Image = null;
                this.sidelabel5.Text = null;
                this.sideBox5.Show();
                this.sideBox5.Refresh();
                this.sideBox4.Image = null;
                this.sidelabel4.Text = null;
                this.sideBox4.Show();
                this.sideBox4.Refresh();
            }


        }

        private void update_score_text()
        {
            string img_score = collec.Rows[ind]["mean"].ToString();
            this.read_score_label.Text=img_score;  // +1 for the column name row
            string img_score2 = indeks.Rows[ind]["opinion_score"].ToString();
            this.your_label.Text = img_score2;
            this.textBox2.Text = img_score;
        }

        private void update_level_box()
        {
            string img_level = indeks.Rows[ind]["level"].ToString();
            if (img_level == "0")
            {
                this.radioButton1.Checked = true;
            }
            else if (img_level == "1")
            {
                this.radioButton2.Checked=true;
            }
            else if (img_level == "2")
            {
                this.radioButton3.Checked = true;
            }
            else
            {
                this.radioButton3.Checked = false;
                this.radioButton2.Checked = false;
                this.radioButton1.Checked = false;
            }


        }// 更新已经平分的等级，没有则全部uncheck



        private void update_csv_file()
        {
            CsvOperator.SaveCSV(indeks, work_space + csv_name_saving);
        }

        private static Bitmap resizeImage(Bitmap imgToResize, float scale)
        {
            //获取图片size
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float ratio = 0;
            //计算宽度的缩放比例,按照picbox的宽度进行计算
            int target_width = (int)(640*scale);
            ratio = ((float)target_width / (float)sourceWidth);


            //期望的高度
            int desHeight = (int)(sourceHeight * ratio);

            Bitmap b = new Bitmap(imgToResize, target_width, desHeight);
            //Graphics g = Graphics.FromImage(b);
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            //g.DrawImage(imgToResize, 0, 0, target_width, desHeight);
            //g.Dispose();
            return b;
        }

        private int find_img_from(int begin_ind)
        {
            int target_ind = -1;
            for (int i = begin_ind; i < max_ind; i++)
            {
                string this_score = indeks.Rows[i]["opinion_score"].ToString();
                try
                {
                    int score_int = int.Parse(this_score);
                    if (score_int == -1)
                    {
                        target_ind = i;
                        break;
                    }
                }
                catch (Exception ex)
                {
                    this.sshinfo.Text = (ex.Message);
                    return target_ind;
                }
            }
            if (target_ind == -1)
            {
                MessageBox.Show("There is no more image without score.!!");
                return target_ind;
            }
            else
            {
                // 如果能找到就更新ind
                return target_ind;
            }
        }

        public static Bitmap ReadImageFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;//文件不存在
            }
            FileStream fs = File.OpenRead(path); //OpenRead
            int filelength = 0;
            filelength = (int)fs.Length; //获得文件长度 
            Byte[] image = new Byte[filelength]; //建立一个字节数组 
            fs.Read(image, 0, filelength); //按字节流读取 
            System.Drawing.Image result = System.Drawing.Image.FromStream(fs);
            fs.Close();
            Bitmap bit = new Bitmap(result);
            return bit;
        }

        private void button1_Click(object sender, EventArgs e) // find a un score img
        {
            int next_ind = find_img_from(0); // search form index 0
            if (next_ind == -1)
            {
                return;
            }
            else
            {
                this.ind = next_ind;
                this.ShowImage();
            }
        }

        private void button6_Click(object sender, EventArgs e) // 确认评分按钮 
        {
            if (this.textBox2.Text != null)
            {
                string new_score = this.textBox2.Text;
                string sub_level = "-1";
                try
                {
                    int score_int = int.Parse(new_score);
                    if (score_int < 0 || score_int > 100)
                    {
                        MessageBox.Show("请输入0-100之间任意整数");
                        return;
                    }
                }
                catch (Exception ex)
                {
                    this.sshinfo.Text = (ex.Message);
                    return;
                } // to int 

                if (this.radioButton1.Checked == true)
                {
                    sub_level = "0";
                }
                else if (this.radioButton2.Checked == true)
                {
                    sub_level = "1";
                }
                else if (this.radioButton3.Checked == true)
                {
                    sub_level = "2";
                }
                else
                {
                    MessageBox.Show("请对图片的等级进行评价");
                    return;
                }


                this.indeks.Rows[ind]["opinion_score"] = new_score;
                this.indeks.Rows[ind]["level"] = sub_level;
                this.update_score_text();
                this.update_level_box();
                this.sshinfo.Text = "New Score added.";
                // save the new csv file
                update_csv_file();

                // if checked box ,find another img
                if (this.checkBox1.Checked == true)
                {
                    int next_ind = find_img_from(ind); // search form index now
                    if (next_ind == -1)
                    {
                        return;
                    }
                    else
                    {
                        this.ind = next_ind;
                        this.ShowImage();
                    }
                }
  
                // if checked update rated number, do this
                if (this.checkBox2.Checked == true)
                {
                    update_num_rated();
                }
            }

            return;
        }

        private void button3_Click(object sender, EventArgs e) // next img
        {
            this.ind +=1;
            if (this.ind >= max_ind)
            {
                MessageBox.Show("Out of index. 已经没有下一张图片！！");
                this.ind-=1;
                return ;
            }
            else
            {
                this.ShowImage();
                this.sshinfo.Text = "this is the next image you want.";
            }
        }

        private void button4_Click(object sender, EventArgs e) // last img
        {
            this.ind -= 1;
            if (this.ind <0)
            {
                MessageBox.Show("Out of index. 已经没有上一张图片！！");
                this.ind += 1;
                return;
            }
            else
            {
                this.ShowImage();
                this.sshinfo.Text = "this is the last image you want.";
            }
        }

        private void button2_Click(object sender, EventArgs e) //找到下一张没有的照片，这个方法不是很行，但是效率比第一种强一些，尤其是面对数千张照片已经进行到末尾时
        {
            int next_ind = find_img_from(ind); // search form index now
            if (next_ind == -1)
            {
                return;
            }
            else
            {
                this.ind = next_ind;
                this.ShowImage();
            }
        }

        private void button5_Click(object sender, EventArgs e) // search by name 
        {
            DataRow[] res = indeks.Select("file_name like '" + this.textBox3.Text + ".%'");
            if (res.Length > 0)
            {
                Console.WriteLine("flag!");
                string find_ind =res[0]["column1"].ToString();
                try
                {
                    this.ind=int.Parse(find_ind);
                    this.ShowImage();
                    this.sshinfo.Text = "A image has been found:    " + res[0]["file_name"].ToString();
                }
                catch (Exception ex)
                {
                    this.sshinfo.Text=ex.Message;
                }
            }
            Console.WriteLine("flag!");
        }

        private void button7_Click(object sender, EventArgs e)//更新已评分图片数量
        {
            update_num_rated();
        }

        private void update_num_rated()
        {
            int count=0;
            for (int i = 0; i < this.max_ind; i++)
            {
                string this_score = indeks.Rows[i]["opinion_score"].ToString();
                try
                {
                    int score_int = int.Parse(this_score);
                    if (score_int == -1)
                    {
                        count+=1;
                    }
                }
                catch (Exception ex)
                {
                    this.sshinfo.Text = ("error in update_num_rated()"+ex.Message);
                    return ;
                }
            }
            this.label5.Text = "labelled: "+(this.max_ind+1-count).ToString()+"/"+(this.max_ind+1).ToString();

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void nextref_Click(object sender, EventArgs e)
        {
            this.ref_ind += 1;
            if (this.ref_ind >= max_ref_ind)
            {
                MessageBox.Show("Out of index.(ref img) 已经没有下一张参考图片！！");
                this.ref_ind -= 1;
                this.ShowRefImage();
                return;
            }
            else
            {
                this.ShowRefImage();
                this.sshinfo.Text = "this is the higher reference image you want.";
            }
        }

        private void lastref_Click(object sender, EventArgs e)
        {
            this.ref_ind -= 1;
            if (this.ref_ind < 0)
            {
                MessageBox.Show("Out of index.(ref img) 已经没有上一张参考图片！！");
                this.ref_ind = 0;
                this.ShowRefImage();
                return;
            }
            else
            {
                this.ShowRefImage();
                this.sshinfo.Text = "this is the lower reference image you want.";
            }
        }

        private void next10ref_Click(object sender, EventArgs e)
        {
            this.ref_ind += 10;
            if (this.ref_ind >= max_ref_ind)
            {
                this.sshinfo.Text = "不足10张图片，将显示最高得分图片";
                this.ref_ind =max_ref_ind-1;
                this.ShowRefImage();

                return;
            }
            else
            {
                this.ShowRefImage();
                this.sshinfo.Text = "this is the higher reference (10) image you want.";
            }
        }

        private void last10ref_Click(object sender, EventArgs e)
        {
            this.ref_ind -= 10;
            if (this.ref_ind <0)
            {
                this.sshinfo.Text = "不足10张图片，将显示最低得分图片";
                this.ref_ind = 0;
                this.ShowRefImage();

                return;
            }
            else
            {
                this.ShowRefImage();
                this.sshinfo.Text = "this is the lowerr reference (10) image you want.";
            }
        }


        private void help_bt_Click(object sender, EventArgs e)
        {
            MessageBox.Show("该程序用于辅助眼底图片评分\n\n" +
                "1.中央4个按钮用于切换用于评分的图片，寻找未评分图片时会计入当前图片\n" +
                "2.使用名称索引功能请输入完整名称（含数字前0，无后缀），可得到唯一的图片不产生歧义\n" +
                "3.按下确认评分按钮之后分数会实时保存，并自动寻找下一张未评分的图片\n" +
                "4.右下角的按钮用于切换用作参考的图片，使用向前向后10张可以快速翻阅\n" +
                "5.图片分数仅供参考，某些图片可能会存在歧义，如有多张相似图片供参考但分数不唯一，请自行判断\n\n" +
                "Tips：如果觉得参考图片风格相似但是有分数差异，可以参考如下重要的特征：视杯视盘区，黄斑区，是否过曝或过暗，异常的病理特征");
        }

        private void button8_Click(object sender, EventArgs e)//导出csv
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "csv file(*.csv)|*.csv";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                string filename = saveFile.FileName;
                CsvOperator.SaveCSV(indeks, filename);
            }
        }

    }
}
