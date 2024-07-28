// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.PlatformDelegatedAuthorizationNotificationService
// Assembly: Microsoft.VisualStudio.Services.DelegatedAuthorization, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76926D67-5A10-414E-AFAB-34A210884CEB
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DelegatedAuthorization.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications;
using Microsoft.VisualStudio.Services.Notifications.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  internal class PlatformDelegatedAuthorizationNotificationService : 
    IDelegatedAuthorizationNotificationService,
    IVssFrameworkService
  {
    private const string SkipAutomatedExpirationRegistryRoot = "/Service/DelegatedAuthorization/AutomatedTokenNameFormats/";
    public const string PatRevokedEvent = "ms.vss-sps-notifications.pat-revoked-event";
    public const string SshRevokedEvent = "ms.vss-sps-notifications.ssh-revoked-event";
    private const string Area = "DelegatedAuthorization";
    private const string Layer = "PlatformDelegatedAuthorizationNotificationService";
    private readonly Guid AzureAppServiceClientId = new Guid("F4BE7C3C-B663-4F87-8ACB-EB9E391BC251");
    private RegistryEntryCollection m_skipAutomatedExpirationRegistryEntries;
    private Dictionary<string, string> m_scopeDictionary;

    public void ServiceStart(IVssRequestContext requestContext)
    {
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/DelegatedAuthorization/AutomatedTokenNameFormats/**");
      this.InitializeSkipAutomationRegistryEntries(requestContext);
      this.m_scopeDictionary = new Dictionary<string, string>();
      this.m_scopeDictionary.Add("app_token", "All scopes");
      foreach (AuthorizationScopeDefinition scope in AuthorizationScopeDefinitions.Default.scopes)
      {
        if (scope.availability == AuthorizationScopeAvailability.Public)
          this.m_scopeDictionary.Add(scope.scope, scope.title);
      }
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    private void OnRegistryChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      this.InitializeSkipAutomationRegistryEntries(requestContext);
    }

    private void InitializeSkipAutomationRegistryEntries(IVssRequestContext requestContext) => this.m_skipAutomatedExpirationRegistryEntries = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, new RegistryQuery("/Service/DelegatedAuthorization/AutomatedTokenNameFormats/..."));

    protected virtual string GetAccountUrl(
      IVssRequestContext requestContext,
      IList<Guid> targetAccounts,
      string urlPath)
    {
      return this.GetAccountUrl(requestContext, targetAccounts, urlPath, Guid.Empty);
    }

    protected virtual string GetAccountUrl(
      IVssRequestContext requestContext,
      IList<Guid> targetAccounts,
      string urlPath,
      Guid authorizationId)
    {
      Guid? nullable = new Guid?(targetAccounts.FirstOrDefault<Guid>());
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      if (nullable.HasValue)
      {
        Uri hostUri = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, nullable.Value, false, ServiceInstanceTypes.TFS);
        if (hostUri != (Uri) null)
        {
          string accountUrl = hostUri.ToString().TrimEnd('/') + urlPath;
          if (authorizationId != Guid.Empty)
            accountUrl = accountUrl + "?id=" + authorizationId.ToString();
          return accountUrl;
        }
      }
      return (string) null;
    }

    private string GetAccountUrl(IVssRequestContext requestContext)
    {
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, instanceId, false, ServiceInstanceTypes.TFS)?.ToString().TrimEnd('/');
    }

    private void SendPATRevokedNotification(
      IVssRequestContext requestContext,
      Guid userId,
      IEnumerable<SessionToken> sessionTokens,
      string adminDisplayName,
      string adminEmailAddress,
      string accountUrl)
    {
      Microsoft.VisualStudio.Services.TokenAdmin.Client.PatRevokedEvent patRevokedEvent = new Microsoft.VisualStudio.Services.TokenAdmin.Client.PatRevokedEvent();
      List<string> stringList = new List<string>();
      foreach (SessionToken sessionToken in sessionTokens)
        stringList.Add(HttpUtility.HtmlDecode(sessionToken.DisplayName));
      patRevokedEvent.PatNames = (IList<string>) stringList;
      patRevokedEvent.AccountUrl = accountUrl;
      patRevokedEvent.AdminEmailAddress = adminEmailAddress;
      patRevokedEvent.AdminEmailAddressMailTo = "mailto:" + adminEmailAddress;
      patRevokedEvent.AdminName = adminDisplayName;
      patRevokedEvent.ManagePatUrl = accountUrl + "/_details/security/tokens";
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.EventType = "ms.vss-sps-notifications.pat-revoked-event";
      theEvent.Data = (object) patRevokedEvent;
      theEvent.AddActor("user", userId);
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
    }

    private void SendSSHRevokedNotification(
      IVssRequestContext requestContext,
      Guid userId,
      IEnumerable<SessionToken> sessionTokens,
      string adminDisplayName,
      string adminEmailAddress,
      string accountUrl)
    {
      Microsoft.VisualStudio.Services.TokenAdmin.Client.SshRevokedEvent sshRevokedEvent = new Microsoft.VisualStudio.Services.TokenAdmin.Client.SshRevokedEvent();
      List<string> stringList = new List<string>();
      foreach (SessionToken sessionToken in sessionTokens)
        stringList.Add(HttpUtility.HtmlDecode(sessionToken.DisplayName));
      sshRevokedEvent.SshNames = (IList<string>) stringList;
      sshRevokedEvent.AccountUrl = accountUrl;
      sshRevokedEvent.AdminEmailAddress = adminEmailAddress;
      sshRevokedEvent.AdminEmailAddressMailTo = "mailto:" + adminEmailAddress;
      sshRevokedEvent.AdminName = adminDisplayName;
      sshRevokedEvent.ManageSshUrl = accountUrl + "/_details/security/keys";
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.EventType = "ms.vss-sps-notifications.ssh-revoked-event";
      theEvent.Data = (object) sshRevokedEvent;
      theEvent.AddActor("user", userId);
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
    }

    public void SendSSHKeyAddedNotification(
      IVssRequestContext requestContext,
      Guid userId,
      string keyName,
      string keyData,
      IList<Guid> targetAccounts)
    {
      SshKeyAddedEvent sshKeyAddedEvent = new SshKeyAddedEvent();
      sshKeyAddedEvent.KeyName = HttpUtility.HtmlDecode(keyName);
      byte[] buffer = Convert.FromBase64String(keyData);
      using (MD5 md5 = MD5.Create())
      {
        string stringLowerCase = HexConverter.ToStringLowerCase(md5.ComputeHash(buffer));
        sshKeyAddedEvent.KeySHA = stringLowerCase;
      }
      sshKeyAddedEvent.AccountUrl = (string) null;
      if (targetAccounts != null)
        sshKeyAddedEvent.AccountUrl = this.GetAccountUrl(requestContext, targetAccounts, "/_details/security/keys");
      string str = string.Empty;
      try
      {
        str = AfdClientIpHandler.ExtractIp(requestContext.RootContext.WebRequestContextInternal().HttpContext.Request, requestContext);
        string message = string.Format("Retrieved complete information for SSH Key Added notification. IP Address: {0}", (object) str);
        requestContext.Trace(1048038, TraceLevel.Info, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), message);
      }
      catch
      {
        requestContext.Trace(1048044, TraceLevel.Error, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), "Failed to get origin IP address for SSH Key Added notification.");
      }
      sshKeyAddedEvent.IPAddress = str;
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.ItemId = userId.ToString();
      theEvent.EventType = "ms.vss-sps-notifications.ssh-key-added-event";
      theEvent.Data = (object) sshKeyAddedEvent;
      theEvent.AddActor("user", userId);
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
    }

    public void SendPATAddedNotification(
      IVssRequestContext requestContext,
      Guid userId,
      Guid? clientId,
      string name,
      DateTime expiration,
      string scopes,
      IList<Guid> targetAccounts,
      Guid authorizationId)
    {
      if (clientId.HasValue)
      {
        Guid? nullable = clientId;
        Guid appServiceClientId = this.AzureAppServiceClientId;
        if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == appServiceClientId ? 1 : 0) : 1) : 0) != 0)
        {
          requestContext.Trace(1048033, TraceLevel.Info, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), "Skipping sending a notification for PAT " + name);
          return;
        }
      }
      foreach (RegistryEntry expirationRegistryEntry in this.m_skipAutomatedExpirationRegistryEntries)
      {
        if (name.StartsWith(expirationRegistryEntry.Value, StringComparison.InvariantCultureIgnoreCase))
        {
          requestContext.Trace(1048033, TraceLevel.Info, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), "Skipping sending a notification for PAT " + name);
          return;
        }
      }
      PATAddedEvent patAddedEvent = new PATAddedEvent();
      patAddedEvent.Name = HttpUtility.HtmlDecode(name);
      StringBuilder stringBuilder = new StringBuilder();
      string str1 = scopes;
      char[] chArray = new char[1]{ ' ' };
      foreach (string key in str1.Split(chArray))
      {
        string str2;
        try
        {
          str2 = this.m_scopeDictionary[key];
        }
        catch
        {
          requestContext.Trace(1048035, TraceLevel.Info, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), "Scope not found " + key);
          str2 = key;
        }
        stringBuilder.Append(str2 + "<br/>");
      }
      patAddedEvent.Scopes = stringBuilder.ToString();
      string str3 = string.Empty;
      string str4 = string.Empty;
      try
      {
        HttpRequestBase request = requestContext.RootContext.WebRequestContextInternal().HttpContext.Request;
        str3 = AfdClientIpHandler.ExtractIp(request, requestContext);
        str4 = OriginUserAgentHandler.ExtractUserAgent(request, requestContext);
        string message = string.Format("Retrieved complete information for PAT Added notification. IP Address: {0}, User Agent: {1}", (object) str3, (object) str4);
        requestContext.Trace(1048034, TraceLevel.Info, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), message);
      }
      catch
      {
        requestContext.Trace(1048046, TraceLevel.Error, "DelegatedAuthorization", nameof (PlatformDelegatedAuthorizationNotificationService), "Failed to get complete information for PAT Added notification.");
      }
      patAddedEvent.UserAgent = str4;
      patAddedEvent.IPAddress = str3;
      if (name.StartsWith("git: https://", StringComparison.InvariantCultureIgnoreCase))
        patAddedEvent.AdditionalInfo = DelegatedAuthorizationResources.GCMPATAdditionalInfo();
      else if (name.StartsWith("drop:https://", StringComparison.InvariantCultureIgnoreCase))
        patAddedEvent.AdditionalInfo = DelegatedAuthorizationResources.DropPATAdditionalInfo();
      else if (name.StartsWith("vpack:https://", StringComparison.InvariantCultureIgnoreCase))
        patAddedEvent.AdditionalInfo = DelegatedAuthorizationResources.VPackPATAdditionalInfo();
      string shortDatePattern = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
      UserPreferences userPreferences = requestContext.GetService<IUserPreferencesService>().GetUserPreferences(requestContext);
      string format = string.IsNullOrEmpty(userPreferences?.DatePattern) ? CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern : userPreferences.DatePattern;
      patAddedEvent.Expiration = expiration.ToString(format);
      patAddedEvent.AccountUrl = (string) null;
      if (targetAccounts != null)
        patAddedEvent.AccountUrl = this.GetAccountUrl(requestContext, targetAccounts, "/_details/security/tokens", authorizationId);
      VssNotificationEvent theEvent = new VssNotificationEvent();
      theEvent.ItemId = userId.ToString();
      theEvent.EventType = "ms.vss-sps-notifications.pat-added-event";
      theEvent.Data = (object) patAddedEvent;
      theEvent.AddActor("user", userId);
      requestContext.GetService<INotificationEventService>().PublishSystemEvent(requestContext, theEvent);
    }

    public void SendAuthorizationRevokedNotification(
      IVssRequestContext requestContext,
      IList<SessionToken> sessionTokens,
      bool isPublic)
    {
      if (sessionTokens == null)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity authenticatedIdentity = requestContext.GetAuthenticatedIdentity();
      string displayName = authenticatedIdentity.DisplayName;
      string property = authenticatedIdentity.GetProperty<string>("Account", string.Empty);
      string accountUrl = this.GetAccountUrl(requestContext);
      foreach (KeyValuePair<Guid, IEnumerable<SessionToken>> keyValuePair in sessionTokens.GroupBy<SessionToken, Guid>((Func<SessionToken, Guid>) (x => x.UserId)).ToDictionary<IGrouping<Guid, SessionToken>, Guid, IEnumerable<SessionToken>>((Func<IGrouping<Guid, SessionToken>, Guid>) (x => x.Key), (Func<IGrouping<Guid, SessionToken>, IEnumerable<SessionToken>>) (x => (IEnumerable<SessionToken>) x)))
      {
        if (isPublic)
          this.SendSSHRevokedNotification(requestContext, keyValuePair.Key, keyValuePair.Value, displayName, property, accountUrl);
        else
          this.SendPATRevokedNotification(requestContext, keyValuePair.Key, keyValuePair.Value, displayName, property, accountUrl);
      }
    }
  }
}
