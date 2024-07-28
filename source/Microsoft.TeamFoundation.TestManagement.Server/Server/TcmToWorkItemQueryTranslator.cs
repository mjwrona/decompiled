// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TcmToWorkItemQueryTranslator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Globalization;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal abstract class TcmToWorkItemQueryTranslator : TcmQueryTranslator
  {
    public TcmToWorkItemQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query)
      : base(context, query, tables: TestPlanningWiqlConstants.TestPlanningTablesForWiql)
    {
    }

    protected override void TranslateQueryInternal()
    {
      base.TranslateQueryInternal();
      this.TranslateTable();
      this.AppendCategoryClause();
    }

    protected override bool TranslateFieldName(NodeFieldName nodeFieldName)
    {
      if (!base.TranslateFieldName(nodeFieldName))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.FieldNotSupportedInQuery, (object) nodeFieldName.Value));
      return true;
    }

    protected override void TranslateTable() => this.Root.From.Value = "WorkItem";

    protected virtual void AppendCategoryClause()
    {
      NodeSelect syntax = Parser.ParseSyntax(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT * FROM WorkItem WHERE [System.WorkItemType] IN GROUP '{0}'", (object) this.GetCategoryRefName()));
      if (this.Root.Where == null)
      {
        this.Root.Where = syntax.Where;
      }
      else
      {
        NodeAndOperator nodeAndOperator = new NodeAndOperator();
        nodeAndOperator.Add(this.Root.Where);
        nodeAndOperator.Add(syntax.Where);
        this.Root.Where = (Node) nodeAndOperator;
      }
    }
  }
}
