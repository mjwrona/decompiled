// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules.ResolveVsidVisitor
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Rules
{
  public class ResolveVsidVisitor : IRuleVisitor
  {
    private IVssRequestContext m_requestContext;

    public ResolveVsidVisitor(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    protected ResolveVsidVisitor()
    {
    }

    public void Visit(WorkItemRule rule)
    {
      if (!(rule is IdentityCopyRule identityCopyRule))
        return;
      ConstantRecord constantRecord = this.GetConstantRecord(identityCopyRule.Vsid);
      if (constantRecord == null)
        return;
      identityCopyRule.Value = constantRecord.DisplayText;
      identityCopyRule.ConstId = constantRecord.Id;
    }

    protected virtual ConstantRecord GetConstantRecord(Guid vsid)
    {
      ConstantRecord constantRecord = this.m_requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecord(this.m_requestContext, vsid);
      if (constantRecord != null)
        return constantRecord;
      this.LogVsidNotFoundInConstantsTable();
      return constantRecord;
    }

    private void LogVsidNotFoundInConstantsTable()
    {
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("eventName", "Vsid not found in constants table");
      this.m_requestContext.GetService<CustomerIntelligenceService>().Publish(this.m_requestContext, "WorkItemService", "FormLayout", properties);
    }
  }
}
