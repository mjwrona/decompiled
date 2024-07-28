// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype.DeleteResultFromCommitEntryHandler
// Assembly: Microsoft.VisualStudio.Services.NuGet.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B6DD8F0-A807-4AA3-9A6E-1E5CDBF27B34
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.NuGet.Server.dll

using Microsoft.VisualStudio.Services.NuGet.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.NuGet.Server.BlobPrototype
{
  public class DeleteResultFromCommitEntryHandler : 
    IAsyncHandler<ICommitLogEntry, Package>,
    IHaveInputType<ICommitLogEntry>,
    IHaveOutputType<Package>
  {
    public Task<Package> Handle(ICommitLogEntry request)
    {
      IDeleteOperationData commitOperationData = (IDeleteOperationData) request.CommitOperationData;
      return Task.FromResult<Package>(new Package()
      {
        DeletedDate = new DateTime?(commitOperationData.DeletedDate),
        Version = commitOperationData.Identity.Version.NormalizedVersion,
        Name = commitOperationData.Identity.Name.DisplayName,
        Id = commitOperationData.Identity.Name.NormalizedName
      });
    }
  }
}
