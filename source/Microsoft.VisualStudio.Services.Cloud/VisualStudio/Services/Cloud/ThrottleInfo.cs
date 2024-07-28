// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ThrottleInfo
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ThrottleInfo
  {
    private ResourceState2 m_throttleType2;
    private static readonly RegistryQuery s_mdmServiceRegistryQuery = new RegistryQuery("/Diagnostics/Hosting/MdmDiagnostics/MdmService");
    private const int c_longWindowSeconds = 900;

    public ProcessedRURule Rule { get; private set; }

    public Guid Namespace { get; private set; }

    public int WindowSeconds { get; private set; }

    public string Key { get; private set; }

    public ResourceState ThrottleType { get; private set; }

    public ResourceState2 ThrottleType2
    {
      get => this.m_throttleType2;
      private set
      {
        this.m_throttleType2 = value;
        this.ThrottleType = (this.m_throttleType2 & ResourceState2.Block) > ResourceState2.Normal ? ResourceState.Block : ((this.m_throttleType2 & ResourceState2.Tarpit) > ResourceState2.Normal ? ResourceState.Tarpit : ((this.m_throttleType2 & ResourceState2.Flag) > ResourceState2.Normal ? ResourceState.Flagged : ResourceState.Normal));
      }
    }

    public long Limit { get; private set; }

    public long Usage { get; private set; }

    public DateTimeOffset Reset { get; set; }

    public DateTimeOffset RetryAfterTimestamp { get; private set; }

    public DateTime TimeOfThrottleEvent { get; private set; }

    public Microsoft.VisualStudio.Services.Identity.Identity UserIdentity { get; set; }

    public int DelayForNextAwait { get; set; }

    public bool ShouldNotifyUser { get; set; }

    public bool IsPublicUser { get; set; }

    public bool BlockedByConcurrencyTimeout { get; set; }

    public ThrottleInfo(
      ProcessedRURule rule,
      Guid namespaceId,
      string key,
      ResourceState2 resourceState2)
      : this(rule, namespaceId, 0, key, resourceState2, -1L, -1L, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
    {
    }

    public ThrottleInfo(
      ProcessedRURule rule,
      Guid namespaceId,
      int dpImpact,
      string key,
      ResourceState2 resourceState2,
      long throttlingThreshold,
      long usage,
      DateTimeOffset reset,
      DateTimeOffset retryAfterTimestamp)
      : this(rule, namespaceId, dpImpact, key, ResourceState.Normal, throttlingThreshold, usage, reset, retryAfterTimestamp)
    {
      this.ThrottleType2 = resourceState2;
      this.ShouldNotifyUser = (resourceState2 & ResourceState2.Block) > ResourceState2.Normal;
    }

    public ThrottleInfo(
      ProcessedRURule rule,
      Guid namespaceId,
      string key,
      ResourceState resourceState)
      : this(rule, namespaceId, 0, key, resourceState, -1L, -1L, DateTimeOffset.UtcNow, DateTimeOffset.UtcNow)
    {
    }

    public ThrottleInfo(
      ProcessedRURule rule,
      Guid namespaceId,
      int windowSeconds,
      string key,
      ResourceState resourceState,
      long throttlingThreshold,
      long usage,
      DateTimeOffset reset,
      DateTimeOffset retryAfterTimestamp)
    {
      this.Rule = rule != null ? rule : throw new ArgumentNullException(nameof (rule));
      this.Namespace = namespaceId;
      this.WindowSeconds = windowSeconds;
      this.Key = key;
      this.ThrottleType = resourceState;
      this.Limit = throttlingThreshold;
      this.Usage = usage;
      this.Reset = reset;
      this.RetryAfterTimestamp = retryAfterTimestamp;
      this.TimeOfThrottleEvent = DateTime.UtcNow;
      this.ShouldNotifyUser = resourceState == ResourceState.Block;
      this.UserIdentity = (Microsoft.VisualStudio.Services.Identity.Identity) null;
      this.DelayForNextAwait = 0;
    }

    public string GetGlobalMessageResponseHeader(IVssRequestContext requestContext)
    {
      if (this.Rule.EntityTypes.Any<RURequestProperty>((Func<RURequestProperty, bool>) (t => t is RURequestProperty_IsAnonymous)))
        return GetAnonymousOrPublicBanner(HostingResources.YouAreBeingThrottledAnonymousFormat((object) this.Key, (object) "{0}"));
      if (requestContext.IsPublicUser())
        return GetAnonymousOrPublicBanner(HostingResources.YouAreBeingThrottledPublicUserFormat((object) "{0}"));
      ILocationService service = requestContext.GetService<ILocationService>();
      string str;
      try
      {
        if (this.UserIdentity == null)
          this.UserIdentity = requestContext.GetUserIdentity();
        ILocationDataProvider locationData = service.GetLocationData(requestContext, ServiceInstanceTypes.TFS);
        string baseUsageUrl = locationData.LocationForAccessMapping(requestContext, "UtilizationUsageSummary", FrameworkServiceIdentifiers.UtilizationUsageSummary, locationData.DetermineAccessMapping(requestContext));
        str = this.CreateTimeboxedUsageUrl(requestContext, baseUsageUrl);
      }
      catch
      {
        str = string.Empty;
      }
      return new JObject()
      {
        {
          "level",
          (JToken) 1
        },
        {
          "messageFormat",
          (JToken) HostingResources.YouAreBeingThrottledFormat((object) "{0}", (object) "{1}")
        },
        {
          "messageLinks",
          (JToken) new JArray()
          {
            (JToken) new JObject()
            {
              {
                "name",
                (JToken) HostingResources.YouAreBeingThrottledUsageLinkText()
              },
              {
                "href",
                (JToken) str
              }
            },
            (JToken) new JObject()
            {
              {
                "name",
                (JToken) HostingResources.YouAreBeingThrottledDocumentationLinkText()
              },
              {
                "href",
                (JToken) HostingResources.YouAreBeingThrottledDocumentationLink()
              }
            }
          }
        }
      }.ToString(Formatting.None);

      static string GetAnonymousOrPublicBanner(string messageFormat) => new JObject()
      {
        {
          "level",
          (JToken) 1
        },
        {
          nameof (messageFormat),
          (JToken) messageFormat
        },
        {
          "messageLinks",
          (JToken) new JArray()
          {
            (JToken) new JObject()
            {
              {
                "name",
                (JToken) HostingResources.YouAreBeingThrottledLearnMoreText()
              },
              {
                "href",
                (JToken) HostingResources.YouAreBeingThrottledDocumentationLink()
              }
            }
          }
        }
      }.ToString(Formatting.None);
    }

    public string CreateTimeboxedUsageUrl(IVssRequestContext requestContext, string baseUsageUrl)
    {
      string str1 = requestContext.To(TeamFoundationHostType.Deployment).GetService<IVssRegistryService>().GetValue<string>(requestContext, in ThrottleInfo.s_mdmServiceRegistryQuery, string.Empty);
      UriBuilder uriBuilder = new UriBuilder(baseUsageUrl);
      NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
      NameValueCollection nameValueCollection = queryString;
      DateTime dateTime = this.TimeOfThrottleEvent;
      dateTime = dateTime.AddHours(-0.5);
      string str2 = dateTime.ToString("s");
      dateTime = this.TimeOfThrottleEvent;
      dateTime = dateTime.AddHours(0.5);
      string str3 = dateTime.ToString("s");
      string str4 = str2 + "," + str3;
      nameValueCollection["queryDate"] = str4;
      queryString["identity"] = this.UserIdentity.Id.ToString();
      queryString["services"] = str1;
      uriBuilder.Query = queryString.ToString();
      return uriBuilder.ToString();
    }

    public void Test_SetTimeOfThrottleEvent(DateTime testTime) => this.TimeOfThrottleEvent = testTime;

    internal void MergeWithNewThrottleInfo(ThrottleInfo other)
    {
      bool flag = false;
      if (this.ThrottleType == other.ThrottleType)
        flag = this.ThrottleType != ResourceState.Flagged ? this.RetryAfterTimestamp < other.RetryAfterTimestamp : other.Limit != -1L && (this.Limit == -1L || (double) other.Usage / (double) Math.Max(1L, other.Limit) > (double) this.Usage / (double) Math.Max(1L, this.Limit));
      else if (other.ThrottleType == ResourceState.Block)
        flag = true;
      else if (other.ThrottleType == ResourceState.Tarpit)
        flag = this.ThrottleType != ResourceState.Block;
      else if (other.ThrottleType == ResourceState.Flagged)
        flag = this.ThrottleType == ResourceState.Normal;
      if (flag)
      {
        this.ThrottleType2 = other.ThrottleType2;
        this.ThrottleType = other.ThrottleType;
        this.BlockedByConcurrencyTimeout = other.BlockedByConcurrencyTimeout;
        this.Rule = other.Rule;
        this.Namespace = other.Namespace;
        this.Key = other.Key;
        this.WindowSeconds = other.WindowSeconds;
        this.Usage = other.Usage;
        this.Limit = other.Limit;
        this.Reset = other.Reset;
      }
      this.ShouldNotifyUser |= other.ShouldNotifyUser;
      if (other.RetryAfterTimestamp > this.RetryAfterTimestamp)
        this.RetryAfterTimestamp = other.RetryAfterTimestamp;
      this.DelayForNextAwait = Math.Max(this.DelayForNextAwait, other.DelayForNextAwait);
    }

    internal string GetResourceHeader(IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      return vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstanceType(vssRequestContext, requestContext.ServiceHost.DeploymentServiceHost.ServiceInstanceType)?.Name + "/" + this.GetFriendlyWindowName();
    }

    internal string GetThrottleReason()
    {
      string str = this.BlockedByConcurrencyTimeout ? HostingResources.ConcurrencyTimeout() : string.Empty;
      return this.GetFriendlyWindowName() + str + "/" + this.Rule.GetFriendlyNamespace() + "/" + this.Rule.GetResourceName();
    }

    internal string GetThrottleReasonWithRule() => this.GetThrottleReason() + "/" + this.Rule.RuleName;

    public string GetFriendlyWindowName()
    {
      if (this.WindowSeconds <= 0)
        return HostingResources.ConcurrentResource();
      return this.WindowSeconds <= 900 ? HostingResources.ShortResourceWindow() : HostingResources.LongResourceWindow();
    }
  }
}
