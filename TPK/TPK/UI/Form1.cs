using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            //InitializeComponent();
            this.Text = "Welcome!";
            this.Size = new System.Drawing.Size(450, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            Form_Load();
        }
        private System.Windows.Forms.DataGridView dgv1, dgv2, dgv3;//datagrid
        private System.Windows.Forms.Button btnAddPrK,btnCheckPrK,btnReducePrK, btnReset;//butoane
        private ComboBox cmb;//lista tabele tip combobox
        public void Form_Load()
        {
            //cmb
            cmb = new ComboBox();
            cmb.Name = "cmb";
            cmb.Location = new System.Drawing.Point(0, 0);
            cmb.Size = new System.Drawing.Size(80, 20);
            cmb.SelectedIndexChanged += new System.EventHandler(cmb_Select);

            String[] s = new String[5] { "t1", "t2", "t3", "t4", "t5" };
            for (int i = 0; i < s.Length; i++)
            {
                cmb.Items.Add(s[i]);
            }
            Controls.Add(cmb);
            //btnAddPrK
            btnAddPrK = new System.Windows.Forms.Button();
            btnAddPrK.Location = new System.Drawing.Point(80, 0);
            btnAddPrK.Size = new System.Drawing.Size(80, 20);
            btnAddPrK.Text = "Add";
            btnAddPrK.UseVisualStyleBackColor = true;
            btnAddPrK.Click += new System.EventHandler(btnAddPrK_Click);
            Controls.Add(btnAddPrK);
            //btnCheckPrK
            btnCheckPrK = new System.Windows.Forms.Button();
            btnCheckPrK.Location = new System.Drawing.Point(160, 0);
            btnCheckPrK.Size = new System.Drawing.Size(80, 20);
            btnCheckPrK.Text = "Check";
            btnCheckPrK.UseVisualStyleBackColor = true;
            btnCheckPrK.Click += new System.EventHandler(btnCheckPrK_Click);
            Controls.Add(btnCheckPrK);
            //btnReducePrK
            btnReducePrK = new System.Windows.Forms.Button();
            btnReducePrK.Location = new System.Drawing.Point(240, 0);
            btnReducePrK.Size = new System.Drawing.Size(80, 20);
            btnReducePrK.Text = "Reduce";
            btnReducePrK.UseVisualStyleBackColor = true;
            btnReducePrK.Click += new System.EventHandler(btnReducePrK_Click);
            Controls.Add(btnReducePrK);
            //btnReset
            btnReset = new System.Windows.Forms.Button();
            btnReset.Location = new System.Drawing.Point(320, 0);
            btnReset.Size = new System.Drawing.Size(80, 20);
            btnReset.Text = "Reset";
            btnReset.UseVisualStyleBackColor = true;
            btnReset.Click += new System.EventHandler(btnReset_Click);
            Controls.Add(btnReset);
            //dgv1
            dgv1 = new System.Windows.Forms.DataGridView();
            dgv1.Name = "dgv1";
            dgv1.Location = new System.Drawing.Point(0, 20);
            dgv1.Size = new System.Drawing.Size(440, 50);
            Controls.Add(dgv1);
            //dgv1 restraints
            dgv1.AllowUserToAddRows = false;
            dgv1.AllowUserToDeleteRows = false;
            dgv1.ReadOnly = true;
            //dgv2
            dgv2 = new System.Windows.Forms.DataGridView();
            dgv2.Name = "dgv2";
            dgv2.Location = new System.Drawing.Point(0, 70);
            dgv2.Size = new System.Drawing.Size(440, 200);
            Controls.Add(dgv2);
            //dgv2 restraints
            dgv2.AllowUserToAddRows = false;
            dgv2.AllowUserToDeleteRows = false;
            dgv2.ReadOnly = true;
            //dgv3
            dgv3 = new System.Windows.Forms.DataGridView();
            dgv3.Name = "dgv3";
            dgv3.Location = new System.Drawing.Point(0, 270);
            dgv3.Size = new System.Drawing.Size(440, 100);
            Controls.Add(dgv3);
            //dgv2 restraints
            dgv3.AllowUserToAddRows = false;
            dgv3.AllowUserToDeleteRows = false;
            dgv3.ReadOnly = true;
        }
        private void dgv_Fill(String table)
        {
            //dgv1
            dgv1.Columns.Clear();
            dgv1.Rows.Clear();
            dgv1.Columns.Add("Value", "Table Name");
            dgv1.Rows.Add(new object[] { table });
            //dgv2
            dgv2.Columns.Clear();
            dgv2.Rows.Clear();
            DB db = new DB();
            List<string> list = db.GetColumn(table);
            for (int i = 0; i < list.Count; i++)
            {
                dgv2.Columns.Add("Value", list[i]);
            }
            List<string>[] row = db.Select(table, list);
            for (int i = 0; i < row[0].Count; i++)
            {
                int index = dgv2.Rows.Add();
                for (int j = 0; j < row.Length; j++)
                {
                    dgv2.Rows[index].Cells[j].Value = row[j][i];
                }
            }
            //dgv3
            dgv3.Columns.Clear();
            dgv3.Rows.Clear();
            dgv3.Columns.Add("Value", "PK");
            list = db.GetPrKey(table);
            for (int i = 0; i < list.Count; i++)
            {
                dgv3.Rows.Add(new object[] {list[i]});
            }
        }
        private void cmb_Select(object sender, EventArgs e)
        {
            dgv_Fill(cmb.Text);
        }
        //adauga cheie primara daca nu exista
        private void btnAddPrK_Click(object sender, EventArgs e)
        {
            new DB().AddPrimaryKey(dgv1.Rows[0].Cells[0].Value.ToString());
            dgv_Fill(dgv1.Rows[0].Cells[0].Value.ToString());
        }
        //verifica cheie primara, daca nu e numerica schimba
        private void btnCheckPrK_Click(object sender, EventArgs e)
        {
            new DB().CheckPrimaryKey(dgv1.Rows[0].Cells[0].Value.ToString());
            dgv_Fill(dgv1.Rows[0].Cells[0].Value.ToString());
        }
        //verifica cheie primara, daca nu e singulara schimba
        private void btnReducePrK_Click(object sender, EventArgs e)
        {
            new DB().ReducePrimaryKey(dgv1.Rows[0].Cells[0].Value.ToString());
            dgv_Fill(dgv1.Rows[0].Cells[0].Value.ToString());
        }
        //reseteaza baza de date
        private void btnReset_Click(object sender, EventArgs e)
        {
            new DB().Reset(dgv1.Rows[0].Cells[0].Value.ToString());
            dgv_Fill(dgv1.Rows[0].Cells[0].Value.ToString());
        }
    }
}
