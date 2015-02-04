// This file is part of CodeWatchdog.
// https://bitbucket.org/flberger/codewatchdog

using System;

namespace CodeWatchdog
{
    /// <summary>
    /// A simple multi-level console logger.
    /// </summary>
    public class Logging
    {
        public static bool debugMode = true;
        
        /// <summary>
        /// Log a debug-level message.
        /// </summary>
        public static void Debug(string message)
        {
            if (debugMode)
            {
                // Console.WriteLine("[DEBUG] " + message);
            
                return;
            }
        }
        
        /// <summary>
        /// Log an info-level message.
        /// </summary>
        public static void Info(string message)
        {
            if (debugMode)
            {
                Console.WriteLine("[INFO] " + message);
            
                return;
            }
        }
        
        /// <summary>
        /// Log an error-level message.
        /// </summary>
        public static void Error(string message)
        {
            if (debugMode)
            {
                Console.WriteLine("[ERROR] " + message);
                
                return;
            }
        }
    }
}
