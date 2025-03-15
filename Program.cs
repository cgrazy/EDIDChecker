
using System;
using System.Drawing;

namespace EDIDChecker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("EDID Checker");

            //string edid1_Customer =
            //        "00FFFFFFFFFFFF0010AC5D4157353946101E010380351E78EEEE95A3544C99260F5054A54B00714F8180A9C0D1C00101010101010101023A801871382D40582C45000F282100001E000000FF00324A38435631330A2020202020000000FC0044454C4C205532343139480A20000000FD00384C1E5311000A2020202020200126";

            string edid2_Customer =
                    "00FFFFFFFFFFFF00371801000100000004190103807341780ACF74A3574CB02309484C2108008180950090408100B300A940D1008140023A801871382D40582C450010090000001E011D8018711C1620582C2500C48E2100009E000000FC004D58582D412D320A2020202020000000FD00163E0E5010000A2020202020200190";

            //string edid3_Customer =
            //        "00FFFFFFFFFFFF00371801000100000004190103807341780ACF74A3574CB02309484C2108008180950090408100B300A940D1008140023A801871382D40582C450010090000001E011D8018711C1620582C2500C48E2100009E000000FC004D58582D412D320A2020202020000000FD00163E0E5010000A2020202020200190";

            string edid_Benq = "00ffffffffffff0009d12580455400000e1e0103804628782e87d1a8554d9f250e5054a56b80818081c08100a9c0b300d1c001010101023a801871382d40582c4500ffff0000001e000000ff0033344c30303633383031390a20000000fd00324c1e8c24000a202020202020000000fc0042656e5120504433323030550a017a020333f15161605f5e5d101f2221200514041312030123090707830100006c030c001000183c200040010267d85dc40178880104740030f2705a80b0588a00c48f2100001e565e00a0a0a029502f203500c48f2100001a000000000000000000000000000000000000000000000000000000000000000000000000000000004a";

            var size = new Size(1920,1080);
            CheckEdid(edid2_Customer, size);

            size = new Size(1920,1080);
            CheckEdid(edid_Benq, size);

            size = new Size(3840,2160);
            CheckEdid(edid_Benq, size, 1);
            CheckEdid(edid_Benq, size, 2);
            CheckEdid(edid_Benq, size, 3);
            CheckEdid(edid_Benq, size, 4);
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
