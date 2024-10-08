﻿using Microsoft.VisualBasic.FileIO;
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
using System.Text.RegularExpressions;
using System.Data.SqlTypes;
using System.Diagnostics.Metrics;

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

            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - -W E L C O M E   T O   T H E   L I B R A R Y- - - - - - - - - - - - - - - - - - -\n\n");
            PrintCastle();
            Console.Write("\n\t\t\t\t\t     Press enter to continue...");
            Console.ReadKey();

            LeaderBoard();
            Console.Write("\t\t\t\t\t  Press enter to continue...");
            Console.ReadKey();

            bool Authentication = false;
            do
            {
                Console.Clear();
                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                Console.Write("\n\n\n\n\t\t\t\t\t\t   MAIN MENU:\n\n\n");
                Console.WriteLine("\t\t\t\t\t\t  1.  Reader Login\n");
                Console.WriteLine("\t\t\t\t\t\t  2.  Librarian Login\n");
                Console.WriteLine("\t\t\t\t\t\t  3.  Register\n");
                Console.WriteLine("\t\t\t\t\t\t  4.  Exit\n\n\n");
                int Option = 0;
                Console.Write("\t\t\t\t\t\t   Enter: ");
               

                try
                {
                    Option = int.Parse(Console.ReadLine());
                }catch (Exception ex) { Console.WriteLine(ex.Message); Console.WriteLine("\n Press enter to continue :(");  Console.ReadKey(); }

                switch (Option)
                {
                    case 1:
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        Console.Write("\n\n\n\n\n\t\t\t\t\t\t   READER LOGIN:\n\n");
                        Console.Write("\t\t\t\t\tUsername: ");
                        string Usr = Console.ReadLine();
                        Console.Write("\t\t\t\t\tPassword: ");
                        string Pswd = Console.ReadLine();
                        bool UsrAuth = ReaderLogin(Usr, Pswd);

                        if (UsrAuth == true)
                        {

                            for (int i = 0; i < Users.Count; i++)
                            {
                                if (Users[i].UserUserName.Trim() == Usr)
                                {
                                    CurrentUser = Users[i].UserID;
                                }
                            }
                            Console.Clear();

                            UserPage();
                        }

                        else
                        {
                            bool UsrFound = false;
                            for (int i = 0; i < Users.Count; i++)
                            {
                                if (Users[i].UserUserName.Trim() == Usr)
                                {
                                    UsrFound = true;
                                }
                            }

                            if (UsrFound == false)
                            {
                                Console.WriteLine("\n\t\t\t\t\t<!>This username is not in our system<!> \n\n\t\t\t\tWould you like to register? Yes to register, enter to exit");
                                Console.Write("\t\t\t\t\tEnter: ");
                                string NewRegistration = (Console.ReadLine()).ToLower();

                                if (NewRegistration == "yes")
                                {
                                    Register();
                                }
                            }
                            else
                            {
                                Console.WriteLine("\n\t\t\t\t\t<!>Incorrect password please try again :( <!>");
                                Console.WriteLine("\t\t\t\t\t<!>Press enter to try again<!>");
                                Console.ReadKey();
                            }
                        }
                        break;

                    case 2:
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        Console.Write("\n\n\n\n\t\t\t\t\t\t   LIBRARIAN LOGIN:\n\n");
                        Console.Write("\t\t\t\t\tUsername: ");
                        string AdminUsr = Console.ReadLine();
                        Console.Write("\t\t\t\t\tPassword: ");
                        string AdminPswd = Console.ReadLine();
                        bool AdminAuth = LibrarianLogin(AdminUsr, AdminPswd);

                        if (AdminAuth)
                        {

                            for (int i = 0; i < Admins.Count; i++)
                            {
                                if (Admins[i].AdminUserName.Trim() == AdminUsr)
                                {
                                    CurrentUser = Admins[i].AdminID;
                                }
                            }

                            AdminPage();
                        }
                        else
                        {
                            bool AdminUserFound = false;
                            for (int i = 0; i < Admins.Count; i++)
                            {
                                if (Admins[i].AdminUserName.Trim() == AdminUsr)
                                {
                                    AdminUserFound = true;
                                }
                            }

                            if (AdminUserFound == false)
                            {
                                Console.WriteLine("\n\t\t\t\t\t<!>This username is not in our system<!> \n\n\t\t\t\tWould you like to register? Yes to register, enter to exit");
                                Console.Write("\t\t\t\t\tEnter: ");
                                string NewRegistration = (Console.ReadLine()).ToLower();

                                if (NewRegistration == "yes")
                                {
                                    Register();
                                }
                            }
                            else
                            {
                                Console.WriteLine("\n\t\t\t\t\t<!>Incorrect password please try again :( <!>");
                                Console.Write("\n\t\t\t\t\t<!>Press enter to try again<!>");
                                Console.ReadKey();
                            }
                        }
                        
                        break;

                    case 3:
                        Console.Clear();
                        Register();
                        Console.Clear();
                        break;

                    case 4:
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - -L E A V I N G   T H E   L I B R A R Y- - - - - - - - - - - - - - - - - - -\n\n");
                        PrintAlien();
                        Console.WriteLine("\t\t\t\tPress enter to continue...");
                        Console.ReadKey();
                        Authentication = true;
                        break;

                    default:
                        Console.WriteLine("\n\t\t\t\t\t<!>Invalid input :( <!> \n\t\t\t<!>Please try again, enter one of the given options<!>");
                        Console.WriteLine("\n\t\t\t\t\t<!>Press enter to continue<!>"); Console.ReadKey();
                        break;
                }
            } while (Authentication != true);
        }




        //- - - - - - - - - - - - - FUNCTIONS SHARED BETWEEN ADMIN AND USER - - - - - - - - - - - - - - - //

        //REGISTERS NEW USERS
        static void Register()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\t\t\t\t\t\t      REGISTER:\n\n\n");
            Console.WriteLine("\n\t\t\t\t\t\t  OPTIONS: \n");
            Console.WriteLine("\t\t\t\t\t\t  1.  Reader\n");
            Console.WriteLine("\t\t\t\t\t\t  2.  Librarian\n");
            Console.WriteLine("\t\t\t\t\t\t  3.  Exit\n\n\n");
            int Identity = 0;
            Console.Write("\t\t\t\t\t\t  Enter: ");

            try 
            {
                Identity = int.Parse(Console.ReadLine());

            } catch (Exception ex) { Console.WriteLine(ex.Message); }

            switch (Identity)
            {
                case 1:
                    //User registration
                    Console.Clear();
                    Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                    Console.Write("\n\t\t\t\t\t\tREADER REGISTRATION:\n\n\n");
                    Console.WriteLine("\t\t\t\t\t\tWelcome new reader!\n\n");
                    string UserPassword1 = " "; //This has one space
                    string UserPassword2 = "  "; //This has two spaces so that it doesn't affect do while loop condition below
                    string Email1 = " ";
                    string Email2 = "  ";
                    bool EmailTemp = false;
                    bool UsrContinue = false;
                    bool ExistingUsr = false;
                    string EmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                    Regex userRegex = new Regex(EmailPattern);

                    //Checks email validity
                    Console.WriteLine("\n\t\t\t\t\tHint: Make sure your emails match");
                    do
                    {
                        Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                        Console.Write("\t\t\t\t\t\tEmail: ");
                        Email1 = Console.ReadLine();
                        Console.Write("\t\t\t\t\t\tRe-enter Email: ");
                        Email2 = Console.ReadLine();


                        bool isValid = userRegex.IsMatch(Email1);

                        if (isValid == true)
                        {
                            EmailTemp = true;
                        }

                        else
                        {
                            Console.WriteLine("\n\t\t\t\t<!>Sorry this email is not in the correct format :( <!>");
                        }

                        for (int i = 0; i < Users.Count; i++)
                        {
                            if (Users[i].UserEmail.Trim() == Email1.Trim())
                            {
                                Console.WriteLine("\n\t\t\t\t<!>This email has already been used to create an account :( <!>");
                                ExistingUsr = true;
                            }
                        }

                        if (Email1 != Email2)
                        {
                            Console.WriteLine("\n\t\t\t\t\t<!>These emails do not match :( <!>");
                        }

                        if (EmailTemp != false && Email1 == Email2 && ExistingUsr != true) //ensures that emails match and in correct format 
                        {
                            UsrContinue = true;
                        }

                    } while (UsrContinue != true);


                    //Checks that usernames are not repeated 
                    bool RepeatedName = false;
                    string UserName;
                    do
                    {
                        Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                        RepeatedName = false;
                        Console.Write("\t\t\t\t\t\t   User Name: ");
                        UserName = Console.ReadLine();

                        for (int i = 0; i < Users.Count; i++)
                        {
                            if (Users[i].UserUserName.ToLower().Trim() == UserName.ToLower().Trim())
                            {
                                RepeatedName = true;
                                Console.WriteLine("\n<!>This username is taken please try another one :( <!>");
                            }
                        }
                    } while (RepeatedName != false);



                    //Password format validation 
                    bool PassValid = false;
                    bool PassOk = false;
                    string PasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()]).{8,}$";
                    Regex UserPassRegex = new Regex(PasswordPattern);

                    Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                    Console.WriteLine("\n\t\tHint: Make sure your passwords match and follow criteria below");
                    Console.WriteLine("\t\tAt least 8 characters, includes upper and lower case characters, contains number and special character\n");
                    do
                    {
                        Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                        Console.Write("\t\t\t\t\t\t   Password: ");
                        UserPassword1 = Console.ReadLine();
                        Console.Write("\t\t\t\t\t\t   Re-enter Password: ");
                        UserPassword2 = Console.ReadLine();
                        PassValid = UserPassRegex.IsMatch(UserPassword1);
                        if (PassValid != true) 
                        {
                            Console.WriteLine("\n\t\t\t\t\t<!>This passowrd is not in the correct format :( <!>\n");
                        }

                        if (UserPassword1 != UserPassword2)
                        {
                            Console.WriteLine("\n\t\t\t\t\t<!>The passwords do not match :( <!>");
                        }

                        if (UserPassword1 == UserPassword2 && PassValid == true) // checks pass format is valid 
                        {
                            PassOk = true;
                        }
                       

                    } while (PassOk !=true);

                    //Geneate ID
                    int UserID = Users.Count + 10;

                    Users.Add((UserID, UserName, Email1, UserPassword1));
                    Console.Clear();
                    Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                    PrintPenguin();
                    Console.WriteLine("\t\t\t\tPress enter to continue!");
                    Console.ReadKey();
                    SaveUsers();
                    break;

                case 2:
                    //Admin registration
                    Console.Clear();
                    Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                    Console.Write("\n\n\n\n\t\t\t\t\t\t   LIBRARIAN REGISTRATION:\n\n");


                    //AUTHENTICATE MASTER ADMIN 
                    Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                    PrintGrimReaper();
                    Console.Write("\t\t\t\t\t\t   Master Username: ");
                    string Usr = Console.ReadLine();
                    Console.Write("\t\t\t\t\t\t   Master Password: ");
                    string Pswd = Console.ReadLine();

                    bool Auth = CheckMaster(Usr, Pswd);

                    if (Auth != false)
                    {
                        Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                        Console.WriteLine("\t\t\t\t\t\t   Welcome new librarian!");
                        string AdminPassword1 = " "; //This has one space
                        string AdminPassword2 = "  "; //This has two spaces so that it doesn't affect do while loop condition below
                        string AdminEmail1 = " ";
                        string AdminEmail2 = "  ";
                        string EmailHolder;
                        bool EmailFormat = false;
                        bool PasswordFormat = false;
                        bool Reused = false;    
                        bool Continue = false;

                        string AdminEmailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                        Regex regex = new Regex(AdminEmailPattern);

                        //Checks email validity
                        Console.WriteLine("\n\t\t\t\t\t\t   Hint: Make sure your emails match");
                        do
                        {
                            Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                            EmailFormat = false;
                            Console.Write("\t\t\t\t\t\t   Email: ");
                            AdminEmail1 = Console.ReadLine();
                            Console.Write("\t\t\t\t\t\t   Re-enter Email: ");
                            AdminEmail2 = Console.ReadLine();

                            bool isValid = regex.IsMatch(AdminEmail1);

                            if (isValid == true)
                            {
                                EmailFormat = true;
                            }

                            else 
                            {
                                Console.WriteLine("\n\t\t\t\t<!>Sorry this email is not in the correct format :( <!>");
                            }

                            for (int i = 0; i < Admins.Count; i++)
                            {
                                if (Admins[i].AdminEmail.Trim() == AdminEmail1.Trim())
                                {
                                    Console.WriteLine("\n\t\t\t\t<!>This email has already been used to create an account :( <!>");
                                    Reused = true;
                                }
                            }

                            if (AdminEmail1 != AdminEmail2)
                            {
                                Console.WriteLine("\n\t\t\t\t\t\t<!>These emails do not match :( <!>");
                            }

                            if (EmailFormat != false && AdminEmail1 == AdminEmail2 && Reused != true) //ensures that emails match and in correct format 
                            {
                                Continue = true;
                            }

                        } while (Continue != true);


                        //Checks that usernames are not repeated 
                        bool RepeatedAdminName = false;
                        string AdminUserName;
                        do
                        {
                            Console.WriteLine("\n- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                            RepeatedName = false;
                            Console.Write("\t\t\t\t\t\t   User Name: ");
                            AdminUserName = Console.ReadLine();

                            for (int i = 0; i < Admins.Count; i++)
                            {
                                if (Admins[i].AdminUserName.ToLower().Trim() == AdminUserName.ToLower().Trim())
                                {
                                    RepeatedName = true;
                                    Console.WriteLine("\n\t\t\t\t\t\t<!>This username is taken please try another one :( <!>");
                                }
                            }
                        } while (RepeatedName != false);



                        bool AdminPassOk = false;
                        string AdminPasswordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()]).{8,}$";
                        Regex AdminPassRegex = new Regex(AdminPasswordPattern);


                        Console.WriteLine("\n\t\tHint: Make sure your passwords match and follow criteria below");
                        Console.WriteLine("\t\tAt least 8 characters, includes upper and lower case characters, contains number and special character\n");
                        do
                        {
                            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -\n");
                            Console.Write("\t\t\t\t\t\t   Password: ");
                            AdminPassword1 = Console.ReadLine();
                            Console.Write("\t\t\t\t\t\t   Re-enter Password: ");
                            AdminPassword2 = Console.ReadLine();
                            PassValid = AdminPassRegex.IsMatch(AdminPassword1);

                            if (PassValid != true)
                            {
                                Console.WriteLine("\n\t\t\t\t<!>This passowrd is not in the correct format :( <!>\n");
                            }

                            if (AdminPassword1 != AdminPassword2)
                            {
                                Console.WriteLine("\n\t\t\t\t\t<!>The passwords do not match :( <!>");
                            }

                            if (AdminPassword1 == AdminPassword2 && PassValid == true)
                            {
                                AdminPassOk = true;
                            }

                        } while (AdminPassOk != true);

                        

                        //Geneate id
                        int AdminID = Admins.Count + 10;
                        Admins.Add((AdminID, AdminUserName, AdminEmail1, AdminPassword1));
                        SaveAdmins();
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        PrintDino();
                        Console.WriteLine("\n\t\t\t\t\t\t   Press enter to continue!");
                        Console.ReadKey();
                    }
                    else
                    { 
                        Console.WriteLine("\n\t\t\t<!>The inputted credentials are incorrect, please try again :( <!>");
                        Console.WriteLine("\n\t\t\t<!>Press enter to try again<!> ");
                        Console.ReadKey();
                    }
                    break;

                case 3:
                    break;

                default:
                    Console.WriteLine("\n\t\t\t\t\t\t<!>Invalid input :( <!>\n\t\t\t\t\t\t<!>Please try again, enter one of the given options<!>"); 
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
            Console.WriteLine("\n\nAre you sure you want to leave? \n\nYes to leave anything else to stay.");
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
                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                Console.WriteLine("\n\nThank you for visiting the library :) \nCome again soon!\n\n");
                return true;
            }
        }


        //DISPLAYS ALL BOOKS
        static void ViewAllBooks()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   AVAILABLE BOOKS:\n\n");
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            if (Books != null) 
            {
                var BooksTable = new DataTable("Books");
                BooksTable.Columns.Add("ID", typeof(int));
                BooksTable.Columns.Add("NAME", typeof(string));
                BooksTable.Columns.Add("AUTHOR", typeof (string));
                BooksTable.Columns.Add("CATEGORY", typeof (string));
                BooksTable.Columns.Add("AVAILABLE QTY", typeof(int));

                for (int i = 0; i <Books.Count; i++) 
                {
                    BooksTable.Rows.Add(Books[i].BookID , Books[i].BookName.Trim(), Books[i].BookAuthor.Trim(), Books[i].Category.Trim(), Books[i].BookQuantity);
                }

                foreach (DataColumn column in BooksTable.Columns)
                {
                    Console.Write($"{column.ColumnName,-25}");
                }
                Console.WriteLine();


                foreach (DataRow row in BooksTable.Rows)
                {
                    foreach (var item in row.ItemArray)
                    {
                        Console.Write($"{item,-25}");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
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


        //LEADERBOARD OF MOST ACTIVE READERS 
        static void LeaderBoard()
        {  
            int Counter  = 0;
            int HighestID = 0;
            int SecondScore = 0;
            int ThirdScore = 0;
            int HighestScore = 0;
            List<int> ReaderIDs = new List<int>();

            //Isolating reader IDs
            for (int i = 0; i < Borrowing.Count; i++) 
            {
                if (Borrowing[i].IsReturned) //We only want values of books completed by reader (so borrowed and returned as well)
                {
                    ReaderIDs.Add(Borrowing[i].UserID);
                }
            }


            //Finds the top 3 readers and shows their score 
            for (int i = 0; i < ReaderIDs.Count; i++) 
            {
                Counter = 0;
                for(int j=0; j<ReaderIDs.Count; j++) 
                {
                    if (ReaderIDs[i] == ReaderIDs[j])
                    {
                        Counter++;
                    }
                }

                if (Counter > HighestScore) //New High score 
                {
                    //Rearranging top scorers
                    ThirdScore = SecondScore;
                    SecondScore = HighestScore;
                    HighestScore = Counter;
                    
                    HighestID = ReaderIDs[i];
                }


                Console.Clear();
                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - -W E L C O M E   T O   T H E   L I B R A R Y- - - - - - - - - - - - - - - - - - -\n\n");
                Console.WriteLine("\n\t\t\t\t\t     L E A D E R   B O A R D:\n\n");
                PrintCrown();
                Console.WriteLine($"\t\t\t\t\t  HIGHEST SCORE -> {HighestScore} books read!");
                Console.WriteLine($"\t\t\t\t\t  SECOND PLACE -> {SecondScore} books read");
                Console.WriteLine($"\t\t\t\t\t  THIRD PLACE -> {ThirdScore} books read\n");
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
                            var parts = line.Split("|");
                            if (parts.Length == 4)
                            {
                                if (Usr == parts[1].Trim() && parts[3].Trim() == Pswd)
                                {
                                    i = true;
                                    CurrentUser = int.Parse(parts[0]);
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
            bool Overdue = false;
            string BookName = " ";
            List<(int BookID, string BookName, DateTime ReturnDate, double DaysLate)> OverdueBooks = new List<(int BookID, string BookName, DateTime ReturnDate, double DaysLate)>();

            for (int i = 0; i < Borrowing.Count; i++)
            {
                double DaysLeft = (Borrowing[i].ReturnBy - DateTime.Now).TotalDays;
                if (DaysLeft < 0 && Borrowing[i].IsReturned == false && Borrowing[i].UserID == CurrentUser)//user has overdue books
                {
                    Overdue = true;
                    for (int j = 0; j < Books.Count; j++)
                    {
                        if (Books[j].BookID == Borrowing[i].BookID)
                        {
                            BookName = Books[j].BookName;
                        }
                    }

                    double DaysCount = (Borrowing[i].ReturnBy - DateTime.Now).TotalDays;
                    int OverdueDays = (int)Math.Truncate(DaysCount);

                    OverdueBooks.Add((Borrowing[i].BookID, BookName, Borrowing[i].ReturnBy, OverdueDays)); //Add pendalty here as well 

                    Console.Clear();
                    Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                    Console.WriteLine("! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ACCOUNT SUSPENDED :( ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! ! !");
                    PrintSpaceShip();

                    Console.WriteLine("Please return the following overdue books to unlock your account\n");
                    for (int j = 0; j < OverdueBooks.Count; j++)
                    {
                        Console.WriteLine($"Book ID: {OverdueBooks[j].BookID} \nBook Name: {OverdueBooks[j].BookName} \nReturn Date: {OverdueBooks[j].ReturnDate} \nDays Late: {OverdueBooks[j].DaysLate}\n\n");
                    }

                    bool ReturnLoop = true;
                    do
                    {
                        Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   READER OPTIONS:\n");
                        Console.WriteLine("\t\t\t\t\t1.  Return A Book\n");
                        Console.WriteLine("\t\t\t\t\t2.  Logout\n\n");
                        Console.Write("\t\t\t\t\tEnter: ");
                        int choice = 0;

                        try
                        {
                            choice = int.Parse(Console.ReadLine());
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }

                        switch (choice)
                        {
                            case 1:
                                Console.Clear();
                                ReturnBook();
                                break;

                            case 2:
                                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - -L O G I N G   O U T- - - - - - - - - - - - - - - - - - - - - - -\n\n");
                                PrintMonkey();
                                Console.Write("\t\t\t\t\tPress enter to continue...");
                                Console.ReadKey();
                                ReturnLoop = false;
                                break;

                            default:
                                Console.WriteLine("\n\t\t\t\t\t<!>Invalid choice :( <!>");
                                break;

                        }
                    } while (ReturnLoop == true);
                }
            }
                if (Overdue)
                { }

                //Only happens if user does not have overdue books 
                else
                {
                    do
                    {
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        Console.Write("\n\n\n\n\t\t\t\t\t\tREADER OPTIONS:\n\n\n");
                        Console.WriteLine("\t\t\t\t\t1. View All Books\n");
                        Console.WriteLine("\t\t\t\t\t2. Search by Book Name or Author\n");
                        Console.WriteLine("\t\t\t\t\t3. View Profile\n");
                        Console.WriteLine("\t\t\t\t\t4. Borrow A Book\n");
                        Console.WriteLine("\t\t\t\t\t5. Return A Book\n");
                        Console.WriteLine("\t\t\t\t\t6. View Leader Board\n");
                        Console.WriteLine("\t\t\t\t\t7. Log out\n\n\n");
                        Console.Write("\t\t\t\t\tEnter: ");
                        int choice = 0;

                        try
                        {
                            choice = int.Parse(Console.ReadLine());
                        }
                        catch (Exception ex) { Console.WriteLine(ex.Message); }

                        switch (choice)
                        {
                            case 1:
                                ;
                                Console.Clear();
                                ViewAllBooks();
                                if (Books.Count != 0)
                                {
                                    Console.WriteLine("Would you like to borrow a book? \n\nYes to continue enter to leave.");
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
                                 LeaderBoard();
                                Console.Write("Press enter to continue...");
                            Console.ReadKey();
                                 break;

                            case 7:
                                Console.Clear();
                                SaveBooksToFile();
                                bool Response = LeaveLibrary(ExitFlag);
                                if (Response == true)
                                {
                                    ExitFlag = true;
                                }
                                break;

                            default:
                                Console.WriteLine("\n\t\t\t\t\t<!>Sorry your choice was wrong<!>");
                                break;

                        }
                        Console.Clear();
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - -L O G I N G   O U T- - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        PrintMonkey(); 
                        Console.WriteLine("\t\t\t\t\tPress enter to continue...");
                        Console.ReadKey();
                        string cont = Console.ReadLine();
                        Console.Clear();

                    } while (ExitFlag != true);
                }
            
        }


        //BORROW BOOK
        static void BorrowBook()
        {
            bool CanBorrow = true;
            if (Books.Count != 0)
            {
                ViewAllBooks();

                Console.Write("\n\n\n\n\t\t\t\t\t\t   BORROWING A BOOK:\n\n");
                Console.Write("Enter ID: ");
                int BorrowID = 0;

                try
                {
                    BorrowID = int.Parse(Console.ReadLine());
                }catch(Exception ex) { Console.WriteLine(ex.Message+ "\n"); }

                for (int i = 0; i < Borrowing.Count; i++)
                {
                    if (Borrowing[i].UserID == CurrentUser && Borrowing[i].BookID == BorrowID && Borrowing[i].IsReturned == false) //checks if user has this book borrowed currently 
                    {
                        CanBorrow = false;
                        Console.WriteLine("Looks like you already borrowed this book, sorry you can't borrow more than one copy :(");
                        Console.WriteLine("Press enter...");
                        Console.ReadKey();
                    }
                }

                if (CanBorrow == true)
                {
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
                                Borrowing.Add((UserID: CurrentUser, BorrowID: Books[Location].BookID, BorrowedOn: Now, ReturnBy: Return, ActualReturn: Return, Rating: -1, IsReturned: false));

                                SaveBorrowInfo();
                                SaveBooksToFile();

                                Invoices.Add((CurrentUser, DateTime.Now, Books[Location].BookID, Books[Location].BookName, Books[Location].BookAuthor, 1));
                                SaveInvoice();

                                //Finding book category to print related ASCII art
                                string CurrentCategory = Books[Location].Category.Trim();


                                //Printing recipt 
                                Console.Clear();
                                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                                Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  \n\n");
                                Console.WriteLine("\t\t\t\t\t" + Now);
                                Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");

                                //Getting related ASCII art
                                switch (CurrentCategory)
                                {
                                    case "Children":
                                        PrintBear();
                                        break;

                                    case "Cooking":
                                        PrintPie();
                                        break;

                                    case "History":
                                        PrintScroll();
                                        break;

                                    case "IT":
                                        PrintComputer();
                                        break;

                                    case "Non-Fiction":
                                        PrintPerson();
                                        break;

                                    case "Science":
                                        PrintBooks();
                                        break;

                                    case "Self Help":
                                        PrintMoon();
                                        break;

                                    case "Software":
                                        PrintWindowsLogo();
                                        break;

                                    case "Stories":
                                        PrintSherlock();
                                        break;

                                    case "Young Adult":
                                        PrintPacMan();
                                        break;

                                }

                                Console.WriteLine($"BOOK: ID - {Books[Location].BookID} \nNAME - {Books[Location].BookName} \nAUTHOR - {Books[Location].BookAuthor} \nRETURN BY - {Return}");
                                Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n\n");
                                Console.WriteLine("\n\t\t\tThank you for visiting the library come again soon!");
                                Console.WriteLine("\t\t\tHappy Reading :)");
                                Console.WriteLine("\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n ");
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
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   RETURN BOOK:\n\n"); 

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

                Console.WriteLine("Press enter to continue...");
                Console.ReadKey();

            }
            else { Console.WriteLine("You have not taken out this book :) \nPlease check your recipt for book ID"); }
        }

        
        //PRINTS USER DETAILS
        static void ViewUsrProfile()
        {
            List<int> SearchIDs = new List<int>();
            List<int> BookID = new List<int>();
            List<int> BorrowedBookIDs = new List<int>();
            List<int> ReaderIDs = new List<int>();

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

            //Getting user reading ranking across all readers 
            
            int UserCounter = 0;
            int Counter = 0;
            int CurrentCount = 0;
            int HighScore = 0;
            int UserRead = 0;

            for (int i = 0; i < Borrowing.Count; i++)
            {
                if (Borrowing[i].IsReturned) //We only want values of books completed by reader (so borrowed and returned as well)
                {
                    ReaderIDs.Add(Borrowing[i].UserID);
                }
            }
            int UserRank = ReaderIDs.Count;

            for (int i = 0; i < ReaderIDs.Count; i++)
            {
                Counter = 0;
                for (int j = 0; j < ReaderIDs.Count; j++)
                {
                    if (ReaderIDs[i] == ReaderIDs[j])
                    {
                        Counter++;
                    }
                }

                if (ReaderIDs[i] != CurrentUser) //Ensures that user is not being compared to themself
                {
                    if (CurrentCount < UserCounter) //Moves users rank down one as if the counter is higher than they have a higher rank 
                    {
                        UserRank--;
                    }
                }
            }


            for (int i = 0; i < ReaderIDs.Count; i++)
            {
                if (ReaderIDs[i] == CurrentUser)
                {
                    UserRead++;
                }
            }
            

            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n");
            PrintOwl();
            Console.WriteLine($"\n\t\t\t\t\t\t {Users[CurrentIndex].UserUserName}'s Home Page :) \n ");
            Console.WriteLine($"MY DETAILS: \nUser ID: {Users[CurrentIndex].UserID} \nUser Name: {Users[CurrentIndex].UserUserName} \nEmail: {Users[CurrentIndex].UserEmail} \nUser Ranking: #{UserRank}, Books Read: {UserRead}\n");
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n");
            Console.WriteLine($"CURRENTLY BORROWED:"); 
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
            Console.WriteLine("\n");
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n");
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
            Console.WriteLine("^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^\n");
            Console.WriteLine("\n\t\t\t\t\t Press enter to continue...");
            Console.ReadKey();

        }


        //PRINT RETRUN RECIPT 
        static void ReturnRecipt(int i)
        {
            DateTime Now = DateTime.Now;

            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  \n\n");
            Console.WriteLine("\t\t\t\t\t Returned: " + Now);
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  \n\n");
            PrintDucks();
            Console.WriteLine($"\t\t\t\t\tBOOK: \nID - {Books[i].BookID} \nNAME - {Books[i].BookName} \nAUTHOR - {Books[i].BookAuthor}");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  \n\n");
            Console.WriteLine($"\t\t\t\t\tThank you for returning {Books[i].BookName} :)\n\n");
            Console.WriteLine("\t\t\t\t\t\tCome again soon!");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *  \n\n");

            Console.Write("Press enter to continue");
            Console.ReadKey();
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
                        writer.WriteLine($"{user.UserID}| {user.UserUserName.Trim()} | {user.UserEmail.Trim()} | {user.UserPswd.Trim()}");
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
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   SEARCH LIBRARY:\n\n"); 
            Console.Write("\t\t\t\t\tBook name or author: ");
            string name = (Console.ReadLine().Trim()).ToLower();
            string SearchPattern = Regex.Escape(name);
            Regex regex = new Regex(SearchPattern, RegexOptions.IgnoreCase);

            bool flag = false;

            for (int i = 0; i < Books.Count; i++)
            {

                if (regex.IsMatch(Books[i].BookName) || regex.IsMatch(Books[i].BookAuthor))
                {
                    Console.WriteLine($"\nBook Title: {Books[i].BookName} \nBook Author: {Books[i].BookAuthor} \nID: {Books[i].BookID} \nCategory: {Books[i].Category} \nPrice: {Books[i].Price} \nBorrow Period: {Books[i].BorrowPeriod} days\n");
                    flag = true;
                }
                
            }

            if (flag != true)
            { Console.WriteLine("\t\t\t\t\t<!>Book not found :( <!>"); }

            Console.Write("\n\t\t\t\t\tPress enter to continue");
            Console.ReadKey();
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
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
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
                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                Console.Write("\n\n\n\n\t\t\t\t\t\t   LIBRARIAN OPTIONS:\n\n\n");
                Console.WriteLine("\t\t\t\t\t1. Add New Book\n");
                Console.WriteLine("\t\t\t\t\t2. Display All Books\n");
                Console.WriteLine("\t\t\t\t\t3. Search by Book Name or Author\n");
                Console.WriteLine("\t\t\t\t\t4. Edit Book\n");
                Console.WriteLine("\t\t\t\t\t5. Delete Book\n");
                Console.WriteLine("\t\t\t\t\t6. Show Reports\n");
                Console.WriteLine("\t\t\t\t\t7. Log out\n\n\n");
                Console.Write("\t\t\t\t\tEnter: ");
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
                        Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - -L O G I N G   O U T- - - - - - - - - - - - - - - - - - - - - - -\n\n");
                        PrintFish();
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("\t\t\t\t\t<!>Sorry your choice was wrong<!>");
                        break;

                }
                Console.WriteLine("\t\t\t\t\tPress enter to continue...");
                string cont = Console.ReadLine();
                Console.Clear();

            } while (ExitFlag != true);

        }


        //GETS BOOK INFORMATION FROM THE USER 
        static void AddNewBook() 
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   ADDING NEW BOOK:\n\n");
            bool Repeated = false;
            string Name = " ";

            do
            {
                Repeated = false;
                Console.Write("\t\t\t\t\tEnter Book Name: ");
                Name = Console.ReadLine().Trim(); //Trim added for more accurate search  
               
                for (int i = 0; i < Books.Count; i++)
                {
                    if ((Books[i].BookName).Trim() == Name)
                    {
                        Repeated = true;
                        break;
                    }
                }

                if (Repeated != false) 
                { 
                    Console.WriteLine("\n\t\t\t\t<!>This book already exists please enter a new book<!>"); 
                   Repeated = true; 
                }

            } while (Repeated != false);


            Console.Write("\t\t\t\t\tEnter Book Author: ");
            string Author= Console.ReadLine().Trim();

            int ID = Books.Count + 1;

            Console.Write($"\t\t\t\t\tBook ID: {ID}\n");
            

            Console.Write("\t\t\t\t\tEnter Book Quantity: ");
            int Qty = 0;
            try
            {
                Qty = int.Parse(Console.ReadLine());
            }
            catch (Exception ex) { Console.WriteLine("\n\t\t\t<!>" + ex.Message+"<!>"); Console.WriteLine("\n\t\t\t<!>Defualt Quantity of 0 set<!>\n"); }

            Console.Write("\t\t\t\t\tEnter Book Price: ");
            float Price = 0;
            try
            {
                Price = float.Parse(Console.ReadLine());
            }
            catch (Exception ex) 
            { 
                Console.WriteLine("\n\t\t\t < !> " +ex.Message + "<!>"); 
                Console.WriteLine("\n\t\t\t<!>Defualt Price of 0 set\n\n\t\t\t\t\tPress enter to continue<!>\n"); 
                Console.ReadKey();
            }

            //Functionality to allow user to choose category from specific options :)
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   ADDING NEW BOOK:\n\n");
            Console.WriteLine("\t\t\t\t\tChoose a Book Category: ");
            Console.WriteLine("\t\t\t\t\t1. Children");
            Console.WriteLine("\t\t\t\t\t2. Cooking");
            Console.WriteLine("\t\t\t\t\t3. History");
            Console.WriteLine("\t\t\t\t\t4. IT");
            Console.WriteLine("\t\t\t\t\t5. Non-Fiction");
            Console.WriteLine("\t\t\t\t\t6. Science");
            Console.WriteLine("\t\t\t\t\t7. Self Help ");
            Console.WriteLine("\t\t\t\t\t8. Software");
            Console.WriteLine("\t\t\t\t\t9. Stories");
            Console.WriteLine("\t\t\t\t\t10. Young Adult");

            int CategoryChoice = 0;
            string Category = " ";
            bool FormComplete = true;

            do
            {
                do
                {

                    Console.Write("\t\t\t\t\tEnter: ");
                    try
                    {
                        CategoryChoice = int.Parse(Console.ReadLine());
                    }
                    catch (Exception ex) { Console.WriteLine("\n" + ex.Message); }

                    FormComplete = true;

                    switch (CategoryChoice)
                    {
                        case 1:
                            Category = "Children";
                            break;

                        case 2:
                            Category = "Cooking";
                            break;

                        case 3:
                            Category = "History";
                            break;

                        case 4:
                            Category = "IT";
                            break;

                        case 5:
                            Category = "Non-Fiction";
                            break;

                        case 6:
                            Category = "Science";
                            break;

                        case 7:
                            Category = "Self Help";
                            break;

                        case 8:
                            Category = "Software";
                            break;

                        case 9:
                            Category = "Stories";
                            break;

                        case 10:
                            Category = "Young Adult";
                            break;

                        default:
                            Console.WriteLine("\n\t\t\t\t\t<!>Improper input :( <!>");
                            FormComplete = false;
                            break;

                    }
                } while (FormComplete != true);
            if (FormComplete != true)
                { break; }


            Console.WriteLine($"\n\t\t\t\t\tYou have chosen {Category}. \n\n\t\t\t\t\tEnter Yes to continue No to choose again");
            Console.Write("\n\t\t\t\t\tEnter: ");
            string Confirm = (Console.ReadLine()).ToLower();

                if (Confirm != "yes")
                { 
                    Category = " "; 
                }

            } while (Category == " ");

            if (FormComplete == true)
            {
                Console.Clear();
                Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
                Console.Write("\n\n\n\n\t\t\t\t\t\t   ADDING NEW BOOK:\n\n");
                Console.Write("\t\t\t\t\tEnter Book BorrowPeriod: ");
                int BorrowPeriod = 10;

                try
                {
                    BorrowPeriod = int.Parse(Console.ReadLine());
                }
                catch (Exception ex) { Console.WriteLine("\t\t\t\t\t<!>" + ex.Message+"<!>"); Console.WriteLine("\n\t\t\t\t\tDefualt Borrow Period of 10 set\n"); }

                Books.Add((ID, Name, Author, Qty, 0, Price, Category, BorrowPeriod));
                SaveBooksToFile();

                for (int i = 0; i < Categories.Count; i++) 
                {
                    if (Categories[i].CategoryName.Trim() == Category.Trim())
                    {
                        int New = Categories[i].NoOfBooks + 1;
                        Categories[i] = ((Categories[i].CategoryID, Categories[i].CategoryName, NoOfBooks: New));
                        break;
                    }
                }
                SaveCategories();
            }
            //SaveCategories();
        }


        //ALLOWS USER TO SEARCH FOR BOOK - SPECIAL ADMIN OUTPUT 
        static void SearchForBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.Write("\n\n\n\n\t\t\t\t\t\t   SEARCH LIBRARY:\n\n");
            Console.Write("\n\t\t\t\t\t\tBook name or author: ");
            string name = (Console.ReadLine().Trim()).ToLower();  
            bool flag=false;
            string SearchPattern = Regex.Escape(name);
            Regex regex = new Regex(SearchPattern, RegexOptions.IgnoreCase);

            for (int i = 0; i< Books.Count;i++)
            {
                if (regex.IsMatch(Books[i].BookName) || regex.IsMatch(Books[i].BookAuthor))
                {
                    Console.WriteLine($"\nBook ID: {Books[i].BookID} \nBook Title: {Books[i].BookName} \nBook Author: {Books[i].BookAuthor} \nCategory: {Books[i].Category} \nPrice: {Books[i].Price} \nAvailable Stock: {Books[i].BookQuantity} \nBorrowed Copies: {Books[i].Borrowed} \nBorrowed Period: {Books[i].BorrowPeriod} \n");
                    flag = true;
                }
            }

            if (flag != true)
            { Console.WriteLine("\n\t\t\t\t\t\t<!>Book not found :( <!>"); }
        }


        //ALLOWS LIBRARIAN TO EDIT BOOK INFO
        static void EditBooks()
        {
            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   EDIT BOOKS:\n\n\n");
            Console.WriteLine("\t\t\t\t\t\t1.  Edit Book Title\n");
            Console.WriteLine("\t\t\t\t\t\t2.  Edit Author Name\n");
            Console.WriteLine("\t\t\t\t\t\t3.  Add More Copies of Available Books\n");
            Console.WriteLine("\t\t\t\t\t\t4.  Save and exit\n\n\n");
            Console.Write("\t\t\t\t\t\tEnter Option: ");
            int Choice=0;

            try 
            {
                Choice = int.Parse(Console.ReadLine()); 
            }catch(Exception ex) { Console.WriteLine(ex.Message); }

            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");

            bool ChooseOption = true;
            do
            {
                ChooseOption = true;
                switch (Choice)
                {
                    //Editing book title
                    case 1:
                        int Location = GetInformation();
                        if (Location != -1)
                        {
                            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   EDIT BOOK TITLE:\n");
                            string NewBookName;
                            bool Repeated;

                            do
                            {
                                Repeated = false;
                                Console.Write("\t\t\t\t\t\tEnter Book Name: ");
                                NewBookName = Console.ReadLine().Trim(); //Trim added for more accurate search  

                                for (int i = 0; i < Books.Count; i++)
                                {
                                    if ((Books[i].BookName).Trim() == NewBookName)
                                    {
                                        Repeated = true;
                                        break;
                                    }
                                }

                                if (Repeated != false)
                                {
                                    Console.WriteLine("\n\t\t\t\t<!>This book already exists please enter a new book name<!>");
                                    Repeated = true;
                                }

                            } while (Repeated != false);

                            Books[Location] = ((Books[Location].BookID, BookName: NewBookName, Books[Location].BookAuthor, Books[Location].BookQuantity, Books[Location].Borrowed, Books[Location].Price, Books[Location].Category, Books[Location].BorrowPeriod));
                            Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Location].BookName}  Author: {Books[Location].BookAuthor}  ID: {Books[Location].BookID}  x{Books[Location].BookQuantity}  Issues Borrowed: {Books[Location].Borrowed}\n ");
                            SaveBooksToFile();
                        }

                        break;


                    //Editing author name 
                    case 2:
                        int Position = GetInformation();
                        if (Position != -1)
                        {
                            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   EDIT AUTHOR NAME:\n");
                            Console.Write("\n\t\t\t\t\t\tNew author name: ");
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
                            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   EDIT BOOK QUANTITY:\n");
                            Console.Write("\n\t\t\t\t\t\tHow many would you like to add: ");
                            int Add = 0;

                            try
                            {
                                Add = int.Parse(Console.ReadLine());
                            }
                            catch (Exception ex) { Console.WriteLine(ex.Message); }

                            //Checking the positive number inputted so that books aren't minused 
                            if (Add > 0)
                            {
                                Add = Books[Index].BookQuantity + Add;
                                Books[Index] = ((Books[Index].BookID, Books[Index].BookName, Books[Index].BookAuthor, BookQuantity: Add, Books[Index].Borrowed, Books[Index].Price, Books[Index].Category, Books[Index].BorrowPeriod));
                                Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Index].BookName}  Author: {Books[Index].BookAuthor}  ID: {Books[Index].BookID}  x{Books[Index].BookQuantity}  Issues Borrowed: {Books[Index].Borrowed}\n ");
                                SaveBooksToFile();
                            }
                            else { Console.WriteLine("\t\t\t\t<!>Improper input please input a number greater than 0 :( <!>"); }
                        }
                        break;


                    case 4:
                        SaveBooksToFile();
                        break;


                    default:
                        Console.WriteLine("\t\t\t\t<!>Improper input, please choose one of the given options :( <!>");
                        break;
                }
            } while (ChooseOption != false);


        }


        //DELETE BOOKS 
        static void DeleteBook()
        {
            Console.Clear();
            Console.WriteLine("\n\n- - - - - - - - - - - - - - - - - - - - - - - -C I T Y   L I B R A R Y- - - - - - - - - - - - - - - - - - - - - - - - -\n\n");
            Console.WriteLine("\n\n\n\n\t\t\t\t\t\t   DELETE A BOOK:\n");
            ViewAllBooks();

            int DeleteIndex = GetInformation(); 
            
            if (DeleteIndex !=-1) 
            {
                if (Books[DeleteIndex].Borrowed > 0)  //Book currently borrowed 
                {
                    Console.WriteLine("\n\t\t\t\t<!>Sorry you can't delete this book as someone currently has it borrowed :( <!>\n");
                    for (int i = 0; i < Borrowing.Count; i++)
                    {
                        if (Books[DeleteIndex].BookID == Borrowing[i].BookID)
                        {
                            Console.WriteLine($"User {Borrowing[i].UserID} is currently borrowing this book \nThey should return the book by {Borrowing[i].ReturnBy} ");
                        }
                    }

                }

                else
                {
                    Console.WriteLine($"DELETING: Name: {Books[DeleteIndex].BookName}  Author: {Books[DeleteIndex].BookAuthor}  ID: {Books[DeleteIndex].BookID}  x{Books[DeleteIndex].BookQuantity}  Issues Borrowed: {Books[DeleteIndex].Borrowed} ");
                    Console.WriteLine("\n\t\t\t\t\t\tTo delete press X:");
                    string Delete = Console.ReadLine().ToLower();

                    if (Delete != "x")
                    { Console.WriteLine("\n\t\t\t\t\t\tThe book was not deleted :)"); }
                    else
                    {
                        Books.Remove((Books[DeleteIndex] = (Books[DeleteIndex].BookID, Books[DeleteIndex].BookName, Books[DeleteIndex].BookAuthor, Books[DeleteIndex].BookQuantity, Books[DeleteIndex].Borrowed, Books[DeleteIndex].Price, Books[DeleteIndex].Category, Books[DeleteIndex].BorrowPeriod)));
                        Console.WriteLine("\n\t\t\t\t\t\tThe book was deleted sucessfully :)");
                    }
                    SaveBooksToFile();
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

            //Number of categories
            Console.WriteLine("Number of Available Categories: " + Categories.Count);

            //Category information 
            var CategoriesTable = new DataTable("Categories");
            CategoriesTable.Columns.Add("ID", typeof(int));
            CategoriesTable.Columns.Add("Name", typeof(string));
            CategoriesTable.Columns.Add("No. Books", typeof(int));

            for (int i = 0; i < Categories.Count; i++)
            {
                CategoriesTable.Rows.Add(Categories[i].CategoryID, Categories[i].CategoryName, Categories[i].NoOfBooks);
            }

            foreach (DataColumn column in CategoriesTable.Columns)
            {
                Console.Write($"{column.ColumnName,-25}");
            }
            Console.WriteLine();


            foreach (DataRow row in CategoriesTable.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    Console.Write($"{item,-25}");
                }
                Console.WriteLine();
            }
            Console.WriteLine();



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




        //- - - - - - - - - - - - - - - - - - - - CUTE DRAWINGS :)  - - - - - - - - - - - - - - - - - - //
    
         static void PrintBear()
        {
            string multilineString = @"
             
                                     ()=()   ()-()   ()=()   ()-()
                                     ('Y')   (':')   (^;^)   ('&')
                                     q . p   d . b   C   C   c . c
                                     ()_()   ()_()   ()_()   ()=()
            ";
            Console.WriteLine(multilineString); 
        }

        static void PrintPie()
        {
            string multilineString = @"
                                                         (
                                                          )
                                                     __..---..__
                                                 ,-='  /  |  \  `=-.
                                                :--..___________..--;
                                                 \.,_____________,./
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintScroll()
        {
            string multilineString = @"
           
                                            (\
                                            \'\
                                             \'\     __________
                                             / '|   ()_________)
                                             \ '/    \ ~~~~~~~~ \
                                               \       \ ~~~~~~   \
                                               ==).      \__________\
                                              (__)       ()__________)
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintComputer()
        {
            string multilineString = @"

                                                    .----.
                                        .---------. | == |
                                        |  .--.   | |----|
                                        ||       || | == |
                                        ||       || |----|
                                        |'-.....-'| |::::|
                                        `   )---(  ` |___.|
                                        /:::::::::::\"" _  ""
                                    /:::=======:::\`\`\
                                `""""""""""""""""""""""""""`  '-'
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintPerson()
        {
            string multilineString = @"

                                                 ,,,
                                                (o o)
                                        ----oOO--( )--OOo----
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintSherlock()
        {
            string multilineString = @"
                                                       ,_
                                                     ,'  `\,_
                                                     |_,-'_)
                                                     /##c '\  (
                                                    ' |'  -{.  )
                                                      /\__-' \[]
                                                     /`-_`\
                                                     '     \
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintMoon()
        {
            string multilineString = @"
                                                 ,-,-.
                                                /.( +.\
                                                \ {. */
                                                 `-`-'
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintWindowsLogo()
        {
            string multilineString = @"
                   
                                                         _.-;;-._
                                                  '-..-'|   ||   |
                                                  '-..-'|_.-;;-._|
                                                  '-..-'|   ||   |
                                                  '-..-'|_.-''-._|
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintPacMan()
        {
            string multilineString = @"
                                                  __        ___
                                                 / o\      /o o\
                                                |   <      |   |
                                                 \__/      |,,,|
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintBooks()
        {
            string multilineString = @"
               
                                                          _ _
                                                     .-. | | |
                                                     |M|_|A|N|
                                                     |A|a|.|.|<\
                                                     |T|r| | | \\
                                                     |H|t|M|Z|  \\      
                                                     | |!| | |   \>     
                                                    """"""""""""""""""
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintCastle()
        {
            string multilineString = @"
           
                            |>>>                                                      |>>>
                            |                     |>>>          |>>>                  |
                            *                     |             |                     *
                           / \                    *             *                    / \
                          /___\                 _/ \           / \_                 /___\
                          [   ]                |/   \_________/   \|                [   ]
                          [ I ]                /     \       /     \                [ I ]
                          [   ]_ _ _          /       \     /       \          _ _ _[   ]
                          [   ] U U |        {#########}   {#########}        | U U [   ]
                          [   ]====/          \=======/     \=======/          \====[   ]
                          [   ]    |           |   I |_ _ _ _| I   |           |    [   ]
                          [___]    |_ _ _ _ _ _|     | U U U |     |_ _ _ _ _ _|    [___]
                          \===/  I | U U U U U |     |=======|     | U U U U U | I  \===/
                           \=/     |===========| I   | + W + |   I |===========|     \=/
                            |  I   |           |     |_______|     |           |   I  |
                            |      |           |     |||||||||     |           |      |
                            |      |           |   I ||vvvvv|| I   |           |      |
                        _-_-|______|-----------|_____||     ||_____|-----------|______|-_-_
                           /________\         /______||     ||______\         /________\
                          |__________|-------|________\_____/________|-------|__________|
            ";
            Console.WriteLine(multilineString);
        }

        static void PrintSpaceShip()
        {
            string multilineString = @"

        
                                                         ___
                                                     ___/   \___
                                                    /   '---'   \
                                                    '--_______--'
                                                         / \
                                                        /   \
                                                        /\O/\
                                                        / | \
                                                        // \\


        ";
            Console.WriteLine(multilineString);
        }

        static void PrintGrimReaper()
        { 
            string multilineString = @"

                                                         ___          
                                                        /   \\        
                                                   /\\ | . . \\       
                 Answer correctly to enter       ////\\|     ||       
                                               ////   \\ ___//\       
                                              ///      \\      \      
                                             ///       |\\      |     
                                            //         | \\  \   \    
                                            /          |  \\  \   \   
                                                       |   \\ /   /   
                                                       |    \/   /    
                                                       |     \\/|     
                                                       |      \\|     
                                                       |       \\     
                                                       |        |     
                                                       |_________\ 
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintFish()
        {
            string multilineString = @"

                                                            O  o
                                                          _\_   o
                                                >('>   \\/  o\ .  Bye! 
                                                       //\___=      
                                                          ''       
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintAlien()
        {
            string multilineString = @"

                   
                                                                      .-.
                                                       .-""`""-.    |(@ @)
                                                    _/`oOoOoOoOo`\_ \ \-/  Bye!
                                                   '.-=-=-=-=-=-=-.' \/ \
                                                     `-=.=-.-=.=-'    \ /\
                                                        ^  ^  ^       _H_ \ 
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintMonkey()
        {
            string multilineString = @"

                                                          __
                                                     w  c(..)o   (
                                          Bye!        \__(-)    __)
                                                          /\   (
                                                         /(_)___)
                                                         w /|
                                                          | \
                                                         m  m  
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintDucks()
        {
            string multilineString = @"

                                          Thank you!  >(.)__ <(.)__ =(.)__
                                                       (___/  (___/  (___/  
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintPenguin()
        {
            string multilineString = @"



                                                             __
                                                            ( o>  User Created!
                                                            ///\
                                                            \V_/_   



        ";
            Console.WriteLine(multilineString);
        }

        static void PrintDino()
        {
            string multilineString = @"
                                                              __
                                                              / _)  Admin created!
                                                     _.----._/ /
                                                    /         /
                                                 __/ (  | (  |
                                                /__.-'|_|--|_|     
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintOwl()
        {
            string multilineString = @"

                                                             ^ ^
                                                            (O,O)
                                                            (   )
                                                        ----""-""-----
                   
        ";
            Console.WriteLine(multilineString);
        }

        static void PrintCrown()
        {
            string multilineString = @"

                                               .       |        .    .
                                         .  *         -*-        *
                                              \        |        /   .
                                . .            .      /^\     .              .    .
                                *    |\   /\    /\  / / \ \  /\    /\   /|    *
                              .   .  |  \ \/ /\ \ / /     \ \ / /\ \/ /  | .     .
                                      \ | _ _\/_ _ \_\_ _ /_/_ _\/_ _ \_/
                                        \  *  *  *   \ \/ /  *  *  *  /
                                         ` ~ ~ ~ ~ ~  ~\/~ ~ ~ ~ ~ ~ '                                                           
                   
        ";
            Console.WriteLine(multilineString);
        }

    }
}
