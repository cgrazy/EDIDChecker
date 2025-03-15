
using System;
using System.Drawing;

namespace EDIDChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EDID Checker");

            string fileName=string.Empty;
            string edid=string.Empty;
            
            if (args.Length>=1)
            {
                if(string.Compare(args[0].ToUpperInvariant(),"-F")==0)
                {
                    if(args.Length!=2)
                    {
                        Console.WriteLine($"No file name specified. Abort!");
                        return;
                    }

                    fileName = args[1];

                    if(!File.Exists(fileName))
                    {
                        Console.WriteLine($"The file '{fileName}' doesn't exists.");
                        return;
                    }
                    else
                    {
                        if(!string.IsNullOrEmpty(fileName))
                        {
                            edid = File.ReadAllText(fileName).Replace(" ","");
                        }
                    }
                }

                if(string.Compare(args[0].ToUpperInvariant(),"-E")==0)
                {
                    if(args.Length!=2)
                    {
                        Console.WriteLine($"No edid information specified. Abort!");
                        return;
                    }

                    edid = args[1];
                }
            }

            if(string.IsNullOrWhiteSpace(edid))
            {
                Console.WriteLine($"No edid '{edid}' given. Abort!");
                return;
            }

            if(!IsValidEDID(edid))
            {
                Console.WriteLine($"No valid edid given: '{edid}'. Abort!");
                return;
            }

            var size = new Size(1920,1080);
            CheckEdid(edid, size);
        }

        static bool IsValidEDID(string edid)
        {
            bool isValid = false;

            edid=edid.ToUpperInvariant();
            if(edid.StartsWith("00FFFFFFFFFFFF00"))
            {
                isValid = true;
            }

            return isValid;
        }

        static void CheckEdid(string edid, Size size)
        {
            CheckEdid(edid, size, 1);
        }

        static void CheckEdid(string edid, Size size, int blockAddressNumber)
        {
            Console.WriteLine($"Check edid: '{edid}'");
            Console.WriteLine("-----------------------");
        
            byte[] edidByteArray=DisplayInfoProvider.CreateByteArrayFromString(edid);

            var dip = new DisplayInfoProvider(edidByteArray);
            
            var result = dip.GetPhysicalSizeInCM(size, blockAddressNumber);

            var version = dip.GetEDIDVersion();
            var yom = dip.GetYearOfManufacture();
            var serialNumber = dip.GetSerialNumber();
           
            var screenSizeInCm = dip.GetScreenSizeInCm();
            Console.WriteLine($"  Version: {version}");
            Console.WriteLine($"  Serial Number: {serialNumber}");
            Console.WriteLine($"  Monitor Serial Number: {dip.GetMonitorSerialNumber()}");
            Console.WriteLine($"  Monitor Name: {dip.GetMonitorName()}");
            Console.WriteLine($"  Year of manufacture: {yom}");
            Console.WriteLine($"  ScreenSize: {screenSizeInCm} in cm");
            Console.WriteLine($"  Calculated physical size: {result} in cm");

            Console.WriteLine($"     dump: {dip.DumpBlockAddressBlock(blockAddressNumber)}");
        }
    }
}
