// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.NotificationHubs.Messaging.ExpressionEvaluator
// Assembly: Microsoft.Azure.NotificationHubs, Version=2.16.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 1F43328A-44A2-48DE-9CBC-06F3C4A41C2A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.NotificationHubs.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Azure.NotificationHubs.Messaging
{
  internal static class ExpressionEvaluator
  {
    public const string InternalBodyProperty = "READ_ONCE_BODY_PUSHNOTIFICATION";
    public const string InternalJsonNavigationProperty = "FIRST_LEVEL_JSON_NAVIGATION_PUSHNOTIFICATION";
    private const string BodyExpression = "$body";
    public const int MaxLengthOfPropertyName = 120;
    public static readonly Regex PropertyNameRegEx = new Regex("^[A-Za-z0-9_]+$");

    public static ExpressionEvaluator.ExpressionType Validate(string expression, ApiVersion version)
    {
      ExpressionEvaluator.ExpressionType expressionType;
      List<ExpressionEvaluator.Token> tokenList = ExpressionEvaluator.ValidateAndTokenize(expression, out expressionType);
      if (version > ApiVersion.Three && tokenList.Find((Predicate<ExpressionEvaluator.Token>) (t => t.Type == ExpressionEvaluator.TokenType.Body)) != null)
        throw new InvalidDataContractException(SRClient.BodyIsNotSupportedExpression);
      return expressionType;
    }

    private static List<ExpressionEvaluator.Token> ValidateAndTokenize(
      string expression,
      out ExpressionEvaluator.ExpressionType expressionType)
    {
      expressionType = ExpressionEvaluator.PeekExpressionType(expression);
      if (expressionType == ExpressionEvaluator.ExpressionType.Literal)
        return new List<ExpressionEvaluator.Token>();
      List<ExpressionEvaluator.Token> tokenList = new List<ExpressionEvaluator.Token>();
      string str1 = expression;
      if (expressionType == ExpressionEvaluator.ExpressionType.Composite)
      {
        string str2 = expression;
        str1 = str2[str2.Length - 1] == '}' ? expression.Substring(1, expression.Length - 2).TrimEnd() : throw new InvalidDataContractException(SRClient.ExpressionMissingClosingParenthesesNoToken((object) expression));
      }
      ExpressionEvaluator.Token token1;
      string tokenBegin;
      string str3;
      while (true)
      {
        token1 = new ExpressionEvaluator.Token();
        tokenBegin = str1.TrimStart();
        if (tokenBegin.Length >= 3)
        {
          int num;
          switch (tokenBegin[0])
          {
            case '"':
              token1.Type = ExpressionEvaluator.TokenType.DoubleLiteral;
              num = ExpressionEvaluator.ExtractLiteral(expression, tokenBegin, token1);
              break;
            case '#':
              token1.Type = ExpressionEvaluator.TokenType.Hash;
              num = ExpressionEvaluator.ExtractToken(expression, tokenBegin, token1);
              break;
            case '$':
              if (tokenBegin.ToLowerInvariant().StartsWith("$body", StringComparison.OrdinalIgnoreCase))
              {
                num = "$body".Length - 1;
                token1.Type = ExpressionEvaluator.TokenType.Body;
                break;
              }
              token1.Type = ExpressionEvaluator.TokenType.Dollar;
              num = ExpressionEvaluator.ExtractToken(expression, tokenBegin, token1);
              break;
            case '%':
              token1.Type = ExpressionEvaluator.TokenType.Percentage;
              num = ExpressionEvaluator.ExtractToken(expression, tokenBegin, token1);
              break;
            case '\'':
              token1.Type = ExpressionEvaluator.TokenType.SingleLiteral;
              num = ExpressionEvaluator.ExtractLiteral(expression, tokenBegin, token1);
              break;
            case '.':
              token1.Type = ExpressionEvaluator.TokenType.Dot;
              num = ExpressionEvaluator.ExtractToken(expression, tokenBegin, token1);
              break;
            default:
              goto label_17;
          }
          if (token1.Type != ExpressionEvaluator.TokenType.SingleLiteral && token1.Type != ExpressionEvaluator.TokenType.DoubleLiteral && token1.Type != ExpressionEvaluator.TokenType.Body && !string.Equals(token1.Property, "$body", StringComparison.OrdinalIgnoreCase))
          {
            if (ExpressionEvaluator.PropertyNameRegEx.IsMatch(token1.Property))
            {
              if (token1.Property.Length > 120)
                goto label_22;
            }
            else
              goto label_20;
          }
          tokenList.Add(token1);
          if (tokenBegin.Length != num + 1)
          {
            str3 = tokenBegin.Substring(num + 1).TrimStart();
            if (str3[0] == '+')
              str1 = str3.Substring(1);
            else
              goto label_25;
          }
          else
            goto label_27;
        }
        else
          break;
      }
      throw new InvalidDataContractException(SRClient.ExpressionInvalidToken((object) expression, (object) tokenBegin));
label_17:
      throw new InvalidDataContractException(SRClient.ExpressionInvalidTokenType((object) expression, (object) tokenBegin));
label_20:
      throw new InvalidDataContractException(SRClient.PropertyNameIsBad((object) token1.Property));
label_22:
      throw new InvalidDataContractException(SRClient.PropertyTooLong((object) token1.Property.Length, (object) 120));
label_25:
      throw new InvalidDataContractException(SRClient.ExpressionInvalidCompositionOperator((object) expression, (object) str3));
label_27:
      if (tokenList.Count > 1 && tokenList.Find((Predicate<ExpressionEvaluator.Token>) (token => token.Type == ExpressionEvaluator.TokenType.Hash)) != null)
        throw new InvalidDataContractException(SRClient.ExpressionHashInComposite);
      return tokenList;
    }

    private static string Evaluate(List<string> values)
    {
      if (values.Count == 1)
        return values[0];
      int expectedStringSize = 0;
      values.ForEach((Action<string>) (s => expectedStringSize += s.Length));
      StringBuilder finalString = new StringBuilder(expectedStringSize);
      values.ForEach((Action<string>) (s => finalString.Append(s)));
      return finalString.ToString();
    }

    private static ExpressionEvaluator.ExpressionType PeekExpressionType(string expression)
    {
      if (string.IsNullOrWhiteSpace(expression))
        return ExpressionEvaluator.ExpressionType.Literal;
      switch (expression[0])
      {
        case '#':
          return ExpressionEvaluator.ExpressionType.Numeric;
        case '$':
        case '%':
        case '.':
          return ExpressionEvaluator.ExpressionType.String;
        case '{':
          return ExpressionEvaluator.ExpressionType.Composite;
        default:
          return ExpressionEvaluator.ExpressionType.Literal;
      }
    }

    private static int ExtractLiteral(
      string fullExpression,
      string tokenBegin,
      ExpressionEvaluator.Token token)
    {
      int startIndex = 1;
      char ch = token.Type == ExpressionEvaluator.TokenType.SingleLiteral ? '\'' : '"';
      bool flag = false;
      int literal;
      while (true)
      {
        literal = tokenBegin.IndexOf(ch, startIndex);
        if (literal != -1)
        {
          if (literal + 1 < tokenBegin.Length && (int) tokenBegin[literal + 1] == (int) ch)
          {
            flag = true;
            startIndex = literal + 2;
          }
          else
            goto label_5;
        }
        else
          break;
      }
      throw new InvalidDataContractException(SRClient.ExpressionLiteralMissingClosingNotation((object) fullExpression, (object) tokenBegin));
label_5:
      string str = tokenBegin.Substring(1, literal - 1);
      if (flag)
      {
        ExpressionEvaluator.Token token1 = token;
        token1.Property = token1.Type == ExpressionEvaluator.TokenType.DoubleLiteral ? str.Replace("\"\"", "\"") : str.Replace("''", "'");
      }
      else
        token.Property = str;
      return literal;
    }

    private static int ExtractToken(
      string fullExpression,
      string tokenBegin,
      ExpressionEvaluator.Token token)
    {
      int token1 = tokenBegin[1] == '(' ? tokenBegin.IndexOf(')') : throw new InvalidDataContractException(SRClient.ExpressionMissingOpenParentheses((object) fullExpression, (object) tokenBegin));
      int num1 = tokenBegin.IndexOf(',');
      int num2 = tokenBegin.IndexOf(":{", StringComparison.InvariantCultureIgnoreCase);
      int num3 = num2 + 2;
      int num4 = 0;
      if (num2 > 1 && num2 < token1)
      {
        if (num2 < 3)
          throw new InvalidDataContractException(SRClient.ExpressionMissingProperty((object) fullExpression, (object) tokenBegin));
        bool flag = false;
        for (; num3 < tokenBegin.Length; ++num3)
        {
          if (tokenBegin[num3] != '}' || flag)
          {
            flag = tokenBegin[num3] == '\\' && !flag;
          }
          else
          {
            num4 = num3 - num2 + 1;
            token.DefaultValue = tokenBegin.Substring(num2 + 2, num4 - 3);
            num1 = tokenBegin.IndexOf(',', num3);
            token1 = tokenBegin.IndexOf(')', num3);
            goto label_11;
          }
        }
        throw new InvalidDataContractException(SRClient.ExpressionMissingDefaultEnd((object) fullExpression, (object) tokenBegin));
      }
label_11:
      if (token1 == -1)
        throw new InvalidDataContractException(SRClient.ExpressionMissingClosingParentheses((object) fullExpression, (object) tokenBegin));
      if ((token.Type == ExpressionEvaluator.TokenType.Percentage || token.Type == ExpressionEvaluator.TokenType.Hash) && num1 != -1 && num1 < token1)
        throw new InvalidDataContractException(SRClient.ExpressionErrorParsePercentFormat((object) fullExpression, (object) tokenBegin));
      if (token.Type == ExpressionEvaluator.TokenType.Dot && num1 == -1)
        throw new InvalidDataContractException(SRClient.ExpressionErrorParseDotFormat((object) fullExpression, (object) tokenBegin));
      int result = 0;
      if (num1 != -1 && num1 < token1)
      {
        string s = tokenBegin.Substring(num1 + 1, token1 - num1 - 1);
        if (!int.TryParse(s, out result) || result < 0)
          throw new InvalidDataContractException(SRClient.ExpressionIsNotPositiveInteger((object) fullExpression, (object) tokenBegin, (object) s));
        if (result == 0)
          token.EmptyString = true;
        token.Length = result;
        token.Property = tokenBegin.Substring(2, num1 - 2 - num4);
      }
      else
        token.Property = tokenBegin.Substring(2, token1 - 2 - num4);
      ExpressionEvaluator.Token token2 = token;
      token2.Property = token2.Property.Trim();
      return token1;
    }

    private enum TokenType
    {
      None,
      Dollar,
      Hash,
      Dot,
      Percentage,
      SingleLiteral,
      DoubleLiteral,
      Body,
    }

    public enum ExpressionType
    {
      Literal,
      Numeric,
      String,
      Composite,
      None,
    }

    private class Token
    {
      public bool EmptyString { get; set; }

      public int Length { get; set; }

      public string Property { get; set; }

      public string DefaultValue { get; set; }

      public ExpressionEvaluator.TokenType Type { get; set; }
    }
  }
}
