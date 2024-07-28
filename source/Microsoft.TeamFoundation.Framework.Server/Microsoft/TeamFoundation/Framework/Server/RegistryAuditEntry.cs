// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RegistryAuditEntry
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Private)]
  public sealed class RegistryAuditEntry
  {
    private int m_changeIndex;
    private int m_changeTypeValue;
    private DateTime m_changeTime;
    private string m_identityName;
    private RegistryEntry m_registryEntryData;

    internal RegistryAuditEntry()
    {
    }

    internal RegistryAuditEntry(
      int changeIndex,
      RegistryChangeType changeType,
      DateTime changeTime,
      string identityName,
      RegistryEntry registryEntryData)
    {
      this.m_changeIndex = changeIndex;
      this.m_changeTypeValue = (int) changeType;
      this.m_changeTime = changeTime;
      this.m_identityName = identityName;
      this.m_registryEntryData = registryEntryData;
    }

    [XmlAttribute("index")]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public int ChangeIndex
    {
      get => this.m_changeIndex;
      set => this.m_changeIndex = value;
    }

    [XmlAttribute("ctype")]
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private)]
    public int ChangeTypeValue
    {
      get => this.m_changeTypeValue;
      set => this.m_changeTypeValue = value;
    }

    [XmlIgnore]
    public RegistryChangeType ChangeType
    {
      get => (RegistryChangeType) this.m_changeTypeValue;
      internal set => this.m_changeTypeValue = (int) value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime ChangeTime
    {
      get => this.m_changeTime;
      set => this.m_changeTime = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string IdentityName
    {
      get => this.m_identityName;
      set => this.m_identityName = value;
    }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public RegistryEntry Entry
    {
      get => this.m_registryEntryData;
      set => this.m_registryEntryData = value;
    }
  }
}
