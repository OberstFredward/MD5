using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MD5
{
    
    class Program
    {
        static bool finished = false,syntax;
        private static byte counter;
        static void Main(string[] args)
        {
            char choice_ed = ' ', choice_tf = ' ';
            counter = 0;
            do
            {
                do
                {
                    counter++;
                    syntax = false;
                    if(counter == 1) Console.WriteLine("HELLO AND WELCOME TO THE MD5-ENCRYPTION/DECRYPTION TOOL\n<e> ENCRYPT\n<d> DECRYPT");
                    else Console.WriteLine("\nHELLO AND WELCOME TO THE MD5-ENCRYPTION/DECRYPTION TOOL\n<e> ENCRYPT\n<d> DECRYPT");
                    try
                    {
                        choice_ed = Convert.ToChar(ReadLine());
                        if (choice_ed == 68) choice_ed = (char) 100;
                        if (choice_ed == 69) choice_ed = (char) 101;
                        if (choice_ed == 100 || choice_ed == 101) syntax = true;
                    }
                    catch (Exception)
                    {
                    }
                    if(syntax == false) Console.WriteLine("\n\nFAIL");
                } while (!syntax);
                
                do
                {
                    syntax = false;
                    Console.WriteLine("\n\nDO YOU WANT TO ENCRYPT/DECRYPT A TEXT OR A FILE?(t/f)");
                    try
                    {
                        choice_tf = Convert.ToChar(ReadLine());
                        if (choice_tf == 70) choice_tf = (char) 102;
                        if (choice_tf == 84) choice_tf = (char) 116;
                        if (choice_tf == 102 || choice_tf == 116) syntax = true;
                    }
                    catch (Exception)
                    {
                    }
                    if(syntax == false) Console.WriteLine("\n\nFAIL");
                } while (!syntax);

                string text = "";
                string path = @"";
                switch (choice_ed)
                {
                    case 'e': //encrypt
                        switch (choice_tf)
                        {
                            case 't':
                                string eingabe = ReadText();
                                Console.WriteLine("\n\n"+MD5.hash(eingabe));
                                Console.WriteLine(MD5DotNet(eingabe));
                                break;
                            case 'f':
                                Console.WriteLine("\n\nCOMING SOON");
                                break;
                        }
                        break;
                    case 'd': //decrypt
                        switch (choice_tf)
                        {
                            case 't':
                                Console.WriteLine("\n\nDecrypt Text");
                                break;
                            case'f':
                                Console.WriteLine("\n\nDycrypt File");
                                break;
                        }
                        break;
                }
            } while (!finished);
        }
        private static string ReadText()
        {
            string text = "";
            do
            {
                syntax = false;
                Console.WriteLine("\n\nPLEASE WRITE YOUR TEXT");
                try
                {
                    text = ReadLine();
                    syntax = true;
                }
                catch (Exception)
                {
                }
                if (text == "") syntax = false;
            } while (!syntax);
            return text;
        }

        private static string ReadLine() //Eingabe auf eine Ziffer beschränken
        {
            StringBuilder sb = new StringBuilder();
            bool loop = true;
            while (loop)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true); // won't show up in console
                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        {
                            loop = false;
                            break;
                        }
                    default:
                        {
                            if (sb.Length < 200)
                            {
                                sb.Append(keyInfo.KeyChar);
                                Console.Write(keyInfo.KeyChar);
                            }
                            break;
                        }
                }
            }
            return sb.ToString();
        }
        private static string MD5DotNet(string input)

        {

            // step 1, calculate MD5 hash from input

            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)

            {

                sb.Append(hash[i].ToString("X2"));

            }

            return sb.ToString();

        }
    }
}
