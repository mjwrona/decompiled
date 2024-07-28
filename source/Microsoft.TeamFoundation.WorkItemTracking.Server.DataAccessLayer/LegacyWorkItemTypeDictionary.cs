// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.LegacyWorkItemTypeDictionary
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class LegacyWorkItemTypeDictionary : WorkItemTrackingDictionaryService
  {
    protected override IEnumerable<MetadataTable> MetadataTables => (IEnumerable<MetadataTable>) new MetadataTable[2]
    {
      MetadataTable.WorkItemTypes,
      MetadataTable.WorkItemTypeUsages
    };

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.RegisterSqlNotifications(systemRequestContext, BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.WorkItemTypeChanged), BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.TreeChanged));
    }

    protected override CacheSnapshotBase CreateSnapshot(
      IVssRequestContext requestContext,
      CacheSnapshotBase existingSnapshot)
    {
      MetadataDBStamps stamps = requestContext.MetadataDbStamps(this.MetadataTables);
      return (CacheSnapshotBase) new LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl(requestContext, stamps);
    }

    public IReadOnlyWorkItemTypeCollection GetWorkItemTypesForProject(
      IVssRequestContext requestContext,
      int projectId)
    {
      IReadOnlyWorkItemTypeCollection workItemTypes;
      if (!this.TryGetWorkItemTypesForProject(requestContext, projectId, out workItemTypes))
        throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorProjectIdNotExist", (object) projectId));
      return workItemTypes;
    }

    public IReadOnlyWorkItemTypeCollection GetWorkItemTypesForProject(
      IVssRequestContext requestContext,
      string projectName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectName, nameof (projectName));
      int projectId = this.GetProjectId(requestContext, projectName);
      IReadOnlyWorkItemTypeCollection workItemTypes;
      if (!this.TryGetWorkItemTypesForProject(requestContext, projectId, out workItemTypes))
        throw new LegacyValidationException(InternalsResourceStrings.Format("ErrorProjectNotExist", (object) projectName));
      return workItemTypes;
    }

    public bool TryGetWorkItemTypesForProject(
      IVssRequestContext requestContext,
      int projectId,
      out IReadOnlyWorkItemTypeCollection workItemTypes)
    {
      return this.GetSnapshot<LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl>(requestContext, false).TryGetWorkItemTypesForProject(projectId, out workItemTypes) || this.GetSnapshot<LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl>(requestContext).TryGetWorkItemTypesForProject(projectId, out workItemTypes);
    }

    private int GetProjectId(IVssRequestContext requestContext, string projectName)
    {
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext.Elevate(), projectName);
      return requestContext.WitContext().TreeService.GetTreeNode(projectId, projectId).Id;
    }

    internal bool HasWorkItemType(IVssRequestContext requestContext) => this.GetSnapshot<LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl>(requestContext, false).HasWorkItemType;

    private class WorkItemTypeDictionaryImpl : CacheSnapshotBase
    {
      private Dictionary<int, LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection> m_workItemTypesByProjId;

      public WorkItemTypeDictionaryImpl(IVssRequestContext requestContext, MetadataDBStamps stamps)
        : base(stamps)
      {
        requestContext.TraceEnter(0, "Services", "WorkItemTypeDictionary", "RefreshCache");
        this.m_workItemTypesByProjId = new Dictionary<int, LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection>();
        IList<DalWorkItemTypeField> workItemTypeFields;
        using (ScopedMetadataComponent component = ScopedMetadataComponent.CreateComponent(requestContext))
          workItemTypeFields = component.GetWorkItemTypeFields(true);
        LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.EnsureAllWorkItemTypesExistInWitCollectionDictionary(requestContext, this.m_workItemTypesByProjId);
        foreach (DalWorkItemTypeField workItemTypeField in (IEnumerable<DalWorkItemTypeField>) workItemTypeFields)
        {
          LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection itemTypeCollection;
          if (!this.m_workItemTypesByProjId.TryGetValue(workItemTypeField.ProjectId, out itemTypeCollection))
          {
            itemTypeCollection = new LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection();
            this.m_workItemTypesByProjId[workItemTypeField.ProjectId] = itemTypeCollection;
          }
          LegacyWorkItemType wit;
          if (!itemTypeCollection.TryGetByName(workItemTypeField.WorkItemTypeName, out wit))
          {
            wit = new LegacyWorkItemType(workItemTypeField.ProjectId, workItemTypeField.WorkItemTypeId, workItemTypeField.WorkItemTypeName);
            itemTypeCollection.Add(wit);
            foreach (int id in CoreField.All)
              wit.AddField(id);
          }
          wit.AddField(workItemTypeField.FieldId);
        }
        requestContext.TraceLeave(0, "Services", "WorkItemTypeDictionary", "RefreshCache");
      }

      internal bool TryGetWorkItemTypesForProject(
        int projectId,
        out IReadOnlyWorkItemTypeCollection workItemTypes)
      {
        workItemTypes = (IReadOnlyWorkItemTypeCollection) null;
        LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection itemTypeCollection;
        if (!this.m_workItemTypesByProjId.TryGetValue(projectId, out itemTypeCollection))
          return false;
        workItemTypes = (IReadOnlyWorkItemTypeCollection) itemTypeCollection;
        return true;
      }

      internal bool HasWorkItemType => this.m_workItemTypesByProjId.Values.Any<LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection>((Func<LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection, bool>) (x => ((IReadOnlyWorkItemTypeCollection) x).Count > 0));

      private static void EnsureAllWorkItemTypesExistInWitCollectionDictionary(
        IVssRequestContext requestContext,
        Dictionary<int, LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection> workItemTypes)
      {
        WorkItemTrackingTreeService tree = requestContext.GetService<WorkItemTrackingTreeService>();
        IList<int> list1 = (IList<int>) requestContext.GetService<IProjectService>().GetProjects(requestContext.Elevate()).Select<ProjectInfo, Guid>((Func<ProjectInfo, Guid>) (p => p.Id)).ToList<Guid>().Select<Guid, int?>((Func<Guid, int?>) (projectGuid => tree.GetTreeNode(requestContext, projectGuid, projectGuid, false)?.Id)).Where<int?>((Func<int?, bool>) (ni => ni.HasValue)).Select<int?, int>((Func<int?, int>) (ni => ni.Value)).ToList<int>();
        IList<IGrouping<int, WorkItemTypeEntry>> list2;
        using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
          list2 = (IList<IGrouping<int, WorkItemTypeEntry>>) component.GetWorkItemTypes((IEnumerable<int>) list1, false, true).Where<WorkItemTypeEntry>((Func<WorkItemTypeEntry, bool>) (wit => wit.ProjectId != 0)).GroupBy<WorkItemTypeEntry, int>((Func<WorkItemTypeEntry, int>) (wit => wit.ProjectId)).ToList<IGrouping<int, WorkItemTypeEntry>>();
        foreach (IGrouping<int, WorkItemTypeEntry> grouping in (IEnumerable<IGrouping<int, WorkItemTypeEntry>>) list2)
        {
          LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection itemTypeCollection = new LegacyWorkItemTypeDictionary.WorkItemTypeDictionaryImpl.WorkItemTypeCollection();
          foreach (WorkItemTypeEntry workItemTypeEntry in (IEnumerable<WorkItemTypeEntry>) grouping)
          {
            LegacyWorkItemType wit = new LegacyWorkItemType(workItemTypeEntry.ProjectId, workItemTypeEntry.Id.Value, workItemTypeEntry.Name);
            itemTypeCollection.Add(wit);
          }
          workItemTypes[grouping.Key] = itemTypeCollection;
        }
      }

      private class WorkItemTypeCollection : 
        IReadOnlyWorkItemTypeCollection,
        IEnumerable<LegacyWorkItemType>,
        IEnumerable
      {
        private Dictionary<int, LegacyWorkItemType> m_witById = new Dictionary<int, LegacyWorkItemType>();
        private Dictionary<string, LegacyWorkItemType> m_witByName = new Dictionary<string, LegacyWorkItemType>();

        internal WorkItemTypeCollection()
        {
        }

        internal void Add(LegacyWorkItemType wit)
        {
          this.m_witById.Add(wit.Id, wit);
          this.m_witByName.Add(wit.Name, wit);
        }

        public bool TryGetById(int id, out LegacyWorkItemType wit) => this.m_witById.TryGetValue(id, out wit);

        public bool TryGetByName(string name, out LegacyWorkItemType wit) => !string.IsNullOrEmpty(name) ? this.m_witByName.TryGetValue(name, out wit) : throw new ArgumentNullException(nameof (name));

        int IReadOnlyWorkItemTypeCollection.Count => this.m_witById.Count;

        IEnumerator<LegacyWorkItemType> IEnumerable<LegacyWorkItemType>.GetEnumerator() => (IEnumerator<LegacyWorkItemType>) this.m_witById.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_witById.Values.GetEnumerator();
      }
    }
  }
}
