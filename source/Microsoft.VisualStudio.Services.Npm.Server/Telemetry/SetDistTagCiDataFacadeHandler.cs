// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.Telemetry.SetDistTagCiDataFacadeHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Telemetry;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.Telemetry
{
  public class SetDistTagCiDataFacadeHandler : 
    IAsyncHandler<(PackageRequest<NpmPackageIdentity, string> Request, NpmDistTagSetOperationData Op), ICiData>,
    IHaveInputType<(PackageRequest<NpmPackageIdentity, string> Request, NpmDistTagSetOperationData Op)>,
    IHaveOutputType<ICiData>
  {
    private readonly IVssRequestContext requestContext;

    public SetDistTagCiDataFacadeHandler(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public Task<ICiData> Handle(
      (PackageRequest<NpmPackageIdentity, string> Request, NpmDistTagSetOperationData Op) input)
    {
      PackageRequest<NpmPackageIdentity, string> request = input.Request;
      return Task.FromResult<ICiData>((ICiData) new DistTagSetCiData(this.requestContext, input.Request.Protocol, string.Empty, request.Feed, input.Request.PackageId.Name.FullName, input.Request.PackageId.Version.NormalizedVersion, request.AdditionalData));
    }
  }
}
