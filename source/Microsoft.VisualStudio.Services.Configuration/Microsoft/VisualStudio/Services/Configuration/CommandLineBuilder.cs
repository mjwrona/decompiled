// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.CommandLineBuilder
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class CommandLineBuilder
  {
    private readonly StringBuilder m_commandLine = new StringBuilder();
    private readonly StringBuilder m_hiddenCommandLine = new StringBuilder();

    public CommandLineBuilder AppendIf(bool condition, params string[] args)
    {
      if (condition)
      {
        foreach (string parameter in args)
          this.Append(parameter);
      }
      return this;
    }

    public CommandLineBuilder Append(string parameter) => this.AppendParamAndHidden(parameter, parameter);

    private static bool ContainsWhiteSpace(string str)
    {
      foreach (char c in str)
      {
        if (char.IsWhiteSpace(c))
          return true;
      }
      return false;
    }

    private string EscapeQuotesAndSlashes(string param)
    {
      bool flag1 = CommandLineBuilder.ContainsWhiteSpace(param);
      bool flag2 = flag1;
      int length1 = 2 * param.Length + 2;
      char[] chArray = new char[length1];
      int startIndex = length1;
      if (flag1)
        chArray[--startIndex] = '"';
      int length2 = param.Length;
      while (--length2 >= 0)
      {
        char ch = param[length2];
        chArray[--startIndex] = ch;
        switch (ch)
        {
          case '"':
            chArray[--startIndex] = '\\';
            flag2 = true;
            continue;
          case '\\':
            if (flag2)
            {
              chArray[--startIndex] = '\\';
              continue;
            }
            continue;
          default:
            flag2 = false;
            continue;
        }
      }
      if (flag1)
        chArray[--startIndex] = '"';
      return new string(chArray, startIndex, length1 - startIndex);
    }

    public CommandLineBuilder AppendParamAndHidden(string parameter, string hiddenParameter)
    {
      if (!string.IsNullOrEmpty(parameter))
      {
        parameter = this.EscapeQuotesAndSlashes(parameter);
        this.m_commandLine.AppendFormat(this.m_commandLine.Length > 0 ? " {0}" : "{0}", (object) parameter);
      }
      if (!string.IsNullOrEmpty(hiddenParameter))
      {
        hiddenParameter = this.EscapeQuotesAndSlashes(hiddenParameter);
        this.m_hiddenCommandLine.AppendFormat(this.m_hiddenCommandLine.Length > 0 ? " {0}" : "{0}", (object) hiddenParameter);
      }
      return this;
    }

    public CommandLineBuilder AppendFormat(string format, params object[] args) => this.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, format, args));

    public override string ToString() => this.m_commandLine.ToString();

    public string ToHiddenString() => this.m_hiddenCommandLine.ToString();
  }
}
