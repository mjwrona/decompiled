// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Parser
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4A32169-9B8B-4726-A9F6-41569B7C3273
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Client.QueryLanguage.dll

using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class Parser
  {
    private Parser()
    {
    }

    public static List<Node> ParseLexems(string s)
    {
      List<Node> lexems = new List<Node>();
      int length = s.Length;
      int num1 = 0;
      while (num1 < length)
      {
        if (char.IsWhiteSpace(s, num1) || char.IsControl(s, num1))
          ++num1;
        else if (char.IsLetter(s, num1) || s[num1] == '_')
        {
          int startIndex = num1;
          while (num1 < length && (char.IsLetterOrDigit(s, num1) || s[num1] == '_' || s[num1] == '.'))
            ++num1;
          if (s[num1 - 1] == '.')
            --num1;
          string s1 = s.Substring(startIndex, num1 - startIndex);
          bool result;
          Node node = !Tools.TranslateBoolToken(s1, out result) ? (Node) new NodeName(s1) : (Node) new NodeBoolValue(result);
          node.StartOffset = startIndex;
          node.EndOffset = num1;
          lexems.Add(node);
        }
        else if (s[num1] == '@')
        {
          int startIndex = ++num1;
          while (num1 < length && char.IsLetterOrDigit(s, num1))
            ++num1;
          NodeVariable nodeVariable = new NodeVariable(s.Substring(startIndex, num1 - startIndex));
          nodeVariable.StartOffset = startIndex - 1;
          nodeVariable.EndOffset = num1;
          lexems.Add((Node) nodeVariable);
        }
        else if (s[num1] == '[')
        {
          int startIndex = ++num1;
          int num2 = length;
          bool flag = true;
          for (; num1 < length; ++num1)
          {
            if (s[num1] == ']')
            {
              num2 = num1++;
              flag = false;
              break;
            }
          }
          NodeName nodeName = new NodeName(s.Substring(startIndex, num2 - startIndex));
          nodeName.StartOffset = startIndex - 1;
          nodeName.EndOffset = num2 + 1;
          if (flag)
            throw new SyntaxException((Node) nodeName, SyntaxError.ExpectingClosingSquareBracket);
          if (startIndex == num2)
            throw new SyntaxException((Node) nodeName, SyntaxError.EmptyName);
          lexems.Add((Node) nodeName);
        }
        else if ((s[num1] == '-' || s[num1] == '+') && num1 + 1 < length && char.IsDigit(s, num1 + 1) || char.IsDigit(s, num1))
        {
          int startIndex = num1;
          if (s[num1] == '-' || s[num1] == '+')
            ++num1;
          while (num1 < length && char.IsDigit(s[num1]))
            ++num1;
          if (num1 < length && s[num1] == '.')
          {
            ++num1;
            while (num1 < length && char.IsDigit(s[num1]))
              ++num1;
          }
          if (num1 < length && (s[num1] == 'e' || s[num1] == 'E'))
          {
            ++num1;
            if (num1 < length && (s[num1] == '-' || s[num1] == '+'))
              ++num1;
            while (num1 < length && char.IsDigit(s[num1]))
              ++num1;
          }
          NodeNumber nodeNumber = new NodeNumber(s.Substring(startIndex, num1 - startIndex));
          nodeNumber.StartOffset = startIndex;
          nodeNumber.EndOffset = num1;
          lexems.Add((Node) nodeNumber);
        }
        else if (s[num1] == '\'' || s[num1] == '"')
        {
          char ch = s[num1++];
          int startIndex = num1;
          int num3 = length;
          bool flag = true;
          for (; num1 < length; ++num1)
          {
            if ((int) s[num1] == (int) ch)
            {
              ++num1;
              if (num1 == length || (int) s[num1] != (int) ch)
              {
                num3 = num1 - 1;
                flag = false;
                break;
              }
            }
          }
          string newValue = ch.ToString();
          NodeString nodeString = new NodeString(s.Substring(startIndex, num3 - startIndex).Replace(newValue + newValue, newValue));
          nodeString.StartOffset = startIndex - 1;
          nodeString.EndOffset = num3 + 1;
          if (flag)
            throw new SyntaxException((Node) nodeString, SyntaxError.ExpectingClosingQuote);
          lexems.Add((Node) nodeString);
        }
        else if (num1 + 1 < length && (s[num1] == '<' && (s[num1 + 1] == '=' || s[num1 + 1] == '>') || s[num1] == '>' && s[num1 + 1] == '=' || s[num1] == '!' && s[num1 + 1] == '=' || s[num1] == '=' && (s[num1 + 1] == '=' || s[num1 + 1] == '>' || s[num1 + 1] == '<') || s[num1] == '&' && s[num1 + 1] == '&' || s[num1] == '|' && s[num1 + 1] == '|'))
        {
          NodeOperation nodeOperation = new NodeOperation(s.Substring(num1, 2));
          nodeOperation.StartOffset = num1;
          nodeOperation.EndOffset = num1 + 2;
          lexems.Add((Node) nodeOperation);
          num1 += 2;
        }
        else
        {
          NodeOperation nodeOperation = new NodeOperation(s.Substring(num1, 1));
          nodeOperation.StartOffset = num1;
          nodeOperation.EndOffset = num1 + 1;
          lexems.Add((Node) nodeOperation);
          ++num1;
        }
      }
      return lexems;
    }

    public static NodeSelect ParseSyntax(string wiql)
    {
      Scanner scanner = new Scanner(Parser.ParseLexems(wiql));
      NodeSelect syntax = scanner.Scan();
      scanner.CheckTail();
      return syntax;
    }
  }
}
