using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;        //需要新加的命名空间（头文件）
using System.IO;

namespace mywork_chuan
{
    public partial class MainForm : Form
    {
        /// <summary>
        /// has a  Serial Port Component
        /// </summary>
        SerialPort m_serialPort = null;
        bool m_isOn = false;//打开串口标志位
        byte[] m_byteStorage ;
        ISerializeHandler m_serializeHandler;
        int m_startIndex = 0;
        int m_bmpWidth = 320;
        int m_bmpHeight = 240;
        int m_currentIndex = 0;

        /// <summary>
        /// Initialize Components
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            this.m_serializeHandler = new RGB888SerializeHandler();
            m_byteStorage = new byte[m_startIndex + m_bmpHeight * m_bmpWidth * m_serializeHandler.PixelWidth];
            m_serialPort = new SerialPort();
        }
        

        /// <summary>
        /// Set up UI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            this.MaximumSize = this.Size;
            this.MinimumSize = this.Size;
            this.MaximizeBox = false;
            foreach (var p in System.IO.Ports.SerialPort.GetPortNames())
            {
                comboBoxPort.Items.Add(p);
            }
            comboBoxBaud.Items.Add("9600");//常用波特率
            comboBoxBaud.Items.Add("19200");
            comboBoxBaud.Items.Add("38400");
            comboBoxBaud.Items.Add("115200");
            this.timerDrawImage.Enabled = false;


            this.pictureBoxRender.Image = new Bitmap(320, 240);


            // default
#if DEBUG
            this.comboBoxPort.SelectedItem = "COM1";
            this.comboBoxBaud.SelectedItem = "115200";
#endif
        }


        private void buttonSwitch_Click(object sender, EventArgs e)
        {
            if (m_isOn)
            {
                this.SwitchOff();
            }
            else
            {
                this.SwitchOn();
            }
        }

        
        
        private void buttonClear_Click(object sender, EventArgs e)//清除接收按钮
        {
            this.Reset();
        }

        private void buttonSave_Click(object sender, EventArgs e)//数据存储，存储至文件夹下“数据.txt”文件中
        {
            string filename="数据" + DateTime.Now.ToFileTimeUtc() + ".txt";
            this.pictureBoxRender.Image.Save(filename,System.Drawing.Imaging.ImageFormat.Bmp);
        }

        private void timerDrawImage_Tick(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)(delegate
            {
                string text = "";
                for (int i = m_startIndex; i < m_currentIndex; i++)
                { text += string.Format("{0:x2} ", m_byteStorage[i]); }
                this.richTextBoxOutput.Text= text;


                this.pictureBoxRender.Image=BitmapSerializer.DeSerialize(this.m_byteStorage,
                    m_startIndex,
                    this.pictureBoxRender.Width,
                    this.pictureBoxRender.Height,
                    this.m_serializeHandler.BitmapDeSerialize);
            }));
        }

        private void SetupSerialPort()
        {
            m_serialPort.PortName = string.Format("{0}", comboBoxPort.SelectedItem);
            m_serialPort.BaudRate = Convert.ToInt32(comboBoxBaud.SelectedItem);//设置波特率
            m_serialPort.StopBits = StopBits.One;//1位停止位
            m_serialPort.DataBits = 8;//8位数据位
            m_serialPort.Parity = Parity.None;//无校验位
            m_serialPort.ReadTimeout = -1;//设置超时读取时间
            m_serialPort.RtsEnable = true;
            m_serialPort.DataReceived += delegate(object sender, SerialDataReceivedEventArgs e)
            {
                int length = m_serialPort.BytesToRead;
                m_serialPort.Read(m_byteStorage, m_currentIndex, length);
                m_currentIndex += length;
            };
        }


        private void SwitchOn()
        {
            buttonSwitch.Text = "关闭串口";
            comboBoxPort.Enabled = false;
            comboBoxBaud.Enabled = false;
            this.richTextBoxOutput.Text = "";
            this.timerDrawImage.Enabled = true;
            SetupSerialPort();
            this.m_serialPort.Open();
            m_isOn = true;
        }

        private void SwitchOff()
        {
            this.m_serialPort.Close();
            buttonSwitch.Text = "打开串口";
            comboBoxPort.Enabled = true;
            comboBoxBaud.Enabled = true;
            this.timerDrawImage.Enabled = false;
            m_isOn = false;
        }

        private void Reset()
        {
            m_byteStorage = new byte[m_byteStorage.Length];
            m_currentIndex = m_startIndex;
            richTextBoxOutput.Text = "";
        }

    }
}
