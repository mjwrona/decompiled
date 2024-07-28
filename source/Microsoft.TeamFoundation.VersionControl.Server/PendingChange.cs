// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingChange
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [RequiredClientService("VersionControlServer")]
  [CallOnDeserialization("AfterDeserialize")]
  public class PendingChange : ICloneable, ISignable, ICacheable, IPropertyMergerItem
  {
    internal PendingSet pendingSet;
    internal int uploadFileId;
    internal int fileId;
    internal ChangeType ChangeType;
    private DateTime m_creationDate;
    private int m_deletionId;
    private ItemType m_itemType;
    private int m_encoding = -2;
    private int m_itemId;
    private string m_localItem;
    private LockLevel m_lockLevel;
    private ItemPathPair m_itemPathPair;
    private string m_sourceLocalItem;
    private ItemPathPair m_sourceItemPathPair;
    private int m_version;
    private byte[] m_hashValue;
    private long m_length = -1;
    private byte[] m_uploadHashValue;
    private int m_pendingChangeId;
    private string m_downloadUrl;
    private string m_shelvedDownloadUrl;
    private int m_sourceVersionFrom;
    private int m_sourceDeletionId;

    [XmlAttribute("chgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeEx")]
    public int ChangeEx
    {
      get => (int) this.ChangeType;
      set => this.ChangeType |= (ChangeType) value;
    }

    [XmlAttribute("chg")]
    [DefaultValue(ChangeType.None)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeType")]
    public ChangeType ChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.ChangeType);
      set => this.ChangeType |= value;
    }

    [XmlAttribute("date")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime CreationDate
    {
      get => this.m_creationDate;
      set => this.m_creationDate = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    [XmlAttribute("type")]
    [DefaultValue(ItemType.Any)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ItemType ItemType
    {
      get => this.m_itemType;
      set => this.m_itemType = value;
    }

    [XmlAttribute("enc")]
    [DefaultValue(-2)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int Encoding
    {
      get => this.m_encoding;
      set => this.m_encoding = value;
    }

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    [XmlAttribute("local")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LocalItem
    {
      get => this.m_localItem;
      set => this.m_localItem = value;
    }

    [XmlAttribute("lock")]
    [DefaultValue(LockLevel.None)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public LockLevel LockLevel
    {
      get => this.m_lockLevel;
      set => this.m_lockLevel = value;
    }

    [XmlAttribute("item")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ServerItem
    {
      get => this.ItemPathPair.ProjectNamePath;
      set => this.ItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair ItemPathPair
    {
      get => this.m_itemPathPair;
      set => this.m_itemPathPair = value;
    }

    [XmlAttribute("srclocal")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string SourceLocalItem
    {
      get => this.m_sourceLocalItem;
      set => this.m_sourceLocalItem = value;
    }

    [XmlAttribute("srcitem")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string SourceServerItem
    {
      get => this.SourceItemPathPair.ProjectNamePath;
      set => this.SourceItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair SourceItemPathPair
    {
      get => this.m_sourceItemPathPair;
      set => this.m_sourceItemPathPair = value;
    }

    [XmlAttribute("svrfm")]
    [DefaultValue(0)]
    public int SourceVersionFrom
    {
      get => this.m_sourceVersionFrom;
      set => this.m_sourceVersionFrom = value;
    }

    [XmlAttribute("sdi")]
    [DefaultValue(0)]
    public int SourceDeletionId
    {
      get => this.m_sourceDeletionId;
      set => this.m_sourceDeletionId = value;
    }

    [XmlAttribute("ver")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int Version
    {
      get => this.m_version;
      set => this.m_version = value;
    }

    [XmlAttribute("hash")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public byte[] HashValue
    {
      get => this.m_hashValue;
      set => this.m_hashValue = value;
    }

    [XmlAttribute("len")]
    [DefaultValue(-1)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public long Length
    {
      get => this.m_length;
      set => this.m_length = value;
    }

    [XmlAttribute("uhash")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public byte[] UploadHashValue
    {
      get => this.m_uploadHashValue;
      set => this.m_uploadHashValue = value;
    }

    [XmlAttribute("pcid")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int PendingChangeId
    {
      get => this.m_pendingChangeId;
      set => this.m_pendingChangeId = value;
    }

    [XmlAttribute("durl")]
    [ClientProperty(ClientVisibility.Private)]
    public string DownloadUrl
    {
      get => this.m_downloadUrl;
      set => this.m_downloadUrl = value;
    }

    [XmlAttribute("shelvedurl")]
    [ClientProperty(ClientVisibility.Private)]
    public string ShelvedDownloadUrl
    {
      get => this.m_shelvedDownloadUrl;
      set => this.m_shelvedDownloadUrl = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public List<MergeSource> MergeSources { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    [XmlAttribute("ct")]
    public int ConflictType { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public StreamingCollection<PropertyValue> PropertyValues { get; set; }

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => this.PropertyValues;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      this.PropertyValues = properties;
    }

    [XmlIgnore]
    int IPropertyMergerItem.SequenceId { get; set; }

    [XmlIgnore]
    internal int PropertyId { get; set; }

    [XmlIgnore]
    internal Guid PropertyDataspaceId { get; set; }

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => this.PropertyId == -1 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.PropertyId, 0, this.PropertyDataspaceId);

    public int GetDownloadUrlCount() => 2;

    public int GetFileId(int index)
    {
      if (index == 0)
        return this.uploadFileId;
      return index == 1 ? this.fileId : 0;
    }

    public byte[] GetHashValue(int index)
    {
      if (index == 0)
        return this.m_uploadHashValue;
      return index == 1 ? this.m_hashValue : (byte[]) null;
    }

    public void SetDownloadUrl(int index, string downloadUrl)
    {
      if (index != 0)
      {
        if (index != 1)
          return;
        this.DownloadUrl = downloadUrl;
      }
      else
        this.ShelvedDownloadUrl = downloadUrl;
    }

    internal bool HasPermission(
      VersionControlRequestContext versionControlRequestContext,
      Guid callingTeamFoundationId)
    {
      SecurityManager securityWrapper = versionControlRequestContext.VersionControlService.SecurityWrapper;
      return object.Equals((object) callingTeamFoundationId, (object) this.pendingSet.OwnerTeamFoundationId) || (string.IsNullOrEmpty(this.SourceServerItem) || securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, this.SourceItemPathPair)) && (string.IsNullOrEmpty(this.ServerItem) || securityWrapper.HasItemPermission(versionControlRequestContext, VersionedItemPermissions.Read, this.ItemPathPair));
    }

    internal static PendingChange[] QueryPendingChangesById(
      VersionControlRequestContext versionControlRequestContext,
      int[] pendingChangeIds,
      bool generateDownloadUrls)
    {
      PendingChange[] objectsToSign = new PendingChange[pendingChangeIds.Length];
      Guid id = versionControlRequestContext.VersionControlService.SecurityWrapper.FindIdentity(versionControlRequestContext.RequestContext).Id;
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        for (int index = 0; index < pendingChangeIds.Length; ++index)
        {
          ObjectBinder<PendingChange> current1 = versionedItemComponent.FindPendingChangeById(pendingChangeIds[index]).GetCurrent<PendingChange>();
          if (current1.MoveNext())
          {
            PendingChange current2 = current1.Current;
            if (current2.HasPermission(versionControlRequestContext, id))
              objectsToSign[index] = current2;
          }
        }
      }
      if (generateDownloadUrls)
      {
        using (UrlSigner urlSigner = new UrlSigner(versionControlRequestContext.RequestContext))
          urlSigner.SignObjects((ICollection) objectsToSign);
      }
      return objectsToSign;
    }

    internal static void CheckPendingChanges(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      string[] serverItems,
      List<Failure> failures)
    {
      PendingChange.CheckPendingChanges(versionControlRequestContext, workspace, (IEnumerable<string>) serverItems, failures, false);
    }

    internal static void CheckPendingChanges(
      VersionControlRequestContext versionControlRequestContext,
      Workspace workspace,
      IEnumerable<string> serverItems,
      List<Failure> failures,
      bool skipPermissionChecks)
    {
      versionControlRequestContext.VersionControlService.SecurityWrapper.CheckWorkspacePermission(versionControlRequestContext, 2, workspace);
      List<string> itemManager = new List<string>();
      foreach (string serverItem in serverItems)
      {
        if (!skipPermissionChecks)
        {
          try
          {
            versionControlRequestContext.VersionControlService.SecurityWrapper.CheckItemPermission(versionControlRequestContext, VersionedItemPermissions.Checkin, serverItem);
          }
          catch (RequestCanceledException ex)
          {
            throw;
          }
          catch (ApplicationException ex)
          {
            versionControlRequestContext.RequestContext.TraceException(700100, TraceLevel.Info, TraceArea.PendChanges, TraceLayer.BusinessLogic, (Exception) ex);
            failures.Add(new Failure((Exception) ex, serverItem, RequestType.None));
            continue;
          }
        }
        itemManager.Add(serverItem);
      }
      if (itemManager.Count <= 0)
        return;
      using (VersionedItemComponent versionedItemComponent = versionControlRequestContext.VersionControlService.GetVersionedItemComponent(versionControlRequestContext))
      {
        using (ResultCollection resultCollection = versionedItemComponent.Checkin(workspace.OwnerId, workspace.Name, PendingSetType.Workspace, (IEnumerable<string>) itemManager, string.Empty, DateTime.MinValue, workspace.OwnerId, (CheckinNote) null, (PolicyOverrideInfo) null, workspace.OwnerId, false, false, false, itemManager.Count, versionControlRequestContext.MaxSupportedServerPathLength))
        {
          resultCollection.NextResult();
          resultCollection.NextResult();
          resultCollection.NextResult();
          foreach (PendingChangeConflict pendingChangeConflict in resultCollection.GetCurrent<PendingChangeConflict>().Items)
            failures.Add(new Failure((Exception) pendingChangeConflict.toException()));
        }
      }
    }

    internal bool IsFolder => this.ItemType == ItemType.Folder;

    public object Clone() => this.MemberwiseClone();

    public int GetCachedSize() => 2000;

    internal static ChangeType GetLegacyChangeType(ChangeType type)
    {
      type &= ~(ChangeType.Rollback | ChangeType.SourceRename | ChangeType.Property);
      return type;
    }

    internal int InputIndex { get; set; }
  }
}
