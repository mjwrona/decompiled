// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.PublishExtensionJobData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using System.Xml.Serialization;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class PublishExtensionJobData
  {
    [XmlAttribute("publisherName")]
    public string PublisherName { get; set; }

    [XmlAttribute("extensionName")]
    public string ExtensionName { get; set; }

    [XmlAttribute("onlyUpdateForNewVersion")]
    public bool OnlyUpdateForNewVersion { get; set; }

    [XmlAttribute("blockPreviewToPaidUpgrade")]
    public bool BlockPreviewToPaidUpgrade { get; set; }
  }
}
