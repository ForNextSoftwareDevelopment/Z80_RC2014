using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Z80_RC2014
{
    public partial class FormCommand : Form
    {
        #region Members

        public string instruction;

        #endregion

        #region Constructor

        public FormCommand(string explanation, string description, string mnemonic)
        {
            InitializeComponent();

            this.Text = description;
            tbInstruction.Text = mnemonic;
            tbExplanation.Text = explanation;
        }

        #endregion

        #region EventHandlers

        private void btnOK_Click(object sender, EventArgs e)
        {
            instruction = tbInstruction.Text;
        }

        #endregion
    }
}
