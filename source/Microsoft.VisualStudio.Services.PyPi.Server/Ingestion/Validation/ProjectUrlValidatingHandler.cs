// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.ProjectUrlValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class ProjectUrlValidatingHandler : 
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IAsyncHandler<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>, NullResult>,
    IHaveInputType<IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata>>,
    IHaveOutputType<NullResult>
  {
    public Task<NullResult> Handle(
      IStorablePackageInfo<PyPiPackageIdentity, PyPiUploadedPackageMetadata> request)
    {
      bool flag = request.IngestionDirection == IngestionDirection.PullFromUpstream;
      if (request.ProtocolSpecificInfo.Metadata.ProjectUrls == null | flag)
        return Task.FromResult<NullResult>((NullResult) null);
      foreach (string projectUrl in (IEnumerable<string>) request.ProtocolSpecificInfo.Metadata.ProjectUrls)
      {
        string[] strArray = projectUrl.Split(',');
        if (strArray.Length != 2)
          throw new InvalidProjectUrlException(Resources.Error_InvalidProjectUrl((object) projectUrl));
        if (string.IsNullOrEmpty(strArray[0]) || strArray[0].Length > 32)
          throw new InvalidPackageException(Resources.Error_InvalidProjectUrlLabel((object) projectUrl[0]));
        if (string.IsNullOrEmpty(strArray[1]) || !UriValidator.IsValidUri(strArray[1]))
          throw new InvalidPackageException(Resources.Error_InvalidUrl((object) strArray[1]));
      }
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
