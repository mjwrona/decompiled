// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.JsonPathSubscriptionAdapter
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Notifications.WebApi;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class JsonPathSubscriptionAdapter : PathSubscriptionAdapter
  {
    private string c_jsonPathFormat = "$.{0} {1} '{2}'";

    public override string GetMatcher(IVssRequestContext requestContext, string eventType) => "JsonPathMatcher";

    public override ExpressionFilterModel ParseCondition(
      IVssRequestContext requestcontext,
      string conditionString)
    {
      return this.ParseCondition(requestcontext, conditionString, "JsonPathMatcher");
    }

    protected override string GetFilterString(
      string rawFieldName,
      byte rawOperator,
      string rawValue,
      IVssRequestContext requestContext = null)
    {
      return string.Format(this.c_jsonPathFormat, (object) rawFieldName, (object) Token.spellings[(int) rawOperator], (object) rawValue);
    }

    protected override PathSubscriptionExpression GetExpressionParser()
    {
      if (this.m_pathSubscriptionExpression == null)
        this.m_pathSubscriptionExpression = (PathSubscriptionExpression) new JsonPathSubscriptionExpression();
      return this.m_pathSubscriptionExpression;
    }

    protected override bool IsChangeFieldNewValue(PathSubscriptionExpression path) => false;

    protected override bool IsChangeFieldOldValue(PathSubscriptionExpression path) => false;

    public override string SubscriptionTypeName => "*";
  }
}
