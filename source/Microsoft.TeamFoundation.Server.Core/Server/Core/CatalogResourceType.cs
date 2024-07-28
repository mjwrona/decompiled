// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogResourceType
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Internal)]
  public class CatalogResourceType
  {
    public CatalogResourceType()
    {
    }

    public CatalogResourceType(Guid identifier, string displayName, string description)
    {
      this.Identifier = identifier;
      this.DisplayName = displayName;
      this.Description = description;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public Guid Identifier { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string DisplayName { get; set; }

    [ClientProperty(ClientVisibility.Private)]
    public string Description { get; set; }
  }
}
