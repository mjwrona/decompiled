// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.ProblemPackages.CargoProblemPackagesAggregation
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.NameDetails;
using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.VersionDetails;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Aggregations.ProblemPackages
{
  public class CargoProblemPackagesAggregation : ProblemPackagesAggregation
  {
    public static readonly CargoProblemPackagesAggregation V1 = new CargoProblemPackagesAggregation();

    public CargoProblemPackagesAggregation()
      : base(ProblemPackagesAggregation.MakeDefinition((IProtocol) Protocol.Cargo), CargoProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse ?? (CargoProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, IPackageName>(CargoPackageNameParser.Parse)), CargoProblemPackagesAggregation.\u003C\u003EO.\u003C1\u003E__Parse ?? (CargoProblemPackagesAggregation.\u003C\u003EO.\u003C1\u003E__Parse = new Func<string, IPackageVersion>(CargoPackageVersionParser.Parse)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }
  }
}
