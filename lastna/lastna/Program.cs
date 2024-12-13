using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public interface IUserInteraction
{
    void DisplayUserTypeMenu();
    void DisplayMainMenu();
    void DisplayStudentMenu();
    void HandleGuest();
    void HandleStudentRegistration();
    void HandleStudentLogin();
    void DisplayCenteredMessage(string message);
}

public abstract class BaseInteraction
{
    protected string studentFilePath;
    protected int selectedIndex = 0;

    protected void DisplayCenteredHeader(string header)
    {
        int windowWidth = Console.WindowWidth;
        int spaces = (windowWidth - header.Length) / 2;
        Console.WriteLine(new string(' ', spaces) + header);
    }

    protected void DisplayOptions(string[] options)
    {
        int windowWidth = Console.WindowWidth;
        for (int i = 0; i < options.Length; i++)
        {
            string menuLine = options[i];
            if (i == selectedIndex)
            {
                menuLine = $"> {menuLine} <";
            }

            Console.ForegroundColor = (i == selectedIndex) ? ConsoleColor.Green : ConsoleColor.DarkRed;
            Console.SetCursorPosition((windowWidth - menuLine.Length) / 2, Console.CursorTop);
            Console.WriteLine(menuLine);
        }
        Console.ResetColor();
    }
}

public class UserInteraction : BaseInteraction, IUserInteraction
{
    private string[] userTypeOptions = { "Guest", "Student" };
    private string[] studentMenuOptions = { "Register", "Login" };
    private string[] mainMenuOptions = { "Create", "Load", "Manage", "Exit" };

    public void DisplayUserTypeMenu()
    {
        selectedIndex = 0;
        while (true)
        {
            try
            {
                Console.Clear();
                DisplayCenteredHeader("User  Type Menu\n");
                DisplayOptions(userTypeOptions);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : userTypeOptions.Length - 1;
                else if (key.Key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex < userTypeOptions.Length - 1) ? selectedIndex + 1 : 0;
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (selectedIndex == 0)
                        HandleGuest();
                    else if (selectedIndex == 1)
                        DisplayStudentMenu();
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in user type menu: {ex.Message}");
            }
        }
    }

    public void DisplayMainMenu()
    {
        selectedIndex = 0;
        while (true)
        {
            try
            {
                Console.Clear();
                DisplayCenteredHeader("  Main Menu\n");
                DisplayOptions(mainMenuOptions);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : mainMenuOptions.Length - 1;
                else if (key.Key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex < mainMenuOptions.Length - 1) ? selectedIndex + 1 : 0;
                else if (key.Key == ConsoleKey.Enter)
                {
                    switch (selectedIndex)
                    {
                        case 0: CreateQuiz(); break;
                        case 1: LoadQuiz(); break;
                        case 2: ManageQuiz(); break;
                        case 3: Environment.Exit(0); break;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in main menu: {ex.Message}");
            }
        }
    }

    public void DisplayStudentMenu()
    {
        selectedIndex = 0;
        while (true)
        {
            try
            {
                Console.Clear();
                DisplayCenteredHeader("  Student Menu\n");
                DisplayOptions(studentMenuOptions);

                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.UpArrow)
                    selectedIndex = (selectedIndex > 0) ? selectedIndex - 1 : studentMenuOptions.Length - 1;
                else if (key.Key == ConsoleKey.DownArrow)
                    selectedIndex = (selectedIndex < studentMenuOptions.Length - 1) ? selectedIndex + 1 : 0;
                else if (key.Key == ConsoleKey.Enter)
                {
                    if (selectedIndex == 0) HandleStudentRegistration();
                    else if (selectedIndex == 1) HandleStudentLogin();
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in student menu: {ex.Message}");
            }
        }
    }

    public void HandleGuest()
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Guest Access\n");
            DisplayMainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling guest: {ex.Message}");
        }
    }

    public void HandleStudentRegistration()
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Student Registration\n");

            Console.Write("First Name: ");
            string firstName = Console.ReadLine().Trim();
            Console.Write("Last Name: ");
            string lastName = Console.ReadLine().Trim();

            studentFilePath = $"{lastName}_{firstName}.txt";

            if (!File.Exists(studentFilePath))
            {
                File.Create(studentFilePath).Close();
                DisplayCenteredMessage("Account created! You can now load and manage your data.");
            }
            else
            {
                DisplayCenteredMessage("This account already exists. Please log in.");
            }

