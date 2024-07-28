// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.Change
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class Change : ICacheable, IPropertyMergerItem
  {
    private Item m_item = new Item();

    public Change() => this.ChangeType = ChangeType.None;

    [XmlAttribute("type")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "ChangeType")]
    public ChangeType ChangeTypeOld
    {
      get => PendingChange.GetLegacyChangeType(this.ChangeType);
      set => this.ChangeType |= value;
    }

    [XmlAttribute("typeEx")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, PropertyName = "TypeEx")]
    public int ChangeEx
    {
      get => (int) this.ChangeType;
      set => this.ChangeType |= (ChangeType) value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Item Item
    {
      get => this.m_item;
      set => this.m_item = value;
    }

    [XmlIgnore]
    public ChangeType ChangeType { get; internal set; }

    public int GetCachedSize() => 800;

    public ArtifactSpec GetArtifactSpec(Guid artifactKind) => this.m_item.GetArtifactSpec(artifactKind);

    [XmlIgnore]
    [Obsolete("Please use the Attributes property to access the change properties")]
    public StreamingCollection<PropertyValue> Properties
    {
      get => this.Attributes;
      set => this.Attributes = value;
    }

    [XmlIgnore]
    public StreamingCollection<PropertyValue> Attributes
    {
      get => this.m_item.Attributes;
      set => this.m_item.Attributes = value;
    }

    [XmlIgnore]
    public StreamingCollection<PropertyValue> PropertyValues
    {
      get => this.m_item.PropertyValues;
      set => this.m_item.PropertyValues = value;
    }

    StreamingCollection<PropertyValue> IPropertyMergerItem.GetProperties(Guid artifactKind) => ((IPropertyMergerItem) this.m_item).GetProperties(artifactKind);

    void IPropertyMergerItem.SetProperties(
      Guid artifactKind,
      StreamingCollection<PropertyValue> properties)
    {
      ((IPropertyMergerItem) this.m_item).SetProperties(artifactKind, properties);
    }

    [XmlIgnore]
    public int SequenceId
    {
      get => this.m_item.SequenceId;
      set => this.m_item.SequenceId = value;
    }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public List<MergeSource> MergeSources { get; set; }
  }
}
