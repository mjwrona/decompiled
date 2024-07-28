// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Conflict
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  public class Conflict : ISignable, ICacheable
  {
    internal int m_baseFileId;
    internal int m_theirFileId;
    internal int m_yourFileId;
    private int m_conflictId;
    private int m_pendingChangeId;
    internal ChangeType YourChangeType;
    private ItemPathPair m_yourItemPathPair;
    private ItemPathPair m_yourItemSourcePathPair;
    private int m_yourEncoding;
    private ItemType m_yourItemType;
    private int m_yourVersion;
    private int m_yourItemId;
    private int m_yourDeletionId;
    internal ChangeType YourLocalChangeType;
    private int m_yourLastMergedVersion;
    private ItemPathPair m_baseItemPathPair;
    private int m_baseEncoding;
    private int m_baseItemId;
    private int m_baseVersion;
    private byte[] m_baseHashValue;
    private int m_baseDeletionId;
    private ItemType m_baseItemType;
    internal ChangeType BaseChangeType;
    private int m_theirItemId;
    private int m_theirVersion;
    private ItemPathPair m_theirItemPathPair;
    private int m_theirEncoding;
    private byte[] m_theirHashValue;
    private int m_theirDeletionId;
    private ItemType m_theirItemType;
    private int m_theirLastMergedVersion;
    private int m_theirVersionFrom;
    private int m_theirChangeType;
    private string m_sourceLocalItem;
    private string m_targetLocalItem;
    private ConflictType m_type;
    private int m_reason;
    private bool m_isNamespaceConflict;
    private bool m_isForced;
    private Resolution m_resolution;
    private bool m_isResolved;
    private string m_baseDownloadUrl;
    private string m_theirDownloadUrl;
    private string m_yourDownloadUrl;
    private int m_conflictOptions;
    private static readonly TimeSpan c_tenSecondsTimeSpan = new TimeSpan(0, 0, 10);

    [XmlAttribute("cid")]
    public int ConflictId
    {
      get => this.m_conflictId;
      set => this.m_conflictId = value;
    }

    [XmlAttribute("pcid")]
    public int PendingChangeId
    {
      get => this.m_pendingChangeId;
      set => this.m_pendingChangeId = value;
    }

    [XmlAttribute("ychg")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "YourChangeType")]
    public ChangeType YourChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.YourChangeType);
      set => this.YourChangeType |= value;
    }

    [XmlAttribute("ychgEx")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "YourChangeEx")]
    public int YourChangeTypeEx
    {
      get => (int) this.YourChangeType;
      set => this.YourChangeType |= (ChangeType) value;
    }

    [XmlAttribute("ysitem")]
    public string YourServerItem
    {
      get => this.YourItemPathPair.ProjectNamePath;
      set => this.YourItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair YourItemPathPair
    {
      get => this.m_yourItemPathPair;
      set => this.m_yourItemPathPair = value;
    }

    [XmlAttribute("ysitemsrc")]
    public string YourServerItemSource
    {
      get => this.YourItemSourcePathPair.ProjectNamePath;
      set => this.YourItemSourcePathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair YourItemSourcePathPair
    {
      get => this.m_yourItemSourcePathPair;
      set => this.m_yourItemSourcePathPair = value;
    }

    [XmlAttribute("yenc")]
    public int YourEncoding
    {
      get => this.m_yourEncoding;
      set => this.m_yourEncoding = value;
    }

    [XmlAttribute("yprop")]
    public int YourPropertyId { get; set; }

    [XmlAttribute("ytype")]
    public ItemType YourItemType
    {
      get => this.m_yourItemType;
      set => this.m_yourItemType = value;
    }

    [XmlAttribute("yver")]
    public int YourVersion
    {
      get => this.m_yourVersion;
      set => this.m_yourVersion = value;
    }

    [XmlAttribute("yitemid")]
    public int YourItemId
    {
      get => this.m_yourItemId;
      set => this.m_yourItemId = value;
    }

    [XmlAttribute("ydid")]
    public int YourDeletionId
    {
      get => this.m_yourDeletionId;
      set => this.m_yourDeletionId = value;
    }

    [XmlAttribute("ylchg")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "YourLocalChangeType")]
    public ChangeType YourLocalChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.YourLocalChangeType);
      set => this.YourLocalChangeType |= value;
    }

    [XmlAttribute("ylchgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "YourLocalChangeEx")]
    public int YourLocalChangeTypeEx
    {
      get => (int) this.YourLocalChangeType;
      set => this.YourLocalChangeType |= (ChangeType) value;
    }

    [XmlAttribute("ylmver")]
    public int YourLastMergedVersion
    {
      get => this.m_yourLastMergedVersion;
      set => this.m_yourLastMergedVersion = value;
    }

    [XmlAttribute("bsitem")]
    public string BaseServerItem
    {
      get => this.BaseItemPathPair.ProjectNamePath;
      set => this.BaseItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair BaseItemPathPair
    {
      get => this.m_baseItemPathPair;
      set => this.m_baseItemPathPair = value;
    }

    [XmlAttribute("benc")]
    public int BaseEncoding
    {
      get => this.m_baseEncoding;
      set => this.m_baseEncoding = value;
    }

    [XmlAttribute("bprop")]
    public int BasePropertyId { get; set; }

    [XmlAttribute("bitemid")]
    public int BaseItemId
    {
      get => this.m_baseItemId;
      set => this.m_baseItemId = value;
    }

    [XmlAttribute("bver")]
    public int BaseVersion
    {
      get => this.m_baseVersion;
      set => this.m_baseVersion = value;
    }

    [XmlAttribute("bhash")]
    public byte[] BaseHashValue
    {
      get => this.m_baseHashValue;
      set => this.m_baseHashValue = value;
    }

    [XmlAttribute("bdid")]
    public int BaseDeletionId
    {
      get => this.m_baseDeletionId;
      set => this.m_baseDeletionId = value;
    }

    [XmlAttribute("btype")]
    public ItemType BaseItemType
    {
      get => this.m_baseItemType;
      set => this.m_baseItemType = value;
    }

    [XmlAttribute("bchg")]
    [DefaultValue(ChangeType.None)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "BaseChangeType")]
    public ChangeType BaseChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.BaseChangeType);
      set => this.BaseChangeType |= value;
    }

    [XmlAttribute("bchgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "BaseChangeEx")]
    public int BaseChangeTypeEx
    {
      get => (int) this.BaseChangeType;
      set => this.BaseChangeType |= (ChangeType) value;
    }

    [XmlAttribute("titemid")]
    public int TheirItemId
    {
      get => this.m_theirItemId;
      set => this.m_theirItemId = value;
    }

    [XmlAttribute("tver")]
    public int TheirVersion
    {
      get => this.m_theirVersion;
      set => this.m_theirVersion = value;
    }

    [XmlAttribute("tsitem")]
    public string TheirServerItem
    {
      get => this.TheirItemPathPair.ProjectNamePath;
      set => this.TheirItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair TheirItemPathPair
    {
      get => this.m_theirItemPathPair;
      set => this.m_theirItemPathPair = value;
    }

    [XmlAttribute("tenc")]
    public int TheirEncoding
    {
      get => this.m_theirEncoding;
      set => this.m_theirEncoding = value;
    }

    [XmlAttribute("tprop")]
    public int TheirPropertyId { get; set; }

    [XmlAttribute("thash")]
    public byte[] TheirHashValue
    {
      get => this.m_theirHashValue;
      set => this.m_theirHashValue = value;
    }

    [XmlAttribute("tdid")]
    public int TheirDeletionId
    {
      get => this.m_theirDeletionId;
      set => this.m_theirDeletionId = value;
    }

    [XmlAttribute("ttype")]
    public ItemType TheirItemType
    {
      get => this.m_theirItemType;
      set => this.m_theirItemType = value;
    }

    [XmlAttribute("tlmver")]
    public int TheirLastMergedVersion
    {
      get => this.m_theirLastMergedVersion;
      set => this.m_theirLastMergedVersion = value;
    }

    [XmlAttribute("tverf")]
    public int TheirVersionFrom
    {
      get => this.m_theirVersionFrom;
      set => this.m_theirVersionFrom = value;
    }

    [XmlAttribute("tctyp")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int TheirChangeType
    {
      get => this.m_theirChangeType;
      set => this.m_theirChangeType = value;
    }

    [XmlAttribute("isc")]
    public bool IsShelvesetConflict { get; set; }

    [XmlAttribute("tsn")]
    public string TheirShelvesetName { get; set; }

    [XmlAttribute("tson")]
    public string TheirShelvesetOwnerName { get; set; }

    internal Guid TheirShelvesetOwnerId { get; set; }

    [XmlAttribute("srclitem")]
    public string SourceLocalItem
    {
      get => this.m_sourceLocalItem;
      set => this.m_sourceLocalItem = value;
    }

    [XmlAttribute("tgtlitem")]
    public string TargetLocalItem
    {
      get => this.m_targetLocalItem;
      set => this.m_targetLocalItem = value;
    }

    [XmlAttribute("ctype")]
    public ConflictType Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    [XmlAttribute("reason")]
    public int Reason
    {
      get => this.m_reason;
      set => this.m_reason = value;
    }

    [XmlAttribute("isnamecflict")]
    public bool IsNamespaceConflict
    {
      get => this.m_isNamespaceConflict;
      set => this.m_isNamespaceConflict = value;
    }

    [XmlAttribute("isforced")]
    public bool IsForced
    {
      get => this.m_isForced;
      set => this.m_isForced = value;
    }

    [XmlAttribute("res")]
    [DefaultValue(Resolution.None)]
    public Resolution Resolution
    {
      get => this.m_resolution;
      set => this.m_resolution = value;
    }

    [XmlAttribute("isresolved")]
    public bool IsResolved
    {
      get => this.m_isResolved;
      set => this.m_isResolved = value;
    }

    [XmlAttribute("bdurl")]
    [ClientProperty(ClientVisibility.Private)]
    public string BaseDownloadUrl
    {
      get => this.m_baseDownloadUrl;
      set => this.m_baseDownloadUrl = value;
    }

    [XmlAttribute("tdurl")]
    [ClientProperty(ClientVisibility.Private)]
    public string TheirDownloadUrl
    {
      get => this.m_theirDownloadUrl;
      set => this.m_theirDownloadUrl = value;
    }

    [XmlAttribute("ydurl")]
    [ClientProperty(ClientVisibility.Private)]
    public string YourDownloadUrl
    {
      get => this.m_yourDownloadUrl;
      set => this.m_yourDownloadUrl = value;
    }

    [XmlAttribute("copt")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private)]
    public int ConflictOptions
    {
      get => this.m_conflictOptions;
      set => this.m_conflictOptions = value;
    }

    internal void DetermineShelvesetOwnerName(
      VersionControlRequestContext versionControlRequestContext)
    {
      if (this.IsShelvesetConflict && this.TheirShelvesetOwnerId != Guid.Empty)
        this.TheirShelvesetOwnerName = versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentityDisplayName(versionControlRequestContext.RequestContext, this.TheirShelvesetOwnerId);
      else
        this.TheirShelvesetOwnerName = (string) null;
    }

    internal static void AddConflict(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      ConflictType conflictType,
      int itemId,
      int versionFrom,
      int pendingChangeId,
      string sourceLocalItem,
      string targetLocalItem,
      int reason)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      if (conflictType != ConflictType.Local && conflictType != ConflictType.Get)
        throw new ArgumentException(Resources.Get("InvalidConflictType"), conflictType.ToString());
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        DateTime utcNow = DateTime.UtcNow;
        try
        {
          versionedItemComponent.AddConflict(workspace, conflictType, itemId, versionFrom, pendingChangeId, sourceLocalItem, targetLocalItem, reason, -3, versionControlRequestContext.MaxSupportedServerPathLength);
        }
        finally
        {
          if (DateTime.UtcNow - utcNow > Conflict.c_tenSecondsTimeSpan)
            versionControlRequestContext.RequestContext.Trace(1013125, TraceLevel.Warning, TraceArea.Conflicts, TraceLayer.Command, "AddConflict database call exceeded 10 seconds! Parameters: Workspace: {0}, ConflictType: {1}, ItemId: {2}, VersionFrom: {3}, PendingChangeId: {4}, SourceLocalItem: {5}, TargetLocalItem: {6}, Reason: {7}", (object) workspace.Name, (object) conflictType, (object) itemId, (object) versionFrom, (object) pendingChangeId, (object) sourceLocalItem, (object) targetLocalItem, (object) reason);
        }
      }
    }

    internal static void RemoveLocalConflict(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      int conflictId)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
        versionedItemComponent.RemoveLocalConflict(workspace, conflictId);
    }

    internal static bool MatchesFilter(string path, ItemSpec[] items)
    {
      if (items == null || items.Length == 0)
        return true;
      if (string.IsNullOrEmpty(path))
        return false;
      bool flag = VersionControlPath.IsServerItem(path);
      foreach (ItemSpec itemSpec in items)
      {
        bool recursive = itemSpec.RecursionType == RecursionType.Full;
        string matchFolder;
        string matchPattern;
        if (flag)
        {
          if (itemSpec.isServerItem)
          {
            string fullPath = VersionControlPath.GetFullPath(itemSpec.Item);
            VersionControlPath.Parse(fullPath, out matchFolder, out matchPattern);
            if (VersionControlPath.Match(path, fullPath, (string) null, recursive) || VersionControlPath.Match(path, matchFolder, matchPattern, recursive))
              return true;
          }
        }
        else if (!itemSpec.isServerItem)
        {
          string fullPath = FileSpec.GetFullPath(itemSpec.Item);
          FileSpec.Parse(fullPath, out matchFolder, out matchPattern);
          if (FileSpec.Match(path, fullPath, (string) null, recursive) || FileSpec.Match(path, matchFolder, matchPattern, recursive))
            return true;
        }
      }
      return false;
    }

    public int GetDownloadUrlCount() => 3;

    public int GetFileId(int index)
    {
      switch (index)
      {
        case 0:
          return this.m_baseFileId;
        case 1:
          return this.m_theirFileId;
        case 2:
          return this.m_yourFileId;
        default:
          return 0;
      }
    }

    public byte[] GetHashValue(int index)
    {
      switch (index)
      {
        case 0:
          return this.m_baseHashValue;
        case 1:
          return this.m_theirHashValue;
        case 2:
          return (byte[]) null;
        default:
          return (byte[]) null;
      }
    }

    public void SetDownloadUrl(int index, string downloadUrl)
    {
      switch (index)
      {
        case 0:
          this.BaseDownloadUrl = downloadUrl;
          break;
        case 1:
          this.TheirDownloadUrl = downloadUrl;
          break;
        case 2:
          this.YourDownloadUrl = downloadUrl;
          break;
      }
    }

    public int GetCachedSize() => 2500;
  }
}
