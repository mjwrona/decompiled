// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation.OnlyOneSourceDistValidatingHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.PyPi.Server.Metadata;
using System;
using System.Linq;
using System.Threading.Tasks;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.Ingestion.Validation
{
  public class OnlyOneSourceDistValidatingHandler : 
    IAsyncHandler<(IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry)>,
    IAsyncHandler<(IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry), 
    #nullable disable
    NullResult>,
    IHaveInputType<(
    #nullable enable
    IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry)>,
    IHaveOutputType<
    #nullable disable
    NullResult>
  {
    public 
    #nullable enable
    Task<NullResult> Handle(
      (IPyPiStorablePackageInfo Storable, IPyPiMetadataEntry? Entry) request)
    {
      IPyPiStorablePackageInfo storable = request.Storable;
      IPyPiResolvedMetadata metadata = storable.ProtocolSpecificInfo.Metadata;
      bool flag = storable.IngestionDirection == IngestionDirection.PullFromUpstream;
      IPyPiMetadataEntry entry = request.Entry;
      if (((entry == null ? 1 : (metadata.DistType != PyPiDistType.sdist ? 1 : 0)) | (flag ? 1 : 0)) != 0)
        return Task.FromResult<NullResult>((NullResult) null);
      PyPiPackageFile pyPiPackageFile = entry.PackageFiles.FirstOrDefault<PyPiPackageFile>((Func<PyPiPackageFile, bool>) (f => f.DistType == PyPiDistType.sdist && f.StorageId.IsLocal));
      if (pyPiPackageFile != null)
        throw new InvalidPackageException(Resources.Error_OnlyOneSourceDistribution((object) pyPiPackageFile.Path));
      return Task.FromResult<NullResult>((NullResult) null);
    }
  }
}
