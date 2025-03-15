using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text;

namespace EDIDChecker
{
    internal class DisplayInfoProvider
    {
        private List<int> blockAddresses14 = new List<int> { 54, 72, 90, 108 };

         private List<int> blockAddresses13 = new List<int> { 54, 72, 90, 108 };

        private readonly byte[] myEdidBytes;

        private const int MONITOR_SERIAL_NUMBER = 0xFF;
        private const int MONITOR_NAME=0xFC;

        private const int ZERO = 0x00;

        private const int MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH = 12;

        public DisplayInfoProvider(byte[] edidByteArray)
        {
            myEdidBytes = edidByteArray;
        }

        internal string GetSerialNumber()
        {
            byte[] a = new byte[4];
            a[0] = myEdidBytes[12];
            a[1] = myEdidBytes[13];
            a[2] = myEdidBytes[14];
            a[3] = myEdidBytes[15];
            int n = BitConverter.ToInt32(a, 0);
            return string.Format("{0}", n);
        }

        internal string GetMonitorSerialNumber()
        {
            string serialNumber = string.Empty;
            foreach (int blockAddress in blockAddresses14)
            {
                if(myEdidBytes[blockAddress+3] == MONITOR_SERIAL_NUMBER)
                {
                    StringBuilder sb = new StringBuilder();

                    for(int i=0;i<MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH;i++)
                        sb.AppendFormat("{0}",Convert.ToChar(myEdidBytes[blockAddress+5+i]));

                    serialNumber = sb.ToString().TrimEnd('\0').TrimEnd('\n');
                    break;
                }
            }

            return serialNumber;
        }

        internal string GetMonitorName()
        {
            string monitorName = string.Empty;
            foreach (int blockAddress in blockAddresses14)
            {
                if(IsMonitorDescriptor(blockAddress))
                {           
                    if(myEdidBytes[blockAddress+3] == MONITOR_NAME)
                    {
                        StringBuilder sb = new StringBuilder();

                        for(int i=0;i<MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH;i++)
                            sb.AppendFormat("{0}",Convert.ToChar(myEdidBytes[blockAddress+5+i]));

                        monitorName = sb.ToString().TrimEnd('\0');
                        break;
                    }
                }
            }
            return monitorName;
        }

        internal string GetYearOfManufacture()
        {
            return string.Format("Week {0} of year {1}", myEdidBytes[16], 1990 + myEdidBytes[17]);
        }

        internal string GetEDIDVersion()
        {
            string version = string.Empty;

            version = $"{myEdidBytes[18]}.{myEdidBytes[19]}";

            return version;
        }

        internal string DumpBlockAddressBlock(int blockAddressNumber)
        {
            int length=18;
            byte[] a = new byte[length];
            for(int i=0;i<length;i++)
                a[i]=myEdidBytes[blockAddresses14[blockAddressNumber-1]+i];

            return BitConverter.ToString(a);
        }

        internal SizeF GetPhysicalSizeInCM(Size monitorResolution, int blockAddressNumber)
        {
            int blockAddress = blockAddresses14[blockAddressNumber-1];

                if (IsDetailedTimingDescriptor(blockAddress))
                {
                    var edidResolution = GetResolution(blockAddress);
                    if (edidResolution.Width == monitorResolution.Width && edidResolution.Height == monitorResolution.Height)
                    {
                        return GetSizeInCm(blockAddress);
                    }
                }

         
            return new SizeF();
        }

        internal SizeF GetScreenSizeInCm()
        {
            return new SizeF(Convert.ToInt32(myEdidBytes[21]), Convert.ToInt32(myEdidBytes[22]));
        }


        internal SizeF GetPhysicalSizeInCM(Size monitorResolution)
        {
            foreach (int blockAddress in blockAddresses14)
            {
                if (IsDetailedTimingDescriptor(blockAddress))
                {
                    var edidResolution = GetResolution(blockAddress);
                    if (edidResolution.Width == monitorResolution.Width && edidResolution.Height == monitorResolution.Height)
                    {
                        Console.WriteLine("da");
                        return GetSizeInCm(blockAddress);
                    }
                }
            }

            return new SizeF();
        }

        private bool IsMonitorDescriptor(int blockAddress)
        {
            return !IsDetailedTimingDescriptor(blockAddress);
        }

        private bool IsDetailedTimingDescriptor(int blockAddress)
        {
            //Console.WriteLine($"{myEdidBytes[blockAddress]} {myEdidBytes[blockAddress + 1]}");

            // 0,1 byte of the descriptor 
            // if both first two bytes are 0x00 -> monitor descriptor
            // otherwise detailed timing descriptor    
            return (myEdidBytes[blockAddress] != ZERO && myEdidBytes[blockAddress + 1] != ZERO) ? true: false;
        }

        private Size GetResolution(int blockAddress)
        {
            Size result = Size.Empty;
            // 2nd, 4th byte of the descriptor blcok gives Horizontal Addressable Videolines in Pixels
            result.Width = MakeInt32FromBytes(myEdidBytes[blockAddress + 2], Convert.ToByte((myEdidBytes[blockAddress + 4] & 0xf0) >> 4));
            // 5th, 7th byte  gives Vertical Addressable Video lines
            result.Height = MakeInt32FromBytes(myEdidBytes[blockAddress + 5], Convert.ToByte((myEdidBytes[blockAddress + 7] & 0xf0) >> 4));

            Console.WriteLine("resolution for blockAddress {0} is {1}x{2}", blockAddress, result.Width, result.Height);
            return result;
        }

        private int MakeInt32FromBytes(byte lowerByte, byte higherByte)
        {
            var bytes = new byte[] { lowerByte, higherByte, ZERO, ZERO };
            return BitConverter.ToInt32(bytes, 0);
        }

        private SizeF GetSizeInCm(int blockAddress)
        {
            SizeF result = Size.Empty;

            // 12, 13, 14 byte of the descriptor block gives the horizontal and vertical size 
            var int32Value = MakeInt32FromBytes(myEdidBytes[blockAddress + 12], Convert.ToByte(myEdidBytes[blockAddress + 14] >> 4));
            result.Width = int32Value * 0.1F;
            int32Value = MakeInt32FromBytes(myEdidBytes[blockAddress + 13], Convert.ToByte(myEdidBytes[blockAddress + 14] & 0x0F));
            result.Height = int32Value * 0.1F;

            Console.WriteLine("size for blockAddress {0} is {1}x{2}", blockAddress, result.Width, result.Height);
            return result;

        }

        internal static byte[] CreateByteArrayFromString(string edid1)
        {
            Dictionary<string, byte> hexindex = new Dictionary<string, byte>();
            for (int i = 0; i <= 255; i++)
                hexindex.Add(i.ToString("X2"), (byte)i);

            edid1 = edid1.ToUpperInvariant();

            List<byte> hexres = new List<byte>();
            for (int i = 0; i < edid1.Length; i += 2)
                hexres.Add(hexindex[edid1.Substring(i, 2)]);

            return hexres.ToArray();
        }
    }
}