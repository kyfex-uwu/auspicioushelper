using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace Celeste.Mod.auspicioushelper;
public static class DebugConsole {
    private static Thread consoleThread;
    private static readonly BlockingCollection<string> messageQueue = new BlockingCollection<string>();
    public static bool open;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AllocConsole();

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    public static void Open() {
        if (consoleThread != null) return;

        if (!AllocConsole()) {
            throw new InvalidOperationException("Failed to allocate a console.");
        }

        consoleThread = new Thread(() => {
            Console.Title = "Debug Consolew"; // This line works only if the console is attached
            StreamWriter writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            writer.WriteLine("Console thread started!");
            while (true) {
                string message = messageQueue.Take();
                if (message == null) break;
                writer.WriteLine(message);
            }
        }) {
            IsBackground = true
        };

        consoleThread.Start();
        open = true;
    }

    public static void Write(string message) {
        if(!open) return;
        if (consoleThread == null) throw new InvalidOperationException("Debug console not open.");
        try{
            messageQueue.Add(message);
        }catch(Exception){

        }
    }
    public static void Write(char[,] arr){
        string res = "";
        for(int i=0; i<arr.GetLength(0); i++){
            if(i != 0) res+='\n';
            for(int j=0; j<arr.GetLength(1); j++){
                res+=arr[i,j];
            }
        }
        Write(res);
    }
    public static void Close() {
        if(!open) return;
        if (consoleThread == null) return;
        messageQueue.Add(null); // Signal the thread to exit (doesn't work??)
        FreeConsole();
        consoleThread = null;
        open = false;
    }
}
