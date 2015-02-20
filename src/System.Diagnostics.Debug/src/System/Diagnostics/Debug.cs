// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace System.Diagnostics
{
    /// <summary>
    /// Provides a set of properties and methods for debugging code.
    /// </summary>
    public static class Debug
    {
        private static readonly object s_ForLock = new Object();

        // This is the number of characters that OutputDebugstring chunks at.
        private const int InternalWriteSize = 4091;

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition)
        {
            Assert(condition, string.Empty, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message)
        {
            Assert(condition, message, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Security.SecuritySafeCritical]
        public static void Assert(bool condition, string message, string detailMessage)
        {
            if (!condition)
            {
                string stackTrace;

                try
                {
                    stackTrace = Environment.StackTrace;
                }
                catch
                {
                    stackTrace = "";
                }

                WriteAssert(stackTrace, message, detailMessage);
                AssertWrapper.ShowAssert(stackTrace, message, detailMessage);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Fail(string message)
        {
            Assert(false, message, string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Fail(string message, string detailMessage)
        {
            Assert(false, message, detailMessage);
        }

        private static void WriteAssert(string stackTrace, string message, string detailMessage)
        {
            string assertMessage = Res.Strings.DebugAssertBanner + Environment.NewLine
                                            + Res.Strings.DebugAssertShortMessage + Environment.NewLine
                                            + message + Environment.NewLine
                                            + Res.Strings.DebugAssertLongMessage + Environment.NewLine +
                                            detailMessage + Environment.NewLine
                                            + stackTrace;
            WriteLine(assertMessage);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Assert(bool condition, string message, string detailMessageFormat, params object[] args)
        {
            Assert(condition, message, string.Format(detailMessageFormat, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string message)
        {
            message = message + "\r\n"; // Use Windows end line on *all* Platforms
            Write(message);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(string message)
        {
            // We don't want output from multiple threads to be interleaved.
            lock (s_ForLock)
            {
                // really huge messages mess up both VS and dbmon, so we chop it up into 
                // reasonable chunks if it's too big
                if (message == null || message.Length <= InternalWriteSize)
                {
                    InternalWrite(message);
                }
                else
                {
                    int offset;
                    for (offset = 0; offset < message.Length - InternalWriteSize; offset += InternalWriteSize)
                    {
                        InternalWrite(message.Substring(offset, InternalWriteSize));
                    }
                    InternalWrite(message.Substring(offset));
                }
            }

        }

        [System.Security.SecuritySafeCritical]
        private static void InternalWrite(string message)
        {
           Interop.mincore.OutputDebugString(message ?? string.Empty);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(object value)
        {
            WriteLine((value == null) ? string.Empty : value.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(object value, string category)
        {
            WriteLine((value == null) ? string.Empty : value.ToString(), category);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string format, params object[] args)
        {
            WriteLine(string.Format(null, format, args));
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLine(string message, string category)
        {
            if (category == null)
            {
                WriteLine(message);
            }
            else
            {
                WriteLine(category + ":" + ((message == null) ? string.Empty : message));
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(object value)
        {
            Write((value == null) ? string.Empty : value.ToString());
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(string message, string category)
        {
            if (category == null)
            {
                Write(message);
            }
            else
            {
                Write(category + ":" + ((message == null) ? string.Empty : message));
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void Write(object value, string category)
        {
            Write((value == null) ? string.Empty : value.ToString(), category);
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, string message)
        {
            if (condition)
            {
                Write(message);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value)
        {
            if (condition)
            {
                Write(value);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, string message, string category)
        {
            if (condition)
            {
                Write(message, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteIf(bool condition, object value, string category)
        {
            if (condition)
            {
                Write(value, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value)
        {
            if (condition)
            {
                WriteLine(value);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, object value, string category)
        {
            if (condition)
            {
                WriteLine(value, category);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string value)
        {
            if (condition)
            {
                WriteLine(value);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        public static void WriteLineIf(bool condition, string value, string category)
        {
            if (condition)
            {
                WriteLine(value, category);
            }
        }
    }
}
