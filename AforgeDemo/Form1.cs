using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AforgeDemo
{
	public partial class Form1 : Form
	{
		//用来操作摄像头 
		private VideoCaptureDevice Camera = null;
		//用来把每一帧图像编码到视频文件
		private VideoFileWriter VideoOutPut = new VideoFileWriter();
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_Paint(object sender, PaintEventArgs e)
		{

		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			pictureBox1.Refresh();
		}

		private void Form1_Shown(object sender, EventArgs e)
		{
			//获取摄像头列表
			var devs = new FilterInfoCollection(FilterCategory.VideoInputDevice);

			//实例化设备控制类
			Camera = new VideoCaptureDevice(devs[0].MonikerString);

			//配置录像参数(宽,高,帧率,比特率,等)
			Camera.VideoResolution = Camera.VideoCapabilities[0];

			//设置回调,aforge会不断从这个回调推出图像数据
			Camera.NewFrame += Camera_NewFrame;

			//开始
			Camera.Start();

			//打开录像文件(如果没有则创建,如果有也会清空).
			VideoOutPut.Open("E:/VIDEO.MP4", Camera.VideoResolution.FrameSize.Width, Camera.VideoResolution.FrameSize.Height, Camera.VideoResolution.AverageFrameRate, VideoCodec.MPEG4, Camera.VideoResolution.BitCount);
		}

		//图像缓存
		private Bitmap bmp = new Bitmap(1, 1);
		////摄像头输出回调
		private void Camera_NewFrame(object sender, NewFrameEventArgs eventArgs)
		{
			//写到文件
			VideoOutPut.WriteVideoFrame(eventArgs.Frame);
			lock (bmp)
			{
				//释放上一个缓存
				bmp.Dispose();
				//保存一份缓存
				bmp = eventArgs.Frame.Clone() as Bitmap;
			}
		}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			//停摄像头
			Camera.Stop();

			//关闭录像文件,如果忘了不关闭,将会得到一个损坏的文件,无法播放
			VideoOutPut.Close();
		}

		private void pictureBox1_Paint(object sender, PaintEventArgs e)
		{
			if (bmp == null) return;

			lock (bmp) e.Graphics.DrawImage(bmp, 0, 0);
		}
	}
}
