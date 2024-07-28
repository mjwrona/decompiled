// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ServiceSettings
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ServiceSettings
  {
    private readonly string m_jobAgentSearchPlatformConnectionString;
    private readonly string m_atSearchPlatformConnectionString;

    public ServiceSettings(IVssRequestContext requestContext)
    {
      this.JobAgentSearchPlatformSettings = requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/JobAgentSearchPlatformSettings");
      this.ATSearchPlatformSettings = requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/ATSearchPlatformSettings");
      this.CustomSearchPlatformSettings = requestContext.GetElasticsearchPlatformSettings("/Service/ALMSearch/Settings/CustomSearchPlatformSettings", "ConnectionTimeout=180");
      this.m_jobAgentSearchPlatformConnectionString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString");
      this.m_atSearchPlatformConnectionString = requestContext.GetConfigValue("/Service/ALMSearch/Settings/ATSearchPlatformConnectionString");
      this.PipelineSettings = new PipelineSettings(requestContext);
      this.ProvisionerSettings = new ProvisionerSettings(requestContext);
      this.FaultManagementSettings = new FaultManagementSettings(requestContext);
      this.StoreSettings = new StoreSettings(requestContext);
      this.JobSettings = new JobSettings(requestContext);
    }

    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Throwing from the constructor will break when upgrading from TFS 2013 (#1577017)")]
    public string JobAgentSearchPlatformConnectionString => !string.IsNullOrWhiteSpace(this.m_jobAgentSearchPlatformConnectionString) ? this.m_jobAgentSearchPlatformConnectionString : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/JobAgentSearchPlatformConnectionString] is unvailable and the current value is null/empty.");

    public string JobAgentSearchPlatformSettings { get; set; }

    [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "Throwing from the constructor will break when upgrading from TFS 2013 (#1577017)")]
    public string ATSearchPlatformConnectionString => !string.IsNullOrWhiteSpace(this.m_atSearchPlatformConnectionString) ? this.m_atSearchPlatformConnectionString : throw new ServiceInitializationException("Setting [/Service/ALMSearch/Settings/ATSearchPlatformConnectionString] is unvailable and the current value is null/empty.");

    public string ATSearchPlatformSettings { get; set; }

    public string CustomSearchPlatformSettings { get; set; }

    public PipelineSettings PipelineSettings { get; set; }

    public ProvisionerSettings ProvisionerSettings { get; set; }

    public FaultManagementSettings FaultManagementSettings { get; set; }

    public StoreSettings StoreSettings { get; set; }

    public JobSettings JobSettings { get; set; }
  }
}
