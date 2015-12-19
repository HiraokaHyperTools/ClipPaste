using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;

namespace ClipPaste {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            DoubleBuffered = true;
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
        }

        private void Form1_Load(object sender, EventArgs e) {
            var rc = Screen.GetWorkingArea(this);
            Left = rc.Right - Width;
            Top = rc.Bottom - Height;
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.Copy;
        }

        String fpPNG;

        private void Form1_DragDrop(object sender, DragEventArgs e) {
            label1.Hide();

            String s = "" + e.Data.GetData("Sender");
            if (s == "" + GetHashCode()) return;

            var alfp = e.Data.GetData(DataFormats.FileDrop) as String[];
            if (alfp != null) {
                foreach (var fp in alfp) {
                    try {
                        Bitmap pic = new Bitmap(fp);
                        float nw = pic.Width / pic.HorizontalResolution * 96;
                        float nh = pic.Height / pic.VerticalResolution * 96;
                        var rc = Screen.GetWorkingArea(this);
                        rc.Inflate(-rc.Width / 5, -rc.Height / 5);
                        float f = Math.Max(1, Math.Max(nw / rc.Width, nh / rc.Height));

                        Debug.WriteLine("# {0},{1}  {2},{3}  {4}", pic.Width, pic.Height, pic.HorizontalResolution, pic.VerticalResolution, f);

                        pic.SetResolution(pic.HorizontalResolution * f, pic.VerticalResolution * f);

                        pic.Save(fpPNG = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".png"), ImageFormat.Png);
                        BackgroundImage = pic;
                    }
                    catch (Exception err) {
                        BackgroundImage = null;
                        label1.Text = "" + err;
                        label1.Show();
                    }
                    break;
                }
            }
            else {
            }
        }

        DataObject ole = new DataObject();

        private void Form1_MouseDown(object sender, MouseEventArgs e) {
            if (BackgroundImage == null) return;
            ole.SetImage(BackgroundImage);
            ole.SetData(DataFormats.FileDrop, new String[] { fpPNG });
            ole.SetData("Sender", GetHashCode());
            DoDragDrop(ole, DragDropEffects.All);
        }
    }
}
