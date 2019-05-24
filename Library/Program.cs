using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using System.Data.Common;
using System.Configuration;
using System.Data;

namespace Library
{

    
    class Program
    {

        static void Main(string[] args)
        {
            string title, author, name, surname;
            int bookId, userOption, memberId, lendStatus, bookStatus, menuOption;
            double value, newValue = 0.0;
            bool flag = false;
            DateTime joinDate, tempDate;
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-588CTKD;Initial Catalog=Library_01;Integrated Security=True;Pooling=False");

            void OpenConnection()
            {
                connection.Open();
            }
            void CloseConnection()
            {
                connection.Close();
            }

            int GetBookId()
            {
                Console.Clear();
                Console.WriteLine("Enter the ID of the book :");
                try
                {
                    bookId = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                    Console.ReadKey();
                    GetBookId();
                }
                OpenConnection();
                SqlCommand checkId = new SqlCommand("SELECT Id from Books where Id='" + bookId + "'", connection);
                SqlDataReader reader = checkId.ExecuteReader();
                if (reader.Read())
                {
                    flag = true;
                }
                reader.Close();
                if (flag == false)
                {
                    Console.WriteLine("There is no book with that Id. Press ENTER to reset.");
                    Console.ReadLine();
                    CloseConnection();
                    GetBookId();
                }
                CloseConnection();
                return bookId;
            }
            int GetMemberId()
            {
                Console.Clear();
                Console.WriteLine("Enter the ID of the member :");
                try
                {
                    memberId = Convert.ToInt32(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                    Console.ReadKey();
                    GetMemberId();
                }
                OpenConnection();
                SqlCommand checkId = new SqlCommand("SELECT Id from Members where Id='" + memberId + "'", connection);
                SqlDataReader reader = checkId.ExecuteReader();
                if (reader.Read())
                {
                    flag = true;
                }
                reader.Close();
                if (flag == false)
                {
                    Console.WriteLine("There is no member with that Id. Press ENTER to reset.");
                    Console.ReadLine();
                    CloseConnection();
                    GetMemberId();
                }
                CloseConnection();
                return memberId;
            }

            void AddBook()
            {
                Int32 newBookId = 0;

                Console.WriteLine("Enter the title of the book :");
                title = Console.ReadLine();
                Console.WriteLine("Enter the author of the bool :");
                author = Console.ReadLine();
                Console.WriteLine("Enter the value of the book :");
                try
                {
                    value = Convert.ToDouble(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                    Console.ReadKey();
                    AddBook();
                }
                OpenConnection();
                SqlCommand command = new SqlCommand("insert into Books (Title, Author, Value) values('" + title + "', '" + author + "', '" + value + "')" + "SELECT CAST (scope_identity() AS int)", connection);
                newBookId = (int)command.ExecuteScalar();

                if (newBookId != 0)
                {
                    Console.WriteLine("Insertion successfull");
                    Console.WriteLine("Your new book Id is : " + newBookId);
                    tempDate = DateTime.Now;
                    SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (BookID, BookTitle, BookAuthor, DateTime, Type) VALUES ('" + newBookId + "', '" + title + "', '" + author + "', @value , 'Book Adition')", connection);
                    historyUpdate.Parameters.AddWithValue("@value", tempDate);
                    historyUpdate.ExecuteNonQuery();
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();

            }

            int CheckLendStatus1()
            {
                lendStatus = 0;
                Console.Clear();
                    SqlCommand command = new SqlCommand("SELECT BorrowedBook1ID from Members where Id= " + memberId + " ", connection);
                    OpenConnection();
                    var queryResult = command.ExecuteScalar();
                    CloseConnection();

                    if (queryResult is DBNull)
                    {
                        lendStatus = 0;
                    }
                    else
                    {
                        lendStatus = 1;
                    }

                return lendStatus;
            }
            int CheckLendStatus2()
            {
                lendStatus = 0;
                Console.Clear();
                SqlCommand command = new SqlCommand("SELECT BorrowedBook2ID from Members where Id= " + memberId + " ", connection);
                OpenConnection();
                var queryResult = command.ExecuteScalar();
                CloseConnection();

                if (queryResult is DBNull)
                {
                    lendStatus = 0;
                }
                else
                {
                    lendStatus = 1;
                }

                return lendStatus;
            }
            int CheckLendStatus3()
            {
                lendStatus = 0;
                Console.Clear();
                SqlCommand command = new SqlCommand("SELECT BorrowedBook3ID from Members where Id= " + memberId + " ", connection);
                OpenConnection();
                var queryResult = command.ExecuteScalar();
                CloseConnection();

                if (queryResult is DBNull)
                {
                    lendStatus = 0;
                }
                else
                {
                    lendStatus = 1;
                }

                return lendStatus;
            }
            int CheckBookStatus()
             {
                Console.Clear();
                GetBookId();
                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT Status from Books where Id= " + bookId + " ", connection);
                var queryResult = command.ExecuteScalar();
                CloseConnection();
                if (Convert.ToInt32(queryResult) == 1)
                {
                     bookStatus = 1;
                }
                else
                {
                    bookStatus = 0;
                }
                return bookStatus;
             }

            void LendBook()
            {
                Console.Clear();
                GetMemberId();
                if(CheckLendStatus1() == 0)
                {

                    if (CheckBookStatus() == 0)
                    {
                        Console.WriteLine("Book is not available.");
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                    else
                    {
                        OpenConnection();
                        SqlCommand command1 = new SqlCommand("UPDATE Members SET BorrowedBook1ID = '" + bookId + "' WHERE Id = " + memberId + " ", connection);
                        command1.ExecuteNonQuery();
                        CloseConnection();
                        OpenConnection();
                        SqlCommand command2 = new SqlCommand("UPDATE Books SET Status = " + 0 + " WHERE Id = " + bookId + " ", connection);
                        int i = command2.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Console.WriteLine("Lend successfull");
                            tempDate = DateTime.Now;
                            SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, BookID, DateTime, Type) VALUES ('" + memberId + "', '" + bookId + "', @value , 'Book Lending')", connection);
                            historyUpdate.Parameters.AddWithValue("@value", tempDate);
                            historyUpdate.ExecuteNonQuery();
                        }
                        CloseConnection();
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                }
                else if (CheckLendStatus2() == 0)
                {
                    if (CheckBookStatus() == 0)
                    {
                        Console.WriteLine("Book is not available.");
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                    else
                    {
                        OpenConnection();
                        SqlCommand command1 = new SqlCommand("UPDATE Members SET BorrowedBook2ID = '" + bookId + "' WHERE Id = " + memberId + " ", connection);
                        command1.ExecuteNonQuery();
                        CloseConnection();
                        OpenConnection();
                        SqlCommand command2 = new SqlCommand("UPDATE Books SET Status = " + 0 + " WHERE Id = " + bookId + " ", connection);
                        int i = command2.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Console.WriteLine("Lend successfull");
                            tempDate = DateTime.Now;
                            SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, BookID, DateTime, Type) VALUES ('" + memberId + "', '" + bookId + "', @value , 'Book Lending')", connection);
                            historyUpdate.Parameters.AddWithValue("@value", tempDate);
                            historyUpdate.ExecuteNonQuery();
                        }
                        CloseConnection();
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                }
                else if (CheckLendStatus3() == 0)
                {
                    if (CheckBookStatus() == 0)
                    {
                        Console.WriteLine("Book is not available.");
                        Console.WriteLine("");
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                    else
                    {
                        OpenConnection();
                        SqlCommand command1 = new SqlCommand("UPDATE Members SET BorrowedBook1ID = '" + bookId + "' WHERE Id = " + memberId + " ", connection);
                        command1.ExecuteNonQuery();
                        CloseConnection();
                        OpenConnection();
                        SqlCommand command2 = new SqlCommand("UPDATE Books SET Status = " + 0 + " WHERE Id = " + bookId + " ", connection);
                        int i = command2.ExecuteNonQuery();
                        if (i > 0)
                        {
                            Console.WriteLine("Lend successfull");
                            tempDate = DateTime.Now;
                            SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, BookID, DateTime, Type) VALUES ('" + memberId + "', '" + bookId + "', @value , 'Book Lending')", connection);
                            historyUpdate.Parameters.AddWithValue("@value", tempDate);
                            historyUpdate.ExecuteNonQuery();
                        }
                        CloseConnection();
                        Console.WriteLine("Press ENTER to return to main menu...");
                        Console.ReadLine();
                        Menu();
                    }
                }
                else
                {
                    Console.WriteLine("User has lended too many books. Please return one of them before lending another.");
                    Console.WriteLine("");
                    Console.WriteLine("Press ENTER to return to main menu...");
                    Console.ReadLine();
                    //Menu();
                }
                
            }
            
            void ReturnBook()
            {
               GetMemberId();
               if( CheckBookStatus() == 0)
               { 
                    OpenConnection();
                    SqlCommand command1 = new SqlCommand("UPDATE Books SET Status = " + 1 + " WHERE Id = " + bookId + " ", connection);
                    int i =command1.ExecuteNonQuery();
                    CloseConnection();
                    OpenConnection();
                    SqlCommand command2 = new SqlCommand("UPDATE Members SET BorrowedBook1ID = NULL WHERE Id = " + memberId + " AND BorrowedBook1ID = " + bookId + " ", connection);
                    int j = command2.ExecuteNonQuery();
                    SqlCommand command3 = new SqlCommand("UPDATE Members SET BorrowedBook2ID = NULL WHERE Id = " + memberId + " AND BorrowedBook2ID = " + bookId + " ", connection);
                    int k = command3.ExecuteNonQuery();
                    SqlCommand command4 = new SqlCommand("UPDATE Members SET BorrowedBook3ID = NULL WHERE Id = " + memberId + " AND BorrowedBook3ID = " + bookId + " ", connection);
                    int l = command4.ExecuteNonQuery();
                    
                    if (i > 0 && (j > 0 || k > 0 || l > 0))
                    {
                        Console.WriteLine("Return successfull");
                        tempDate = DateTime.Now;
                        SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, BookID, DateTime, Type) VALUES ('" + memberId + "', '" + bookId + "', @value , 'Book Returned')", connection);
                        historyUpdate.Parameters.AddWithValue("@value", tempDate);
                        historyUpdate.ExecuteNonQuery();
                    }
                    CloseConnection();
                    Console.WriteLine("Press ENTER to return to main menu...");
                    Console.ReadLine();
                    Menu();

               }
               else
               {
                    Console.WriteLine("Selected book is not lended.");
                    Console.WriteLine("Press ENTER to return to main menu...");
                    Console.ReadLine();
                    Menu();
               }
            }

            void EditBook()
            {
                Console.Clear();
                GetBookId();
                Console.WriteLine("Select an option to edit :");
                Console.WriteLine("");
                Console.WriteLine("1 - Edit Title ");
                Console.WriteLine("2 - Edit Author ");
                Console.WriteLine("3 - Edit Value");
                try
                {
                    userOption = Convert.ToInt16(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                    Console.ReadKey();
                    CloseConnection();
                    EditBook();
                }

                if (userOption == 1)
                {
                    Console.WriteLine("Enter the new title :");
                    string newTitle = Console.ReadLine();
                    OpenConnection();
                    SqlCommand command = new SqlCommand("UPDATE Books SET Title = '" + newTitle + "' WHERE Id = " + bookId + " ", connection);
                    int i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        Console.WriteLine("Update successfull");
                        tempDate = DateTime.Now;
                        SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (BookID, BookTitle, DateTime, Type) VALUES ('" + bookId + "', '" + newTitle + "', @value , 'Title Modification')", connection);
                        historyUpdate.Parameters.AddWithValue("@value", tempDate);
                        historyUpdate.ExecuteNonQuery();
                    }
                    Console.WriteLine("Press ENTER to return to main menu.");
                    Console.ReadLine();
                    CloseConnection();
                    Menu();
                }
                if (userOption == 2)
                {
                    Console.WriteLine("Enter the new author :");
                    string newAuthor = Console.ReadLine();
                    OpenConnection();
                    SqlCommand command = new SqlCommand("UPDATE Books SET Author = '" + newAuthor + "' WHERE Id = " + bookId + " ", connection);
                    int i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        Console.WriteLine("Update successfull");
                        tempDate = DateTime.Now;
                        SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (BookID, BookAuthor, DateTime, Type) VALUES ('" + bookId + "', '" + newAuthor + "', @value , 'Author Modification')", connection);
                        historyUpdate.Parameters.AddWithValue("@value", tempDate);
                        historyUpdate.ExecuteNonQuery();
                    }
                    Console.WriteLine("Press ENTER to return to main menu.");
                    Console.ReadLine();
                    CloseConnection();
                    Menu();
                }
                if (userOption == 3)
                {
                    Console.WriteLine("Enter the new value :");
                    try
                    {
                        newValue = Convert.ToDouble(Console.ReadLine());
                    }
                    catch (FormatException)
                    {
                        Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                        Console.ReadKey();
                        EditBook();
                    }
                    OpenConnection();
                    SqlCommand command = new SqlCommand("UPDATE Books SET Value = '" + newValue + "' WHERE Id = " + bookId + " ", connection);

                    int i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        Console.WriteLine("Update successfull");
                        tempDate = DateTime.Now;
                        SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (BookID, Value, DateTime, Type) VALUES ('" + bookId + "', '" + newValue + "', @value , 'Value Modification')", connection);
                        historyUpdate.Parameters.AddWithValue("@value", tempDate);
                        historyUpdate.ExecuteNonQuery();
                    }
                    Console.WriteLine("Press ENTER to return to main menu.");
                    Console.ReadLine();
                    CloseConnection();
                    Menu();
                }

                if (userOption < 1 || userOption > 3)
                {
                    Console.WriteLine("Invalid selection. Press ENTER to reset.");
                    Console.ReadLine();
                    EditBook();
                }
                
            }

            void DeleteBook()
            {
                Console.Clear();
                if (CheckBookStatus() == 0)
                {
                    Console.WriteLine("Selected book is lended. It has to be returned first in order to delete it.");
                    Console.WriteLine("Press ENTER to return to main menu.");
                    Console.ReadLine();
                    Menu();
                }
                else
                {
                    OpenConnection();
                    SqlCommand command = new SqlCommand("DELETE FROM Books WHERE Id =" + bookId + " ", connection);

                    int i = command.ExecuteNonQuery();
                    if (i > 0)
                    {
                        Console.WriteLine("Deletion successfull.");
                        tempDate = DateTime.Now;
                        SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (BookID, DateTime, Type) VALUES ('" + bookId + "', @value , 'Book Deletion')", connection);
                        historyUpdate.Parameters.AddWithValue("@value", tempDate);
                        historyUpdate.ExecuteNonQuery();
                    }
                    Console.WriteLine("Press ENTER to return to main menu.");
                    Console.ReadLine();
                    CloseConnection();
                    Menu();
                }
            }

            void AddMember()
            {
                Int32 newMemberId = 0;

                Console.WriteLine("Enter the name of the member :");
                name = Console.ReadLine();
                Console.WriteLine("Enter the surname of the member :");
                surname = Console.ReadLine();
                joinDate = DateTime.Now;

                OpenConnection();
                SqlCommand command = new SqlCommand("insert into Members (Name, Surname, JoinDate) values('" + name + "', '" + surname + "', @value)" + "SELECT CAST (scope_identity() AS int)", connection);
                command.Parameters.AddWithValue("@value", joinDate);
                newMemberId = (int)command.ExecuteScalar();

                if(newMemberId != 0)
                {
                    Console.WriteLine("Insertion successfull.");
                    Console.WriteLine("Your new member Id is : " + newMemberId);
                    SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, MemberName, DateTime, Type) VALUES('" + newMemberId + "','" + name + "', @value, 'Member Adition')", connection);
                    historyUpdate.Parameters.AddWithValue("@value", joinDate);
                    historyUpdate.ExecuteNonQuery();

                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void DeleteMember() 
            {
                Console.Clear();
                GetMemberId();
                OpenConnection();
                SqlCommand command = new SqlCommand("DELETE FROM Members WHERE Id =" + memberId + " ", connection);

                int i = command.ExecuteNonQuery();
                if (i > 0)
                {
                    Console.WriteLine("Deletion successfull.");
                    tempDate = DateTime.Now;
                    SqlCommand historyUpdate = new SqlCommand("INSERT INTO History (MemberID, DateTime, Type) VALUES ('" + memberId + "', @value , 'Member Deletion')", connection);
                    historyUpdate.Parameters.AddWithValue("@value", tempDate);
                    historyUpdate.ExecuteNonQuery();
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void ListBooks()
            {
                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT * FROM Books", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t | {4,-10} ", "Book Id", "Title", "Author", "Status", "Value");
                    Console.WriteLine("");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t | {4,-10} ",reader[0], reader[1], reader[2], reader[3], reader[4]));
                    }
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void ListMembers()
            {

                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT * FROM Members", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10} \t |{7,-10} \t |{8,-10} ", "Member Id", "Member Name", "Member Surname", "CNP", "Join Date", "Leave Date", "Borrowed Book1 ID", "Borrowed Book2 ID", "Borrowed Book3 ID");
                    Console.WriteLine("");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10}\t |{7,-10}\t |{8,-10} ", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7], reader[8]));
                    }
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void ListHistory()
            {

                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT * FROM History", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10} \t |{7,-10} ", "Transaction Id", "Member ID", "Member Name", "Book ID", "Book Title", "Book Author", "Date Time", "Type");
                    Console.WriteLine("");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10}\t |{7,-10} ", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7]));
                    }
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void SearchBookById()
            {
                Console.Clear();
                GetBookId();
                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT * FROM Books WHERE Id =" + bookId + "", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t | {4,-10} ", "Book Id", "Title", "Author", "Status", "Value");
                    Console.WriteLine("");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t | {4,-10} ", reader[0], reader[1], reader[2], reader[3], reader[4]));
                    }
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void SearchMemberById() 
            {
                Console.Clear();
                GetMemberId();
                OpenConnection();
                SqlCommand command = new SqlCommand("SELECT * FROM Members WHERE Id =" + memberId + "", connection);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    Console.WriteLine("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10} \t |{7,-10} \t |{8,-10} ", "Member Id", "Member Name", "Member Surname", "CNP", "Join Date", "Leave Date", "Borrowed Book1 ID", "Borrowed Book2 ID", "Borrowed Book3 ID");
                    Console.WriteLine("");
                    while (reader.Read())
                    {
                        Console.WriteLine(string.Format("{0,-10}\t | {1,-10}\t | {2,-10}\t | {3,-10}\t\t | {4,-10}\t\t | {5,-10}\t\t |{6,-10}\t |{7,-10}\t |{8,-10} ", reader[0], reader[1], reader[2], reader[3], reader[4], reader[5], reader[6], reader[7], reader[8]));
                    }
                }
                Console.WriteLine("Press ENTER to return to main menu.");
                Console.ReadLine();
                CloseConnection();
                Menu();
            }

            void Menu()
            {
                Console.Clear();
                Console.WriteLine("*********************");
                Console.WriteLine("* Askraed's Library *");
                Console.WriteLine("*********************");
                Console.WriteLine(" 1 - Add Book");
                Console.WriteLine(" 2 - Edit Book");
                Console.WriteLine(" 3 - Delete Book");
                Console.WriteLine(" 4 - Add Member");
                Console.WriteLine(" 5 - Delete Member");
                Console.WriteLine(" 6 - Lend Book");
                Console.WriteLine(" 7 - Return Book");
                Console.WriteLine(" 8 - List Books");
                Console.WriteLine(" 9 - List Members");
                Console.WriteLine("10 - List History");
                Console.WriteLine("11 - Search Member by ID");
                Console.WriteLine("12 - Search Book by ID");
                Console.WriteLine("13 - Exit application");
                Console.WriteLine("");
                Console.WriteLine("Please select an option... ");
                try
                {
                    menuOption = Convert.ToInt16(Console.ReadLine());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Wrong input, please try again. Press any key to reset.");
                    Console.ReadKey();
                    Menu();
                }
                switch(menuOption)
                {
                    case 1:
                        AddBook();
                        break;
                    case 2:
                        EditBook();
                        break;
                    case 3:
                        DeleteBook();
                        break;
                    case 4:
                        AddMember();
                        break;
                    case 5:
                        DeleteMember();
                        break;
                    case 6:
                        LendBook();
                        break;
                    case 7:
                        ReturnBook();
                        break;
                    case 8:
                        ListBooks();
                        break;
                    case 9:
                        ListMembers();
                        break;
                    case 10:
                        ListHistory();
                        break;
                    case 11:
                        SearchMemberById();
                        break;
                    case 12:
                        SearchBookById();
                        break;
                    case 13:
                        Environment.Exit(0);
                        break;
                    default:
                        {
                            Console.WriteLine("You have entered an invalid option.");
                            Console.WriteLine("Press ENTER to return to main menu.");
                            Console.ReadLine();
                            Menu();
                        }
                        break;
                }
            }

           // Menu();
        }
    }
}
