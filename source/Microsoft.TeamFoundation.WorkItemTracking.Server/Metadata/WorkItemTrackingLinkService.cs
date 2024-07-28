// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemTrackingLinkService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class WorkItemTrackingLinkService : 
    WorkItemTrackingDictionaryService,
    IWorkItemTrackingLinkService,
    IVssFrameworkService
  {
    protected override IEnumerable<MetadataTable> MetadataTables => (IEnumerable<MetadataTable>) new MetadataTable[1]
    {
      MetadataTable.LinkTypes
    };

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      this.RegisterSqlNotifications(systemRequestContext, BaseTeamFoundationWorkItemTrackingService.SqlNotificationSubscription.Default(SpecialGuids.LinkTypeChanged));
    }

    protected override CacheSnapshotBase CreateSnapshot(
      IVssRequestContext requestContext,
      CacheSnapshotBase existingSnapshot)
    {
      MetadataDBStamps stamps = requestContext.MetadataDbStamps(this.MetadataTables);
      return (CacheSnapshotBase) new WorkItemTrackingLinkService.LinkTypeDictionaryImpl(requestContext, this, stamps);
    }

    protected virtual IEnumerable<MDWorkItemLinkType> GetLinkTypesInternal(
      IVssRequestContext requestContext,
      bool disableDataspaceRls = false)
    {
      IEnumerable<WorkItemLinkTypeRecord> linkTypes;
      using (WorkItemTrackingMetadataComponent component = requestContext.CreateComponent<WorkItemTrackingMetadataComponent>())
        linkTypes = (IEnumerable<WorkItemLinkTypeRecord>) component.GetLinkTypes(disableDataspaceRls);
      Dictionary<string, MDWorkItemLinkType> dictionary = new Dictionary<string, MDWorkItemLinkType>();
      foreach (WorkItemLinkTypeRecord itemLinkTypeRecord in linkTypes)
      {
        MDWorkItemLinkType workItemLinkType;
        if (!dictionary.TryGetValue(itemLinkTypeRecord.ReferenceName, out workItemLinkType))
        {
          dictionary.Add(itemLinkTypeRecord.ReferenceName, workItemLinkType = new MDWorkItemLinkType());
          workItemLinkType.ReferenceName = itemLinkTypeRecord.ReferenceName;
          workItemLinkType.Rules = itemLinkTypeRecord.Rules;
        }
        WorkItemLinkTypeEnd workItemLinkTypeEnd = new WorkItemLinkTypeEnd();
        workItemLinkTypeEnd.Name = itemLinkTypeRecord.Name;
        workItemLinkTypeEnd.Id = itemLinkTypeRecord.Id;
        workItemLinkTypeEnd.LinkType = workItemLinkType;
        if (itemLinkTypeRecord.ReversedDirection)
        {
          workItemLinkTypeEnd.IsForwardEnd = false;
          workItemLinkType.ReverseEnd = workItemLinkTypeEnd;
        }
        else
        {
          workItemLinkTypeEnd.IsForwardEnd = true;
          workItemLinkType.ForwardEnd = workItemLinkTypeEnd;
        }
      }
      return (IEnumerable<MDWorkItemLinkType>) dictionary.Values;
    }

    public IEnumerable<OobLinkType> GetAllOutOfBoxLinkTypes(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      List<Guid> guidList = new List<Guid>();
      guidList.Add(ProcessTemplateTypeIdentifiers.MsfAgileSoftwareDevelopment);
      guidList.Add(ProcessTemplateTypeIdentifiers.MsfCmmiProcessImprovement);
      guidList.Add(ProcessTemplateTypeIdentifiers.VisualStudioScrum);
      guidList.Add(ProcessTemplateTypeIdentifiers.MsfHydroProcess);
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      Dictionary<string, OobLinkType> dictionary = new Dictionary<string, OobLinkType>();
      foreach (Guid guid in guidList)
      {
        ProcessDescriptor processDescriptor = service.GetProcessDescriptor(requestContext, guid);
        if (processDescriptor.IsDerived)
          processDescriptor = service.GetProcessDescriptor(requestContext, processDescriptor.Inherits);
        if (!processDescriptor.IsSystem)
          throw new ProcessOutOfBoxFieldsNotFound(guid);
        foreach (OobLinkType oobLinkType in requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, processDescriptor).LinkTypes.Values)
        {
          if (!dictionary.ContainsKey(oobLinkType.ReferenceName))
            dictionary.Add(oobLinkType.ReferenceName, oobLinkType);
        }
      }
      return (IEnumerable<OobLinkType>) dictionary.Values;
    }

    public virtual IEnumerable<MDWorkItemLinkType> GetLinkTypes(IVssRequestContext requestContext) => this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).GetLinkTypes(CommonWITUtils.IsRemoteLinkingEnabled(requestContext));

    public virtual MDWorkItemLinkType GetLinkTypeById(IVssRequestContext requestContext, int id)
    {
      MDWorkItemLinkType linkType;
      if (!this.TryGetLinkTypeById(requestContext, id, out linkType))
        throw new WorkItemTrackingLinkTypeNotFoundException(id);
      return linkType;
    }

    public virtual MDWorkItemLinkType GetLinkTypeByName(
      IVssRequestContext requestContext,
      string name)
    {
      MDWorkItemLinkType linkType;
      if (!this.TryGetLinkTypeByName(requestContext, name, out linkType))
        throw new WorkItemTrackingLinkTypeNotFoundException(name);
      return linkType;
    }

    public virtual MDWorkItemLinkType GetLinkTypeByReferenceName(
      IVssRequestContext requestContext,
      string referenceName)
    {
      MDWorkItemLinkType linkType;
      if (!this.TryGetLinkTypeByReferenceName(requestContext, referenceName, out linkType))
        throw new WorkItemTrackingLinkTypeNotFoundException(referenceName);
      return linkType;
    }

    public virtual bool ContainsLinkTypeName(IVssRequestContext requestContext, string name) => this.TryGetLinkTypeByName(requestContext, name, out MDWorkItemLinkType _);

    public virtual bool ContainsLinkTypeReferenceName(
      IVssRequestContext requestContext,
      string referenceName)
    {
      return this.TryGetLinkTypeByReferenceName(requestContext, referenceName, out MDWorkItemLinkType _);
    }

    public virtual bool TryGetLinkTypeById(
      IVssRequestContext requestContext,
      int id,
      out MDWorkItemLinkType linkType)
    {
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeById(id, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeById(id, out linkType, isRemoteLinkingEnabled);
    }

    public virtual bool TryGetLinkTypeByName(
      IVssRequestContext requestContext,
      string name,
      out MDWorkItemLinkType linkType)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeByName(name, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeByName(name, out linkType, isRemoteLinkingEnabled);
    }

    public virtual bool TryGetLinkTypeByReferenceName(
      IVssRequestContext requestContext,
      string referenceName,
      out MDWorkItemLinkType linkType)
    {
      if (string.IsNullOrEmpty(referenceName))
        throw new ArgumentNullException(nameof (referenceName));
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeByReferenceName(referenceName, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeByReferenceName(referenceName, out linkType, isRemoteLinkingEnabled);
    }

    public virtual WorkItemLinkTypeEnd GetLinkTypeEndById(IVssRequestContext requestContext, int id)
    {
      WorkItemLinkTypeEnd linkType;
      if (!this.TryGetLinkTypeEndById(requestContext, id, out linkType))
        throw new WorkItemTrackingLinkTypeEndNotFoundException(id);
      return linkType;
    }

    public virtual WorkItemLinkTypeEnd GetLinkTypeEndByName(
      IVssRequestContext requestContext,
      string name)
    {
      WorkItemLinkTypeEnd linkType;
      if (!this.TryGetLinkTypeEndByName(requestContext, name, out linkType))
        throw new WorkItemTrackingLinkTypeEndNotFoundException(name);
      return linkType;
    }

    public virtual WorkItemLinkTypeEnd GetLinkTypeEndByReferenceName(
      IVssRequestContext requestContext,
      string referenceName)
    {
      WorkItemLinkTypeEnd linkType;
      if (!this.TryGetLinkTypeEndByReferenceName(requestContext, referenceName, out linkType))
        throw new WorkItemTrackingLinkTypeEndNotFoundException(referenceName);
      return linkType;
    }

    public virtual bool ContainsLinkTypeEndReferenceName(
      IVssRequestContext requestContext,
      string referenceName)
    {
      return this.TryGetLinkTypeEndByReferenceName(requestContext, referenceName, out WorkItemLinkTypeEnd _);
    }

    public virtual bool TryGetLinkTypeEndById(
      IVssRequestContext requestContext,
      int id,
      out WorkItemLinkTypeEnd linkType)
    {
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeEndById(id, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeEndById(id, out linkType, isRemoteLinkingEnabled);
    }

    public virtual bool TryGetLinkTypeEndByName(
      IVssRequestContext requestContext,
      string name,
      out WorkItemLinkTypeEnd linkType)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentNullException(nameof (name));
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeEndByName(name, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeEndByName(name, out linkType, isRemoteLinkingEnabled);
    }

    public virtual bool TryGetLinkTypeEndByReferenceName(
      IVssRequestContext requestContext,
      string referenceName,
      out WorkItemLinkTypeEnd linkType)
    {
      if (string.IsNullOrEmpty(referenceName))
        throw new ArgumentNullException(nameof (referenceName));
      bool isRemoteLinkingEnabled = CommonWITUtils.IsRemoteLinkingEnabled(requestContext);
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).TryGetLinkTypeEndByReferenceName(referenceName, out linkType, isRemoteLinkingEnabled) || this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext).TryGetLinkTypeEndByReferenceName(referenceName, out linkType, isRemoteLinkingEnabled);
    }

    public virtual int GetCount(IVssRequestContext requestContext) => this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).GetCount(CommonWITUtils.IsRemoteLinkingEnabled(requestContext));

    public virtual IEnumerable<string> GetLinkTypeReferenceNames(IVssRequestContext requestContext) => this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).GetLinkTypeReferenceNames(CommonWITUtils.IsRemoteLinkingEnabled(requestContext));

    public virtual IEnumerable<string> GetLinkTypeEndReferenceNames(
      IVssRequestContext requestContext)
    {
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).GetLinkTypeEndReferenceNames(CommonWITUtils.IsRemoteLinkingEnabled(requestContext));
    }

    public virtual IEnumerable<int> GetLinkTypeIds(
      IVssRequestContext requestContext,
      bool includeRemoteLinks = true)
    {
      return this.GetSnapshot<WorkItemTrackingLinkService.LinkTypeDictionaryImpl>(requestContext, false).GetLinkTypeIds(includeRemoteLinks && CommonWITUtils.IsRemoteLinkingEnabled(requestContext));
    }

    private class LinkTypeDictionaryImpl : CacheSnapshotBase
    {
      private Dictionary<int, MDWorkItemLinkType> m_linkTypeById;
      private Dictionary<string, MDWorkItemLinkType> m_linkTypeByName;
      private Dictionary<string, MDWorkItemLinkType> m_linkTypeByReferenceName;
      private Dictionary<int, WorkItemLinkTypeEnd> m_linkTypeEndById;
      private Dictionary<string, WorkItemLinkTypeEnd> m_linkTypeEndByName;
      private Dictionary<string, WorkItemLinkTypeEnd> m_linkTypeEndByReferenceName;

      public LinkTypeDictionaryImpl(
        IVssRequestContext requestContext,
        WorkItemTrackingLinkService dict,
        MetadataDBStamps stamps)
        : base(stamps)
      {
        WorkItemTrackingLinkService.LinkTypeDictionaryImpl typeDictionaryImpl = this;
        requestContext.TraceBlock(900445, 900446, "Dictionaries", "LinkTypeDictionary", "LinkTypeDictionaryImpl.Initialize", (Action) (() =>
        {
          typeDictionaryImpl.m_linkTypeById = new Dictionary<int, MDWorkItemLinkType>();
          typeDictionaryImpl.m_linkTypeByName = new Dictionary<string, MDWorkItemLinkType>();
          typeDictionaryImpl.m_linkTypeByReferenceName = new Dictionary<string, MDWorkItemLinkType>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
          typeDictionaryImpl.m_linkTypeEndById = new Dictionary<int, WorkItemLinkTypeEnd>();
          typeDictionaryImpl.m_linkTypeEndByName = new Dictionary<string, WorkItemLinkTypeEnd>();
          typeDictionaryImpl.m_linkTypeEndByReferenceName = new Dictionary<string, WorkItemLinkTypeEnd>((IEqualityComparer<string>) TFStringComparer.WorkItemFieldReferenceName);
          typeDictionaryImpl.LinkTypes = dict.GetLinkTypesInternal(requestContext, true);
          foreach (MDWorkItemLinkType linkType in typeDictionaryImpl.LinkTypes)
          {
            typeDictionaryImpl.m_linkTypeById[linkType.ForwardId] = linkType;
            typeDictionaryImpl.m_linkTypeById[linkType.ReverseId] = linkType;
            typeDictionaryImpl.m_linkTypeByName[linkType.ForwardEndName] = linkType;
            typeDictionaryImpl.m_linkTypeByName[linkType.ReverseEndName] = linkType;
            typeDictionaryImpl.m_linkTypeByReferenceName[linkType.ReferenceName] = linkType;
            WorkItemLinkTypeEnd forwardEnd = linkType.ForwardEnd;
            WorkItemLinkTypeEnd reverseEnd = linkType.ReverseEnd;
            typeDictionaryImpl.m_linkTypeEndById[forwardEnd.Id] = forwardEnd;
            typeDictionaryImpl.m_linkTypeEndById[reverseEnd.Id] = reverseEnd;
            typeDictionaryImpl.m_linkTypeEndByName[forwardEnd.Name] = forwardEnd;
            typeDictionaryImpl.m_linkTypeEndByName[reverseEnd.Name] = reverseEnd;
            typeDictionaryImpl.m_linkTypeEndByReferenceName[forwardEnd.ReferenceName] = forwardEnd;
            typeDictionaryImpl.m_linkTypeEndByReferenceName[reverseEnd.ReferenceName] = reverseEnd;
          }
        }));
      }

      private IEnumerable<MDWorkItemLinkType> LinkTypes { get; set; }

      internal IEnumerable<MDWorkItemLinkType> GetLinkTypes(bool isRemoteLinkingEnabled) => this.LinkTypes.Where<MDWorkItemLinkType>((Func<MDWorkItemLinkType, bool>) (lt => !lt.IsRemote | isRemoteLinkingEnabled));

      internal bool TryGetLinkTypeByName(
        string name,
        out MDWorkItemLinkType linkType,
        bool isRemoteLinkingEnabled)
      {
        linkType = (MDWorkItemLinkType) null;
        return this.FilterRemoteLinks(this.m_linkTypeByName.TryGetValue(name, out linkType), isRemoteLinkingEnabled, ref linkType);
      }

      internal bool TryGetLinkTypeByReferenceName(
        string referenceName,
        out MDWorkItemLinkType linkType,
        bool isRemoteLinkingEnabled)
      {
        linkType = (MDWorkItemLinkType) null;
        return this.FilterRemoteLinks(this.m_linkTypeByReferenceName.TryGetValue(referenceName, out linkType), isRemoteLinkingEnabled, ref linkType);
      }

      internal bool TryGetLinkTypeById(
        int id,
        out MDWorkItemLinkType linkType,
        bool isRemoteLinkingEnabled)
      {
        linkType = (MDWorkItemLinkType) null;
        return this.FilterRemoteLinks(this.m_linkTypeById.TryGetValue(id, out linkType), isRemoteLinkingEnabled, ref linkType);
      }

      internal bool TryGetLinkTypeEndByName(
        string name,
        out WorkItemLinkTypeEnd linkType,
        bool isRemoteLinkingEnabled)
      {
        linkType = (WorkItemLinkTypeEnd) null;
        return this.FilterRemoteLinks(this.m_linkTypeEndByName.TryGetValue(name, out linkType), isRemoteLinkingEnabled, ref linkType);
      }

      internal bool TryGetLinkTypeEndByReferenceName(
        string referenceName,
        out WorkItemLinkTypeEnd linkTypeEnd,
        bool isRemoteLinkingEnabled)
      {
        linkTypeEnd = (WorkItemLinkTypeEnd) null;
        return this.FilterRemoteLinks(this.m_linkTypeEndByReferenceName.TryGetValue(referenceName, out linkTypeEnd), isRemoteLinkingEnabled, ref linkTypeEnd);
      }

      internal bool TryGetLinkTypeEndById(
        int id,
        out WorkItemLinkTypeEnd linkTypeEnd,
        bool isRemoteLinkingEnabled)
      {
        linkTypeEnd = (WorkItemLinkTypeEnd) null;
        return this.FilterRemoteLinks(this.m_linkTypeEndById.TryGetValue(id, out linkTypeEnd), isRemoteLinkingEnabled, ref linkTypeEnd);
      }

      internal int GetCount(bool isRemoteLinkingEnabled) => this.m_linkTypeByName.Where<KeyValuePair<string, MDWorkItemLinkType>>((Func<KeyValuePair<string, MDWorkItemLinkType>, bool>) (lt => !lt.Value.IsRemote | isRemoteLinkingEnabled)).Count<KeyValuePair<string, MDWorkItemLinkType>>();

      internal IEnumerable<string> GetLinkTypeReferenceNames(bool isRemoteLinkingEnabled) => this.m_linkTypeByReferenceName.Where<KeyValuePair<string, MDWorkItemLinkType>>((Func<KeyValuePair<string, MDWorkItemLinkType>, bool>) (lt => !lt.Value.IsRemote | isRemoteLinkingEnabled)).Select<KeyValuePair<string, MDWorkItemLinkType>, string>((Func<KeyValuePair<string, MDWorkItemLinkType>, string>) (lt => lt.Key));

      internal IEnumerable<string> GetLinkTypeEndReferenceNames(bool isRemoteLinkingEnabled) => this.m_linkTypeEndByReferenceName.Where<KeyValuePair<string, WorkItemLinkTypeEnd>>((Func<KeyValuePair<string, WorkItemLinkTypeEnd>, bool>) (lt => !lt.Value.LinkType.IsRemote | isRemoteLinkingEnabled)).Select<KeyValuePair<string, WorkItemLinkTypeEnd>, string>((Func<KeyValuePair<string, WorkItemLinkTypeEnd>, string>) (lt => lt.Key));

      internal IEnumerable<int> GetLinkTypeIds(bool isRemoteLinkingEnabled) => this.m_linkTypeById.Where<KeyValuePair<int, MDWorkItemLinkType>>((Func<KeyValuePair<int, MDWorkItemLinkType>, bool>) (lt => !lt.Value.IsRemote | isRemoteLinkingEnabled)).Select<KeyValuePair<int, MDWorkItemLinkType>, int>((Func<KeyValuePair<int, MDWorkItemLinkType>, int>) (lt => lt.Key));

      private bool FilterRemoteLinks(
        bool linkExists,
        bool isRemoteLinkingEnabled,
        ref MDWorkItemLinkType linkType)
      {
        if (linkExists && !isRemoteLinkingEnabled && linkType.IsRemote)
        {
          linkExists = false;
          linkType = (MDWorkItemLinkType) null;
        }
        return linkExists;
      }

      private bool FilterRemoteLinks(
        bool linkExists,
        bool isRemoteLinkingEnabled,
        ref WorkItemLinkTypeEnd linkTypeEnd)
      {
        if (linkExists && !isRemoteLinkingEnabled && linkTypeEnd.LinkType.IsRemote)
        {
          linkExists = false;
          linkTypeEnd = (WorkItemLinkTypeEnd) null;
        }
        return linkExists;
      }
    }
  }
}
