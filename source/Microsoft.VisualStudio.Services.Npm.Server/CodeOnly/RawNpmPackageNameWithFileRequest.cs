// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.RawNpmPackageNameWithFileRequest
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class RawNpmPackageNameWithFileRequest : FeedRequest
  {
    public RawNpmPackageNameWithFileRequest(
      IFeedRequest feedRequest,
      string packageScope,
      string unscopedPackageName,
      string fileName)
      : base(feedRequest)
    {
      this.PackageScope = packageScope;
      this.UnscopedPackageName = unscopedPackageName ?? throw new ArgumentNullException(nameof (unscopedPackageName));
      this.FileName = fileName ?? throw new ArgumentNullException(nameof (fileName));
    }

    public string PackageScope { get; }

    public string UnscopedPackageName { get; }

    public string FileName { get; }
  }
}
