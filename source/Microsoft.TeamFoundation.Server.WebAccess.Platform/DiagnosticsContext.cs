// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DiagnosticsContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Health;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class DiagnosticsContext : WebSdkMetadata
  {
    public const string ScriptDebugModeCookie = "TFS-DEBUG";
    public const string BundlingOverrideCookie = "TFS-BUNDLING";
    public const string CdnOverrideCookie = "TFS-CDN";
    public const string TracePointCollectorCookie = "TFS-TRACEPOINT-COLLECTOR";
    public const string TracePointProfileStartCookie = "TFS-TRACEPOINT-START";
    public const string TracePointProfileEndCookie = "TFS-TRACEPOINT-END";
    public const string ClientLogLevelCookie = "TFS-LOG-LEVEL";
    private static readonly RegistryQuery s_debugModeRegistryQuery = new RegistryQuery(WebAccessRegistryConstants.DebugModeDefaultValue);
    private static readonly RegistryQuery s_bundlingRegistryQuery = new RegistryQuery(WebAccessRegistryConstants.BundlingModeDefaultValue);
    private static readonly RegistryQuery s_tracePointsRegistryQuery = new RegistryQuery(WebAccessRegistryConstants.TracePointCollectionDefaultValue);
    private IVssRequestContext m_tfsRequestContext;
    private RequestContext m_requestContext;
    private bool? m_debugMode;
    private bool? m_bundlingEnabled;
    private bool? m_diagnoseBundles;
    private bool? m_cdnAvailable;
    private bool? m_cdnEnabled;
    private bool? m_tracePointCollectionEnabled;

    public DiagnosticsContext(IVssRequestContext tfsRequestContext, RequestContext requestContext)
    {
      this.m_tfsRequestContext = tfsRequestContext;
      this.m_requestContext = requestContext;
    }

    public DiagnosticsContext()
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public Guid SessionId => this.m_tfsRequestContext != null ? this.m_tfsRequestContext.UniqueIdentifier : Guid.Empty;

    [DataMember(EmitDefaultValue = false)]
    public Guid ActivityId => this.m_tfsRequestContext != null ? this.m_tfsRequestContext.ActivityId : Guid.Empty;

    [DataMember(EmitDefaultValue = false)]
    public virtual bool AllowStatsCollection
    {
      get
      {
        if (this.m_tfsRequestContext == null)
          return false;
        return this.m_tfsRequestContext.ExecutionEnvironment.IsDevFabricDeployment || this.DebugMode || this.m_tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.StatsCollection");
      }
    }

    public bool RunningDebugBits => false;

    [DataMember(EmitDefaultValue = false)]
    public bool DebugMode
    {
      get
      {
        if (!this.m_debugMode.HasValue)
        {
          if (!this.BundlingEnabled)
            this.m_debugMode = new bool?(true);
          else if (this.m_requestContext != null)
          {
            HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-DEBUG"];
            if (cookie != null)
            {
              this.m_debugMode = new bool?(!string.IsNullOrWhiteSpace(cookie.Value) && !"disabled".Equals(cookie.Value, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
              bool defaultValue = this.m_requestContext.HttpContext.IsDebuggingEnabled || this.RunningDebugBits;
              this.m_debugMode = new bool?(this.m_tfsRequestContext.GetService<IVssRegistryService>().GetValue<bool>(this.m_tfsRequestContext, in DiagnosticsContext.s_debugModeRegistryQuery, true, defaultValue));
            }
          }
          else
            this.m_debugMode = new bool?(false);
        }
        return this.m_debugMode.Value;
      }
      set => this.m_debugMode = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public bool BundlingEnabled
    {
      get
      {
        if (!this.m_bundlingEnabled.HasValue)
        {
          if (this.m_tfsRequestContext == null)
          {
            this.m_bundlingEnabled = new bool?(false);
          }
          else
          {
            HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-BUNDLING"];
            if (cookie != null)
            {
              this.m_bundlingEnabled = new bool?(!string.IsNullOrWhiteSpace(cookie.Value) && !"disabled".Equals(cookie.Value, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
              bool defaultValue = true;
              this.m_bundlingEnabled = new bool?(this.m_tfsRequestContext.GetService<IVssRegistryService>().GetValue<bool>(this.m_tfsRequestContext, in DiagnosticsContext.s_bundlingRegistryQuery, true, defaultValue));
            }
          }
        }
        return this.m_bundlingEnabled.Value;
      }
      set => this.m_bundlingEnabled = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public bool DiagnoseBundles
    {
      get
      {
        if (!this.m_diagnoseBundles.HasValue)
        {
          this.m_diagnoseBundles = new bool?(false);
          if (this.BundlingEnabled && this.m_requestContext != null)
          {
            string a = this.m_requestContext.HttpContext.Request.Params["diagnose-bundles"];
            this.m_diagnoseBundles = new bool?(string.Equals(a, "1", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "true", StringComparison.OrdinalIgnoreCase) || string.Equals(a, "on", StringComparison.OrdinalIgnoreCase));
          }
        }
        return this.m_diagnoseBundles.Value;
      }
      set => this.m_diagnoseBundles = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public bool CdnAvailable
    {
      get
      {
        if (!this.m_cdnAvailable.HasValue)
          this.m_cdnAvailable = this.m_tfsRequestContext == null || !this.m_tfsRequestContext.IsFeatureEnabled("VisualStudio.Services.WebAccess.UseCDN") ? new bool?(false) : new bool?(true);
        return this.m_cdnAvailable.Value;
      }
      set => this.m_cdnAvailable = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public bool CdnEnabled
    {
      get
      {
        if (!this.m_cdnEnabled.HasValue)
        {
          if (this.m_tfsRequestContext == null || !this.CdnAvailable)
            this.m_cdnEnabled = new bool?(false);
          else if (!this.BundlingEnabled)
          {
            this.m_cdnEnabled = new bool?(false);
          }
          else
          {
            this.m_cdnEnabled = new bool?(true);
            HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-CDN"];
            if (cookie != null)
              this.m_cdnEnabled = new bool?(!string.IsNullOrWhiteSpace(cookie.Value) && !"disabled".Equals(cookie.Value, StringComparison.OrdinalIgnoreCase));
          }
        }
        return this.m_cdnEnabled.Value;
      }
      set => this.m_cdnEnabled = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public bool TracePointCollectionEnabled
    {
      get
      {
        if (!this.m_tracePointCollectionEnabled.HasValue)
        {
          if (this.m_tfsRequestContext != null)
          {
            HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-TRACEPOINT-COLLECTOR"];
            if (cookie != null)
            {
              this.m_tracePointCollectionEnabled = new bool?(!string.IsNullOrWhiteSpace(cookie.Value) && !"disabled".Equals(cookie.Value, StringComparison.OrdinalIgnoreCase));
            }
            else
            {
              bool defaultValue = this.m_requestContext.HttpContext.IsDebuggingEnabled || this.RunningDebugBits;
              this.m_tracePointCollectionEnabled = new bool?(this.m_tfsRequestContext.GetService<IVssRegistryService>().GetValue<bool>(this.m_tfsRequestContext, in DiagnosticsContext.s_tracePointsRegistryQuery, true, defaultValue));
            }
          }
          else
            this.m_tracePointCollectionEnabled = new bool?(false);
        }
        return this.m_tracePointCollectionEnabled.Value;
      }
      set => this.m_tracePointCollectionEnabled = new bool?(value);
    }

    [DataMember(EmitDefaultValue = false)]
    public string TracePointProfileStart
    {
      get
      {
        if (this.TracePointCollectionEnabled && this.m_requestContext != null)
        {
          HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-TRACEPOINT-START"];
          if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            return cookie.Value;
        }
        return (string) null;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string TracePointProfileEnd
    {
      get
      {
        if (this.TracePointCollectionEnabled && this.m_requestContext != null)
        {
          HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-TRACEPOINT-END"];
          if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            return cookie.Value;
        }
        return (string) null;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public int? ClientLogLevel
    {
      get
      {
        if (this.m_requestContext != null)
        {
          HttpCookie cookie = this.m_requestContext.HttpContext.Request.Cookies["TFS-LOG-LEVEL"];
          TraceLevel result;
          if (cookie != null && !string.IsNullOrEmpty(cookie.Value) && Enum.TryParse<TraceLevel>(cookie.Value, true, out result))
            return new int?((int) result);
        }
        return new int?();
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool IsDevFabric => this.m_tfsRequestContext != null && this.m_tfsRequestContext.ExecutionEnvironment.IsDevFabricDeployment;

    [DataMember(EmitDefaultValue = false)]
    public string WebPlatformVersion
    {
      get
      {
        ServiceLevel currentServiceLevel = TeamFoundationSqlResourceComponent.CurrentServiceLevel;
        return TeamFoundationSqlResourceComponent.CurrentServiceLevel.Milestone;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public bool InExtensionFallbackMode
    {
      get
      {
        bool flag = false;
        return ((this.m_tfsRequestContext == null ? 0 : (this.m_tfsRequestContext.TryGetItem<bool>(nameof (InExtensionFallbackMode), out flag) ? 1 : 0)) & (flag ? 1 : 0)) != 0;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string ServiceVersion
    {
      get
      {
        BuildInfo[] buildInfo = BuildInfoReader.GetBuildInfo();
        string str = "unknown";
        if (buildInfo != null && buildInfo.Length != 0)
          str = buildInfo[0].BuildNumber;
        return TeamFoundationSqlResourceComponent.CurrentServiceLevel.ToString() + " (build: " + str + ")";
      }
    }
  }
}
