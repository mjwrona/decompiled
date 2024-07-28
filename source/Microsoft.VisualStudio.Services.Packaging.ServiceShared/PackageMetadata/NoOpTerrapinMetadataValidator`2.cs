// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.NoOpTerrapinMetadataValidator`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Ingestion.CentralFeedServices;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public class NoOpTerrapinMetadataValidator<TPackageName, TPackageVersion> : 
    ITerrapinMetadataValidator<TPackageName, TPackageVersion>
    where TPackageName : IPackageName
    where TPackageVersion : IPackageVersion
  {
    public Task<Dictionary<TPackageVersion, TerrapinIngestionValidationReason>> GetTerrapinData(
      TPackageName packageName,
      IEnumerable<UpstreamVersionInstance<TPackageVersion>> entries)
    {
      return Task.FromResult<Dictionary<TPackageVersion, TerrapinIngestionValidationReason>>(new Dictionary<TPackageVersion, TerrapinIngestionValidationReason>());
    }
  }
}
