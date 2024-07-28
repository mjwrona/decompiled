// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion.DeleteResultFromCommitEntryHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types.API;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.NonProtocolApis.DeletePackageVersion
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
        UnpublishedDate = new DateTime?(commitOperationData.DeletedDate),
        Version = commitOperationData.Identity.Version.NormalizedVersion,
        Name = commitOperationData.Identity.Name.DisplayName,
        Id = commitOperationData.Identity.Name.NormalizedName
      });
    }
  }
}
