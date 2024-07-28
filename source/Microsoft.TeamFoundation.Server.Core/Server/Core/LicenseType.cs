// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.LicenseType
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class LicenseType : ILicenseType
  {
    [XmlAttribute("name")]
    public string Name { get; set; }

    [XmlAttribute("description")]
    public string Description { get; set; }

    [XmlAttribute("id")]
    public Guid Id { get; set; }

    [XmlAttribute("isdefault")]
    public bool IsDefault { get; set; }

    [XmlArray("Features")]
    public Guid[] Features { get; set; }
  }
}
