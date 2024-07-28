// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ConfigurationContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ConfigurationContext : WebSdkMetadata
  {
    public const string ClientHostQueryParam = "clienthost";

    public ConfigurationContext(IVssRequestContext tfsRequestContext, RequestContext requestContext)
      : this()
    {
      this.Paths = new ConfigurationContextPaths(tfsRequestContext);
      this.Api = new ConfigurationContextApis();
      this.IsHosted = tfsRequestContext.ExecutionEnvironment.IsHostedDeployment;
      this.UseDevOpsDomainUrls = tfsRequestContext.UseDevOpsDomainUrls();
      this.ClientHost = ConfigurationContext.GetClientHost(requestContext);
      this.MailSettings = new TfsMailSettings(tfsRequestContext);
      this.RegistryItems = new Dictionary<string, string>();
    }

    public ConfigurationContext()
    {
    }

    [DataMember(EmitDefaultValue = false)]
    public bool IsHosted { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public bool UseDevOpsDomainUrls { get; private set; }

    [DataMember]
    public ConfigurationContextPaths Paths { get; private set; }

    [DataMember]
    public ConfigurationContextApis Api { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public TfsMailSettings MailSettings { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public virtual string ClientHost { get; private set; }

    [DataMember]
    public virtual Dictionary<string, string> RegistryItems { get; private set; }

    public static string GetClientHost(RequestContext requestContext) => requestContext.HttpContext.Request.Unvalidated.QueryString["clienthost"];

    public void IncludeRegistryItem(
      IVssRequestContext requestContext,
      string registryKey,
      bool fallThru,
      string defaultValue = null)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      if (service.GetValue(requestContext, (RegistryQuery) registryKey, fallThru, defaultValue) == null)
        return;
      this.RegistryItems.Add(registryKey, service.GetValue(requestContext, (RegistryQuery) registryKey, fallThru, defaultValue));
    }
  }
}
