// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.GetOperation
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [CallOnDeserialization("AfterDeserialize")]
  public class GetOperation : ISignable, ICacheable, IPropertyMergerItem
  {
    internal int fileId;
    private ItemType m_itemType;
    private int m_itemId;
    private string m_sourceLocalItem;
    private string m_targetLocalItem;
    private ItemPathPair m_targetItemPathPair;
    private int m_versionServer;
    private int m_versionLocal;
    private int m_versionRevertTo;
    private int m_deletionId;
    internal ChangeType ChangeType;
    private LockLevel m_lockLevel;
    private bool m_isLatest = true;
    private byte[] m_hashValue;
    private int m_pendingChangeId;
    private bool m_hasConflict;
    internal ChangeType ConflictingChangeType;
    private int m_conflictingItemId;
    private string m_downloadUrl;
    private byte m_isNamespaceConflict;

    [XmlAttribute("type")]
    [DefaultValue(ItemType.Any)]
    public ItemType ItemType
    {
      get => this.m_itemType;
      set => this.m_itemType = value;
    }

    [XmlAttribute("itemid")]
    [DefaultValue(0)]
    public int ItemId
    {
      get => this.m_itemId;
      set => this.m_itemId = value;
    }

    [XmlAttribute("slocal")]
    public string SourceLocalItem
    {
      get => this.m_sourceLocalItem;
      set => this.m_sourceLocalItem = value;
    }

    [XmlAttribute("tlocal")]
    public string TargetLocalItem
    {
      get => this.m_targetLocalItem;
      set => this.m_targetLocalItem = value;
    }

    [XmlAttribute("sitem")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public string SourceServerItem
    {
      get => this.SourceItemPathPair.ProjectNamePath;
      set => this.SourceItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair SourceItemPathPair { get; set; }

    [XmlAttribute("titem")]
    public string TargetServerItem
    {
      get => this.TargetItemPathPair.ProjectNamePath;
      set => this.TargetItemPathPair = ItemPathPair.FromServerItem(value);
    }

    [XmlIgnore]
    internal ItemPathPair TargetItemPathPair
    {
      get => this.m_targetItemPathPair;
      set => this.m_targetItemPathPair = value;
    }

    [XmlAttribute("sver")]
    [DefaultValue(0)]
    public int VersionServer
    {
      get => this.m_versionServer;
      set => this.m_versionServer = value;
    }

    [XmlAttribute("vrevto")]
    [DefaultValue(-2)]
    public int VersionRevertTo
    {
      get => this.m_versionRevertTo;
      set => this.m_versionRevertTo = value;
    }

    [XmlAttribute("lver")]
    [DefaultValue(0)]
    public int VersionLocal
    {
      get => this.m_versionLocal;
      set => this.m_versionLocal = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

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

    [XmlAttribute("lock")]
    [DefaultValue(LockLevel.None)]
    public LockLevel LockLevel
    {
      get => this.m_lockLevel;
      set => this.m_lockLevel = value;
    }

    [XmlAttribute("il")]
    [DefaultValue(true)]
    public bool IsLatest
    {
      get => this.m_isLatest;
      set => this.m_isLatest = value;
    }

    public byte[] HashValue
    {
      get => this.m_hashValue;
      set => this.m_hashValue = value;
    }

    [XmlAttribute("pcid")]
    [DefaultValue(0)]
    public int PendingChangeId
    {
      get => this.m_pendingChangeId;
      set => this.m_pendingChangeId = value;
    }

    [XmlAttribute("cnflct")]
    [DefaultValue(false)]
    public bool HasConflict
    {
      get => this.m_hasConflict;
      set => this.m_hasConflict = value;
    }

    [XmlAttribute("cnflctchg")]
    [DefaultValue(ChangeType.None)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ConflictingChangeType")]
    public ChangeType ConflictingChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.ConflictingChangeType);
      set => this.ConflictingChangeType = value;
    }

    [XmlAttribute("cnflctchgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ConflictingChangeEx")]
    public int ConflictingChangeTypeEx
    {
      get => (int) this.ConflictingChangeType;
      set => this.ConflictingChangeType = (ChangeType) value;
    }

    [XmlAttribute("cnflctitemid")]
    [DefaultValue(0)]
    public int ConflictingItemId
    {
      get => this.m_conflictingItemId;
      set => this.m_conflictingItemId = value;
    }

    [XmlAttribute("nmscnflct")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "IsNamespaceConflict", Direction = ClientPropertySerialization.Bidirectional)]
    public byte IsNamespaceConflict
    {
      get => this.m_isNamespaceConflict;
      set => this.m_isNamespaceConflict = value;
    }

    [XmlAttribute("durl")]
    [ClientProperty(ClientVisibility.Private)]
    public string DownloadUrl
    {
      get => this.m_downloadUrl;
      set => this.m_downloadUrl = value;
    }

    [XmlAttribute("enc")]
    [DefaultValue(-2)]
    public int Encoding { get; set; }

    [XmlAttribute("vsd")]
    [DefaultValue(typeof (DateTime), "00:00:00.0000000, January 1, 0001")]
    public DateTime VersionServerDate { get; set; }

    public int GetDownloadUrlCount() => 1;

    public int GetFileId(int index) => string.IsNullOrEmpty(this.TargetLocalItem) ? 0 : this.fileId;

    public byte[] GetHashValue(int index) => string.IsNullOrEmpty(this.TargetLocalItem) ? (byte[]) null : this.m_hashValue;

    public void SetDownloadUrl(int index, string downloadUrl) => this.DownloadUrl = downloadUrl;

    public int GetCachedSize() => 1800;

    [XmlIgnore]
    int IPropertyMergerItem.SequenceId { get; set; }

    [XmlIgnore]
    internal int PropertyId { get; set; }

    [XmlIgnore]
    internal Guid ItemDataspaceId { get; set; }

    [Obsolete("Please use the Attributes property instead", false)]
    [XmlIgnore]
    public StreamingCollection<PropertyValue> Properties
    {
      get => this.Attributes;
      set => this.Attributes = value;
    }

    [XmlArray("Properties")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "InternalAttributes")]
    public StreamingCollection<PropertyValue> Attributes { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalPropertyValues")]
    public StreamingCollection<PropertyValue> PropertyValues { get; set; }

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => object.Equals((object) artifactKind, (object) VersionControlPropertyKinds.VersionedItem) || object.Equals((object) artifactKind, (object) VersionControlPropertyKinds.Annotation) ? (this.VersionServer == 0 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.ItemId, this.VersionServer, this.ItemDataspaceId)) : (this.PropertyId == -1 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.PropertyId, 0, this.ItemDataspaceId));

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => artifactKind.Equals(VersionControlPropertyKinds.VersionedItem) || artifactKind.Equals(VersionControlPropertyKinds.Annotation) ? this.Attributes : this.PropertyValues;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      if (artifactKind.Equals(VersionControlPropertyKinds.VersionedItem) || artifactKind.Equals(VersionControlPropertyKinds.Annotation))
        this.Attributes = properties;
      else
        this.PropertyValues = properties;
    }
  }
}
