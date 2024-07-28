// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.ExtendedItem
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class ExtendedItem : ICacheable, IPropertyMergerItem
  {
    internal Guid lockOwnerId;
    private int m_versionLocal;
    private int m_deletionId;
    private int m_versionLatest;
    private ItemType m_itemType;
    private int m_encoding = -3;
    private int m_itemId;
    private string m_localItem;
    private ItemPathPair m_targetItemPathPair;
    private ItemPathPair m_sourceItemPathPair;
    internal ChangeType ChangeType;
    private bool m_hasOtherPendingChange;
    private LockLevel m_lockStatus;
    private string m_lockOwner;
    private string m_lockOwnerDisplayName;
    private bool m_isBranch;
    private DateTime m_checkinDate;

    [XmlAttribute("lver")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int VersionLocal
    {
      get => this.m_versionLocal;
      set => this.m_versionLocal = value;
    }

    [XmlAttribute("did")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int DeletionId
    {
      get => this.m_deletionId;
      set => this.m_deletionId = value;
    }

    [XmlAttribute("latest")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int VersionLatest
    {
      get => this.m_versionLatest;
      set => this.m_versionLatest = value;
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
    [DefaultValue(-3)]
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

    [XmlAttribute("titem")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
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

    [XmlAttribute("sitem")]
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

    [XmlAttribute("chg")]
    [DefaultValue(ChangeType.None)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeType")]
    public ChangeType ChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.ChangeType);
      set => this.ChangeType |= value;
    }

    [XmlAttribute("chgEx")]
    [DefaultValue(0)]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeEx")]
    public int ChangeEx
    {
      get => (int) this.ChangeType;
      set => this.ChangeType |= (ChangeType) value;
    }

    [XmlAttribute("ochg")]
    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool HasOtherPendingChange
    {
      get => this.m_hasOtherPendingChange;
      set => this.m_hasOtherPendingChange = value;
    }

    [XmlAttribute("lock")]
    [DefaultValue(LockLevel.None)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public LockLevel LockStatus
    {
      get => this.m_lockStatus;
      set => this.m_lockStatus = value;
    }

    [XmlAttribute("lowner")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LockOwner
    {
      get => this.m_lockOwner;
      set => this.m_lockOwner = value;
    }

    [XmlAttribute("lownerdisp")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string LockOwnerDisplayName
    {
      get => this.m_lockOwnerDisplayName;
      set => this.m_lockOwnerDisplayName = value;
    }

    [DefaultValue(false)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsBranch
    {
      get => this.m_isBranch;
      set => this.m_isBranch = value;
    }

    [XmlAttribute("date")]
    public DateTime CheckinDate
    {
      get => this.m_checkinDate;
      set => this.m_checkinDate = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public StreamingCollection<PropertyValue> PropertyValues { get; set; }

    [XmlIgnore]
    int IPropertyMergerItem.SequenceId { get; set; }

    [XmlIgnore]
    internal int PropertyId { get; set; }

    [XmlIgnore]
    internal Guid ItemDataspaceId { get; set; }

    ArtifactSpec IPropertyMergerItem.GetArtifactSpec(Guid artifactKind) => this.PropertyId == -1 ? (ArtifactSpec) null : new ArtifactSpec(artifactKind, this.PropertyId, 0, this.ItemDataspaceId);

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => this.PropertyValues;

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      this.PropertyValues = properties;
    }

    public int GetCachedSize() => 1000;
  }
}
