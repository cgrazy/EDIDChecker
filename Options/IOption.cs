using System.Drawing;

namespace EDIDChecker
{
    internal interface IOption
    {
        internal  virtual void Run(){}

        internal virtual string Dump(Size size){ return string.Empty;}
    }
}
