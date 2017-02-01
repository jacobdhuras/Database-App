/*
* FILE:				mySqlConnector.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains the MySqlConnector class and all of its properties and methods.
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace JHWallysWorld
{
    class MySqlConnector
    {
        private MySqlConnection connection;



        /*
        METHOD: Connect
        DESCRIPTION:
	        This method does the inital connection with the MySql database.
        PARAMETERS:
	        string connectionString: the connection string to connect to (containing id, password, etc).
        RETURNS:
	        None.
        */
        public void Connect(string connectionString)
        {
            try
            {
                // Connect logic
                connection = new MySqlConnection(connectionString);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        /*
        METHOD: GetRows
        DESCRIPTION:
	        This method does a select statement with the MySql database using the columns specified and
            table and other optional arguments specified.
        PARAMETERS:
	        string columns: the column the user wishes to search for; can have multiple columns seperated by commas (ie. "customer, order").
            string table_args: the table in the database to use with the select statement. optional args follow the format of: "Customer where...".
        RETURNS:
	        List<string[]> : the list contains the rows returned by the database call, the string[] contains strings correpsonding to each column
                             returned by each row.
        */
        public List<string[]> GetRows(string columns, string table_args)
        {
            List<string[]> result = new List<string[]>();
            try
            {
                connection.Open();
                MySqlDataReader reader;
                MySqlCommand command = new MySqlCommand();
                command.Connection = connection;

                command.CommandText = "SELECT " + columns + " FROM " + table_args;
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        string[] row = new string[reader.FieldCount];
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[i] = reader.GetString(i);
                        }
                        result.Add(row);
                    }
                }
                reader.Close();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }

            return result;
        }



        /*
        METHOD: InsertCustomer
        DESCRIPTION:
	        This method does an insert statement into the database with a customer. Customer information is
            gathered from the Customer properties passed in as a Customer.
        PARAMETERS:
	        Customer c: the customer to insert into the table.
        RETURNS:
	        None.
        */
        public void InsertCustomer(Customer c)
        {
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "INSERT INTO customer (FirstName, LastName, PhoneNumber) values(" +
                    "'" + c.Firstname + "', '" + c.LastName + "', '" + c.PhoneNumber + "');";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        /*
        METHOD: InsertOrder
        DESCRIPTION:
	        This method does an insert statement into the database with an order. Order information is
            gathered from the Order properties passed in as a Order.
        PARAMETERS:
	        List<Order> ord: the list of orders/orderlines to insert as one order.
        RETURNS:
	        None.
        */
        public void InsertOrder(List<Order> ord)
        {
            MySqlCommand command = new MySqlCommand();
            MySqlDataReader reader;
            int orderId;
            List<string[]> readResult = new List<string[]>();

            try
            {
                connection.Open();
                command.Connection = connection;

                if (ord.Count > 0)
                {
                    command.CommandText = "INSERT INTO `order` (customerid, branchid, date, status) values(" +
                    "'" + ord[0].CustomerId + "', '" + ord[0].BranchId + "', '" + ord[0].Date.Year + "-" + ord[0].Date.Month + "-" + ord[0].Date.Day + "', '" + ord[0].Status + "')";
                    command.ExecuteNonQuery();

                    if (ord[0].Status != "RFND")
                    {
                        // get orderid from previously added order
                        command.CommandText = "SELECT COUNT(*) FROM `order`";
                        reader = command.ExecuteReader();
                        reader.Read();
                        orderId = reader.GetInt32(0);
                        reader.Close();

                        // use orderid in the orderline
                        foreach (Order o in ord)
                        {
                            // get productID by name
                            connection.Close(); // close the connection before calling GetRows because GetRows opens the connection itself
                            readResult = GetRows("id", "product where name='" + o.Product.Name + "'");
                            connection.Open();

                            command.CommandText = "INSERT INTO orderline (orderid, productid, quantity) values(" +
                            orderId + ", '" + readResult[0][0] + "', " + o.Product.Quantity + ")";
                            command.ExecuteNonQuery();
                        }
                    }
                }
                if (ord[0].Status == "PAID")
                {
                    // get the productID by searching the name in the database
                    foreach (Order o in ord)
                    {
                        // decrease the inventory levels by the number of products ordered
                        connection.Close();
                        readResult = GetRows("quantity", "product where name='" + o.Product.Name + "'");
                        connection.Open();

                        command.CommandText = "UPDATE product SET quantity=" + (Convert.ToInt32(readResult[0][0]) - o.Quantity) + " WHERE name='" + o.Product.Name + "'";
                        command.ExecuteNonQuery();
                    }
                }
                
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        public void InsertProduct(Product p)
        {
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "INSERT INTO product (`Name`, Price, Quantity, InventoryID, Discontinued) values(" +
                    "'" + p.Name + "', '" + p.Price + "', " + p.InventoryQuantity + ", " + p.InventoryID +", false);";
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        public void DiscontinueProduct(string productName, bool discontinue)
        {
            MySqlCommand command = new MySqlCommand();

            try
            {
                connection.Open();
                command.Connection = connection;

                if (discontinue)
                {
                    command.CommandText = "UPDATE Product SET Discontinued=true where `Name`='" + productName + "'";
                }
                else
                {
                    command.CommandText = "UPDATE Product SET Discontinued=false where `Name`='" + productName + "'";
                }
                command.ExecuteNonQuery();

                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (IndexOutOfRangeException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        /*
        METHOD: CancelOrder
        DESCRIPTION:
	        This method does an update statement to the database to update an order's status to cancelled and
            date to today's date.
        PARAMETERS:
	        Order o: the order to cancel.
        RETURNS:
	        None.
        */
        public void CancelOrder(Order o)
        {
            MySqlCommand command = new MySqlCommand();
            DateTime newDate = DateTime.Today;
            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "UPDATE `order` SET status='CNCL', date='" + newDate.Year + "-" + newDate.Month + "-" + newDate.Day + "' where id='" + o.Id + "'";
                command.ExecuteNonQuery();
                connection.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }



        /*
        METHOD: RefundOrder
        DESCRIPTION:
	        This method does an update statement to the database to update an order's status to refunded and
            date to today's date. The inventory levels are then adjusted.
        PARAMETERS:
	        Order o: the order to refund.
        RETURNS:
	        None.
        */
        public void RefundOrder(Order o)
        {
            MySqlCommand command = new MySqlCommand();
            DateTime newDate = DateTime.Today;
            List<string[]> readResult = new List<string[]>();
            try
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = "UPDATE `order` SET status='RFND', date='" + newDate.Year + "-" + newDate.Month + "-" + newDate.Day + "' where id='" + o.Id + "'";
                command.ExecuteNonQuery();

                // decrease the inventory levels by the number of products ordered
                connection.Close();
                readResult = GetRows("*", "orderline WHERE orderid='" + o.Id + "'");
                foreach (string[] line in readResult)
                {
                    string productId = line[2];
                    int productQuantity = Convert.ToInt32(line[3]);
                    List<string[]> readResult2 = GetRows("quantity", "product WHERE id='" + productId + "'");
                    int productInventory = Convert.ToInt32(readResult2[0][0]);

                    connection.Open();
                    command.CommandText = "UPDATE product SET quantity=" + (productInventory + productQuantity) + " WHERE id='" + productId + "'";
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
