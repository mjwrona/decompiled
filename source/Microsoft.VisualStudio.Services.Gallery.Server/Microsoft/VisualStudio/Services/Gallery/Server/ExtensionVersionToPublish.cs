// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionVersionToPublish
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  public class ExtensionVersionToPublish
  {
    public string PublisherName { get; set; }

    public string ExtensionName { get; set; }

    public string DisplayName { get; set; }

    public Guid OwnerId { get; set; }

    public PublishedExtensionFlags Flags { get; set; }

    public string ShortDescription { get; set; }

    public string LongDescription { get; set; }

    public PackageDetails PackageDetails { get; set; }

    public IEnumerable<ExtensionFile> Assets { get; set; }

    public IList<KeyValuePair<int, string>> Tags { get; set; }

    public IList<KeyValuePair<string, string>> MetadataValues { get; set; }

    public string Version { get; set; }

    public string TargetPlatform { get; set; }

    public string VersionDescription { get; set; }

    public ExtensionVersionFlags VersionFlags { get; set; }

    public Guid PrivateIdentityId { get; set; }

    public Guid ExtensionId { get; set; }

    public DateTime PublishedTime { get; set; }

    public string CdnDirectory { get; set; }

    public bool IsCdnEnabled { get; set; }

    public IList<int> ExtensionLcids { get; set; }

    public IEnumerable<InstallationTarget> InstallationTargets { get; set; }

    public ExtensionVersionToPublish()
    {
      this.CdnDirectory = (string) null;
      this.IsCdnEnabled = false;
      this.PackageDetails = (PackageDetails) null;
    }
  }
}
