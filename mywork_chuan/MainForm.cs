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
    public partial class MainForm : Form
    {
        /// <summary>
        /// has a  Serial Port Component
        /// </summary>
        SerialPort serialPort = null;
        bool isOn = false;//打开串口标志位
        ByteArrayRenderDelegate byteArrayRenderDelegate;



        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            foreach (var p in System.IO.Ports.SerialPort.GetPortNames())//最初设定5个串口
            {
                comboBoxPort.Items.Add(p);
            }
            comboBoxBaud.Items.Add("9600");//常用波特率
            comboBoxBaud.Items.Add("19200");
            comboBoxBaud.Items.Add("38400");
            comboBoxBaud.Items.Add("115200");
            this.timerDrawImage.Enabled = false;

            this.pictureBoxRender.Image = new Bitmap(320,240);

            // default
#if DEBUG
            this.comboBoxPort.SelectedItem = "COM1";
            this.comboBoxBaud.SelectedItem = "115200";
            RawRender rawRender = new RawRender();
            this.byteArrayRenderDelegate = rawRender.SimpleRGB;
#endif
        }

        /*
         主按钮，控制串口打开与关闭，在此之前对串口进行配置 
         */
        private void buttonSwitch_Click(object sender, EventArgs e)//打开/关闭串口按钮，开始任务
        {
            if (isOn)
            {
                SwitchOff();
            }
            else
            {
                SwitchOn();
            }
        }

        /*
         串口配置方法 
         */
        private void SetPortProperty()
        {
            serialPort = new SerialPort();
            serialPort.PortName = string.Format("{0}", comboBoxPort.SelectedItem);
            serialPort.BaudRate = Convert.ToInt32(comboBoxBaud.SelectedItem);//设置波特率
            serialPort.StopBits = StopBits.One;//1位停止位
            serialPort.DataBits = 8;//8位数据位
            serialPort.Parity = Parity.None;//无校验位
            serialPort.ReadTimeout = -1;//设置超时读取时间
            serialPort.RtsEnable = true;
            serialPort.DataReceived += delegate(object sender, SerialDataReceivedEventArgs e)
            {
                // cycled array
                int count = serialPort.BytesToRead;
                if (count + Shared.CurrentIndex >= Shared.RawData.Length)
                {
                    serialPort.Read(Shared.RawData, Shared.CurrentIndex, Shared.RawData.Length - Shared.CurrentIndex - 1);
                    serialPort.Read(Shared.RawData, 0, count + Shared.CurrentIndex - Shared.RawData.Length + 1);
                }
                else
                {
                    serialPort.Read(Shared.RawData, Shared.CurrentIndex, serialPort.BytesToRead);

                }
                Shared.CurrentIndex = (Shared.CurrentIndex + count) % Shared.RawData.Length;
            };
        }

        private void buttonClear_Click(object sender, EventArgs e)//清除接收按钮
        {
            Shared.CurrentIndex = 0;
            richTextBoxOutput.Text = "";
        }

        /// <summary>
        /// set current to ZERO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRefresh_Click(object sender, EventArgs e)//清除画图按钮，清除FORM画图，同时重新画线，last和j置初始值
        {
            Shared.CurrentIndex = 0;
        }

        private void buttonSave_Click(object sender, EventArgs e)//数据存储，存储至文件夹下“数据.txt”文件中
        {
            using (FileStream s = new FileStream("数据" + DateTime.Now.ToLongDateString() + ".txt",FileMode.CreateNew))
            {
                // todo: write image data to file
                Image img = this.pictureBoxRender.Image;
                img.Save(s,img.RawFormat);
            }
        }

        private void timerDrawImage_Tick(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)(delegate
            {
                if (Shared.CurrentIndex > 0)
                    this.richTextBoxOutput.Text = string.Format("{0} is {1}", Shared.CurrentIndex, Shared.RawData[Shared.CurrentIndex - 1]);
                // Re-render bytes to image
                //Bitmap bmp = new Bitmap(this.pictureBoxRender.Image);
                //if(this.byteArrayRenderDelegate!=null)
                //{
                //    this.byteArrayRenderDelegate(bmp,Shared.RawData);
                //}
                //this.pictureBoxRender.Image = bmp;

                GenericBitmap genericBitmap = new GenericBitmap();
                genericBitmap.Width = this.pictureBoxRender.Width;
                genericBitmap.Height = this.pictureBoxRender.Height;
                BitmapColorReader reader = new BitmapColorReader();
                genericBitmap.ReadFromByteArray(reader.TestHandler,Shared.RawData);
                this.pictureBoxRender.Image = genericBitmap.ToBitmap();
            }));
        }

        private void SwitchOn()
        {
            buttonSwitch.Text = "关闭串口";
            comboBoxPort.Enabled = false;
            comboBoxBaud.Enabled = false;
            this.richTextBoxOutput.Text = "";
            this.timerDrawImage.Enabled = true;
            SetPortProperty();
            this.serialPort.Open();
            isOn = true;
        }

        private void SwitchOff()
        {
            this.serialPort.Close();
            buttonSwitch.Text = "打开串口";
            comboBoxPort.Enabled = true;
            comboBoxBaud.Enabled = true;
            this.timerDrawImage.Enabled = false;
            isOn = false;
        }

        private void Reset()
        {
            Shared.RawData = new byte[Shared.RawData.Length];
            Shared.CurrentIndex = 0;
        }

        private void RefreshUI()
        {

        }
    }
}
