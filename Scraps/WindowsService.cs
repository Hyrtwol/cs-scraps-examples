using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;

namespace Scraps
{
    public interface IWindowsService : IDisposable
    {
        string Name { get; }
        void Start();
        void Stop();
    }

    public class WindowsService : ServiceBase
    {
        // Required designer variable.
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private System.ComponentModel.IContainer components = null;

        // http://msdn.microsoft.com/en-us/library/system.serviceprocess.serviceinstaller(v=vs.110).aspx

        private const int DefaultConsoleWidth = 120;
        private const int DefaultConsoleHeight = 25;
        private static readonly string Title = Assembly.GetEntryAssembly().GetName().Name;
        private static readonly ConsoleColor DefaultColor = Console.ForegroundColor;

        public static void Run(IWindowsService service)
        {
            var texlService = new WindowsService(service);
            if (Environment.UserInteractive)
            {
                ConfigureConsole();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   .oOo.   .oOo.   .oOo.    Starting {0}     .oOo.   .oOo.   .oOo.   ", service.Name);
                Console.ForegroundColor = DefaultColor;
                texlService.OnStart(null);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   .oOo.   .oOo.   .oOo.    Press ESC to stop    .oOo.   .oOo.   .oOo.   ");
                Console.ForegroundColor = DefaultColor;
                WairForEscape();

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("   .oOo.   .oOo.   .oOo.    Stopping {0}     .oOo.   .oOo.   .oOo.   ", service.Name);
                Console.ForegroundColor = DefaultColor;
                texlService.Stop();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("   .oOo.   .oOo.   .oOo.    Press ESC to exit    .oOo.   .oOo.   .oOo.   ");
                Console.ForegroundColor = DefaultColor;
                WairForEscape();
            }
            else
            {
                Run(texlService);
            }
        }

        private readonly IWindowsService _service;

        private WindowsService(IWindowsService service)
        {
            ServiceName = service.Name;
            _service = service;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            if (_service != null) _service.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnStart(string[] args)
        {
            _service.Start();
        }

        protected override void OnStop()
        {
            _service.Stop();
        }

        private static void WairForEscape()
        {
            ConsoleKey key;
            do
            {
                if (Console.KeyAvailable) key = Console.ReadKey(true).Key;
                else
                {
                    key = 0;
                    Thread.Sleep(200);
                }
            } while (key != ConsoleKey.Escape);
        }

        private static void ConfigureConsole()
        {
            Console.Title = Title;
            Console.SetWindowSize(DefaultConsoleWidth, DefaultConsoleHeight);
            Console.SetBufferSize(DefaultConsoleWidth, 100);

            var windowPosition = ConfigurationManager.AppSettings["window-position"];
            if (!string.IsNullOrEmpty(windowPosition))
            {
                var vals = windowPosition.Split(' ').Select(int.Parse).ToArray();
                if (vals.Length == 2) ConsoleUtils.SetWindowPos(vals[0], vals[1]);
            }
        }

        private static class ConsoleUtils
        {
            private static readonly IntPtr ThisConsole = GetConsoleWindow();

            public static void SetWindowPos(int left, int top)
            {
                SetWindowPos(ThisConsole, 0, left, top, 0, 0, 0x0001);
            }

            [DllImport("kernel32.dll", ExactSpelling = true)]
            private static extern IntPtr GetConsoleWindow();

            [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
            private static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        }
    }
}
