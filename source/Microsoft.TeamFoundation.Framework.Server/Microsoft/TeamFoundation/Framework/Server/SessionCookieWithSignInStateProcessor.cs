// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SessionCookieWithSignInStateProcessor
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.LoopingLoginCheck;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class SessionCookieWithSignInStateProcessor : ISessionCookieProcessor
  {
    private const string DefaultSessionCookieLifetimeRegistryPath = "/Service/SessionTracking/CookieLifetime";
    private static readonly TimeSpan DefaultSessionCookieLifetime = new TimeSpan(365, 0, 0, 0);
    private const string DefaultSessionSignInTicksThresholdRegistryPath = "/Service/SessionTracking/SignInTicksThreshold";
    private static readonly long DefaultSessionSignInTicksThreshold = new TimeSpan(0, 0, 1, 0).Ticks;
    private const string DefaultSessionSignInTicksResetThresholdRegistryPath = "/Service/SessionTracking/SignInTicksResetThreshold";
    private static readonly long DefaultSessionSignInTicksResetThreshold = new TimeSpan(0, 0, 10, 0).Ticks;
    private const string DefaultSessionSignInCountThresholdRegistryPath = "/Service/SessionTracking/SignInCountThreshold";
    private static readonly int DefaultSessionSignInCountThreshold = 3;
    private const string Area = "Server";
    private const string Layer = "SessionCookieWithSignInStateProcessor";

    public void EnsureSessionCookieExists(IVssRequestContext requestContext)
    {
      try
      {
        TeamFoundationTracingService.TraceEnterRaw(15111000, "Server", nameof (SessionCookieWithSignInStateProcessor), nameof (EnsureSessionCookieExists), new Guid?(), new Guid?(), new Guid?(), new Guid?(), (string) null);
        if (requestContext == null)
        {
          TeamFoundationTracingService.TraceRaw(15111001, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), "SessionCookieProcessor invoked without a RequestContext.");
        }
        else
        {
          if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
            return;
          HttpRequestBase request = HttpContextFactory.Current?.Request;
          if (request == null)
          {
            requestContext.Trace(15111002, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), "SessionCookieProcessor invoked on a non-web request");
          }
          else
          {
            HttpCookie cookie = request.Cookies["VstsSession"];
            if (string.IsNullOrWhiteSpace(cookie?.Value))
            {
              VssSessionCookieWithSignInPerfCounters.CallsWithoutSessionCookiePerSecCounter.Increment();
              requestContext.Trace(15111003, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), "No {0} cookie was found. Writing a new one.", (object) "VstsSession");
              this.WriteNewSessionTrackingCookie(requestContext);
            }
            else
            {
              requestContext.TraceSerializedConditionally(15111004, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), false, "Session cookie: {0}", (object) cookie);
              try
              {
                VssSessionCookieWithSignInPerfCounters.CallsWithSessionCookiePerSecCounter.Increment();
                SessionTrackingState sessionTrackingState = JsonConvert.DeserializeObject<SessionTrackingState>(Uri.UnescapeDataString(cookie.Value));
                requestContext.RootContext.Items["VstsSession"] = (object) sessionTrackingState;
              }
              catch (JsonException ex)
              {
                this.WriteNewSessionTrackingCookie(requestContext);
                VssSessionCookieWithSignInPerfCounters.CallsWithMalformedSessionCookiePerSecCounter.Increment();
                requestContext.TraceException(15111005, TraceLevel.Warning, "Server", nameof (SessionCookieWithSignInStateProcessor), (Exception) new SessionCookieDeserializationException("The VstsSession failed to deserialize.", (Exception) ex));
              }
            }
          }
        }
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(15111006, "Server", nameof (SessionCookieWithSignInStateProcessor), ex);
      }
      finally
      {
        TeamFoundationTracingService.TraceLeaveRaw(15111000, "Server", nameof (SessionCookieWithSignInStateProcessor), nameof (EnsureSessionCookieExists));
      }
    }

    public void StartNewAuthenticationSession(IVssRequestContext requestContext)
    {
      Action<SessionTrackingState> validateOperation = (Action<SessionTrackingState>) (sessionState =>
      {
        VssSessionCookieWithSignInPerfCounters.CallsForLoopingSignInValidationPerSecCounter.Increment();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRegistryService service = vssRequestContext.GetService<IVssRegistryService>();
        long num1 = service.GetValue<long>(vssRequestContext, (RegistryQuery) "/Service/SessionTracking/SignInTicksThreshold", SessionCookieWithSignInStateProcessor.DefaultSessionSignInTicksThreshold);
        int num2 = service.GetValue<int>(vssRequestContext, (RegistryQuery) "/Service/SessionTracking/SignInCountThreshold", SessionCookieWithSignInStateProcessor.DefaultSessionSignInCountThreshold);
        Dictionary<string, SignInTrackingState> signInState = sessionState.SignInState;
        string currentRealm = this.GetCurrentRealm(requestContext);
        requestContext.Trace(15111024, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("Looping SigIn Validation of Realm {0} with signInTicksThreshold of {1} and signInCountThreshold of {2}", (object) currentRealm, (object) num1, (object) num2));
        string key = currentRealm;
        SignInTrackingState signInTrackingState;
        ref SignInTrackingState local = ref signInTrackingState;
        if (!signInState.TryGetValue(key, out local))
          return;
        long ticks = DateTime.UtcNow.Ticks;
        VssSessionCookieWithSignInPerfCounters.CallsForLoopingSignInValidationWithRealmPerSecCounter.Increment();
        requestContext.TraceDataConditionally(15111007, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("SignInState of Realm {0} at CurrentTicks {1}", (object) currentRealm, (object) ticks), (Func<object>) (() => (object) new
        {
          signInTrackingState = signInTrackingState
        }), nameof (StartNewAuthenticationSession));
        if (ticks - signInTrackingState.LastSignInTick >= num1 || signInTrackingState.SignInCount <= num2)
          return;
        VssSessionCookieWithSignInPerfCounters.CallsForLoopingSignInDetectionPerSecCounter.Increment();
        requestContext.TraceDataConditionally(15111008, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("Looping Login Detection in Realm {0} at CurrentTicks {1}", (object) currentRealm, (object) ticks), (Func<object>) (() => (object) new
        {
          signInTrackingState = signInTrackingState
        }), nameof (StartNewAuthenticationSession));
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.LoopingLogin.ThrowException"))
          return;
        try
        {
          requestContext.GetService<ILoopingLoginCheckService>().KillLoopingLogin(requestContext);
        }
        catch (IdentityLoopingLoginException ex)
        {
          requestContext.TraceException(15111028, "Server", nameof (SessionCookieWithSignInStateProcessor), (Exception) ex);
          requestContext.TraceDataConditionally(15111029, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("Looping Login Detection in Realm {0} at CurrentTicks {1}", (object) currentRealm, (object) ticks), (Func<object>) (() => (object) new
          {
            signInTrackingState = signInTrackingState
          }), nameof (StartNewAuthenticationSession));
          throw;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15111030, "Server", nameof (SessionCookieWithSignInStateProcessor), ex);
        }
      });
      Action<SessionTrackingState> sessionStateOperation = (Action<SessionTrackingState>) (sessionState =>
      {
        VssSessionCookieWithSignInPerfCounters.CallsForNewPendingAuthenticationPerSecCounter.Increment();
        Guid guid = Guid.NewGuid();
        if (sessionState.PendingAuthenticationSessionId != Guid.Empty)
          VssSessionCookieWithSignInPerfCounters.CallsForNewPendingAuthenticationWithPendingSessionPerSecCounter.Increment();
        requestContext.TraceSerializedConditionally(15111009, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), "Writing new cookie. Existing state: {0}. New pending auth session id: {1}", (object) sessionState, (object) guid);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        long num = vssRequestContext.GetService<IVssRegistryService>().GetValue<long>(vssRequestContext, (RegistryQuery) "/Service/SessionTracking/SignInTicksResetThreshold", SessionCookieWithSignInStateProcessor.DefaultSessionSignInTicksResetThreshold);
        string currentRealm = this.GetCurrentRealm(requestContext);
        DateTime utcNow = DateTime.UtcNow;
        long ticks1 = utcNow.Ticks;
        SignInTrackingState signInTrackingState;
        if (sessionState.SignInState.TryGetValue(currentRealm, out signInTrackingState) && ticks1 - signInTrackingState.LastSignInTick < num)
        {
          requestContext.TraceDataConditionally(15111027, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("Incrementing SignInCount as the user return to /signIn within {0} in Realm {1} ", (object) num, (object) currentRealm), (Func<object>) (() => (object) new
          {
            signInTrackingState = signInTrackingState
          }), nameof (StartNewAuthenticationSession));
          SignInTrackingState signInTrackingState1 = signInTrackingState;
          utcNow = DateTime.UtcNow;
          long ticks2 = utcNow.Ticks;
          signInTrackingState1.LastSignInTick = ticks2;
          ++signInTrackingState.SignInCount;
        }
        else
        {
          requestContext.TraceDataConditionally(15111027, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), string.Format("Resetting SignInCount as the user return to /signIn after {0} in Realm {1} ", (object) num, (object) currentRealm), (Func<object>) (() => (object) new
          {
            signInTrackingState = signInTrackingState
          }), nameof (StartNewAuthenticationSession));
          SignInTrackingState signInTrackingState2 = new SignInTrackingState();
          utcNow = DateTime.UtcNow;
          signInTrackingState2.LastSignInTick = utcNow.Ticks;
          signInTrackingState2.SignInCount = 1;
          signInTrackingState = signInTrackingState2;
        }
        requestContext.TraceDataConditionally(15111025, TraceLevel.Info, "Server", nameof (SessionCookieWithSignInStateProcessor), "Creating new pending authentication with SignInState of Realm " + currentRealm + " ", (Func<object>) (() => (object) new
        {
          signInTrackingState = signInTrackingState
        }), nameof (StartNewAuthenticationSession));
        sessionState.SignInState[currentRealm] = signInTrackingState;
        this.WriteNewSessionTrackingCookie(requestContext, new SessionTrackingState()
        {
          PersistentSessionId = sessionState.PersistentSessionId,
          PendingAuthenticationSessionId = guid,
          CurrentAuthenticationSessionId = sessionState.CurrentAuthenticationSessionId,
          SignInState = sessionState.SignInState
        });
      });
      this.ValidateSessionState(requestContext, validateOperation);
      this.ProcessSessionState(requestContext, sessionStateOperation);
    }

    public void CompleteNewAuthenticationSession(IVssRequestContext requestContext)
    {
      Action<SessionTrackingState> sessionStateOperation = (Action<SessionTrackingState>) (sessionState =>
      {
        VssSessionCookieWithSignInPerfCounters.CallsForCompletedAuthenticationsPerSecCounter.Increment();
        if (sessionState.PendingAuthenticationSessionId == Guid.Empty)
        {
          VssSessionCookieWithSignInPerfCounters.CallsForCompletedAuthenticationsWithoutPendingSessionPerSecCounter.Increment();
          Guid guid = Guid.NewGuid();
          SessionTrackingState SessionTrackingState = new SessionTrackingState()
          {
            PersistentSessionId = sessionState.PersistentSessionId,
            PendingAuthenticationSessionId = Guid.Empty,
            CurrentAuthenticationSessionId = guid,
            SignInState = sessionState.SignInState
          };
          requestContext.TraceSerializedConditionally(15111010, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), "Writing new cookie. Existing state: {0}. New current auth session id: {1}", (object) sessionState, (object) guid);
          this.WriteNewSessionTrackingCookie(requestContext, SessionTrackingState);
        }
        else
        {
          SessionTrackingState SessionTrackingState = new SessionTrackingState()
          {
            PersistentSessionId = sessionState.PersistentSessionId,
            PendingAuthenticationSessionId = Guid.Empty,
            CurrentAuthenticationSessionId = sessionState.PendingAuthenticationSessionId,
            SignInState = sessionState.SignInState
          };
          requestContext.TraceSerializedConditionally(15111011, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), "Writing new cookie. Existing state: {0}. New current auth session id: {1}", (object) sessionState, (object) sessionState.PendingAuthenticationSessionId);
          this.WriteNewSessionTrackingCookie(requestContext, SessionTrackingState);
        }
      });
      this.ProcessSessionState(requestContext, sessionStateOperation);
    }

    public void ClearCurrentAuthenticationSession(IVssRequestContext requestContext)
    {
      Action<SessionTrackingState> sessionStateOperation = (Action<SessionTrackingState>) (sessionState =>
      {
        requestContext.TraceSerializedConditionally(15111012, TraceLevel.Verbose, "Server", nameof (SessionCookieWithSignInStateProcessor), "Writing new cookie. Existing state: {0}. Erasing current auth session.", (object) sessionState);
        this.WriteNewSessionTrackingCookie(requestContext, new SessionTrackingState()
        {
          PersistentSessionId = sessionState.PersistentSessionId,
          PendingAuthenticationSessionId = sessionState.PendingAuthenticationSessionId,
          CurrentAuthenticationSessionId = Guid.Empty
        });
      });
      this.ProcessSessionState(requestContext, sessionStateOperation);
    }

    internal void ValidateSessionState(
      IVssRequestContext requestContext,
      Action<SessionTrackingState> validateOperation)
    {
      if (requestContext == null)
      {
        TeamFoundationTracingService.TraceRaw(15111013, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), "SessionCookieProcessor invoked without a RequestContext.");
      }
      else
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        try
        {
          SessionTrackingState castedValueOrDefault = requestContext.RootContext.Items.GetCastedValueOrDefault<string, SessionTrackingState>("VstsSession");
          if (castedValueOrDefault == null)
            requestContext.TraceException(15111014, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), (Exception) new MissingSessionCookieException("SessionTrackingState is missing from the requestContext Items even though it should have been added."));
          else
            validateOperation(castedValueOrDefault);
        }
        catch (IdentityLoopingLoginException ex)
        {
          requestContext.TraceException(15111015, "Server", nameof (SessionCookieWithSignInStateProcessor), (Exception) ex);
          throw;
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15111015, "Server", nameof (SessionCookieWithSignInStateProcessor), ex);
        }
        finally
        {
          requestContext.TraceLeave(15111016, "Server", nameof (SessionCookieWithSignInStateProcessor), nameof (ValidateSessionState));
        }
      }
    }

    internal string GetCurrentRealm(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return new Uri(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker)).Host;
    }

    internal void ProcessSessionState(
      IVssRequestContext requestContext,
      Action<SessionTrackingState> sessionStateOperation)
    {
      if (requestContext == null)
      {
        TeamFoundationTracingService.TraceRaw(15111017, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), "SessionCookieProcessor invoked without a RequestContext.");
      }
      else
      {
        if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
          return;
        try
        {
          requestContext.TraceEnter(15111018, "Server", nameof (SessionCookieWithSignInStateProcessor), nameof (ProcessSessionState));
          SessionTrackingState castedValueOrDefault = requestContext.RootContext.Items.GetCastedValueOrDefault<string, SessionTrackingState>("VstsSession");
          if (castedValueOrDefault == null)
            requestContext.TraceException(15111019, TraceLevel.Error, "Server", nameof (SessionCookieWithSignInStateProcessor), (Exception) new MissingSessionCookieException("SessionTrackingState is missing from the requestContext Items even though it should have been added."));
          else
            sessionStateOperation(castedValueOrDefault);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15111020, "Server", nameof (SessionCookieWithSignInStateProcessor), ex);
        }
        finally
        {
          requestContext.TraceLeave(15111021, "Server", nameof (SessionCookieWithSignInStateProcessor), nameof (ProcessSessionState));
        }
      }
    }

    internal void WriteNewSessionTrackingCookie(
      IVssRequestContext requestContext,
      SessionTrackingState SessionTrackingState)
    {
      if (requestContext.GetService<IInvalidRequestCompletionService>().SuppressRedirect(requestContext, HttpContextFactory.Current))
        requestContext.Trace(15111022, TraceLevel.Warning, "Server", nameof (SessionCookieWithSignInStateProcessor), "The request specifies that we cannot redirect the request to the IdP. This may not be a cookie-enabled request. Skipping writing cookie.");
      else if (SessionTrackingState == null)
      {
        requestContext.Trace(15111023, TraceLevel.Warning, "Server", nameof (SessionCookieWithSignInStateProcessor), "Null SessionTrackingState is passed in to write. Skipping writing cookie.");
      }
      else
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TimeSpan timeSpan = vssRequestContext.GetService<IVssRegistryService>().GetValue<TimeSpan>(vssRequestContext, (RegistryQuery) "/Service/SessionTracking/CookieLifetime", SessionCookieWithSignInStateProcessor.DefaultSessionCookieLifetime);
        string str = Uri.EscapeDataString(JsonConvert.SerializeObject((object) SessionTrackingState));
        ITeamFoundationAuthenticationServiceInternal authenticationServiceInternal = vssRequestContext.GetService<ITeamFoundationAuthenticationService>().AuthenticationServiceInternal();
        HttpCookie cookie = new HttpCookie("VstsSession", str)
        {
          Expires = DateTime.UtcNow + timeSpan,
          Domain = authenticationServiceInternal.GetCookieRootDomain(vssRequestContext),
          HttpOnly = true,
          Secure = true
        };
        CookieModifier.AddSameSiteNoneToCookie(requestContext, cookie);
        HttpContextFactory.Current?.Response?.Cookies?.Set(cookie);
        requestContext.RootContext.Items["VstsSession"] = (object) SessionTrackingState;
      }
    }

    internal void WriteNewSessionTrackingCookie(IVssRequestContext requestContext)
    {
      SessionTrackingState SessionTrackingState = new SessionTrackingState()
      {
        PersistentSessionId = Guid.NewGuid(),
        PendingAuthenticationSessionId = Guid.Empty,
        CurrentAuthenticationSessionId = Guid.Empty
      };
      this.WriteNewSessionTrackingCookie(requestContext, SessionTrackingState);
    }
  }
}
