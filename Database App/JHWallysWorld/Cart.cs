/*
* FILE:				Cart.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains the Cart class and all of its properties and methods.
*/


using System.Collections.Generic;

namespace JHWallysWorld
{
    /*
    * CLASS: Cart
    * DESCRIPTION:
    *	The Cart class simply contains properties belonging to a Cart (used for ObservalbeCollections).
    *   The Cart contains a List of Products called a shopping list. This is the user's current order before they
    *   submit it as a new order.
    */
    class Cart
    {
        private List<Product> shoppingList;

        public List<Product> ShoppingList
        {
            get { return shoppingList; }
            set { shoppingList = value; }
        }
    }
}
