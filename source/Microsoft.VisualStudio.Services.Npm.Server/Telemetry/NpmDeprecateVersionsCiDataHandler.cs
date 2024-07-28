// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.NpmDeprecateVersionsCiDataHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.UpdateDocument;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.CommitLog.BaseOperations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public class NpmDeprecateVersionsCiDataHandler : 
    IAsyncHandler<(PackageMetadataRequest Request, ICommitOperationData Op), ICiData>,
    IHaveInputType<(PackageMetadataRequest Request, ICommitOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public NpmDeprecateVersionsCiDataHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageMetadataRequest Request, ICommitOperationData Op) input)
    {
      PackageMetadataRequest request = input.Request;
      switch (input.Op)
      {
        case IBatchCommitOperationData commitOperationData:
          return Task.FromResult<ICiData>((ICiData) NpmCiDataFactory.GetNpmBatchCiData(this.requestContext, request.Feed, NpmBatchOperationType.Deprecate, commitOperationData.Serialize<IBatchCommitOperationData>(), commitOperationData.Operations.Count<ICommitOperationData>()));
        case NpmDeprecateOperationData deprecateOperationData:
          return Task.FromResult<ICiData>((ICiData) NpmCiDataFactory.GetNpmDeprecateCiData(this.requestContext, request.Feed, deprecateOperationData.Identity.Name.NormalizedName, deprecateOperationData.Identity.Version.NormalizedVersion, long.MinValue, "feed"));
        default:
          throw new InvalidOperationException();
      }
    }
  }
}
