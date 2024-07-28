// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.JSON
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

using System.ComponentModel;
using System.Text;

namespace Microsoft.Ajax.Utilities
{
  public class JSON
  {
    private string m_jsonText;
    private int m_currentIndex;
    private StringBuilder m_builder;

    private bool IsAtEnd => this.SkipSpace() == char.MinValue;

    private char Current => this.m_currentIndex >= this.m_jsonText.Length ? char.MinValue : this.m_jsonText[this.m_currentIndex];

    private string Minified => this.m_builder.ToString();

    private JSON(string jsonText)
    {
      this.m_jsonText = jsonText;
      this.m_currentIndex = 0;
      this.m_builder = new StringBuilder();
    }

    public static string Validate(string jsonText)
    {
      JSON json = new JSON(jsonText);
      return !json.IsValidValue() || !json.IsAtEnd ? (string) null : json.Minified;
    }

    private bool IsValidValue()
    {
      bool flag = false;
      switch (this.SkipSpace())
      {
        case '"':
          flag = this.IsValidString();
          break;
        case '-':
        case '0':
        case '1':
        case '2':
        case '3':
        case '4':
        case '5':
        case '6':
        case '7':
        case '8':
        case '9':
          flag = this.IsValidNumber();
          break;
        case '[':
          flag = this.IsValidArray();
          break;
        case 'f':
          flag = this.IsFollowedBy("alse");
          break;
        case 'n':
          flag = this.IsFollowedBy("ull");
          break;
        case 't':
          flag = this.IsFollowedBy("rue");
          break;
        case '{':
          flag = this.IsValidObject();
          break;
      }
      return flag;
    }

    [Localizable(false)]
    private bool IsFollowedBy(string text)
    {
      for (int index = 0; index < text.Length; ++index)
      {
        if ((int) this.Peek(index + 1) != (int) text[index])
          return false;
      }
      int count = text.Length + 1;
      this.m_builder.Append(this.m_jsonText, this.m_currentIndex, count);
      this.m_currentIndex += count;
      return true;
    }

    private bool IsValidNumber()
    {
      bool flag = false;
      int currentIndex = this.m_currentIndex;
      char ch1 = this.Current;
      if (ch1 == '-')
        ch1 = this.Next();
      if ('0' <= ch1 && ch1 <= '9')
      {
        flag = true;
        char ch2;
        if (ch1 == '0')
        {
          if ('0' <= (ch2 = this.Next()) && ch2 <= '9')
            flag = false;
        }
        else
        {
          while ('0' <= (ch2 = this.Next()) && ch2 <= '9')
            ;
        }
        if (flag && ch2 == '.')
        {
          ch2 = this.Next();
          if ('0' <= ch2 && ch2 <= '9')
          {
            while ('0' <= (ch2 = this.Next()) && ch2 <= '9')
              ;
          }
          else
            flag = false;
        }
        if (flag && ch2 == 'e' || ch2 == 'E')
        {
          char ch3 = this.Next();
          switch (ch3)
          {
            case '+':
            case '-':
              ch3 = this.Next();
              break;
          }
          if ('0' <= ch3 && ch3 <= '9')
          {
            char ch4;
            while ('0' <= (ch4 = this.Next()) && ch4 <= '9')
              ;
          }
          else
            flag = false;
        }
      }
      this.m_builder.Append(this.m_jsonText, currentIndex, this.m_currentIndex - currentIndex);
      return flag;
    }

    private bool IsValidString()
    {
      int currentIndex = this.m_currentIndex;
      char ch1 = this.Next();
      while (true)
      {
        switch (ch1)
        {
          case char.MinValue:
          case '"':
            goto label_10;
          case '\\':
            switch (this.Next())
            {
              case '"':
              case '/':
              case '\\':
              case 'b':
              case 'f':
              case 'n':
              case 'r':
              case 't':
                break;
              case 'u':
                for (int index = 0; index < 4; ++index)
                {
                  char ch2 = this.Next();
                  if (('0' > ch2 || ch2 > '9') && ('A' > ch2 || ch2 > 'F') && ('a' > ch2 || ch2 > 'f'))
                    return false;
                }
                break;
              default:
                goto label_7;
            }
            break;
        }
        ch1 = this.Next();
      }
label_7:
      return false;
label_10:
      if (ch1 != '"')
        return false;
      int num = (int) this.Next();
      this.m_builder.Append(this.m_jsonText, currentIndex, this.m_currentIndex - currentIndex);
      return true;
    }

    private bool IsValidArray()
    {
      int num1 = (int) this.Next();
      this.m_builder.Append('[');
      if (this.SkipSpace() != ']')
      {
        if (!this.IsValidValue())
          return false;
        while (this.SkipSpace() == ',')
        {
          this.m_builder.Append(',');
          int num2 = (int) this.Next();
          if (!this.IsValidValue())
            return false;
        }
      }
      if (this.SkipSpace() != ']')
        return false;
      int num3 = (int) this.Next();
      this.m_builder.Append(']');
      return true;
    }

    private bool IsValidObject()
    {
      int num1 = (int) this.Next();
      this.m_builder.Append('{');
      if (this.SkipSpace() != '}')
      {
        if (!this.IsValidProperty())
          return false;
        while (this.SkipSpace() == ',')
        {
          int num2 = (int) this.Next();
          int num3 = (int) this.SkipSpace();
          this.m_builder.Append(',');
          if (!this.IsValidProperty())
            return false;
        }
      }
      if (this.SkipSpace() != '}')
        return false;
      int num4 = (int) this.Next();
      this.m_builder.Append('}');
      return true;
    }

    private bool IsValidProperty()
    {
      if (!this.IsValidString() || this.SkipSpace() != ':')
        return false;
      int num = (int) this.Next();
      this.m_builder.Append(':');
      return this.IsValidValue();
    }

    private char Peek(int offset = 0) => this.m_currentIndex + offset >= this.m_jsonText.Length ? char.MinValue : this.m_jsonText[this.m_currentIndex + offset];

    private char Next() => ++this.m_currentIndex >= this.m_jsonText.Length ? char.MinValue : this.m_jsonText[this.m_currentIndex];

    private char SkipSpace()
    {
      char ch = this.Current;
      while (ch == '\t' || ch == '\n' || ch == '\r' || ch == ' ')
        ch = this.Next();
      return ch;
    }
  }
}
