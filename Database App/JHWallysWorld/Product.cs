/*
* FILE:				Product.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains the Product class and all of its properties and methods.
*/


namespace JHWallysWorld
{
    /*
    * CLASS: Product
    * DESCRIPTION:
    *	The Product class simply contains properties belonging to a Product (used for ObservalbeCollections).
    *	Inventory Quantity is different from Quantity in that the Inventory Quantity is how much overall that the product exists.
    */
    class Product
    {
        private int id;
        private string name;
        private double price;
        private int inventoryQuantity;
        private int quantity;
        private int inventoryID;
        private bool discontinued;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public double Price
        {
            get { return price; }
            set { price = value; }
        }

        public int InventoryQuantity
        {
            get { return inventoryQuantity; }
            set { inventoryQuantity = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public int InventoryID
        {
            get { return inventoryID; }
            set { inventoryID = value; }
        }

        public Product()
        {
            id = -1;
            name = null;
            price = -1;
            inventoryQuantity = -1;
            quantity = -1;
        }

        public bool Discontinued
        {
            get { return discontinued; }
            set { discontinued = value; }
        }

        public Product(string prodName, double prodPrice, int invqty, int qty)
        {
            name = prodName;
            price = prodPrice;
            inventoryQuantity = invqty;
            quantity = qty;
        }
    }
}