            DisplayCenteredMessage("Press any key to continue...");
            Console.ReadKey();
            DisplayStudentMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during registration: {ex.Message}");
        }
    }

    public void HandleStudentLogin()
    {
        try
        {
            bool loginSuccessful = false;

            while (!loginSuccessful)
            {
                Console.Clear();
                DisplayCenteredHeader("Student Login\n");

                Console.Write("First Name: ");
                string firstName = Console.ReadLine().Trim();
                Console.Write("Last Name: ");
                string lastName = Console.ReadLine().Trim();

                studentFilePath = $"{lastName}_{firstName}.txt";

                if (File.Exists(studentFilePath))
                {
                    DisplayCenteredMessage("Login successful! Welcome.");
                    DisplayMainMenu();
                    loginSuccessful = true;
                }
                else
                {
                    DisplayCenteredMessage("No account found. Please register.");
                    Console.WriteLine("\nWould you like to try logging in again or register? (L = Login Again / R = Register)");
                    string choice = Console.ReadLine().Trim().ToUpper();

                    if (choice == "R")
                    {
                        HandleStudentRegistration();
                        break;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
        }
    }

    private void CreateQuiz()
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Word Quiz\n");

            List<QuizQuestion> questions = new List<QuizQuestion>();
            List<string> allWords = new List<string>();

            Console.Write("Enter quiz title: ");
            string quizTitle = Console.ReadLine().Trim();
            quizTitle = string.Join("_", quizTitle.Split(Path.GetInvalidFileNameChars()));
            string quizFilePath = $"{studentFilePath}_{quizTitle}.txt";

            Console.Write("Number of Questions: ");
            int numQuestions;
            while (!int.TryParse(Console.ReadLine().Trim(), out numQuestions) || numQuestions <= 0)
            {
                Console.Write("Invalid input. Enter a positive number: ");
            }

            for (int i = 0; i < numQuestions; i++)
            {
                Console.Clear();
                DisplayCenteredHeader("Word Quiz\n");
                Console.WriteLine($"Adding question {i + 1} of {numQuestions}...");

                Console.Write("Enter word: ");
                string word = Console.ReadLine().Trim();
                while (string.IsNullOrWhiteSpace(word) || allWords.Contains(word))
                {
                    Console.Write("Invalid input or duplicate word. Please enter a unique word: ");
                    word = Console.ReadLine().Trim();
                }

                Console.Write("Enter the correct meaning for the word: ");
                string correctMeaning = Console.ReadLine().Trim();
                while (string.IsNullOrWhiteSpace(correctMeaning))
                {
                    Console.Write("Invalid input. Please enter a valid meaning: ");
                    correctMeaning = Console.ReadLine().Trim();
                }

                allWords.Add(word);
                questions.Add(new QuizQuestion(word, correctMeaning));
            }

            FileHandler.SaveQuestionsToFile(questions, quizFilePath);
            Console.Clear();
            DisplayCenteredHeader("Word Quiz\n");
            Console.WriteLine($"Quiz '{quizTitle}' saved successfully to {quizFilePath}!");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();

            if (TakeQuizSelection())
            {
                int difficulty = DifficultySelection();
                StartQuiz(questions, difficulty);
            }
            else
            {
                Console.WriteLine("Quiz creation complete. Returning to the main menu.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateQuiz: {ex.Message}");
        }
    }

    private void StartQuiz(List<QuizQuestion> questions, int difficulty)
    {
        foreach (var question in questions)
        {
            string displayedWord = question.Word;
            string meaning = question.CorrectMeaning;

            if (difficulty == 1)
            {
                displayedWord = GetEasyLevelWord(displayedWord);
            }
            else if (difficulty == 2)
            {
                displayedWord = GetMediumLevelWord(displayedWord);
            }
            else if (difficulty == 3)
            {
                displayedWord = GetDifficultLevelWord(displayedWord);
            }

            Console.Clear();
            DisplayCenteredHeader("Word Quiz\n");
            Console.WriteLine($"Meaning: {meaning}");
            Console.WriteLine($"Clue: {displayedWord}");

            Console.Write("Your answer: ");
            string userAnswer = Console.ReadLine().Trim();

            if (string.Equals(userAnswer, question.Word, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("Correct!");
            }
            else
            {
                Console.WriteLine($"Incorrect! The correct word is: {question.Word}");
            }

            Console.WriteLine("Press any key to continue to the next flashcard...");
            Console.ReadKey();
        }
    }

    private bool TakeQuizSelection()
    {
        int selectedIndex = 0;
        string[] options = { "Start Quiz", "Main Menu" };
        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            DisplayCenteredHeader("Flashcard Quiz\n");
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % options.Length;
            }
        } while (key.Key != ConsoleKey.Enter);

        return selectedIndex == 0;
    }

    private int DifficultySelection()
    {
        int selectedIndex = 0;
        string[] options = { "Easy", "Medium", "Difficult" };
        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            DisplayCenteredHeader("Select Difficulty\n");
            for (int i = 0; i < options.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {options[i]}");
                }
            }

            key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex == 0) ? options.Length - 1 : selectedIndex - 1;
            }
            else if (key.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % options.Length;
            }
        } while (key.Key != ConsoleKey.Enter);

        return selectedIndex + 1;
    }

    private string GetEasyLevelWord(string word)
    {
        char[] wordChars = word.ToCharArray();
        Random random = new Random();

        for (int i = 1; i < wordChars.Length; i += 2)
        {
            wordChars[i] = '_';
        }

        return new string(wordChars);
    }

    private string GetMediumLevelWord(string word)
    {
        Random random = new Random();
        char[] wordChars = word.ToCharArray();
        wordChars = wordChars.OrderBy(c => random.Next()).ToArray();
        return new string(wordChars);
    }

    private static string GetDifficultLevelWord(string word)
    {
        return "No Clue for Difficult Level";
    }

    private void LoadQuiz()
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Load Quiz\n");

            var quizFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{studentFilePath}_*.txt");

            if (quizFiles.Length == 0)
            {
                Console.WriteLine("No quizzes found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Select a quiz to load:");
            for (int i = 0; i < quizFiles.Length; i++)
            {
                string quizTitle = Path.GetFileNameWithoutExtension(quizFiles[i]).Replace($"{studentFilePath}_", "");

                Console.WriteLine($"{i + 1}. {quizTitle}");
            }

            int selectedQuizIndex;
            while (!int.TryParse(Console.ReadLine().Trim(), out selectedQuizIndex) || selectedQuizIndex < 1 || selectedQuizIndex > quizFiles.Length)
            {
                Console.Write("Invalid choice. Please select a valid quiz number: ");
            }

            string selectedQuizFile = quizFiles[selectedQuizIndex - 1];
            List<QuizQuestion> loadedQuestions = FileHandler.LoadQuestionsFromFile(selectedQuizFile);

            if (loadedQuestions.Count == 0)
            {
                Console.WriteLine("No questions found in the selected quiz.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            int difficulty = DifficultySelection();
            StartQuiz(loadedQuestions, difficulty);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while loading the quiz: {ex.Message}");
        }
    }

    private void ManageQuiz()
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Manage Quiz\n");

            var quizFiles = Directory.GetFiles(Directory.GetCurrentDirectory(), $"{studentFilePath}_*.txt");

            if (quizFiles.Length == 0)
            {
                Console.WriteLine("No quizzes found.");
                Console.WriteLine("Press any key to return...");
                Console.ReadKey();
                return;
            }

            int selectedQuizIndex = QuizSelection(quizFiles);
            string selectedQuizFile = quizFiles[selectedQuizIndex];
            List<QuizQuestion> questions = FileHandler.LoadQuestionsFromFile(selectedQuizFile);

            int actionChoice = ActionSelection();

            switch (actionChoice)
            {
                case 1:
                    AddFlashcards(selectedQuizFile, questions);
                    break;
                case 2:
                    DeleteQuiz(selectedQuizFile);
                    break;
                case 3:
                    Console.WriteLine("Returning to main menu...");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while managing the quiz: {ex.Message}");
        }
    }

    private int QuizSelection(string[] quizFiles)
    {
        int selectedIndex = 0;
        ConsoleKeyInfo keyInfo;

        while (true)
        {
            Console.Clear();
            DisplayCenteredHeader("Manage Quiz\n");
            Console.WriteLine("Select a quiz to manage:");

            for (int i = 0; i < quizFiles.Length; i++)
            {
                string quizTitle = Path.GetFileNameWithoutExtension(quizFiles[i]).Replace($"{studentFilePath}_", "");
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> {quizTitle}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {quizTitle}");
                }
            }

            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + quizFiles.Length) % quizFiles.Length;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % quizFiles.Length;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return selectedIndex;
            }
        }
    }

    private int ActionSelection()
    {
        int selectedIndex = 0;
        ConsoleKeyInfo keyInfo;

        while (true)
        {
            Console.Clear();
            Console.WriteLine("Choose an action:");

            string[] actions = { "Add Flashcards", "Delete Quiz", "Return to Main Menu" };
            for (int i = 0; i < actions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> {actions[i]}");
                    Console.ResetColor();
                }
                else
                {
                    Console.WriteLine($"  {actions[i]}");
                }
            }

            keyInfo = Console.ReadKey(true);
            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selectedIndex = (selectedIndex - 1 + actions.Length) % actions.Length;
            }
            else if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selectedIndex = (selectedIndex + 1) % actions.Length;
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                return selectedIndex + 1;
            }
        }
    }

    private void AddFlashcards(string quizFilePath, List<QuizQuestion> questions)
    {
        try
        {
            Console.Clear();
            DisplayCenteredHeader("Add Flashcards\n");

            List<string> allWords = questions.Select(q => q.Word).ToList();

            Console.Write("How many flashcards do you want to add? ");
            int numFlashcards;
            while (!int.TryParse(Console.ReadLine().Trim(), out numFlashcards) || numFlashcards <= 0)
            {
                Console.Write("Invalid input. Enter a positive number: ");
            }

            for (int i = 0; i < numFlashcards; i++)
            {
                Console.Clear();
                DisplayCenteredHeader("Add Flashcard");
                Console.WriteLine($"Adding flashcard {i + 1} of {numFlashcards}...");

                Console.Write("Enter word: ");
                string word = Console.ReadLine().Trim();
                while (string.IsNullOrWhiteSpace(word) || allWords.Contains(word))
                {
                    Console.Write("Invalid input or duplicate word. Please enter a unique word: ");
                    word = Console.ReadLine().Trim();
                }

                Console.Write("Enter the correct meaning for the word: ");
                string correctMeaning = Console.ReadLine().Trim();
                while (string.IsNullOrWhiteSpace(correctMeaning))
                {
                    Console.Write("Invalid input. Please enter a valid meaning: ");
                    correctMeaning = Console.ReadLine().Trim();
                }

                questions.Add(new QuizQuestion(word, correctMeaning));
                allWords.Add(word);
            }

            FileHandler.SaveQuestionsToFile(questions, quizFilePath);
            Console.WriteLine("Flashcards added successfully!");
            Console.WriteLine("Press any key to return...");
            Console.ReadKey();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error while adding flashcards: {ex.Message}");
        }
    }

    private void DeleteQuiz(string quizFilePath)
    {
        try
        {
            if (File.Exists(quizFilePath))
            {
                File.Delete(quizFilePath);
                Console.WriteLine("Quiz deleted successfully!");
            }
            else
            {
                Console.WriteLine("Quiz file not found.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting quiz: {ex.Message}");
        }
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    public void DisplayCenteredMessage(string message)
    {
        int windowWidth = Console.WindowWidth;
        Console.SetCursorPosition((windowWidth - message.Length) / 2, Console.CursorTop);
        Console.WriteLine(message);
    }
}

public class QuizQuestion
{
    private string word;
    private string correctMeaning;

    public string Word
    {
        get { return word; }
    }

    public string CorrectMeaning
    {
        get { return correctMeaning; }
    }

    public QuizQuestion(string word, string correctMeaning)
    {
        this.word = word;
        this.correctMeaning = correctMeaning;
    }
}

public static class FileHandler
{
    public static void SaveQuestionsToFile(List<QuizQuestion> questions, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (var question in questions)
            {
                writer.WriteLine($"{question.Word}|{question.CorrectMeaning}");
            }
        }
    }

    public static List<QuizQuestion> LoadQuestionsFromFile(string filePath)
    {
        var questions = new List<QuizQuestion>();
        using (StreamReader reader = new StreamReader(filePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var parts = line.Split('|');
                if (parts.Length == 2)
                {
                    questions.Add(new QuizQuestion(parts[0], parts[1]));
                }
            }
        }
        return questions;
    }
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(" ██████╗ ██╗   ██╗██╗███████╗ ██████╗██████╗  █████╗ ███████╗████████╗");
            Console.WriteLine("██╔═══██╗██║   ██║██║╚══███╔╝██╔════╝██╔══██╗██╔══██╗██╔════╝╚══██╔══╝");
            Console.WriteLine("██║   ██║██║   ██║██║  ███╔╝ ██║     ██████╔╝███████║█████╗     ██║   ");
            Console.WriteLine("██║▄▄ ██║██║   ██║██║ ███╔╝  ██║     ██╔══██╗██╔══██║██╔══╝     ██║   ");
            Console.WriteLine("╚██████╔╝╚██████╔╝██║███████╗╚██████╗██║  ██║██║  ██║██║        ██║   ");
            Console.WriteLine(" ╚══▀▀═╝  ╚═════╝ ╚═╝╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝  ╚═╝╚═╝        ╚═╝   ");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            UserInteraction userInteraction = new UserInteraction();
            userInteraction.DisplayUserTypeMenu();
        }
    }
}