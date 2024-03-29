﻿using System;
/*
 * Lab 1
 * Name: Samuel Omaghomi
 * Student ID: 041012180
 * Course: CST8359 - .Net Enterprise Appl. Dev.
 * Lab Section: 301
 */
class Lab1
{
    static void Main(String[] args)
    {
        string filePath = @"../../../Words.txt";    //file path for the word.txt file
        IList<string> wordsList = new List<string>();

        string? userInput;

        do
        {
            menu();
            userInput = Console.ReadLine();

            switch (userInput)
            {
                /* Remove this later */
                case "0":
                    printList(wordsList);
                    break;
                case "1":
                    wordsList = textToList(filePath, wordsList);
                    break;
                case "2":
                    IList<String> bSortResult = BubbleSort(wordsList);
                    break;
                case "3":
                    IList<String> LSortResult = LINQSort(wordsList);
                    break;
                case "4":
                    countDistinct(wordsList);
                    break;
                case "5":
                    lastTen(wordsList);
                    break;
                case "6":
                    printReverse(wordsList);
                    break;
                case "7":
                    endingWith(wordsList);
                    break;
                case "8":
                    contains(wordsList);
                    break;
                case "9":
                    moreThanStartsWith(wordsList);
                    break;
            }
        } while (userInput != "x");

    }

    /*
     * Converts a string from a text file into a list
     */
    static IList<string> textToList(string filePath, IList<string> wordsList)
    {
        int count = 0;  //number of words in the file
        try
        {
            using (StreamReader file = new StreamReader(filePath))  //reads string from the words file
            {
                string? word;
                while ((word = file.ReadLine()) != null)    //while there is a word in the next like, add the word to the words list
                {
                    wordsList.Add(word);
                    count++;
                }
            }
            Console.WriteLine("The number of words in file is: " + count + "\n");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return wordsList;
    }

    /*
     * Prints each item in a list on the console
     */
    static void printList(IList<string> wordsList)
    {
        for (int i = 0; i < wordsList.Count; i++)   //loops through the entire word list 
        {
            Console.WriteLine(wordsList[i]);
        }
    }

    /*
     * Sorts items in a list using bubble sort sorting method
     */
    static IList<string> BubbleSort(IList<string> words)
    {
        IList<string> wordsAsc = new List<string>(words);
        if (wordsAsc.Count != 0)    //if the words list isn't emmpty
        {
            var timer = new System.Diagnostics.Stopwatch(); 
            timer.Start();  //time the algorithm started executing

            int wordsLength = wordsAsc.Count;   //length of the wordslist 
            /*
             * The algorithm compares 2 words, starting from the beginning, if the first word is greater(based on the alphabets in the word)
             * the words are switched, the process continues until the last word 
             */
            for (int i = 0; i < wordsLength - 1; i++)   
            {
                for (int j = 0; j < wordsLength-i-1; j++)
                {
                    if (string.Compare(wordsAsc[j], wordsAsc[j + 1]) == 1)
                    {
                        string temp = wordsAsc[j];  //temporary location to store word being switched
                        wordsAsc[j] = wordsAsc[j+1];
                        wordsAsc[j+1] = temp;
                    }
                }
            }
            timer.Stop();   //time the algorithm finished executing
            Console.WriteLine("Execution time: " + timer.ElapsedMilliseconds + "ms\n");
        }
        else
        {
            Console.WriteLine("Please load words first!\n");
        }

        return wordsAsc;

    }

    /*
     * Sorts items in a list using LINQ orderby sorting method
     */
    static IList<string> LINQSort(IList<string> words)
    {
        IList<string> wordsAsc = words;
        if (words.Count != 0)
        {
            var timer = new System.Diagnostics.Stopwatch();
            timer.Start();

            //query using LINQ to return words in ascending order
            wordsAsc = (from word in words
                        orderby word ascending
                        select word).ToList();  

            timer.Stop();
            Console.WriteLine("Execution time: " + timer.ElapsedMilliseconds + "ms\n");
        }
        else
        {
            Console.WriteLine("Please load words first!\n");
        }
        return wordsAsc;

    }

    /*
     * Counts distinct words in a words list
     */
    static void countDistinct(IList<string> words)
    {
        if (words.Count != 0)
        {
            IList<string> wordsDist = (words.Distinct()).ToList(); 
            int distinctWords = wordsDist.Count;
            Console.WriteLine("Distinct count is " + distinctWords);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Prints last 10 words in a words list
     */
    static void lastTen(IList<string> words)
    {
        if (words.Count != 0)
        {
            words = ((from word in words
                      select word).TakeLast(10)).ToList();
            printList(words);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Prints the words list in reverse
     */
    static void printReverse(IList<string> words)
    {
        if (words.Count != 0)
        {
            words = ((from word in words
                      select word).Reverse()).ToList();
            printList(words);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Prints the words ending with 'd' in the list
     */
    static void endingWith(IList<string> words)
    {
        if (words.Count != 0)
        {
            words = (from word in words
                     where word.EndsWith("d")
                     select word).ToList();
            int wordCount = words.Count;
            Console.WriteLine("Number of words containing 'd': " + wordCount);
            printList(words);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Print the words in the list containing 'q'
     */
    static void contains(IList<string> words)
    {
        if (words.Count != 0)
        {
            words = (from word in words
                     where word.Contains("q")
                     select word).ToList();
            int wordCount = words.Count;
            Console.WriteLine("Number of words containing 'q': " + wordCount);
            printList(words);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Prints wors that start with 'a' and are more than 3 letters
     */
    static void moreThanStartsWith(IList<string> words)
    {
        if (words.Count != 0)
        {
            words = (from word in words
                     where word.Length > 3 && word.StartsWith("a")
                     select word).ToList();
            int wordCount = words.Count;
            Console.WriteLine("Number of words more than 3 charcters long and start with letter 'a': " + wordCount);
            printList(words);
            return;
        }
        Console.WriteLine("Please load words first!\n");
    }

    /*
     * Menu that shows options for user
     */
    static void menu()
    {
        Console.Write("Choose an option:\n1. Import Words from file\n2. Bubble sort words\n3. LINQ/Lambda sort words\n4. Count the distinct words \n5. Take the last 10 words " +
            "\n6. Reverse print the words \n7. Get and display words that end with 'd' and display the count \n8. Get and display words that contain 'q' and display the count \n9. " +
            "Get and display wors that are more than 3 charcters long and start with the letter 'a', and display the count \nx. Exit\n\nChoose an Option: ");
    }


}



