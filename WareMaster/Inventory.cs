﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WareMaster
{
    public class InventoryData
    {
        public int id { get; set; } //Item_Id or Category_Id or transaction.id
        public string Name { get; set; }    //Itemname or Category_Name or Itemname
        public int Quantity { get; set; }   
        public decimal Total { get; set; }
        public DateTime Date { get; set; }
        public string comment {  get; set; }
        public InventoryData Add(InventoryData other)
        {
            if (other != null )
            {
                this.Quantity += other.Quantity;
                this.Total += other.Total;
            }
            return this;
        }
        public override string ToString()
        {
            return $"id: {id}, Name: {Name}, Quantity: {Quantity}, Total: {Total}, Date: {Date}";
        }
    }
    internal class Inventory
    {
        static internal DateTime GetFirstSettleDate()
        {
            DateTime firstSettleDate = Globals.wareMasterEntities.Settlements
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Min();
            return firstSettleDate;
        }
        static internal DateTime GetLastSettleDate()
        {
            DateTime lastSettleDate = Globals.wareMasterEntities.Settlements
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();
            return lastSettleDate;
        }
        static internal DateTime GetFirstSettleDateByItem(Item item)
        {
            DateTime firstSettleDate = Globals.wareMasterEntities.Settlements
                .Where(s => s.Item_Id == item.id)
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Min();
            return firstSettleDate;
        }
        static internal DateTime GetLastSettleDateByItem(Item item) {
            DateTime lastSettleDate = Globals.wareMasterEntities.Settlements
                .Where(s=>s.Item_Id==item.id)
                .Select(s => s.Settle_Date)
                .DefaultIfEmpty(DateTime.MinValue)
                .Max();
            return lastSettleDate;
        }


        static internal Settlement GetLastSettlementByItem(Item item, DateTime dateBefore)
        {
            return Globals.wareMasterEntities.Settlements
                    .Where(s => s.Item_Id == item.id && s.Settle_Date <=  dateBefore)
                    .OrderByDescending(s => s.Settle_Date)
                    .FirstOrDefault();
        }

        static internal List<InventoryData> GetFirstInventories()
        {
            DateTime firstSettleDate = GetFirstSettleDate();
            return GetAllInventoriesByItem(firstSettleDate);
        }

        static internal InventoryData GetInventoryByItem(Item item, DateTime dateOfInventory)
        {
            Settlement latestSettlemnt= GetLastSettlementByItem(item,dateOfInventory);
            DateTime beginDate=(latestSettlemnt!=null)? latestSettlemnt.Settle_Date: DateTime.MinValue;
            InventoryData inventoryData = new InventoryData
            {
                id = item.id,
                Name = item.Itemname,
                Quantity=(latestSettlemnt!=null) ?latestSettlemnt.Quantity:0,
                Total= (latestSettlemnt != null) ? latestSettlemnt.Total:0,
                Date = dateOfInventory.Date,
            };
            
            inventoryData.Add(GetSumInboundsByItem(item, beginDate, dateOfInventory))
                .Add(GetSumOutboundsByItem(item, beginDate, dateOfInventory));

            return inventoryData;
        }

        static internal InventoryData GetInventoryByCategory(Category category,DateTime dateOfInventory)
        {
            List<Item> items = Globals.wareMasterEntities.Items.Where(item=>item.Category_Id == category.id).ToList();
            InventoryData inventoryData = new InventoryData();
            inventoryData.id = category.id;
            inventoryData.Name = category.Category_Name; 
            inventoryData.Date = dateOfInventory;

            foreach(Item item in items)
            {
                inventoryData.Add(GetInventoryByItem(item, dateOfInventory));
            }
            return inventoryData;
        }
        static internal List<InventoryData> GetAllInventoriesByItem(DateTime dateOfInventory)
        {
            List<InventoryData> inventories = new List<InventoryData>();
            List<Item> items=Globals.wareMasterEntities.Items.ToList();
            foreach(Item item in items)
            {
                inventories.Add(GetInventoryByItem(item,dateOfInventory));
            }
            return inventories;
        }
        static internal List<InventoryData> GetAllInventoriesByCategory(DateTime dateOfInventory)
        {
            List<InventoryData> inventories=new List<InventoryData>();
            List<Category> categories = Globals.wareMasterEntities.Categories.ToList();
            foreach (Category category in categories)
            {
                inventories.Add(GetInventoryByCategory(category, dateOfInventory));
            }
            return inventories;
        }

        static internal List<InventoryData> GetInboundsByItem(Item item, DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> inbounds = Globals.wareMasterEntities.Transactions
                .Where(t => t.Item_Id == item.id && t.Quantity > 0 && t.Transaction_Date > dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();

            List<InventoryData> inventoryList = inbounds.Select(t => new InventoryData
            {
                id = t.id,
                Name = item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();

            return inventoryList;
        }
        static internal List<InventoryData> GetInboundsByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> inbounds = Globals.wareMasterEntities.Transactions
                .Where(t => t.Item.Category_Id == category.id && t.Quantity > 0 && t.Transaction_Date > dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();

            List<InventoryData> inventoryList = inbounds.Select(t => new InventoryData
            {
                id = t.id,
                Name = t.Item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();

            return inventoryList;
        }

        static internal InventoryData GetSumInboundsByItem(Item item,DateTime dateBegin,DateTime dateEnd)
        {
            InventoryData inventory=new InventoryData();
            inventory.id = item.id;
            inventory.Name = item.Itemname;
            inventory.Date = dateEnd;
            inventory.comment="Inbounds";
            List<InventoryData> inbounds = GetInboundsByItem(item, dateBegin, dateEnd);
            foreach(InventoryData inbound in inbounds)
            {
                inventory.Add(inbound);
            }

            return inventory;
        }

        static internal InventoryData GetSumInboundsByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            InventoryData inventory=new InventoryData();
            inventory.id=category.id;
            inventory.Name=category.Category_Name; 
            inventory.Date = dateEnd;
            inventory.comment = "Inbounds";
            List<InventoryData> inbounds = GetInboundsByCategory(category,  dateBegin,  dateEnd);
            foreach (InventoryData inbound in inbounds)
            {
                inventory.Add(inbound);
            }
            return inventory;
        }

        static internal List<InventoryData> GetOutboundsByItem(Item item, DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> outbounds = Globals.wareMasterEntities.Transactions
                .Where(t => t.Item_Id == item.id && t.Quantity < 0 && t.Transaction_Date > dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();

            List<InventoryData> inventoryList = outbounds.Select(t => new InventoryData
            {
                id = t.id,
                Name = item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();

            return inventoryList;
        }
        static internal List<InventoryData> GetOutboundsByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> outbounds = Globals.wareMasterEntities.Transactions
                .Where(t => t.Item.Category_Id == category.id && t.Quantity < 0 && t.Transaction_Date > dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();

            List<InventoryData> inventoryList = outbounds.Select(t => new InventoryData
            {
                id = t.id,
                Name = t.Item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();

            return inventoryList;
        }

        static internal InventoryData GetSumOutboundsByItem(Item item, DateTime dateBegin, DateTime dateEnd)
        {
            InventoryData inventory = new InventoryData();
            inventory.id = item.id;
            inventory.Name = item.Itemname;
            inventory.Date = dateEnd;
            inventory.comment = "Outbounds";
            List<InventoryData> outbounds = GetOutboundsByItem(item, dateBegin, dateEnd);
            foreach (InventoryData outbound in outbounds)
            {
                inventory.Add(outbound);
            }

            return inventory;
        }

        static internal InventoryData GetSumOutboundsByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            InventoryData inventory = new InventoryData();
            inventory.id = category.id;
            inventory.Name = category.Category_Name;
            inventory.Date = dateEnd;
            inventory.comment = "Outbounds";
            List<InventoryData> outbounds=GetOutboundsByCategory(category, dateBegin, dateEnd);
            foreach (InventoryData outbound in outbounds)
            {
                inventory.Add(outbound);
            }
            return inventory;
        }

        static internal List<InventoryData> GetSummaryByItem(Item item, DateTime dateBegin, DateTime dateEnd)
        {
            List<InventoryData> summary = new List<InventoryData>();
            InventoryData inventoryBegin = GetInventoryByItem(item, dateBegin);
            inventoryBegin.comment = "Beginning Inventory";
            summary.Add(inventoryBegin);
            summary.Add(GetSumInboundsByItem(item, dateBegin, dateEnd));
            summary.Add(GetSumOutboundsByItem(item, dateBegin, dateEnd));
            InventoryData inventoryEnd = GetInventoryByItem(item, dateEnd);
            inventoryEnd.comment = "Ending Inventory";
            summary.Add(inventoryEnd);
            return summary;
        }
        static internal List<InventoryData> GetAllSummaryByItem(DateTime dateBegin, DateTime dateEnd)
        {
            List<InventoryData> summary = new List<InventoryData>();
            List<Item> items = Globals.wareMasterEntities.Items.ToList();
            foreach(Item item in items)
            {
                summary.AddRange(GetSummaryByItem(item, dateBegin, dateEnd));
            }
            return summary;
        }
        static internal List<InventoryData> GetSummaryByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            List<InventoryData> summary = new List<InventoryData>();
            InventoryData inventoryBegin = GetInventoryByCategory(category, dateBegin);
            inventoryBegin.comment = "Beginning Inventory";
            summary.Add(inventoryBegin);
            summary.Add(GetSumInboundsByCategory(category, dateBegin, dateEnd));
            summary.Add(GetSumOutboundsByCategory(category, dateBegin, dateEnd));
            InventoryData inventoryEnd = GetInventoryByCategory(category, dateEnd);
            inventoryEnd.comment = "Ending Inventory";
            summary.Add(inventoryEnd);
            return summary;
        }
        static internal List<InventoryData> GetAllSummaryByCategory(DateTime dateBegin, DateTime dateEnd)
        {
            List<InventoryData> summary = new List<InventoryData>();
            List<Category> categories = Globals.wareMasterEntities.Categories.ToList();
            foreach (Category  category in categories)
            {
                summary.AddRange(GetSummaryByCategory(category, dateBegin, dateEnd));
            }
            return summary;
        }

        static internal List<InventoryData> GetInventoryChangeDetailsByItem(Item item,DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> transactions = Globals.wareMasterEntities.Transactions
                .Where(t=>t.Item.id==item.id && t.Transaction_Date>=dateBegin && t.Transaction_Date<=dateEnd)
                .ToList();
            List<InventoryData> inventoryChanges=transactions.Select(t=>new InventoryData
            {
                id=t.id,
                Name=t.Item.Itemname,
                Quantity=t.Quantity,
                Total=t.Total,
                Date=t.Transaction_Date
            }).ToList();
            return inventoryChanges;
        }
        static internal List<InventoryData> GetAllInventoryChangeDetailsByItem( DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> transactions = Globals.wareMasterEntities.Transactions
                .Where(t => t.Transaction_Date >= dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();
            List<InventoryData> inventoryChanges = transactions.Select(t => new InventoryData
            {
                id = t.id,
                Name = t.Item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();
            return inventoryChanges;
        }
        static internal List<InventoryData> GetInventoryChangeDetailsByCategory(Category category, DateTime dateBegin, DateTime dateEnd)
        {
            List<Transaction> transactions = Globals.wareMasterEntities.Transactions
                .Where(t => t.Item.Category.id == category.id && t.Transaction_Date >= dateBegin && t.Transaction_Date <= dateEnd)
                .ToList();
            List<InventoryData> inventoryChanges = transactions.Select(t => new InventoryData
            {
                id = t.id,
                Name = t.Item.Itemname,
                Quantity = t.Quantity,
                Total = t.Total,
                Date = t.Transaction_Date
            }).ToList();
            return inventoryChanges;
        }
    }
}
