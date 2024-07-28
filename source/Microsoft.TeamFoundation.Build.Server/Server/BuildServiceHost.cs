// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildServiceHost
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildServiceHost : IValidatable
  {
    private IDictionary<string, object> m_properties;
    private static Lazy<Regex> s_versionRegex = new Lazy<Regex>((Func<Regex>) (() => new Regex("^/Build/v(?<version>([1-9]+\\.[0-9]+))/Services", RegexOptions.Compiled)));

    public BuildServiceHost() => this.Status = ServiceHostStatus.Offline;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string BaseUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public bool IsVirtual { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string MessageQueueUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool RequireClientCertificates { get; set; }

    [XmlAttribute]
    [DefaultValue(ServiceHostStatus.Offline)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public ServiceHostStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime StatusChangedOn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime AcquiredOn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime ConnectedOn { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    internal bool IsAllocationPending { get; set; }

    internal Guid OwnerSessionId { get; set; }

    internal Guid ServiceIdentityId { get; set; }

    internal IDictionary<string, object> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = (IDictionary<string, object>) new Dictionary<string, object>();
        return this.m_properties;
      }
      set => this.m_properties = value;
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidServiceHostName(this.Name, false);
      ArgumentValidation.CheckUri("BaseUrl", this.BaseUrl, false, (string) null);
      if (this.IsVirtual && !requestContext.IsServicingContext && !requestContext.IsSystemContext)
        throw new NotSupportedException(AdministrationResources.BuildServiceHostIsVirtualReserved());
    }

    internal string GetUrlForService(string serviceUri) => BuildServiceHost.GetUrlForService(this.BaseUrl, serviceUri);

    internal static string GetUrlForService(string serviceHostUrl, string serviceUri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(serviceUri);
      UriBuilder uriBuilder = new UriBuilder(serviceHostUrl);
      uriBuilder.Path = VirtualPathUtility.AppendTrailingSlash(uriBuilder.Path);
      uriBuilder.Path += string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/{1}", (object) artifactId.ArtifactType, (object) artifactId.ToolSpecificId);
      return uriBuilder.Uri.AbsoluteUri;
    }

    internal void AgentUpdated(
      IVssRequestContext requestContext,
      BuildAgent agent,
      ServiceAction action)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (AgentUpdated));
      Message agentUpdated = HostService.CreateAgentUpdated(agent.Uri, action);
      agentUpdated.Headers.To = new System.Uri(this.MessageQueueUrl);
      try
      {
        TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Enqueuing message '{0}'", (object) agentUpdated);
        IVssRequestContext requestContext1 = requestContext;
        Message message = agentUpdated;
        string baseUrl = this.BaseUrl;
        service.QueueMessage(requestContext1, message, baseUrl);
      }
      catch (MessageQueueNotFoundException ex)
      {
        requestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (AgentUpdated));
    }

    internal void ControllerUpdated(
      IVssRequestContext requestContext,
      BuildController controller,
      ServiceAction action)
    {
      requestContext.TraceEnter(0, "BuildAdministration", "Service", nameof (ControllerUpdated));
      Message controllerUpdated = HostService.CreateControllerUpdated(controller.Uri, action);
      controllerUpdated.Headers.To = new System.Uri(this.MessageQueueUrl);
      try
      {
        TeamFoundationBuildResourceService service = requestContext.GetService<TeamFoundationBuildResourceService>();
        requestContext.Trace(0, TraceLevel.Verbose, "BuildAdministration", "Service", "Enqueuing message '{0}'", (object) controllerUpdated);
        IVssRequestContext requestContext1 = requestContext;
        Message message = controllerUpdated;
        string baseUrl = this.BaseUrl;
        service.QueueMessage(requestContext1, message, baseUrl);
      }
      catch (MessageQueueNotFoundException ex)
      {
        requestContext.TraceException(0, "BuildAdministration", "Service", (Exception) ex);
      }
      requestContext.TraceLeave(0, "BuildAdministration", "Service", nameof (ControllerUpdated));
    }

    public int Version => BuildServiceHost.GetVersion(this.BaseUrl);

    internal static int GetVersion(string urlString)
    {
      System.Uri uri = new System.Uri(urlString, UriKind.Absolute);
      MatchCollection matchCollection = BuildServiceHost.s_versionRegex.Value.Matches(uri.AbsolutePath);
      if (matchCollection.Count > 0)
      {
        Group group = matchCollection[0].Groups["version"];
        if (group != null && !string.IsNullOrEmpty(group.Value))
          return VersionHelper.Parse(group.Value);
      }
      return 0;
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildServiceHost Uri={0} Name={1} Status={2}]", (object) this.Uri, (object) this.Name, (object) this.Status);
  }
}
