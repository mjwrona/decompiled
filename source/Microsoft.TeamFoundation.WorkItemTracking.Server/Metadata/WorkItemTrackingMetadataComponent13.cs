// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent13
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent13 : WorkItemTrackingMetadataComponent12
  {
    private void BindUserIdentifier()
    {
      if (!this.RequestContext.IsUserContext)
        return;
      string instanceId = this.RequestContext.ServiceHost.DeploymentServiceHost.InstanceId.ToString();
      string collectionId = this.RequestContext.ServiceHost.InstanceId.ToString();
      this.BindGuid("@userId", IdentityConstantsNormalizer.NormalizeIdentity(this.RequestContext.WitContext().RequestIdentity, instanceId, collectionId).Id);
    }

    private void BindChangerId() => this.BindInt("@changerID", this.RequestContext.GetUserIdentity().UniqueUserId);

    public override IEnumerable<GroupMembershipRecord> GetRuleDependentGroups()
    {
      this.PrepareStoredProcedure(nameof (GetRuleDependentGroups));
      this.BindUserIdentifier();
      return this.ExecuteUnknown<IEnumerable<GroupMembershipRecord>>((System.Func<IDataReader, IEnumerable<GroupMembershipRecord>>) (reader => (IEnumerable<GroupMembershipRecord>) new WorkItemTrackingMetadataComponent8.GroupMembershipRecordBinder().BindAll(reader).ToList<GroupMembershipRecord>()));
    }

    protected virtual void BindIdentityCategory(Microsoft.VisualStudio.Services.Identity.Identity identity)
    {
      int bisIdentityType = (int) IdentityConstantsNormalizer.GetBisIdentityType((IVssIdentity) identity);
      int parameterValue = (int) Enum.Parse(typeof (GroupSpecialType), identity.GetProperty<string>("SpecialType", string.Empty));
      this.BindInt("@objectCategory", bisIdentityType);
      this.BindInt("@objectSpecialType", parameterValue);
    }

    protected virtual void BindCollectionAndAccountHostIds()
    {
      if (this.RequestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
        return;
      this.BindString("@collectionHostId", this.RequestContext.ServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
      this.BindString("@accountHostId", this.RequestContext.ServiceHost.ParentServiceHost.InstanceId.ToString(), 256, false, SqlDbType.NVarChar);
    }

    public override void SaveConstantSets(IEnumerable<RuleSetRecord> ruleSetRecords)
    {
      this.BeginTransaction(IsolationLevel.ReadCommitted);
      this.PrepareStoredProcedure(nameof (SaveConstantSets));
      this.BindChangerId();
      this.BindDateTime("@changeDate", DateTime.UtcNow);
      this.BindRuleSetRecordTable("@sets", ruleSetRecords);
      this.BindTempIdMapTable("@tempIdMap", Enumerable.Empty<TempIdRecord>());
      this.BindInt("@isIdentityWithDomainPart", 1);
      try
      {
        this.ExecuteUnknown((object) null);
        this.CommitTransaction();
      }
      catch
      {
        this.RollbackTransaction();
        throw;
      }
    }

    internal override void CreateDefaultWorkItemType(Guid projectId)
    {
      this.PrepareStoredProcedure("prc_CreateDefaultWorkItemType");
      this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));
      this.ExecuteNonQuery();
    }
  }
}
