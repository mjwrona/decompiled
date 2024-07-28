// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.OAuth2.S2SArrTokenService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.ConfigFramework;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server.OAuth2
{
  public class S2SArrTokenService : IS2SArrTokenService, IVssFrameworkService
  {
    private static readonly Uri _serverUrl = new Uri("https://dummy.url.com");
    private const string Area = "S2SArrTokenService";
    private const string Layer = "TokenRefresh";
    private static readonly Guid ArrServicePrincipalGuid = new Guid("00000073-0000-8888-8000-000000000000");
    private static readonly IConfigPrototype<bool> UseArrServiceForS2STokenPrototype = ConfigPrototype.Create<bool>("VisualStudio.Services.Identity.UseArrServiceForS2SToken", false);
    private readonly IConfigQueryable<bool> UseArrServiceForS2STokenConfig;
    private readonly TimeSpan _tillTokenExpiration;
    private readonly TimeSpan _renewalRetryCooldown;
    private readonly ITimerClock _clock;
    private volatile ExpiringIssuedToken _currentToken;

    public S2SArrTokenService()
      : this((ITimerClock) new UtcTimerClock())
    {
    }

    public S2SArrTokenService(ITimerClock clock)
      : this(clock, TimeSpan.FromMinutes(10.0), TimeSpan.FromSeconds(30.0), ConfigProxy.Create<bool>(S2SArrTokenService.UseArrServiceForS2STokenPrototype))
    {
    }

    public S2SArrTokenService(
      ITimerClock clock,
      TimeSpan tillTokenExpiration,
      TimeSpan renewalRetryCooldown)
      : this(clock, TimeSpan.FromMinutes(10.0), TimeSpan.FromSeconds(30.0), ConfigProxy.Create<bool>(S2SArrTokenService.UseArrServiceForS2STokenPrototype))
    {
    }

    public S2SArrTokenService(
      ITimerClock clock,
      TimeSpan tillTokenExpiration,
      TimeSpan renewalRetryCooldown,
      IConfigQueryable<bool> useArrServiceForS2STokenConfig)
    {
      this._clock = clock;
      this._tillTokenExpiration = tillTokenExpiration;
      this._renewalRetryCooldown = renewalRetryCooldown;
      this.UseArrServiceForS2STokenConfig = useArrServiceForS2STokenConfig;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.RenewToken(systemRequestContext);
      this.ScheduleSuccessRenewal(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<ITeamFoundationTaskService>().RemoveTask(systemRequestContext, new TeamFoundationTaskCallback(this.TokenRenewalJob));

    public ExpiringIssuedToken GetS2SApplicationToken(IVssRequestContext requestContext)
    {
      if (this.IsTokenExpired(this._currentToken))
        this.RenewToken(requestContext.To(TeamFoundationHostType.Deployment));
      return this._currentToken;
    }

    private void RenewToken(IVssRequestContext requestContext)
    {
      ExpiringIssuedToken currentToken = this._currentToken;
      IssuedTokenProvider tokenProvider = this.GetTokenProvider(requestContext);
      tokenProvider.InvalidateToken((IssuedToken) currentToken);
      if (!(tokenProvider.GetTokenAsync((IssuedToken) currentToken, CancellationToken.None).SyncResult<IssuedToken>() is ExpiringIssuedToken expiringIssuedToken))
        throw new InvalidOperationException("S2S Application Token Provider returned null token");
      this._currentToken = expiringIssuedToken;
    }

    private void TokenRenewalJob(IVssRequestContext requestContext, object args)
    {
      try
      {
        this.RenewToken(requestContext);
        this.ScheduleSuccessRenewal(requestContext);
      }
      catch (Exception ex)
      {
        this.ScheduleFailureRenewal(requestContext, ex);
      }
    }

    private IssuedTokenProvider GetTokenProvider(IVssRequestContext requestContext)
    {
      IOAuth2SettingsService service1 = requestContext.GetService<IOAuth2SettingsService>();
      IS2SCredentialsService service2 = requestContext.GetService<IS2SCredentialsService>();
      Guid? nullable = this.UseArrServiceForS2STokenConfig.QueryByCtx<bool>(requestContext) ? new Guid?(S2SArrTokenService.ArrServicePrincipalGuid) : service1.GetS2SAuthSettings(requestContext)?.PrimaryServicePrincipal;
      if (!nullable.HasValue)
        throw new InvalidOperationException("Could not get S2S principal");
      IVssRequestContext requestContext1 = requestContext;
      Guid servicePrincipal = nullable.Value;
      VssCredentials s2Scredentials = service2.GetS2SCredentials(requestContext1, servicePrincipal);
      if (s2Scredentials == null)
        throw new InvalidOperationException("Could not get S2S credentials");
      IssuedTokenProvider provider;
      if (!s2Scredentials.TryGetTokenProvider(S2SArrTokenService._serverUrl, out provider))
        throw new InvalidOperationException("Could not get S2S Application Token Provider");
      return provider;
    }

    private void ScheduleSuccessRenewal(IVssRequestContext requestContext)
    {
      DateTime startTime = this.GetTokenValidTo(this._currentToken) - this._tillTokenExpiration;
      requestContext.Trace(15240003, TraceLevel.Info, nameof (S2SArrTokenService), "TokenRefresh", "Renewed token successfully, token validTo: {0}, will refresh at: {1}", (object) this._currentToken.ValidTo, (object) startTime);
      requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.TokenRenewalJob), (object) null, startTime, 0));
    }

    private void ScheduleFailureRenewal(IVssRequestContext requestContext, Exception e)
    {
      requestContext.TraceException(15240005, nameof (S2SArrTokenService), "TokenRefresh", e);
      requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.TokenRenewalJob), (object) null, this._clock.Now.UtcDateTime + this._renewalRetryCooldown, 0));
    }

    private bool IsTokenExpired(ExpiringIssuedToken token) => this._clock.Now > (DateTimeOffset) (this.GetTokenValidTo(token) - TimeSpan.FromMinutes(1.0));

    private DateTime GetTokenValidTo(ExpiringIssuedToken token)
    {
      DateTime validTo = token.ValidTo;
      return validTo.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(validTo, DateTimeKind.Utc) : validTo;
    }
  }
}
