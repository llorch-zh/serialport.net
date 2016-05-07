using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;        //需要新加的命名空间（头文件）
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace mywork_chuan
{
    /*
    本程序的设计目的在于接收蓝牙通过串口发来的数据不进行转码，计算后在画图区进行画图
    本人非软件专业，很多东西也是一知半解，只根据自己需求写一个串口程序，索性并不复杂
    串口为USART串口，8数据位，1停止位，无校验位，这是一般蓝牙默认配置
    串口号和波特率提供修改
     */
    public partial class Form1 : Form
    {
        SerialPort sp = null;//声明一个串口类
        bool isOpen = false;//打开串口标志位
        bool isSetProperty = false;//属性设置标志位
        // bool isHEX = false;//16进制显示标志位
        Graphics _graphics = null;
        StreamWriter s = null;
        float last = 290;//数据显示时使用，用于记录上一个数据，初始值为基线
        float last_h = 490;//数据显示时使用，用于记录上一个数据，初始值为基线，心电
        float j = 30;//数据显示时使用，用于标记X轴，心音
        float j_h = 30;//数据显示时使用，用于标记X轴，心电
        int num = 0;//计数
        int count = 1;//计数，每第10个数为心电
        int count_txt = 1;//计第i个文件
        Bitmap cache = null;



        public Form1()
        {
            InitializeComponent();
        }
        /*
         在窗体加载时对两个combobox内容进行配置，可供选择
         */

        private void Form1_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            for (int i = 0; i < 5; i++)//最初设定5个串口
            {
                comboBox1.Items.Add("COM" + (i + 1).ToString());
            }
            comboBox2.Items.Add("9600");//常用波特率
            comboBox2.Items.Add("19200");
            comboBox2.Items.Add("38400");
            comboBox2.Items.Add("115200");
            label4.Text = num.ToString();
            cache = new Bitmap(320, 240, PixelFormat.Format24bppRgb);
            this.timerDrawImage.Enabled = false;
        }

        /*
         主按钮，控制串口打开与关闭，在此之前对串口进行配置 
         */
        private void button4_Click(object sender, EventArgs e)//打开/关闭串口按钮，开始任务
        {
            if (!isOpen)
            {
                if (!isSetProperty)
                {
                    SetPortProperty();
                    isSetProperty = true;
                }
                try//打开串口
                {
                    sp.Open();
                    isOpen = true;
                    button4.Text = "关闭串口";
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    isSetProperty = true;
                    this.richTextBox1.Text = "";
                    s = new StreamWriter("数据.txt");
                    this.timerDrawImage.Enabled = true;
                }
                catch
                {
                    isSetProperty = false;
                    isOpen = false;
                    button4.Text = "打开串口";
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    //MessageBox.Show("打开串口失败，请重试！", "错误提示");
                }
            }
            else
            {
                try//关闭串口
                {
                    sp.Close();
                    isOpen = false;
                    isSetProperty = false;
                    isSetProperty = false;
                    button4.Text = "打开串口";
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    s.Close();
                    this.timerDrawImage.Enabled = false;
                }
                catch
                {
                    button4.Text = "关闭串口";
                    comboBox1.Enabled = false;
                    comboBox2.Enabled = false;
                    MessageBox.Show("关闭串口失败，请重试！", "错误提示");
                }
            }
        }

        /*
         串口配置方法 
         */
        private void SetPortProperty()
        {
            sp = new SerialPort();
            sp.PortName = comboBox1.Text.Trim();
            //以下都是蓝牙常用配置，一般不做修改
            sp.BaudRate = Convert.ToInt32(comboBox2.Text.Trim());//设置波特率
            sp.StopBits = StopBits.One;//1位停止位
            sp.DataBits = 8;//8位数据位
            sp.Parity = Parity.None;//无校验位
            sp.ReadTimeout = -1;//设置超时读取时间
            sp.RtsEnable = true;
            sp.DataReceived += new SerialDataReceivedEventHandler(sp_dataReceived);//接收到数据触发spGlobal.DATAReceived方法
        }

        /*
         串口接收处理 
         */
        private void sp_dataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (this != null)
            {
                this.Invoke((EventHandler)delegate
                {
                    while (Global.CurrentIndex >= Global.DATA.Length)
                        Global.CurrentIndex -= Global.DATA.Length;
                    Global.DATA[Global.CurrentIndex] = (byte)sp.ReadByte();
                    //this.richTextBox1.Text += Global.DATA[Global.CurrentIndex]+ " ";
                    Global.CurrentIndex++;

                });
            }
        }



        private void button1_Click(object sender, EventArgs e)//清除接收按钮
        {
            Global.CurrentIndex = 0;
            richTextBox1.Text = "";
            num = 0;
            label4.Text = num.ToString();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)//测试画图，画线，区分画图区
        {
            _graphics = e.Graphics;
            Pen pn = new Pen(Color.Black);
            Point pt1 = new Point(0, 200);
            Point pt2 = new Point(480, 200);
            _graphics.DrawLine(pn, pt1, pt2);
        }

        private void button3_Click(object sender, EventArgs e)//清除画图按钮，清除FORM画图，同时重新画线，last和j置初始值
        {
            Global.CurrentIndex = 0;
            _graphics = this.CreateGraphics();
            _graphics.Clear(Color.White);
            Pen pn = new Pen(Color.Red);
            Point pt1 = new Point(0, 300);
            Point pt2 = new Point(480, 300);
            _graphics.DrawLine(pn, pt1, pt2);
            last = 390;
            j = 30;
            num = 0;
        }

        private void button5_Click(object sender, EventArgs e)//数据存储，存储至文件夹下“数据.txt”文件中
        {
            s.Close();
            s = new StreamWriter("数据" + count_txt.ToString() + ".txt");
            count_txt++;
            _graphics = this.CreateGraphics();
            _graphics.Clear(Color.White);
            num = 0;
            label4.Text = num.ToString();
            count = 1;
            j_h = j = 30;
            last = 290;
            last_h = 490;

        }

        private void timerDrawImage_Tick(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)(delegate
            {
                using (_graphics = this.CreateGraphics())
                {
                    if (Global.CurrentIndex > 0)
                        this.richTextBox1.Text = string.Format("{0} is {1}", Global.CurrentIndex, Global.DATA[Global.CurrentIndex - 1]);

                    //for (int i = 10; i > 0; i--)
                    //{
                    //    this.richTextBox1.Text += Global.DATA[i].ToString()+ " ";
                    //}
                    PointBitmap pb = new PointBitmap(cache);

                    pb.LockBits();
                    for (int i = 0; i < Global.DATA.Length; i += 2)
                    {
                        int x = (i % 640) / 2;
                        int y = i / 640;

                        int red = Global.DATA[i] * 4 > 255 ? Global.DATA[i] : 255;
                        int green = Global.DATA[i+1] * 4 > 255 ? Global.DATA[i+1] : 255;
                        int blue = red / 3 + green * 2 / 3;
                        //int red = (Global.DATA[i] & 0xF8) >> 3;
                        //int green = ((Global.DATA[i] & 0x07) << 3) | ((Global.DATA[i + 1] & 0xE0) >> 5);
                        //int blue = Global.DATA[i + 1] & 0x1F;
                        Color c = Color.FromArgb(red,green,blue);
                        pb.SetPixel(x, y, c);
                        //this.richTextBox1.Text += Global.DATA[i] + "-" + Global.DATA[i + 1] + "";
                    }
                    pb.UnlockBits();
                    _graphics.DrawImage(cache, 10, 200);

                }

            }));
        }


    }
}
