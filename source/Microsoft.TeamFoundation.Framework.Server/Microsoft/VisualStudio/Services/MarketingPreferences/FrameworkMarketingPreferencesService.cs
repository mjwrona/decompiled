// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MarketingPreferences.FrameworkMarketingPreferencesService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.MarketingPreferences.Client;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using System;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.MarketingPreferences
{
  internal class FrameworkMarketingPreferencesService : 
    IInternalMarketingPreferencesService,
    IMarketingPreferencesService,
    IVssFrameworkService
  {
    private const string c_commandIdentifier = "MarketingPreferences";
    private const string c_area = "MarketingPreferencesService";
    private const string c_layer = "FrameworkService";
    internal static TimeSpan CommandTimeout = TimeSpan.FromSeconds(10.0);
    private static readonly CommandSetter s_commandSetter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) "MarketingPreferences").AndCommandPropertiesDefaults(new CommandPropertiesSetter().WithExecutionTimeout(new TimeSpan(0, 0, 2)));

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public bool GetContactUserWithOffersSetting(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      Guid userId = FrameworkMarketingPreferencesService.CheckDescriptorForRequestingUser(requestContext, descriptor);
      return this.GetContactUserWithOffersSetting(requestContext, userId);
    }

    public bool GetContactUserWithOffersSetting(IVssRequestContext requestContext, Guid userId)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return false;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        UserHelper.TranslateUserId(requestContext, ref userId, out bool _);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IMarketingPreferencesService>().GetContactUserWithOffersSetting(vssRequestContext, userId);
      }
      requestContext.CheckDeploymentRequestContext();
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
      {
        MarketingPreferencesHttpClient client = requestContext.GetClient<MarketingPreferencesHttpClient>();
        return this.WrapCircuitBreaker<bool>(requestContext, (Func<bool>) (() => client.GetContactWithOffersAsync(userId.ToString("D"), cancellationToken: requestContext.CancellationToken).SyncResult<bool>()));
      }
    }

    public void SetContactUserWithOffersSetting(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      bool value)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      Guid userId = FrameworkMarketingPreferencesService.CheckDescriptorForRequestingUser(requestContext, descriptor);
      this.SetContactUserWithOffersSetting(requestContext, userId, value);
    }

    public void SetContactUserWithOffersSetting(
      IVssRequestContext requestContext,
      Guid userId,
      bool value)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        UserHelper.TranslateUserId(requestContext, ref userId, out bool _);
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IMarketingPreferencesService>().SetContactUserWithOffersSetting(vssRequestContext, userId, value);
      }
      else
      {
        requestContext.CheckDeploymentRequestContext();
        using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
        {
          MarketingPreferencesHttpClient client = requestContext.GetClient<MarketingPreferencesHttpClient>();
          this.WrapCircuitBreaker<object>(requestContext, (Func<object>) (() =>
          {
            client.SetContactWithOffersAsync(userId.ToString("D"), value, cancellationToken: requestContext.CancellationToken).SyncResult();
            return (object) null;
          }));
        }
      }
    }

    public Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences GetMarketingPreferences(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return new Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences()
        {
          VisualStudio = new bool?(false),
          VisualStudioSubscriptions = new bool?(false),
          AzureDevOps = new bool?(false)
        };
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        return vssRequestContext.GetService<IMarketingPreferencesService>().GetMarketingPreferences(vssRequestContext, descriptor);
      }
      requestContext.CheckDeploymentRequestContext();
      using (UserHelper.GetTemporaryUseDelegatedS2STokens(requestContext))
      {
        MarketingPreferencesHttpClient client = requestContext.GetClient<MarketingPreferencesHttpClient>();
        return this.WrapCircuitBreaker<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences>(requestContext, (Func<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences>) (() => client.GetMarketingPreferencesAsync((string) descriptor, cancellationToken: requestContext.CancellationToken).SyncResult<Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences>()));
      }
    }

    public void SetMarketingPreferences(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences preferences)
    {
      if (requestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        return;
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        vssRequestContext.GetService<IMarketingPreferencesService>().SetMarketingPreferences(vssRequestContext, descriptor, preferences);
      }
      else
      {
        MarketingPreferencesHttpClient client = requestContext.GetClient<MarketingPreferencesHttpClient>();
        this.WrapCircuitBreaker<object>(requestContext, (Func<object>) (() =>
        {
          client.SetMarketingPreferencesAsync(preferences, (string) descriptor, cancellationToken: requestContext.CancellationToken).SyncResult();
          return (object) null;
        }));
      }
    }

    private T WrapCircuitBreaker<T>(IVssRequestContext requestContext, Func<T> func)
    {
      CommandService<T> commandService = new CommandService<T>(requestContext, FrameworkMarketingPreferencesService.s_commandSetter, func);
      try
      {
        using (requestContext.CreateAsyncTimeOutScope(FrameworkMarketingPreferencesService.CommandTimeout))
          return commandService.Execute();
      }
      catch (TaskCanceledException ex)
      {
        requestContext.TraceException(156263542, "MarketingPreferencesService", "FrameworkService", (Exception) ex);
        throw new UserServiceUnavailableException(FrameworkResources.ProfileServiceUnavailableMessage((object) "MarketingPreferences"), (Exception) ex);
      }
      catch (CircuitBreakerShortCircuitException ex)
      {
        throw new UserServiceUnavailableException(FrameworkResources.ProfileServiceUnavailableMessage((object) "MarketingPreferences"), (Exception) ex);
      }
    }

    public void UpdateEmailAddress(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor,
      string mail)
    {
      throw new NotImplementedException();
    }

    public void UpdateEmailAddress(IVssRequestContext requestContext, Guid userId, string mail) => throw new NotImplementedException();

    private static Guid CheckDescriptorForRequestingUser(
      IVssRequestContext requestContext,
      SubjectDescriptor descriptor)
    {
      if (StringComparer.OrdinalIgnoreCase.Equals(descriptor.SubjectType, "vsid"))
        return new Guid(descriptor.Identifier.FromBase64StringNoPadding());
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      if (userIdentity.SubjectDescriptor != descriptor)
        throw new BadUserRequestException("This operation cannot be used to query users other than the authorized user.");
      return userIdentity.Id;
    }
  }
}
