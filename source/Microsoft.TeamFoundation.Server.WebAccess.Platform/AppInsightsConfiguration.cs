// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.AppInsightsConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class AppInsightsConfiguration : WebSdkMetadata
  {
    public AppInsightsConfiguration(WebContext webContext)
    {
      IVssRequestContext vssRequestContext = webContext.TfsRequestContext.To(TeamFoundationHostType.Deployment).Elevate();
      CachedRegistryService service = vssRequestContext.GetService<CachedRegistryService>();
      if (!service.GetValue<bool>(vssRequestContext, (RegistryQuery) "/Configuration/Telemetry/AppInsights/WebClient/Enabled", false, false))
        return;
      this.InstrumentationKey = service.GetValue<Guid>(vssRequestContext, (RegistryQuery) "/Configuration/Telemetry/AppInsights/WebClient/InstrumentationKey", false, new Guid());
      this.InsightsScriptUrl = service.GetValue<string>(vssRequestContext, (RegistryQuery) "/Configuration/Telemetry/AppInsights/WebClient/ScriptUrl", false, (string) null);
      this.TrackProjectInfo = service.GetValue<bool>(vssRequestContext, (RegistryQuery) "/Configuration/Telemetry/AppInsights/WebClient/TrackProjectInfo", false, false);
      if (!string.IsNullOrEmpty(this.InsightsScriptUrl) && this.InstrumentationKey != Guid.Empty)
        this.Enabled = true;
      this.AutoTrackPage = true;
    }

    [DataMember]
    public bool Enabled { get; set; }

    [DataMember]
    public Guid InstrumentationKey { get; set; }

    [DataMember]
    public string InsightsScriptUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool AutoTrackPage { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool TrackProjectInfo { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AppInsightsCustomTrackPageData CustomTrackPageData { get; set; }
  }
}
