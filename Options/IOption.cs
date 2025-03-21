using System.Drawing;

namespace EDIDChecker
{
    internal interface IOption
    {
        internal  Action<string>? OutputAction { get; set; }

        internal  virtual void Run(){}

        internal virtual void Initialize(){}

        internal virtual string Dump(Size size){ return string.Empty;}
    }
}
