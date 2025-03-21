using System.Drawing;

namespace EDIDChecker
{
    internal class EDIDChecker
    {
         static ConsoleOutput _consoleOutput = new();
         Action<string> _OutputAction = (value) => _consoleOutput.Print(value);

        List<Option> _optionsToExecute = new List<Option>();

         List<string> _arguments;

         private const int DEFAULT_WIDTH=1920;
         private const int DEFAULT_HEIGHT=1080;

        internal EDIDChecker(string[] args)
        {
            _arguments = [.. args];
        }

        internal void Run()
        {
            _OutputAction.Invoke("EDID Checker");

            string edid = string.Empty;

            Width? widthOption = null;
            Hight? heightOption = null;

            if (_arguments.Count >= 1)
            {
                for (int i = 0; i < _arguments.Count; i++)
                {
                    if (_arguments[i].ToUpperInvariant() == FileOption._identifier)
                    {
                        var optionToAdd = new FileOption(_arguments[++i]);
                        optionToAdd.OutputAction = _OutputAction;
                        _optionsToExecute.Add(optionToAdd);

                    }
                    else if (_arguments[i].ToUpperInvariant() == EdidOption._identifier)
                    {
                        var optionToAdd = new EdidOption(_arguments[++i]);
                        optionToAdd.OutputAction = _OutputAction;
                        _optionsToExecute.Add(optionToAdd);
                    }
                    else if (_arguments[i].ToUpperInvariant() == Width._identifier)
                    {
                        widthOption = new Width(int.Parse(_arguments[++i]));
                    }
                    else if (_arguments[i].ToUpperInvariant() == Hight._identifier)
                    {
                        heightOption = new Hight(int.Parse(_arguments[++i]));
                    }
                }
            }

            int widthToChheck=(null==widthOption)?DEFAULT_WIDTH:widthOption._value;
            int heightToCheck=(null==heightOption)?DEFAULT_HEIGHT:heightOption._value;
            
            Size? size = new Size(widthToChheck, heightToCheck);

            _OutputAction.Invoke($"Size to check: {size}");

            _optionsToExecute.ForEach(action =>
            {
                action.Initialize();
                _OutputAction.Invoke($"EDIT to check: {action._EDID}");
                
                action.Run();
                string value = action.Dump(size.Value);

                _OutputAction.Invoke(value);
            });
        }
    }
}
