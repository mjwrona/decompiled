// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.WorkItemModelFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Devops.Tags.Server;
using Microsoft.Azure.Devops.Tags.Server.Models;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Internals;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Identity;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.RemoteWorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public class WorkItemModelFactory : WorkItemModel
  {
    private const string c_constantRecordsById = "WebAccess/WorkItemTracking/ConstantRecordsById";
    private const string c_constantRecordsByDisplayName = "WebAccess/WorkItemTracking/ConstantRecordsByDisplayName";
    private const string c_revisedBy = "Revised By";
    private const string c_changedBy = "Changed By";

    private WorkItemModelFactory(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public static WorkItemModel Create(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItem workItem,
      DateTime loadTime,
      ISecuredObject securedObject)
    {
      using (witRequestContext.RequestContext.TraceBlock(290062, 290063, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, "WorkItemModel.Factory"))
      {
        WorkItemModelFactory itemModelFactory = new WorkItemModelFactory(securedObject);
        itemModelFactory.Id = workItem.Id;
        itemModelFactory.Revision = workItem.Revision;
        itemModelFactory.LoadTime = loadTime;
        itemModelFactory.ProjectId = workItem.GetProjectGuid(witRequestContext);
        IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
        Dictionary<int, object> dictionary1 = workItem.LatestData.ToDictionary<KeyValuePair<int, object>, int, object>((Func<KeyValuePair<int, object>, int>) (kvp => kvp.Key), (Func<KeyValuePair<int, object>, object>) (kvp => ConversionHelper.ConvertPotentialWitIdentityRef(kvp.Value)));
        dictionary1[-12] = (object) workItem.GetFieldValue<string>(witRequestContext, -12, true);
        dictionary1[-42] = (object) workItem.GetFieldValue<string>(witRequestContext, -42, true);
        dictionary1[-7] = (object) workItem.GetFieldValue<string>(witRequestContext, -7, true);
        dictionary1[-105] = (object) workItem.GetFieldValue<string>(witRequestContext, -105, true);
        itemModelFactory.LatestData = (IReadOnlyDictionary<int, object>) new ReadOnlyDictionary<int, object>((IDictionary<int, object>) dictionary1);
        Dictionary<int, WorkItemCommentVersionRecord> commentVersions = new Dictionary<int, WorkItemCommentVersionRecord>();
        Dictionary<(int, int), WorkItemCommentVersionRecord> dictionary2 = new Dictionary<(int, int), WorkItemCommentVersionRecord>();
        if (workItem.CommentVersion != null)
        {
          WorkItemCommentVersionRecord commentVersion = workItem.CommentVersion;
          commentVersions.Add(workItem.Revision, commentVersion);
          dictionary2.Add((commentVersion.CommentId, commentVersion.Version), commentVersion);
        }
        List<IReadOnlyDictionary<int, object>> readOnlyDictionaryList = new List<IReadOnlyDictionary<int, object>>();
        IReadOnlyList<WorkItemRevision> revisions = workItem.Revisions;
        WorkItemRevision workItemRevision1 = (WorkItemRevision) workItem;
        for (int index = revisions.Count - 1; index >= 0; --index)
        {
          Dictionary<int, object> dictionary3 = new Dictionary<int, object>();
          WorkItemRevision workItemRevision2 = revisions[index];
          foreach (KeyValuePair<int, object> keyValuePair in workItemRevision1.LatestData)
          {
            object objA;
            if (workItemRevision2.LatestData.TryGetValue(keyValuePair.Key, out objA))
            {
              object objB = keyValuePair.Value;
              if (!object.Equals(objA, objB) || keyValuePair.Key == 54 || keyValuePair.Key == -42 || keyValuePair.Key == -7 || keyValuePair.Key == -105)
                dictionary3[keyValuePair.Key] = ConversionHelper.ConvertPotentialWitIdentityRef(objA);
            }
            else
              dictionary3[keyValuePair.Key] = (object) null;
          }
          foreach (KeyValuePair<int, object> keyValuePair in workItemRevision2.LatestData)
          {
            if (!workItemRevision1.LatestData.ContainsKey(keyValuePair.Key))
            {
              object obj = keyValuePair.Value;
              dictionary3[keyValuePair.Key] = obj;
            }
          }
          readOnlyDictionaryList.Add((IReadOnlyDictionary<int, object>) new ReadOnlyDictionary<int, object>((IDictionary<int, object>) dictionary3));
          if (workItemRevision2.CommentVersion != null)
          {
            WorkItemCommentVersionRecord commentVersion = workItemRevision2.CommentVersion;
            commentVersions.Add(workItemRevision2.Revision, commentVersion);
            dictionary2.Add((commentVersion.CommentId, commentVersion.Version), commentVersion);
          }
          workItemRevision1 = workItemRevision2;
        }
        foreach (WorkItemCommentVersionRecord commentVersionRecord1 in dictionary2.Values)
        {
          if (commentVersionRecord1.Version > 1 && !commentVersionRecord1.IsDeleted)
          {
            (int, int) key = (commentVersionRecord1.CommentId, commentVersionRecord1.Version - 1);
            WorkItemCommentVersionRecord commentVersionRecord2;
            if (dictionary2.TryGetValue(key, out commentVersionRecord2))
            {
              commentVersionRecord1.OriginalFormat = commentVersionRecord2.Format;
              commentVersionRecord1.OriginalText = commentVersionRecord2.Text;
              commentVersionRecord1.OriginalRenderedText = commentVersionRecord2.RenderedText;
            }
          }
        }
        readOnlyDictionaryList.Reverse();
        itemModelFactory.RevisionData = (IReadOnlyCollection<IReadOnlyDictionary<int, object>>) readOnlyDictionaryList.AsReadOnly();
        HashSet<WorkItemResourceLinkInfo> source1 = new HashSet<WorkItemResourceLinkInfo>((IEnumerable<WorkItemResourceLinkInfo>) workItem.ResourceLinks);
        HashSet<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> source2 = new HashSet<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) workItem.AllLinks);
        foreach (WorkItemRevision revision in (IEnumerable<WorkItemRevision>) workItem.Revisions)
        {
          source1.UnionWith((IEnumerable<WorkItemResourceLinkInfo>) revision.ResourceLinks);
          source2.UnionWith((IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) revision.AllLinks);
        }
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo workItemLink in workItem.WorkItemLinks)
          source2.Remove(workItemLink);
        IOrderedEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> orderedEnumerable = workItem.WorkItemLinks.OrderBy<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>) (wl => wl.RevisedDate));
        itemModelFactory.ResolveRemoteHostUrls(witRequestContext.RequestContext, (IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>) orderedEnumerable);
        itemModelFactory.Relations = orderedEnumerable.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, SecuredDictionary<string, object>>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, SecuredDictionary<string, object>>) (wl => WorkItemModelFactory.ToSecuredDict(witRequestContext.RequestContext, wl, securedObject)));
        itemModelFactory.RelationRevisions = source2.OrderBy<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, DateTime>) (wl => wl.RevisedDate)).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, SecuredDictionary<string, object>>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, SecuredDictionary<string, object>>) (wl => WorkItemModelFactory.ToSecuredDict(witRequestContext.RequestContext, wl, securedObject)));
        itemModelFactory.Files = (IEnumerable<SecuredDictionary<string, object>>) source1.OrderBy<WorkItemResourceLinkInfo, DateTime>((Func<WorkItemResourceLinkInfo, DateTime>) (rl => rl.RevisedDate)).Select<WorkItemResourceLinkInfo, SecuredDictionary<string, object>>((Func<WorkItemResourceLinkInfo, SecuredDictionary<string, object>>) (rl => WorkItemModelFactory.ToSecuredDict(rl, securedObject))).ToList<SecuredDictionary<string, object>>();
        if (WorkItemTrackingFeatureFlags.IsDisabledLinksOfDeletedAttachments(witRequestContext.RequestContext))
        {
          foreach (SecuredDictionary<string, object> file in itemModelFactory.Files)
          {
            if (file.ContainsKey("RemovedDate") && (int) file["FldID"] == 50 && file.ContainsKey("FilePath") && ((DateTime) file["RemovedDate"]).Ticks < new DateTime(9999, 1, 1, 0, 0, 0).Ticks)
              file["FilePath"] = (object) null;
          }
        }
        itemModelFactory.CurrentExtensions = (IEnumerable<Guid>) ((IEnumerable<WorkItemTypeExtension>) workItem.GetApplicableExtensions(witRequestContext)).Select<WorkItemTypeExtension, Guid>((Func<WorkItemTypeExtension, Guid>) (e => e.Id)).ToArray<Guid>();
        TagDefinition[] source3 = Array.Empty<TagDefinition>();
        itemModelFactory.Tags = ((IEnumerable<TagDefinition>) source3).Select<TagDefinition, TagDefinitionModel>((Func<TagDefinition, TagDefinitionModel>) (tag => new TagDefinitionModel(tag)));
        itemModelFactory.CalculateReferencedNodes(witRequestContext, workItem, securedObject);
        itemModelFactory.InitializeExtendedProperties(witRequestContext.RequestContext, securedObject);
        PermissionCheckHelper permissionCheckHelper = new PermissionCheckHelper(witRequestContext.RequestContext);
        itemModelFactory.IsReadOnly = !permissionCheckHelper.HasWorkItemPermission(workItem.AreaId, 32);
        itemModelFactory.isCommentingAvailable = permissionCheckHelper.HasWorkItemPermission(workItem.AreaId, 512, 32);
        if (securedObject != null)
        {
          itemModelFactory.Fields = itemModelFactory.LatestData.Where<KeyValuePair<int, object>>((Func<KeyValuePair<int, object>, bool>) (pair => pair.Value != null)).ToDictionary<KeyValuePair<int, object>, string, object>((Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, object>, object>) (pair => pair.Value));
          itemModelFactory.Revisions = itemModelFactory.RevisionData.Select<IReadOnlyDictionary<int, object>, Dictionary<string, object>>((Func<IReadOnlyDictionary<int, object>, Dictionary<string, object>>) (dict => dict.ToDictionary<KeyValuePair<int, object>, string, object>((Func<KeyValuePair<int, object>, string>) (pair => pair.Key.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo)), (Func<KeyValuePair<int, object>, object>) (pair => pair.Value))));
          itemModelFactory.CommentVersions = WorkItemCommentVersionModelFactory.Create(witRequestContext.RequestContext, commentVersions, securedObject);
        }
        return (WorkItemModel) itemModelFactory;
      }
    }

    private void ResolveRemoteHostUrls(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo> relations)
    {
      List<Guid> list = relations.Where<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, bool>) (x =>
      {
        if (!x.RemoteHostId.HasValue)
          return false;
        Guid? remoteHostId = x.RemoteHostId;
        Guid empty = Guid.Empty;
        if (!remoteHostId.HasValue)
          return true;
        return remoteHostId.HasValue && remoteHostId.GetValueOrDefault() != empty;
      })).Select<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo, Guid>) (x => x.RemoteHostId.Value)).ToList<Guid>();
      IDictionary<Guid, RemoteHostContext> remoteHostUrls = RemoteWorkItemHostResolver.GetRemoteHostUrls(requestContext, (IEnumerable<Guid>) list, true);
      if (!remoteHostUrls.Any<KeyValuePair<Guid, RemoteHostContext>>())
        return;
      requestContext.To(TeamFoundationHostType.Deployment).GetService<IUrlHostResolutionService>();
      foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo relation in relations)
      {
        Guid? remoteHostId = relation.RemoteHostId;
        if (remoteHostId.HasValue)
        {
          remoteHostId = relation.RemoteHostId;
          if (remoteHostId.Value != Guid.Empty)
          {
            IDictionary<Guid, RemoteHostContext> dictionary1 = remoteHostUrls;
            remoteHostId = relation.RemoteHostId;
            Guid key1 = remoteHostId.Value;
            if (dictionary1.ContainsKey(key1))
            {
              IDictionary<Guid, RemoteHostContext> dictionary2 = remoteHostUrls;
              remoteHostId = relation.RemoteHostId;
              Guid key2 = remoteHostId.Value;
              RemoteHostContext remoteHostContext = dictionary2[key2];
              relation.RemoteHostUrl = remoteHostContext.Url;
              relation.RemoteHostName = remoteHostContext.Name;
            }
          }
        }
      }
    }

    private static RemoteStatusMessage ParseRemoteStatusMessageWithDefault(
      IVssRequestContext requestContext,
      string remoteStatusMessageText)
    {
      if (requestContext == null || remoteStatusMessageText == null)
        return (RemoteStatusMessage) null;
      RemoteStatusMessage messageWithDefault;
      if (!JsonUtilities.TryDeserialize<RemoteStatusMessage>(remoteStatusMessageText, out messageWithDefault))
        requestContext.Trace(1300578, TraceLevel.Error, nameof (WorkItemModelFactory), nameof (ParseRemoteStatusMessageWithDefault), "Unable to deserialize remote status message: '" + remoteStatusMessageText + "'");
      return messageWithDefault;
    }

    private void CalculateReferencedNodes(
      WorkItemTrackingRequestContext witRequestContext,
      WorkItem workItem,
      ISecuredObject securedObject)
    {
      ITreeDictionary treeService = witRequestContext.TreeService;
      IVssRequestContext requestContext = witRequestContext.RequestContext;
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode1 = treeService.GetTreeNode(this.ProjectId, workItem.AreaId);
      ExtendedTreeNode extendedTreeNode1 = ExtendedTreeNode.Create(requestContext, treeNode1.GetPath(requestContext), treeNode1, securedObject);
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.TreeNode treeNode2 = treeService.GetTreeNode(this.ProjectId, workItem.IterationId);
      ExtendedTreeNode extendedTreeNode2 = ExtendedTreeNode.Create(requestContext, treeNode2.GetPath(requestContext), treeNode2, securedObject);
      this.ReferencedNodes = new ReferencedNodes((IEnumerable<ExtendedTreeNode>) new List<ExtendedTreeNode>()
      {
        extendedTreeNode1
      }, (IEnumerable<ExtendedTreeNode>) new List<ExtendedTreeNode>()
      {
        extendedTreeNode2
      }, securedObject);
    }

    private void InitializeExtendedProperties(
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      HashSet<int> source = new HashSet<int>();
      object obj1;
      foreach (IReadOnlyDictionary<int, object> readOnlyDictionary in this.RevisionData.Concat<IReadOnlyDictionary<int, object>>(Enumerable.Repeat<IReadOnlyDictionary<int, object>>(this.LatestData, 1)))
      {
        if (readOnlyDictionary.TryGetValue(-6, out obj1))
          source.Add((int) obj1);
      }
      foreach (SecuredDictionary<string, object> securedDictionary in this.Relations.Concat<SecuredDictionary<string, object>>(this.RelationRevisions))
      {
        if (securedDictionary.TryGetValue("Revised By", out obj1))
          source.Add((int) obj1);
        if (securedDictionary.TryGetValue("Changed By", out obj1))
          source.Add((int) obj1);
      }
      source.Remove(-1);
      object obj2;
      IEnumerable<int> ints;
      Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord> constantsByIdMap;
      if (requestContext.Items.TryGetValue("WebAccess/WorkItemTracking/ConstantRecordsById", out obj2))
      {
        constantsByIdMap = (Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord>) obj2;
        ints = (IEnumerable<int>) source.Where<int>((Func<int, bool>) (id => !constantsByIdMap.ContainsKey(id))).ToArray<int>();
      }
      else
      {
        ints = (IEnumerable<int>) source;
        constantsByIdMap = new Dictionary<int, Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord>();
        requestContext.Items["WebAccess/WorkItemTracking/ConstantRecordsById"] = (object) constantsByIdMap;
      }
      if (ints.Any<int>())
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord in requestContext.GetService<ITeamFoundationWorkItemTrackingMetadataService>().GetConstantRecords(requestContext, ints, true))
          constantsByIdMap[constantRecord.Id] = constantRecord;
      }
      this.ReferencedPersons = source.ToDictionary<int, int, WitIdentityRef>((Func<int, int>) (id => id), (Func<int, WitIdentityRef>) (id =>
      {
        Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord;
        if (constantsByIdMap.TryGetValue(id, out constantRecord))
        {
          WorkItemIdentity distinctDisplayName = WorkItemIdentityHelper.GetResolvedIdentityFromDistinctDisplayName(requestContext, constantRecord.DisplayText);
          if (distinctDisplayName != null)
            return distinctDisplayName.ToWitIdentityRef();
          requestContext.Trace(290066, TraceLevel.Error, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemIdentityNotResovledById((object) id));
          return this.getUnknownIdentityRef(securedObject);
        }
        requestContext.Trace(290067, TraceLevel.Error, "WebAccess.WorkItem", TfsTraceLayers.BusinessLogic, Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.WorkItemIdentityNotResovledById((object) id));
        return this.getUnknownIdentityRef(securedObject);
      })).ToDictionary<KeyValuePair<int, WitIdentityRef>, int, WitIdentityRef>((Func<KeyValuePair<int, WitIdentityRef>, int>) (pair => pair.Key), (Func<KeyValuePair<int, WitIdentityRef>, WitIdentityRef>) (pair => pair.Value));
    }

    private WitIdentityRef getUnknownIdentityRef(ISecuredObject securedObject)
    {
      string str = InternalsResourceStrings.Get("UnknownUser");
      WitIdentityRef unknownIdentityRef = new WitIdentityRef(securedObject?.GetToken());
      unknownIdentityRef.DistinctDisplayName = str;
      ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
      constantIdentityRef.DisplayName = str;
      unknownIdentityRef.IdentityRef = (IdentityRef) constantIdentityRef;
      return unknownIdentityRef;
    }

    private WitIdentityRef getIdentityRefForNonIdentityConstant(
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DataModels.ConstantRecord constantRecord,
      ISecuredObject securedObject)
    {
      if (constantRecord.TeamFoundationId != Guid.Empty)
        throw new InvalidConstantIdentityException(constantRecord.TeamFoundationId);
      WitIdentityRef identityConstant = new WitIdentityRef(securedObject?.GetToken());
      identityConstant.DistinctDisplayName = constantRecord.DisplayText;
      ConstantIdentityRef constantIdentityRef = new ConstantIdentityRef(securedObject);
      constantIdentityRef.DisplayName = constantRecord.DisplayText;
      identityConstant.IdentityRef = (IdentityRef) constantIdentityRef;
      return identityConstant;
    }

    private static SecuredDictionary<string, object> ToSecuredDict(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkInfo wl,
      ISecuredObject securedObject)
    {
      SecuredDictionary<string, object> securedDict = new SecuredDictionary<string, object>(securedObject);
      securedDict["ID"] = (object) wl.TargetId;
      securedDict["LinkType"] = (object) wl.LinkType;
      securedDict["Changed Date"] = (object) wl.AuthorizedDate;
      securedDict["Revised Date"] = (object) wl.RevisedDate;
      securedDict["AuthorizedAddedDate"] = (object) wl.AuthorizedDate;
      securedDict["AuthorizedRemovedDate"] = (object) wl.RevisedDate;
      securedDict["Changed By"] = (object) wl.AuthorizedById;
      securedDict["Revised By"] = (object) wl.RevisedById;
      if (wl.RemoteHostId.HasValue && wl.RemoteHostId.Value != Guid.Empty)
      {
        securedDict["RemoteHostId"] = (object) wl.RemoteHostId;
        securedDict["RemoteProjectId"] = (object) wl.RemoteProjectId;
        securedDict["RemoteHostUrl"] = (object) wl.RemoteHostUrl;
        securedDict["RemoteStatus"] = (object) wl.RemoteStatus;
        securedDict["RemoteHostName"] = (object) wl.RemoteHostName;
        securedDict["RemoteStatusMessage"] = (object) WorkItemModelFactory.ParseRemoteStatusMessageWithDefault(requestContext, wl.RemoteStatusMessage)?.StatusMessage;
      }
      securedDict["Lock"] = (object) wl.IsLocked;
      securedDict["Comment"] = (object) wl.Comment;
      return securedDict;
    }

    private static SecuredDictionary<string, object> ToSecuredDict(
      WorkItemResourceLinkInfo rl,
      ISecuredObject securedObject)
    {
      SecuredDictionary<string, object> securedDict = new SecuredDictionary<string, object>(securedObject);
      securedDict["FldID"] = (object) (int) rl.ResourceType;
      securedDict["ExtID"] = (object) rl.ResourceId;
      securedDict["AddedDate"] = (object) rl.AuthorizedDate;
      securedDict["RemovedDate"] = (object) rl.RevisedDate;
      securedDict["AuthorizedAddedDate"] = (object) rl.AuthorizedDate;
      securedDict["AuthorizedRemovedDate"] = (object) rl.RevisedDate;
      securedDict["FilePath"] = (object) rl.Location;
      securedDict["OriginalName"] = (object) rl.Name;
      securedDict["Comment"] = (object) rl.Comment;
      securedDict["CreationDate"] = (object) rl.ResourceCreatedDate;
      securedDict["LastWriteDate"] = (object) rl.ResourceModifiedDate;
      securedDict["Length"] = (object) rl.ResourceSize;
      return securedDict;
    }
  }
}
