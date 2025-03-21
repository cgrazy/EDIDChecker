using System.Collections.Concurrent;
using System.Drawing;
using System.Text;

namespace EDIDChecker
{
    internal class Option : IOption
    {
        private Action<string> _outputAction;

        public Action<string>? OutputAction { get => _outputAction; set => _outputAction = value; }

        internal bool _initialized = false;

        internal string? _EDID;

        internal  DisplayInformationSupplier? _dip;

        internal virtual void Run() {}

        internal virtual string DumpFileOption(Size size) { return string.Empty; }

        internal bool IsValidEDID()
        {
              bool isValid = false;

            if(string.IsNullOrWhiteSpace(_EDID))
            {
                Console.WriteLine($"No edid '{_EDID}' given. Abort!");
                return isValid;
            }

            if(_EDID.ToUpperInvariant().StartsWith("00FFFFFFFFFFFF00"))
            {
                isValid = true;
            }

            return isValid;
        }
        internal virtual void Initialize()
        {

        }

        internal string Dump(Size size)
        {
            string returnValue = string.Empty;

            var sb = new StringBuilder();
            
            sb.AppendFormat($"Dump for size {size.Width}x{size.Height} in pixel.{Environment.NewLine}");

            var result = _dip.GetPhysicalSizeInCM(size, 1);

            var version = _dip.GetEDIDVersion();
            var yom = _dip.GetYearOfManufacture();
            var serialNumber = _dip.GetSerialNumber();
           
            var screenSizeInCm = _dip.GetScreenSizeInCM();
            string sceenSizeInCmAsString = screenSizeInCm.ToString();
            sb.AppendFormat($"  Version: {version}{Environment.NewLine}");
            sb.AppendFormat($"  Serial Number: {serialNumber}{Environment.NewLine}");
            sb.AppendFormat($"  Monitor Serial Number: {_dip.GetMonitorSerialNumber()}{Environment.NewLine}");
            sb.AppendFormat($"  Monitor Name: {_dip.GetMonitorName()}{Environment.NewLine}");
            sb.AppendFormat($"  Year of manufacture: {yom}{Environment.NewLine}");
            sb.AppendFormat("  ScreenSize: {0} in cm{1}", sceenSizeInCmAsString, Environment.NewLine);
            sb.AppendFormat("  Calculated physical size: {0} in cm{1}",result, Environment.NewLine);

            sb.AppendFormat($"     dump: {_dip.DumpBlockAddressBlock(1)}{Environment.NewLine}");

            return sb.ToString();
        }
    }
}
