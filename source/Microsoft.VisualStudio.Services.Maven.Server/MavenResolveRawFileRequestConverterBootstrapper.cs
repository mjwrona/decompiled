// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.MavenResolveRawFileRequestConverterBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Maven.Server.Implementations;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using System;

namespace Microsoft.VisualStudio.Services.Maven.Server
{
  internal class MavenResolveRawFileRequestConverterBootstrapper : 
    IBootstrapper<IConverter<MavenRawFileRequest, MavenFileRequest>>
  {
    private readonly IVssRequestContext requestContext;

    public MavenResolveRawFileRequestConverterBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IConverter<MavenRawFileRequest, MavenFileRequest> Bootstrap()
    {
      MavenFilePathToRequestContextDataConveter delegatedConverter = new MavenFilePathToRequestContextDataConveter();
      return (IConverter<MavenRawFileRequest, MavenFileRequest>) new MavenResolveRawFileRequestConverter((IConverter<IMavenFilePath, object>) new PopulateRequestContextItemDelegatingConverter<IMavenFilePath, object>(this.requestContext, new Func<IMavenFilePath, object, string>(delegatedConverter.GetKey), (IConverter<IMavenFilePath, object>) delegatedConverter), this.requestContext.GetFeatureFlagFacade());
    }
  }
}
