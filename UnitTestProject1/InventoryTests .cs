using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Markup;

namespace WareMaster.Tests
{
    [TestClass]
    public class InventoryTests
    {
        [TestMethod]
        public void TestGetFirstSettleDate()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 2) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object; 

            DateTime result = Inventory.GetFirstSettleDate();

            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }

        [TestMethod]
        public void TestGetFirstSettleDateByItem()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 2) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 3) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object;
            Item item = new Item { id = 1 };
            DateTime result = Inventory.GetFirstSettleDateByItem(item);

            Assert.AreEqual(new DateTime(2023, 1, 1), result);
        }

        [TestMethod]
        public void TestGetLastSettleDateByItem()
        {
            var mockSettlements = new Mock<DbSet<Settlement>>();
            var mockContext = new Mock<WareMasterEntities>();

            var data = new List<Settlement>
            {
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 1) },
                new Settlement { id = 1, Item_Id = 1, Quantity = 10, Total = 100, Settle_Date = new DateTime(2023, 1, 2) },
                new Settlement { id = 2, Item_Id = 2, Quantity = 20, Total = 200, Settle_Date = new DateTime(2023, 1, 3) },
            }.AsQueryable();

            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSettlements.As<IQueryable<Settlement>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());

            mockContext.Setup(m => m.Settlements).Returns(mockSettlements.Object);

            Console.WriteLine("Mock Settlements:");
            foreach (var settlement in mockContext.Object.Settlements)
            {
                Console.WriteLine($"{settlement.id}, {settlement.Item_Id}, {settlement.Quantity}, {settlement.Total}, {settlement.Settle_Date}");
            }
            Globals.wareMasterEntities = mockContext.Object;
            Item item = new Item { id = 1 };
            DateTime result = Inventory.GetLastSettleDateByItem(item);

            Assert.AreEqual(new DateTime(2023, 1, 2), result);
        }

    }
}
