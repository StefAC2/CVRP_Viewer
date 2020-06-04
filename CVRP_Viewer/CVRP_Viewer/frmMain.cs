using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public partial class frmMain : Form
    {
        DepotManager depotManager;

        bool FirstTime = true;

        public frmMain()
        {
            InitializeComponent();

            DataImporter dataImporter = new DataImporter();

            //dataImporter.ImportFromFile(@".\1G2.DAT");
            dataImporter.ImportFromFile(@".\A-n37-k6.vrp");

            depotManager = dataImporter.GetManager();

            Paint += depotManager.Paint;

            depotManager.CreateRandomRoutes();
            //depotManager.OptRoutes();
        }

        private void frmMain_Paint(object sender, PaintEventArgs e)
        {
            int size = 7;

            for (int i = 0; i < depotManager.NbClients; i++)
            {
                Node node = depotManager.GetClient(i);

                Rectangle rect = new Rectangle(node.DrawPos.X - size / 2, node.DrawPos.Y - size / 2, size, size);

                if (i == depotManager.DepotIndex)
                {
                    e.Graphics.FillEllipse(Brushes.Black, rect);
                }
                else
                {
                    e.Graphics.DrawEllipse(Pens.Black, rect);
                }
            }
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            if (FirstTime)
            {
                FirstTime = false;
                btnRestart.Text = "Restart";
            }
            else
            {
                depotManager.CreateRandomRoutes();
            }

            depotManager.Solve(this);

            Refresh();

            this.Text = "CVRP Viewer | Total cost: " + depotManager.TotalCost();
        }
    }
}