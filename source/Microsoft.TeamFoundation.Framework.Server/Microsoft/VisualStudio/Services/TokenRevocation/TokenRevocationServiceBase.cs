// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenRevocation.TokenRevocationServiceBase
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.VisualStudio.Services.TokenRevocation
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class TokenRevocationServiceBase : ITokenRevocationService, IVssFrameworkService
  {
    protected const string TraceArea = "TokenRevocation";
    protected const string TraceLayer = "Service";
    protected TokenRevocationRuleCache LocalCache;
    private int CacheCounter;
    private const string prefix = "vso:";
    private ILockName CacheLockName;
    private readonly Guid SqlNotificationEventClass = SqlNotificationEventClasses.TokenRevocationRuleTableChanged;
    private const int DefaultExpirationHours = 24;
    private const string CheckTokenHostMappingRules = "VisualStudio.Services.Authentication.CheckTokenHostMappingRules";
    private const string CheckTokenCollectionHostRevocationRules = "VisualStudio.Services.Authentication.CheckTokenCollectionHostRevocationRules";
    private Guid ServiceHostInstanceId;
    private INotificationRegistration m_revocationServiceRegistration;

    protected virtual string ExpirationRegistrationPath => "/TokenRevocation.Platform.ExpirationHours";

    public void ServiceStart(IVssRequestContext requestContext)
    {
      this.LocalCache = new TokenRevocationRuleCache(TimeSpan.FromHours((double) this.GetExpirationHours(requestContext)));
      this.CacheLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}.{1}", (object) nameof (TokenRevocationServiceBase), (object) requestContext.ServiceHost.InstanceId));
      ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
      this.ServiceHostInstanceId = requestContext.ServiceHost.InstanceId;
      this.m_revocationServiceRegistration = service.CreateRegistration(requestContext, "Default", this.SqlNotificationEventClass, new SqlNotificationCallback(this.OnTokenRevocationServiceChangedNotification), true, false);
    }

    public void ServiceEnd(IVssRequestContext requestContext) => this.m_revocationServiceRegistration.Unregister(requestContext);

    protected virtual int GetExpirationHours(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) (FrameworkServerConstants.RegistrationRoot + this.ExpirationRegistrationPath), 24);

    public TokenRevocationRule[] GetAllRules(IVssRequestContext requestContext)
    {
      try
      {
        this.ValidateServiceHost(requestContext);
        requestContext.TraceEnter(1049082, "TokenRevocation", "Service", nameof (GetAllRules));
        TokenRevocationRule[] rules = this.LocalCache.GetAll(requestContext);
        if (rules == null)
        {
          int cacheCounter;
          do
          {
            IVssLockManager lockManager = requestContext.LockManager;
            cacheCounter = this.CacheCounter;
            rules = this.RetrieveAllRules(requestContext);
            ILockName cacheLockName = this.CacheLockName;
            using (lockManager.Lock(cacheLockName))
            {
              if (cacheCounter == this.CacheCounter)
                this.LocalCache.Set(requestContext, rules);
            }
          }
          while (cacheCounter != this.CacheCounter);
        }
        return rules;
      }
      finally
      {
        requestContext.TraceLeave(1049084, "TokenRevocation", "Service", nameof (GetAllRules));
      }
    }

    protected void ValidateServiceHost(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.CheckServiceHostId(this.ServiceHostInstanceId, (IVssFrameworkService) this);
    }

    protected internal abstract TokenRevocationRule[] RetrieveAllRules(
      IVssRequestContext requestContext);

    public abstract IList<Guid> CreateRules(
      IVssRequestContext requestContext,
      IEnumerable<TokenRevocationRule> rules);

    public abstract void DeleteRule(IVssRequestContext requestContext, Guid ruleId);

    public TokenRevocationRule[] GetHostTranslationRules(IVssRequestContext requestContext) => ((IEnumerable<TokenRevocationRule>) this.GetAllRules(requestContext)).Where<TokenRevocationRule>((Func<TokenRevocationRule, bool>) (x => x.RuleType == TokenRevocationRuleType.ActivatedParentHost)).ToArray<TokenRevocationRule>();

    public virtual IList<Guid> GetHostidsFromAudiences(IAuthCredential authCredential)
    {
      List<Guid> hostidsFromAudiences = new List<Guid>();
      string input1 = authCredential.AuthenticatedPrincipal?.FindFirst("aud")?.Value;
      if (input1 != null)
      {
        if (input1.Contains("vso:"))
        {
          string str = input1;
          char[] chArray = new char[1]{ '|' };
          foreach (string input2 in ((IEnumerable<string>) str.Split(chArray)).Where<string>((Func<string, bool>) (x => x.StartsWith("vso:"))).Select<string, string>((Func<string, string>) (x => x.Substring("vso:".Length))).ToList<string>())
          {
            Guid empty = Guid.Empty;
            ref Guid local = ref empty;
            if (Guid.TryParse(input2, out local) && empty != Guid.Empty)
              hostidsFromAudiences.Add(empty);
          }
        }
        else
        {
          Guid result;
          if (Guid.TryParse(input1, out result))
            hostidsFromAudiences.Add(result);
        }
      }
      return (IList<Guid>) hostidsFromAudiences;
    }

    public bool IsValid(
      IVssRequestContext requestContext,
      IAuthCredential authCredential,
      Guid userId,
      out Guid failingRuleId,
      bool ignoreOrgHostCheck = false)
    {
      this.ValidateServiceHost(requestContext);
      failingRuleId = Guid.Empty;
      switch (authCredential)
      {
        case OAuth2AuthCredential _:
        case FederatedAuthCredential _:
          if (userId == Guid.Empty)
            userId = requestContext.GetUserId();
          bool flag1 = false;
          bool flag2 = false;
          if (requestContext != null)
          {
            IVssRequestContext requestContext1 = requestContext.IsVirtualServiceHost() ? requestContext.To(TeamFoundationHostType.Parent) : requestContext;
            flag1 = requestContext1.IsFeatureEnabled("VisualStudio.Services.Authentication.CheckTokenHostMappingRules");
            flag2 = requestContext1.IsFeatureEnabled("VisualStudio.Services.Authentication.CheckTokenCollectionHostRevocationRules");
          }
          string str = authCredential.AuthenticatedPrincipal?.FindFirst("http://schemas.microsoft.com/identity/claims/scope")?.Value;
          IList<Guid> hostIdsInToken = (IList<Guid>) new List<Guid>();
          string[] strArray;
          if (str == null)
            strArray = (string[]) null;
          else
            strArray = str.Split(' ');
          string[] tokenParts = strArray;
          DateTimeOffset validFrom = authCredential.ValidFrom;
          TokenRevocationRule[] allRules = this.GetAllRules(requestContext);
          bool? nullable = new bool?();
          if (flag1)
          {
            hostIdsInToken = this.GetHostidsFromAudiences(authCredential);
            if (hostIdsInToken != null)
              requestContext.TraceConditionally(104991, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Audiences from token correspond to the following HostIds: " + string.Join<Guid>(", ", (IEnumerable<Guid>) hostIdsInToken)));
          }
          foreach (TokenRevocationRule tokenRevocationRule in allRules)
          {
            TokenRevocationRule rule = tokenRevocationRule;
            switch (rule.RuleType)
            {
              case TokenRevocationRuleType.TokenRevocation:
                requestContext.TraceConditionally(1049093, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Token revocation rule at current context is: " + rule.RuleId.ToString()));
                bool flag3 = rule.MatchRule(validFrom, tokenParts, userId);
                if (flag3 && (!rule.HostId.HasValue || rule.HostId.Value == Guid.Empty))
                {
                  TokenRevocationServiceBase.LogTokenRevocationEvent(requestContext, "TokenRevocationBasedOnUserIdentity", rule, userId, hostIdsInToken, validFrom);
                  failingRuleId = rule.RuleId;
                  return false;
                }
                if (flag2)
                {
                  if (requestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
                  {
                    requestContext.Trace(104989, TraceLevel.Warning, "TokenRevocation", "Service", "Rejection rule found at unexpected request level");
                    break;
                  }
                  if (flag3 && hostIdsInToken.Any<Guid>((Func<Guid, bool>) (x =>
                  {
                    Guid guid = x;
                    Guid? hostId = rule.HostId;
                    return hostId.HasValue && guid == hostId.GetValueOrDefault();
                  })))
                  {
                    TokenRevocationServiceBase.LogTokenRevocationEvent(requestContext, "TokenRevocationBasedOnCollectionHost", rule, userId, hostIdsInToken, validFrom);
                    failingRuleId = rule.RuleId;
                    return false;
                  }
                  break;
                }
                break;
              case TokenRevocationRuleType.ActivatedParentHost:
                if (flag1)
                {
                  requestContext.TraceConditionally(1049093, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Acceptance rule at current context is: " + rule.RuleId.ToString() + ", createdBefore: " + rule.CreatedBefore.ToString() + ", token is valid from: " + validFrom.ToString()));
                  if (validFrom < (DateTimeOffset) rule.CreatedBefore.Value && !nullable.HasValue)
                  {
                    if (requestContext.ServiceHost.HostType != TeamFoundationHostType.ProjectCollection)
                    {
                      requestContext.TraceConditionally(104989, TraceLevel.Warning, "TokenRevocation", "Service", (Func<string>) (() => "Acceptance rule found at unexpected request level"));
                      break;
                    }
                    if (hostIdsInToken.Count > 0)
                    {
                      if (hostIdsInToken.Any<Guid>((Func<Guid, bool>) (x =>
                      {
                        Guid guid = x;
                        Guid? hostId = rule.HostId;
                        return hostId.HasValue && guid == hostId.GetValueOrDefault();
                      })))
                      {
                        requestContext.TraceConditionally(104986, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Acceptance rule matched for host: " + rule.HostId.ToString() + "and createdBefore:" + rule.CreatedBefore.ToString()));
                        nullable = new bool?(true);
                        break;
                      }
                      nullable = new bool?(false);
                      break;
                    }
                    break;
                  }
                  break;
                }
                break;
              case TokenRevocationRuleType.LegacyParentHost:
                if (flag1)
                {
                  requestContext.TraceConditionally(1049093, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Rejection rule at current context is: " + rule.RuleId.ToString() + ", createdBefore: " + rule.CreatedBefore.ToString() + ", token is valid from: " + validFrom.ToString()));
                  if (validFrom < (DateTimeOffset) rule.CreatedBefore.Value && !ignoreOrgHostCheck)
                  {
                    if (requestContext.ServiceHost.HostType != TeamFoundationHostType.Application)
                    {
                      requestContext.TraceConditionally(104988, TraceLevel.Warning, "TokenRevocation", "Service", (Func<string>) (() => "Rejection rule found at unexpected request level"));
                      break;
                    }
                    if (hostIdsInToken.Any<Guid>((Func<Guid, bool>) (x =>
                    {
                      Guid guid = x;
                      Guid? hostId = rule.HostId;
                      return hostId.HasValue && guid == hostId.GetValueOrDefault();
                    })))
                    {
                      requestContext.TraceConditionally(104987, TraceLevel.Info, "TokenRevocation", "Service", (Func<string>) (() => "Rejection rule matched for host: " + rule.HostId.ToString() + "and createdBefore:" + rule.CreatedBefore.ToString()));
                      return false;
                    }
                    break;
                  }
                  break;
                }
                break;
            }
          }
          if (nullable.HasValue && nullable.Value)
            ignoreOrgHostCheck = nullable.Value;
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Parent);
          if (vssRequestContext != null)
            return vssRequestContext.GetService<ITokenRevocationService>().IsValid(vssRequestContext, authCredential, userId, out failingRuleId, ignoreOrgHostCheck);
          break;
      }
      return true;
    }

    private static void LogTokenRevocationEvent(
      IVssRequestContext requestContext,
      string eventName,
      TokenRevocationRule rule,
      Guid userId,
      IList<Guid> hostIdsInToken,
      DateTimeOffset validFrom)
    {
      IVssRequestContext requestContext1 = requestContext;
      object[] objArray = new object[12]
      {
        (object) eventName,
        (object) userId,
        (object) string.Join<Guid>(",", (IEnumerable<Guid>) hostIdsInToken),
        (object) validFrom,
        (object) (requestContext.GetAuthenticationMechanism() ?? "<null>"),
        (object) rule.RuleType,
        (object) rule.RuleId,
        (object) (rule.Scopes ?? "<null>"),
        null,
        null,
        null,
        null
      };
      Guid? nullable = rule.IdentityId;
      objArray[8] = (object) (nullable ?? Guid.Empty);
      nullable = rule.HostId;
      objArray[9] = (object) (nullable ?? Guid.Empty);
      objArray[10] = (object) rule.CreationDate;
      objArray[11] = (object) (rule.CreatedBefore ?? DateTime.MaxValue);
      requestContext1.TraceAlways(104992, TraceLevel.Info, "TokenRevocation", "Service", "{0}: Token(UserId={1},HostsIds={2},ValidFrom={3},AuthenticationMechanism={4}) matched on Rule(Type={5},Id={6},Scopes={7},IdentityId={8},HostId={9},CreationDate={10},CreatedBefore={11})", objArray);
    }

    private void OnTokenRevocationServiceChangedNotification(
      IVssRequestContext requestContext,
      Guid eventClass,
      string eventData)
    {
      requestContext.Trace(1049085, TraceLevel.Info, "TokenRevocation", "Service", "Sql notification received for invalidating cached token revocation rules.");
      this.ClearCache(requestContext);
    }

    protected void ClearCache(IVssRequestContext requestContext)
    {
      using (requestContext.LockManager.Lock(this.CacheLockName))
      {
        ++this.CacheCounter;
        this.LocalCache.Clear();
      }
    }

    protected void FireTokenRevocationRuleChangedNotification(IVssRequestContext requestContext) => requestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(requestContext, this.SqlNotificationEventClass, (string) null);
  }
}
