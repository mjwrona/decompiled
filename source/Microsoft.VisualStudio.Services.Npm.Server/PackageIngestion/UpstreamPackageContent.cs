// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion.UpstreamPackageContent
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Npm.Server.PackageIngestion
{
  public class UpstreamPackageContent
  {
    public UpstreamPackageContent()
    {
    }

    public UpstreamPackageContent(IEnumerable<UpstreamSourceInfo> sourceChain, Stream tarballStream)
    {
      this.SourceChain = sourceChain;
      this.TarballStream = tarballStream;
    }

    [DataMember]
    public IEnumerable<UpstreamSourceInfo> SourceChain { get; }

    [DataMember]
    public Stream TarballStream { get; }
  }
}
