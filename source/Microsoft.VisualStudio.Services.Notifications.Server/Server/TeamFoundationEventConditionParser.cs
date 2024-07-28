// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.TeamFoundationEventConditionParser
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public abstract class TeamFoundationEventConditionParser
  {
    protected string m_input;
    protected StringFieldMode m_stringFieldMode;
    protected Subscription m_subscription;
    protected IVssRequestContext m_requestContext;
    private Token m_currentToken;
    protected TeamFoundationEventConditionScanner m_scanner;
    protected IReadOnlyList<IDynamicEventPredicate> m_dynamicEventPredicates;

    public TeamFoundationEventConditionParser(
      string input,
      IReadOnlyList<IDynamicEventPredicate> dynamicEventPredicates = null,
      StringFieldMode stringFieldMode = StringFieldMode.Legacy,
      IVssRequestContext requestContext = null,
      Subscription subscription = null)
    {
      this.m_dynamicEventPredicates = dynamicEventPredicates;
      this.m_input = input;
      this.m_stringFieldMode = stringFieldMode;
      this.m_subscription = subscription;
      this.m_requestContext = requestContext;
    }

    public static TeamFoundationEventConditionParser GetParser(
      EventSerializerType eventSerializerType,
      string input,
      IReadOnlyList<IDynamicEventPredicate> dynamicEventPredicates = null,
      StringFieldMode stringFieldMode = StringFieldMode.Legacy,
      IVssRequestContext requestContext = null,
      Subscription subscription = null)
    {
      return eventSerializerType == EventSerializerType.Json ? (TeamFoundationEventConditionParser) new TeamFoundationJsonEventConditionParser(input, dynamicEventPredicates, stringFieldMode, requestContext, subscription) : (TeamFoundationEventConditionParser) new TeamFoundationXmlEventConditionParser(input, dynamicEventPredicates, stringFieldMode, requestContext, subscription);
    }

    public abstract EventSerializerType SerializerType { get; }

    public Condition Parse()
    {
      if (string.IsNullOrEmpty(this.m_input))
        return (Condition) new EmptyCondition();
      this.TakeIt();
      Condition condition = this.PrimaryCond();
      if (this.m_currentToken == null)
        return condition;
      throw new ParseException(CoreRes.EventConditionSyntaxError((object) this.m_currentToken.Spelling));
    }

    private void TakeIt() => this.m_currentToken = this.m_scanner.NextToken();

    private void Take(byte type)
    {
      if ((int) this.m_currentToken.TokenType != (int) type)
        throw new ParseException(CoreRes.EventConditionExpected((object) Token.spellings[(int) type]));
      this.TakeIt();
    }

    private Condition PrimaryCond()
    {
      Condition condition1 = this.SecondaryCond();
      while (this.m_currentToken != null && this.m_currentToken.TokenType == (byte) 5)
      {
        this.TakeIt();
        Condition condition2 = this.SecondaryCond();
        condition1 = (Condition) new OrCondition(condition1, condition2);
      }
      return condition1;
    }

    private Condition SecondaryCond()
    {
      Condition condition1 = this.TertiaryCond();
      while (this.m_currentToken != null && this.m_currentToken.TokenType == (byte) 6)
      {
        this.TakeIt();
        Condition condition2 = this.TertiaryCond();
        condition1 = (Condition) new AndCondition(condition1, condition2);
      }
      return condition1;
    }

    private Condition TertiaryCond()
    {
      bool flag = false;
      if (this.m_currentToken != null && this.m_currentToken.TokenType == (byte) 7)
      {
        this.TakeIt();
        flag = true;
      }
      Condition condition = this.Expr();
      if (flag)
        condition = (Condition) new NotCondition(condition);
      return condition;
    }

    private Condition Expr()
    {
      if (this.m_currentToken == null || this.m_currentToken.TokenType != (byte) 20)
        return this.PrimaryExpr();
      this.TakeIt();
      Condition condition = this.PrimaryCond();
      this.Take((byte) 21);
      AndCondition andCondition = condition as AndCondition;
      OrCondition orCondition = condition as OrCondition;
      if (andCondition != null)
      {
        andCondition.hasParens = true;
        return condition;
      }
      if (orCondition == null)
        return condition;
      orCondition.m_hasParens = true;
      return condition;
    }

    private Condition PrimaryExpr()
    {
      Token currentToken1 = this.m_currentToken;
      this.ValidateTokenName(currentToken1);
      this.TakeIt();
      Token currentToken2 = this.m_currentToken;
      if (currentToken2 == null || !currentToken2.IsBoolOperator())
        throw new ParseException(CoreRes.EventConditionExpectedBoolean((object) currentToken2));
      this.TakeIt();
      Token currentToken3 = this.m_currentToken;
      this.TakeIt();
      if (currentToken3 == null)
        throw new ParseException(CoreRes.EventConditionSyntaxError((object) currentToken1.Spelling));
      if (currentToken3.TokenType != (byte) 0 && currentToken3.TokenType != (byte) 1 && currentToken3.TokenType != (byte) 4 && currentToken3.TokenType != (byte) 22 && currentToken3.TokenType != (byte) 3 && currentToken3.TokenType != (byte) 23 && currentToken3.TokenType != (byte) 24)
        throw new ParseException(CoreRes.EventConditionSyntaxError((object) currentToken3));
      if (currentToken2.TokenType == (byte) 17)
        return (Condition) new DynamicPredicateCondition(this.m_dynamicEventPredicates, currentToken1, currentToken3);
      if (currentToken3.TokenType == (byte) 4 || currentToken3.TokenType == (byte) 22)
      {
        Date date = this.ParseDate(currentToken3);
        return (Condition) new DateFieldCondition(currentToken1, currentToken2.TokenType, date);
      }
      if (currentToken3.TokenType == (byte) 0)
      {
        int result = -1;
        if (int.TryParse(currentToken3.Spelling, out result))
          return (Condition) new NumericCondition(currentToken1, currentToken2.TokenType, result);
      }
      if (currentToken3.TokenType == (byte) 23 || currentToken3.TokenType == (byte) 24)
      {
        bool result = false;
        if (bool.TryParse(currentToken3.Spelling, out result))
          return (Condition) new ConstantCondition(currentToken1, currentToken2.TokenType, result);
      }
      return StringFieldFactory.GetCondition(this.m_requestContext, this.m_subscription, this.m_stringFieldMode, this.SerializerType, currentToken1, currentToken2.TokenType, currentToken3);
    }

    protected abstract void ValidateTokenName(Token fieldName);

    private string EscapeQuotes(string value)
    {
      if (!value.Contains("\""))
        return "\"" + value + "\"";
      if (!value.Contains("'"))
        return "'" + value + "'";
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append("concat(");
      string[] strArray = value.Split('"');
      for (int index = 0; index < strArray.Length; ++index)
      {
        bool flag = index > 0;
        if (strArray[index] != "")
        {
          if (flag)
            stringBuilder.Append(", ");
          stringBuilder.Append("\"");
          stringBuilder.Append(strArray[index]);
          stringBuilder.Append("\"");
          flag = true;
        }
        if (index < strArray.Length - 1)
        {
          if (flag)
            stringBuilder.Append(", ");
          stringBuilder.Append("'\"'");
        }
      }
      stringBuilder.Append(")");
      return stringBuilder.ToString();
    }

    private Date ParseDate(Token val)
    {
      switch (val.TokenType)
      {
        case 4:
          if (this.m_currentToken == null)
            return (Date) new RelativeDate(1, 0);
          switch (this.m_currentToken.TokenType)
          {
            case 18:
              this.TakeIt();
              if (this.m_currentToken.TokenType != (byte) 0)
                throw new ArgumentException(CoreRes.EventConditionExpectedInt());
              int num1 = int.Parse(this.m_currentToken.Spelling, (IFormatProvider) CultureInfo.CurrentCulture);
              this.TakeIt();
              return (Date) new RelativeDate(1, num1);
            case 19:
              this.TakeIt();
              int num2 = int.Parse(this.m_currentToken.Spelling, (IFormatProvider) CultureInfo.CurrentCulture);
              this.TakeIt();
              return (Date) new RelativeDate(-1, num2);
            default:
              return (Date) new RelativeDate(1, 0);
          }
        case 22:
          this.Take((byte) 20);
          string spelling = this.m_currentToken.Spelling;
          this.TakeIt();
          CultureInfo currentCulture = CultureInfo.CurrentCulture;
          LiteralDate date = new LiteralDate(DateTime.Parse(spelling, (IFormatProvider) currentCulture));
          this.Take((byte) 21);
          return (Date) date;
        default:
          Trace.Assert(false, "Should not be here");
          return (Date) new RelativeDate(1, 0);
      }
    }
  }
}
