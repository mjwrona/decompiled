// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Common.ServiceHooksTokenHelper
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E36C8A02-D97F-45E0-9F96-E7385D8CA092
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ServiceHooks.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Common
{
  public static class ServiceHooksTokenHelper
  {
    private const string c_strongBoxTokenDrawerNamePrefix = "Microsoft.VisualStudio.Services.ServiceHooks.Tokens";
    internal const string c_reuseTokensFeatureFlagName = "ServiceHooks.Notification.ReuseTokens";
    private static readonly string s_layer = typeof (ServiceHooksTokenHelper).Name;
    private static readonly string s_area = typeof (ServiceHooksTokenHelper).Namespace;

    public static NotificationToken GetToken(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification notification,
      SessionTokenConfigurationDescriptor tokenConfig,
      string defaultNewTokenName)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.ServiceHooks.WebApi.Notification>(notification, nameof (notification));
      ArgumentUtility.CheckForNull<SessionTokenConfigurationDescriptor>(tokenConfig, nameof (tokenConfig));
      ArgumentUtility.CheckForEmptyGuid(tokenConfig.ClientId, "tokenConfig.clientId");
      requestContext.TraceEnter(1051910, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, nameof (GetToken));
      NotificationToken notificationToken = (NotificationToken) null;
      NotificationToken token = (NotificationToken) null;
      string reason = (string) null;
      NotificationTokenIdentifier tokenId = new NotificationTokenIdentifier()
      {
        SubscriptionId = notification.SubscriptionId,
        SubscriberId = notification.SubscriberId,
        ClientAppId = tokenConfig.ClientId
      };
      if (requestContext.IsFeatureEnabled("ServiceHooks.Notification.ReuseTokens"))
        notificationToken = ServiceHooksTokenHelper.LoadToken(requestContext, tokenId);
      try
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[1]
        {
          tokenId.SubscriberId
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (identity == null)
          throw requestContext.TraceThrow<NotificationTokenAcquisitionException>(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, new NotificationTokenAcquisitionException(CommonResources.SessionTokenIdentityNotFound));
        using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginUserRequest(requestContext, requestContext.ServiceHost.InstanceId, identity.Descriptor))
        {
          IDelegatedAuthorizationService service = vssRequestContext.GetService<IDelegatedAuthorizationService>();
          Guid instanceId = requestContext.ServiceHost.InstanceId;
          if (notificationToken != null)
          {
            try
            {
              Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken sessionToken1 = service.GetSessionToken(vssRequestContext, notificationToken.AuthorizationId);
              if (sessionToken1.IsValid)
              {
                if (sessionToken1.ValidTo < DateTime.UtcNow.AddMinutes((double) tokenConfig.TimeoutMinutes))
                {
                  Microsoft.VisualStudio.Services.DelegatedAuthorization.SessionToken sessionToken2 = service.UpdateSessionToken(vssRequestContext, notificationToken.AuthorizationId, validTo: new DateTime?(DateTime.UtcNow.AddMinutes((double) (tokenConfig.TimeoutMinutes + 5))));
                  notificationToken.ExpirationDate = sessionToken2.ValidTo;
                }
                else
                  notificationToken.ExpirationDate = sessionToken1.ValidTo;
                token = notificationToken;
              }
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, ex);
            }
          }
          if (token == null)
          {
            string str1 = (string) null;
            if (tokenConfig.TokenNameBuilder != null)
            {
              try
              {
                str1 = tokenConfig.TokenNameBuilder(requestContext, notification);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, ex);
              }
            }
            if (string.IsNullOrWhiteSpace(str1))
              str1 = defaultNewTokenName;
            try
            {
              IDelegatedAuthorizationService authorizationService = service;
              IVssRequestContext requestContext1 = vssRequestContext;
              string str2 = str1;
              Guid? clientId = new Guid?(tokenConfig.ClientId);
              DateTime? nullable = new DateTime?(DateTime.UtcNow.AddMinutes((double) (tokenConfig.TimeoutMinutes + 5)));
              IList<Guid> guidList = (IList<Guid>) new Guid[1]
              {
                instanceId
              };
              SessionTokenType tokenType1 = tokenConfig.TokenType;
              string scope1 = tokenConfig.Scope;
              Guid? userId = new Guid?();
              string name = str2;
              DateTime? validTo = nullable;
              string scope2 = scope1;
              IList<Guid> targetAccounts = guidList;
              int tokenType2 = (int) tokenType1;
              Guid? authorizationId = new Guid?();
              Guid? accessId = new Guid?();
              SessionTokenResult sessionTokenResult = authorizationService.IssueSessionToken(requestContext1, clientId, userId, name, validTo, scope2, targetAccounts, (SessionTokenType) tokenType2, authorizationId: authorizationId, accessId: accessId);
              if (!sessionTokenResult.HasError)
                token = new NotificationToken(tokenId)
                {
                  AccessToken = sessionTokenResult.SessionToken.Token,
                  AuthorizationId = sessionTokenResult.SessionToken.AuthorizationId,
                  ExpirationDate = sessionTokenResult.SessionToken.ValidTo
                };
            }
            catch (SessionTokenCreateException ex)
            {
              requestContext.TraceCatch(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, (Exception) ex);
              reason = ex.Message;
            }
          }
        }
        if (token == null || !string.IsNullOrEmpty(reason))
          throw requestContext.TraceThrow<NotificationTokenAcquisitionException>(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, new NotificationTokenAcquisitionException(reason));
        if (requestContext.IsFeatureEnabled("ServiceHooks.Notification.ReuseTokens"))
          ServiceHooksTokenHelper.StoreToken(requestContext, token);
        return token;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051912, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1051914, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, string.Format("GetToken (Token Error: {0})", (object) (reason ?? string.Empty)));
      }
    }

    private static NotificationToken LoadToken(
      IVssRequestContext requestContext,
      NotificationTokenIdentifier tokenId)
    {
      NotificationToken notificationToken1 = (NotificationToken) null;
      ITeamFoundationStrongBoxService strongBoxService = ServiceHooksTokenHelper.GetStrongBoxService(ref requestContext);
      Guid strongBoxDrawerId = ServiceHooksTokenHelper.GetStrongBoxDrawerId(requestContext, tokenId.SubscriptionId);
      if (strongBoxDrawerId != Guid.Empty)
      {
        StrongBoxItemInfo itemInfo = strongBoxService.GetItemInfo(requestContext, strongBoxDrawerId, tokenId.LookupKey, false);
        if (itemInfo != null)
        {
          NotificationToken notificationToken2 = new NotificationToken(tokenId);
          notificationToken2.AccessToken = strongBoxService.GetString(requestContext, itemInfo);
          notificationToken2.AuthorizationId = Guid.Parse(itemInfo.CredentialName);
          DateTime? expirationDate = itemInfo.ExpirationDate;
          DateTime minValue;
          if (!expirationDate.HasValue)
          {
            minValue = DateTime.MinValue;
          }
          else
          {
            expirationDate = itemInfo.ExpirationDate;
            minValue = expirationDate.Value;
          }
          notificationToken2.ExpirationDate = minValue;
          notificationToken1 = notificationToken2;
        }
      }
      return notificationToken1;
    }

    private static void StoreToken(IVssRequestContext requestContext, NotificationToken token)
    {
      ArgumentUtility.CheckForNull<NotificationToken>(token, nameof (token));
      ITeamFoundationStrongBoxService strongBoxService = ServiceHooksTokenHelper.GetStrongBoxService(ref requestContext);
      Guid strongBoxDrawerId = ServiceHooksTokenHelper.GetStrongBoxDrawerId(requestContext, token.SubscriptionId, true);
      StrongBoxItemInfo strongBoxItemInfo = new StrongBoxItemInfo()
      {
        DrawerId = strongBoxDrawerId,
        LookupKey = token.LookupKey,
        CredentialName = token.AuthorizationId.ToString(),
        ExpirationDate = new DateTime?(token.ExpirationDate)
      };
      IVssRequestContext requestContext1 = requestContext;
      StrongBoxItemInfo info = strongBoxItemInfo;
      string accessToken = token.AccessToken;
      strongBoxService.AddString(requestContext1, info, accessToken);
    }

    public static void DeleteTokensForSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId)
    {
      ITeamFoundationStrongBoxService strongBoxService = ServiceHooksTokenHelper.GetStrongBoxService(ref requestContext);
      Guid strongBoxDrawerId = ServiceHooksTokenHelper.GetStrongBoxDrawerId(requestContext, subscriptionId);
      if (!(strongBoxDrawerId != Guid.Empty))
        return;
      try
      {
        List<NotificationToken> tokens = new List<NotificationToken>();
        strongBoxService.GetDrawerContents(requestContext, strongBoxDrawerId).ForEach((Action<StrongBoxItemInfo>) (item =>
        {
          NotificationToken notificationToken = new NotificationToken()
          {
            LookupKey = item.LookupKey,
            AuthorizationId = Guid.Parse(item.CredentialName)
          };
          if (!(notificationToken.SubscriberId != Guid.Empty) || !(notificationToken.AuthorizationId != Guid.Empty))
            return;
          tokens.Add(notificationToken);
        }));
        IDelegatedAuthorizationService delegatedAuthService = requestContext.GetService<IDelegatedAuthorizationService>();
        tokens.ForEach((Action<NotificationToken>) (token =>
        {
          try
          {
            delegatedAuthService.Revoke(requestContext, token.SubscriberId, token.AuthorizationId);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(1051920, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, ex);
          }
        }));
        strongBoxService.DeleteDrawer(requestContext, strongBoxDrawerId);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1051920, ServiceHooksTokenHelper.s_area, ServiceHooksTokenHelper.s_layer, ex);
      }
    }

    private static Guid GetStrongBoxDrawerId(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      bool createIfNotExist = false)
    {
      ITeamFoundationStrongBoxService strongBoxService = ServiceHooksTokenHelper.GetStrongBoxService(ref requestContext);
      string name = string.Format("{0}_{1}_{2}", (object) "Microsoft.VisualStudio.Services.ServiceHooks.Tokens", (object) requestContext.ServiceHost.InstanceId, (object) subscriptionId);
      Guid strongBoxDrawerId = strongBoxService.UnlockDrawer(requestContext, name, false);
      if (strongBoxDrawerId == Guid.Empty & createIfNotExist)
        strongBoxDrawerId = strongBoxService.CreateDrawer(requestContext, name);
      return strongBoxDrawerId;
    }

    private static ITeamFoundationStrongBoxService GetStrongBoxService(
      ref IVssRequestContext requestContext)
    {
      requestContext = requestContext.Elevate();
      return requestContext.GetService<ITeamFoundationStrongBoxService>();
    }
  }
}
