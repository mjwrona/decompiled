// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.ConsoleUtils
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.TeamFoundation.Client
{
  internal static class ConsoleUtils
  {
    private static int s_redirectedState;
    private static Encoding s_redirectedOutputEncoding;

    internal static bool IsPrintableConsoleChar(char ch)
    {
      bool flag = true;
      if (char.IsControl(ch))
      {
        switch (ch)
        {
          case '\t':
          case '\n':
          case '\r':
            break;
          default:
            flag = false;
            break;
        }
      }
      return flag;
    }

    internal static Encoding RedirectedOutputEncoding
    {
      get
      {
        if (ConsoleUtils.s_redirectedOutputEncoding == null)
        {
          int result = 0;
          string environmentVariable = Environment.GetEnvironmentVariable("TF_INTERNAL_OUTPUT_CP");
          if (environmentVariable != null)
            int.TryParse(environmentVariable, out result);
          ConsoleUtils.s_redirectedOutputEncoding = result == 65001 ? (Encoding) new UTF8Encoding(false) : Encoding.GetEncoding(result);
        }
        return ConsoleUtils.s_redirectedOutputEncoding;
      }
    }

    internal static bool Write(string message, bool toStandardOutput) => ConsoleUtils.Write(message.ToCharArray(), toStandardOutput);

    internal static bool Write(char[] chars, bool toStandardOutput) => ConsoleUtils.Write(chars, chars.Length, toStandardOutput);

    internal static bool Write(char[] chars, int numCharsToWrite, bool toStandardOutput)
    {
      if (chars == null)
        return true;
      numCharsToWrite = Math.Min(numCharsToWrite, chars.Length);
      bool flag;
      if (numCharsToWrite > 0)
      {
        int nStdHandle = toStandardOutput ? -11 : -12;
        ConsoleHandle consoleHandle = new ConsoleHandle(nStdHandle);
        if (ConsoleUtils.IsRedirected(nStdHandle))
        {
          byte[] bytes = ConsoleUtils.RedirectedOutputEncoding.GetBytes(chars, 0, numCharsToWrite);
          flag = Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteFile(consoleHandle.Handle, bytes, bytes.Length);
        }
        else
        {
          char[] chars1 = new char[numCharsToWrite];
          for (int index = 0; index < numCharsToWrite; ++index)
            chars1[index] = ConsoleUtils.IsPrintableConsoleChar(chars[index]) ? chars[index] : ' ';
          flag = Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteConsole(consoleHandle.Handle, chars1);
        }
      }
      else
        flag = true;
      return flag;
    }

    internal static bool Write(byte[] bytes, bool toStandardOutput) => ConsoleUtils.Write(bytes, bytes.Length, toStandardOutput);

    internal static bool Write(byte[] bytes, int numBytesToWrite, bool toStandardOutput)
    {
      if (bytes == null)
        return true;
      numBytesToWrite = Math.Min(numBytesToWrite, bytes.Length);
      bool flag;
      if (numBytesToWrite > 0)
      {
        int nStdHandle = toStandardOutput ? -11 : -12;
        ConsoleHandle consoleHandle = new ConsoleHandle(nStdHandle);
        if (ConsoleUtils.IsRedirected(nStdHandle))
        {
          flag = Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteFile(consoleHandle.Handle, bytes, numBytesToWrite);
        }
        else
        {
          byte[] bytes1 = new byte[numBytesToWrite];
          for (int index = 0; index < numBytesToWrite; ++index)
          {
            char ch = (char) bytes[index];
            bytes1[index] = ConsoleUtils.IsPrintableConsoleChar(ch) ? bytes[index] : (byte) 32;
          }
          flag = Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteFile(consoleHandle.Handle, bytes1, numBytesToWrite);
        }
      }
      else
        flag = true;
      return flag;
    }

    internal static bool IsStdInRedirected => ConsoleUtils.IsRedirected(-10);

    internal static bool IsStdOutRedirected => ConsoleUtils.IsRedirected(-11);

    internal static bool IsStdErrRedirected => ConsoleUtils.IsRedirected(-12);

    private static unsafe bool IsRedirected(int nStdHandle)
    {
      int num1;
      switch (nStdHandle)
      {
        case -12:
          num1 = 4;
          break;
        case -11:
          num1 = 2;
          break;
        case -10:
          num1 = 1;
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof (nStdHandle));
      }
      int num2 = num1 << 4;
      if ((num1 & ConsoleUtils.s_redirectedState) == 0)
      {
        bool flag = false;
        ConsoleHandle consoleHandle = new ConsoleHandle(nStdHandle);
        uint num3;
        if (consoleHandle.IsValid)
        {
          if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.GetFileType(consoleHandle.Handle) == 2)
          {
            switch (nStdHandle)
            {
              case -12:
              case -11:
                if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.WriteConsole(consoleHandle.Handle, (char*) null, 0U, out num3, IntPtr.Zero) && Marshal.GetLastWin32Error() == 6)
                {
                  flag = true;
                  break;
                }
                break;
              case -10:
                if (!Microsoft.TeamFoundation.Common.Internal.NativeMethods.PeekConsoleInput(consoleHandle.Handle, IntPtr.Zero, 0U, out num3) && Marshal.GetLastWin32Error() == 6)
                {
                  flag = true;
                  break;
                }
                break;
            }
          }
          else
            flag = true;
        }
        ConsoleUtils.s_redirectedState |= num1;
        if (flag)
          ConsoleUtils.s_redirectedState |= num2;
      }
      return (ConsoleUtils.s_redirectedState & num2) == num2;
    }

    [Flags]
    private enum ConsoleRedirectFlags
    {
      StdIn = 1,
      StdOut = 2,
      StdErr = 4,
    }
  }
}
