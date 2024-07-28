// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.PendingSet
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  public class PendingSet : ICacheable
  {
    internal int workspaceId;
    internal Guid OwnerTeamFoundationId;
    private string m_computer;
    private string m_ownerName;
    private string m_ownerDisplayName;
    private StreamingCollection<PendingChange> m_pendingChanges;
    private string m_name;
    private PendingSetType m_type;
    private Guid m_signature;

    [XmlAttribute("computer")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Computer
    {
      get => this.m_computer;
      set => this.m_computer = value;
    }

    [XmlAttribute("owner")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string OwnerName
    {
      get => this.m_ownerName;
      set => this.m_ownerName = value;
    }

    [XmlAttribute("ownerdisp")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string OwnerDisplayName
    {
      get => this.m_ownerDisplayName;
      set => this.m_ownerDisplayName = value;
    }

    [XmlAttribute("owneruniq")]
    [ClientProperty(ClientVisibility.Private)]
    public string OwnerUniqueName
    {
      get => this.m_ownerName;
      set
      {
        if (!string.IsNullOrEmpty(this.m_ownerName))
          return;
        this.m_ownerName = value;
      }
    }

    [XmlIgnore]
    public OwnershipState Ownership
    {
      get => (OwnershipState) this.OwnershipValue;
      set => this.OwnershipValue = (int) value;
    }

    [XmlAttribute("ownership")]
    [ClientProperty(ClientVisibility.Private)]
    public int OwnershipValue { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public StreamingCollection<PendingChange> PendingChanges
    {
      get => this.m_pendingChanges;
      set => this.m_pendingChanges = value;
    }

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("type")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public PendingSetType Type
    {
      get => this.m_type;
      set => this.m_type = value;
    }

    [XmlAttribute("signature")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public Guid PendingChangeSignature
    {
      get => this.m_signature;
      set => this.m_signature = value;
    }

    public int GetCachedSize()
    {
      int cachedSize = 250;
      if (this.Computer != null)
        cachedSize += this.Computer.Length;
      if (this.OwnerName != null)
        cachedSize += this.OwnerName.Length;
      if (this.OwnerDisplayName != null)
        cachedSize += this.OwnerDisplayName.Length;
      if (this.Name != null)
        cachedSize += this.Name.Length;
      return cachedSize;
    }
  }
}
