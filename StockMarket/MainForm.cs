using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace HZJ.StockMarket
{
    public partial class MainForm : Form
    {
        private List<string> StockList = null;

        private bool IsChange=false;
        public MainForm()
        {
            InitializeComponent();
            ltView.MouseWheel += ltView_MouseWheel;
            InitData();
            ShowPoint();
        }

        public void ShowPoint()
        {
            int xWidth = SystemInformation.PrimaryMonitorSize.Width;//获取显示器屏幕宽度

            int yHeight = SystemInformation.PrimaryMonitorSize.Height;//高度
            this.Location = new Point(xWidth-this.Width, yHeight-this.Height-30);//这里需要再减去窗体本身的宽度和高度的一半
        }

        public void InitData()
        {
            StockList = AppSeting.Read();
        }

        #region 代码管理
        private void StockManager_Click(object sender, System.EventArgs e)
        {
            StockManagerForm fm = new StockManagerForm();
            if (fm.ShowDialog() == DialogResult.OK)
            {
                InitData();
                IsChange = true;
            }
        }
        #endregion

        #region 窗体透明度设置
        /// <summary>
        /// 滚动鼠标设置窗体的透明度
        /// </summary>
        private void ltView_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                ChangeOpacity(0.05);
            }
            else
            {
                ChangeOpacity(-0.05);
            }
            
        }

        public void ChangeOpacity(double Num=0.05)
        {
            double OpNum = this.Opacity + Num;
            if (OpNum>=1||OpNum<=0)
            {
                return;
            }
            this.Opacity = OpNum;
        }

        #endregion

        #region 数据绑定
        private void timer1_Tick(object sender, System.EventArgs e)
        {
            if (StockList != null && StockList.Count > 0)
            {
               var Tsource  = StockMarket.GetStockInfo(StockList);
               BindltView(Tsource);
            }
        }

        /// <summary>
        /// 数据绑定方法
        /// </summary>
        /// <param name="TSource">数据源</param>
        public void BindltView( List<StockModel> TSource)
        {
            ltView.BeginUpdate();
            if (IsChange)
            {
                ltView.Items.Clear();
                IsChange = false;
            }
            foreach (StockModel m in TSource)
            {
                int index= IndexOf(m.Code);
                if (index <0)
                {
                    AddItem(m);
                }
                else 
                {
                    ModifyItem(m,index);
                }
            }
            ltView.EndUpdate();
        }
        public int IndexOf(string Code)
        {
            for (int i = 0; i < ltView.Items.Count; i++)
            {
                if (ltView.Items[i].SubItems[1].Text.Equals(Code))
                {
                    return i;
                }
            }
            return -1;
        }
        public void AddItem(StockModel m)
        {
            ListViewItem item = new ListViewItem();
            if (m.DiffPrice > 0)
                item.ForeColor = Color.Red;
            else
                item.ForeColor = Color.Green;
            item.Text = m.StockName;
            item.SubItems.Add(m.Code);
            item.SubItems.Add(m.NowPrice.ToString());
            item.SubItems.Add(string.Format("{0:0.00%}", m.DiffPercent));
            item.SubItems.Add(m.DiffPrice.ToString());
            ltView.Items.Add(item);
        }
        public void DelItem(StockModel m)
        {
            ListViewItem item = new ListViewItem();
            if (m.DiffPrice > 0)
                item.ForeColor = Color.Red;
            else
                item.ForeColor = Color.Green;
            item.Text = m.StockName;
            item.SubItems.Add(m.Code);
            item.SubItems.Add(m.NowPrice.ToString());
            item.SubItems.Add(string.Format("{0:0.00%}", m.DiffPercent));
            item.SubItems.Add(m.DiffPrice.ToString());
            ltView.Items.Remove(item);
        }
        public void ModifyItem(StockModel m,int index)
        {
            if (m.DiffPrice > 0)
                ltView.Items[index].ForeColor = Color.Red;
            else
                ltView.Items[index].ForeColor = Color.Green;

            ltView.Items[index].SubItems[1].Text=m.Code;
            ltView.Items[index].SubItems[2].Text = m.NowPrice.ToString();
            ltView.Items[index].SubItems[3].Text = string.Format("{0:0.00%}", m.DiffPercent);
            ltView.Items[index].SubItems[4].Text = m.DiffPrice.ToString();
        }

        #endregion

        #region ESC 退出程序

        private void ESCKeyDown_AppExit(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Application.Exit();
            }
            else if (e.KeyCode == Keys.Up)
            {
                ChangeOpacity();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                ChangeOpacity(-0.05);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F1)
            {
                this.Opacity = 0.2;
            }
            else if (e.KeyCode == Keys.F2)
            {
                this.Opacity = 0.4;
            }
            else if (e.KeyCode == Keys.F3)
            {
                this.Opacity = 0.5;
            }
            else if (e.KeyCode == Keys.F4)
            {
                this.Opacity = 0.6;
            }
        }
        #endregion

    }
}
