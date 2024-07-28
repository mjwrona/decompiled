// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ProjectQueryTranslator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class ProjectQueryTranslator : TcmQueryTranslator
  {
    protected Dictionary<string, string> m_fieldMapping = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private ResultsStoreQuery m_query;

    public ProjectQueryTranslator(TestManagementRequestContext context, ResultsStoreQuery query)
      : base(context, query)
    {
      this.m_query = query;
      this.InitializeFieldMap();
    }

    public ProjectQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      Func<string, string, string> conditionNodeValueTranslator,
      List<Tuple<Type, string, string>> tables = null)
      : base(context, query, conditionNodeValueTranslator, tables)
    {
      this.m_query = query;
      this.InitializeFieldMap();
    }

    private void InitializeFieldMap() => this.m_fieldMapping.Add("TeamProject", "DataspaceId");

    protected override void TranslateValue(NodeCondition cond)
    {
      if (!string.Equals(cond.Left.Value, "TeamProject", StringComparison.OrdinalIgnoreCase))
        return;
      this.HandleProjectNameCondition(cond);
    }

    internal override void TranslateCondition(NodeCondition cond)
    {
      switch (cond.Condition)
      {
        case Condition.Equals:
        case Condition.NotEquals:
        case Condition.In:
        case Condition.EqualsAlias:
        case Condition.NotEqualsAlias:
          this.TranslateValue(cond);
          goto case Condition.Less;
        case Condition.Less:
        case Condition.Greater:
        case Condition.LessOrEquals:
        case Condition.GreaterOrEquals:
          this.TranslateFieldName(cond.Left);
          this.TranslateNode(cond.Right);
          break;
        case Condition.Under:
        case Condition.Contains:
          this.TranslateFieldName(cond.Left);
          this.TranslateValue(cond);
          break;
        default:
          Tools.EnsureSyntax(true, SyntaxError.InvalidConditionalOperator, (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) cond);
          break;
      }
    }

    protected override IList<string> GetAllTableFields() => (IList<string>) new List<string>()
    {
      "*"
    };

    protected override string GetCategoryRefName() => throw new NotImplementedException();

    protected override IDictionary<string, string> GetFieldMap() => (IDictionary<string, string>) this.m_fieldMapping;

    protected override string GetDefaultProjectName() => this.m_query.TeamProjectName;
  }
}
