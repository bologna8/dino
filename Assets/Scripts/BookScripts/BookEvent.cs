using System;

public static class BookEvent 
{
    public static event EventHandler CloseBook;

    public static void CloseBookFunction()
    {
        CloseBook?.Invoke(null, EventArgs.Empty);
    }
}
