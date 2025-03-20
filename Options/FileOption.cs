using System.Drawing;

namespace EDIDChecker
{

    internal class FileOption : Option
    {
        internal static string _identifier = "-F";

        private string _fileName=string.Empty;
        public FileOption(string fileName)
        {
            _fileName = fileName;   
        }

        internal override void Run()
        {
            base.Run();

            Console.WriteLine($"Run.");   

            if(Exists())
            {
                _EDID = File.ReadAllText(_fileName).Replace(" ","");

                _dip = new DisplayInformationSupplier(DisplayInformationSupplier.CreateByteArrayFromString(base._EDID));
            }
            else
            {
                Console.WriteLine($"The file {_fileName} doesn't exists.");                
            }
        }

        private bool Exists()
        {
            return File.Exists(Environment.ExpandEnvironmentVariables(_fileName));
        }


    }
}
