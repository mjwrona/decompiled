// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.FormatNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class FormatNode : FunctionNode
  {
    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      string str = this.Parameters[0].EvaluateString(context);
      int startIndex = 0;
      FormatNode.FormatResultBuilder formatResultBuilder = new FormatNode.FormatResultBuilder(this, context, this.CreateMemoryCounter(context));
      while (startIndex < str.Length)
      {
        int num = str.IndexOf('{', startIndex);
        int rbrace = str.IndexOf('}', startIndex);
        if (num >= 0 && (rbrace < 0 || rbrace > num))
        {
          if (ExpressionUtil.SafeCharAt(str, num + 1) == '{')
          {
            formatResultBuilder.Append(str.Substring(startIndex, num - startIndex + 1));
            startIndex = num + 2;
          }
          else
          {
            byte result1;
            int endIndex;
            string result2;
            if (rbrace <= num + 1 || !this.ReadArgIndex(str, num + 1, out result1, out endIndex) || !this.ReadFormatSpecifiers(str, endIndex + 1, out result2, out rbrace))
              throw new FormatException(ExpressionResources.InvalidFormatString((object) str));
            if ((int) result1 > this.Parameters.Count - 2)
              throw new FormatException(ExpressionResources.InvalidFormatArgIndex((object) str));
            if (num > startIndex)
              formatResultBuilder.Append(str.Substring(startIndex, num - startIndex));
            formatResultBuilder.Append((int) result1, result2);
            startIndex = rbrace + 1;
          }
        }
        else if (rbrace >= 0)
        {
          if (ExpressionUtil.SafeCharAt(str, rbrace + 1) != '}')
            throw new FormatException(ExpressionResources.InvalidFormatString((object) str));
          formatResultBuilder.Append(str.Substring(startIndex, rbrace - startIndex + 1));
          startIndex = rbrace + 2;
        }
        else
        {
          formatResultBuilder.Append(str.Substring(startIndex));
          break;
        }
      }
      return (object) formatResultBuilder.ToString();
    }

    private bool ReadArgIndex(string str, int startIndex, out byte result, out int endIndex)
    {
      int length = 0;
      while (char.IsDigit(ExpressionUtil.SafeCharAt(str, startIndex + length)))
        ++length;
      if (length < 1)
      {
        result = (byte) 0;
        endIndex = 0;
        return false;
      }
      endIndex = startIndex + length - 1;
      return byte.TryParse(str.Substring(startIndex, length), NumberStyles.None, (IFormatProvider) CultureInfo.InvariantCulture, out result);
    }

    private bool ReadFormatSpecifiers(
      string str,
      int startIndex,
      out string result,
      out int rbrace)
    {
      switch (ExpressionUtil.SafeCharAt(str, startIndex))
      {
        case ':':
          StringBuilder stringBuilder = new StringBuilder();
          int index = startIndex + 1;
          while (index < str.Length)
          {
            char ch = str[index];
            if (ch != '}')
            {
              stringBuilder.Append(ch);
              ++index;
            }
            else if (ExpressionUtil.SafeCharAt(str, index + 1) == '}')
            {
              stringBuilder.Append('}');
              index += 2;
            }
            else
            {
              result = stringBuilder.ToString();
              rbrace = index;
              return true;
            }
          }
          result = (string) null;
          rbrace = 0;
          return false;
        case '}':
          result = string.Empty;
          rbrace = startIndex;
          return true;
        default:
          result = (string) null;
          rbrace = 0;
          return false;
      }
    }

    private sealed class FormatResultBuilder
    {
      private readonly FormatNode.ArgValue[] m_cache;
      private readonly EvaluationContext m_context;
      private readonly MemoryCounter m_counter;
      private readonly FormatNode m_node;
      private readonly List<object> m_segments = new List<object>();

      internal FormatResultBuilder(
        FormatNode node,
        EvaluationContext context,
        MemoryCounter counter)
      {
        this.m_node = node;
        this.m_context = context;
        this.m_counter = counter;
        this.m_cache = new FormatNode.ArgValue[node.Parameters.Count - 1];
      }

      public override string ToString() => string.Join(string.Empty, this.m_segments.Select<object, string>((Func<object, string>) (obj => obj is Lazy<string> lazy ? lazy.Value : obj as string)));

      internal void Append(string value)
      {
        if (value == null || value.Length <= 0)
          return;
        this.m_counter.Add(value);
        this.m_segments.Add((object) value);
      }

      internal void Append(int argIndex, string formatSpecifiers) => this.m_segments.Add((object) new Lazy<string>((Func<string>) (() =>
      {
        FormatNode.ArgValue argValue = this.m_cache[argIndex];
        if (argValue == null)
        {
          EvaluationResult evaluationResult = this.m_node.Parameters[argIndex + 1].Evaluate(this.m_context);
          argValue = new FormatNode.ArgValue(evaluationResult, evaluationResult.ConvertToString(this.m_context));
          this.m_cache[argIndex] = argValue;
        }
        string str;
        if (string.IsNullOrEmpty(formatSpecifiers))
        {
          str = argValue.StringResult;
        }
        else
        {
          if (argValue.EvaluationResult.Kind != ValueKind.DateTime)
            throw new FormatException(ExpressionResources.InvalidFormatSpecifiers((object) formatSpecifiers, (object) argValue.EvaluationResult.Kind));
          str = this.FormatDateTime((DateTimeOffset) argValue.EvaluationResult.Value, formatSpecifiers);
        }
        if (!string.IsNullOrEmpty(str))
          this.m_counter.Add(str);
        return str;
      })));

      private string FormatDateTime(DateTimeOffset dateTime, string specifiers)
      {
        StringBuilder stringBuilder = new StringBuilder();
        int index = 0;
        while (true)
        {
          string nextSpecifier = this.GetNextSpecifier(specifiers, ref index);
          if (!string.IsNullOrEmpty(nextSpecifier))
          {
            if (nextSpecifier != null)
            {
              switch (nextSpecifier.Length)
              {
                case 1:
                  switch (nextSpecifier[0])
                  {
                    case 'H':
                    case 'K':
                    case 'M':
                    case 'd':
                    case 'f':
                    case 'm':
                    case 's':
                      stringBuilder.Append(dateTime.ToString("%" + nextSpecifier));
                      continue;
                    default:
                      goto label_24;
                  }
                case 2:
                  switch (nextSpecifier[0])
                  {
                    case 'H':
                      if (nextSpecifier == "HH")
                        break;
                      goto label_24;
                    case 'M':
                      if (nextSpecifier == "MM")
                        break;
                      goto label_24;
                    case 'd':
                      if (nextSpecifier == "dd")
                        break;
                      goto label_24;
                    case 'f':
                      if (nextSpecifier == "ff")
                        break;
                      goto label_24;
                    case 'm':
                      if (nextSpecifier == "mm")
                        break;
                      goto label_24;
                    case 's':
                      if (nextSpecifier == "ss")
                        break;
                      goto label_24;
                    case 'y':
                      if (nextSpecifier == "yy")
                        break;
                      goto label_24;
                    default:
                      goto label_24;
                  }
                  break;
                case 3:
                  switch (nextSpecifier[0])
                  {
                    case 'f':
                      if (nextSpecifier == "fff")
                        break;
                      goto label_24;
                    case 'z':
                      if (nextSpecifier == "zzz")
                        break;
                      goto label_24;
                    default:
                      goto label_24;
                  }
                  break;
                case 4:
                  switch (nextSpecifier[0])
                  {
                    case 'f':
                      if (nextSpecifier == "ffff")
                        break;
                      goto label_24;
                    case 'y':
                      if (nextSpecifier == "yyyy")
                        break;
                      goto label_24;
                    default:
                      goto label_24;
                  }
                  break;
                case 5:
                  if (nextSpecifier == "fffff")
                    break;
                  goto label_24;
                case 6:
                  if (nextSpecifier == "ffffff")
                    break;
                  goto label_24;
                case 7:
                  if (!(nextSpecifier == "fffffff"))
                    goto label_24;
                  else
                    break;
                default:
                  goto label_24;
              }
              stringBuilder.Append(dateTime.ToString(nextSpecifier));
              continue;
            }
label_24:
            if (nextSpecifier[0] == '\\')
              stringBuilder.Append(nextSpecifier[1]);
            else if (nextSpecifier[0] == ' ')
              stringBuilder.Append(nextSpecifier);
            else
              break;
          }
          else
            goto label_29;
        }
        throw new FormatException(ExpressionResources.InvalidFormatSpecifiers((object) specifiers, (object) ValueKind.DateTime));
label_29:
        return stringBuilder.ToString();
      }

      private string GetNextSpecifier(string specifiers, ref int index)
      {
        if (index >= specifiers.Length)
          return string.Empty;
        int startIndex = index;
        char specifier = specifiers[index++];
        if (specifier == '\\')
        {
          if (index >= specifiers.Length)
            throw new FormatException(ExpressionResources.InvalidFormatSpecifiers((object) specifiers, (object) ValueKind.DateTime));
          ++index;
        }
        else
        {
          while (index < specifiers.Length && (int) specifiers[index] == (int) specifier)
            ++index;
        }
        return specifiers.Substring(startIndex, index - startIndex);
      }
    }

    private sealed class ArgValue
    {
      public ArgValue(EvaluationResult evaluationResult, string stringResult)
      {
        this.EvaluationResult = evaluationResult;
        this.StringResult = stringResult;
      }

      public EvaluationResult EvaluationResult { get; }

      public string StringResult { get; }
    }
  }
}
