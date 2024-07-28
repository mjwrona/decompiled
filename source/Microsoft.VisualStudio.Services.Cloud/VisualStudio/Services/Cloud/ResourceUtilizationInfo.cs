// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ResourceUtilizationInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;
using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [XmlRoot("Resource")]
  public class ResourceUtilizationInfo
  {
    [XmlAttribute("resource")]
    public string Resource { get; set; }

    [XmlAttribute("namespaceId")]
    public Guid NamespaceId { get; set; }

    [XmlAttribute("windowSeconds")]
    public int WindowSeconds { get; set; }

    [XmlAttribute("key")]
    public string Key { get; set; }

    [XmlAttribute("result")]
    public long Result { get; set; }

    [XmlAttribute("dpImpact")]
    public double DPImpact { get; set; }

    [XmlAttribute("timeStamp")]
    public DateTime TimeStamp { get; set; }

    [XmlAttribute("state")]
    public int State { get; set; }

    [XmlAttribute("expiration")]
    public DateTime Expiration { get; set; }
  }
}
