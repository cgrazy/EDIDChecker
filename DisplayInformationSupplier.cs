using System;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Drawing;
using System.Text;

namespace EDIDChecker
{
    internal class DisplayInformationSupplier
    {
        private List<int> displayTimingDescrptorBlock = new List<int> { 54, 72, 90, 108 };

        private readonly byte[] _edidBytes;

        private const int MONITOR_SERIAL_NUMBER = 0xFF;
        private const int MONITOR_NAME = 0xFC;

        private const int ZERO = 0x00;

        private const int MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH = 12;

        public DisplayInformationSupplier(byte[] edidByteArray)
        {
            _edidBytes = edidByteArray;
        }

        internal string GetSerialNumber()
        {
            byte[] a = new byte[4];
            a[0] = _edidBytes[12];
            a[1] = _edidBytes[13];
            a[2] = _edidBytes[14];
            a[3] = _edidBytes[15];
            int n = BitConverter.ToInt32(a, 0);
            return string.Format("{0}", n);
        }

        internal string GetMonitorSerialNumber()
        {
            string serialNumber = string.Empty;
            foreach (int blockAddress in displayTimingDescrptorBlock)
            {
                if (_edidBytes[blockAddress + 3] == MONITOR_SERIAL_NUMBER)
                {
                    StringBuilder sb = new StringBuilder();

                    for (int i = 0; i < MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH; i++)
                        sb.AppendFormat("{0}", Convert.ToChar(_edidBytes[blockAddress + 5 + i]));

                    serialNumber = sb.ToString().TrimEnd('\0').TrimEnd('\n');
                    break;
                }
            }

            return serialNumber;
        }

        internal string GetMonitorName()
        {
            string monitorName = string.Empty;
            foreach (int blockAddress in displayTimingDescrptorBlock)
            {
                if (IsMonitorDescriptor(blockAddress))
                {
                    if (_edidBytes[blockAddress + 3] == MONITOR_NAME)
                    {
                        StringBuilder sb = new StringBuilder();

                        for (int i = 0; i < MONITOR_DESCRIPTOR_TYPE_DESCRIPTION_LENGTH; i++)
                            sb.AppendFormat("{0}", Convert.ToChar(_edidBytes[blockAddress + 5 + i]));

                        monitorName = sb.ToString().TrimEnd('\0');
                        break;
                    }
                }
            }
            return monitorName;
        }

        internal string GetYearOfManufacture()
        {
            return string.Format("Week {0} of year {1}", _edidBytes[16], 1990 + _edidBytes[17]);
        }

        internal string GetEDIDVersion()
        {
            string version = string.Empty;

            version = $"{_edidBytes[18]}.{_edidBytes[19]}";

            return version;
        }

        internal string DumpBlockAddressBlock(int blockAddressNumber)
        {
            int length = 18;
            byte[] a = new byte[length];
            for (int i = 0; i < length; i++)
                a[i] = _edidBytes[displayTimingDescrptorBlock[blockAddressNumber - 1] + i];

            return BitConverter.ToString(a);
        }

        internal SizeF GetPhysicalSizeInCM(Size monitorResolution, int blockAddressNumber)
        {
            int blockAddress = displayTimingDescrptorBlock[blockAddressNumber - 1];

            if (IsDetailedTimingDescriptor(blockAddress))
            {
                var edidResolution = GetScreenResolutionInPixel(blockAddress);
                if (edidResolution.Width == monitorResolution.Width && edidResolution.Height == monitorResolution.Height)
                {
                    return GetScreenSizeInCm(blockAddress);
                }
            }

            return new SizeF();
        }

        internal bool IsChecksumByteValid(out int modValue)
        {
            modValue = _edidBytes[127]%256;
            return ( modValue == 0);
        }

        internal SizeF GetScreenSizeInCM()
        {
            return new SizeF(Convert.ToInt32(_edidBytes[21]), Convert.ToInt32(_edidBytes[22]));
        }


        internal SizeF GetPhysicalSizeInCM(Size monitorResolution)
        {
            foreach (int blockAddress in displayTimingDescrptorBlock)
            {
                if (IsDetailedTimingDescriptor(blockAddress))
                {
                    var edidResolution = GetScreenResolutionInPixel(blockAddress);
                    if (edidResolution.Width == monitorResolution.Width && edidResolution.Height == monitorResolution.Height)
                    {
                        return GetScreenSizeInCm(blockAddress);
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
            // 1st two bytes of the descriptor are defined
            // if both first two bytes are 0x00 -> monitor descriptor
            // otherwise detailed timing descriptor with pixel, size and such information    
            return (_edidBytes[blockAddress] != ZERO && _edidBytes[blockAddress + 1] != ZERO) ? true : false;
        }

        private Size GetScreenResolutionInPixel(int blockAddress)
        {
            Size result = Size.Empty;
            // 2nd and 4th byte upper bits of the descriptor blcok gives Horizontal Addressable Videolines in Pixels
            result.Width = ConvertByteArrayToInt32(_edidBytes[blockAddress + 2], Convert.ToByte((_edidBytes[blockAddress + 4] & 0xf0) >> 4));
            // 5th and 7th byte upper bits gives Vertical Addressable Video lines
            result.Height = ConvertByteArrayToInt32(_edidBytes[blockAddress + 5], Convert.ToByte((_edidBytes[blockAddress + 7] & 0xf0) >> 4));

            Console.WriteLine("Resolution calculated for displayTimingDescrptorBlock {0} is {1}x{2}", blockAddress, result.Width, result.Height);
            return result;
        }

        private int ConvertByteArrayToInt32(byte lowerByte, byte higherByte)
        {
            var bytes = new byte[] { lowerByte, higherByte, ZERO, ZERO };
            return BitConverter.ToInt32(bytes, 0);
        }

        private SizeF GetScreenSizeInCm(int blockAddress)
        {
            SizeF result = Size.Empty;

            // 12, 13, 14 byte of the descriptor block gives the horizontal and vertical size 
            var int32Value = ConvertByteArrayToInt32(_edidBytes[blockAddress + 12], Convert.ToByte(_edidBytes[blockAddress + 14] >> 4));
            result.Width = int32Value * 0.1F;
            int32Value = ConvertByteArrayToInt32(_edidBytes[blockAddress + 13], Convert.ToByte(_edidBytes[blockAddress + 14] & 0x0F));
            result.Height = int32Value * 0.1F;

            Console.WriteLine("Screen size calculated for displayTimingDescrptorBlock {0} is {1}x{2}", blockAddress, result.Width, result.Height);
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