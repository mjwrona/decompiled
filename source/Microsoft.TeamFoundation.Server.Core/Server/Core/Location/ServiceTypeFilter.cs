// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.Location.ServiceTypeFilter
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core.Location
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class ServiceTypeFilter
  {
    [XmlAttribute]
    public string ServiceType { get; set; }

    [XmlAttribute]
    public Guid Identifier { get; set; }
  }
}
