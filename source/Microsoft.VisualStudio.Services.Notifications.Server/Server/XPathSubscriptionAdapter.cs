// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.XPathSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class XPathSubscriptionAdapter : PathSubscriptionAdapter
  {
    public virtual bool UseSingleQuoteInXPath => false;

    public override string GetMatcher(IVssRequestContext requestContext, string eventType) => "XPathMatcher";

    protected override string GetFilterString(
      string rawFieldName,
      byte rawOperator,
      string rawValue,
      IVssRequestContext requestContext = null)
    {
      switch (rawOperator)
      {
        case 15:
          if (rawValue == null)
            rawValue = string.Empty;
          if (!rawValue.Contains("?"))
            rawValue = Regex.Escape(rawValue);
          rawValue = PathSubscriptionAdapter.QuoteString(rawValue);
          break;
        case 25:
          XPathSubscriptionExpression subscriptionExpression = new XPathSubscriptionExpression();
          subscriptionExpression.Path = rawFieldName;
          subscriptionExpression.FilterType = FieldFilterType.NotContains;
          subscriptionExpression.FilterName = (Token) new XPathToken(".");
          subscriptionExpression.FilterValue = (Token) new XPathToken(rawValue);
          subscriptionExpression.FilterNameIgnoreCase = true;
          subscriptionExpression.UseSingleQuoteChar = this.UseSingleQuoteInXPath;
          rawFieldName = subscriptionExpression.ToPath();
          rawOperator = (byte) 13;
          rawValue = "null";
          break;
        default:
          if (!"null".Equals(rawValue))
          {
            rawValue = PathSubscriptionAdapter.QuoteString(rawValue);
            break;
          }
          break;
      }
      return string.Format("{0} {1} {2}", (object) PathSubscriptionAdapter.QuoteString(rawFieldName), (object) Token.spellings[(int) rawOperator], (object) rawValue);
    }

    protected override PathSubscriptionExpression GetExpressionParser()
    {
      if (this.m_pathSubscriptionExpression == null)
        this.m_pathSubscriptionExpression = (PathSubscriptionExpression) new XPathSubscriptionExpression();
      return this.m_pathSubscriptionExpression;
    }

    protected override bool IsChangeFieldNewValue(PathSubscriptionExpression xpath) => string.Equals("/NewValue", xpath.PostFilterPath, StringComparison.OrdinalIgnoreCase);

    protected override bool IsChangeFieldOldValue(PathSubscriptionExpression xpath) => string.Equals("/OldValue", xpath.PostFilterPath, StringComparison.OrdinalIgnoreCase);

    protected virtual bool AreOldAndNewChangeFields(
      XPathSubscriptionExpression xpath1,
      XPathSubscriptionExpression xpath2)
    {
      return this.AreOldAndNewChangeFields(xpath1, xpath2);
    }

    public override string SubscriptionTypeName => "*";

    protected override void UpdateValuesFromPath(
      ExpressionFilterField field,
      PathSubscriptionExpression xpath,
      ref byte rawOperator,
      ref string value,
      IVssRequestContext requestContext)
    {
      base.UpdateValuesFromPath(field, xpath, ref rawOperator, ref value, requestContext);
    }

    protected override PathSubscriptionExpression ParsePathFromFieldName(string fieldName) => (PathSubscriptionExpression) XPathSubscriptionExpression.Parse(fieldName);

    public override ExpressionFilterModel ParseCondition(
      IVssRequestContext requestcontext,
      string conditionString)
    {
      return this.ParseCondition(requestcontext, conditionString, "XPathMatcher");
    }

    protected virtual StringFieldCondition DefaultGetOptimizedCondition(
      Token fieldName,
      byte op,
      Token target)
    {
      StringFieldCondition optimizedCondition = (StringFieldCondition) null;
      if (op == (byte) 13 && string.Equals("null", target.Spelling))
      {
        Token optimizedFieldName = (Token) null;
        Token optimizedTarget = (Token) null;
        byte optimizedOp = 3;
        if (this.GetOptimizedTokensAndOp(XPathSubscriptionExpression.Parse(fieldName.Spelling), ref optimizedFieldName, ref optimizedOp, ref optimizedTarget))
          optimizedCondition = new StringFieldCondition(optimizedFieldName, optimizedOp, optimizedTarget);
      }
      return optimizedCondition;
    }

    protected virtual bool GetOptimizedTokensAndOp(
      XPathSubscriptionExpression expression,
      ref Token optimizedFieldName,
      ref byte optimizedOp,
      ref Token optimizedTarget)
    {
      return false;
    }
  }
}
