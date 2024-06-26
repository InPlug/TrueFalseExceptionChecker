﻿using System.ComponentModel;
using Vishnu.Demos;
using Vishnu.Interchange;

namespace TrueFalseExceptionCheckerDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TrueFalseExceptionChecker trueFalseExceptionChecker = new TrueFalseExceptionChecker();
            trueFalseExceptionChecker.NodeProgressChanged += Program.CheckerProgressChanged;

            Console.WriteLine("Ende mit irgendeiner Taste");
            try
            {
                while (true)
                {
                    try
                    {
                        //bool? logical = trueFalseExceptionChecker.Run("True:Null|5|Checker_12-Result", "Tree1", TreeEvent.UndefinedTreeEvent);
                        //bool? logical = trueFalseExceptionChecker.Run("Exception:True:Null|5|Checker_12-Result", "Tree1", TreeEvent.UndefinedTreeEvent);
                        //bool? logical = trueFalseExceptionChecker.Run("Exception:Null:False:True|7|Checker_12-Result", new TreeParameters("MainTree", null), TreeEvent.UndefinedTreeEvent);
                        bool? logical = trueFalseExceptionChecker.Run("Exception:Null:False:True|0|Result|1000", new TreeParameters("MainTree", null), TreeEvent.UndefinedTreeEvent);
                        Console.WriteLine("Checker: {0}, {1}", logical == null ? "null" : logical.ToString(),
                          trueFalseExceptionChecker.ReturnObject == null ? "null" : trueFalseExceptionChecker.ReturnObject.ToString());
                    }
                    catch (ApplicationException ex)
                    {
                        Console.WriteLine("Checker: {0}", ex.Message);
                    }
                    Thread.Sleep(1000);
                }

            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(Environment.NewLine + "beendet.");
            }
        }

        /// <summary>
        /// Wird angesprungen, wenn sich der Verarbeitungsfortschritt des Checkers geändert hat.
        /// </summary>
        /// <param name="sender">Der Checker.</param>
        /// <param name="args">Argumente mit Progress-Fortschritt.</param>
        static void CheckerProgressChanged(object? sender, ProgressChangedEventArgs args)
        {
            Console.WriteLine(args.ProgressPercentage);
            checkBreak();
        }

        static void checkBreak()
        {
            if (Console.KeyAvailable)
            {
                //ConsoleKeyInfo ki = Console.ReadKey();
                //string inKey = ki.KeyChar.ToString().ToUpper();
                //if (inKey == "E")
                //{
                throw new OperationCanceledException("Abbruch!");
                //}
            }
        }

    }
}