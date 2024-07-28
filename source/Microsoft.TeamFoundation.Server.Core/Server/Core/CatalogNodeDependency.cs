// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.CatalogNodeDependency
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  [ClassVisibility(ClientVisibility.Internal)]
  public class CatalogNodeDependency
  {
    [XmlAttribute]
    public string FullPath { get; set; }

    [XmlAttribute]
    public string AssociationKey { get; set; }

    [XmlAttribute]
    public string RequiredNodeFullPath { get; set; }

    [XmlAttribute]
    public bool IsSingleton { get; set; }
  }
}
