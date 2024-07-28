// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingMetadataComponent8
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  internal class WorkItemTrackingMetadataComponent8 : WorkItemTrackingMetadataComponent
  {
    internal override IDictionary<ConstantSetReference, SetRecord[]> GetConstantSets(
      IEnumerable<ConstantSetReference> setReferences)
    {
      ArgumentUtility.CheckForNull<IEnumerable<ConstantSetReference>>(setReferences, nameof (setReferences));
      WorkItemTrackingMetadataComponent8.FilterableSetReferenceCollection setCollection = new WorkItemTrackingMetadataComponent8.FilterableSetReferenceCollection();
      setCollection.AddSetReferences(setReferences);
      this.PrepareStoredProcedure(nameof (GetConstantSets));
      this.BindUserSid();
      this.BindConstantSetReferenceTable("@sets", setReferences.Distinct<ConstantSetReference>());
      return this.ExecuteUnknown<IDictionary<ConstantSetReference, SetRecord[]>>((System.Func<IDataReader, IDictionary<ConstantSetReference, SetRecord[]>>) (reader =>
      {
        WorkItemTrackingMetadataComponent.SetRecordBinder setRecordBinder = (WorkItemTrackingMetadataComponent.SetRecordBinder) new WorkItemTrackingMetadataComponent8.SetRecordBinder2();
        while (reader.Read())
          setCollection.SetResult(setRecordBinder.Bind(reader));
        return setCollection.GetResult();
      }));
    }

    public override IEnumerable<ConstantRecord> GetConstantRecords(
      IEnumerable<int> constantIds,
      bool includeInactiveConstants = false)
    {
      IEnumerable<int> rows = constantIds.Distinct<int>();
      this.PrepareStoredProcedure("prc_GetConstantRecords");
      this.BindInt32Table("@constantIds", rows);
      return (IEnumerable<ConstantRecord>) this.ExecuteUnknown<List<ConstantRecord>>((System.Func<IDataReader, List<ConstantRecord>>) (reader => new WorkItemTrackingMetadataComponent.ConstantRecordBinder().BindAll(reader).ToList<ConstantRecord>()));
    }

    protected class SetRecordBinder2 : WorkItemTrackingMetadataComponent.SetRecordBinder
    {
      private SqlColumnBinder Generation = new SqlColumnBinder(nameof (Generation));

      public override SetRecord Bind(IDataReader reader)
      {
        SetRecord setRecord = base.Bind(reader);
        setRecord.Generation = this.Generation.GetInt32(reader);
        return setRecord;
      }
    }

    protected class GroupMembershipRecordBinder : WorkItemTrackingObjectBinder<GroupMembershipRecord>
    {
      private SqlColumnBinder ParentIdColumn = new SqlColumnBinder("ParentID");
      private SqlColumnBinder ConstIdColumn = new SqlColumnBinder("ConstID");
      private SqlColumnBinder TeamFoundationIdColumn = new SqlColumnBinder("TeamFoundationId");
      private SqlColumnBinder IsMemberColumn = new SqlColumnBinder("IsMember");

      public override GroupMembershipRecord Bind(IDataReader reader) => new GroupMembershipRecord()
      {
        ParentId = this.ParentIdColumn.GetInt32(reader),
        ConstId = this.ParentIdColumn.GetInt32(reader),
        TeamFoundationId = this.TeamFoundationIdColumn.GetGuid(reader),
        IsMember = this.IsMemberColumn.GetBoolean(reader)
      };
    }

    protected class FilterableSetReference : ConstantSetReference
    {
      private List<SetRecord> m_records = new List<SetRecord>();

      public static WorkItemTrackingMetadataComponent8.FilterableSetReference Create(
        ConstantSetReference setReference)
      {
        WorkItemTrackingMetadataComponent8.FilterableSetReference filterableSetReference = new WorkItemTrackingMetadataComponent8.FilterableSetReference();
        filterableSetReference.Id = setReference.Id;
        filterableSetReference.IncludeTop = setReference.IncludeTop;
        filterableSetReference.Direct = setReference.Direct;
        filterableSetReference.ExcludeGroups = setReference.ExcludeGroups;
        return filterableSetReference;
      }

      public static WorkItemTrackingMetadataComponent8.FilterableSetReference CreateSuperSet(
        WorkItemTrackingMetadataComponent8.FilterableSetReference a,
        WorkItemTrackingMetadataComponent8.FilterableSetReference b)
      {
        WorkItemTrackingMetadataComponent8.FilterableSetReference superSet = new WorkItemTrackingMetadataComponent8.FilterableSetReference();
        superSet.Id = a.Id;
        superSet.IncludeTop = a.IncludeTop || b.IncludeTop;
        superSet.Direct = a.Direct && b.Direct;
        superSet.ExcludeGroups = a.ExcludeGroups && b.ExcludeGroups;
        return superSet;
      }

      public bool IsSuperSet(ConstantSetReference other)
      {
        if (this.Direct && !other.Direct || this.ExcludeGroups && !other.ExcludeGroups)
          return false;
        return this.IncludeTop || !other.IncludeTop;
      }

      public void SetRawResult(SetRecord record) => this.m_records.Add(record);

      public List<SetRecord> GetRawResult() => this.SuperSet == null ? this.m_records : this.SuperSet.GetRawResult();

      public SetRecord[] GetFilteredResult() => this.GetRawResult().Where<SetRecord>((System.Func<SetRecord, bool>) (x =>
      {
        if (!this.IncludeTop && x.Generation <= 1 || this.Direct && x.Generation > 2)
          return false;
        return !this.ExcludeGroups || !x.IsList;
      })).OrderBy<SetRecord, string>((System.Func<SetRecord, string>) (sr => sr.Item), (IComparer<string>) StringComparer.CurrentCultureIgnoreCase).ToArray<SetRecord>();

      public WorkItemTrackingMetadataComponent8.FilterableSetReference SuperSet { get; internal set; }
    }

    protected class FilterableSetReferenceCollection
    {
      private const int c_initialSize = 10;
      private Dictionary<int, WorkItemTrackingMetadataComponent8.FilterableSetReference> m_superSets = new Dictionary<int, WorkItemTrackingMetadataComponent8.FilterableSetReference>(10);
      private List<WorkItemTrackingMetadataComponent8.FilterableSetReference> m_sets = new List<WorkItemTrackingMetadataComponent8.FilterableSetReference>(10);

      public void AddSetReferences(IEnumerable<ConstantSetReference> setReferences)
      {
        foreach (ConstantSetReference setReference in setReferences)
        {
          WorkItemTrackingMetadataComponent8.FilterableSetReference filterableSetReference1 = WorkItemTrackingMetadataComponent8.FilterableSetReference.Create(setReference);
          WorkItemTrackingMetadataComponent8.FilterableSetReference filterableSetReference2;
          if (!this.m_superSets.TryGetValue(filterableSetReference1.Id, out filterableSetReference2))
            this.m_superSets.Add(filterableSetReference1.Id, filterableSetReference1);
          else if (filterableSetReference1.IsSuperSet((ConstantSetReference) filterableSetReference2))
          {
            if (!filterableSetReference2.IsSuperSet((ConstantSetReference) filterableSetReference1))
            {
              filterableSetReference2.SuperSet = filterableSetReference1;
              this.m_superSets[filterableSetReference1.Id] = filterableSetReference1;
            }
            else
              continue;
          }
          else if (filterableSetReference2.IsSuperSet((ConstantSetReference) filterableSetReference1))
          {
            filterableSetReference1.SuperSet = filterableSetReference2;
          }
          else
          {
            WorkItemTrackingMetadataComponent8.FilterableSetReference superSet = WorkItemTrackingMetadataComponent8.FilterableSetReference.CreateSuperSet(filterableSetReference1, filterableSetReference2);
            this.m_superSets[superSet.Id] = superSet;
            filterableSetReference2.SuperSet = superSet;
            filterableSetReference1.SuperSet = superSet;
          }
          this.m_sets.Add(filterableSetReference1);
        }
      }

      public void SetResult(SetRecord record) => this.m_superSets[record.ParentId].SetRawResult(record);

      public IDictionary<ConstantSetReference, SetRecord[]> GetResult() => (IDictionary<ConstantSetReference, SetRecord[]>) this.m_sets.ToDictionary<WorkItemTrackingMetadataComponent8.FilterableSetReference, ConstantSetReference, SetRecord[]>((System.Func<WorkItemTrackingMetadataComponent8.FilterableSetReference, ConstantSetReference>) (x => (ConstantSetReference) x), (System.Func<WorkItemTrackingMetadataComponent8.FilterableSetReference, SetRecord[]>) (x => x.GetFilteredResult()));

      public ICollection<WorkItemTrackingMetadataComponent8.FilterableSetReference> SetReferences => (ICollection<WorkItemTrackingMetadataComponent8.FilterableSetReference>) this.m_superSets.Values;
    }
  }
}
