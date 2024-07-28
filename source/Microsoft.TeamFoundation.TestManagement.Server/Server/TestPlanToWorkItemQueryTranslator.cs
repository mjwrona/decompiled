// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestPlanToWorkItemQueryTranslator
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestPlanToWorkItemQueryTranslator : TcmToWorkItemQueryTranslator
  {
    private Dictionary<string, string> m_fieldMapping = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private TestManagementRequestContext m_context;
    private GuidAndString m_projectId;
    private const string c_me = "me";

    public TestPlanToWorkItemQueryTranslator(
      TestManagementRequestContext context,
      ResultsStoreQuery query,
      GuidAndString projectId)
      : base(context, query)
    {
      this.m_projectId = projectId;
      this.m_context = context;
      this.InitializeFieldMap();
    }

    private void InitializeFieldMap()
    {
      this.m_fieldMapping.Add("PlanId", "System.Id");
      this.m_fieldMapping.Add("System.Id", "System.Id");
      this.m_fieldMapping.Add("TeamProject", "System.TeamProject");
      this.m_fieldMapping.Add("System.TeamProject", "System.TeamProject");
      this.m_fieldMapping.Add("PlanName", "System.Title");
      this.m_fieldMapping.Add("Description", "System.Description");
      this.m_fieldMapping.Add("Owner", "System.AssignedTo");
      this.m_fieldMapping.Add("PlanState", "System.State");
      this.m_fieldMapping.Add("StartDate", TCMWitFields.StartDate);
      this.m_fieldMapping.Add(TCMWitFields.StartDate, TCMWitFields.StartDate);
      this.m_fieldMapping.Add("EndDate", TCMWitFields.EndDate);
      this.m_fieldMapping.Add(TCMWitFields.EndDate, TCMWitFields.EndDate);
      this.m_fieldMapping.Add("AreaPath", "System.AreaPath");
      this.m_fieldMapping.Add("System.AreaPath", "System.AreaPath");
      this.m_fieldMapping.Add("Iteration", "System.IterationPath");
      this.m_fieldMapping.Add("System.IterationPath", "System.IterationPath");
      this.m_fieldMapping.Add("PlanLastUpdatedBy", "System.ChangedBy");
      this.m_fieldMapping.Add("PlanLastUpdated", "System.ChangedDate");
      this.m_fieldMapping.Add("System.ChangedDate", "System.ChangedDate");
      this.m_fieldMapping.Add("PlanRevision", "System.Rev");
    }

    protected override IList<string> GetAllTableFields() => (IList<string>) this.m_fieldMapping.Values.Distinct<string>().ToList<string>();

    protected override void TranslateValue(NodeCondition cond)
    {
      if (string.Equals(cond.Left.Value, "PlanState", StringComparison.OrdinalIgnoreCase))
      {
        this.HandleStateCondition(cond);
      }
      else
      {
        if (!string.Equals(cond.Left.Value, "PlanLastUpdatedBy", StringComparison.OrdinalIgnoreCase) && !string.Equals(cond.Left.Value, "Owner", StringComparison.OrdinalIgnoreCase))
          return;
        this.HandleTeamFoundationGuidCondition(cond, cond.Left.Value);
      }
    }

    protected override IDictionary<string, string> GetFieldMap() => (IDictionary<string, string>) this.m_fieldMapping;

    protected override string GetCategoryRefName() => WitCategoryRefName.TestPlan;

    protected override IList<string> GetMappedStateValues(string stateString, bool converse)
    {
      TestPlanState result;
      if (!Enum.TryParse<TestPlanState>(stateString, out result) || result != TestPlanState.Active && result != TestPlanState.Inactive)
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidTcmState, (object) stateString, (object) this.GetCategoryRefName().ToLower(CultureInfo.CurrentCulture)));
      if (converse)
        result = result == TestPlanState.Active ? TestPlanState.Inactive : TestPlanState.Active;
      return ProcessConfigurationHelper.GetWorkItemStates(this.m_context, this.m_projectId.String, (byte) result, WitCategoryRefName.TestPlan);
    }

    protected virtual string GetDisplayNameFromGuidString(string guidStr)
    {
      Guid result;
      if (!Guid.TryParse(guidStr, out result))
        throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidGuidFormat, (object) guidStr));
      string nameFromGuidString = IdentityHelper.ResolveIdentityToName(this.m_context, result);
      if (string.IsNullOrEmpty(nameFromGuidString))
        nameFromGuidString = guidStr;
      return nameFromGuidString;
    }

    private void HandleTeamFoundationGuidCondition(NodeCondition cond, string fieldName)
    {
      switch (cond.Condition)
      {
        case Condition.Equals:
        case Condition.NotEquals:
        case Condition.EqualsAlias:
        case Condition.NotEqualsAlias:
          if (this.IsMe(cond.Right))
            break;
          string nameFromGuidString = this.GetDisplayNameFromGuidString(cond.Right.ConstStringValue);
          cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeString(nameFromGuidString);
          break;
        case Condition.In:
          this.HandleTeamFoundationIdList(cond);
          break;
        default:
          throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidConditionForTcmField, (object) cond.ToString(), (object) fieldName));
      }
    }

    private void HandleTeamFoundationIdList(NodeCondition cond)
    {
      bool flag = false;
      NodeValueList right = (NodeValueList) cond.Right;
      Dictionary<string, string> dictionary = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.CurrentCultureIgnoreCase);
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node in (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) right)
      {
        if (node is NodeString nodeString)
        {
          string nameFromGuidString = this.GetDisplayNameFromGuidString(nodeString.ConstStringValue);
          if (!dictionary.ContainsKey(nameFromGuidString))
            dictionary[nameFromGuidString] = nameFromGuidString;
        }
        else
        {
          if (!this.IsMe(node))
            throw new TestManagementValidationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidGuidFormat, (object) node.ToString()));
          flag = true;
        }
      }
      NodeValueList nodeValueList = TcmQueryTranslator.GetNodeValueList((IList<string>) dictionary.Keys.ToList<string>());
      if (flag)
        nodeValueList.Add((Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) new NodeVariable("me"));
      cond.Right = (Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node) nodeValueList;
    }

    private bool IsMe(Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node node) => node is NodeVariable nodeVariable && string.Equals(nodeVariable.Value, "me", StringComparison.OrdinalIgnoreCase);
  }
}
