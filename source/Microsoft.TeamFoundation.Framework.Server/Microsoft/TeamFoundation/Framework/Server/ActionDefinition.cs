// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ActionDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ActionDefinition
  {
    private string m_displayName;

    public ActionDefinition()
    {
    }

    public ActionDefinition(int bit, string name, string displayName)
      : this(Guid.Empty, bit, name, displayName)
    {
    }

    internal ActionDefinition(Guid namespaceId, int bit, string name, string displayName)
    {
      this.Bit = bit;
      this.Name = name;
      this.DisplayName = displayName;
      this.NamespaceId = namespaceId;
    }

    [XmlAttribute("bit")]
    [ClientProperty(ClientVisibility.Private)]
    public int Bit { get; set; }

    [XmlAttribute("name")]
    [ClientProperty(ClientVisibility.Private)]
    public string Name { get; set; }

    [XmlAttribute("displayName")]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName
    {
      get => string.IsNullOrEmpty(this.m_displayName) ? this.Name : this.m_displayName;
      set => this.m_displayName = value;
    }

    internal string InternalDisplayName => this.m_displayName;

    internal Guid NamespaceId { get; set; }
  }
}
