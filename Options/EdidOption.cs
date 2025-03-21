namespace EDIDChecker
{
    internal class EdidOption:Option
    {
        internal static string _identifier = "-E";
    
        public EdidOption(string edidContent)
        {
            _EDID = edidContent;
        }

        internal override void Initialize()
        {
            base._dip = new DisplayInformationSupplier(DisplayInformationSupplier.CreateByteArrayFromString(base._EDID));

            _initialized=true;
        }

        internal override void Run()
        {
             if(!_initialized)
            {
                OutputAction?.Invoke("FileOption not yet initialized.");
            }
        }
    }
}
