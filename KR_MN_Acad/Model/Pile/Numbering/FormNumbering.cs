using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KR_MN_Acad.Model.Pile.Numbering
{
    partial class FormNumbering : Form
    {
        public Options Options { get; set; }

        public FormNumbering(Options options)
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = options;
            Options = options;
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            propertyGrid1.ResetSelectedProperty();
        }
    }
}
