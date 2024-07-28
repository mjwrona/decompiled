// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations.NuGetProblemPackagesAggregation
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.Server.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.Aggregations
{
  public class NuGetProblemPackagesAggregation : ProblemPackagesAggregation
  {
    public static readonly NuGetProblemPackagesAggregation V1 = new NuGetProblemPackagesAggregation();

    public NuGetProblemPackagesAggregation()
      : base(ProblemPackagesAggregation.MakeDefinition((IProtocol) Protocol.NuGet), (Func<string, IPackageName>) (x => (IPackageName) new VssNuGetPackageName(x)), (Func<string, IPackageVersion>) (x => (IPackageVersion) new VssNuGetPackageVersion(x)))
    {
    }
  }
}
