using System;
using System.Windows.Forms;

namespace CVRP_Viewer
{
    public partial class frmMain : Form
    {
        DataImporter dataImporter;

        DepotManager depotManager;

        bool FirstTime = true;

        public frmMain()
        {
            InitializeComponent();

            dataImporter = new DataImporter();

            //dataImporter.ImportFromFile(@".\1G2.DAT");
            dataImporter.ImportFromFile(@".\A-n37-k6.vrp");

            depotManager = dataImporter.GetManager();

            Paint += depotManager.Paint;

            depotManager.CreateRandomRoutes();
            //depotManager.OptRoutes();
        }

        private void btnSolve_Click(object sender, EventArgs e)
        {
            if (FirstTime)
            {
                FirstTime = false;
            }
            else
            {
                depotManager.CreateRandomRoutes();
            }

            depotManager.Solve(this);

            Refresh();

            this.Text = "CVRP Viewer | Total cost: " + depotManager.TotalCost();
        }

        private void btnChooseFile_Click(object sender, EventArgs e)
        {
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Paint -= depotManager.Paint;

                dataImporter.ImportFromFile(ofd.FileName);

                depotManager = dataImporter.DepotManager;

                depotManager.CreateRandomRoutes();

                Paint += depotManager.Paint;

                FirstTime = true;

                Refresh();
            }
        }
    }
}