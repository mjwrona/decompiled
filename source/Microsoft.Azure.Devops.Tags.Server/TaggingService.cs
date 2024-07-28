// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.TaggingService
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.Azure.Devops.Tags.Server.Components;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.Azure.Devops.Tags.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class TaggingService : ITeamFoundationTaggingService, IVssFrameworkService
  {
    private Guid m_serviceHostId;
    private ITeamFoundationSecurityService m_securityService;
    private byte[] m_timestamp;
    private bool m_isCacheValid = true;
    private PluginTagProviderFactory m_tagProviderFactory;
    private const string s_area = "TeamFoundationTaggingService";
    private const string s_layer = "BusinessLogic";

    void IVssFrameworkService.ServiceStart(IVssRequestContext requestContext)
    {
      this.m_serviceHostId = requestContext.ServiceHost.InstanceId;
      this.m_tagProviderFactory = new PluginTagProviderFactory(requestContext.ServiceHost);
      this.m_securityService = requestContext.GetService<ITeamFoundationSecurityService>();
      requestContext.GetService<ITeamFoundationSqlNotificationService>().RegisterNotification(requestContext, "Default", SqlNotificationEventClasses.TaggingDefinitionsChanged, new SqlNotificationCallback(this.OnTaggingDefinitionsChangedNotification), false);
      this.PopulateTagDefinitionsCache(requestContext);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext requestContext)
    {
      requestContext.GetService<TeamFoundationSqlNotificationService>().UnregisterNotification(requestContext, "Default", SqlNotificationEventClasses.TaggingDefinitionsChanged, new SqlNotificationCallback(this.OnTaggingDefinitionsChangedNotification), false);
      if (this.m_tagProviderFactory == null)
        return;
      this.m_tagProviderFactory.Dispose();
      this.m_tagProviderFactory = (PluginTagProviderFactory) null;
    }

    public IEnumerable<ArtifactKind> GetAvailableArtifactKinds(IVssRequestContext requestContext) => requestContext.TraceBlock<IEnumerable<ArtifactKind>>(93326, 93327, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetAvailableArtifactKinds), (Func<IEnumerable<ArtifactKind>>) (() =>
    {
      this.ValidateRequestContext(requestContext);
      return this.GetAvailableArtifactKindsInternal(requestContext);
    }));

    public void CleanupUnusedTagDefinitions(IVssRequestContext requestContext, DateTime cutoffTime) => requestContext.TraceBlock(93361, 93362, "TeamFoundationTaggingService", "BusinessLogic", nameof (CleanupUnusedTagDefinitions), (Action) (() =>
    {
      this.ValidateRequestContext(requestContext);
      this.CheckPermission(requestContext, TaggingPermissions.Update, new Guid?());
      requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        component.CleanUnusedTagDefinitions(cutoffTime, true);
    }));

    public TagDefinition CreateTagDefinition(IVssRequestContext requestContext, string name) => this.CreateTagDefinition(requestContext, name, Guid.Empty);

    public TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds)
    {
      return this.CreateTagDefinition(requestContext, name, applicableArtifactKindIds, Guid.Empty);
    }

    public TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds,
      Guid scope)
    {
      return this.CreateTagDefinition(requestContext, name, applicableArtifactKindIds, scope, TagDefinitionStatus.Normal);
    }

    public TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds,
      Guid scope,
      TagDefinitionStatus status)
    {
      return requestContext.TraceBlock<TagDefinition>(93300, 93301, "TeamFoundationTaggingService", "BusinessLogic", nameof (CreateTagDefinition), (Func<TagDefinition>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.CheckPermission(requestContext, TaggingPermissions.Create, new Guid?(scope));
        this.CheckTagLimit(requestContext);
        name = TaggingService.CleanTagName(requestContext, name);
        TagsUtil.ValidateTagName(name);
        this.ValidateArtifactKinds(requestContext, applicableArtifactKindIds);
        return this.CreateTagDefinitionInternal(requestContext, name, applicableArtifactKindIds, scope, status);
      }));
    }

    public TagDefinition CreateTagDefinition(
      IVssRequestContext requestContext,
      string name,
      Guid scope)
    {
      return requestContext.TraceBlock<TagDefinition>(93302, 93303, "TeamFoundationTaggingService", "BusinessLogic", nameof (CreateTagDefinition), (Func<TagDefinition>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.CheckPermission(requestContext, TaggingPermissions.Create, new Guid?(scope));
        this.CheckTagLimit(requestContext);
        name = TaggingService.CleanTagName(requestContext, name);
        TagsUtil.ValidateTagName(name);
        return this.CreateTagDefinitionInternal(requestContext, name, (IEnumerable<Guid>) null, scope, TagDefinitionStatus.Normal);
      }));
    }

    public IEnumerable<TagDefinition> GetTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return requestContext.TraceBlock<IEnumerable<TagDefinition>>(93308, 93309, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinitions), (Func<IEnumerable<TagDefinition>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        TaggingService.ValidateTagIds(tagIds);
        return this.GetTagDefinitionsInternal(requestContext, tagIds);
      }));
    }

    public IEnumerable<TagDefinition> GetTagDefinitions(IVssRequestContext requestContext) => (IEnumerable<TagDefinition>) requestContext.TraceBlock<IOrderedEnumerable<TagDefinition>>(93308, 93311, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinitions), (Func<IOrderedEnumerable<TagDefinition>>) (() =>
    {
      this.ValidateRequestContext(requestContext);
      this.CheckPermission(requestContext, TaggingPermissions.Enumerate, new Guid?());
      this.EnsureCache(requestContext);
      return requestContext.GetService<TeamFoundationTaggingCacheService>().GetAllValues(requestContext).OrderBy<TagDefinition, string>((Func<TagDefinition, string>) (tag => tag.Name), (IComparer<string>) VssStringComparer.TagName);
    }));

    public IEnumerable<TagDefinition> GetTagDefinitions(
      IVssRequestContext requestContext,
      Guid scope)
    {
      return (IEnumerable<TagDefinition>) requestContext.TraceBlock<IOrderedEnumerable<TagDefinition>>(93334, 93335, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinitions), (Func<IOrderedEnumerable<TagDefinition>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.CheckPermission(requestContext, TaggingPermissions.Enumerate, new Guid?(scope));
        this.EnsureCache(requestContext);
        return requestContext.GetService<TeamFoundationTaggingCacheService>().GetValues(requestContext, scope).OrderBy<TagDefinition, string>((Func<TagDefinition, string>) (tag => tag.Name), (IComparer<string>) VssStringComparer.TagName);
      }));
    }

    public TagDefinition GetTagDefinition(IVssRequestContext requestContext, string name) => this.GetTagDefinition(requestContext, name, Guid.Empty);

    public TagDefinition GetTagDefinition(
      IVssRequestContext requestContext,
      string name,
      Guid scope)
    {
      return requestContext.TraceBlock<TagDefinition>(93312, 93313, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinition), (Func<TagDefinition>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.CheckPermission(requestContext, TaggingPermissions.Enumerate, new Guid?(scope));
        name = TaggingService.CleanTagName(requestContext, name);
        TagsUtil.ValidateTagName(name);
        TagDefinition tagDefinition;
        if (!requestContext.GetService<TeamFoundationTaggingCacheService>().TryGetValue(requestContext, scope, name, out tagDefinition))
        {
          using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
            tagDefinition = component.GetTagDefinitionByName(name, scope);
          if (tagDefinition != null)
            this.AddToCache(requestContext, (IEnumerable<TagDefinition>) new TagDefinition[1]
            {
              tagDefinition
            });
        }
        return tagDefinition;
      }));
    }

    public TagDefinition GetTagDefinition(IVssRequestContext requestContext, Guid tagId) => requestContext.TraceBlock<TagDefinition>(93314, 93315, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinition), (Func<TagDefinition>) (() =>
    {
      this.ValidateRequestContext(requestContext);
      TaggingService.ValidateTagId(tagId);
      TagDefinition tagDefinitionById;
      if (!requestContext.GetService<TeamFoundationTaggingCacheService>().TryGetValue(requestContext, tagId, out tagDefinitionById))
      {
        using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
          tagDefinitionById = component.GetTagDefinitionById(tagId);
        if (tagDefinitionById != null)
          this.AddToCache(requestContext, (IEnumerable<TagDefinition>) new TagDefinition[1]
          {
            tagDefinitionById
          });
      }
      if (tagDefinitionById != null)
        this.CheckPermission(requestContext, TaggingPermissions.Enumerate, new Guid?(tagDefinitionById.Scope));
      return tagDefinitionById;
    }));

    public IEnumerable<TagDefinition> QueryTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> applicableKinds)
    {
      return this.QueryTagDefinitions(requestContext, applicableKinds, Guid.Empty);
    }

    public IEnumerable<TagDefinition> QueryTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<Guid> applicableKinds,
      Guid scope)
    {
      return (IEnumerable<TagDefinition>) requestContext.TraceBlock<List<TagDefinition>>(93330, 93331, "TeamFoundationTaggingService", "BusinessLogic", nameof (QueryTagDefinitions), (Func<List<TagDefinition>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.CheckPermission(requestContext, TaggingPermissions.Enumerate, new Guid?(scope));
        this.ValidateArtifactKinds(requestContext, applicableKinds);
        this.EnsureCache(requestContext);
        IEnumerable<TagDefinition> values = requestContext.GetService<TeamFoundationTaggingCacheService>().GetValues(requestContext, scope);
        HashSet<Guid> applicableKindsSet = new HashSet<Guid>(applicableKinds ?? Enumerable.Empty<Guid>());
        int kindSetCount = applicableKindsSet.Count;
        Func<TagDefinition, bool> predicate = (Func<TagDefinition, bool>) (tagDefinition => tagDefinition.IncludesAllArtifactKinds || kindSetCount == 0 || tagDefinition.ApplicableKindIds.Count<Guid>((Func<Guid, bool>) (kindId => applicableKindsSet.Contains(kindId))) == kindSetCount);
        return values.Where<TagDefinition>(predicate).OrderBy<TagDefinition, string>((Func<TagDefinition, string>) (tag => tag.Name), (IComparer<string>) VssStringComparer.TagName).ToList<TagDefinition>();
      }));
    }

    public TagDefinition UpdateTagDefinition(
      IVssRequestContext requestContext,
      TagDefinition tagDefinition)
    {
      return requestContext.TraceBlock<TagDefinition>(93332, 93333, "TeamFoundationTaggingService", "BusinessLogic", nameof (UpdateTagDefinition), (Func<TagDefinition>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<TagDefinition>(tagDefinition, nameof (tagDefinition));
        this.CheckPermission(requestContext, TaggingPermissions.Update, new Guid?(tagDefinition.Scope));
        Guid tagId = tagDefinition.TagId;
        TaggingService.ValidateTagId(tagId);
        TagsUtil.ValidateTagName(TaggingService.CleanTagName(requestContext, tagDefinition.Name));
        using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        {
          tagDefinition = component.UpdateTagDefinition(tagDefinition);
          if (tagDefinition == null)
            throw new TagDefinitionNotFoundException(tagId);
        }
        this.AddToCache(requestContext, (IEnumerable<TagDefinition>) new TagDefinition[1]
        {
          tagDefinition
        });
        this.PublishTagDefinitionChangedEvent(requestContext);
        return tagDefinition;
      }));
    }

    public bool DeleteTagDefinition(IVssRequestContext requestContext, Guid tagId) => requestContext.TraceBlock<bool>(93336, 93337, "TeamFoundationTaggingService", "BusinessLogic", nameof (DeleteTagDefinition), (Func<bool>) (() =>
    {
      this.ValidateRequestContext(requestContext);
      TaggingService.ValidateTagId(tagId);
      IVssRequestContext requestContext1 = requestContext.Elevate();
      TagDefinition tagDefinition = this.GetTagDefinition(requestContext1, tagId);
      if (tagDefinition == null)
        throw new TagDefinitionNotFoundException(tagId);
      this.CheckPermission(requestContext, TaggingPermissions.Delete, new Guid?(tagDefinition.Scope));
      if (this.DeleteTags(requestContext1, (IEnumerable<Guid>) new Guid[1]
      {
        tagId
      }).Contains<Guid>(tagId))
        return false;
      using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        component.DeleteTagDefinitions((IEnumerable<Guid>) new Guid[1]
        {
          tagId
        });
      this.RemoveFromCache(requestContext, (IEnumerable<TagDefinition>) new TagDefinition[1]
      {
        tagDefinition
      });
      this.PublishTagDefinitionChangedEvent(requestContext);
      return true;
    }));

    public void DeleteTagDefinitionsInScope(IVssRequestContext requestContext, Guid scope) => requestContext.TraceBlock(93338, 93339, "TeamFoundationTaggingService", "BusinessLogic", nameof (DeleteTagDefinitionsInScope), (Action) (() =>
    {
      this.ValidateRequestContext(requestContext);
      this.CheckPermission(requestContext, TaggingPermissions.Delete, new Guid?(scope));
      Dictionary<Guid, TagDefinition> dictionary = this.GetTagDefinitions(requestContext.Elevate(), scope).ToDictionary<TagDefinition, Guid>((Func<TagDefinition, Guid>) (td => td.TagId));
      if (dictionary.Count == 0)
        return;
      foreach (Guid deleteTag in this.DeleteTags(requestContext, (IEnumerable<Guid>) dictionary.Keys))
        dictionary.Remove(deleteTag);
      if (dictionary.Count > 0)
      {
        using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
          component.DeleteTagDefinitions((IEnumerable<Guid>) dictionary.Keys);
        this.RemoveFromCache(requestContext, (IEnumerable<TagDefinition>) dictionary.Values);
      }
      this.PublishTagDefinitionChangedEvent(requestContext);
    }));

    public IEnumerable<TagDefinition> EnsureTagDefinitions(
      IVssRequestContext requestContext,
      IEnumerable<string> tagNames,
      IEnumerable<Guid> applicableKinds,
      Guid scope)
    {
      return (IEnumerable<TagDefinition>) requestContext.TraceBlock<List<TagDefinition>>(93342, 93344, "TeamFoundationTaggingService", "BusinessLogic", nameof (EnsureTagDefinitions), (Func<List<TagDefinition>>) (() =>
      {
        this.EnsureCache(requestContext);
        TeamFoundationTaggingCacheService service = requestContext.GetService<TeamFoundationTaggingCacheService>();
        tagNames = tagNames.Select<string, string>((Func<string, string>) (t => TaggingService.CleanTagName(requestContext, t)));
        HashSet<string> stringSet = new HashSet<string>(tagNames, (IEqualityComparer<string>) VssStringComparer.TagName);
        List<string> stringList = new List<string>();
        List<TagDefinition> tagDefinitionList1 = new List<TagDefinition>();
        List<TagDefinition> tagDefinitionList2 = new List<TagDefinition>();
        foreach (string name1 in stringSet)
        {
          TagDefinition tagDefinition1;
          if (service.TryGetValue(requestContext, scope, name1, out tagDefinition1))
          {
            if (tagDefinition1.IsDeleted)
            {
              List<TagDefinition> tagDefinitionList3 = tagDefinitionList1;
              TagDefinition tag = tagDefinition1;
              string str = name1;
              TagDefinitionStatus? nullable = new TagDefinitionStatus?(TagDefinitionStatus.Normal);
              Guid? tagId = new Guid?();
              string name2 = str;
              bool? includesAllArtifactKinds = new bool?();
              Guid? scope1 = new Guid?();
              TagDefinitionStatus? status = nullable;
              DateTime? lastUpdated = new DateTime?();
              TagDefinition tagDefinition2 = tag.Clone(tagId, name2, includesAllArtifactKinds: includesAllArtifactKinds, scope: scope1, status: status, lastUpdated: lastUpdated);
              tagDefinitionList3.Add(tagDefinition2);
            }
            else
              tagDefinitionList2.Add(tagDefinition1);
          }
          else
            stringList.Add(name1);
        }
        if (stringList.Count > 0)
        {
          foreach (string name in stringList)
          {
            TagDefinition tagDefinition;
            try
            {
              tagDefinition = this.CreateTagDefinition(requestContext, name, scope);
            }
            catch (DuplicateTagNameException ex)
            {
              tagDefinition = this.GetTagDefinition(requestContext, name, scope);
            }
            tagDefinitionList2.Add(tagDefinition);
          }
        }
        if (tagDefinitionList1.Count > 0)
        {
          this.CheckPermission(requestContext, TaggingPermissions.Create, new Guid?(scope));
          IVssRequestContext requestContext1 = requestContext.Elevate();
          foreach (TagDefinition tagDefinition3 in tagDefinitionList1)
          {
            TagDefinition tagDefinition4 = this.UpdateTagDefinition(requestContext1, tagDefinition3);
            tagDefinitionList2.Add(tagDefinition4);
          }
        }
        return tagDefinitionList2;
      }));
    }

    public TagHistoryEntry<T>[] GetTagsHistoryForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact)
    {
      return requestContext.TraceBlock<TagHistoryEntry<T>[]>(93328, 93329, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagsHistoryForArtifact), (Func<TagHistoryEntry<T>[]>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        this.ValidateArtifactKind(requestContext, artifactKind);
        return !this.FilterOnReadPermissions<T>(requestContext, artifactKind, (IEnumerable<T>) new T[1]
        {
          artifact.Id
        }).Any<T>() ? new TagHistoryEntry<T>[0] : this.RebuildTagsHistoryFromRawData<T>(requestContext, this.GetTagStorageProvider<T>(requestContext, artifactKind).GetTagsHistoryForArtifact(requestContext, artifact));
      }));
    }

    internal TagHistoryEntry<T>[] RebuildTagsHistoryFromRawData<T>(
      IVssRequestContext requestContext,
      TagIdsHistoryEntry<T>[] history)
    {
      Dictionary<Guid, TagDefinition> tagsMap = this.GetTagDefinitionMapForTagIds(requestContext, ((IEnumerable<TagIdsHistoryEntry<T>>) history).SelectMany<TagIdsHistoryEntry<T>, Guid>((Func<TagIdsHistoryEntry<T>, IEnumerable<Guid>>) (artifactTagIds => artifactTagIds.Tags)));
      return ((IEnumerable<TagIdsHistoryEntry<T>>) history).Select<TagIdsHistoryEntry<T>, TagHistoryEntry<T>>((Func<TagIdsHistoryEntry<T>, TagHistoryEntry<T>>) (artifactTagIds => new TagHistoryEntry<T>(artifactTagIds.ArtifactVersion, artifactTagIds.Tags.Where<Guid>((Func<Guid, bool>) (id => tagsMap.ContainsKey(id))).Select<Guid, TagDefinition>((Func<Guid, TagDefinition>) (id => tagsMap[id])), artifactTagIds.ChangedDate, artifactTagIds.ChangedBy))).ToArray<TagHistoryEntry<T>>();
    }

    public ArtifactTags<T> GetTagsForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact)
    {
      return this.GetTagsForArtifacts<T>(requestContext, artifactKind, (IEnumerable<TagArtifact<T>>) new TagArtifact<T>[1]
      {
        artifact
      }).FirstOrDefault<ArtifactTags<T>>() ?? new ArtifactTags<T>(new VersionedTagArtifact<T>(artifact.DataspaceIdentifier, artifact.Id, 0), (IEnumerable<TagDefinition>) new TagDefinition[0]);
    }

    public IEnumerable<ArtifactTags<T>> GetTagsForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      return requestContext.TraceBlock<IEnumerable<ArtifactTags<T>>>(93328, 93329, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagsForArtifacts), (Func<IEnumerable<ArtifactTags<T>>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<IEnumerable<TagArtifact<T>>>(artifacts, nameof (artifacts));
        return this.GetTagForArtifactsInternal<T>(requestContext, artifactKind, artifacts);
      }));
    }

    public IEnumerable<ArtifactTags<T>> GetTagsForVersionedArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ICollection<VersionedTagArtifact<T>> artifacts)
    {
      return requestContext.TraceBlock<IEnumerable<ArtifactTags<T>>>(93328, 93329, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagsForVersionedArtifacts), (Func<IEnumerable<ArtifactTags<T>>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<ICollection<VersionedTagArtifact<T>>>(artifacts, nameof (artifacts));
        return this.GetTagForVersionedArtifactsInternal<T>(requestContext, artifactKind, artifacts);
      }));
    }

    public void UpdateTagsForArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      TagArtifact<T> artifact,
      IEnumerable<Guid> addedTagIds,
      IEnumerable<Guid> removedTagIds,
      Guid changedBy,
      int? version = null)
    {
      requestContext.TraceBlock(93318, 93319, "TeamFoundationTaggingService", "BusinessLogic", nameof (UpdateTagsForArtifact), (Action) (() =>
      {
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(addedTagIds, nameof (addedTagIds));
        ArgumentUtility.CheckForNull<IEnumerable<Guid>>(removedTagIds, nameof (removedTagIds));
        this.ValidateRequestContext(requestContext);
        this.ValidateArtifactKind(requestContext, artifactKind);
        this.ValidateTagIds(requestContext, artifactKind, addedTagIds.Union<Guid>(removedTagIds));
        if (addedTagIds.Intersect<Guid>(removedTagIds).Any<Guid>())
          throw new InvalidOperationException();
        if (!this.HasWritePermissionsOnArtifact<T>(requestContext, artifactKind, artifact.Id))
          return;
        ITagStorageProvider<T> tagStorageProvider = this.GetTagStorageProvider<T>(requestContext, artifactKind);
        if (!version.HasValue)
        {
          TagIdsHistoryEntry<T>[] historyForArtifact = tagStorageProvider.GetTagsHistoryForArtifact(requestContext, artifact);
          version = new int?(1);
          if (((IEnumerable<TagIdsHistoryEntry<T>>) historyForArtifact).Any<TagIdsHistoryEntry<T>>())
          {
            TagIdsHistoryEntry<T> tagIdsHistoryEntry = ((IEnumerable<TagIdsHistoryEntry<T>>) historyForArtifact).Last<TagIdsHistoryEntry<T>>();
            version = new int?(tagIdsHistoryEntry.ArtifactVersion.Version + 1);
            addedTagIds = addedTagIds.Except<Guid>(tagIdsHistoryEntry.Tags);
            removedTagIds = removedTagIds.Intersect<Guid>(tagIdsHistoryEntry.Tags);
          }
        }
        ArtifactTagUpdate<T> artifactTagUpdate = new ArtifactTagUpdate<T>()
        {
          Artifact = new VersionedTagArtifact<T>(artifact.DataspaceIdentifier, artifact.Id, version.Value),
          AddedTagIds = addedTagIds,
          RemovedTagIds = removedTagIds
        };
        tagStorageProvider.UpdateTagsForArtifacts(requestContext, (IEnumerable<ArtifactTagUpdate<T>>) new ArtifactTagUpdate<T>[1]
        {
          artifactTagUpdate
        }, new DateTime?(DateTime.UtcNow), new Guid?(changedBy));
      }));
    }

    public void UpdateTagsForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<ArtifactTagUpdate<T>> tagUpdates,
      DateTime? changedDate,
      Guid? changedBy)
    {
      requestContext.TraceBlock(93347, 93351, "TeamFoundationTaggingService", "BusinessLogic", nameof (UpdateTagsForArtifacts), (Action) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<IEnumerable<ArtifactTagUpdate<T>>>(tagUpdates, nameof (tagUpdates));
        this.ValidateArtifactKind(requestContext, artifactKind);
        List<ArtifactTagUpdate<T>> artifactTagUpdateList = new List<ArtifactTagUpdate<T>>();
        foreach (ArtifactTagUpdate<T> tagUpdate in tagUpdates)
        {
          if (tagUpdate.AddedTagIds != null && tagUpdate.AddedTagIds.Any<Guid>())
          {
            if (tagUpdate.RemovedTagIds != null && tagUpdate.RemovedTagIds.Any<Guid>() && tagUpdate.AddedTagIds.Intersect<Guid>(tagUpdate.RemovedTagIds).Any<Guid>())
              throw new ArgumentException(Resources.TagUpdateAddRemoveConflict(), nameof (tagUpdates));
          }
          else if (tagUpdate.RemovedTagIds == null || !tagUpdate.RemovedTagIds.Any<Guid>())
            continue;
          artifactTagUpdateList.Add(tagUpdate);
        }
        if (artifactTagUpdateList.Count <= 0)
          return;
        if (!this.HasWritePermissionsOnArtifacts<T>(requestContext, artifactKind, artifactTagUpdateList.Select<ArtifactTagUpdate<T>, T>((Func<ArtifactTagUpdate<T>, T>) (tu => tu.Artifact.Id))))
          throw new TagException(Resources.TagExceptionWriteAccessDenied());
        this.GetTagStorageProvider<T>(requestContext, artifactKind).UpdateTagsForArtifacts(requestContext, (IEnumerable<ArtifactTagUpdate<T>>) artifactTagUpdateList, changedDate, changedBy);
      }));
    }

    public IEnumerable<VersionedTagArtifact<T>> QueryArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<Guid> tagIds)
    {
      return requestContext.TraceBlock<IEnumerable<VersionedTagArtifact<T>>>(93324, 93325, "TeamFoundationTaggingService", "BusinessLogic", nameof (QueryArtifacts), (Func<IEnumerable<VersionedTagArtifact<T>>>) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tagIds, nameof (tagIds));
        this.ValidateArtifactKind(requestContext, artifactKind);
        TaggingService.ValidateTagIds(tagIds);
        IEnumerable<VersionedTagArtifact<T>> source = this.GetTagStorageProvider<T>(requestContext, artifactKind).QueryArtifacts(requestContext, tagIds);
        Dictionary<T, VersionedTagArtifact<T>> artifactMap = new Dictionary<T, VersionedTagArtifact<T>>(source.Count<VersionedTagArtifact<T>>());
        foreach (VersionedTagArtifact<T> versionedTagArtifact in source)
          artifactMap[versionedTagArtifact.Id] = versionedTagArtifact;
        return this.FilterOnReadPermissions<T>(requestContext, artifactKind, (IEnumerable<T>) artifactMap.Keys).Select<T, VersionedTagArtifact<T>>((Func<T, VersionedTagArtifact<T>>) (id => artifactMap[id]));
      }));
    }

    public void DeleteTagHistoryForArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      requestContext.TraceBlock(93352, 93353, "TeamFoundationTaggingService", "BusinessLogic", nameof (DeleteTagHistoryForArtifacts), (Action) (() =>
      {
        this.ValidateRequestContext(requestContext);
        ArgumentUtility.CheckForNull<IEnumerable<TagArtifact<T>>>(artifacts, nameof (artifacts));
        this.ValidateArtifactKind(requestContext, artifactKind);
        if (!artifacts.Any<TagArtifact<T>>())
          return;
        if (!this.HasWritePermissionsOnArtifacts<T>(requestContext, artifactKind, artifacts.Select<TagArtifact<T>, T>((Func<TagArtifact<T>, T>) (a => a.Id))))
          throw new TagException(Resources.TagExceptionWriteAccessDenied());
        this.GetTagStorageProvider<T>(requestContext, artifactKind).DeleteTagHistoryForArtifacts(requestContext, artifacts);
      }));
    }

    protected virtual IEnumerable<Guid> DeleteTags(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return (IEnumerable<Guid>) requestContext.TraceBlock<Guid[]>(93341, 93343, "TeamFoundationTaggingService", "BusinessLogic", nameof (DeleteTags), (Func<Guid[]>) (() =>
      {
        IEnumerable<Guid> knownArtifactKinds = DefaultTagProvider<object>.GetKnownArtifactKinds(requestContext);
        IEnumerable<ITagProvider> list = (IEnumerable<ITagProvider>) this.m_tagProviderFactory.GetTagProviders(requestContext).ToList<ITagProvider>();
        IEnumerable<Guid> second1 = list.Select<ITagProvider, Guid>((Func<ITagProvider, Guid>) (p => p.ArtifactKind));
        IEnumerable<ITagProvider> second2 = (IEnumerable<ITagProvider>) knownArtifactKinds.Except<Guid>(second1).Select<Guid, DefaultTagProvider<object>>((Func<Guid, DefaultTagProvider<object>>) (kind => new DefaultTagProvider<object>(kind)));
        List<Guid> source = new List<Guid>();
        foreach (ITagProvider tagProvider in list.Union<ITagProvider>(second2))
        {
          IEnumerable<Guid> collection = tagProvider.DeleteAllTagAssociations(requestContext, tagIds);
          source.AddRange(collection);
        }
        return source.Distinct<Guid>().ToArray<Guid>();
      }));
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool DatabaseSupportsGetTaggedArtifactsFunctions(IVssRequestContext requestContext) => requestContext.TraceBlock<bool>(93354, 93355, "TeamFoundationTaggingService", "BusinessLogic", nameof (DatabaseSupportsGetTaggedArtifactsFunctions), (Func<bool>) (() =>
    {
      using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        return component.SupportsGetTaggedArtifactsFunctions;
    }));

    private IEnumerable<ArtifactTags<T>> GetTagForArtifactsInternal<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<TagArtifact<T>> artifacts)
    {
      return requestContext.TraceBlock<IEnumerable<ArtifactTags<T>>>(93356, 93357, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagForArtifactsInternal), (Func<IEnumerable<ArtifactTags<T>>>) (() =>
      {
        this.ValidateArtifactKind(requestContext, artifactKind);
        if (!artifacts.Any<TagArtifact<T>>())
          return Enumerable.Empty<ArtifactTags<T>>();
        Dictionary<T, TagArtifact<T>> artifactMap = new Dictionary<T, TagArtifact<T>>(artifacts.Count<TagArtifact<T>>());
        foreach (TagArtifact<T> artifact in artifacts)
          artifactMap[artifact.Id] = artifact;
        IEnumerable<T> source = this.FilterOnReadPermissions<T>(requestContext, artifactKind, (IEnumerable<T>) artifactMap.Keys);
        return !source.Any<T>() ? Enumerable.Empty<ArtifactTags<T>>() : this.ConvertArtifactIdsToTags<T>(requestContext, this.GetTagStorageProvider<T>(requestContext, artifactKind).GetTagsForArtifacts(requestContext, source.Select<T, TagArtifact<T>>((Func<T, TagArtifact<T>>) (id => artifactMap[id]))));
      }));
    }

    private IEnumerable<ArtifactTags<T>> GetTagForVersionedArtifactsInternal<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      ICollection<VersionedTagArtifact<T>> artifacts)
    {
      return requestContext.TraceBlock<IEnumerable<ArtifactTags<T>>>(93360, 93366, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagForVersionedArtifactsInternal), (Func<IEnumerable<ArtifactTags<T>>>) (() =>
      {
        this.ValidateArtifactKind(requestContext, artifactKind);
        if (artifacts.Count == 0)
          return Enumerable.Empty<ArtifactTags<T>>();
        Dictionary<T, IList<VersionedTagArtifact<T>>> artifactMap = new Dictionary<T, IList<VersionedTagArtifact<T>>>(((IEnumerable<VersionedTagArtifact<T>>) artifacts).Count<VersionedTagArtifact<T>>());
        foreach (VersionedTagArtifact<T> artifact in (IEnumerable<VersionedTagArtifact<T>>) artifacts)
        {
          IList<VersionedTagArtifact<T>> versionedTagArtifactList;
          if (!artifactMap.TryGetValue(artifact.Id, out versionedTagArtifactList))
          {
            versionedTagArtifactList = (IList<VersionedTagArtifact<T>>) new List<VersionedTagArtifact<T>>();
            artifactMap.Add(artifact.Id, versionedTagArtifactList);
          }
          versionedTagArtifactList.Add(artifact);
        }
        IEnumerable<T> source = this.FilterOnReadPermissions<T>(requestContext, artifactKind, (IEnumerable<T>) artifactMap.Keys);
        return !source.Any<T>() ? Enumerable.Empty<ArtifactTags<T>>() : this.ConvertArtifactIdsToTags<T>(requestContext, this.GetTagStorageProvider<T>(requestContext, artifactKind).GetTagsForVersionedArtifacts(requestContext, source.SelectMany<T, VersionedTagArtifact<T>>((Func<T, IEnumerable<VersionedTagArtifact<T>>>) (id => (IEnumerable<VersionedTagArtifact<T>>) artifactMap[id]))));
      }));
    }

    private IEnumerable<ArtifactTags<T>> ConvertArtifactIdsToTags<T>(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactTagIds<T>> artifactTagIds)
    {
      return (IEnumerable<ArtifactTags<T>>) requestContext.TraceBlock<ArtifactTags<T>[]>(93367, 93368, "TeamFoundationTaggingService", "BusinessLogic", nameof (ConvertArtifactIdsToTags), (Func<ArtifactTags<T>[]>) (() =>
      {
        Guid[] array = artifactTagIds.SelectMany<ArtifactTagIds<T>, Guid>((Func<ArtifactTagIds<T>, IEnumerable<Guid>>) (a => a.Tags ?? Enumerable.Empty<Guid>())).ToArray<Guid>();
        Dictionary<Guid, TagDefinition> tagsMap = this.GetTagDefinitionMapForTagIds(requestContext, (IEnumerable<Guid>) array);
        return artifactTagIds.Select<ArtifactTagIds<T>, ArtifactTags<T>>((Func<ArtifactTagIds<T>, ArtifactTags<T>>) (artifactTagId =>
        {
          TagDefinition tagDefinition;
          IEnumerable<TagDefinition> tags = artifactTagId.Tags == null || !artifactTagId.Tags.Any<Guid>() ? Enumerable.Empty<TagDefinition>() : (IEnumerable<TagDefinition>) artifactTagId.Tags.Select<Guid, TagDefinition>((Func<Guid, TagDefinition>) (tagId => tagsMap.TryGetValue(tagId, out tagDefinition) ? tagDefinition : (TagDefinition) null)).Where<TagDefinition>((Func<TagDefinition, bool>) (tagDef => tagDef != null)).ToArray<TagDefinition>();
          return new ArtifactTags<T>(artifactTagId.Artifact, tags);
        })).ToArray<ArtifactTags<T>>();
      }));
    }

    private void PublishTagDefinitionChangedEvent(IVssRequestContext requestContext)
    {
      TaggingDefinitionsChangedEvent notificationEvent = new TaggingDefinitionsChangedEvent();
      requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) notificationEvent);
    }

    private Dictionary<Guid, TagDefinition> GetTagDefinitionMapForTagIds(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return this.GetTagDefinitionsInternal(requestContext, tagIds).ToDictionary<TagDefinition, Guid>((Func<TagDefinition, Guid>) (tag => tag.TagId));
    }

    private IEnumerable<ArtifactKind> GetAvailableArtifactKindsInternal(
      IVssRequestContext requestContext)
    {
      return requestContext.TraceBlock<IEnumerable<ArtifactKind>>(93369, 93370, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetAvailableArtifactKindsInternal), (Func<IEnumerable<ArtifactKind>>) (() => requestContext.GetService<ITeamFoundationPropertyService>().GetArtifactKinds(requestContext).Where<ArtifactKind>((Func<ArtifactKind, bool>) (kind => ArtifactKindFlags.TagEnabled == (kind.Flags & ArtifactKindFlags.TagEnabled)))));
    }

    private TagDefinition CreateTagDefinitionInternal(
      IVssRequestContext requestContext,
      string name,
      IEnumerable<Guid> applicableArtifactKindIds,
      Guid scope,
      TagDefinitionStatus status)
    {
      return requestContext.TraceBlock<TagDefinition>(93371, 93372, "TeamFoundationTaggingService", "BusinessLogic", nameof (CreateTagDefinitionInternal), (Func<TagDefinition>) (() =>
      {
        TagDefinition tag;
        if (applicableArtifactKindIds == null)
        {
          tag = new TagDefinition(Guid.NewGuid(), name, (IEnumerable<Guid>) null, true, scope, TagDefinitionStatus.Normal, DateTime.UtcNow);
        }
        else
        {
          TeamFoundationPropertyService propertyService = requestContext.GetService<TeamFoundationPropertyService>();
          applicableArtifactKindIds.Select<Guid, ArtifactKind>((Func<Guid, ArtifactKind>) (id => propertyService.GetArtifactKind(requestContext, id)));
          tag = new TagDefinition(Guid.NewGuid(), name, (IEnumerable<Guid>) applicableArtifactKindIds.ToList<Guid>().AsReadOnly(), false, scope, status, DateTime.UtcNow);
        }
        using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        {
          if (requestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(requestContext, "TeamFoundationTaggingService.SkipInvalidCharactersVerificationDuringTagCreation") && component is TaggingComponent11 taggingComponent11_2)
            taggingComponent11_2.CreateTagDefinitionExtendedCharSupport(tag);
          else
            component.CreateTagDefinition(tag);
        }
        this.AddToCache(requestContext, (IEnumerable<TagDefinition>) new TagDefinition[1]
        {
          tag
        });
        this.PublishTagDefinitionChangedEvent(requestContext);
        return tag;
      }));
    }

    private IEnumerable<TagDefinition> GetTagDefinitionsInternal(
      IVssRequestContext requestContext,
      IEnumerable<Guid> tagIds)
    {
      return requestContext.TraceBlock<IEnumerable<TagDefinition>>(93373, 93374, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagDefinitionsInternal), (Func<IEnumerable<TagDefinition>>) (() =>
      {
        if (!tagIds.Any<Guid>())
          return Enumerable.Empty<TagDefinition>();
        tagIds = tagIds.Distinct<Guid>();
        TeamFoundationTaggingCacheService service = requestContext.GetService<TeamFoundationTaggingCacheService>();
        List<TagDefinition> definitionsInternal = new List<TagDefinition>();
        List<Guid> tagIds1 = new List<Guid>();
        foreach (Guid tagId in tagIds)
        {
          TagDefinition tagDefinition;
          if (service.TryGetValue(requestContext, tagId, out tagDefinition))
            definitionsInternal.Add(tagDefinition);
          else
            tagIds1.Add(tagId);
        }
        if (tagIds1.Count > 0)
        {
          IEnumerable<TagDefinition> tagDefinitionsById;
          using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
            tagDefinitionsById = component.GetTagDefinitionsById((IEnumerable<Guid>) tagIds1);
          if (tagDefinitionsById.Any<TagDefinition>())
          {
            definitionsInternal.AddRange(tagDefinitionsById);
            this.AddToCache(requestContext, tagDefinitionsById);
          }
        }
        return (IEnumerable<TagDefinition>) definitionsInternal;
      }));
    }

    private void CheckTagLimit(IVssRequestContext requestContext) => requestContext.TraceBlock(93383, 93384, "TeamFoundationTaggingService", "BusinessLogic", nameof (CheckTagLimit), (Action) (() =>
    {
      int allTagCount = requestContext.GetService<TeamFoundationTaggingCacheService>().GetAllTagCount(requestContext);
      int tagsLimit = this.GetTagsLimit(requestContext);
      int num = tagsLimit;
      if (allTagCount > num)
        throw new TagDefinitionLimitExceededException(tagsLimit);
    }));

    private int GetTagsLimit(IVssRequestContext requestContext)
    {
      int num = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Tagging/Settings/TagLimit", true, 150000);
      return num > 100000 && num < 150000 ? 150000 : num;
    }

    private void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.TaggingServiceRequestContextHostMessage((object) this.m_serviceHostId, (object) requestContext.ServiceHost.InstanceId));
    }

    private static void ValidateTagId(Guid tagId) => ArgumentUtility.CheckForEmptyGuid(tagId, nameof (tagId));

    private static void ValidateTagIds(IEnumerable<Guid> tagIds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) tagIds, nameof (tagIds));
      foreach (Guid tagId in tagIds)
        TaggingService.ValidateTagId(tagId);
    }

    private void ValidateTagIds(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<Guid> tagIds)
    {
      TaggingService.ValidateTagIds(tagIds);
      if (this.GetTagDefinitions(requestContext.Elevate(), tagIds).Any<TagDefinition>((Func<TagDefinition, bool>) (tagDefinition => !tagDefinition.IncludesAllArtifactKinds && !tagDefinition.ApplicableKindIds.Contains<Guid>(artifactKind))))
        throw new InvalidTagArtifactKindException(artifactKind);
    }

    private void ValidateArtifactKind(IVssRequestContext requestContext, Guid artifactKind)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      if (!this.GetAvailableArtifactKindsInternal(requestContext).Select<ArtifactKind, Guid>((Func<ArtifactKind, Guid>) (ak => ak.Kind)).Contains<Guid>(artifactKind))
        throw new InvalidTagArtifactKindException(artifactKind);
    }

    private void ValidateArtifactKinds(
      IVssRequestContext requestContext,
      IEnumerable<Guid> artifactKinds)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) artifactKinds, nameof (artifactKinds));
      IEnumerable<Guid> source = this.GetAvailableArtifactKindsInternal(requestContext).Select<ArtifactKind, Guid>((Func<ArtifactKind, Guid>) (ak => ak.Kind));
      foreach (Guid artifactKind in artifactKinds)
      {
        ArgumentUtility.CheckForEmptyGuid(artifactKind, "artifactKind");
        if (!source.Contains<Guid>(artifactKind))
          throw new InvalidTagArtifactKindException(artifactKind);
      }
    }

    protected virtual void CheckPermission(
      IVssRequestContext requestContext,
      int permissions,
      Guid? scope)
    {
      requestContext.TraceBlock(93345, 93346, "TeamFoundationTaggingService", "BusinessLogic", nameof (CheckPermission), (Action) (() =>
      {
        IVssSecurityNamespace securityNamespace = requestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(requestContext, FrameworkSecurity.TaggingNamespaceId);
        bool supportsPermissions;
        using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
          supportsPermissions = component.SupportsPermissions;
        if (securityNamespace != null & supportsPermissions)
        {
          string securityToken = TaggingService.GetSecurityToken(scope);
          if (!securityNamespace.HasPermission(requestContext, securityToken, permissions, false))
          {
            string message = string.Empty;
            if ((permissions & TaggingPermissions.Delete) == TaggingPermissions.Delete)
              message = Resources.TagDeleteAccessDenied();
            else if ((permissions & TaggingPermissions.Update) == TaggingPermissions.Update)
              message = Resources.TagUpdateAccessDenied();
            else if ((permissions & TaggingPermissions.Create) == TaggingPermissions.Create)
              message = Resources.TagCreateAccessDenied();
            else if ((permissions & TaggingPermissions.Enumerate) == TaggingPermissions.Enumerate)
              message = Resources.TagEnumerateAccessDenied();
            throw new AccessCheckException(requestContext.UserContext, securityToken, permissions, FrameworkSecurity.TaggingNamespaceId, message);
          }
        }
        else
          this.m_securityService.GetSecurityNamespace(requestContext, FrameworkSecurity.FrameworkNamespaceId).CheckPermission(requestContext, FrameworkSecurity.FrameworkNamespaceToken, 1);
      }));
    }

    public static string GetSecurityToken(Guid? scope) => scope.HasValue ? "/" + scope.ToString() : "/";

    public static string CleanTagName(IVssRequestContext requestContext, string name)
    {
      if (name == null)
        return (string) null;
      name = name.Trim();
      if (TaggingHelper.IsUnicodeNormalizationEnabled(requestContext))
        name = TaggingHelper.NormalizeUnicode(name);
      return name;
    }

    private bool HasReadPermissionsOnArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      T artifactId)
    {
      if (requestContext.IsSystemContext)
        return true;
      return this.FilterOnReadPermissions<T>(requestContext, artifactKind, (IEnumerable<T>) new T[1]
      {
        artifactId
      }).Any<T>();
    }

    private IEnumerable<T> FilterOnReadPermissions<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<T> artifactIds)
    {
      return requestContext.TraceBlock<IEnumerable<T>>(93375, 93376, "TeamFoundationTaggingService", "BusinessLogic", nameof (FilterOnReadPermissions), (Func<IEnumerable<T>>) (() =>
      {
        if (requestContext.IsSystemContext)
          return artifactIds;
        ITagSecurityProvider<T> securityProvider = this.GetTagSecurityProvider<T>(requestContext, artifactKind);
        if (securityProvider == null)
          return artifactIds;
        artifactIds = securityProvider.FilterOnReadPermissions(requestContext, artifactIds);
        return artifactIds;
      }));
    }

    private bool HasWritePermissionsOnArtifact<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      T artifactId)
    {
      if (requestContext.IsSystemContext)
        return true;
      return this.FilterOnWritePermissions<T>(requestContext, artifactKind, (IEnumerable<T>) new T[1]
      {
        artifactId
      }).Any<T>();
    }

    private bool HasWritePermissionsOnArtifacts<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<T> artifactIds)
    {
      if (requestContext.IsSystemContext)
        return true;
      T[] array = artifactIds.ToArray<T>();
      IEnumerable<T> source = this.FilterOnWritePermissions<T>(requestContext, artifactKind, (IEnumerable<T>) array);
      return array.Length == source.Count<T>();
    }

    private IEnumerable<T> FilterOnWritePermissions<T>(
      IVssRequestContext requestContext,
      Guid artifactKind,
      IEnumerable<T> artifactIds)
    {
      return requestContext.TraceBlock<IEnumerable<T>>(93377, 93378, "TeamFoundationTaggingService", "BusinessLogic", nameof (FilterOnWritePermissions), (Func<IEnumerable<T>>) (() =>
      {
        if (requestContext.IsSystemContext)
          return artifactIds;
        ITagSecurityProvider<T> securityProvider = this.GetTagSecurityProvider<T>(requestContext, artifactKind);
        if (securityProvider == null)
          return artifactIds;
        artifactIds = securityProvider.FilterOnWritePermissions(requestContext, artifactIds);
        return artifactIds;
      }));
    }

    private ITagProvider<T> GetTagProvider<T>(IVssRequestContext requestContext, Guid artifactKind) => requestContext.TraceBlock<ITagProvider<T>>(93379, 93380, "TeamFoundationTaggingService", "BusinessLogic", nameof (GetTagProvider), (Func<ITagProvider<T>>) (() => this.m_tagProviderFactory.GetOrCreateTagProvider<T>(requestContext, artifactKind) ?? (ITagProvider<T>) new DefaultTagProvider<T>(artifactKind)));

    private ITagStorageProvider<T> GetTagStorageProvider<T>(
      IVssRequestContext requestContext,
      Guid artifactKind)
    {
      ITagProvider<T> tagProvider = this.GetTagProvider<T>(requestContext, artifactKind);
      return tagProvider.StorageProvider != null ? tagProvider.StorageProvider : new DefaultTagProvider<T>(artifactKind).StorageProvider;
    }

    private ITagSecurityProvider<T> GetTagSecurityProvider<T>(
      IVssRequestContext requestContext,
      Guid artifactKind)
    {
      return this.GetTagProvider<T>(requestContext, artifactKind).SecurityProvider;
    }

    private void AddToCache(IVssRequestContext requestContext, IEnumerable<TagDefinition> tags)
    {
      TeamFoundationTaggingCacheService service = requestContext.GetService<TeamFoundationTaggingCacheService>();
      foreach (TagDefinition tag in tags)
      {
        if (TaggingHelper.IsUnicodeNormalizationEnabled(requestContext))
          tag.Normalize();
        service.Set(requestContext, tag.TagId, tag);
      }
    }

    private void RemoveFromCache(IVssRequestContext requestContext, IEnumerable<TagDefinition> tags)
    {
      TeamFoundationTaggingCacheService service = requestContext.GetService<TeamFoundationTaggingCacheService>();
      foreach (TagDefinition tag in tags)
        service.Remove(requestContext, tag.TagId);
    }

    private void ClearCache(IVssRequestContext requestContext) => requestContext.GetService<TeamFoundationTaggingCacheService>().Clear(requestContext);

    private void PopulateTagDefinitionsCache(IVssRequestContext requestContext) => requestContext.TraceBlock(93350, 93359, 93358, "TeamFoundationTaggingService", "BusinessLogic", nameof (PopulateTagDefinitionsCache), (Action) (() =>
    {
      IList<TagDefinition> list;
      using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        list = (IList<TagDefinition>) component.GetTagDefinitionsById(Enumerable.Empty<Guid>()).ToList<TagDefinition>();
      this.ClearCache(requestContext);
      this.AddToCache(requestContext, (IEnumerable<TagDefinition>) list);
      this.PublishTagCountTelemetry(requestContext, list.Count);
      this.m_isCacheValid = true;
    }));

    private void PublishTagCountTelemetry(IVssRequestContext requestContext, int count)
    {
      int tagsLimit = this.GetTagsLimit(requestContext);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("TagCount", (double) count);
      properties.Add("TagLimit", (double) tagsLimit);
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "TeamFoundationTaggingService", nameof (TaggingService), properties);
    }

    private void OnTaggingDefinitionsChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.TraceBlock(93340, 93349, 93348, "TeamFoundationTaggingService", "BusinessLogic", nameof (OnTaggingDefinitionsChangedNotification), (Action) (() =>
      {
        if (requestContext.IsFeatureEnabled("TeamFoundationTaggingService.Safeguard.RebuildCacheOnChangeNotification"))
        {
          if (eventData == null)
            this.PopulateTagDefinitionsCache(requestContext);
          else
            this.RefreshUpdatedTagDefinitions(requestContext, TFCommonUtil.GetSqlTimestamp(eventData));
        }
        else
          this.InvalidateCache(eventData);
      }));
    }

    internal void RefreshUpdatedTagDefinitions(IVssRequestContext requestContext, byte[] timestamp) => requestContext.TraceBlock(93381, 93382, "TeamFoundationTaggingService", "BusinessLogic", nameof (RefreshUpdatedTagDefinitions), (Action) (() =>
    {
      requestContext.Trace(15030001, TraceLevel.Info, "TeamFoundationTaggingService", "BusinessLogic", "RefreshUpdatedTagDefinitions called for account name: {0} with timestamp: {1} partitionid: {2} host id: {3}", (object) requestContext.ServiceHost.Name, (object) ArrayUtility.StringFromByteArray(timestamp), (object) requestContext.ServiceHost.PartitionId, (object) requestContext.ServiceHost.InstanceId);
      IEnumerable<TagDefinition> definitionsByTimestamp;
      using (TaggingComponent component = requestContext.CreateComponent<TaggingComponent>())
        definitionsByTimestamp = component.GetTagDefinitionsByTimestamp(timestamp);
      this.AddToCache(requestContext, definitionsByTimestamp);
      this.m_isCacheValid = true;
    }));

    private void InvalidateCache(string eventData)
    {
      byte[] timestamp = this.m_timestamp;
      byte[] sqlTimestamp = eventData == null ? (byte[]) null : TFCommonUtil.GetSqlTimestamp(eventData);
      ulong uint64_1 = timestamp == null ? 0UL : BitConverter.ToUInt64(((IEnumerable<byte>) timestamp).Reverse<byte>().ToArray<byte>(), 0);
      ulong uint64_2 = sqlTimestamp == null ? 0UL : BitConverter.ToUInt64(((IEnumerable<byte>) sqlTimestamp).Reverse<byte>().ToArray<byte>(), 0);
      if (this.m_isCacheValid || uint64_2 < uint64_1)
        this.m_timestamp = sqlTimestamp;
      this.m_isCacheValid = false;
    }

    private void EnsureCache(IVssRequestContext requestContext)
    {
      if (this.m_isCacheValid)
        return;
      byte[] timestamp = this.m_timestamp;
      if (timestamp == null)
        this.PopulateTagDefinitionsCache(requestContext);
      else
        this.RefreshUpdatedTagDefinitions(requestContext, timestamp);
      this.m_isCacheValid = true;
    }

    public (int TagsCount, int TagsLimit) GetTagCount(IVssRequestContext requestContext)
    {
      this.ValidateRequestContext(requestContext);
      return (requestContext.GetService<TeamFoundationTaggingCacheService>().GetAllTagCount(requestContext), this.GetTagsLimit(requestContext));
    }
  }
}
