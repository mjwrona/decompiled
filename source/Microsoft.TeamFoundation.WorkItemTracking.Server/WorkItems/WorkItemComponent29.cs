// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent29
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent29 : WorkItemComponent28
  {
    protected virtual WorkItemComponent.DeletedProjectWithRemoteLinkBinder GetDeletedProjectWithRemoteLinkBinder() => new WorkItemComponent.DeletedProjectWithRemoteLinkBinder();

    public override IEnumerable<DeletedProjectWithRemoteLink> GetDeletedProjectsWithRemoteLink()
    {
      this.PrepareStoredProcedure("prc_GetDeletedProjectsWithRemoteLink");
      return (IEnumerable<DeletedProjectWithRemoteLink>) this.ExecuteUnknown<List<DeletedProjectWithRemoteLink>>((System.Func<IDataReader, List<DeletedProjectWithRemoteLink>>) (reader => this.GetDeletedProjectWithRemoteLinkBinder().BindAll(reader).ToList<DeletedProjectWithRemoteLink>()));
    }

    public override void DeleteRemoteLinksWhoseRemoteProjectDeleted(
      IEnumerable<(Guid RemoteHostId, Guid RemoteProjectId)> remoteProjects,
      Guid changerTeamFoundationId)
    {
      this.PrepareStoredProcedure("prc_DeleteRemoteLinksWhoseRemoteProjectDeleted", 3600);
      this.BindGuid("@teamFoundationId", changerTeamFoundationId);
      this.BindGuidGuidTable("@remoteProjects", remoteProjects);
      this.ExecuteNonQuery();
    }

    protected override WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset> GetWorkItemDataSetBinder(
      bool bindTitle,
      bool bindCountFields,
      IdentityDisplayType identityDisplayType)
    {
      return (WorkItemComponent.WorkItemDatasetBinder<WorkItemDataset>) new WorkItemComponent.WorkItemDatasetBinder7<WorkItemDataset>(bindTitle, bindCountFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), this.RequestContext.WitContext().FieldDictionary);
    }

    protected override WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues> GetWorkItemFieldValuesBinder(
      IEnumerable<int> wideTableFields,
      IdentityDisplayType identityDisplayType,
      bool disableProjectionLevelThree)
    {
      return (WorkItemComponent.WorkItemFieldValuesBinder<WorkItemFieldValues>) new WorkItemComponent.WorkItemFieldValuesBinder7<WorkItemFieldValues>(wideTableFields, identityDisplayType, new System.Func<int, Guid>(((TeamFoundationSqlResourceComponent) this).GetDataspaceIdentifier), disableProjectionLevelThree, this.RequestContext.WitContext().FieldDictionary);
    }
  }
}
