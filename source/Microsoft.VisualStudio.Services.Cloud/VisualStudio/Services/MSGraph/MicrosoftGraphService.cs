// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.MicrosoftGraphService
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Cloud;
using Microsoft.VisualStudio.Services.Cloud.Aad.Throttling;
using Microsoft.VisualStudio.Services.Cloud.AzureActiveDirectory.Tokens;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Graph;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.MSGraph.Client;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi.Exceptions;
using System;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.MSGraph
{
  internal class MicrosoftGraphService : IMicrosoftGraphService, IVssFrameworkService
  {
    private static readonly CommandPropertiesSetter CircuitBreakerSettings = new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 5));
    protected const string Area = "VisualStudio.Services.MSGraph";
    private const string Layer = "MicrosoftGraphService";

    void IVssFrameworkService.ServiceStart(IVssRequestContext context)
    {
      context.CheckHostedDeployment();
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().RegisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/MicrosoftGraph/...");
      this.UpdateSettings(context);
      this.ServiceStart(context);
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext context)
    {
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      vssRequestContext.GetService<IVssRegistryService>().UnregisterNotification(vssRequestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
      this.ServiceEnd(context);
    }

    protected virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    protected virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    InvitationResult IMicrosoftGraphService.InviteGuestUser(
      IVssRequestContext collectionContext,
      string userEmail,
      string userDisplayName,
      string redirectUrl,
      bool sendInvitationEmailFromAad)
    {
      try
      {
        collectionContext.TraceEnter(90000100, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "InviteGuestUser");
        collectionContext.TraceDataConditionally(90000101, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Input", (Func<object>) (() => (object) new
        {
          userEmail = userEmail,
          userDisplayName = userDisplayName,
          redirectUrl = redirectUrl,
          sendInvitationEmailFromAad = sendInvitationEmailFromAad
        }), "InviteGuestUser");
        this.ValidateInvitationContext(collectionContext);
        string tenantId;
        JwtSecurityToken accessToken = this.IsInvitationRequestAllowed(collectionContext, out tenantId) ? this.GetAccessToken(collectionContext, tenantId) : throw new AadGuestInvitationAccessException(HostingResources.TenantAccessDeniedError((object) collectionContext.UserContext.Identifier, (object) tenantId));
        InvitationResult result = this.ExecuteWithThrottlingAndCircuitBreaker<InvitationResult>(collectionContext, tenantId, "MicrosoftGraphService.InviteUser", (Func<InvitationResult>) (() =>
        {
          using (MicrosoftGraphHttpClient microsoftGraphHttpClient = new MicrosoftGraphHttpClient(accessToken.RawData, this.Settings.GraphApiResource, this.Settings.GraphApiVersion))
          {
            Invitation invitation = new Invitation(userEmail, userDisplayName, sendInvitationEmailFromAad, redirectUrl);
            return microsoftGraphHttpClient.CreateInvitation(invitation);
          }
        }));
        collectionContext.TraceDataConditionally(90000102, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Output", (Func<object>) (() => (object) new
        {
          userEmail = userEmail,
          userDisplayName = userDisplayName,
          redirectUrl = redirectUrl,
          sendInvitationEmailFromAad = sendInvitationEmailFromAad,
          result = result
        }), "InviteGuestUser");
        return result;
      }
      catch (Exception ex) when (!(ex is MicrosoftGraphServiceException))
      {
        throw new AadGuestInvitationFailedException(HostingResources.AadGuestInvitationFailed(), ex);
      }
      finally
      {
        collectionContext.TraceLeave(90000100, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "InviteGuestUser");
      }
    }

    private void ValidateInvitationContext(IVssRequestContext context)
    {
      context.CheckProjectCollectionRequestContext();
      if (context.IsSystemContext)
        return;
      ArgumentUtility.CheckForNull<IdentityDescriptor>(context.UserContext, "context.UserContext");
    }

    private bool IsInvitationRequestAllowed(IVssRequestContext context, out string tenantId)
    {
      tenantId = (string) null;
      Guid organizationTenantId = context.Elevate().GetOrganizationAadTenantId();
      bool result = false;
      if (organizationTenantId == Guid.Empty)
      {
        context.TraceDataConditionally(90000113, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Organization tenant ID is not a valid GUID", (Func<object>) (() => (object) new
        {
          organizationTenantId = organizationTenantId,
          result = result
        }), nameof (IsInvitationRequestAllowed));
        return result;
      }
      Guid userTenantId = AadIdentityHelper.GetIdentityTenantId(context.UserContext);
      if (userTenantId != organizationTenantId)
      {
        context.TraceDataConditionally(90000114, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "User and organization tenant ID do no match", (Func<object>) (() => (object) new
        {
          organizationTenantId = organizationTenantId,
          userTenantId = userTenantId,
          result = result
        }), nameof (IsInvitationRequestAllowed));
        return result;
      }
      tenantId = organizationTenantId.ToString();
      result = true;
      context.TraceDataConditionally(90000115, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Request allowed", (Func<object>) (() => (object) new
      {
        organizationTenantId = organizationTenantId,
        userTenantId = userTenantId,
        result = result
      }), nameof (IsInvitationRequestAllowed));
      return result;
    }

    protected JwtSecurityToken GetAccessToken(
      IVssRequestContext context,
      string tenantId,
      IdentityDescriptor identityDescriptor = null)
    {
      context.TraceEnter(90000120, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), nameof (GetAccessToken));
      try
      {
        string resource = this.Settings.GraphApiResource;
        context.TraceDataConditionally(90000121, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Input", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          identityDescriptor = identityDescriptor,
          resource = resource
        }), nameof (GetAccessToken));
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
        JwtSecurityToken token = vssRequestContext.GetService<IAadTokenService>().AcquireToken(vssRequestContext, resource, tenantId, identityDescriptor);
        context.TraceDataConditionally(90000122, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Output", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          identityDescriptor = identityDescriptor,
          resource = resource,
          tokenHash = TokenHasher.Hash(context, token.RawData),
          tokenClaims = token.Payload.Values
        }), nameof (GetAccessToken));
        return token;
      }
      finally
      {
        context.TraceLeave(90000120, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), nameof (GetAccessToken));
      }
    }

    protected TResponse ExecuteWithThrottlingAndCircuitBreaker<TResponse>(
      IVssRequestContext context,
      string tenantId,
      string commandKey,
      Func<TResponse> executeRequest)
    {
      context.TraceEnter(90000130, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), nameof (ExecuteWithThrottlingAndCircuitBreaker));
      try
      {
        if (context.IsFeatureEnabled("VisualStudio.Services.MSGraph.DisableThrottling"))
        {
          context.TraceDataConditionally(90000131, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Executing with circuit breaker only (skipping throttling) because feature is disabled", (Func<object>) (() => (object) new
          {
            tenantId = tenantId,
            commandKey = commandKey,
            feature = "VisualStudio.Services.MSGraph.DisableThrottling"
          }), nameof (ExecuteWithThrottlingAndCircuitBreaker));
          return ExecuteWithCircuitBreaker();
        }
        context.TraceDataConditionally(90000132, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Executing with throttling and circuit breaker", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          commandKey = commandKey
        }), nameof (ExecuteWithThrottlingAndCircuitBreaker));
        IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
        TResponse response = vssRequestContext.GetService<AadThrottlingService>().Execute<TResponse>(vssRequestContext, tenantId, AadServiceType.MicrosoftGraph, new Func<TResponse>(ExecuteWithCircuitBreaker), new Func<Exception, AadThrottleInfo>(GetThrottlingInfo));
        context.TraceDataConditionally(90000133, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Executed with throttling and circuit breaker", (Func<object>) (() => (object) new
        {
          tenantId = tenantId,
          commandKey = commandKey,
          response = response
        }), nameof (ExecuteWithThrottlingAndCircuitBreaker));
        return response;
      }
      catch (AadThrottlingException ex)
      {
        throw new MicrosoftGraphServiceNotAvailableException(ex.Message, (Exception) ex);
      }
      catch (MicrosoftGraphClientException ex) when (ex.StatusCode == (HttpStatusCode) 429 || ex.StatusCode == HttpStatusCode.ServiceUnavailable)
      {
        throw new MicrosoftGraphServiceNotAvailableException(ex.Message, (Exception) ex);
      }
      finally
      {
        context.TraceLeave(90000130, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), nameof (ExecuteWithThrottlingAndCircuitBreaker));
      }

      static TimeSpan? GetRetryAfterTimeSpan(RetryConditionHeaderValue retryAfter)
      {
        if (retryAfter == null)
          return new TimeSpan?();
        TimeSpan? retryAfterTimeSpan = retryAfter.Delta;
        if (retryAfterTimeSpan.HasValue)
        {
          retryAfterTimeSpan = retryAfter.Delta;
          return new TimeSpan?(retryAfterTimeSpan.Value);
        }
        DateTimeOffset? date = retryAfter.Date;
        if (date.HasValue)
        {
          date = retryAfter.Date;
          return new TimeSpan?(date.Value - DateTimeOffset.UtcNow);
        }
        retryAfterTimeSpan = new TimeSpan?();
        return retryAfterTimeSpan;
      }

      TResponse ExecuteWithCircuitBreaker()
      {
        context.TraceEnter(90000140, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "ExecuteWithThrottlingAndCircuitBreaker");
        try
        {
          TResponse response = default (TResponse);
          CommandService commandService = new CommandService(context, CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) commandKey).AndCommandPropertiesDefaults(MicrosoftGraphService.CircuitBreakerSettings), (Action) (() => response = executeRequest()));
          try
          {
            context.TraceDataConditionally(90000141, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Executing with circuit breaker", (Func<object>) (() => (object) new
            {
              tenantId = tenantId,
              commandKey = commandKey
            }), "ExecuteWithThrottlingAndCircuitBreaker");
            commandService.Execute();
            context.TraceDataConditionally(90000142, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Executed with circuit breaker", (Func<object>) (() => (object) new
            {
              tenantId = tenantId,
              commandKey = commandKey,
              response = response
            }), "ExecuteWithThrottlingAndCircuitBreaker");
          }
          catch (CircuitBreakerShortCircuitException ex)
          {
            throw new MicrosoftGraphServiceNotAvailableException(ex.Message, (Exception) ex);
          }
          return response;
        }
        finally
        {
          context.TraceLeave(90000140, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "ExecuteWithThrottlingAndCircuitBreaker");
        }
      }

      AadThrottleInfo GetThrottlingInfo(Exception exception)
      {
        context.TraceEnter(90000150, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "ExecuteWithThrottlingAndCircuitBreaker");
        try
        {
          context.TraceDataConditionally(90000151, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Checking for throttling", (Func<object>) (() => (object) new
          {
            exception = exception
          }), "ExecuteWithThrottlingAndCircuitBreaker");
          int innerExceptionCounter = 0;
          AadThrottleInfo throttlingInfo = (AadThrottleInfo) null;
          for (; exception != null && innerExceptionCounter < 100; exception = exception.InnerException)
          {
            if (exception is MicrosoftGraphClientException graphClientException && graphClientException.StatusCode == (HttpStatusCode) 429)
            {
              TimeSpan? retryAfterTimeSpan = GetRetryAfterTimeSpan(graphClientException.RetryAfter);
              throttlingInfo = new AadThrottleInfo()
              {
                IsThrottled = true,
                RetryAfter = retryAfterTimeSpan
              };
              context.TraceDataConditionally(90000152, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Returning positive throttling info", (Func<object>) (() => (object) new
              {
                exception = exception,
                innerExceptionCounter = innerExceptionCounter,
                throttlingInfo = throttlingInfo
              }), "ExecuteWithThrottlingAndCircuitBreaker");
              return throttlingInfo;
            }
            ++innerExceptionCounter;
          }
          throttlingInfo = new AadThrottleInfo()
          {
            IsThrottled = false
          };
          context.TraceDataConditionally(90000153, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Returning negative throttling info", (Func<object>) (() => (object) new
          {
            exception = exception,
            innerExceptionCounter = innerExceptionCounter,
            throttlingInfo = throttlingInfo
          }), "ExecuteWithThrottlingAndCircuitBreaker");
          return throttlingInfo;
        }
        finally
        {
          context.TraceLeave(90000150, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "ExecuteWithThrottlingAndCircuitBreaker");
        }
      }
    }

    private void OnRegistryChanged(
      IVssRequestContext context,
      RegistryEntryCollection changedEntries)
    {
      this.UpdateSettings(context);
    }

    private void UpdateSettings(IVssRequestContext context)
    {
      IVssRequestContext context1 = context.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = context1.GetService<IVssRegistryService>();
      MicrosoftGraphService.ServiceSettings settings = new MicrosoftGraphService.ServiceSettings()
      {
        GraphApiResource = MicrosoftGraphService.GetRegistryValue(context1, service, "/Service/MicrosoftGraph/GraphApiResource"),
        GraphApiVersion = MicrosoftGraphService.GetRegistryValue(context1, service, "/Service/MicrosoftGraph/GraphApiVersion")
      };
      context.TraceDataConditionally(90000162, TraceLevel.Verbose, "VisualStudio.Services.MSGraph", nameof (MicrosoftGraphService), "Updated settings", (Func<object>) (() => (object) new
      {
        @new = settings,
        old = this.Settings
      }), nameof (UpdateSettings));
      this.Settings = settings;
    }

    private static string GetRegistryValue(
      IVssRequestContext context,
      IVssRegistryService registrService,
      string key,
      string @default = null)
    {
      string registryValue = registrService.GetValue<string>(context, (RegistryQuery) key, (string) null);
      if (string.IsNullOrWhiteSpace(registryValue))
        registryValue = !string.IsNullOrEmpty(@default) ? @default : throw new MicrosoftGraphServiceConfigurationException(HostingResources.NoRegistryValue((object) key));
      return registryValue;
    }

    protected MicrosoftGraphService.ServiceSettings Settings { get; private set; }

    protected class ServiceSettings
    {
      internal string GraphApiResource { get; set; }

      internal string GraphApiVersion { get; set; }
    }

    private static class Commands
    {
      internal const string InviteUser = "MicrosoftGraphService.InviteUser";
    }

    private static class Features
    {
      internal const string DisableThrottling = "VisualStudio.Services.MSGraph.DisableThrottling";
    }
  }
}
