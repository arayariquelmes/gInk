﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Ink;

namespace gInk
{
	public partial class FormDisplay : Form
	{
		public Root Root;
		public InkCollector IC;
		Graphics g;
		Bitmap collectionbitmap;
		public bool Refreshed = false;

		[DllImport("user32.dll")]
		static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
		[DllImport("user32.dll", SetLastError = true)]
		static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);
		[DllImport("user32.dll")]
		static extern int SetWindowLong(IntPtr hWnd, int nIndex, UInt32 dwNewLong);
		[DllImport("user32.dll")]
		public extern static bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
		[DllImport("user32.dll", SetLastError = false)]
		static extern IntPtr GetDesktopWindow();

		public FormDisplay(Root root)
		{
			Root = root;
			collectionbitmap = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
			InitializeComponent();

			this.Left = 0;
			this.Top = 0;
			this.Width = Screen.PrimaryScreen.Bounds.Width;
			this.Height = Screen.PrimaryScreen.Bounds.Height;
			Console.WriteLine(this.Height);

			IC = new InkCollector(this.Handle);
			IC.CollectionMode = CollectionMode.InkOnly;
			//IC.DefaultDrawingAttributes.PenTip = PenTip.Rectangle;
			IC.DefaultDrawingAttributes.AntiAliased = false;
			IC.Enabled = true;

			ToTopMost();
		}

		public void ToTopMost()
		{
			UInt32 dwExStyle = GetWindowLong(this.Handle, -20);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000);
			SetWindowPos(this.Handle, (IntPtr)0, 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0004 | 0x0010 | 0x0020);
			SetLayeredWindowAttributes(this.Handle, 0x00FFFFFF, 0, 0x00000001);
			SetWindowLong(this.Handle, -20, dwExStyle | 0x00080000 | 0x00000020);
			SetWindowPos(this.Handle, (IntPtr)(-1), 0, 0, 0, 0, 0x0002 | 0x0001 | 0x0010 | 0x0020);
		}

		private void Form1_KeyPress(object sender, KeyPressEventArgs e)
		{
			//if (e.KeyChar == 27)
			//	Root.StopInk();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			//Root.StopInk();
		}

		private void timer1_Tick(object sender, EventArgs e)
		{
			if (!Root.FormCollection.IC.CollectingInk && !Refreshed)
			{
				Root.FormCollection.DrawToBitmap(collectionbitmap, new Rectangle(0, 0, Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height));
				this.BackgroundImage = collectionbitmap;
				//this.Refresh();
				Refreshed = true;
			}

			if (Root.FormCollection.IC.CollectingInk)
			{
				g = this.CreateGraphics();
				Stroke stroke = IC.Ink.Strokes[IC.Ink.Strokes.Count - 1];
				IC.Renderer.Draw(g, stroke, IC.DefaultDrawingAttributes);
				Refreshed = false;
			}
		}
	}
}
