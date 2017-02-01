/*
* FILE:				Order.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains the Order class and all of its properties and methods.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JHWallysWorld
{
    /*
    * CLASS: Order
    * DESCRIPTION:
    *	The Order class simply contains properties belonging to a Order (used for ObservalbeCollections).
    *	CustomerFullName is simple the customer's first and last name by id. BranchName is likewise.
    */
    class Order
    {
        private int id;
        private int customerId;
        private int branchId;
        private DateTime date;
        private string status;
        private int quantity;
        private Product product;
        private string customerFullName;
        private string branchName;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public int CustomerId
        {
            get { return customerId; }
            set { customerId = value; }
        }

        public int BranchId
        {
            get { return branchId; }
            set { branchId = value; }
        }

        public DateTime Date
        {
            get { return date; }
            set { date = value; }
        }

        public string Status
        {
            get { return status; }
            set { status = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public Product Product
        {
            get { return product; }
            set { product = value; }
        }

        public string CustomerFullName
        {
            get { return customerFullName; }
            set { customerFullName = value; }
        }

        public string BranchName
        {
            get { return branchName; }
            set { branchName = value; }
        }
    }
}
