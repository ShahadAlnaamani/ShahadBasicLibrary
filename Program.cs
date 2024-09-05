using System.Data.Common;
using System.Formats.Asn1;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed)> Books = new List<(string BookName, string BookAuthor, int BookID, int BookQuantity, int Borrowed) >();

        //Info saved -> BookTitle|Author|ID|Quantity
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

        //METHOD TO EXIT THE LIBRARY ---> need to add saving the changes before exiting 
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
                Console.WriteLine(" 4. Save and Exit\n");
                Console.Write("Enter: ");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddNewBook();
                        break;

                    case 2:
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

                    case 3:
                        SearchForBook();
                        break;

                    case 4:
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

        //PRINT RETRUN RECIPT
        static void ReturnRecipt(int i)
        {
            DateTime Now = DateTime.Now;
            Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n\n");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n");
            Console.WriteLine("\t\t Returned: " + Now);
            Console.WriteLine("\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"BOOK: ID - {Books[i].BookID} \nNAME - {Books[i].BookName} \nAUTHOR - {Books[i].BookAuthor}");
            Console.WriteLine("\n\n\n* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * \n\n");
            Console.WriteLine($"Thank you for returning {Books[i].BookName} :)\n\n");
            Console.WriteLine("\t\tCome again soon!");
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * ");

        }

    }
}
