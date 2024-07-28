// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmProblemPackagesAggregation
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class NpmProblemPackagesAggregation : ProblemPackagesAggregation
  {
    public static readonly NpmProblemPackagesAggregation V1 = new NpmProblemPackagesAggregation();

    public NpmProblemPackagesAggregation()
      : base(ProblemPackagesAggregation.MakeDefinition((IProtocol) Protocol.npm), (Func<string, IPackageName>) (x => (IPackageName) new NpmPackageName(x)), NpmProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse ?? (NpmProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, IPackageVersion>(SemanticVersion.Parse)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }
  }
}
