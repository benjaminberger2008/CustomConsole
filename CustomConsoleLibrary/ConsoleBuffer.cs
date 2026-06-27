using System;
using System.Text;
using System.Runtime.InteropServices;
using Figgle;
using Figgle.Fonts;

namespace CustomConsole
{
    public static class Ansi
    {
        private const int STD_OUTPUT_HANDLE = -11;
        private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll")]
        private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        public static void EnableAnsi()
        {
            IntPtr handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (handle == IntPtr.Zero) return;

            if (GetConsoleMode(handle, out uint mode))
            {
                mode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING;
                SetConsoleMode(handle, mode);
            }
        }
    }

    public static class ConsoleBuffer
    {
        public struct Pixel
        {
            public char c;
            public byte r;
            public byte g;
            public byte b;

            public Pixel(char c, byte r, byte g, byte b)
            {
                this.c = c;
                this.r = r;
                this.g = g;
                this.b = b;
            }
        }

        private static Pixel[,] buffer;
        private static Pixel[,] lastBuffer;

        private static int lastWidth;
        private static int lastHeight;

        public static int Width { get; private set; }
        public static int Height { get; private set; }

        public static int CursorX { get; private set; }
        public static int CursorY { get; private set; }

        public static int TabSize { get; set; } = 4;

        static ConsoleBuffer()
        {
            Ansi.EnableAnsi();

            Width = Console.WindowWidth;
            Height = Console.WindowHeight;

            lastWidth = Width;
            lastHeight = Height;

            buffer = new Pixel[Width, Height];
            lastBuffer = new Pixel[Width, Height];

            Console.CursorVisible = false;
            Clear();
        }

        private static bool InBounds(int x, int y)
            => x >= 0 && x < Width && y >= 0 && y < Height;

        public static void Fill(char c, byte r, byte g, byte b)
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    buffer[x, y] = new Pixel(c, r, g, b);

            CursorX = 0;
            CursorY = 0;
        }

        public static void Clear()
        {
            Fill(' ', 0, 0, 0);
        }

        public static void SetCursor(int x, int y)
        {
            CursorX = x;
            CursorY = y;
        }

        public static void Write(int x, int y, string s, byte r = 255, byte g = 255, byte b = 255)
        {
            CursorX = x;
            CursorY = y;

            foreach (char ch in s)
            {
                if (ch == '\n')
                {
                    CursorX = x;
                    CursorY++;
                    continue;
                }

                if (ch == '\t')
                {
                    CursorX += TabSize - (CursorX % TabSize);
                    continue;
                }

                if (InBounds(CursorX, CursorY))
                {
                    buffer[CursorX, CursorY] = new Pixel(ch, r, g, b);
                }

                CursorX++;
            }
        }

        public static void Write(string s, byte r = 255, byte g = 255, byte b = 255)
            => Write(CursorX, CursorY, s, r, g, b);

        public static void WriteFiggle(string s, byte r = 255, byte g = 255, byte b = 255)
        {
            string art = FiggleFonts.Rectangles.Render(s);
            string[] lines = art.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                ConsoleBuffer.Write(0, i, lines[i], r, g, b);
            }
        }
        public static void WriteFiggle(int x, int y, string s, byte r = 255, byte g = 255, byte b = 255)
        {
            string art = FiggleFonts.Rectangles.Render(s);
            string[] lines = art.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                ConsoleBuffer.Write(x, y + i, lines[i], r, g, b);
            }
        }
        public static void ResizeBuffer()
        {
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;

            if (w == Width && h == Height)
                return;

            Width = w;
            Height = h;

            buffer = new Pixel[Width, Height];
            lastBuffer = new Pixel[Width, Height];

            Clear();
        }

        public static void Draw()
        {
            var sb = new StringBuilder();

            int lastR = -1, lastG = -1, lastB = -1;

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    Pixel p = buffer[x, y];

                    if (p.r != lastR || p.g != lastG || p.b != lastB)
                    {
                        sb.Append("\x1b[38;2;");
                        sb.Append(p.r).Append(';');
                        sb.Append(p.g).Append(';');
                        sb.Append(p.b).Append('m');

                        lastR = p.r;
                        lastG = p.g;
                        lastB = p.b;
                    }

                    sb.Append(p.c);
                }

                if (y < Height - 1)
                    sb.Append('\n');
            }

            sb.Append("\x1b[0m");

            Console.SetCursorPosition(0, 0);
            Console.Write(sb.ToString());
        }
    }
}