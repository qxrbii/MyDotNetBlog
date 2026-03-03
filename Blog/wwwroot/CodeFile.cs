using System;
class HelloWorld
{
    static void Main()
    {
        int x = 20;
        int a, b;
        for (a = 1, b = 1; a <= x; a++)
        {
            if (b >= 10) break;
            if (b % 3 == 1) { b += 3; continue; }
        }
        console.WriteLine($"a+1={a+1});
    }
}