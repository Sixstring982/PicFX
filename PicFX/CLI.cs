using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PicFX
{
    class CLI
    {
        
        public CLI()
        {
            ShowSplash();
            FileBrowser browser = new FileBrowser();
            PicMod modder = new PicMod();
            bool running = true;
            while (running)
            {
                running = HandleCommand(GetCommand(browser.GetDirectory()), browser, modder);
            }
        }

        private string GetCommand(string currentDir)
        {
            Console.Write(currentDir + ">");
            return Console.ReadLine().ToLower();
        }



        private bool HandleCommand(string command, FileBrowser browser, PicMod modder)
        {
            string[] commands = command.Split(" ".ToCharArray());
            if (commands.Length > 0)
            {
                switch (commands[0])
                {
                    case "dir":
                        string[] dirstuff = browser.GetFolderItems("");
                        if (dirstuff == null) return true;
                        for (int i = 0; i < dirstuff.Length; i++)
                        {
                            ConX.FileWrite(dirstuff[i]);
                        }
                        break;
                    case "cd":
                        if (commands.Length > 1)
                        {
                            browser.ChangeDir(commands);
                        }
                        break;
                    case "load":
                        if (commands.Length > 1)
                        {
                            modder.LoadFromFilename(browser.GetPath() + "\\" + commands[1]);
                        }
                        else
                            ConX.ErrorWrite("No filename specified.");
                        break;
                    case "save":
                        if (commands.Length > 1)
                            modder.SaveToFilename(browser.GetPath() + "\\" + commands[1]);
                        else
                            ConX.ErrorWrite("No filename specified.");
                        break;
                    case "deres":
                        if (commands.Length > 1)
                        {
                            int tryScale = 0;
                            if (Int32.TryParse(commands[1], out tryScale))
                            {
                                modder.DeRes(tryScale);
                            }
                            else
                            {
                                ConX.ErrorWrite("Scaling argument must be an integer.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("Needs a scaling argument.");
                        }
                        break;
                    case "compress":
                        if (commands.Length > 1)
                        {
                            int tryComp = 0;
                            if (Int32.TryParse(commands[1], out tryComp))
                            {
                                if (tryComp < 0 || tryComp > 7)
                                {
                                    ConX.ErrorWrite("Compression must be a value between 0 and 7.");
                                }
                                else
                                    modder.Compress(tryComp);
                            }
                            else
                                ConX.ErrorWrite("Compression argument is not a number.");
                        }
                        else
                        {
                            ConX.ErrorWrite("Compress needs a compression argument.");
                        }
                        break;
                    case "negate":
                        modder.Negate();
                        break;
                    case "reset":
                        modder.Reset();
                        break;
                    case "m+":
                        modder.Mplus();
                        break;
                    case "m-":
                        modder.Mminus();
                        break;
                    case "m":
                        modder.Mplus();
                        modder.Mminus();
                        break;
                    case "crotate":
                        modder.CRotate();
                        break;
                    case "pane":
                        if (commands.Length > 1)
                        {
                            int tryint = 0;
                            if (Int32.TryParse(commands[1], out tryint))
                                modder.Pane(tryint);
                            else
                                ConX.ErrorWrite("Pane size argument is invalid.");
                        }
                        else
                            ConX.ErrorWrite("Pane requires an int argument (pane size)");
                        break;
                    case "unload":
                        modder.Unload();
                        break;
                    case "csmear":
                        modder.CSmear();
                        break;
                    case "for":
                        if (commands.Length > 2)
                        {
                            int tryFor;
                            if (Int32.TryParse(commands[1], out tryFor))
                            {
                                string[] pieces = new string[commands.Length - 1];
                                for (int i = 1; i < commands.Length; i++)
                                    pieces[i - 1] = commands[i];
                                string finalCmd = Util.PutTogether(pieces);
                                for (int i = 0; i < tryFor; i++)
                                {
                                    HandleCommand(finalCmd, browser, modder);
                                }
                            }
                            else
                            {
                                ConX.ErrorWrite("Loop argument must be an integer.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("Invalid arguments. For [int loops] [command]");
                        }
                        break;
                    case "flip":
                        if (commands.Length > 1)
                        {
                            if (commands[1] == "h")
                                modder.Flip(true);
                            else if (commands[1] == "v")
                                modder.Flip(false);
                            else
                                ConX.ErrorWrite("Flip argument invalid. Must be (H,V)");
                        }
                        else
                        {
                            ConX.ErrorWrite("Flip command needs another argument (H,V)");
                        }
                        break;
                    case "dechannel":
                        if (commands.Length > 1)
                        {
                            if (commands[1] == "r")
                                modder.DeChannel(Channel.R);
                            else if (commands[1] == "g")
                                modder.DeChannel(Channel.G);
                            else if (commands[1] == "b")
                                modder.DeChannel(Channel.B);
                            else
                                ConX.ErrorWrite("DeChannel argument invalid. Must be (R,G,B)");
                        }
                        else
                        {
                            ConX.ErrorWrite("DeChannel needs another argument (R,G,B)");
                        }
                        break;
                    case "rechannel":
                        if (commands.Length > 1)
                        {
                            if (commands[1] == "r")
                                modder.ReChannel(Channel.R);
                            else if (commands[1] == "g")
                                modder.ReChannel(Channel.G);
                            else if (commands[1] == "b")
                                modder.ReChannel(Channel.B);
                            else
                                ConX.ErrorWrite("ReChannel argument invalid. Must be (R,G,B)");
                        }
                        else
                        {
                            ConX.ErrorWrite("ReChannel needs another argument (R,G,B)");
                        }
                        break;
                    case "multiply":
                        if (commands.Length > 2)
                        {
                            double tryThresh = 0;
                            if(Double.TryParse(commands[1], out tryThresh))
                            {
                                if (tryThresh >= 0 && tryThresh <= 255)
                                {
                                    if (commands[2] == "r")
                                        modder.Multiply(tryThresh, Channel.R);
                                    else if (commands[2] == "g")
                                        modder.Multiply(tryThresh, Channel.G);
                                    else if (commands[2] == "b")
                                        modder.Multiply(tryThresh, Channel.B);
                                    else if (commands[2] == "all")
                                    {
                                        modder.Multiply(tryThresh, Channel.R);
                                        modder.Multiply(tryThresh, Channel.G);
                                        modder.Multiply(tryThresh, Channel.B);
                                    }
                                    else
                                        ConX.ErrorWrite("Multiply channel argument invalid. Must be (R,G,B,ALL)");
                                }
                                else
                                {
                                    ConX.ErrorWrite("Multiply threshold argument must be between 0 and 255.");
                                }
                            }
                            else
                            {
                                ConX.ErrorWrite("Multiply threshold argument invalid.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("Multiply requires two arguments (byte threshold, (R,G,B))");
                        }
                        break;
                    case "greychannel":
                        if (commands.Length > 1)
                        {
                            if (commands[1] == "r")
                                modder.GreyChannel(Channel.R);
                            else if (commands[1] == "g")
                                modder.GreyChannel(Channel.G);
                            else if (commands[1] == "b")
                                modder.GreyChannel(Channel.B);
                            else
                                ConX.ErrorWrite("GreyChannel argument invalid. Must be (R,G,B)");
                        }
                        else
                        {
                            ConX.ErrorWrite("GreyChannel needs another argument (R,G,B)");
                        }
                        break;
                    case "cthreshold":
                        if (commands.Length > 2)
                        {
                            int tryThresh = 0;
                            if(Int32.TryParse(commands[1], out tryThresh))
                            {
                                if (tryThresh >= 0 && tryThresh <= 255)
                                {
                                    if (commands[2] == "r")
                                        modder.CThreshold((byte)tryThresh, Channel.R);
                                    else if (commands[2] == "g")
                                        modder.CThreshold((byte)tryThresh, Channel.G);
                                    else if (commands[2] == "b")
                                        modder.CThreshold((byte)tryThresh, Channel.B);
                                    else
                                        ConX.ErrorWrite("CThreshold channel argument invalid. Must be (R,G,B)");
                                }
                                else
                                {
                                    ConX.ErrorWrite("CThreshold threshold argument must be between 0 and 255.");
                                }
                            }
                            else
                            {
                                ConX.ErrorWrite("CThreshold threshold must be a number.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("CThreshold requires two arguments (byte threshold, (R,G,B))");
                        }
                        break;
                    case "ckeep":
                        if (commands.Length > 2)
                        {
                            int tryThresh = 0;
                            if (Int32.TryParse(commands[1], out tryThresh))
                            {
                                if (tryThresh >= 0 && tryThresh <= 255)
                                {
                                    if (commands[2] == "r")
                                        modder.CKeep((byte)tryThresh, Channel.R);
                                    else if (commands[2] == "g")
                                        modder.CKeep((byte)tryThresh, Channel.G);
                                    else if (commands[2] == "b")
                                        modder.CKeep((byte)tryThresh, Channel.B);
                                    else
                                        ConX.ErrorWrite("CKeep channel argument invalid. Must be (R,G,B)");
                                }
                                else
                                {
                                    ConX.ErrorWrite("CKeep threshold argument must be between 0 and 255.");
                                }
                            }
                            else
                            {
                                ConX.ErrorWrite("CKeep threshold argument must be a number.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("CKeep requires two arguments (byte threshold, (R,G,B))");
                        }
                        break;
                    case "clow":
                        if (commands.Length > 2)
                        {
                            int tryThresh = 0;
                            if (Int32.TryParse(commands[1], out tryThresh))
                            {
                                if (tryThresh >= 0 && tryThresh <= 255)
                                {
                                    if (commands[2] == "r")
                                        modder.CLow((byte)tryThresh, Channel.R);
                                    else if (commands[2] == "g")
                                        modder.CLow((byte)tryThresh, Channel.G);
                                    else if (commands[2] == "b")
                                        modder.CLow((byte)tryThresh, Channel.B);
                                    else
                                        ConX.ErrorWrite("CLow channel argument invalid. Must be (R,G,B)");
                                }
                                else
                                {
                                    ConX.ErrorWrite("CLow threshold argument must be between 0 and 255.");
                                }
                            }
                            else
                            {
                                ConX.ErrorWrite("CLow threshold argument must be a number.");
                            }
                        }
                        else
                        {
                            ConX.ErrorWrite("CLow requires two arguments (byte threshold, (R,G,B))");
                        }
                        break;
                    case "show":
                        if (modder.loadedBMP == null)
                            ConX.ErrorWrite("No image loaded. Load one first.");
                        else
                            new ViewLoader(modder);
                        break;
                    case "cam":
                        if (modder.loadedBMP == null)
                            ConX.ErrorWrite("No image loaded. Load one first.");
                        else
                        {
                            if (modder.loadedBMP.Width < 512 &&
                                modder.loadedBMP.Height < 512)
                                new ViewLoader(modder);
                            else
                                new CamMapForm(modder);
                        }
                        break;
                    case "exit":
                    case "quit":
                        return false;
                }
            }
            return true;
        }

        private void ShowSplash()
        {
            Console.Clear();
            Console.WriteLine("PicFX");
            Console.WriteLine("2012 Sixstring982");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
            Console.Clear();
        }

        private void PrintHelp()
        {
            Console.WriteLine("Valid Commands:");
            Console.WriteLine("Load [filename]");
            Console.WriteLine("  Loads an image for FX");
            Console.WriteLine("Show");
            Console.WriteLine("  Shows the current image in a form");
        }

        public static void Main(string[] args)
        {
            new CLI();
        }
    }
}
