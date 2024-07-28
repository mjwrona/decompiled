// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.CodeOnly.NpmUriBuilderFacade
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Npm.Server.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using System;

namespace Microsoft.VisualStudio.Services.Npm.Server.CodeOnly
{
  public class NpmUriBuilderFacade : INpmUriBuilder
  {
    private readonly IVssRequestContext requestContext;

    public NpmUriBuilderFacade(IVssRequestContext requestContext) => this.requestContext = requestContext ?? throw new ArgumentNullException(nameof (requestContext));

    public Uri GetPackageDownloadRedirectUri(
      IPackageNameRequest<NpmPackageName> packageNameRequest,
      string packageVersion)
    {
      return NpmUriBuilder.GetPackageDownloadRedirectUri(this.requestContext, packageNameRequest, packageVersion);
    }
  }
}
