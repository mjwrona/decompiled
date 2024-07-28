// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.RequirementSpecsValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using Microsoft.VisualStudio.Services.PyPi.Server.RequirementSpecs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class RequirementSpecsValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      IReadOnlyList<RequirementSpec> requiresDistributions = request.ProtocolSpecificInfo.Metadata.RequiresDistributions;
      if (requiresDistributions != null && requiresDistributions.Any<RequirementSpec>((Func<RequirementSpec, bool>) (x => x.UrlSpec != null)))
        throw new InvalidPackageException(Resources.Error_UrlsNotAllowedInRequirementSpecs((object) "Requires-Dist"));
      IReadOnlyList<RequirementSpec> providesDistributions = request.ProtocolSpecificInfo.Metadata.ProvidesDistributions;
      if (providesDistributions != null && providesDistributions.Any<RequirementSpec>((Func<RequirementSpec, bool>) (x => x.UrlSpec != null)))
        throw new InvalidPackageException(Resources.Error_UrlsNotAllowedInRequirementSpecs((object) "Provides-Dist"));
      IReadOnlyList<RequirementSpec> obsoletesDistributions = request.ProtocolSpecificInfo.Metadata.ObsoletesDistributions;
      if (obsoletesDistributions != null && obsoletesDistributions.Any<RequirementSpec>((Func<RequirementSpec, bool>) (x => x.UrlSpec != null)))
        throw new InvalidPackageException(Resources.Error_UrlsNotAllowedInRequirementSpecs((object) "Obsoletes-Dist"));
      return NullResult.NullTask;
    }
  }
}
