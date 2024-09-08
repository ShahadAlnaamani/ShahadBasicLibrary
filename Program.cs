using System;
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
        static List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed)> Books = new List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed) >();

        //Info saved -> BookTitle|Author|ID|Quantity|Borrowed
        static string filePath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\BookRecords";

        static void Main(string[] args)
        { 
            Books.Clear(); // empties list so that there are no repititions
            LoadBooksFromFile();

            bool ExitFlag = false;
            do
            {
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("OPTIONS: ");
                Console.WriteLine(" 1. Reader");
                Console.WriteLine(" 2. Librarian");
                Console.WriteLine(" 3. Exit\n");
                int UserIdentity = 0;
                Console.Write("Enter: ");
                UserIdentity = int.Parse(Console.ReadLine());

                switch (UserIdentity)
                {
                    case 1:
                        UserPage();
                        break;
                    case 2:
                        AdminPage();
                        break;
                    case 3:
                        LeaveLibrary(ExitFlag);
                        ExitFlag = true;
                        break;
                    default:
                        Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options.");
                        break;
                }
            }while (!ExitFlag);
        }






        //- - - - - - - - - - - - - FUNCTIONS SHARED BETWEEN ADMIN AND USER - - - - - - - - - - - - - - - //
        //RETRIEVES BOOK DATA FROM FILE 
        static void LoadBooksFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    using (StreamReader reader = new StreamReader(filePath))
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
            Console.WriteLine("Are you sure you want to leave? Yes or No.");
            string Leave = (Console.ReadLine()).ToLower();

            if (Leave != "no")
            {
                SaveBooksToFile();
                ExitFlag = true;
                Console.Clear();
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n\n");
                Console.WriteLine("Thank you for visiting the library :) \nCome again soon!");
                Console.WriteLine("Press any key to leave.");
                Console.ReadKey();
            }

            //save to file 
        }


        //DISPLAYS ALL BOOKS
        static void ViewAllBooks()
        {
            Console.Clear();
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.Write("\n\t\tAVAILABLE BOOKS:\n\n ");
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
                using (StreamWriter writer = new StreamWriter(filePath))
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

        //USER PAGE  
        static void UserPage()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("READER OPTIONS:");
                Console.WriteLine(" 1. View All Books");
                Console.WriteLine(" 2. Borrow A Book");
                Console.WriteLine(" 3. Return A Book");
                Console.WriteLine(" 4. Exit\n");
                Console.Write("Enter: ");
                int choice = int.Parse(Console.ReadLine());

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
                        Console.WriteLine("Exiting...");
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

                Console.Write("\n\t\tBORROWING A BOOK:\n\n ");
                Console.Write("Enter ID: ");
                int BorrowID = int.Parse(Console.ReadLine());
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

                            //Printing recipt 
                            DateTime Now = DateTime.Now;
                            Console.Clear();
                            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
                            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n");
                            Console.WriteLine("\t\t" + Now);
                            Console.WriteLine("\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine($"BOOK: ID - {Books[Location].BookID} \nNAME - {Books[Location].BookName} \nAUTHOR - {Books[Location].BookAuthor}");
                            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
                            Console.WriteLine("Thank you for visiting the library come again soon!");
                            Console.WriteLine("\t\tHappy Reading :)");
                            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");

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
            Console.Write("\n\t\tRETURN A BOOK:\n\n ");
            Console.Write("Enter Book ID: ");
            int ReturnBook = int.Parse(Console.ReadLine());


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
                    break;
                }
            }


        }


        //PRINT RETRUN RECIPT
        static void ReturnRecipt(int i)
        {
            DateTime Now = DateTime.Now;
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n");
            Console.WriteLine("\t\t Returned: " + Now);
            Console.WriteLine("\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"BOOK: \nID - {Books[i].BookID} \nNAME - {Books[i].BookName} \nAUTHOR - {Books[i].BookAuthor}");
            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"Thank you for returning {Books[i].BookName} :)\n\n");
            Console.WriteLine("\t\tCome again soon!");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");

        }






        //- - - - - - - - - - - - - - - - - - - - ADMIN FUNCTIONS  - - - - - - - - - - - - - - - - - - //
        //ADMIN PAGE  
        static void AdminPage()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("LIBRARIAN OPTIONS:");
                Console.WriteLine(" 1. Add New Book");
                Console.WriteLine(" 2. Display All Books");
                Console.WriteLine(" 3. Search for Book by Name");
                Console.WriteLine(" 4. Edit Book");
                Console.WriteLine(" 5. Delete Book");
                Console.WriteLine(" 6. Show Reports");
                Console.WriteLine(" 7. Save and Exit\n");
                Console.Write("Enter: ");
                int choice = int.Parse(Console.ReadLine());

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
            Console.Write("\n\t\tADDING NEW BOOK:\n\n ");
            Console.Write("Enter Book Name: ");
            string Name = Console.ReadLine().Trim(); //trim added for more accurate search  

            Console.Write("Enter Book Author: ");
            string Author= Console.ReadLine().Trim();  

            Console.Write("Enter Book ID: ");
            int ID = int.Parse(Console.ReadLine());

            Console.Write("Enter Book Quantity: ");
            int Qty = int.Parse(Console.ReadLine());

            Console.WriteLine("\n");
            Books.Add(  (Name, Author, ID, Qty, 0 )  );
            SaveBooksToFile();
        }


        //ALLOWS USER TO SEARCH FOR BOOK
        static void SearchForBook()
        {
            Console.Write("\n\t\tSEARCH LIBRARY:\n\n ");
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
            Console.Clear();
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
            Console.WriteLine("\n\t\tEDIT BOOKS:\n\n");
            Console.WriteLine(" 1. Edit Book Title");
            Console.WriteLine(" 2. Edit Author Name");
            Console.WriteLine(" 3. Add More Copies of Available Books");
            Console.WriteLine(" 4. Save and exit\n");
            Console.Write("Enter Option:");
            int Choice = int.Parse(Console.ReadLine());

            Console.Clear();
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");

            switch (Choice)
            {
                //Editing book title
                case 1:
                    int Location = GetInformation();

                    Console.WriteLine("\n\n\t\tEDIT BOOK TITLE:\n");
                    Console.Write("\nNew book name: ");
                    string NewBookName = Console.ReadLine();
                    Books[Location] = (BookName: NewBookName, Books[Location].BookAuthor, Books[Location].BookID, Books[Location].BookQuantity, Books[Location].Borrowed);
                    Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Location].BookName}  Author: {Books[Location].BookAuthor}  ID: {Books[Location].BookID}  x{Books[Location].BookQuantity}  Issues Borrowed: {Books[Location].Borrowed}\n ");
                    SaveBooksToFile();
                    break;

                
                //Editing author name 
                case 2:
                    int Position = GetInformation();
                    Console.WriteLine("\n\n\t\tEDIT AUTHOR NAME:\n");
                    Console.Write("\nNew author name: ");
                    string NewAuthName = Console.ReadLine();
                    Books[Position] = (Books[Position].BookName, BookAuthor: NewAuthName, Books[Position].BookID, Books[Position].BookQuantity, Books[Position].Borrowed);
                    Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Position].BookName}  Author: {Books[Position].BookAuthor}  ID: {Books[Position].BookID}  x{Books[Position].BookQuantity}  Issues Borrowed: {Books[Position].Borrowed}\n ");
                    SaveBooksToFile();
                    break;


                //Adding book copies 
                case 3:
                    int Index = GetInformation();
                    Console.WriteLine("\n\n\t\tEDIT BOOK QUANTITY:\n");
                    Console.Write("\nHow many would you like to add: ");
                    int Add = int.Parse(Console.ReadLine());

                    //Checking the positive number inputted so that books aren't minused 
                    if (Add > 0)
                    {
                        Add = Books[Index].BookQuantity + Add;
                        Books[Index] = (Books[Index].BookName, Books[Index].BookAuthor, Books[Index].BookID, BookQuantity: Add, Books[Index].Borrowed);
                        Console.WriteLine($"\n\nUPDATED DETAILS:  \nName: {Books[Index].BookName}  Author: {Books[Index].BookAuthor}  ID: {Books[Index].BookID}  x{Books[Index].BookQuantity}  Issues Borrowed: {Books[Index].Borrowed}\n ");
                        SaveBooksToFile();
                    }
                    else { Console.WriteLine("Improper input please input a number greater than 0 :("); }
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
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
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
            ViewAllBooks();
            Console.Write("Enter ID: ");
            int ChangeID = int.Parse(Console.ReadLine());


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
            else { return -1;  }
        }


        //SHOWS STATISTICS ON BORROWED AND AVAILABLE BOOKS 
        static public void Reports()
        {
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
            }
            Console.WriteLine("- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -");
        }

    }
}
