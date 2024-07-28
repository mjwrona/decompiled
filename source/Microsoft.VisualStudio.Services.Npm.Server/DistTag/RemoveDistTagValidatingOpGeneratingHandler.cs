// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.DistTag.RemoveDistTagValidatingOpGeneratingHandler
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Npm.Server.CommitLog;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Npm.Server.DistTag
{
  public class RemoveDistTagValidatingOpGeneratingHandler : 
    IAsyncHandler<IPackageNameRequest<NpmPackageName, string>, NpmDistTagRemoveOperationData>,
    IHaveInputType<IPackageNameRequest<NpmPackageName, string>>,
    IHaveOutputType<NpmDistTagRemoveOperationData>
  {
    private readonly IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>> distTagProvider;

    public RemoveDistTagValidatingOpGeneratingHandler(
      IAsyncHandler<IPackageNameRequest<NpmPackageName>, IDictionary<string, string>> distTagProvider)
    {
      this.distTagProvider = distTagProvider;
    }

    public async Task<NpmDistTagRemoveOperationData> Handle(
      IPackageNameRequest<NpmPackageName, string> request)
    {
      string tag = request.AdditionalData;
      if (tag.Equals("latest"))
        throw new InvalidDistTagException(Resources.Error_LatestDistTagCannotBeDeleted());
      if (NpmVersionUtils.TryParseNpmPackageVersion(tag, out SemanticVersion _))
        throw new InvalidDistTagException(Resources.Error_DistTagIsSemanticVersion());
      if (!(await this.distTagProvider.Handle((IPackageNameRequest<NpmPackageName>) request)).ContainsKey(tag))
        throw new DistTagNotFoundException(Resources.Error_DistTagNotFound((object) tag, (object) request.PackageName.FullName));
      NpmDistTagRemoveOperationData removeOperationData = new NpmDistTagRemoveOperationData((IPackageName) request.PackageName, tag);
      tag = (string) null;
      return removeOperationData;
    }
  }
}
