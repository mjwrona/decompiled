// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent35
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent35 : WorkItemTrackingMetadataComponent34
  {
    protected virtual WorkItemTrackingMetadataComponent35.ProjectIdChangedRecordBinder GetProjectIdChangedRecordBinder() => new WorkItemTrackingMetadataComponent35.ProjectIdChangedRecordBinder();

    public override IReadOnlyCollection<ProcessChangedRecord> GetProcessesForChangedWorkItemTypes(
      DateTime sinceWatermark)
    {
      this.PrepareStoredProcedure("prc_GetProcessesForWorkItemTypeletsChangedSinceWatermark");
      this.BindDateTime("@watermark", sinceWatermark);
      IEnumerable<ProcessChangedRecord> source = (IEnumerable<ProcessChangedRecord>) null;
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProcessChangedRecord>((ObjectBinder<ProcessChangedRecord>) new WorkItemTrackingMetadataComponent35.ProcessChangedRecordBinder());
        source = (IEnumerable<ProcessChangedRecord>) resultCollection.GetCurrent<ProcessChangedRecord>().Items;
      }
      return (IReadOnlyCollection<ProcessChangedRecord>) source.ToList<ProcessChangedRecord>();
    }

    public override IReadOnlyCollection<ProjectIdChangedRecord> GetProjectsForChangedWorkItemTypes(
      long sinceWatermark)
    {
      this.PrepareStoredProcedure("prc_GetProjectIdsForWorkItemTypesChangedSinceWatermark");
      this.BindLong("@sinceWorkItemTypeCacheStamp", sinceWatermark);
      IEnumerable<ProjectIdChangedRecord> source = (IEnumerable<ProjectIdChangedRecord>) null;
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectIdChangedRecord>((ObjectBinder<ProjectIdChangedRecord>) this.GetProjectIdChangedRecordBinder());
        source = (IEnumerable<ProjectIdChangedRecord>) resultCollection.GetCurrent<ProjectIdChangedRecord>().Items;
      }
      return (IReadOnlyCollection<ProjectIdChangedRecord>) source.ToList<ProjectIdChangedRecord>();
    }

    public override IReadOnlyCollection<ProjectIdChangedRecord> GetProjectsForChangedWorkItemTypeCategories(
      long sinceWatermark)
    {
      this.PrepareStoredProcedure("prc_GetProjectIdsForWorkItemTypeCategoriesChangedSinceWatermark");
      this.BindLong("@sinceCacheStamp", sinceWatermark);
      IEnumerable<ProjectIdChangedRecord> source = (IEnumerable<ProjectIdChangedRecord>) null;
      using (ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ProjectIdChangedRecord>((ObjectBinder<ProjectIdChangedRecord>) this.GetProjectIdChangedRecordBinder());
        source = (IEnumerable<ProjectIdChangedRecord>) resultCollection.GetCurrent<ProjectIdChangedRecord>().Items;
      }
      return (IReadOnlyCollection<ProjectIdChangedRecord>) source.ToList<ProjectIdChangedRecord>();
    }

    public override IEnumerable<GroupMemberEntry> FindGroupMembersOfType(
      IEnumerable<string> groupNames,
      GroupType groupType,
      int maxMemberForEachGroup = 100,
      int maxIteration = 50)
    {
      this.PrepareStoredProcedure("prc_FindAADGroupMembers");
      this.BindStringTable("@groups", groupNames);
      this.BindInt("@maxMembers", maxMemberForEachGroup);
      this.BindInt("@maxIteration", maxIteration);
      return this.ExecuteUnknown<IEnumerable<GroupMemberEntry>>((System.Func<IDataReader, IEnumerable<GroupMemberEntry>>) (reader => (IEnumerable<GroupMemberEntry>) this.GetGroupMemberBinder().BindAll(reader).ToList<GroupMemberEntry>()));
    }

    public override void DestroyMetadata()
    {
      this.PrepareStoredProcedure(nameof (DestroyMetadata));
      this.ExecuteNonQueryEx();
    }

    protected class ProcessChangedRecordBinder : WorkItemTrackingObjectBinder<ProcessChangedRecord>
    {
      private SqlColumnBinder ProcessIdColumn = new SqlColumnBinder("ProcessId");
      private SqlColumnBinder LatestChangedDateColumn = new SqlColumnBinder("LatestChangedDate");

      public override ProcessChangedRecord Bind(IDataReader reader) => new ProcessChangedRecord()
      {
        ProcessId = this.ProcessIdColumn.GetGuid(this.Reader),
        LatestChangedDate = this.LatestChangedDateColumn.GetDateTime(this.Reader)
      };
    }

    protected class ProjectIdChangedRecordBinder : 
      WorkItemTrackingObjectBinder<ProjectIdChangedRecord>
    {
      private SqlColumnBinder ProjectIdColumn = new SqlColumnBinder("ProjectID");
      private SqlColumnBinder MaxCacheStampColumn = new SqlColumnBinder("MaxCacheStamp");

      public override ProjectIdChangedRecord Bind(IDataReader reader) => new ProjectIdChangedRecord()
      {
        ProjectId = this.ProjectIdColumn.GetInt32(this.Reader),
        MaxCacheStamp = this.MaxCacheStampColumn.GetInt64(this.Reader)
      };
    }
  }
}
