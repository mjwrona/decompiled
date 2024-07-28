// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PropertyDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class PropertyDefinition
  {
    private int m_propertyId;
    private string m_name;
    internal static readonly Guid UninitializedDataspaceIdentifer = new Guid("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");
    private Guid m_dataspaceIdentifier = PropertyDefinition.UninitializedDataspaceIdentifer;

    [XmlAttribute("name")]
    public string Name
    {
      get => this.m_name;
      set => this.m_name = value;
    }

    [XmlAttribute("propertyId")]
    internal int PropertyId
    {
      get => this.m_propertyId;
      set => this.m_propertyId = value;
    }

    public Guid DataspaceIdentifier
    {
      get => this.m_dataspaceIdentifier;
      set => this.m_dataspaceIdentifier = value;
    }
  }
}
