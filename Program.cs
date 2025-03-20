
using System;
using System.Drawing;

namespace EDIDChecker
{


    internal class Program
    {
        static List<Option> optionsToExecute = new List<Option>();

        static void Main(string[] args)
        {
            List<string> arguments = [.. args];

            Console.WriteLine("EDID Checker");

            string edid = string.Empty;

            Width? widthOption = null;
            Hight? heightOption = null;

            if (arguments.Count >= 1)
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    if (arguments[i].ToUpperInvariant() == FileOption._identifier)
                    {

                        Console.WriteLine("FileOption");

                        var optionToAdd = new FileOption(arguments[++i]);
                        optionsToExecute.Add(optionToAdd);

                    }
                    else if (arguments[i].ToUpperInvariant() == EdidOption._identifier)
                    {

                        Console.WriteLine("EdidOption");

                        var optionToAdd = new EdidOption(arguments[++i]);
                        optionsToExecute.Add(optionToAdd);
                    }
                    else if (arguments[i].ToUpperInvariant() == Width._identifier)
                    {
                        widthOption = new Width(int.Parse(arguments[++i]));
                    }
                    else if (arguments[i].ToUpperInvariant() == Hight._identifier)
                    {
                        heightOption = new Hight(int.Parse(arguments[++i]));
                    }
                }
            }

            Size? size = new Size(1920, 1080); // some default display resolution
            if (null != heightOption && null != widthOption)
            {
                size = new Size(widthOption._width, heightOption._height);
            }

            Console.WriteLine($"Options found: {optionsToExecute.Count}");

            optionsToExecute.ForEach(action =>
            {
                action.Run();
                string value = action.Dump(size.Value);

                Console.WriteLine(value);
            });

        }
    }
}
