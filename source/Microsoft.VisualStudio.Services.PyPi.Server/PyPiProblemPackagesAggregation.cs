// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PyPiProblemPackagesAggregation
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Aggregations.ProblemPackages;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System;

namespace Microsoft.VisualStudio.Services.PyPi.Server
{
  public class PyPiProblemPackagesAggregation : ProblemPackagesAggregation
  {
    public static readonly PyPiProblemPackagesAggregation V1 = new PyPiProblemPackagesAggregation();

    public PyPiProblemPackagesAggregation()
      : base(ProblemPackagesAggregation.MakeDefinition((IProtocol) Protocol.PyPi), (Func<string, IPackageName>) (x => (IPackageName) new PyPiPackageName(x)), PyPiProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse ?? (PyPiProblemPackagesAggregation.\u003C\u003EO.\u003C0\u003E__Parse = new Func<string, IPackageVersion>(PyPiPackageVersionParser.Parse)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }
  }
}
