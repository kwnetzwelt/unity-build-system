using System;

namespace CodeWatchdog
{
    public class Logging
    {
        public static bool DebugMode = true;
        
        public static void Debug(string message)
        {
            if (DebugMode)
            {
                //Console.WriteLine("[DEBUG] " + message);
            
                return;
            }
        }
        
        public static void Info(string message)
        {
            if (DebugMode)
            {
                Console.WriteLine("[INFO] " + message);
            
                return;
            }
        }
        
        public static void Error(string message)
        {
            if (DebugMode)
            {
                Console.WriteLine("[ERROR] " + message);
                
                return;
            }
        }
    }
}

