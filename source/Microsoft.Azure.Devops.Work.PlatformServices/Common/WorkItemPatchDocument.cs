// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.PlatformServices.Common.WorkItemPatchDocument
// Assembly: Microsoft.Azure.Devops.Work.PlatformServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7C8E511A-CB9A-4327-9803-A1164853E0F0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Work.PlatformServices.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.Azure.Devops.Work.PlatformServices.Common
{
  public class WorkItemPatchDocument
  {
    internal static readonly IDictionary<string, ResourceLinkType> ResourceLinkTypeMapping = (IDictionary<string, ResourceLinkType>) new Dictionary<string, ResourceLinkType>((IEqualityComparer<string>) TFStringComparer.WorkItemLinkTypeReferenceName)
    {
      {
        "AttachedFile",
        ResourceLinkType.Attachment
      },
      {
        "Hyperlink",
        ResourceLinkType.Hyperlink
      },
      {
        "ArtifactLink",
        ResourceLinkType.ArtifactLink
      }
    };
    internal static readonly Version MaximumSupportedLegacyIndexHandlingVersion = VssRestApiVersion.v3_2.ToVersion();
    private const string FieldsPath = "fields";
    private const string RelationsPath = "relations";
    private const string IdPath = "id";
    private const string RevPath = "rev";
    private readonly string CommentUpdateFormatField = "Format".ToLower();
    private readonly string CommentUpdateTextField = "Text".ToLower();
    private WorkItemTrackingRequestContext witRequestContext;
    private bool useLegacyLinkHandling;
    private Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem;
    private Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem;
    private IPatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document;
    private List<KeyValuePair<string, object>> fieldUpdates = new List<KeyValuePair<string, object>>();
    private List<WorkItemLinkUpdate> workItemLinkUpdates;
    private List<WorkItemResourceLinkUpdate> resourceLinksUpdates;

    public WorkItemPatchDocument(
      WorkItemTrackingRequestContext witRequestContext,
      bool useLegacyLinkHandling,
      IPatchDocument<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> document,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem serverWorkItem,
      bool useIdentityRef = false,
      bool returnProjectScopedUrl = true,
      bool excludeRemoteLinkProperties = true)
    {
      this.witRequestContext = witRequestContext;
      this.useLegacyLinkHandling = useLegacyLinkHandling;
      this.document = document;
      this.serverWorkItem = serverWorkItem;
      this.workItem = WorkItemFactory.Create(witRequestContext, (WorkItemRevision) serverWorkItem, true, true, true, false, false, (IEnumerable<string>) null, false, false, (IDictionary<Guid, IdentityReference>) null, useIdentityRef, returnProjectScopedUrl, excludeRemoteLinkProperties);
      if (!this.workItem.Id.HasValue)
        this.workItem.Id = new int?(-1);
      if (this.workItem.Relations == null)
        this.workItem.Relations = (IList<WorkItemRelation>) new List<WorkItemRelation>();
      this.fieldUpdates = new List<KeyValuePair<string, object>>();
      this.workItemLinkUpdates = new List<WorkItemLinkUpdate>();
      this.resourceLinksUpdates = new List<WorkItemResourceLinkUpdate>();
    }

    public void Evaluate()
    {
      foreach (IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation in this.document.Operations)
        this.EvaluateOperation(operation.EvaluatedPath, operation);
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate GetWorkItemUpdate(
      WorkItemTrackingRequestContext witRequestContext,
      bool useWorkItemIdentity = false)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate workItemUpdate = new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemUpdate();
      int? nullable = this.workItem.Id;
      workItemUpdate.Id = nullable ?? -1;
      nullable = this.workItem.Rev;
      workItemUpdate.Rev = nullable.GetValueOrDefault();
      workItemUpdate.FieldData = (WorkItemFieldData) this.serverWorkItem;
      workItemUpdate.Fields = (IEnumerable<KeyValuePair<string, object>>) this.GetFieldUpdates(witRequestContext, useWorkItemIdentity);
      workItemUpdate.LinkUpdates = (IEnumerable<WorkItemLinkUpdate>) this.workItemLinkUpdates;
      workItemUpdate.ResourceLinkUpdates = (IEnumerable<WorkItemResourceLinkUpdate>) this.resourceLinksUpdates;
      return workItemUpdate;
    }

    private List<KeyValuePair<string, object>> GetFieldUpdates(
      WorkItemTrackingRequestContext witRequestContext,
      bool useWorkItemIdentity = false)
    {
      IFieldTypeDictionary fieldDictionary = witRequestContext.FieldDictionary;
      ITreeDictionary treeService = witRequestContext.TreeService;
      return this.fieldUpdates.Select<KeyValuePair<string, object>, KeyValuePair<string, object>>((Func<KeyValuePair<string, object>, KeyValuePair<string, object>>) (kvp =>
      {
        FieldEntry fieldByNameOrId = fieldDictionary.GetFieldByNameOrId(kvp.Key);
        KeyValuePair<string, object> fieldUpdates;
        switch (fieldByNameOrId.FieldId)
        {
          case -104:
          case -2:
            int nodeId = Convert.ToInt32(kvp.Value);
            TreeNode treeNode = treeService.LegacyGetTreeNode(nodeId);
            if (treeNode.IsStructureSpecifier)
              nodeId = treeNode.Parent.Id;
            fieldUpdates = new KeyValuePair<string, object>(kvp.Key, (object) nodeId);
            break;
          default:
            if (fieldByNameOrId.IsIdentity && kvp.Value is Dictionary<string, object>)
            {
              if (!useWorkItemIdentity)
                throw new IdentityRefNotAcceptedException();
              fieldUpdates = new KeyValuePair<string, object>(kvp.Key, (object) new WorkItemIdentity()
              {
                IdentityRef = this.ParseToIdentityRef((IDictionary<string, object>) kvp.Value)
              });
              break;
            }
            fieldUpdates = kvp;
            break;
        }
        return fieldUpdates;
      })).ToList<KeyValuePair<string, object>>();
    }

    private void EvaluateOperation(
      IEnumerable<string> evaluatedPath,
      IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation)
    {
      string[] array = evaluatedPath.ToArray<string>();
      int length = array.Length;
      if (operation.Operation == Operation.Copy || operation.Operation == Operation.Move)
        throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_OperationNotSuppported((object) operation.Operation));
      if (length == 1)
        throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDoesNotSupportEmptyPath());
      if (length == 2)
      {
        this.HandleTopLevelOperation(array, operation);
      }
      else
      {
        switch (array[1])
        {
          case "fields":
            this.HandleFieldOperation(array, operation);
            break;
          case "relations":
            this.HandleRelationOperation(array, operation);
            break;
        }
      }
    }

    private void HandleTopLevelOperation(string[] path, IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation)
    {
      switch (operation.Operation)
      {
        case Operation.Add:
        case Operation.Replace:
          if (!(path[1] == "id"))
            throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDoesNotSupportPatchingTopLevelProperties((object) operation.Path));
          int num1 = (int) operation.Value;
          int? id = this.workItem.Id;
          int num2 = 0;
          if (id.GetValueOrDefault() > num2 & id.HasValue || num1 > 0)
            throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDoesNotSupportPatchingTopLevelProperties((object) operation.Path));
          this.workItem.Id = new int?(num1);
          break;
        case Operation.Test:
          switch (path[1])
          {
            case "id":
              if (this.workItem.Id.Equals(operation.Value))
                return;
              throw new TestPatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_TestFailed((object) operation.Path, (object) this.workItem.Id, operation.Value));
            case "rev":
              if (this.serverWorkItem.Revision.Equals(operation.Value))
                return;
              throw new TestPatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_TestFailed((object) operation.Path, (object) this.serverWorkItem.Revision, operation.Value));
            default:
              throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDoesNotSupportPatchingTopLevelProperties((object) operation.Path));
          }
        default:
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_OperationNotSuppported((object) operation.Operation));
      }
    }

    private void HandleFieldOperation(string[] path, IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation)
    {
      switch (operation.Operation)
      {
        case Operation.Add:
          switch (path[2])
          {
            case "System.Tags":
              string str1 = this.serverWorkItem.GetFieldValue(this.witRequestContext, "System.Tags")?.ToString();
              string str2;
              if (!string.IsNullOrEmpty(str1))
                str2 = string.Join("; ", (object) str1, operation.Value);
              else
                str2 = operation.Value?.ToString();
              string str3 = str2;
              this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], (object) str3));
              return;
            case "System.History":
              object obj;
              if (operation.Value is IDictionary<string, object> dictionary && dictionary.TryGetValue(this.CommentUpdateTextField, out obj))
              {
                object format;
                if (dictionary.TryGetValue(this.CommentUpdateFormatField, out format))
                {
                  this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], (object) new WorkItemCommentUpdate(format as string, obj as string)));
                  return;
                }
                this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], (object) new WorkItemCommentUpdate(obj as string)));
                return;
              }
              this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], operation.Value));
              return;
            default:
              this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], operation.Value));
              return;
          }
        case Operation.Remove:
          this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], (object) null));
          break;
        case Operation.Replace:
          this.fieldUpdates.Add(new KeyValuePair<string, object>(path[2], operation.Value));
          break;
        case Operation.Test:
          object fieldValue1;
          if (!this.workItem.Fields.TryGetValue(path[2], out fieldValue1))
            fieldValue1 = (object) null;
          if (this.AreFieldValuesEqual(path[2], fieldValue1, operation.Value))
            break;
          throw new TestPatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_TestFailed((object) operation.Path, fieldValue1, operation.Value));
      }
    }

    private bool AreFieldValuesEqual(string fieldRefName, object fieldValue1, object fieldValue2)
    {
      bool isIdentity = this.witRequestContext.FieldDictionary.GetField(fieldRefName).IsIdentity;
      return StringComparer.Ordinal.Equals(this.GetFieldValueStr(fieldValue1, isIdentity), this.GetFieldValueStr(fieldValue2, isIdentity));
    }

    private string GetFieldValueStr(object fieldValue, bool isIdentityField)
    {
      if (fieldValue == null)
        return "";
      if (fieldValue is IdentityRef)
        return CommonUtils.DistinctDisplayName((IdentityRef) fieldValue);
      if (isIdentityField && fieldValue is Dictionary<string, object>)
      {
        string str1;
        ((Dictionary<string, object>) fieldValue).TryGetValue<string, string>("displayName", out str1);
        string str2;
        ((Dictionary<string, object>) fieldValue).TryGetValue<string, string>("uniqueName", out str2);
        return CommonUtils.DistinctDisplayName(new IdentityRef()
        {
          UniqueName = str2,
          DisplayName = str1
        });
      }
      return fieldValue.GetType() == typeof (long) ? ((long) fieldValue).ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) : CommonWITUtils.ConvertToStringForRuleCheck(fieldValue);
    }

    private void HandleRelationOperation(string[] path, IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation)
    {
      WorkItemRelation relation = (WorkItemRelation) null;
      int result;
      if (int.TryParse(path[2], out result))
      {
        IList<WorkItemRelation> relations = this.workItem.Relations;
        if ((relations != null ? (relations.Count <= result ? 1 : 0) : 0) != 0 || result < 0)
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_IndexOutOfRange((object) operation.Path));
        relation = this.workItem.Relations.ElementAtOrDefault<WorkItemRelation>(result);
      }
      else if (operation.Operation == Operation.Remove)
      {
        int id = -1;
        Guid remoteHostGuid = Guid.Empty;
        string linkType = (path.Length != 4 || int.TryParse(path[3], out id)) && (path.Length != 5 || Guid.TryParse(path[3], out remoteHostGuid) && int.TryParse(path[4], out id)) ? path[2] : throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_InvalidRelationPath((object) operation.Path, (object) "Remove"));
        if (WorkItemPatchDocument.ResourceLinkTypeMapping.ContainsKey(linkType))
        {
          WorkItemRelation workItemRelation = this.workItem.Relations.Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (rel => rel.Attributes.ContainsKey("id") && (int) rel.Attributes["id"] == id)).FirstOrDefault<WorkItemRelation>();
          relation = TFStringComparer.WorkItemLinkTypeReferenceName.Equals(workItemRelation?.Rel, linkType) ? workItemRelation : (WorkItemRelation) null;
        }
        else
        {
          if (!this.witRequestContext.LinkService.TryGetLinkTypeEndByReferenceName(this.witRequestContext.RequestContext, linkType, out WorkItemLinkTypeEnd _))
            throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.InvalidRelationType((object) linkType));
          (int targetId, Guid? remoteHostId) target1;
          IEnumerable<WorkItemRelation> source = (this.workItem.Relations.Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (rel => TFStringComparer.WorkItemLinkTypeReferenceName.Equals(rel.Rel, linkType))) ?? Enumerable.Empty<WorkItemRelation>()).Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (link => link.TryGetTarget(out target1) && target1.targetId == id));
          if (source.Count<WorkItemRelation>() > 1)
          {
            if (path.Length != 5)
              throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_AmbiguousRelation((object) this.workItem.Id, (object) linkType, (object) id));
            (int targetId, Guid? remoteHostId) target2;
            relation = source.Where<WorkItemRelation>((Func<WorkItemRelation, bool>) (link => link.TryGetTarget(out target2) && target2.remoteHostId.HasValue && target2.remoteHostId.Value == remoteHostGuid)).FirstOrDefault<WorkItemRelation>();
          }
          else
            relation = source.FirstOrDefault<WorkItemRelation>();
        }
        if (relation == null)
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_InvalidRelationForAGivenWorkItem());
      }
      else
      {
        if (operation.Operation != Operation.Add)
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_IndexOutOfRange((object) operation.Path));
        result = -1;
      }
      if (path.Length == 3)
      {
        switch (operation.Operation)
        {
          case Operation.Add:
          case Operation.Replace:
            this.HandleRelationOperation((WorkItemRelation) operation.Value, operation.Operation);
            break;
          case Operation.Remove:
            if (this.useLegacyLinkHandling)
              this.workItem.Relations.RemoveAt(result);
            this.HandleRelationOperation(relation, operation.Operation);
            break;
          case Operation.Test:
            throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_TestNotSupportedForRelations());
        }
      }
      else if ((path.Length == 4 || path.Length == 5) && operation.Operation == Operation.Remove)
      {
        if (relation == null)
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_InvalidRelationPath((object) operation.Path, (object) "Remove"));
        this.HandleRelationOperation(relation, Operation.Remove);
      }
      else
      {
        this.GetPatchOperation(operation).Apply(this.workItem);
        this.HandleRelationOperation(this.workItem.Relations[result], operation.Operation);
      }
    }

    private void HandleRelationOperation(WorkItemRelation relation, Operation operation)
    {
      ArgumentUtility.CheckForNull<WorkItemRelation>(relation, nameof (relation));
      LinkUpdateType? nullable1 = new LinkUpdateType?();
      switch (operation)
      {
        case Operation.Add:
          nullable1 = new LinkUpdateType?(LinkUpdateType.Add);
          break;
        case Operation.Remove:
          nullable1 = new LinkUpdateType?(LinkUpdateType.Delete);
          break;
        case Operation.Replace:
          nullable1 = new LinkUpdateType?(LinkUpdateType.Update);
          break;
      }
      if (!nullable1.HasValue)
        return;
      WorkItemLinkTypeEnd linkTypeEnd;
      bool workItemLink = this.TryParseWorkItemLink(relation, out linkTypeEnd);
      LinkUpdateType? nullable2 = nullable1;
      LinkUpdateType linkUpdateType1 = LinkUpdateType.Update;
      if (nullable2.GetValueOrDefault() == linkUpdateType1 & nullable2.HasValue && !this.workItem.Relations.Contains<WorkItemRelation>(relation, workItemLink ? WorkItemLinkComparer.Instance : WorkItemResourceLinkComparer.Instance))
      {
        if (relation.Rel == "AttachedFile")
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDoesNotSupportReplacingAttachedFile());
        throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.WorkItemPatchDocument_CannotChangeRelationType());
      }
      if (workItemLink)
      {
        Guid empty = Guid.Empty;
        (int targetId, Guid? remoteHostId) relationTarget = relation.GetTarget();
        WorkItemLinkUpdate workItemLinkUpdate1 = this.workItemLinkUpdates.FirstOrDefault<WorkItemLinkUpdate>((Func<WorkItemLinkUpdate, bool>) (l =>
        {
          if (l.LinkType == linkTypeEnd.Id)
          {
            int sourceWorkItemId = l.SourceWorkItemId;
            int? id = this.workItem.Id;
            int valueOrDefault = id.GetValueOrDefault();
            if (sourceWorkItemId == valueOrDefault & id.HasValue && l.TargetWorkItemId == relationTarget.targetId)
            {
              Guid? remoteHostId1 = l.RemoteHostId;
              Guid? remoteHostId2 = relationTarget.remoteHostId;
              if ((remoteHostId1.HasValue == remoteHostId2.HasValue ? (remoteHostId1.HasValue ? (remoteHostId1.GetValueOrDefault() == remoteHostId2.GetValueOrDefault() ? 1 : 0) : 1) : 0) != 0)
                return l.UpdateType == LinkUpdateType.Delete;
            }
          }
          return false;
        }));
        LinkUpdateType? nullable3 = nullable1;
        LinkUpdateType linkUpdateType2 = LinkUpdateType.Add;
        if (nullable3.GetValueOrDefault() == linkUpdateType2 & nullable3.HasValue && this.workItem.Relations.Contains<WorkItemRelation>(relation, WorkItemLinkComparer.Instance))
        {
          if (workItemLinkUpdate1 != null)
            this.workItemLinkUpdates.Remove(workItemLinkUpdate1);
          nullable1 = new LinkUpdateType?(LinkUpdateType.Update);
        }
        WorkItemLinkUpdate workItemLinkUpdate = relation.ToWorkItemLinkUpdate(this.witRequestContext.RequestContext, nullable1.Value, this.workItem.Id.Value, linkTypeEnd);
        WorkItemLinkInfo workItemLinkInfo = this.serverWorkItem.WorkItemLinks.Where<WorkItemLinkInfo>((Func<WorkItemLinkInfo, bool>) (x =>
        {
          if (x.SourceId != workItemLinkUpdate.SourceWorkItemId || x.TargetId != workItemLinkUpdate.TargetWorkItemId)
            return false;
          Guid? remoteHostId3 = x.RemoteHostId;
          Guid? remoteHostId4 = workItemLinkUpdate.RemoteHostId;
          if (remoteHostId3.HasValue != remoteHostId4.HasValue)
            return false;
          return !remoteHostId3.HasValue || remoteHostId3.GetValueOrDefault() == remoteHostId4.GetValueOrDefault();
        })).FirstOrDefault<WorkItemLinkInfo>();
        workItemLinkUpdate.RemoteStatusMessage = workItemLinkInfo?.RemoteStatusMessage;
        workItemLinkUpdate.RemoteWatermark = (long?) workItemLinkInfo?.RemoteWatermark;
        workItemLinkUpdate.RemoteProjectId = (Guid?) workItemLinkInfo?.RemoteProjectId;
        workItemLinkUpdate.TimeStamp = workItemLinkInfo != null ? workItemLinkInfo.TimeStamp : 0L;
        this.workItemLinkUpdates.Add(workItemLinkUpdate);
      }
      else
      {
        ResourceLinkType resourceLinkType;
        if (!this.TryParseResourceLinkType(relation, out resourceLinkType))
          throw new PatchOperationFailedException(Microsoft.Azure.Devops.Work.PlatformServices.ResourceStrings.InvalidRelationType((object) relation.Rel));
        int resourceId = relation.Attributes != null ? Convert.ToInt32(relation.Attributes.GetValueOrDefault<string, object>("id", (object) 0)) : 0;
        WorkItemResourceLinkUpdate resourceLinkUpdate = this.resourceLinksUpdates.FirstOrDefault<WorkItemResourceLinkUpdate>((Func<WorkItemResourceLinkUpdate, bool>) (l =>
        {
          int? resourceId1 = l.ResourceId;
          int num = resourceId;
          return resourceId1.GetValueOrDefault() == num & resourceId1.HasValue && l.UpdateType == LinkUpdateType.Delete;
        }));
        LinkUpdateType? nullable4 = nullable1;
        LinkUpdateType linkUpdateType3 = LinkUpdateType.Add;
        if (nullable4.GetValueOrDefault() == linkUpdateType3 & nullable4.HasValue && resourceId > 0 && this.workItem.Relations.Contains<WorkItemRelation>(relation, WorkItemResourceLinkComparer.Instance))
        {
          if (resourceLinkUpdate != null)
            this.resourceLinksUpdates.Remove(resourceLinkUpdate);
          nullable1 = new LinkUpdateType?(LinkUpdateType.Update);
        }
        this.resourceLinksUpdates.Add(relation.ToWorkItemResourceLinkUpdate(resourceLinkType, nullable1.Value, new int?(resourceId)));
      }
    }

    private bool TryParseWorkItemLink(
      WorkItemRelation relation,
      out WorkItemLinkTypeEnd linkTypeEnd)
    {
      if (relation.Rel != null)
        return this.witRequestContext.LinkService.TryGetLinkTypeEndByReferenceName(this.witRequestContext.RequestContext, relation.Rel, out linkTypeEnd);
      linkTypeEnd = (WorkItemLinkTypeEnd) null;
      return false;
    }

    private bool TryParseResourceLinkType(
      WorkItemRelation relation,
      out ResourceLinkType resourceLinkType)
    {
      return WorkItemPatchDocument.ResourceLinkTypeMapping.TryGetValue(relation.Rel, out resourceLinkType);
    }

    private PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> GetPatchOperation(
      IPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> operation)
    {
      PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem> patchOperation = (PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) null;
      switch (operation.Operation)
      {
        case Operation.Add:
          return (PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) new AddPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(operation.Path, operation.Value);
        case Operation.Remove:
          return (PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) new RemovePatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(operation.Path);
        case Operation.Replace:
          return (PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) new ReplacePatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(operation.Path, operation.Value);
        case Operation.Test:
          return (PatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>) new TestPatchOperation<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem>(operation.Path, operation.Value);
        default:
          return patchOperation;
      }
    }

    private IdentityRef ParseToIdentityRef(IDictionary<string, object> instance)
    {
      ArgumentUtility.CheckForNull<IDictionary<string, object>>(instance, nameof (instance));
      string str;
      instance.TryGetValue<string>("displayName", out str);
      string subjectDescriptorString;
      instance.TryGetValue<string>("descriptor", out subjectDescriptorString);
      return new IdentityRef()
      {
        Descriptor = SubjectDescriptor.FromString(subjectDescriptorString),
        DisplayName = str
      };
    }
  }
}
