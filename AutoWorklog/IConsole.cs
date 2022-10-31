using Org.BouncyCastle.Asn1;

namespace AutoWorklog;

public interface IConsole
{
    void SetForeGroundColor(ConsoleColor color); 

    void SetBackgroundColor(ConsoleColor color);

    void ResetColor();

    ConsoleKeyInfo ReadKey();

    int CursorTop();

    void SetCursorPosition(int left, int top);

    void WriteLine(string message);

    void WriteLine(string message, params object[] args);

    void Write(char c);

    void Clear();

    string ReadLine();
}
