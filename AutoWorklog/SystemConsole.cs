namespace AutoWorklog
{
    public class SystemConsole : IConsole
    {
        public void SetForeGroundColor(ConsoleColor color) => Console.ForegroundColor = color;

        public void SetBackgroundColor(ConsoleColor color) => Console.BackgroundColor = color;

        public void ResetColor() => Console.ResetColor();

        public ConsoleKeyInfo ReadKey() => Console.ReadKey();

        public int CursorTop() => Console.CursorTop;

        public void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

        public void WriteLine(string text, params object[] args) => Console.WriteLine(text, args);

        public void WriteLine(string text) => Console.WriteLine(text);

        public void WriteLine(WorkLogTask task) => Console.WriteLine(task);

        public string ReadLine() => Console.ReadLine();

        public void Clear() => Console.Clear();

        public void Write(char c) => Console.Write(c);

    }
}
