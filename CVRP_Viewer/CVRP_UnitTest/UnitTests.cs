using CVRP_Viewer;
using NUnit.Framework;
using System.Collections.Generic;

namespace CVRP_UnitTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DataImportTest_vrp()
        {
            // Arrange
            DataImporter dataImporter = new DataImporter();

            // Act
            // Parse vrp file and create DepotManager
            dataImporter.ImportFromFile(@".\A-n37-k6.vrp");

            // Get DepotManager from DataImporter
            DepotManager dm = dataImporter.GetManager();

            // Assert
            Assert.AreEqual(86, dm.GetClient(0).Position.X);
            Assert.AreEqual(22, dm.GetClient(0).Position.Y);
            Assert.AreEqual(0, dm.GetClient(0).Demande);

            Assert.AreEqual(29, dm.GetClient(1).Position.X);
            Assert.AreEqual(17, dm.GetClient(1).Position.Y);
            Assert.AreEqual(1, dm.GetClient(1).Demande);

            Assert.AreEqual(54, dm.GetClient(17).Position.X);
            Assert.AreEqual(39, dm.GetClient(17).Position.Y);
            Assert.AreEqual(14, dm.GetClient(17).Demande);

            Assert.AreEqual(83, dm.GetClient(35).Position.X);
            Assert.AreEqual(74, dm.GetClient(35).Position.Y);
            Assert.AreEqual(66, dm.GetClient(35).Demande);

            Assert.AreEqual(84, dm.GetClient(36).Position.X);
            Assert.AreEqual(2, dm.GetClient(36).Position.Y);
            Assert.AreEqual(21, dm.GetClient(36).Demande);
        }

        [Test]
        public void DataImportTest_dat()
        {
            // Arrange
            DataImporter dataImporter = new DataImporter();

            // Act
            // Parse dat file and create DepotManager
            dataImporter.ImportFromFile(@".\1G2.DAT");

            // Get DepotManager from DataImporter
            DepotManager dm = dataImporter.GetManager();

            // Assert
            Assert.AreEqual(50, dm.GetClient(0).Position.X);
            Assert.AreEqual(50, dm.GetClient(0).Position.Y);
            Assert.AreEqual(0, dm.GetClient(0).Demande);

            Assert.AreEqual(5, dm.GetClient(1).Position.X);
            Assert.AreEqual(5, dm.GetClient(1).Position.Y);
            Assert.AreEqual(10, dm.GetClient(1).Demande);

            Assert.AreEqual(95, dm.GetClient(50).Position.X);
            Assert.AreEqual(45, dm.GetClient(50).Position.Y);
            Assert.AreEqual(10, dm.GetClient(50).Demande);

            Assert.AreEqual(85, dm.GetClient(99).Position.X);
            Assert.AreEqual(95, dm.GetClient(99).Position.Y);
            Assert.AreEqual(10, dm.GetClient(99).Demande);

            Assert.AreEqual(95, dm.GetClient(100).Position.X);
            Assert.AreEqual(95, dm.GetClient(100).Position.Y);
            Assert.AreEqual(10, dm.GetClient(100).Demande);
        }

        [Test]
        public void RouteCostTest()
        {
            // Arrange
            DataImporter dataImporter = new DataImporter();

            // Act
            // Parse vrp file and create DepotManager
            dataImporter.ImportFromFile(@".\A-n37-k6.vrp");

            // Get DepotManager from DataImporter
            DepotManager dm = dataImporter.GetManager();

            List<int[]> locations = new List<int[]>();

            int[] tmp1 = { 7, 25, 35, 16 };
            locations.Add(tmp1);

            int[] tmp2 = { 18, 31, 19, 9, 21, 26 };
            locations.Add(tmp2);

            int[] tmp3 = { 14, 6, 36, 29, 24 };
            locations.Add(tmp3);

            int[] tmp4 = { 33, 2, 28, 23, 22, 12, 11, 10, 4 };
            locations.Add(tmp4);

            int[] tmp5 = { 13, 30, 15, 32, 27 };
            locations.Add(tmp5);

            int[] tmp6 = { 20, 8, 5, 3, 1, 34, 17 };
            locations.Add(tmp6);

            int totalCost = 0;

            Node depot = dm.Depot;

            foreach (int[] tmp in locations)
            {
                Truck truck = new Truck(depot);

                for (int i = tmp.Length - 1; i >= 0; i--)
                {
                    truck.AddNodeAfter(truck.Head, dm.GetClient(tmp[i]));
                }

                totalCost += truck.CalcCost();
            }

            // Assert
            Assert.AreEqual(949, totalCost);
        }
    }
}