// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IterationPathQueryTranslator
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class IterationPathQueryTranslator : ProjectQueryTranslator
  {
    private ResultsStoreQuery m_query;

    public IterationPathQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      Func<string, string, string> conditionNodeValueTranslator = null,
      List<Tuple<Type, string, string>> tables = null)
      : base(context, query, conditionNodeValueTranslator, tables)
    {
      this.m_query = query;
      this.InitializeFieldMap();
    }

    private void InitializeFieldMap() => this.m_fieldMapping.Add("Iteration", "Iteration");

    protected override void TranslateValue(NodeCondition cond)
    {
      base.TranslateValue(cond);
      if (!string.Equals(cond.Left.Value, "Iteration", StringComparison.OrdinalIgnoreCase))
        return;
      this.HandleAreaIterationPathCondition(cond);
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
