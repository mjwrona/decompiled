// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion.DeleteResultFromCommitEntryHandler
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.PyPi.WebApi.Types.API;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.PyPi.Server.NonProtocolApis.DeletePackageVersion
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
