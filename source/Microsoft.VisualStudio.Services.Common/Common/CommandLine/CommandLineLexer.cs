// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.CommandLine.CommandLineLexer
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Common.CommandLine
{
  internal static class CommandLineLexer
  {
    public static IEnumerable<string> Lex(this string commandLine)
    {
      CommandLineLexer.LexerState lexerState = CommandLineLexer.LexerState.Default;
      CommandLineLexer.Buffer buffer = new CommandLineLexer.Buffer();
      string str = commandLine;
      for (int index = 0; index < str.Length; ++index)
      {
        char ch = str[index];
        switch (lexerState)
        {
          case CommandLineLexer.LexerState.Default:
            switch (ch)
            {
              case '\t':
              case ' ':
                continue;
              case '"':
                lexerState = CommandLineLexer.LexerState.Quoted;
                continue;
              default:
                buffer.AppendChar(ch);
                lexerState = CommandLineLexer.LexerState.Argument;
                continue;
            }
          case CommandLineLexer.LexerState.Argument:
            switch (ch)
            {
              case '\t':
              case ' ':
                yield return buffer.Consume();
                lexerState = CommandLineLexer.LexerState.Default;
                continue;
              case '"':
                if (buffer.AppendQuote())
                {
                  lexerState = CommandLineLexer.LexerState.Quoted;
                  continue;
                }
                continue;
              default:
                buffer.AppendChar(ch);
                continue;
            }
          case CommandLineLexer.LexerState.Quoted:
            if (ch == '"')
            {
              if (buffer.AppendQuote())
              {
                lexerState = CommandLineLexer.LexerState.EndQuotedArgument;
                break;
              }
              break;
            }
            buffer.AppendChar(ch);
            break;
          case CommandLineLexer.LexerState.EndQuotedArgument:
            switch (ch)
            {
              case '\t':
              case ' ':
                yield return buffer.Consume();
                lexerState = CommandLineLexer.LexerState.Default;
                continue;
              case '"':
                buffer.AppendNormalChar(ch);
                lexerState = CommandLineLexer.LexerState.Quoted;
                continue;
              default:
                buffer.AppendChar(ch);
                lexerState = CommandLineLexer.LexerState.Argument;
                continue;
            }
        }
      }
      str = (string) null;
      if (lexerState != CommandLineLexer.LexerState.Default)
        yield return buffer.Consume();
    }

    public static IEnumerable<string> Lex() => (IEnumerable<string>) Environment.GetCommandLineArgs();

    public static string Escape(IEnumerable<string> arguments) => string.Join(" ", arguments.Select<string, string>((Func<string, string>) (argument =>
    {
      if (!argument.Any<char>((Func<char, bool>) (ch => ch == ' ' || ch == '\t' || ch == '"')) && !string.IsNullOrEmpty(argument))
        return argument;
      if (argument == null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append('"');
      int count = 0;
      foreach (char ch in argument)
      {
        switch (ch)
        {
          case '"':
            stringBuilder.Append(new string('\\', 2 * count + 1));
            count = 0;
            stringBuilder.Append(ch);
            break;
          case '\\':
            ++count;
            break;
          default:
            stringBuilder.Append(new string('\\', count));
            count = 0;
            stringBuilder.Append(ch);
            break;
        }
      }
      stringBuilder.Append(new string('\\', count));
      stringBuilder.Append('"');
      return stringBuilder.ToString();
    })));

    private enum LexerState
    {
      Default,
      Argument,
      Quoted,
      EndQuotedArgument,
    }

    private sealed class Buffer
    {
      private string result;
      private int backslashes;

      public Buffer()
      {
        this.result = string.Empty;
        this.backslashes = 0;
      }

      private void Normalize()
      {
        this.result += new string('\\', this.backslashes);
        this.backslashes = 0;
      }

      public void AppendNormalChar(char ch)
      {
        this.Normalize();
        this.result += ch.ToString();
      }

      public void AppendBackslash() => ++this.backslashes;

      public bool AppendQuote()
      {
        this.result += new string('\\', this.backslashes / 2);
        int num = this.backslashes % 2 == 0 ? 1 : 0;
        this.backslashes = 0;
        if (num != 0)
          return num != 0;
        this.result += "\"";
        return num != 0;
      }

      public void AppendChar(char ch)
      {
        if (ch == '\\')
          this.AppendBackslash();
        else
          this.AppendNormalChar(ch);
      }

      public string Consume()
      {
        this.Normalize();
        string result = this.result;
        this.result = string.Empty;
        return result;
      }
    }
  }
}
