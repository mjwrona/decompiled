// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.ThrowIfNotInRecycleBinHandler`1
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Exceptions;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class ThrowIfNotInRecycleBinHandler<TMetadataEntry> : 
    IAsyncHandler<TMetadataEntry, TMetadataEntry>,
    IHaveInputType<TMetadataEntry>,
    IHaveOutputType<TMetadataEntry>
    where TMetadataEntry : IMetadataEntry
  {
    public Task<TMetadataEntry> Handle(TMetadataEntry metadataEntry) => this.IsInRecycleBin((IMetadataEntry) metadataEntry) ? Task.FromResult<TMetadataEntry>(metadataEntry) : throw new PackageNotFoundException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_PackageVersionNotFoundInRecycleBin((object) metadataEntry.PackageIdentity.DisplayStringForMessages));

    private bool IsInRecycleBin(IMetadataEntry metadataEntry) => metadataEntry.DeletedDate.HasValue && !metadataEntry.PermanentDeletedDate.HasValue;
  }
}
