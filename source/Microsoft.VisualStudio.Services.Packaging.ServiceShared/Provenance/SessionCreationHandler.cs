// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance.SessionCreationHandler
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Patterns.Interfaces;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Provenance
{
  public class SessionCreationHandler : 
    ISessionCreationHandler,
    IAsyncHandler<RawSessionRequest, SessionResponse>,
    IHaveInputType<RawSessionRequest>,
    IHaveOutputType<SessionResponse>
  {
    private readonly ISessionMetadataServiceFacade sessionMetadataService;
    private readonly IFeedService feedService;
    private readonly IFeatureFlagService featureFlagService;
    private readonly IProtocolValidator protocolValidator;

    public SessionCreationHandler(
      ISessionMetadataServiceFacade sessionMetadataService,
      IFeedService feedService,
      IFeatureFlagService featureFlagService,
      IProtocolValidator protocolValidator)
    {
      this.sessionMetadataService = sessionMetadataService;
      this.feedService = feedService;
      this.featureFlagService = featureFlagService;
      this.protocolValidator = protocolValidator;
    }

    public async Task<SessionResponse> Handle(RawSessionRequest request)
    {
      string protocol = request.Protocol;
      if (!this.protocolValidator.IsValidProtocol(protocol))
        throw new VssPropertyValidationException("protocol", protocol + " is not a supported protocol type");
      this.feedService.GetFeed(request.ProjectId, request.SessionRequest.Feed);
      SessionId sessionId = SessionId.CreateNew(request.SessionRequest.Feed);
      await this.sessionMetadataService.StoreSessionMetadataAsync(new SessionKey(sessionId, protocol), request.SessionRequest);
      SessionResponse sessionResponse = new SessionResponse()
      {
        SessionId = sessionId.Id.ToString(),
        SessionName = sessionId.Name
      };
      sessionId = new SessionId();
      return sessionResponse;
    }
  }
}
