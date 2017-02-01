/*
* FILE:				MainWindow.xaml.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file handles all events actioned from the front-end user interface of the main application.
*	It initializes the interface as well as updates database values to the interface components.
*/



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;

namespace JHWallysWorld
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MySqlConnector myConnection;
        Cart cart;
        ObservableCollection<Product> inventoryList;
        ObservableCollection<Order> orderList;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMySQLValues();
        }



        /*
        METHOD: InitializeMySQLValues
        DESCRIPTION:
	        This method initializes interface values gathered from MySql calls to the Connector.
        PARAMETERS:
	        None.
        RETURNS:
	        None.
        */
        private void InitializeMySQLValues()
        {
            cart = new Cart();
            cart.ShoppingList = new List<Product>();
            
            // Make initial connection to database through connector
            myConnection = new MySqlConnector();
            myConnection.Connect("server=localhost;uid=root;pwd=Conestoga1;database=JHWally");
            List<string[]> branchListString = new List<string[]>();
            List<string[]> productListString = new List<string[]>();
            orderList = new ObservableCollection<Order>();
            inventoryList = new ObservableCollection<Product>();


            // Select logic
            // Populate branches
            branchListString = myConnection.GetRows("branchname", "branch");
            foreach (string[] branch in branchListString)
            {
                branchComboBox.Items.Add(branch[0]);
            }
            branchComboBox.SelectedIndex = 0;

            // Populate customers
            InvalidateCustomerList();
            customerComboBox.SelectedIndex = 0;

            // Populate product list
            InvalidateProductList();
            productComboBox.SelectedIndex = 0;

            // Populate inventory list
            InvalidateInventoryList();
            inventoryListView.ItemsSource = inventoryList; // binded

            // Populate order list
            InvalidateOrderList("", "");
            orderListView.ItemsSource = orderList; // binded

            //Populate order search combo box
            orderSearchComboBox.Items.Add("First Name");
            orderSearchComboBox.Items.Add("Last Name");
            orderSearchComboBox.Items.Add("Order ID");
            orderSearchComboBox.Items.Add("Status");
            orderSearchComboBox.SelectedIndex = 0;
        }



        /*
        METHOD: TryCreateButtonEnable
        DESCRIPTION:
	        This method trys to enable the create button by validating the text boxes (check if null).
        PARAMETERS:
	        None.
        RETURNS:
	        None.
        */
        private void TryCreateButtonEnable()
        {
            if (lastNameTextBox.Text != "" &&
                firstNameTextBox.Text != "" &&
                phoneNumberTextBox.Text.Length == 10)
            {
                createButton.IsEnabled = true;
            }
            else
            {
                createButton.IsEnabled = false;
            }
        }



        /*
        METHOD: InvalidateCustomerList
        DESCRIPTION:
	        This method forces an update to the customerListView by gathering the data from the database all over again.
        PARAMETERS:
	        None.
        RETURNS:
	        None.
        */
        private void InvalidateCustomerList()
        {
            customerComboBox.Items.Clear(); // Remove all current items, add them all in again (including any new ones)

            List<string[]> customerListString = myConnection.GetRows("firstname, lastname", "customer");
            foreach (string[] customer in customerListString)
            {
                customerComboBox.Items.Add(customer[1] + " " + customer[0]);
            }
        }



        private void InvalidateProductList()
        {
            productComboBox.Items.Clear(); // Remove all current items, add them all in again (including any new ones)

            List<string[]> branchID = myConnection.GetRows("ID", "Branch where BranchName='" + branchComboBox.SelectedItem.ToString() + "'");
            List<string[]> inventoryID = myConnection.GetRows("ID", "Inventory where BranchID=" + branchID[0][0] + "");
            List<string[]> productListString = myConnection.GetRows("`Name`", "product where InventoryID=" + inventoryID[0][0]);
            foreach (string[] productName in productListString)
            {
                productComboBox.Items.Add(productName[0]);
            }
        }



        /*
        METHOD: InvalidateInventoryList
        DESCRIPTION:
	        This method forces an update to the inventoryListView by gathering the data from the database all over again.
        PARAMETERS:
	        None.
        RETURNS:
	        None.
        */
        private void InvalidateInventoryList()
        {
            //inventoryListView.Items.Clear();
            inventoryList.Clear();
            List<string[]> branchID = myConnection.GetRows("ID", "Branch where BranchName='" + branchComboBox.SelectedItem.ToString() + "'");
            List<string[]> inventoryID = myConnection.GetRows("ID", "Inventory where BranchID=" + branchID[0][0] + "");
            List<string[]> inventoryListString = myConnection.GetRows("*", "product where InventoryID=" + inventoryID[0][0]);
            foreach (string[] item in inventoryListString)
            {
                // Create a new Product to store into the inventoryList
                Product prod = new Product();
                prod.Id = Convert.ToInt32(item[0]);
                prod.Name = item[2];
                prod.Price = Convert.ToDouble(item[3]);
                prod.InventoryQuantity = Convert.ToInt32(item[4]);
                prod.Discontinued = Convert.ToBoolean(item[5]);
                inventoryList.Add(prod);
            }
        }



        /*
        METHOD: InvalidateOrderList
        DESCRIPTION:
	        This method forces an update to the orderListView by gathering the data from the database all over again.
            The caller can also specify attributes and search terms to narrow their search. Otherwise, it will display all orders.
        PARAMETERS:
	        string attrbiute: the attribute of the customer corresponding to the search term.
        RETURNS:
	        string searchTerm: the search term the user wishes to search for.
        */
        private void InvalidateOrderList(string attribute, string searchTerm)
        {
            orderList.Clear();
            List<string[]> orderListString = new List<string[]>();
            if (searchTerm == "")
            {
                orderListString = myConnection.GetRows("*", "`order`");
            }
            else
            {
                if ((attribute == "firstname") || (attribute == "lastname")) // searching for first/last name only
                {
                    List<string[]> customerIDResult = myConnection.GetRows("id", "customer WHERE " + attribute + " LIKE '" + searchTerm + "%'");
                    if (customerIDResult.Count != 0)
                    {
                        orderListString = myConnection.GetRows("*", "`order` WHERE customerid='" + customerIDResult[0][0] + "'");
                    }
                }
                else // searching for something other than first/last name
                {
                    orderListString = myConnection.GetRows("*", "`order` WHERE " + attribute + " LIKE '" + searchTerm + "%'");
                }
            }


            foreach (string[] item in orderListString)
            {
                // Create a new Order to store into the orderList
                Order ord = new Order();
                ord.Id = Convert.ToInt32(item[0]);
                ord.CustomerId = Convert.ToInt32(item[1]);
                ord.BranchId = Convert.ToInt32(item[2]);
                ord.Date = Convert.ToDateTime(item[3]);
                ord.Status = item[4];

                orderListString = myConnection.GetRows("firstname, lastname", "customer WHERE id='" + ord.CustomerId + "'");
                ord.CustomerFullName = orderListString[0][0] + " " + orderListString[0][1];

                orderListString = myConnection.GetRows("branchname", "branch WHERE id='" + ord.BranchId + "'");
                ord.BranchName = orderListString[0][0];

                orderList.Add(ord);
            }
        }



        /*
        METHOD: showOrderDetails
        DESCRIPTION:
	        This shows the dialog of the popup window with order information on it.
        PARAMETERS:
	        None.
        RETURNS:
	        None.
        */
        private void showOrderDetails(bool newRecord)
        {
            Order selectedOrder;

            if (newRecord)
            {
                orderListView.SelectedIndex = orderListView.Items.Count - 1;
            }

            selectedOrder = (Order)orderListView.SelectedItem;
            string message = "";
            double subtotal = 0;
            double hstTotal = 0;
            int retCode = 0;

            // Building the message of the popup window...
            List<string[]> branchIdResult = myConnection.GetRows("branchname", "branch where id='" + selectedOrder.BranchId + "'");

            message += "Thank you for shopping at Wally’s World " + branchIdResult[0][0] + " on " + selectedOrder.Date + ", " + selectedOrder.CustomerFullName + "\n";
            message += "Order ID: " + selectedOrder.Id + "\n";

            List<string[]> orderResults = myConnection.GetRows("*", "orderline where orderid='" + selectedOrder.Id + "'");
            foreach (string[] line in orderResults)
            {
                List<string[]> selectResults = myConnection.GetRows("*", "product where id='" + line[2] + "'");
                int quantity = Convert.ToInt32(line[3]);
                double eaPrice = Convert.ToDouble(selectResults[0][3]);
                double totalPrice = quantity * eaPrice;
                subtotal += totalPrice;

                message += selectResults[0][2] + " " + quantity + " x $" + eaPrice + " = $" + totalPrice + "\n";
            }

            hstTotal = Math.Round(subtotal * 0.13, 2);

            message += "Subtotal:   $" + subtotal.ToString("0.00") + "\n";
            message += "HST (13%):  $" + hstTotal.ToString("0.00") + "\n";
            message += "Sale Total: $" + (subtotal + hstTotal).ToString("0.00") + "\n";

            // Instantiating a window popup with the message and status of the order
            OrderDetailsWindow w = new OrderDetailsWindow(message, selectedOrder.Status);
            if (w.ShowDialog() == false)
            {
                retCode = w.ReturnCode;
                switch (retCode)
                {
                    case 1:
                        // cancel logic
                        if (selectedOrder.Status == "PEND")
                        {
                            myConnection.CancelOrder(selectedOrder);
                            InvalidateOrderList("", "");
                            InvalidateInventoryList();
                        }
                        break;
                    case 2:
                        // refund logic
                        if (selectedOrder.Status == "PAID")
                        {
                            myConnection.RefundOrder(selectedOrder);
                            InvalidateOrderList("", "");
                            InvalidateInventoryList();
                        }
                        break;
                    default:
                        break;
                }
            }
        }



        /*
        METHOD: quantityTextBox_PreviewTextInput
        DESCRIPTION:
	        This method is an event handler for making sure that the user can only enter digits into the quantity text field.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void quantityTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }



        /*
        METHOD: quantityTextBox_TextChanged
        DESCRIPTION:
	        This method is an event handler for making sure that the user can only enter 4 digits into the quantity text field.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void quantityTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (quantityTextBox.Text.Length > 4)
            {
                quantityTextBox.Text = quantityTextBox.Text.Remove(4);
                quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                quantityTextBox.SelectionLength = 0;
            }
        }



        /*
        METHOD: productComboBox_SelectionChanged
        DESCRIPTION:
	        This method is an event handler for dynamically changing the price text field as the user changes their product.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void productComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            quantityTextBox.Text = "";
            if (productComboBox.SelectedItem != null)
            {
                List<string[]> temp = new List<string[]>();
                temp = myConnection.GetRows("price", "product where name='" + productComboBox.SelectedItem.ToString() + "'");
                priceTextBox.Text = temp[0][0];
            }
            else
            {
                priceTextBox.Text = "";
            }
        }



        /*
        METHOD: firstNameTextBox_TextChanged
        DESCRIPTION:
	        This method is an event handler for the firstNameTextBox changing; it trys to enable the createButton.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void firstNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TryCreateButtonEnable();
        }



        /*
        METHOD: lastNameTextBox_TextChanged
        DESCRIPTION:
	        This method is an event handler for the lastNameTextBox changing; it trys to enable the createButton.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void lastNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TryCreateButtonEnable();
        }



        /*
        METHOD: phoneNumberTextBox_TextChanged
        DESCRIPTION:
	        This method is an event handler for the phoneNumberNameTextBox changing; it trys to enable the createButton.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void phoneNumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TryCreateButtonEnable();
            if (quantityTextBox.Text.Length > 10)
            {
                quantityTextBox.Text = quantityTextBox.Text.Remove(4);
                quantityTextBox.SelectionStart = quantityTextBox.Text.Length;
                quantityTextBox.SelectionLength = 0;
            }
        }



        /*
        METHOD: phoneNumberTextBox_PreviewTextInput
        DESCRIPTION:
	        This method is an event handler for making sure that the user can only type digits into the phone number field.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void phoneNumberTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }



        /*
        METHOD: createButton_Click
        DESCRIPTION:
	        This method is an event handler for creating a new customer. Inserts a new customer into the database and
            invalidates the customerList.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            myConnection.InsertCustomer(new Customer(firstNameTextBox.Text, lastNameTextBox.Text, phoneNumberTextBox.Text));

            firstNameTextBox.Text = "";
            lastNameTextBox.Text = "";
            phoneNumberTextBox.Text = "";
            InvalidateCustomerList();
            customerComboBox.SelectedIndex = customerComboBox.Items.Count - 1;
        }



        /*
        METHOD: checkBox_Checked
        DESCRIPTION:
	        This method is an event handler for enabling the view for the employee-only views.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void checkBox_Checked(object sender, RoutedEventArgs e)
        {
            inventoryListView.Visibility = Visibility.Visible;
            orderListView.Visibility = Visibility.Visible;
            checkBox2.IsChecked = true;
        }


        /*
        METHOD: checkBox_Unchecked
        DESCRIPTION:
	        This method is an event handler for disabling the view for the employee-only views.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void checkBox_Unchecked(object sender, RoutedEventArgs e)
        {
            inventoryListView.Visibility = Visibility.Hidden;
            orderListView.Visibility = Visibility.Hidden;
            checkBox2.IsChecked = false;
        }



        /*
        METHOD: checkBox2_Checked
        DESCRIPTION:
	        This method is an event handler for enabling the view for the employee-only views.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            inventoryListView.Visibility = Visibility.Visible;
            orderListView.Visibility = Visibility.Visible;
            checkBox.IsChecked = true;
        }



        /*
        METHOD: checkBox2_Unchecked
        DESCRIPTION:
	        This method is an event handler for disabling the view for the employee-only views.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void checkBox2_Unchecked(object sender, RoutedEventArgs e)
        {
            inventoryListView.Visibility = Visibility.Hidden;
            orderListView.Visibility = Visibility.Hidden;
            checkBox.IsChecked = false;
        }



        /*
        METHOD: addItemButton_Click
        DESCRIPTION:
	        This method is an event handler for adding an item to the cart.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void addItemButton_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> branchID = myConnection.GetRows("ID", "Branch where BranchName='" + branchComboBox.SelectedItem.ToString() + "'");
            List<string[]> inventoryID = myConnection.GetRows("ID", "Inventory where BranchID=" + branchID[0][0] + "");
            List<string[]> productListString = myConnection.GetRows("*", "product where name='" + productComboBox.SelectedItem.ToString() + "' and InventoryID=" + inventoryID[0][0]);

            foreach (string[] item in productListString)
            {
                Product prod = new Product();
                prod.Id = Convert.ToInt32(item[0]);
                prod.Name = item[2];
                prod.Price = Convert.ToDouble(item[3]);
                prod.Quantity = Convert.ToInt32(quantityTextBox.Text);
                cart.ShoppingList.Add(prod);
            }
            cartListView.Items.Clear();
            for (int i = 0; i < cart.ShoppingList.Count; i++)
            {
                cartListView.Items.Add(cart.ShoppingList[i]);
            }
            
        }



        /*
        METHOD: placeOrderButton_Click
        DESCRIPTION:
	        This method is an event handler for placing the current cart as an order. It iterates through
            all the items in the cart and puts them into a list of orders. The orders' ID will still be the same
            as eachother because it technically as one whole order, as well as the status. Everything else in this
            list of orders is unique.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void placeOrderButton_Click(object sender, RoutedEventArgs e)
        {
            List<Order> orderList = new List<Order>();
            string[] custname;
            bool totalOrderStatus = true;
            int toCompare = 0;

            

            foreach (Product p in cart.ShoppingList)
            {
                List<string[]> selectResults = myConnection.GetRows("quantity", "product where name='" + p.Name + "'");

                Order ord = new Order();

                selectResults = myConnection.GetRows("id", "branch where branchname='" + branchComboBox.Text + "'");
                ord.BranchId = Convert.ToInt32(selectResults[0][0]);

                custname = customerComboBox.Text.Split(' ');
                selectResults = myConnection.GetRows("id", "customer where firstname='" + custname[1] + "' and lastname='" + custname[0] + "'");
                ord.CustomerId = Convert.ToInt32(selectResults[0][0]);

                ord.Product = p;

                ord.Date = DateTime.Today;

                ord.Quantity = p.Quantity;

                // try to match current product name to current inventory product name to find out current inventory levels (bad design)
                for (int i = 0; i < inventoryList.Count; i++)
                {
                    if (inventoryList[i].Name == p.Name)
                    {
                        toCompare = i;
                        break;
                    }
                }

                if (ord.Quantity <= inventoryList[toCompare].InventoryQuantity)
                {
                    // inventory level is sufficient, set one order to paid (if all orders in order list are paid, order will go through)
                    ord.Status = "PAID";
                }
                else
                {
                    ord.Status = "PEND";
                }
                orderList.Add(ord);
            }

            foreach (Order ord in orderList)
            {
                // if just one order in the order list isn't paid, the whole order will be pending
                if (ord.Status == "PEND")
                {
                    totalOrderStatus = false;
                    break;
                }
            }

            if (totalOrderStatus)
            {
                // whole order is paid and ready to go; removing stock is done in the InsertOrder method
                orderList[0].Status = "PAID";
            }
            else
            {
                orderList[0].Status = "PEND";
            }

            // create the order in the database
            myConnection.InsertOrder(orderList);

            cart.ShoppingList.Clear();
            cartListView.Items.Clear();
            InvalidateInventoryList();
            InvalidateOrderList("","");
            showOrderDetails(true);
        }



        /*
        METHOD: orderListView_MouseDoubleClick
        DESCRIPTION:
	        This method is an event handler for double clicking on an order in the order list.
            It shows the order details as the popup window.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void orderListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            showOrderDetails(false);
        }



        /*
        METHOD: firstNameTextBox_PreviewKeyDown
        DESCRIPTION:
	        This method is an event handler for disallowing the typing of a space in the first name
            field.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void firstNameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }



        /*
        METHOD: lastNameTextBox_PreviewKeyDown
        DESCRIPTION:
	        This method is an event handler for disallowing the typing of a space in the last name
            field.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void lastNameTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }



        /*
        METHOD: orderSearchTextBox_TextChanged
        DESCRIPTION:
	        This method is an event handler for dynamically udpating the search field as the user types
            in text.
        PARAMETERS:
	        N/A
        RETURNS:
	        None.
        */
        private void orderSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string attribute = "";
            switch ((string)orderSearchComboBox.SelectedValue)
            {
                case "First Name":
                    attribute = "firstname";
                    break;
                case "Last Name":
                    attribute = "lastname";
                    break;
                case "Phone Number":
                    attribute = "phonenumber";
                    break;
                case "Order ID":
                    attribute = "id";
                    break;
                case "Status":
                    attribute = "status";
                    break;
                default:
                    break;
            }
            InvalidateOrderList(attribute, orderSearchTextBox.Text);
        }

        private void customerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (customerComboBox.Items.Count != 0)
            {
                string[] fullname = customerComboBox.SelectedValue.ToString().Split(' ');
                List<string[]> phoneNumberResults = myConnection.GetRows("phonenumber", "customer where lastname='" + fullname[0] + "' and firstname='" + fullname[1] + "'");
                customerInfoTextBox.Text = "Phone number: " + phoneNumberResults[0][0];
            }
        }

        private void branchComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InvalidateInventoryList();
        }

        private void productPriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!(char.IsDigit(e.Text, e.Text.Length - 1) || e.Text[e.Text.Length-1] == '.'))
            {
                e.Handled = true;
            }
        }

        private void productCreationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (productNameTextBox.Text != "" && productPriceTextBox.Text != "" && productQuantityTextBox.Text != "")
            {
                createProductButton.IsEnabled = true;
            }
            else
            {
                createProductButton.IsEnabled = false;
            }
        }

        private void createProductButton_Click(object sender, RoutedEventArgs e)
        {
            List<string[]> branchID = myConnection.GetRows("ID", "Branch where BranchName='" + branchComboBox.SelectedItem.ToString() + "'");
            List<string[]> inventoryID = myConnection.GetRows("ID", "Inventory where BranchID=" + branchID[0][0] + "");
            List<string[]> productListString = myConnection.GetRows("*", "product where InventoryID=" + inventoryID[0][0]);

            Product newProd = new Product();
            newProd.Name = productNameTextBox.Text;
            newProd.Price = Convert.ToDouble(productPriceTextBox.Text);
            newProd.InventoryQuantity = Convert.ToInt32(productQuantityTextBox.Text);
            newProd.InventoryID = Convert.ToInt32(inventoryID[0][0]);

            myConnection.InsertProduct(newProd);

            productNameTextBox.Text = "";
            productPriceTextBox.Text = "";
            productQuantityTextBox.Text = "";
            InvalidateInventoryList();
            InvalidateProductList();
        }

        private void inventoryListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Product selectedProd = (Product)inventoryListView.SelectedItem;
            if (selectedProd.Discontinued)
            {
                myConnection.DiscontinueProduct(selectedProd.Name, false);
            }
            else
            {
                myConnection.DiscontinueProduct(selectedProd.Name, true);
            }
            
            InvalidateInventoryList();
        }
    }
}
