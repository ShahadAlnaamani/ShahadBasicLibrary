using System.Formats.Asn1;
using System.Net;
using System.Text;

namespace BasicLibrary
{
    internal class Program
    {
        static List<(string BookName, string BookAuthor, int BookID, int BookQuantity)> Books = new List<(string BookName, string BookAuthor, int BookID, int BookQuantity)>();
        static string filePath = "C:\\Users\\Codeline user\\Desktop\\Projects\\BasicLibrary\\BookRecords";

        //Test checkout 
        static void Main(string[] args)
        { 
            Books.Clear(); // empties list so that there are no repititions
            LoadBooksFromFile();

            bool ExitFlag = false;
            do
            {
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("OPTIONS ");
                Console.WriteLine(" 1. Reader");
                Console.WriteLine(" 2. Librarian");
                Console.WriteLine(" 3. Exit\n");
                int UserIdentity = 0;
                Console.WriteLine("Enter: ");
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
                        break;
                    default:
                        Console.WriteLine("Invalid input :( \nPlease try again, enter one of the given options.");
                        break;
                }
            }while (!ExitFlag);
        }


        static void LeaveLibrary(bool ExitFlag)
        {
            Console.WriteLine("Are you sure you want to leave? Yes or No.");

            string Leave = (Console.ReadLine()).ToLower();

            if (Leave != "no")
            {
                ExitFlag = true;
                Console.WriteLine("Thank you for visiting the library :)\n Come again soon!");
            }

            //save to file 
        }

        //ADMIN PAGE  
        static void AdminPage()
        {
            bool ExitFlag = false;
            do
            {
                Console.Clear();
                Console.WriteLine("- - - - - -  - - - -C I T Y   L I B R A R Y- - - - - - - - - - \n\n");
                Console.WriteLine("OPTIONS:");
                Console.WriteLine("\n A- Add New Book");
                Console.WriteLine("\n B- Display All Books");
                Console.WriteLine("\n C- Search for Book by Name");
                Console.WriteLine("\n D- Save and Exit\n");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "A":
                        AddnNewBook();
                        break;

                    case "B":
                        ViewAllBooks();
                        break;

                    case "C":
                        SearchForBook();
                        break;

                    case "D":
                        SaveBooksToFile();
                        ExitFlag = true;
                        break;

                    default:
                        Console.WriteLine("Sorry your choice was wrong");
                        break;


                        Console.WriteLine("press any key to continue");
                        string cont = Console.ReadLine();

                        Console.Clear();

                } 


            } while (ExitFlag != true);

        }


        //USER PAGE  
        static void UserPage()
        { }


        static void AddnNewBook() 
        { 
                 Console.WriteLine("Enter Book Name");
                 string Name = Console.ReadLine();   

                 Console.WriteLine("Enter Book Author");
                 string Author= Console.ReadLine();  

                 Console.WriteLine("Enter Book ID");
                 int ID = int.Parse(Console.ReadLine());

                 Console.WriteLine("Enter Book ID");
                 int Qty = int.Parse(Console.ReadLine());

                 Books.Add(  (Name, Author, ID, Qty )  );
                 Console.WriteLine("Book Added Succefully");

        }

        //DISPLAYS ALL BOOKS
        static void ViewAllBooks()
        {
            StringBuilder sb = new StringBuilder();

            int BookNumber = 0;

            for (int i = 0; i < Books.Count; i++)
            {             
                BookNumber = i + 1;
                sb.Append("Book ").Append(BookNumber).Append(" name : ").Append(Books[i].BookName);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" Author : ").Append(Books[i].BookAuthor);
                sb.AppendLine();
                sb.Append("Book ").Append(BookNumber).Append(" ID : ").Append(Books[i].BookID);
                sb.AppendLine().AppendLine();
                Console.WriteLine(sb.ToString());
                sb.Clear();

            }
        }

        //ALLOWS USER TO SEARCH FOR BOOK
        static void SearchForBook()
        {
            Console.WriteLine("Enter the book name you want");
            string name = Console.ReadLine();  
            bool flag=false;

            for(int i = 0; i< Books.Count;i++)
            {
                if (Books[i].BookName == name)
                {
                    Console.WriteLine("Book Author is : " + Books[i].BookAuthor);
                    flag = true;
                    break;
                }
            }

            if (flag != true)
            { Console.WriteLine("book not found"); }
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
                            if (parts.Length == 3)
                            {
                                Books.Add((parts[0], parts[1], int.Parse(parts[2]), int.Parse(parts[3])));
                            }
                        }
                    }
                    Console.WriteLine("Books loaded from file successfully.");
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
                        writer.WriteLine($"{book.BookName}|{book.BookAuthor}|{book.BookID}|{book.BookQuantity}");
                    }
                }
                Console.WriteLine("Books saved to file successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving to file: {ex.Message}");
            }
        }

    }
}
