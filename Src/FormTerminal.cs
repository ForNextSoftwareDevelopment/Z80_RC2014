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
    public partial class FormTerminal : Form
    {
        #region Members

        // Initial location of window
        private int x, y;

        // Keyboard buffer
        public string keyBuffer;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public FormTerminal(int x, int y)
        {
            InitializeComponent();

            this.x = x;
            this.y = y;

            keyBuffer = "";
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Form loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FormTerminal_Load(object sender, EventArgs e)
        {
            // Set location of window
            this.Location = new Point(x, y);

            tbTerminal.Font = new Font(FontFamily.GenericMonospace, 10.25F);
        }

        /// <summary>
        /// Key pressed, no action from control characters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbTerminal_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Key pressed, send to terminal
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tbTerminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            keyBuffer += e.KeyChar;
            e.Handled = true;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            tbTerminal.Text = "";
        }

        #endregion
    }
}
