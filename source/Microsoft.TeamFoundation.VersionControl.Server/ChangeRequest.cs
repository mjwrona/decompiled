// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ChangeRequest
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ChangeRequest : ICloneable, IValidatable, IRecordable
  {
    private RequestType m_requestType;
    private int m_deletionId;
    private int m_encoding = -2;
    private ItemType m_itemType;
    private ItemSpec m_itemSpec;
    private LockLevel m_lockLevel = LockLevel.Unchanged;
    private string m_targetItem;
    private ItemType m_targetItemType;
    private VersionSpec m_versionSpec;
    private ExpandedChange m_expandedChange;
    private bool m_resultProcessed;
    private int m_foundItems;
    private string m_lastServerItem;
    private DeferredQuery m_deferredQuery;
    private string m_sourceRoot;
    private string m_targetRoot;
    internal DeletedState m_deletedState;

    [XmlAttribute("req")]
    [DefaultValue(RequestType.None)]
    public RequestType RequestType
    {
      get => this.m_requestType;
      set => this.m_requestType = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    [XmlAttribute("enc")]
    [DefaultValue(-2)]
    public int Encoding
    {
      get => this.m_encoding;
      set => this.m_encoding = value;
    }

    [XmlAttribute("type")]
    [DefaultValue(ItemType.Any)]
    public ItemType ItemType
    {
      get => this.m_itemType;
      set => this.m_itemType = value;
    }

    [XmlElement("item")]
    public ItemSpec ItemSpec
    {
      get => this.m_itemSpec;
      set => this.m_itemSpec = value;
    }

    [XmlAttribute("lock")]
    [DefaultValue(LockLevel.Unchanged)]
    public LockLevel LockLevel
    {
      get => this.m_lockLevel;
      set => this.m_lockLevel = value;
    }

    [XmlAttribute("target")]
    public string TargetItem
    {
      get => this.m_targetItem;
      set => this.m_targetItem = value;
    }

    [XmlAttribute("targettype")]
    [DefaultValue(ItemType.Any)]
    public ItemType TargetItemType
    {
      get => this.m_targetItemType;
      set => this.m_targetItemType = value;
    }

    [XmlElement("vspec")]
    public VersionSpec VersionSpec
    {
      get => this.m_versionSpec;
      set => this.m_versionSpec = value;
    }

    public PropertyValue[] Properties { get; set; }

    void IValidatable.Validate(
      VersionControlRequestContext versionControlRequestContext,
      string parameterName)
    {
      if (this.RequestType == RequestType.Add)
        this.m_itemSpec.SetValidationOptions(versionControlRequestContext.VersionControlService.GetAllow8Dot3Paths(versionControlRequestContext), true, false);
      versionControlRequestContext.Validation.check((IValidatable) this.m_itemSpec, "ItemSpec", false);
      switch (this.RequestType)
      {
        case RequestType.Add:
          if (this.Encoding == -2)
            throw new ArgumentException(Resources.Format("InvalidAddEncoding", (object) this.Encoding));
          if (this.ItemType == ItemType.Any)
            throw new ArgumentException(Resources.Format("InvalidAddEncoding", (object) ("ItemType." + this.ItemType.ToString())));
          break;
        case RequestType.Branch:
          if (this.ItemSpec.isWildcard)
            throw new WildcardNotAllowedException("WildcardNotAllowedForBranchSource", Array.Empty<object>());
          versionControlRequestContext.Validation.checkItem(ref this.m_targetItem, "TargetItem", false, true, versionControlRequestContext.VersionControlService.GetAllow8Dot3Paths(versionControlRequestContext), true, versionControlRequestContext.MaxSupportedServerPathLength);
          break;
        case RequestType.Lock:
          if (this.LockLevel == LockLevel.Unchanged)
            throw new ArgumentException(Resources.Format("InvalidLevelForLockItem"), parameterName);
          break;
        case RequestType.Rename:
          versionControlRequestContext.Validation.checkItem(ref this.m_targetItem, "TargetItem", false, true, versionControlRequestContext.VersionControlService.GetAllow8Dot3Paths(versionControlRequestContext), true, versionControlRequestContext.MaxSupportedServerPathLength);
          break;
        case RequestType.Undelete:
          versionControlRequestContext.Validation.checkItem(ref this.m_targetItem, "TargetItem", true, true, versionControlRequestContext.VersionControlService.GetAllow8Dot3Paths(versionControlRequestContext), true, versionControlRequestContext.MaxSupportedServerPathLength);
          break;
        default:
          ArgumentUtility.EnsureIsNull((object) this.TargetItem, "TargetItem");
          break;
      }
      versionControlRequestContext.Validation.checkLockLevel(this.m_lockLevel, "LockLevel");
      versionControlRequestContext.Validation.checkVersionSpec(this.m_versionSpec, "Version", true);
    }

    internal bool IsFolder => this.ItemType == ItemType.Folder;

    internal void Expand(
      VersionControlRequestContext versionControlRequestContext,
      VersionedItemComponent db,
      Workspace workspace,
      IList<DeferredQuery> deferredQueries,
      IList<Exception> exceptions,
      int requestIndex)
    {
      this.m_sourceRoot = (string) null;
      this.m_targetRoot = (string) null;
      if (this.RequestType == RequestType.Add)
      {
        ExpandedChange expandedChange = new ExpandedChange();
        expandedChange.requestIndex = requestIndex;
        if (!this.ItemSpec.isServerItem)
          expandedChange.localItem = this.ItemSpec.toLocalItem(versionControlRequestContext.RequestContext, workspace);
        expandedChange.itemPathPair = this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace);
        this.m_expandedChange = expandedChange;
      }
      else
      {
        if (this.TargetItem != null)
          this.ExpandTarget(versionControlRequestContext, workspace, db, this.RequestType == RequestType.Rename);
        this.m_deletedState = DeletedState.NonDeleted;
        GetItemsOptions options = GetItemsOptions.None;
        if (this.RequestType == RequestType.Undelete)
        {
          this.m_deletedState = DeletedState.Deleted;
          if (this.DeletionId != 0)
            this.ItemSpec.DeletionId = this.DeletionId;
          this.VersionSpec = (VersionSpec) new LatestVersionSpec();
          options |= GetItemsOptions.IncludeSourceRenames;
        }
        else if (this.RequestType != RequestType.Branch || this.VersionSpec == null)
        {
          this.VersionSpec = (VersionSpec) new WorkspaceVersionSpec(workspace);
          this.m_deletedState = DeletedState.Any;
        }
        if (this.ItemSpec.isServerItem && this.ItemSpec.RecursionType == RecursionType.None && this.RequestType != RequestType.Property)
        {
          this.m_expandedChange = new ExpandedChange()
          {
            requestIndex = requestIndex,
            itemPathPair = this.ItemSpec.ItemPathPair,
            targetItemPathPair = ItemPathPair.FromServerItem(this.m_targetRoot)
          };
        }
        else
        {
          if (this.RequestType == RequestType.Branch)
          {
            ItemPathPair itemPathPair = this.VersionSpec is WorkspaceVersionSpec ? this.ItemSpec.ItemPathPair : this.ItemSpec.toServerItemWithoutMappingRenames(versionControlRequestContext, workspace, false);
            this.m_deferredQuery = new DeferredQuery(versionControlRequestContext, new ItemSpec(itemPathPair, RecursionType.None, this.ItemSpec.DeletionId), this.m_deletedState, ItemType.Any, this.VersionSpec, false, (int) options);
          }
          else
          {
            bool isMapped = this.RequestType == RequestType.Lock && this.ItemSpec.isServerItem;
            this.m_deferredQuery = new DeferredQuery(versionControlRequestContext, this.ItemSpec, this.m_deletedState, ItemType.Any, this.VersionSpec, isMapped, (int) options);
          }
          deferredQueries.Add(this.m_deferredQuery);
        }
      }
    }

    public void RecordInformation(MethodInformation methodInformation, int paramIndex)
    {
      string str = "changes[" + (object) paramIndex + "].";
      methodInformation.AddParameter(str + "RequestType", (object) this.RequestType);
      methodInformation.AddParameter(str + "ItemSpec", (object) this.ItemSpec);
      methodInformation.AddParameter(str + "DeletionId", (object) this.DeletionId);
      methodInformation.AddParameter(str + "ItemType", (object) this.ItemType);
      methodInformation.AddParameter(str + "Encoding", (object) this.Encoding);
      if (this.LockLevel != LockLevel.Unchanged)
        methodInformation.AddParameter(str + "LockLevel", (object) this.LockLevel);
      if (this.TargetItem != null)
      {
        methodInformation.AddParameter(str + "TargetItem", (object) this.TargetItem);
        methodInformation.AddParameter(str + "TargetItemType", (object) this.TargetItemType);
      }
      if (this.VersionSpec == null)
        return;
      methodInformation.AddParameter(str + "VersionSpec", (object) this.VersionSpec);
    }

    internal bool GetNextExpandedChange(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      List<Exception> exceptions,
      VersionedItemComponent db,
      int requestIndex,
      out ExpandedChange change)
    {
      change = (ExpandedChange) null;
      if (!this.m_resultProcessed)
      {
        if (this.m_deferredQuery != null)
        {
          if (!this.m_deferredQuery.Executed)
          {
            this.m_deferredQuery.Execute(workspace, db);
            this.m_foundItems = 0;
          }
          if (this.m_deferredQuery.Exception == null)
          {
            Item obj;
            while (this.m_deferredQuery.TryGetNextItem(out obj))
            {
              if (this.ProcessFoundItem(versionControlRequestContext, obj, workspace, (IList<Exception>) exceptions, requestIndex, out change))
                return true;
            }
            this.m_resultProcessed = true;
            if (this.m_foundItems == 0)
              exceptions.Add((Exception) new ItemNotFoundException(versionControlRequestContext.RequestContext, workspace, this.ItemSpec, this.VersionSpec, this.m_deferredQuery.DeletedState));
          }
          else
            exceptions.Add(this.m_deferredQuery.Exception);
        }
        else if (this.m_expandedChange != null)
        {
          change = this.m_expandedChange;
          this.m_resultProcessed = true;
          return true;
        }
      }
      return false;
    }

    private bool ProcessFoundItem(
      VersionControlRequestContext versionControlRequestContext,
      Item item,
      Workspace workspace,
      IList<Exception> exceptions,
      int requestIndex,
      out ExpandedChange change)
    {
      change = (ExpandedChange) null;
      if (item.ServerItem == null || this.RequestType == RequestType.Edit && item.IsFolder && this.ItemSpec.RecursionType != RecursionType.None || !this.ItemSpec.postMatch(item.ServerItem) || this.RequestType == RequestType.Branch && !versionControlRequestContext.VersionControlService.SecurityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, item.ItemPathPair) || this.RequestType == RequestType.Undelete && this.DeletionId == 0 && this.m_lastServerItem != null && VersionControlPath.Equals(this.m_lastServerItem, item.ServerItem))
        return false;
      ExpandedChange expandedChange = new ExpandedChange();
      expandedChange.requestIndex = requestIndex;
      expandedChange.itemPathPair = item.ItemPathPair;
      expandedChange.itemId = item.ItemId;
      expandedChange.propertyId = item.PropertyId;
      this.m_lastServerItem = expandedChange.serverItem;
      if (item is WorkspaceItem workspaceItem)
      {
        expandedChange.localItem = workspaceItem.LocalItem;
      }
      else
      {
        try
        {
          expandedChange.localItem = workspace.ServerToLocalItem(versionControlRequestContext.RequestContext, item.ServerItem, this.RequestType == RequestType.Rename);
        }
        catch (ItemNotMappedException ex)
        {
          versionControlRequestContext.RequestContext.TraceException(700043, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.BusinessLogic, (Exception) ex);
        }
      }
      if (this.TargetItem != null)
      {
        if (this.RequestType != RequestType.Branch && VersionControlPath.IsSubItem(item.ServerItem, this.m_sourceRoot) && (this.RequestType != RequestType.Rename || !VersionControlPath.Equals(item.ServerItem, this.m_targetRoot)))
        {
          string relative = VersionControlPath.MakeRelative(item.ServerItem, this.m_sourceRoot);
          expandedChange.targetItemPathPair = ItemPathPair.FromServerItem(VersionControlPath.Combine(this.m_targetRoot, relative, versionControlRequestContext.MaxSupportedServerPathLength));
        }
        else
          expandedChange.targetItemPathPair = ItemPathPair.FromServerItem(this.m_targetRoot);
        if (this.RequestType != RequestType.Undelete)
        {
          if (VersionControlPath.Equals(expandedChange.serverItem, expandedChange.targetServerItem))
          {
            if (this.RequestType == RequestType.Rename && !VersionControlPath.EqualsCaseSensitive(expandedChange.serverItem, expandedChange.targetServerItem))
            {
              ++this.m_foundItems;
            }
            else
            {
              exceptions.Add((Exception) new ItemExistsException(expandedChange.targetServerItem));
              ++this.m_foundItems;
              return false;
            }
          }
          else if (VersionControlPath.IsSubItem(expandedChange.targetServerItem, expandedChange.serverItem))
          {
            exceptions.Add((Exception) new TargetIsChildException(expandedChange.serverItem, expandedChange.targetServerItem));
            ++this.m_foundItems;
            return false;
          }
        }
        if (this.RequestType == RequestType.Branch && item is WorkspaceItem && ((item as WorkspaceItem).PendingChange & (ChangeType.Add | ChangeType.Undelete | ChangeType.Branch)) != ChangeType.None)
        {
          exceptions.Add((Exception) new BranchSourceNotCommittedException(this.ItemSpec.isServerItem ? expandedChange.serverItem : expandedChange.localItem));
          ++this.m_foundItems;
          return false;
        }
      }
      change = expandedChange;
      ++this.m_foundItems;
      return true;
    }

    private void ExpandTarget(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      VersionedItemComponent db,
      bool honorCloaks)
    {
      this.m_sourceRoot = (string) null;
      ItemSpec itemSpec = new ItemSpec(this.TargetItem, RecursionType.None);
      if (itemSpec.isWildcard)
        throw new WildcardNotAllowedException("WildcardNotAllowedForRenameTarget", Array.Empty<object>());
      if (this.TargetItemType == ItemType.Folder)
        this.m_sourceRoot = VersionControlPath.GetFolderName(this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath);
      else if (this.TargetItemType == ItemType.File)
      {
        this.m_sourceRoot = this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath;
      }
      else
      {
        ResultCollection resultCollection = (ResultCollection) null;
        string queryPath;
        string filePattern;
        try
        {
          resultCollection = !itemSpec.isServerItem ? db.DetermineLocalItemType(workspace, itemSpec.Item, versionControlRequestContext.MaxSupportedServerPathLength) : db.DetermineServerItemType(workspace, itemSpec.ItemPathPair);
          ObjectBinder<DeterminedItem> current = resultCollection.GetCurrent<DeterminedItem>();
          current.MoveNext();
          queryPath = current.Current.QueryPath;
          filePattern = current.Current.FilePattern;
        }
        finally
        {
          resultCollection?.Dispose();
        }
        if (queryPath != null)
        {
          if (filePattern == null)
          {
            this.m_sourceRoot = VersionControlPath.GetFolderName(this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath);
          }
          else
          {
            if (this.ItemSpec.isWildcard)
              throw new WildcardNotAllowedException("WildcardNotAllowedForRenameSource", Array.Empty<object>());
            this.m_sourceRoot = this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath;
          }
        }
        else
        {
          this.m_sourceRoot = this.ItemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath;
          if (this.ItemSpec.isWildcard)
            this.m_sourceRoot = VersionControlPath.GetFolderName(this.m_sourceRoot);
        }
      }
      if (this.RequestType == RequestType.Branch)
        this.m_targetRoot = itemSpec.toServerItemWithoutMappingRenames(versionControlRequestContext, workspace, honorCloaks).ProjectNamePath;
      else
        this.m_targetRoot = itemSpec.toServerItem(versionControlRequestContext.RequestContext, workspace, honorCloaks).ProjectNamePath;
    }

    public object Clone() => this.MemberwiseClone();

    public void Reset()
    {
      if (this.m_deferredQuery != null)
        this.m_deferredQuery.Reset();
      this.m_resultProcessed = false;
    }

    internal static PropertyValue[] MergeProperties(
      IEnumerable<PropertyValue> existingProperties,
      PropertyValue[] newProperties)
    {
      Dictionary<string, PropertyValue> dictionary = new Dictionary<string, PropertyValue>((IEqualityComparer<string>) VssStringComparer.PropertyName);
      if (existingProperties != null)
      {
        foreach (PropertyValue existingProperty in existingProperties)
          dictionary[existingProperty.PropertyName] = existingProperty;
      }
      if (newProperties.Length == 1 && newProperties[0].PropertyName == null && newProperties[0].Value == null)
      {
        dictionary = new Dictionary<string, PropertyValue>();
      }
      else
      {
        foreach (PropertyValue newProperty in newProperties)
        {
          if (newProperty.Value == null)
            dictionary.Remove(newProperty.PropertyName);
          else
            dictionary[newProperty.PropertyName] = newProperty;
        }
      }
      return dictionary.Values.ToArray<PropertyValue>();
    }

    internal static LockLevel EnforceSingleCheckout(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      string serverItem,
      LockLevel requestedLockLevel)
    {
      if (workspace.IsLocal)
      {
        if (LockLevel.CheckOut == requestedLockLevel)
          throw new CannotTakeCheckoutLockInLocalWorkspaceException(serverItem, workspace.Name, workspace.OwnerDisplayName);
      }
      else if (versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(versionControlRequestContext))
      {
        if (LockLevel.CheckOut == requestedLockLevel)
          throw new CheckoutLocksDisabledException(serverItem, workspace.Name, workspace.OwnerDisplayName);
      }
      else if (versionControlRequestContext.VersionControlService.GetTeamProjectFolder(versionControlRequestContext).IsExclusiveCheckout(versionControlRequestContext.RequestContext, serverItem) || versionControlRequestContext.VersionControlService.GetFileTypeManager(versionControlRequestContext).IsExclusiveCheckout(versionControlRequestContext.RequestContext, serverItem))
        return LockLevel.CheckOut;
      return requestedLockLevel;
    }

    internal static bool EnforceGetLatestOnCheckout(
      VersionControlRequestContext versionControlRequestContext,
      string serverItem,
      PendChangesOptions options,
      SupportedFeatures supportedFeatures)
    {
      bool flag1 = (supportedFeatures & SupportedFeatures.GetLatestOnCheckout) != 0;
      bool flag2 = (options & PendChangesOptions.GetLatestOnCheckout) != 0;
      if (versionControlRequestContext.VersionControlService.GetAllowAsynchronousCheckoutInServerWorkspaces(versionControlRequestContext) || !flag1)
        return false;
      return flag2 || versionControlRequestContext.VersionControlService.GetTeamProjectFolder(versionControlRequestContext).IsGetLatestOnCheckout(versionControlRequestContext.RequestContext, serverItem);
    }

    internal static RequestType changeTypeToRequestType(ChangeType changeType)
    {
      if ((changeType & ChangeType.Add) != ChangeType.None)
        return RequestType.Add;
      if ((changeType & ChangeType.Branch) != ChangeType.None)
        return RequestType.Branch;
      if ((changeType & ChangeType.Delete) != ChangeType.None)
        return RequestType.Delete;
      if ((changeType & ChangeType.Undelete) != ChangeType.None)
        return RequestType.Undelete;
      if ((changeType & ChangeType.Rename) != ChangeType.None)
        return RequestType.Rename;
      if ((changeType & ChangeType.Edit) != ChangeType.None)
        return RequestType.Edit;
      if ((changeType & ChangeType.Encoding) != ChangeType.None)
        return RequestType.Encoding;
      return (changeType & ChangeType.Lock) != ChangeType.None ? RequestType.Lock : RequestType.None;
    }
  }
}
