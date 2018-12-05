using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace WARTLS.CLASSES
{
    enum ItemType
    {
        CONSUMABLE= 1,
        PERMANENT = 2,
        DEFAULT = 3,
        NO_REPAIR = 4,
        TIME=5
    }
    class Item
    {
        internal static byte EquipperCalc(int Slot)
        {
            //Основное
            if (Slot == 0) return 0;// Не у кого
            if (Slot == 1) return 1; // Оружие штурмовика
            if (Slot == 32768) return 8; // Оружие медика
            if (Slot == 1048576) return 16; // Оружие инженера
            if (Slot == 1024) return 4; // Оружие снайпера

            //Шлем
            if (Slot == 12) return 1; // Шлем штурмовика
            if (Slot == 393216) return 8; // Шлем медика
            if (Slot == 12582912) return 16; // Шлем инженера
            if (Slot == 12288) return 4; // Шлем снайпера

            //Скины
            if (Slot == 23) return 1; // Скин штурмовика
            if (Slot == 753664) return 8; // Скин медика
            if (Slot == 24117248) return 16; // Скин инженера
            if (Slot == 23552) return 4; // Скин снайпера

            //Особое
            if (Slot == 2) return 1; // Особое штурмовика
            if (Slot == 163840) return 8; // Особое медика
            if (Slot == 2097152) return 16; // Особое инженера
            if (Slot == 23552) return 4; // Особое снайпера


            //Основное-скин
            if (Slot == 26) return 1; // Основное-скин штурмовика
            if (Slot == 851968) return 8; // Основное-скин медика
            if (Slot == 27262976) return 16; // Основное-скин инженера
            if (Slot == 26624) return 4; // Основное-скин снайпера


            //Дополнительное оружие
            if (Slot == 3247107) return 29; // У всех 
            if (Slot == 3) return 1; //Только штурмовик
            if (Slot == 98304) return 8; // Только медик
            if (Slot == 3145728) return 16; // Только инженер
            if (Slot == 3072) return 4; // Только снайпер
            if (Slot == 98307) return 9; //Штурмовик и медик
            if (Slot == 3244032) return 24; //Медик и инженер
            if (Slot == 3148800) return 20; //Инженер и снайпер
            if (Slot == 3145731) return 17; // Штурмовик и инженер
            if (Slot == 3075) return 5; // Штурмовик и снайпер
            if (Slot == 101376) return 12; // Медик и снайпер
            if (Slot == 3148800) return 1; // Инженер и снайпер
            if (Slot == 3247104) return 28; // Только НЕ штурмовик 
            if (Slot == 3148803) return 21; // Только НЕ медик 
            if (Slot == 101379) return 13; // Только НЕ инженер 
            if (Slot == 3244035) return 25; // Только НЕ снайпер 

            //Холодное оружие (нож, топор и так далее)
            if (Slot == 4329476) return 29; // У всех 
            if (Slot == 4) return 1; //Только штурмовик
            if (Slot == 131072) return 8; // Только медик
            if (Slot == 4194304) return 16; // Только инженер
            if (Slot == 4096) return 4; // Только снайпер
            if (Slot == 131076) return 9; //Штурмовик и медик
            if (Slot == 4325376) return 24; //Медик и инженер
            if (Slot == 4198400) return 20; //Инженер и снайпер
            if (Slot == 4194308) return 17; // Штурмовик и инженер
            if (Slot == 4100) return 5; // Штурмовик и снайпер
            if (Slot == 135168) return 12; // Медик и снайпер
            if (Slot == 4198400) return 1; // Инженер и снайпер
            if (Slot == 4329472) return 28; // Только НЕ штурмовик 
            if (Slot == 4198404) return 21; // Только НЕ медик 
            if (Slot == 135172) return 13; // Только НЕ инженер 
            if (Slot == 4325380) return 25; // Только НЕ снайпер 

            // Карманы
            if (Slot == 5411845) return 29; // У всех 
            if (Slot == 163845) return 9; //Штурмовик и медик
            if (Slot == 5406720) return 24; //Медик и инженер
            if (Slot == 4198400) return 20; //Инженер и снайпер
            if (Slot == 4194308) return 17; // Штурмовик и инженер
            if (Slot == 5125) return 5; // Штурмовик и снайпер
            if (Slot == 135168) return 12; // Медик и снайпер
            if (Slot == 5248000) return 1; // Инженер и снайпер
            if (Slot == 5411840) return 28; // Только НЕ штурмовик 
            if (Slot == 5248005) return 21; // Только НЕ медик 
            if (Slot == 168965) return 13; // Только НЕ инженер 
            if (Slot == 5406725) return 25; // Только НЕ снайпер 

            
            if (Slot == 23812118) return 29; // У всех 
            if (Slot == 22) return 1; //Только штурмовик
            if (Slot == 720896) return 8; // Только медик
            if (Slot == 23068672) return 16; // Только инженер
            if (Slot == 22528) return 4; // Только снайпер
            if (Slot == 720918) return 9; //Штурмовик и медик
            if (Slot == 23789568) return 24; //Медик и инженер
            if (Slot == 23091200) return 20; //Инженер и снайпер
            if (Slot == 23068694) return 17; // Штурмовик и инженер
            if (Slot == 22550) return 5; // Штурмовик и снайпер
            if (Slot == 743424) return 12; // Медик и снайпер
            if (Slot == 4329472) return 28; // Только НЕ штурмовик 
            if (Slot == 23091222) return 21; // Только НЕ медик 
            if (Slot == 743446) return 13; // Только НЕ инженер 
            if (Slot == 23789590) return 25; // Только НЕ снайпер 

            //Дополнительное оружие - скин
            if (Slot == 29223963) return 29; // У всех 
            if (Slot == 27) return 1; //Только штурмовик
            if (Slot == 884736) return 8; // Только медик
            if (Slot == 28311552) return 16; // Только инженер
            if (Slot == 27648) return 4; // Только снайпер
            if (Slot == 884763) return 9; //Штурмовик и медик
            if (Slot == 29196288) return 24; //Медик и инженер
            if (Slot == 28339200) return 20; //Инженер и снайпер
            if (Slot == 28311579) return 17; // Штурмовик и инженер
            if (Slot == 27675) return 5; // Штурмовик и снайпер
            if (Slot == 912384) return 12; // Медик и снайпер
            if (Slot == 29223936) return 28; // Только НЕ штурмовик 
            if (Slot == 28339227) return 21; // Только НЕ медик 
            if (Slot == 912411) return 13; // Только НЕ инженер 
            if (Slot == 29196315) return 25; // Только НЕ снайпер 

            //Бронежилет (1)
            if (Slot == 18400273) return 29; // У всех 
            if (Slot == 17) return 1; //Только штурмовик
            if (Slot == 557056) return 8; // Только медик
            if (Slot == 17825792) return 16; // Только инженер
            if (Slot == 17408) return 4; // Только снайпер
            if (Slot == 557073) return 9; //Штурмовик и медик
            if (Slot == 18382848) return 24; //Медик и инженер
            if (Slot == 17843200) return 20; //Инженер и снайпер
            if (Slot == 17825809) return 17; // Штурмовик и инженер
            if (Slot == 17425) return 5; // Штурмовик и снайпер
            if (Slot == 574464) return 12; // Медик и снайпер
            if (Slot == 18400256) return 28; // Только НЕ штурмовик 
            if (Slot == 17843217) return 21; // Только НЕ медик 
            if (Slot == 574481) return 13; // Только НЕ инженер 
            if (Slot == 18382865) return 25; // Только НЕ снайпер 

            //Перчатки (2)
            if (Slot == 7576583) return 29; // У всех 
            if (Slot == 7) return 1; //Только штурмовик
            if (Slot == 229376) return 8; // Только медик
            if (Slot == 7340032) return 16; // Только инженер
            if (Slot == 7168) return 4; // Только снайпер
            if (Slot == 229383) return 9; //Штурмовик и медик
            if (Slot == 7569408) return 24; //Медик и инженер
            if (Slot == 7347200) return 20; //Инженер и снайпер
            if (Slot == 7340039) return 17; // Штурмовик и инженер
            if (Slot == 7175) return 5; // Штурмовик и снайпер
            if (Slot == 236544) return 12; // Медик и снайпер
            if (Slot == 7576576) return 28; // Только НЕ штурмовик 
            if (Slot == 7347207) return 21; // Только НЕ медик 
            if (Slot == 236551) return 13; // Только НЕ инженер 
            if (Slot == 7569415) return 25; // Только НЕ снайпер 

            //Ботинки (3)
            if (Slot == 17317904) return 29; // У всех 
            if (Slot == 16) return 1; //Только штурмовик
            if (Slot == 524288) return 8; // Только медик
            if (Slot == 16777216) return 16; // Только инженер
            if (Slot == 16384) return 4; // Только снайпер
            if (Slot == 524304) return 9; //Штурмовик и медик
            if (Slot == 17301504) return 24; //Медик и инженер
            if (Slot == 16793600) return 20; //Инженер и снайпер
            if (Slot == 16777232) return 17; // Штурмовик и инженер
            if (Slot == 16400) return 5; // Штурмовик и снайпер
            if (Slot == 540672) return 12; // Медик и снайпер
            if (Slot == 17317888) return 28; // Только НЕ штурмовик 
            if (Slot == 16793616) return 21; // Только НЕ медик 
            if (Slot == 540688) return 13; // Только НЕ инженер 
            if (Slot == 17301520) return 25; // Только НЕ снайпер 

            //Шлем (0)
            if (Slot == 12988428) return 29; // У всех 
            if (Slot == 393228) return 9; //Штурмовик и медик
            if (Slot == 12976128) return 24; //Медик и инженер
            if (Slot == 12595200) return 20; //Инженер и снайпер
            if (Slot == 12582924) return 17; // Штурмовик и инженер
            if (Slot == 12300) return 5; // Штурмовик и снайпер
            if (Slot == 405504) return 12; // Медик и снайпер
            if (Slot == 12988416) return 28; // Только НЕ штурмовик 
            if (Slot == 12595212) return 21; // Только НЕ медик 
            if (Slot == 405516) return 13; // Только НЕ инженер 
            if (Slot == 12976140) return 25; // Только НЕ снайпер 
            return 0;
        }
        internal string BundleTime = "0";
        internal string BundleQuantity = "0";
        internal ItemType ItemType;
        internal long ID = 0;
        internal string Name;
        internal string Config;
        internal string AttachedTo;
        internal byte Equipped;
        internal int Slot;
        internal long BuyTime;
        internal long ExpirationTime;
        internal long SecondsLeft => ItemType == ItemType.CONSUMABLE || ItemType == ItemType.NO_REPAIR ? 0: ExpirationTime - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        internal bool ExpiredConfirmed = false;
        internal long TotalDurabilityPoints;
        internal long DurabilityPoints;
        internal long RepairCost => 0;
        internal int Quantity;
        internal Item()
        {

        }
        internal Item(ItemType ItemType,long Id,string Name,int Hours=0,int Quantity=0,long DurabilityPoints=36000)
        {
            this.ItemType = ItemType;
            this.AttachedTo = "";
            this.Config = "";
            this.Name = Name;
            this.ID = Id;
            this.BuyTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            switch (ItemType)
            {
                case ItemType.CONSUMABLE:
                    this.Quantity = Quantity;
                    break;
                case ItemType.PERMANENT:
                    this.DurabilityPoints = DurabilityPoints;
                    this.TotalDurabilityPoints = DurabilityPoints;
                    break;
                case ItemType.TIME:
                    this.ExpirationTime = BuyTime + (int)TimeSpan.FromHours(Hours).TotalSeconds;
                    break;
            }
        }
        internal XElement Serialize()
        {
            XElement Element = new XElement("item");
            Element.Add(new XAttribute("type", (int)ItemType));
            Element.Add(new XAttribute("id", ID));
            Element.Add(new XAttribute("name", Name));
            Element.Add(new XAttribute("attached_to", AttachedTo));
            Element.Add(new XAttribute("config", Config));
            Element.Add(new XAttribute("slot", Slot));
            Element.Add(new XAttribute("equipped", Equipped));
            Element.Add(new XAttribute("default", ItemType == ItemType.DEFAULT ? 1 : 0));
            Element.Add(new XAttribute("permanent", ItemType == ItemType.PERMANENT ? 1 : 0));
            Element.Add(new XAttribute("expired_confirmed", ExpiredConfirmed ? 1 : 0));
            Element.Add(new XAttribute("buy_time_utc", BuyTime));

            if (ItemType == ItemType.CONSUMABLE)
                Element.Add(new XAttribute("quantity", Quantity));
            else if (ItemType == ItemType.DEFAULT)
                Element.Add(new XAttribute("seconds_left", 0));
            else if (ItemType == ItemType.TIME)
            {
                Element.Add(new XAttribute("expiration_time_utc", ExpirationTime));
                if((bool)App.Default["UseOldMode"])
                    Element.Add(new XAttribute("hours_left",  TimeSpan.FromSeconds(SecondsLeft).TotalHours ));
                else
                    Element.Add(new XAttribute("seconds_left", SecondsLeft));
            }
            else if (ItemType == ItemType.PERMANENT)
            {
                Element.Add(new XAttribute("total_durability_points", TotalDurabilityPoints));
                Element.Add(new XAttribute("durability_points", DurabilityPoints));
            }
            return Element;
        }
        internal void Create(XmlElement Item)
        {
            this.ItemType = (ItemType)int.Parse(Item.Attributes["type"].InnerText);
            this.ID = long.Parse(Item.Attributes["id"].InnerText);
            this.Name = Item.Attributes["name"].InnerText;
            this.AttachedTo = Item.Attributes["attached_to"].InnerText;
            this.Config = Item.Attributes["config"].InnerText;
            this.Slot = int.Parse(Item.Attributes["slot"].InnerText);
            this.Equipped = byte.Parse(Item.Attributes["equipped"].InnerText);
            this.ExpiredConfirmed = Item.Attributes["expired_confirmed"].InnerText == "1";
            this.BuyTime = long.Parse(Item.Attributes["buy_time_utc"].InnerText);

            if (ItemType == ItemType.CONSUMABLE)
                this.Quantity = int.Parse(Item.Attributes["quantity"].InnerText);
            else if (ItemType == ItemType.TIME)
                this.ExpirationTime = long.Parse(Item.Attributes["expiration_time_utc"].InnerText);
            else if (ItemType == ItemType.PERMANENT)
            {
                this.TotalDurabilityPoints = long.Parse(Item.Attributes["total_durability_points"].InnerText);
                this.DurabilityPoints = long.Parse(Item.Attributes["durability_points"].InnerText);
            }

        }
    }
}
