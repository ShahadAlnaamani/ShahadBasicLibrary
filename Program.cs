using Microsoft.VisualBasic.FileIO;
using System;
using System.Data;
using System.Data.Common;
using System.Formats.Asn1;
using System.Globalization;
using System.Net;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(int BookID, string BookName, string BookAuthor, int BookQuantity, int Borrowed, float Price, string Category, int BorrowPeriod)> Books = new List<(int BookID, string BookName, string BookAuthor, int BookQuantity, int Borrowed, float Price, string Category, int BorrowPeriod)>();

        //Borrow = 1 means book was taken out, 0 means returned 
        static List<(int CustomerID, DateTime BorrowedOn, int BookID, string BookName, string BookAuthor, int Borrow)> Invoices = new List<(int CustomerID, DateTime BorrowedOn, int BookID, string BookName, string BookAuthor, int Borrow)>();

        static List<(int AdminID, string AdminUserName, string AdminEmail, string AdminPswd)> Admins = new List<(int AdminID, string AdminUserName, string AdminEmail, string AdminPswd)>();
        static List<(int UserID, string UserUserName, string UserEmail, string UserPswd)> Users = new List<(int UserID, string UserUserName, string UserEmail, string UserPswd)>();
        static List<(string MasterUser, string MasterPswd)> Master = new List<(string MasterUser, string MasterPswd)>();
        static List<(int CategoryID, string CategoryName, int NoOfBooks)> Categories = new List<(int CategoryID, string CategoryName, int NoOfBooks)>();
        static List<(int UserID, int BookID, DateTime BorrowedOn, DateTime ReturnBy, DateTime ActualReturn, float Rating, bool IsReturned)> Borrowing = new List<(int UserID, int BorrowID, DateTime BorrowedOn, DateTime ReturnBy, DateTime ActualReturn, float Rating, bool IsReturned)>();

        //MasterAdmin
        static string MasterPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Master.txt";

        //Info saved -> BookTitle|Author|ID|Quantity|Borrowed
        static string BooksPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\BookRecords.txt";

        //Info saved -> ID|UserName|Email|Password
        static string AdminPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\AdminAccounts.txt";

        //Info saved -> ID|UserName|Email|Password
        static string UserPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\UserAccounts.txt";

        //Borrowed 1 means book was taken out, 0 means it was returned
        static string InvoicePath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Invoices.txt";

        //Info saved -> Category ID|CategoryName|NumberOfBooks
        static string CategoriesPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Categories.txt";

        //Info saved -> UserID|BorrowID|BorrowDate|RetrunByDate|ActualReturnDate|Rating|IsReturned
        static string BorrowingPath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\Borrowing.txt";



        static int CurrentUser = -1; //This is the users ID -1 means null


        static void Main(string[] args)
        {
            //Creates a master admin account 
            MasterAdmin();

            // empties list so that there are no repititions
            Admins.Clear();
            Users.Clear();
            Books.Clear();
            Categories.Clear();
            Borrowing.Clear();
            LoadBooksFromFile();
            LoadUsers();
            LoadAdmins();
            LoadCategoriesFromFile();
            LoadBorrowsFromFile();

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
                int Option = 0;
                Console.Write("Enter: ");

                try
                {
                    Option = int.Parse(Console.ReadLine());
                }catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("\n Press enter to continue :(");  Console.ReadKey(); }

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
                            Console.Clear();
                            UserPage();
                        }

                        else
                        { 
                            Console.WriteLine("Incorrect login details please try again :(");
                            Console.WriteLine("Press enter to try again.");
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
                        }
                        else
                        {
                            Console.WriteLine("Incorrect login details please try again :(");
                            Console.WriteLine("Press enter to try again.");
                            Console.ReadKey();
                        }
                        
                        break;

                    case 3:
                        Console.Clear();
                        Register();
                        Console.Clear();
                        break;

                    case 4:
                        Authentication = true;
                        break;

                    default:
                        Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options.");
                        Console.WriteLine("\nPress enter to continue"); Console.ReadKey();
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

            try 
            {
                Identity = int.Parse(Console.ReadLine());

            } catch (Exception ex) { Console.WriteLine(ex.Message); }

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

                    Users.Add((UserID, UserName, Email1, UserPassword1));
                    Console.WriteLine("User created :) \nPress enter to continue!");
                    Console.ReadKey();
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

                        
                        Admins.Add((AdminID, AdminUserName, AdminEmail1, AdminPassword1));
                        SaveAdmins();
                        Console.WriteLine("Admin created :) \nPress enter to continue!");
                        Console.ReadKey();
                    }
                    else
                    { 
                        Console.WriteLine("The inputted credentials are incorrect, please try again :(");
                        Console.WriteLine("Press enter to try again. ");
                        Console.ReadKey();
                    }
                    break;

                case 3:
                    break;

                default:
                    Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options."); 
                   // Console.WriteLine("\nPress enter to try again. ");
                    Console.ReadKey();
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
                            if (parts.Length == 8)
                            {
                                Books.Add((int.Parse(parts[0]), parts[1], parts[2], int.Parse(parts[3]), int.Parse(parts[4]), float.Parse(parts[5]), parts[6], int.Parse(parts[7])));
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
        static bool LeaveLibrary(bool ExitFlag)
        {
            Console.WriteLine("\n\nAre you sure you want to leave? \nYes to leave anything else to stay.");
            Console.Write("Enter: ");
            string Leave = (Console.ReadLine()).ToLower();

            if (Leave != "yes")
            {
                return false;
            }
            else 
            {
                SaveBooksToFile();
                CurrentUser = -1;
                ExitFlag = true;
                Console.Clear();
                Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n\n");
                Console.WriteLine("Thank you for visiting the library :) \nCome again soon!\n\n");
                return true;
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
                    sb.Append("BOOK ").Append(BookNumber).Append(" CATEGORY: ").Append(Books[i].Category);
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
                        writer.WriteLine($"{book.BookID}|{book.BookName}|{book.BookAuthor}|{book.BookQuantity}|{book.Borrowed}|{book.Price}|{book.Category}|{book.BorrowPeriod}");
                    }
                }
                Console.WriteLine("Books saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }


        //LOADING CATEGORY INFORMATION FROM FILE
        static void LoadCategoriesFromFile()
        {
            try
            {
                if (File.Exists(CategoriesPath))
                {
                    using (StreamReader reader = new StreamReader(CategoriesPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 3)
                            {
                                Categories.Add((int.Parse(parts[0]), parts[1], int.Parse(parts[2])));
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



        //SAVING CATEGORY INFORMATION TO FILE 
        static void SaveCategories()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(CategoriesPath))
                {
                    foreach (var category in Categories)
                    {
                        writer.WriteLine($"{category.CategoryID}|{category.CategoryName}|{category.NoOfBooks}");
                    }
                }
                Console.WriteLine("Categories saved to file successfully! :)");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }


        //LOADING CATEGORY INFORMATION FROM FILE
        static void LoadBorrowsFromFile()
        {
            try
            {
                if (File.Exists(BorrowingPath))
                {
                    using (StreamReader reader = new StreamReader(BorrowingPath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var parts = line.Split('|');
                            if (parts.Length == 7)
                            {
                                Borrowing.Add((int.Parse(parts[0]), int.Parse(parts[1]), DateTime.Parse(parts[2]), DateTime.Parse(parts[3]), DateTime.Parse(parts[4]), int.Parse(parts[5]), bool.Parse(parts[6])));
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



        //SAVING BORROWING INFORMATION TO FILE 
        static void SaveBorrowInfo()
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(BorrowingPath))
                {
                    foreach (var borrow in Borrowing)
                    {
                        writer.WriteLine($"{borrow.UserID}|{borrow.BookID}|{borrow.BorrowedOn}|{borrow.ReturnBy}|{borrow.ActualReturn}|{borrow.Rating}|{borrow.IsReturned}");
                    }
                }
                Console.WriteLine("Borrows saved to file successfully! :)");
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
                            var parts = line.Split(" | ");
                            if (parts.Length == 4)
                            {
                                if (Usr == parts[1] && parts[3] == Pswd)
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
                Console.WriteLine(" 2. Search For A Book");
                Console.WriteLine(" 3. View Profile");
                Console.WriteLine(" 4. Borrow A Book");
                Console.WriteLine(" 5. Return A Book");
                Console.WriteLine(" 6. Log out\n");
                Console.Write("Enter: ");
                int choice =0;

                try
                {
                    choice = int.Parse(Console.ReadLine());
                }catch(Exception ex) { Console.WriteLine(ex.Message); }

                switch (choice)
                {
                    case 1:;
                        Console.Clear();
                        ViewAllBooks();
                        if (Books.Count != 0)
                        {
                            Console.WriteLine("Would you like to borrow a book? \n\nYes to continue anything else to leave.");
                            Console.Write("Enter: ");
                            string BorrowNow = Console.ReadLine().ToLower();

                            if (BorrowNow != "yes")
                            {
                                Console.WriteLine("Exiting...");
                                break;
                            }

                            else;
                            { 
                                BorrowBook(); 
                            }

                        }
                        break;

                    case 2:
                        Console.Clear();
                        UserSearchForBook();
                        break;

                    case 3:
                        Console.Clear();
                        ViewUsrProfile();
                        break;

                    case 4:
                        Console.Clear();
                        BorrowBook();
                        break;

                    case 5:
                        Console.Clear();
                        ReturnBook();
                        break;

                    case 6:
                        Console.Clear();
                        SaveBooksToFile();
                        bool Response = LeaveLibrary(ExitFlag);
                        if (Response == true)
                        {
                            ExitFlag = true;
                        }
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;

                }
                Console.Write("Press enter to continue. ");
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
                int BorrowID = 0;

                try
                {
                    BorrowID = int.Parse(Console.ReadLine());
                }catch(Exception ex) { Console.WriteLine(ex.Message+ "\n"); }

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
                        Console.WriteLine("We've got this in stock!\n");
                        Console.Write("Would you like to proceed? Yes or No: ");
                        string Checkout = Console.ReadLine().ToLower();

                        if (Checkout != "no")
                        {

                            DateTime Now = DateTime.Now;

                            //Decreasing book quantity 
                            int NewQuantity = (Books[Location].BookQuantity - 1);
                            int NewBorrowed = (Books[Location].Borrowed + 1);
                            Books[Location] = ((Books[Location].BookID, Books[Location].BookName, Books[Location].BookAuthor, Quantity: NewQuantity, Borrowed: NewBorrowed, Books[Location].Price, Books[Location].Category, Books[Location].BorrowPeriod));

                            //Appending data to borrow tuple list
                            //System.TimeSpan timeSpan = new System.TimeSpan(Books[Location].BorrowPeriod);
                            DateTime Return = Now.AddDays(Books[Location].BorrowPeriod);

                            //DEFUALT: actual return is the return by date | rating -1 | isReturned false
                            Borrowing.Add((UserID: CurrentUser, BorrowID: Books[Location].BookID, BorrowedOn:Now, ReturnBy: Return, ActualReturn: Return, Rating:  -1, IsReturned: false));

                            SaveBorrowInfo();
                            SaveBooksToFile();

                            Invoices.Add((CurrentUser, DateTime.Now, Books[Location].BookID, Books[Location].BookName, Books[Location].BookAuthor, 1));
                            SaveInvoice();

                            //Printing recipt 
                            Console.Clear();
                            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
                            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine("\t\t" + Now);
                            Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine($"BOOK: ID - {Books[Location].BookID} \nNAME - {Books[Location].BookName} \nAUTHOR - {Books[Location].BookAuthor} \nRETURN BY - {Return}");
                            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n\n");
                            Console.WriteLine("\tThank you for visiting the library come again soon!");
                            Console.WriteLine("\tHappy Reading :)");
                            Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n ");

                            Console.WriteLine("Press enter to continue");
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
                    Console.WriteLine("Sorry we don't have this book :(");
                    Console.WriteLine("We might have something else that you might like! \n\nWould you like to see what we have in stock? \nYes to continue anything else to leave.");
                    string ViewOtherBooks = Console.ReadLine().ToLower();

                    if (ViewOtherBooks != "yes")
                    {
                        Console.WriteLine("Exiting...");
                    }
                    else
                            { ViewAllBooks(); }
                }
            }
            else { Console.WriteLine("Sorry it looks like we don't have any books available right now :( \n"); }
        }


        //RETURN BOOK
        static void ReturnBook()
        {
            List<int> BorrowedBookIDs = new List<int>();
            double CountDown;  
            bool Found = false;
            int ReturnBook = 0;
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
            Console.Write("\n\t\tRETURN A BOOK:\n\n");

            Console.WriteLine("BORROWED BOOKS: ");
            for (int i = 0; i < Borrowing.Count; i++)
            {
                if (Borrowing[i].UserID == CurrentUser && Borrowing[i].IsReturned != true)
                { 
                    CountDown = (Borrowing[i].ReturnBy - DateTime.Now).TotalDays;
                    CountDown = Math.Round(CountDown, 0);
                    Console.WriteLine($"Book ID: {Borrowing[i].BookID} \nReturn Date: {Borrowing[i].ReturnBy} \nDays remaining: {CountDown}\n");
                    BorrowedBookIDs.Add(Borrowing[i].BookID);
                    
                }
            }


            Console.Write("Enter Book ID: ");

            try
            {
                ReturnBook = int.Parse(Console.ReadLine());
            }catch (Exception ex) { Console.WriteLine(ex.Message); }
           

            if (BorrowedBookIDs.Contains(ReturnBook)) //Checking if the user has borrowed the book they are trying to return
            {
                for (int i = 0; i < Books.Count; i++)
                {
                    if (Books[i].BookID == ReturnBook)
                    {
                        //Checking if this book has been borrowed -> handels case of books being returned without being borrowed (ie new books added)
                        if (Books[i].Borrowed > 0)
                        {
                            DateTime Now = DateTime.Now;
                            int NewBorrowCount = (Books[i].Borrowed - 1);
                            int NewBookQuantity = (Books[i].BookQuantity + 1);
                            Books[i] = ((Books[i].BookID, Books[i].BookName, Books[i].BookAuthor, Quantity: NewBookQuantity, Borrowed: NewBorrowCount, Books[i].Price, Books[i].Category, Books[i].BorrowPeriod));

                            // Borrowing.Add((UserID: CurrentUser, BorrowID: NewBID, BorrowedOn:Now, ReturnBy: Return, ActualReturn: Return, Rating:  -1, IsReturned: false));
                            Console.WriteLine($"Please rate {Books[i].BookName} out of 5");
                            Console.Write("Rating: ");

                            float UserRate;
                            while (!float.TryParse(Console.ReadLine(), out UserRate) || UserRate < 0)
                            {
                                Console.WriteLine("Invalid input please enter a number greater than 0.");
                                while (UserRate < 0 || UserRate > 6)
                                {
                                    Console.WriteLine("Invalid input please enter a number between 0 and 5.");
                                    Console.Write("Rating: ");
                                    UserRate = float.Parse(Console.ReadLine());
                                }
                            }

                            int Location = -1;
                            for (int j = 0; j < Borrowing.Count; j++)
                            {
                                if (Borrowing[j].BookID == ReturnBook)
                                {
                                    Location = j;
                                    break;
                                }
                            }

                            Borrowing[Location] = ((Borrowing[Location].UserID, Borrowing[Location].BookID, Borrowing[Location].BorrowedOn, Borrowing[Location].ReturnBy, ActualRetrun: Now, Rating: UserRate, IsReturned: true));

                            Console.WriteLine($"Thank you for returning {Books[i].BookName} :) \nPress enter to print your recipt");
                            Console.ReadKey();
                            SaveBooksToFile();
                            SaveBorrowInfo();
                            Console.Clear();
                            ReturnRecipt(i);
                            Found = true;
                        }
                        else
                        {
                            Console.WriteLine("This book has not been borrowed. \nPress enter to continue.");
                            Console.ReadKey();
                            Found = true;

                        }
                        break;
                    }
                    Found = true;


                }
                if (Found != true) { Console.WriteLine("Invalid Book ID :("); }

            }
            else { Console.WriteLine("You have not taken out this book :) \nPlease check your recipt for book ID"); }
        }

        
        //PRINTS USER DETAILS
        static void ViewUsrProfile()
        {
            List<int> SearchIDs = new List<int>();
            List<int> BookID = new List<int>();
            List<int> BorrowedBookIDs = new List<int>();
            double CountDown;
            DateTime Now = DateTime.Now;

            for (int i = 0; i<Users.Count; i++) 
            {
                SearchIDs.Add(Users[i].UserID);
            }

            for (int i = 0; i < Books.Count; i++)
            {
                BookID.Add(Books[i].BookID);
            }

            int CurrentIndex = SearchIDs.IndexOf(CurrentUser);

            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.WriteLine($" * * * * * * * * * * * * {Users[CurrentIndex].UserUserName}'s Home Page :) * * * * * * * * * * * *\n ");
            Console.WriteLine($"MY DETAILS: \nUser ID: {Users[CurrentIndex].UserID} \nUser Name: {Users[CurrentIndex].UserUserName} \nEmail: {Users[CurrentIndex].UserEmail}\n");
            Console.WriteLine(" * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n ");
            Console.WriteLine($"CURRENTLY BORROWED:"); 
            for (int i = 0; i < Borrowing.Count; i++)
            {
                if (Borrowing[i].UserID == CurrentUser && Borrowing[i].IsReturned != true)
                {
                    CountDown = (Borrowing[i].ReturnBy - DateTime.Now).TotalDays;
                    CountDown = Math.Round(CountDown, 0);

                   // int BookIndex = BookID.IndexOf(Borrowing[i].BookID);
                  //  CountDown = Borrowing[i].ReturnBy.CompareTo(Now);
                    Console.WriteLine($"Book ID: {Borrowing[i].BookID} \nReturn Date: {Borrowing[i].ReturnBy} \nDays remaining: {CountDown}\n");
                    BorrowedBookIDs.Add(Borrowing[i].BookID);

                }
            }
            Console.WriteLine("\n");
            Console.WriteLine(" * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n");
            Console.WriteLine("RETURNED BOOKS:");
            for (int i = 0; i < Borrowing.Count; i++)
            {
                if (Borrowing[i].UserID == CurrentUser && Borrowing[i].IsReturned != false)
                {
                    Console.WriteLine($"Book ID: {Borrowing[i].BookID} \nReturn Date: {Borrowing[i].ReturnBy} \nActual Return: {Borrowing[i].ActualReturn}\n");
                    BorrowedBookIDs.Add(Borrowing[i].BookID);

                }
            }
            Console.WriteLine("\n");
            Console.WriteLine(" * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\n");

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
                        writer.WriteLine($"{user.UserID} | {user.UserUserName} | {user.UserEmail} | {user.UserPswd}");
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
                using (StreamWriter writer = new StreamWriter(InvoicePath, true))
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


        //ALLOWS USER TO SEARCH FOR BOOK WITH OUTPUTS SUITED FOR USER 
        static void UserSearchForBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\tSEARCH LIBRARY:\n\n");
            Console.Write("Book name: ");
            string name = (Console.ReadLine().Trim()).ToLower();
            bool flag = false;

            for (int i = 0; i < Books.Count; i++)
            {
                if ((Books[i].BookName).ToLower() == name)
                {
                    Console.WriteLine($"Book Title: {Books[i].BookName} \nBook Author: {Books[i].BookAuthor} \nID: {Books[i].BookID} \nCategory: {Books[i].Category} \nPrice: {Books[i].Price} \nBorrow Period: {Books[i].BorrowPeriod} days");
                    flag = true;
                    break;
                }
            }

            if (flag != true)
            { Console.WriteLine("Book not found :("); }
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
                            var parts = line.Split(" | ");
                            if (parts.Length == 4)
                            {
                                if (Usr == parts[1] && parts[3] == Pswd)
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
                int choice = 0;

                try 
                {
                    choice = int.Parse(Console.ReadLine());
                }catch (Exception ex) { Console.WriteLine(ex.Message); }

                switch (choice)
                {

                    case 1:
                        Console.Clear();
                        AddNewBook();
                        break;

                    case 2:
                        Console.Clear();
                        ViewAllBooks();
                        break;

                    case 3:
                        Console.Clear();
                        SearchForBook();
                        break;

                    case 4:
                        Console.Clear();
                        EditBooks();
                        break;

                    case 5:
                        Console.Clear();
                        DeleteBook();
                        break;

                    case 6:
                        Console.Clear();
                        Reports();
                        break;

                    case 7:
                        Console.Clear();
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;

                }
                Console.WriteLine("Press enter to continue.");
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

            int ID = Books.Count + 100;

            Console.Write($"Book ID: {ID}\n");
            

            Console.Write("Enter Book Quantity: ");
            int Qty = 0;
            try
            {
                Qty = int.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("\nDefualt Quantity of 0 set\n"); }

            Console.Write("Enter Book Price: ");
            float Price = 0;
            try
            {
                Price = float.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("\nDefualt Price of 0 set\n"); }



            Console.Write("Enter Book Category: ");
            string Category = Console.ReadLine();

            Console.Write("Enter Book BorrowPeriod: ");
            int BorrowPeriod = 10;

            try
            { 
                 BorrowPeriod = int.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("\n Defualt Borrow Period of 10 set\n"); }

            Books.Add(  (ID, Name, Author, Qty, 0, Price, Category, BorrowPeriod )  );
            SaveBooksToFile();
        }


        //ALLOWS USER TO SEARCH FOR BOOK - SPECIAL ADMIN OUTPUT 
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
                    Console.WriteLine($"Book ID: {Books[i].BookID} \nBook Title: {Books[i].BookName} \nBook Author: {Books[i].BookAuthor} \nCategory: {Books[i].Category} \nPrice: {Books[i].Price} \nAvailable Stock: {Books[i].BookQuantity} \nBorrowed Copies: {Books[i].Borrowed} \nBorrowed Period: {Books[i].BorrowPeriod} ");
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
            int Choice=0;

            try 
            {
                Choice = int.Parse(Console.ReadLine()); 
            }catch(Exception ex) { Console.WriteLine(ex.Message); }

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
                        Books[Location] = ((Books[Location].BookID,BookName: NewBookName, Books[Location].BookAuthor, Books[Location].BookQuantity, Books[Location].Borrowed, Books[Location].Price, Books[Location].Category, Books[Location].BorrowPeriod));
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
                        Books[Position] = ((Books[Position].BookID, Books[Position].BookName, BookAuthor: NewAuthName, Books[Position].BookQuantity, Books[Position].Borrowed, Books[Position].Price, Books[Position].Category, Books[Position].BorrowPeriod));
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
                        int Add = 0;

                        try 
                        {
                            Add = int.Parse(Console.ReadLine());
                        }catch (Exception ex) { Console.WriteLine(ex.Message); }    

                        //Checking the positive number inputted so that books aren't minused 
                        if (Add > 0)
                        {
                            Add = Books[Index].BookQuantity + Add;
                            Books[Index] = ((Books[Index].BookID, Books[Index].BookName, Books[Index].BookAuthor, BookQuantity: Add, Books[Index].Borrowed, Books[Index].Price, Books[Index].Category, Books[Index].BorrowPeriod));
                            Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Index].BookName}  Author: {Books[Index].BookAuthor}  ID: {Books[Index].BookID}  x{Books[Index].BookQuantity}  Issues Borrowed: {Books[Index].Borrowed}\n ");
                            SaveBooksToFile();
                        }
                        else { Console.WriteLine("Improper input please input a number greater than 0 :("); }
                    }
                    break;


                case 4:
                    SaveBooksToFile();
                    break;
                

                default:
                    Console.WriteLine("Improper input, please choose one of the given options :(");
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
                    Books.Remove(( Books[DeleteIndex] = (Books[DeleteIndex].BookID, Books[DeleteIndex].BookName, Books[DeleteIndex].BookAuthor, Books[DeleteIndex].BookQuantity, Books[DeleteIndex].Borrowed, Books[DeleteIndex].Price, Books[DeleteIndex].Category, Books[DeleteIndex].BorrowPeriod)));
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
            int ChangeID = -1;

            try 
            {
                ChangeID = int.Parse(Console.ReadLine());
            
            }catch(Exception ex) { Console.WriteLine(ex.Message); }

            //Checking if book ID exists in library
            bool Found = false;
            if (ChangeID != -1)
            {
                for (int i = 0; i < Books.Count; i++)
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
                        var (BookID, BookNames, BookAuthors, BookQuantity, Borrowed, Price, Category, BorrowPeriod) = Books[i];
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
                var (BookID, bookNames, bookAuthors, BookQuantity, Borrowed, Price, Category, BorrowPeriod) = Books[i];
                BookNames.Add(bookNames);
                BookAuthors.Add(bookAuthors);
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
           // Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
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
            int Occurances = 0;
            int CompareID = 0;
            int HighestID = 0;


            for (int i = 0; i < ReaderIDs.Count; i++)
            {
                /*
                if (ReaderIDs.Contains(ReaderIDs[i])) //Counting how many times ID repeats
                {
                    Occurances++;
                    
                }
                
                for (int j = 0; j < ReaderIDs.Count; j++)
                {
                    if (ReaderIDs[i] == ReaderIDs[j])
                    {
                        Occurances++;
                    }
                }






                int b = ReaderIDs[i].Count();

                if (Occurances > CompareID)
                {
                    HighestID = ReaderIDs[i];
                }
                Occurances = 0;
            }

            Console.WriteLine("Reader ID: " + HighestID);

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
                        writer.WriteLine($"{admin.AdminID} | {admin.AdminUserName} | {admin.AdminEmail} | {admin.AdminPswd}");
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
                            var parts = line.Split(" | ");
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

/*
 * Not important to load these as it is only used for store records --> black box kind of thing :)
        //LOADS INVOICE RECORDS 
        static void LoadInvoices()
        {
            Invoices.Clear();
            try
            {
                if (File.Exists(InvoicePath))
                {
                    using (StreamReader reader = new StreamReader(InvoicePath, true))
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
*/
    }
}
