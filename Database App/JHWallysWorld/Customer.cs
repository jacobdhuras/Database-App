/*
* FILE:				Customer.cs
* PROJECT:			Relational Databases - Wally's World A4
* PROGRAMMER:		Jacob Huras
* FIRST VERSION:	12/05/2016
* DESCRIPTION:
*	This file contains the Customer class and all of its properties and methods.
*/


namespace JHWallysWorld
{
    /*
    * CLASS: Customer
    * DESCRIPTION:
    *	The Customer class simply contains properties belonging to a Customer (used for ObservalbeCollections).
    *   First name, last name, and customer phone number.
    */
    class Customer
    {
        private string firstname;
        private string lastName;
        private string phoneNumber;

        public Customer(string fname, string lname, string phone)
        {
            firstname = fname;
            LastName = lname;
            phoneNumber = phone;
        }

        public string Firstname
        {
            get {  return firstname; }
            set { firstname = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string PhoneNumber
        {
            get { return phoneNumber; }
            set { phoneNumber = value; }
        }
    }
}
