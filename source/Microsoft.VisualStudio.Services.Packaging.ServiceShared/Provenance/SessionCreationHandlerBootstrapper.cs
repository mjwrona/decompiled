// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionCreationHandlerBootstrapper
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Framework;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionCreationHandlerBootstrapper : 
    IBootstrapper<IAsyncHandler<RawSessionRequest, SessionResponse>>
  {
    private readonly IVssRequestContext requestContext;

    public SessionCreationHandlerBootstrapper(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public IAsyncHandler<RawSessionRequest, SessionResponse> Bootstrap() => (IAsyncHandler<RawSessionRequest, SessionResponse>) new SessionCreationHandler((ISessionMetadataServiceFacade) new SessionMetadataServiceFacade(this.requestContext), (IFeedService) new FeedServiceFacade(this.requestContext), this.requestContext.GetFeatureFlagFacade(), (IProtocolValidator) new ProtocolValidator());
  }
}
