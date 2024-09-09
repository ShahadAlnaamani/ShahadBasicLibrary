﻿using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.Common;
using System.Formats.Asn1;
using System.Globalization;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed)> Books = new List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed)>();

        //Borrow = 1 means book was taken out, 0 means returned 
        static List<(int CustomerID, DateTime BorrowedOn, int BookID, string BookName, string BookAuthor, int Borrow)> Invoices = new List<(int CustomerID, DateTime BorrowedOn, int BookID, string BookName, string BookAuthor, int Borrow)>();

        static List<(int AdminID, string AdminUserName, string AdminPswd, string AdminEmail)> Admins = new List<(int AdminID, string AdminUserName, string AdminPswd, string AdminEmail)>();
        static List<(int UserID, string UserUserName, string UserPswd, string UserEmail)> Users = new List<(int UserID, string UserUserName, string UserPswd, string UserEmail)>();
        static List<(string MasterUser, string MasterPswd)> Master = new List<(string MasterUser, string MasterPswd)>();

        //MasterAdmin
        static string MasterPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Master.txt";

        //Info saved -> BookTitle|Author|ID|Quantity|Borrowed
        static string BooksPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\BookRecords.txt";

        //Info saved -> ID|UserName|Password|Email
        static string AdminPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\AdminAccounts.txt";

        //Info saved -> ID|UserName|Password|Email
        static string UserPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\UserAccounts.txt";

        //Borrowed 1 means book was taken out, 0 means it was returned
        static string InvoicePath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Invoices.txt";

        static int CurrentUser = -1; //This is the users ID -1 means null


        static void Main(string[] args)
        {
            //Creates a master admin account 
            MasterAdmin();

            // empties list so that there are no repititions
            Admins.Clear();
            Users.Clear();
            Books.Clear();
            LoadBooksFromFile();
            LoadUsers();
            LoadAdmins();

            bool Authentication = false;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("\t\tMAIN MENU: ");
                Console.WriteLine(" 1. Reader Login");
                Console.WriteLine(" 2. Librarian Login");
                Console.WriteLine(" 3. Register");
                Console.WriteLine(" 4. Exit\n");
                int Option;
                Console.Write("Enter: ");

                while (!int.TryParse(Console.ReadLine(), out Option))
                { 
                    Console.WriteLine("Invalid option please enter a number greater than 0.");
                }
                

                switch (Option)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                        Console.Write("\n\t\tREADER LOGIN:\n\n");
                        Console.Write("Username: ");
                        string Usr = Console.ReadLine();
                        Console.Write("Password: ");
                        string Pswd = Console.ReadLine();
                        bool UsrAuth = ReaderLogin(Usr, Pswd);

                        if (UsrAuth)
                        {

                            for (int i = 0; i < Users.Count; i++)
                            {
                                if (Users[i].UserUserName == Usr)
                                {
                                    CurrentUser = Users[i].UserID;
                                }
                            }

                            UserPage();
                        }

                        else
                        { 
                            Console.WriteLine("Incorrect login details please try again :(");
                            Console.WriteLine("Press any key to try again.");
                            Console.ReadKey();
                        }
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                        Console.Write("\n\t\tLIBRARIAN LOGIN:\n\n");
                        Console.Write("Username: ");
                        string AdminUsr = Console.ReadLine();
                        Console.Write("Password: ");
                        string AdminPswd = Console.ReadLine();
                        bool AdminAuth = LibrarianLogin(AdminUsr, AdminPswd);

                        if (AdminAuth)
                        {

                            for (int i = 0; i < Admins.Count; i++)
                            {
                                if (Admins[i].AdminUserName == AdminUsr)
                                {
                                    CurrentUser = Admins[i].AdminID;
                                }
                            }

                            AdminPage();
                            LibrarianLogin(AdminUsr, AdminPswd);
                        }
                        else
                        {
                            Console.WriteLine("Incorrect login details please try again :(");
                            Console.WriteLine("Press any key to try again.");
                            Console.ReadKey();
                        }
                        
                        break;

                    case 3:
                        Register();
                        Console.Clear();
                        break;

                    case 4:
                        Authentication = true;
                        break;

                    default:
                        Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options.");
                        break;
                }
            } while (Authentication != true);
        }




        //- - - - - - - - - - - - - FUNCTIONS SHARED BETWEEN ADMIN AND USER - - - - - - - - - - - - - - - //

        //REGISTERS NEW USERS
        static void Register()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\t REGISTER:\n\n");
            Console.WriteLine("OPTIONS: ");
            Console.WriteLine(" 1. Reader");
            Console.WriteLine(" 2. Librarian");
            Console.WriteLine(" 3. Exit\n");
            int Identity = 0;
            Console.Write("Enter: ");

            while (!int.TryParse(Console.ReadLine(), out Identity))
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }

            switch (Identity)
            {
                case 1:
                    //User registration
                    Console.Clear();
                    Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                    Console.Write("\n\t\t USER REGISTRATION:\n\n");
                    Console.WriteLine("Welcome new reader!");
                    string UserPassword1 = " "; //This has one space
                    string UserPassword2 = "  "; //This has two spaces so that it doesn't affect do while loop condition below
                    string Email1 = " ";
                    string Email2 = "  ";

                    do
                    {
                        Console.Write("Email: ");
                        Email1 = Console.ReadLine();
                        Console.Write("Re-enter Email: ");
                        Email2 = Console.ReadLine();
                    } while (Email1 != Email2);

                    Console.Write("User Name: ");
                    string UserName = Console.ReadLine();

                    do
                    {
                        Console.Write("Password: ");
                        UserPassword1 = Console.ReadLine();
                        Console.Write("Re-enter Password: ");
                        UserPassword2 = Console.ReadLine();
                    } while (UserPassword1 != UserPassword2);

                    //Geneate ID
                    int UserID = Users.Count + 10;

                    Users.Add((UserID, UserName, UserPassword1, Email1));
                    SaveUsers();
                    break;

                case 2:
                    //Admin registration
                    Console.Clear();
                    Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                    Console.Write("\n\t\t LIBRARIAN REGISTRATION:\n\n");


                    //AUTHENTICATE MASTER ADMIN 
                    Console.Write("Master Username: ");
                    string Usr = Console.ReadLine();
                    Console.Write("Master Password: ");
                    string Pswd = Console.ReadLine();

                    bool Auth = CheckMaster(Usr, Pswd);

                    if (Auth != false)
                    {
                        Console.WriteLine("Welcome new librarian!");
                        string AdminPassword1 = " "; //This has one space
                        string AdminPassword2 = "  "; //This has two spaces so that it doesn't affect do while loop condition below
                        string AdminEmail1 = " ";
                        string AdminEmail2 = "  ";

                        do
                        {
                            Console.Write("Email: ");
                            AdminEmail1 = Console.ReadLine();
                            Console.Write("Re-enter Email: ");
                            AdminEmail2 = Console.ReadLine();
                        } while (AdminEmail1 != AdminEmail2);

                        Console.Write("User Name: ");
                        string AdminUserName = Console.ReadLine();

                        do
                        {
                            Console.Write("Password: ");
                            AdminPassword1 = Console.ReadLine();
                            Console.Write("Re-enter Password: ");
                            AdminPassword2 = Console.ReadLine();
                        } while (AdminPassword1 != AdminPassword2);

                        //Geneate id
                        int AdminID = Admins.Count + 10;

                        Admins.Add((AdminID, AdminUserName, AdminPassword1, AdminEmail1));
                        SaveAdmins();
                    }
                    else
                    { 
                        Console.WriteLine("The inputted credentials are incorrect, please try again :(");
                        Console.WriteLine("Press any key to try again. ");
                        Console.ReadKey();
                    }
                    break;

                case 3:
                    break;

                default:
                    Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options.");
                    break;
            }
        }


        //RETRIEVES BOOK DATA FROM FILE 
        static void LoadBooksFromFile()
        {
            try
            {
                if (File.Exists(BooksPath))
                {
                    using (StreamReader reader = new StreamReader(BooksPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 5)
                            {
                                Books.Add((parts[0], parts[1], int.Parse(parts[2]), int.Parse(parts[3]), int.Parse(parts[4])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading from file: {ex.Message}");
            }
        }


        //METHOD TO EXIT THE LIBRARY  
        static void LeaveLibrary(bool ExitFlag)
        {
            Console.WriteLine("\n\nAre you sure you want to leave? Yes or No.");
            Console.Write("Enter: ");
            string Leave = (Console.ReadLine()).ToLower();

            if (Leave != "no")
            {
                SaveBooksToFile();
                ExitFlag = true;
                Console.Clear();
                Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n\n");
                Console.WriteLine("Thank you for visiting the library :) \nCome again soon!");
            }
        }


        //DISPLAYS ALL BOOKS
        static void ViewAllBooks()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\tAVAILABLE BOOKS:\n\n");
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;
            
            if (Books != null)
            {
                for (int i = 0; i < Books.Count; i++)
                {
                    BookNumber = i + 1;
                    sb.Append("BOOK ").Append(BookNumber).Append(" ID: ").Append(Books[i].BookID);
                    sb.AppendLine();
                    sb.Append("BOOK ").Append(BookNumber).Append(" NAME: ").Append(Books[i].BookName);
                    sb.AppendLine();
                    sb.Append("BOOK ").Append(BookNumber).Append(" AUTHOR: ").Append(Books[i].BookAuthor);
                    sb.AppendLine();
                    sb.Append("BOOK ").Append(BookNumber).Append(" AVIALABLE QTY: ").Append(Books[i].BookQuantity);
                    sb.AppendLine().AppendLine();
                    Console.WriteLine(sb.ToString());
                    sb.Clear();

                }
            }
            else { Console.WriteLine("Sorry it looks like we don't have any books available :( \nPlease come again another time.\n"); }


        }


        //UPDATES DATA ON FILE 
        static void SaveBooksToFile()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(BooksPath))
                {
                    foreach (var book in Books)
                    {
                        writer.WriteLine($"{book.BookName}|{book.BookAuthor}|{book.BookID}|{book.BookQuantity}|{book.Borrowed}");
                    }
                }
                Console.WriteLine("Books saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }






        //- - - - - - - - - - - - - - - - - - - - - USER FUNCTIONS  - - - - - - - - - - - - - - - - - - -//

        //READER LOGIN
        static bool ReaderLogin(string Usr, string Pswd)
        {
            bool i = false;
            try
            {
                if (File.Exists(UserPath))
                {
                    using (StreamReader reader = new StreamReader(UserPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                if (Usr == parts[1] && parts[2] == Pswd)
                                {
                                    i = true;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admins from file: {ex.Message}");
            }
            return i;
        }


        //USER PAGE  
        static void UserPage()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("\t\t\tREADER OPTIONS:");
                Console.WriteLine(" 1. View All Books");
                Console.WriteLine(" 2. Borrow A Book");
                Console.WriteLine(" 3. Return A Book");
                Console.WriteLine(" 4. Log out\n");
                Console.Write("Enter: ");
                int choice;

                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid option please enter a number greater than 0.");
                }

                switch (choice)
                {
                    case 1:;
                        ViewAllBooks();
                        if (Books.Count != 0)
                        {
                            Console.WriteLine("Would you like to borrow a book? Yes or No.");
                            Console.Write("Enter: ");
                            string BorrowNow = Console.ReadLine().ToLower();

                            if (BorrowNow != "no")
                            {
                                BorrowBook();
                            }
                        }
                        break;

                    case 2:
                        BorrowBook();
                        break;

                    case 3:
                        ReturnBook();
                        break;

                    case 4:
                        SaveBooksToFile();
                        CurrentUser = -1;
                        LeaveLibrary(ExitFlag);
                        ExitFlag = true;    
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;

                }
                Console.WriteLine("Press any key to continue.");
                string cont = Console.ReadLine();
                Console.Clear();

            } while (ExitFlag != true);
        }


        //BORROW BOOK
        static void BorrowBook()
        {
            if (Books.Count != 0)
            {
                ViewAllBooks();

                Console.Write("\n\t\tBORROWING A BOOK:\n\n");
                Console.Write("Enter ID: ");
                int BorrowID;

                while (!int.TryParse(Console.ReadLine(), out BorrowID))
                {
                    Console.WriteLine("Invalid option please enter a number greater than 0.");
                }

                int Location = -1;

                for (int i = 0; i < Books.Count; i++)
                {
                    if (Books[i].BookID == BorrowID)
                    {
                        Location = i;
                        break;
                    }
                }

                if (Location != -1) //Book found
                {
                    Console.WriteLine($"Request to borrow: {Books[Location].BookName}");
                    if (Books[Location].BookQuantity > 0)
                    {
                        Console.WriteLine("We've got this in stock!");
                        Console.Write("Would you like to proceed? Yes or No: ");
                        string Checkout = Console.ReadLine().ToLower();

                        if (Checkout != "no")
                        {
                            //Decreasing book quantity 
                            int NewQuantity = (Books[Location].BookQuantity - 1);
                            int NewBorrowed = (Books[Location].Borrowed + 1);
                            Books[Location] = ((Books[Location].BookName, Books[Location].BookAuthor, Books[Location].BookID, Quantity: NewQuantity, Borrowed: NewBorrowed));
                            SaveBooksToFile();

                            Invoices.Add((CurrentUser, DateTime.Now, Books[Location].BookID, Books[Location].BookName, Books[Location].BookAuthor, 1));
                            SaveInvoice();

                            //Printing recipt 
                            DateTime Now = DateTime.Now;
                            Console.Clear();
                            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
                            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine("\t\t" + Now);
                            Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine($"BOOK: ID - {Books[Location].BookID} \nNAME - {Books[Location].BookName} \nAUTHOR - {Books[Location].BookAuthor}");
                            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n\n");
                            Console.WriteLine("\t\tThank you for visiting the library come again soon!");
                            Console.WriteLine("\t\tHappy Reading :)");
                            Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n ");

                            Console.WriteLine("Press any key to continue");
                            Console.ReadKey();

                            Reccomend(Books[Location].BookAuthor);

                            Console.WriteLine("\n\nWould you like to borrow another book? Yes or No.");
                            Console.Write("Enter: ");
                            string Response = Console.ReadLine().ToLower();

                            if (Response != "no") //Will repeat borrowing process
                            {
                                BorrowBook();
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Sorry this book is out of stock :(");
                        Console.WriteLine("We might have something else that you might like! \n\nWould you like to see what we have in stock? Yes or No");
                        string ViewOtherBooks = Console.ReadLine().ToLower();

                        if (ViewOtherBooks != "no")
                        {
                            ViewAllBooks();
                        }
                    }
                }

                else
                {
                    Console.WriteLine("Sorry we do not have this book :(");
                    Console.WriteLine("We might have something else that you might like! \n\nWould you like to see what we have in stock? Yes or No");
                    string ViewOtherBooks = Console.ReadLine().ToLower();

                    if (ViewOtherBooks != "no")
                    {
                        ViewAllBooks();
                    }
                }
            }
            else { Console.WriteLine("Sorry it looks like we don't have any books available right now :( \n"); }
        }


        //RETURN BOOK
        static void ReturnBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
            Console.Write("\n\t\tRETURN A BOOK:\n\n");
            Console.Write("Enter Book ID: ");
            int ReturnBook;

            while (!int.TryParse(Console.ReadLine(), out ReturnBook) || ReturnBook < 0)
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }


            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].BookID == ReturnBook)
                {
                    //Checking if this book has been borrowed -> handels case of books being returned without being borrowed (ie new books added)
                    if (Books[i].Borrowed > 0)
                    {
                        int NewBorrowCount = (Books[i].Borrowed - 1);
                        int NewBookQuantity = (Books[i].BookQuantity + 1);
                        Books[i] = ((Books[i].BookName, Books[i].BookAuthor, Books[i].BookID, BookQuantity: NewBookQuantity, Borrowed: NewBorrowCount));
                        Console.WriteLine($"Thank you for returning {Books[i].BookName} :) \nPress any key to print your recipt");
                        Console.ReadKey();
                        SaveBooksToFile();
                        Console.Clear();
                        ReturnRecipt(i);
                    }
                    else 
                    {
                        Console.WriteLine("This book has not been borrowed. \nPress any key to continue."); 
                        Console.ReadKey();

                    }
                    break;
                }
            }


        }


        //PRINT RETRUN RECIPT
        static void ReturnRecipt(int i)
        {
            DateTime Now = DateTime.Now;

            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n");
            Console.WriteLine("\t\t Returned: " + Now);
            Console.WriteLine("\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"BOOK: \nID - {Books[i].BookID} \nNAME - {Books[i].BookName} \nAUTHOR - {Books[i].BookAuthor}");
            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"Thank you for returning {Books[i].BookName} :)\n\n");
            Console.WriteLine("\t\tCome again soon!");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");

        }


        //SAVES USER INFO TO FILE
        static void SaveUsers()
        {
            try
            {//Info saved -> ID|UserName|Password|Email
                using (StreamWriter writer = new StreamWriter(UserPath))
                {
                    foreach (var user in Users)
                    {
                        writer.WriteLine($"{user.UserID}|{user.UserUserName}|{user.UserPswd}|{user.UserEmail}");
                    }
                }
                Console.WriteLine("User details saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }


        //READS USER INFO FROM FILE
        static void LoadUsers()
        {
            try
            {
                if (File.Exists(UserPath))
                {
                    using (StreamReader reader = new StreamReader(UserPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                Users.Add((int.Parse(parts[0]), parts[1], parts[2], parts[3]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading users from file: {ex.Message}");
            }
        }


        //RECORDS INVOICES ON FILE
        static void SaveInvoice()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(InvoicePath))
                {
                    foreach (var invoice in Invoices)
                    {
                        writer.WriteLine($"{invoice.CustomerID}|{invoice.BorrowedOn}|{invoice.BookID}|{invoice.BookName}|{invoice.BookAuthor}|{invoice.Borrow}");
                    }
                }
                Console.WriteLine("Invoices saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }


        //BOOK RECCOMENDATION GENERATOR
        static void Reccomend(string Author)
        { 
            //Book author find other books with book author and suggest 
            Console.Clear();
            Console.WriteLine("You might also like: ");
            for (int i = 0; i < Books.Count; i++) 
            { 
                if (Books[i].BookAuthor == Author) 
                {
                    Console.WriteLine($"Book name: {Books[i].BookName}");
                }
            }

            //Most popular book -> suggest 
        
        }





        //- - - - - - - - - - - - - - - - - - - - ADMIN FUNCTIONS  - - - - - - - - - - - - - - - - - - //
        //ADMIN PAGE  

        //CREATES MASTER FILE
        static void MasterAdmin()
        {
            if (!File.Exists(MasterPath))
            {
                File.Create(MasterPath).Close();
                MasterSetUp();
            }
        }


        //GETS MASTER LOGIN DETAILS - will only happen once because this function is only called if the file does not exist
        static void MasterSetUp() //Next modification make sure passwords aren't saved in plaintext file -- will be done when functionality finalized

        {
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.WriteLine("S E T T I N G    U P    M A S T E R    A D M I N\n");
            Console.WriteLine("WARNING: Please note that these details cannot be changed later make sure to remember them :)\n");
            Console.Write("Enter master username: ");
            string MasterUserName = Console.ReadLine();

            Console.Write("Enter master password: ");
            string MasterPassword;
            string Password1;
            string Password2;

            do
            {
                Console.Write("Password: ");
                Password1 = Console.ReadLine();
                Console.Write("Re-enter Password: ");
                Password2 = Console.ReadLine();
            } while (Password1 != Password2);

            MasterPassword = Password1;

            try
            {
                using (StreamWriter writer = new StreamWriter(MasterPath))
                {
                    writer.WriteLine($"{MasterUserName}|{MasterPassword}");
                }
                Console.WriteLine("Admin details saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }

        }


        //CHECKS MASTER CREDENTIALS 
        static bool CheckMaster(string Usr, string Pswd)
        {
            bool i = false;
            try
            {
                if (File.Exists(MasterPath))
                {
                    using (StreamReader reader = new StreamReader(MasterPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 2)
                            {
                                if (Usr == parts[0] && parts[1] == Pswd)
                                {
                                    i = true;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admins from file: {ex.Message}");
            }
            return i;
        }


        //ADMIN LOGIN
        static bool LibrarianLogin(string Usr, string Pswd)
        {
            bool i = false;
            try
            {
                if (File.Exists(AdminPath))
                {
                    using (StreamReader reader = new StreamReader(AdminPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                if (Usr == parts[1] && parts[2] == Pswd)
                                {
                                    i = true;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admins from file: {ex.Message}");
            }
            return i;
        }


        static void AdminPage()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("LIBRARIAN OPTIONS:");
                Console.WriteLine(" 1. Add New Book");
                Console.WriteLine(" 2. Display All Books");
                Console.WriteLine(" 3. Search for Book by Name");
                Console.WriteLine(" 4. Edit Book");
                Console.WriteLine(" 5. Delete Book");
                Console.WriteLine(" 6. Show Reports");
                Console.WriteLine(" 7. Save and Exit\n");
                Console.Write("Enter: ");
                int choice;

                while (!int.TryParse(Console.ReadLine(), out choice))
                {
                    Console.WriteLine("Invalid option please enter a number greater than 0.");
                }

                switch (choice)
                {

                    case 1:
                        AddNewBook();
                        break;

                    case 2:
                        ViewAllBooks();
                        break;

                    case 3:
                        SearchForBook();
                        break;

                    case 4:
                        EditBooks();
                        break;

                    case 5:
                        DeleteBook();
                        break;

                    case 6:
                        Reports();
                        break;

                    case 7:
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;

                }
                Console.WriteLine("Press any key to continue.");
                string cont = Console.ReadLine();
                Console.Clear();

            } while (ExitFlag != true);

        }


        //GETS BOOK INFORMATION FROM THE USER 
        static void AddNewBook() 
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\tADDING NEW BOOK:\n\n");
            Console.Write("Enter Book Name: ");
            string Name = Console.ReadLine().Trim(); //Trim added for more accurate search  

            Console.Write("Enter Book Author: ");
            string Author= Console.ReadLine().Trim();  

            Console.Write("Enter Book ID: ");
            int ID;
            while (!int.TryParse(Console.ReadLine(), out ID) || ID < 0)
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }

            Console.Write("Enter Book Quantity: ");
            int Qty;
            while (!int.TryParse(Console.ReadLine(), out Qty) || Qty < 0)
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }

            Console.WriteLine("\n");
            Books.Add(  (Name, Author, ID, Qty, 0 )  );
            SaveBooksToFile();
        }


        //ALLOWS USER TO SEARCH FOR BOOK
        static void SearchForBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\tSEARCH LIBRARY:\n\n");
            Console.Write("Book name: ");
            string name = (Console.ReadLine().Trim()).ToLower();  
            bool flag=false;

            for(int i = 0; i< Books.Count;i++)
            {
                if ((Books[i].BookName).ToLower() == name)
                {
                    Console.WriteLine($"Book Author: {Books[i].BookAuthor} \nID: {Books[i].BookID} \nAvailable Stock: {Books[i].BookQuantity}\n");
                    flag = true;
                    break;
                }
            }

            if (flag != true)
            { Console.WriteLine("Book not found :("); }
        }


        //ALLOWS LIBRARIAN TO EDIT BOOK INFO
        static void EditBooks()
        {
            Console.WriteLine("\n\t\tEDIT BOOKS:\n\n");
            Console.WriteLine(" 1. Edit Book Title");
            Console.WriteLine(" 2. Edit Author Name");
            Console.WriteLine(" 3. Add More Copies of Available Books");
            Console.WriteLine(" 4. Save and exit\n");
            Console.Write("Enter Option:");
            int Choice;

            while (!int.TryParse(Console.ReadLine(), out Choice))
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }

            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");

            switch (Choice)
            {
                //Editing book title
                case 1:
                    int Location = GetInformation();
                    if (Location != -1)
                    {
                        Console.WriteLine("\n\n\t\tEDIT BOOK TITLE:\n");
                        Console.Write("\nNew book name: ");
                        string NewBookName = Console.ReadLine();
                        Books[Location] = (BookName: NewBookName, Books[Location].BookAuthor, Books[Location].BookID, Books[Location].BookQuantity, Books[Location].Borrowed);
                        Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Location].BookName}  Author: {Books[Location].BookAuthor}  ID: {Books[Location].BookID}  x{Books[Location].BookQuantity}  Issues Borrowed: {Books[Location].Borrowed}\n ");
                        SaveBooksToFile();
                    }
                    
                    break;

                
                //Editing author name 
                case 2:
                    int Position = GetInformation();
                    if (Position != -1)
                    {
                        Console.WriteLine("\n\n\t\tEDIT AUTHOR NAME:\n");
                        Console.Write("\nNew author name: ");
                        string NewAuthName = Console.ReadLine();
                        Books[Position] = (Books[Position].BookName, BookAuthor: NewAuthName, Books[Position].BookID, Books[Position].BookQuantity, Books[Position].Borrowed);
                        Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Position].BookName}  Author: {Books[Position].BookAuthor}  ID: {Books[Position].BookID}  x{Books[Position].BookQuantity}  Issues Borrowed: {Books[Position].Borrowed}\n ");
                        SaveBooksToFile();
                    }
                    break;


                //Adding book copies 
                case 3:
                    int Index = GetInformation();
                    if (Index != -1)
                    {
                        Console.WriteLine("\n\n\t\tEDIT BOOK QUANTITY:\n");
                        Console.Write("\nHow many would you like to add: ");
                        int Add;

                        while (!int.TryParse(Console.ReadLine(), out Add))
                        {
                            Console.WriteLine("Invalid option please enter a number greater than 0.");
                        }

                        //Checking the positive number inputted so that books aren't minused 
                        if (Add > 0)
                        {
                            Add = Books[Index].BookQuantity + Add;
                            Books[Index] = (Books[Index].BookName, Books[Index].BookAuthor, Books[Index].BookID, BookQuantity: Add, Books[Index].Borrowed);
                            Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Index].BookName}  Author: {Books[Index].BookAuthor}  ID: {Books[Index].BookID}  x{Books[Index].BookQuantity}  Issues Borrowed: {Books[Index].Borrowed}\n ");
                            SaveBooksToFile();
                        }
                        else { Console.WriteLine("Improper input please input a number greater than 0 :("); }
                    }
                    break;


                case 4:
                    SaveBooksToFile();
                    break;
            }


        }


        //DELETE BOOKS 
        static void DeleteBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.WriteLine("\n\n\t\tDELETE A BOOK:\n");
            ViewAllBooks();

            int DeleteIndex = GetInformation(); 
            
            if (DeleteIndex !=-1) 
            {
                Console.WriteLine($"DELETING: Name: {Books[DeleteIndex].BookName}  Author: {Books[DeleteIndex].BookAuthor}  ID: {Books[DeleteIndex].BookID}  x{Books[DeleteIndex].BookQuantity}  Issues Borrowed: {Books[DeleteIndex].Borrowed} ");
                Console.WriteLine("To delete press X:");
                string Delete =  Console.ReadLine().ToLower();

                if (Delete != "x")
                { Console.WriteLine("The book was not deleted :)"); }
                else
                {
                    Books.Remove((Books[DeleteIndex].BookName, Books[DeleteIndex].BookAuthor, Books[DeleteIndex].BookID, Books[DeleteIndex].BookQuantity, Books[DeleteIndex].Borrowed));
                    Console.WriteLine("The book was deleted sucessfully :)");
                }
            }
        }


        //GETS THE INDEX OF GIVEN ID
        static public int GetInformation()
        {
            Console.Clear();
            ViewAllBooks();
            Console.Write("Enter ID: ");
            int ChangeID;

            while (!int.TryParse(Console.ReadLine(), out ChangeID))
            {
                Console.WriteLine("Invalid option please enter a number greater than 0.");
            }

            //Checking if book ID exists in library
            bool Found = false;
            for (int i = 0; i<=Books.Count; i ++)
            {
                if (Books[i].BookID == ChangeID)
                {
                    Found = true;
                    break;
                }
            }

            if (Found)
            {
                //Finding the index of given ID
                List<int> LocationList = new List<int>();

                for (int i = 0; i < Books.Count; i++)
                {
                    var (BookNames, BookAuthors, BookID, BookQuantity, Borrowed) = Books[i];
                    LocationList.Add(BookID);
                }

                if (LocationList.Contains(ChangeID))
                {
                    int Location = LocationList.IndexOf(ChangeID);
                    return (Location);
                }
                else { return -1; }
            }
            else { Console.WriteLine("ID does not exist :("); return -1; }
        }


        //SHOWS STATISTICS ON BORROWED AND AVAILABLE BOOKS 
        static public void Reports()
        {
            Console.Clear();

            //List available books 
            ViewAllBooks();

            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("\t\tREPORTS:\n");
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");



            //Breaking down tuple list so we can carry out calculations on info
            List<string> BookNames = new List<string>();
            List<string> BookAuthors = new List<string>();
            List<int> BookIDs = new List<int>();
            List<int> BookQuantities = new List<int>();
            List<int> BorrowedBooks = new List<int>();
            List<string> MostBorrowedAuth = new List<string>();
            List<string> LeastBorrowedAuth = new List<string>();
            List<int> ReaderIDs = new List<int>();
            List<int> TransactionType = new List<int>();

            for (int i = 0; i < Books.Count; i++)
            {
                var (BookName, BookAuthor, BookID, BookQuantity, Borrowed) = Books[i];
                BookNames.Add(BookName);
                BookAuthors.Add(BookAuthor);
                BookIDs.Add(BookID);
                BookQuantities.Add(BookQuantity);
                BorrowedBooks.Add(Borrowed);
            }

            //Total books borrowed
            int SumOfBorrowed = BorrowedBooks.Sum();
            Console.WriteLine("Number of Borrowed Books: " + SumOfBorrowed);


            //Total available books
            int SumOfAvailable = BookQuantities.Sum();
            Console.WriteLine("Number of Available Books: " + SumOfAvailable);


            //Most borrowed book
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("\n\n\tMOST BORROWED BOOK:\n");
            int MostBorrowedBook;

            MostBorrowedBook = BorrowedBooks.IndexOf(BorrowedBooks.Max());

            //To ensure that if more than one book have the maximum borrowed index they are included 
            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].Borrowed == BorrowedBooks[MostBorrowedBook])
                {
                    Console.WriteLine($"BOOK TITLE: {BookNames[i]} \nBOOK AUTHOR: {BookAuthors[i]} \nNUMBER OF COPIES BORROWED: {BorrowedBooks[i]}\n");
                    MostBorrowedAuth.Add(BookAuthors[i]);
                }
            }


            //Least borrowed book
            Console.WriteLine("\n\n\tLEAST BORROWED BOOK:\n");
            int LeastBorrowedBook;

            LeastBorrowedBook = BorrowedBooks.IndexOf(BorrowedBooks.Min());
            //To ensure that if more than one book have the minimum borrowed index they are included 
            for (int i = 0; i < Books.Count; i++)
            {
                if (Books[i].Borrowed == BorrowedBooks[LeastBorrowedBook])
                {
                    Console.WriteLine($"BOOK TITLE: {BookNames[i]} \nBOOK AUTHOR: {BookAuthors[i]} \nNUMBER OF COPIES BORROWED: {BorrowedBooks[i]}\n");
                    LeastBorrowedAuth.Add(BookAuthors[i]);
                }
            }

            //Most borrowed author
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("\n\n\tMOST POPULAR AUTHOR:\n");
            for (int i = 0;i < MostBorrowedAuth.Count;i++) 
            {
                Console.WriteLine(MostBorrowedAuth[i]);
            }

            //Least borrowed author
            Console.WriteLine("\n\n\tLEAST POPULAR AUTHOR:\n");
            for (int i = 0; i < LeastBorrowedAuth.Count; i++)
            {
                Console.WriteLine(LeastBorrowedAuth[i]);
            };
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            /*
            //Most active reader
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
            Console.WriteLine("\n\n\tMOST ACTIVE READER:\n");
            LoadInvoices();
            //CustomerID, DateTime BorrowedOn, int BookID, string BookName, string BookAuthor, int Borrow
            for (int i = 0; i < Invoices.Count; i++)
            {
                var(CustomerID, BorrowedOn, BookID, BookName,BookAuthor, Borrow) = Invoices[i];
                // BookNames.Add(BookName);
                ReaderIDs.Add(CustomerID);
                TransactionType.Add(Borrow); //Will be 1 if Borrow transaction 0 if return transaction
                
            }

            //Finding recurrences of each ID and choosing maximum
            int Occurances;
            for (int i = 0; i < ReaderIDs.Count; i++)
            {
                if (ReaderIDs.Contains())
                Occurances = ReaderIDs.Count(ReaderIDs[i]);
            }

            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
            */
        }


        //SAVES ADMIN INFO TO FILE
        static public void SaveAdmins()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(AdminPath))
                {
                    foreach (var admin in Admins)
                    {
                        writer.WriteLine($"{admin.AdminID}|{admin.AdminUserName}|{admin.AdminPswd}|{admin.AdminEmail}");
                    }
                }
                Console.WriteLine("Admin details saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }


        //READS ADMIN INFO FROM FILE
        static void LoadAdmins()
        {
            try
            {
                if (File.Exists(AdminPath))
                {
                    using (StreamReader reader = new StreamReader(AdminPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 4)
                            {
                                Admins.Add((int.Parse(parts[0]), parts[1], parts[2], parts[3]));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admins from file: {ex.Message}");
            }
        }


        //LOADS INVOICE RECORDS 
        static void LoadInvoices()
        {
            Invoices.Clear();
            try
            {
                if (File.Exists(InvoicePath))
                {
                    using (StreamReader reader = new StreamReader(InvoicePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 6)
                            {
                                Invoices.Add((int.Parse(parts[0]), DateTime.Parse(parts[1]), int.Parse(parts[2]), parts[3], parts[4], int.Parse(parts[5])));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading admins from file: {ex.Message}");
            }
        }
    }
}
