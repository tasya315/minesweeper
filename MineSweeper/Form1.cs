using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing.Text;

namespace MineSweeper
{
    public partial class Form1 : Form
    {
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont,
            IntPtr pdv, [System.Runtime.InteropServices.In] ref uint pcFonts);

        private PrivateFontCollection fonts = new PrivateFontCollection();

        Font myFont, myFont2;
        public Form1()
        {
            InitializeComponent();
            byte[] fontData = Properties.Resources.Minecraftia_Regular;
            IntPtr fontPtr = System.Runtime.InteropServices.Marshal.AllocCoTaskMem(fontData.Length);
            System.Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontData.Length);
            uint dummy = 0;
            fonts.AddMemoryFont(fontPtr, Properties.Resources.Minecraftia_Regular.Length);
            AddFontMemResourceEx(fontPtr, (uint)Properties.Resources.Minecraftia_Regular.Length, IntPtr.Zero, ref dummy);
            System.Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr);
            myFont = new Font(fonts.Families[0], 12.0F);
            myFont2 = new Font(fonts.Families[0], 10.0F);
        }
        List<List<Button>> listButton = new List<List<Button>>();
        int sisaBom;
        int totalBom = 10;
        Boolean cheat = false;
        int time = 0;
        bool start = false;

        public void buatButton()
        {
            for (int i = 0; i < 10; i++)
            {
                listButton.Add(new List<Button>());
                for (int j = 0; j < 10; j++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(30, 30);
                    btn.Top = 60 + (i * 29);
                    btn.Left = 65 + (j * 29);
                    btn.BackColor = Color.LightGray;
                    btn.ForeColor = Color.Black;
                    btn.Font = new Font("Lucida Sans Typewriter", 10, FontStyle.Regular);
                    btn.Tag = 0;
                    btn.FlatAppearance.BorderColor = Color.Black;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.MouseDown += new MouseEventHandler(button1_Click);
                    this.Controls.Add(btn);
                    listButton[i].Add(btn);
                }
            }

            randomBom();
            
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(!listButton[i][j].Tag.Equals("B"))
                    {
                        int count = 0;
                        for(int k = -1; k < 2; k++)
                        {
                            for (int l = -1; l < 2; l++)
                            {
                                if ((k != 0 || l != 0) && i + k >= 0 && i + k < 10 && j + l >= 0 && j + l < 10 && listButton[i + k][j + l].Tag.Equals("B"))
                                    count++;
                            }
                        }
                        listButton[i][j].Tag = count;
                    }
                }
            }
        }

        public void randomBom()
        {
            for (int i = 0; i < totalBom; i++)
            {
                Random rnd = new Random();
                int x, y;
                Boolean ada;
                do
                {
                    y = rnd.Next(0, 10);
                    x = rnd.Next(0, 10);
                    ada = false;
                    if (listButton[y][x].Tag.Equals("B"))
                        ada = true;
                    listButton[y][x].Tag = "B";
                }
                while (ada);
            }
        }

        public void reveal()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    listButton[i][j].Text = listButton[i][j].Tag.ToString();
                    if (listButton[i][j].Text.Equals("B"))
                        listButton[i][j].BackColor = Color.Red;
                    else
                        openButton(listButton[i][j], false);
                }
            }
        }

        public void revealBom(bool win)
        {
            if(win)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (listButton[i][j].Tag.Equals("B"))
                        {
                            listButton[i][j].Text = "F";
                            listButton[i][j].BackColor = Color.LightGreen;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                    {
                        if (listButton[i][j].Tag.Equals("B"))
                        {
                            listButton[i][j].Text = listButton[i][j].Tag.ToString();
                            listButton[i][j].BackColor = Color.Red;
                        }
                    }
                }
            }
        }

        public void hide()
        {
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if(listButton[i][j].BackColor != Color.DarkGray)
                    {
                        listButton[i][j].Text = "";
                        listButton[i][j].BackColor = Color.LightGray;
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.AutoSize = false;
            label1.TextAlign = ContentAlignment.MiddleCenter;
            label1.Dock = DockStyle.None;
            label1.Left = 10;
            label1.Font = myFont;
            label2.Font = myFont;
            label3.Font = myFont;
            button1.Font = myFont2;
            label1.Width = this.Width - (label1.Text.Length / 2) - 40;
            reset(false);
        }

        private void label1_Click(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (label1.Text.Equals("Minesweeper"))
                {
                    if(cheat)
                    {
                        cheat = false;
                        hide();
                    }
                    else
                    {
                        cheat = true;
                        reveal();
                    }
                }
            }
        }

        private void button1_Click(object sender, MouseEventArgs e)
        {   
            if(!start)
            {
                start = true;
                timer1 = new System.Windows.Forms.Timer();
                timer1.Tick += new EventHandler(timer1_Tick);
                timer1.Interval = 1000;
                timer1.Start();
            }
            Button btn = (Button)sender;
            if (btn.Tag.Equals("Reset"))
                reset(true);
            else
            {
                if(e.Button == MouseButtons.Left)
                {
                    if(!btn.Text.Equals("F"))
                    {
                        if (btn.Tag.Equals("B"))
                            gameOver();
                        else if(Int16.Parse(btn.Tag.ToString()) > 0)
                        {
                            if(btn.BackColor != Color.DarkGray)
                                openButton(btn, true);
                        }
                        else
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                for (int j = 0; j < 10; j++)
                                {
                                    if (listButton[i][j] == btn)
                                    {
                                        backtracking(i, j);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(btn.BackColor != Color.DarkGray)
                    {
                        if(btn.Text.Equals("F"))
                        {
                            btn.Text = "";
                            sisaBom++;
                        }
                        else if(sisaBom > 0)
                        {
                            btn.Text = "F";
                            sisaBom--;
                        }
                    }
                    label2.Text = (Math.Floor((double)sisaBom / 100)).ToString() + (Math.Floor((double)sisaBom / 10) % 10).ToString() + (sisaBom % 10).ToString();
                }
                cekWin();
            }
            label1.Focus();
        }

        public void openButton(Button btn, Boolean open)
        {
            btn.Text = btn.Tag.ToString();
            if (btn.Text.Equals("0"))
                btn.Text = "";
            else if (btn.Text.Equals("1"))
                btn.ForeColor = Color.Blue;
            else if (btn.Text.Equals("2"))
                btn.ForeColor = Color.Green;
            else if (btn.Text.Equals("3"))
                btn.ForeColor = Color.Red;
            else if (btn.Text.Equals("4"))
                btn.ForeColor = Color.DarkBlue;
            else if (btn.Text.Equals("5"))
                btn.ForeColor = Color.DarkRed;
            else if (btn.Text.Equals("6"))
                btn.ForeColor = Color.DarkCyan;
            else if (btn.Text.Equals("7"))
                btn.ForeColor = Color.DarkMagenta;
            else if (btn.Text.Equals("8"))
                btn.ForeColor = Color.DarkOrange;
            if (open)
            {
                btn.Enabled = false;
                btn.BackColor = Color.DarkGray;
                btn.Enabled = true;
            }
        }

        public void backtracking(int i, int j)
        {
            if (i < 0 || i > 9 || j < 0 || j > 9 || listButton[i][j].BackColor == Color.DarkGray)
                return;
            openButton(listButton[i][j], true);
            if (Int16.Parse(listButton[i][j].Tag.ToString()) > 0)
                return;
            for (int k = -1; k < 2; k++)
            {
                for (int l = -1; l < 2; l++)
                {
                    if (k != 0 || l != 0)
                        backtracking(i + k, j + l);
                }
            }
        }

        public void gameOver()
        {
            timer1.Stop();
            label1.Text = "You Lose!";
            revealBom(false);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    listButton[i][j].Enabled = false;
            }
            button1.Text = "Play Again";
            button1.BackColor = Color.Yellow;
            button1.ForeColor = Color.Black;
        }

        public void reset(bool res)
        {
            start = false;
            label1.Text = "Minesweeper";
            button1.BackColor = Color.Red;
            button1.Text = "Reset";
            button1.Tag = "Reset";
            button1.ForeColor = Color.White;
            time = 0;
            timer1.Stop();
            sisaBom = totalBom;
            label2.Text = (Math.Floor((double)sisaBom / 100)).ToString() + (Math.Floor((double)sisaBom / 10) % 10).ToString() + (sisaBom % 10).ToString();
            label3.Text = "000";
            if(res)
            {
                for (int i = 0; i < 10; i++)
                {
                    for (int j = 0; j < 10; j++)
                        listButton[i][j].Visible = false;
                }
            }
            listButton = new List<List<Button>>();
            buatButton();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    listButton[i][j].Visible = true;
                    listButton[i][j].FlatStyle = FlatStyle.Flat;
                }
            }
        }

        public void cekWin()
        {
            int count = 0;
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (listButton[i][j].BackColor != Color.DarkGray)
                        count++;
                }
            }
            if (count == totalBom)
                Win();
        }

        public void Win()
        {
            timer1.Stop();
            label1.Text = "You Win!";
            revealBom(true);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                    listButton[i][j].Enabled = false;
            }
            button1.Text = "Play Again";
            button1.ForeColor = Color.Black;
            button1.BackColor = Color.Yellow;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time++;
            label3.Text = (Math.Floor((double)time/100)).ToString() + (Math.Floor((double)time / 10) % 10).ToString() + (time % 10).ToString();
        }
    }
}
