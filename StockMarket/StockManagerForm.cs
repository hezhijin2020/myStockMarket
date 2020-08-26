using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HZJ.StockMarket
{
    public partial class StockManagerForm : Form
    {
        public StockManagerForm()
        {
            InitializeComponent();
        }
        private void StockManagerForm_Load(object sender, EventArgs e)
        {
            Inital();
        }

        public void Inital()
        {
           string codes= AppSeting.ReadToStr();
           List<StockModel> lst=  StockMarket.GetStockInfo(codes, true);
            BindingLTV(lst);
        }

        public void BindingLTV(List<StockModel> lst)
        {
            foreach (StockModel s in lst)
            {
                AddItem(s.Code, s.StockName);
            }
        }
        public void AddItem(string Code,string Name)
        {
            ltv.BeginUpdate();
            var item = new ListViewItem();
            item.Text = Code;
            item.SubItems.Add(Name);
            ltv.Items.Add(item);
            ltv.EndUpdate();
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtCodeName.Text.Trim() != null && txtCodeName.Text.Trim().Length == 6)
            {
                var code = txtCodeName.Text.Trim();
                if (StockMarket.ValiCode(ref code))
                {
                    var name=StockMarket.GetCodeName(code);
                    if (name != "")
                    {
                        AddItem(code, name);
                    }
                    else {
                        MessageBox.Show("代码有误！");
                    }
                }
            }
            else {
                MessageBox.Show("输入错误！");
            }
        }

        private void btnDel_Click(object sender, EventArgs e)
        {
            if (ltv.SelectedItems.Count >= 0)
            {
                var item = ltv.SelectedItems[0];
                ltv.Items.Remove(item);
            }
        }

        private void StockManagerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                List<string> lt = new List<string>();
                foreach (ListViewItem itme in ltv.Items)
                {
                    lt.Add(itme.Text);
                }
                var codes = String.Join(",", lt.ToArray());
                AppSeting.SaveToStr(codes);
                base.DialogResult = DialogResult.OK;
            }
            catch 
            {
                base.DialogResult = DialogResult.Cancel;
            }
           
        }
    }
}
