namespace EDIDChecker
{
    internal class ConsoleOutput
{
    public void Print(string outputToPrint)
    {
        Console.WriteLine(outputToPrint);
    }

    public void SetCurserPosition(int x, int y)
    {
        Console.SetCursorPosition(x, y);
    }
}

}
