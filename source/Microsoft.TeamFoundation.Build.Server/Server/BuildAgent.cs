// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildAgent
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.Server.DataAccess;
using Microsoft.TeamFoundation.Build.Server.ServiceProxies;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [CallOnDeserialization("AfterDeserialize")]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildAgent : IValidatable, IPropertyProvider
  {
    private List<string> m_tags = new List<string>();
    private List<PropertyValue> m_properties;

    public BuildAgent() => this.Status = AgentStatus.Offline;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Private)]
    public string ServiceHostUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ControllerUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string BuildDirectory { get; set; }

    [XmlAttribute]
    [DefaultValue(AgentStatus.Unavailable)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public AgentStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public)]
    public bool Enabled { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    public string Url { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string MessageQueueUrl { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateCreated { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public DateTime DateUpdated { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private)]
    public string ReservedForBuild { get; set; }

    [ClientProperty(ClientVisibility.Internal, PropertyName = "InternalProperties")]
    public List<PropertyValue> Properties
    {
      get
      {
        if (this.m_properties == null)
          this.m_properties = new List<PropertyValue>();
        return this.m_properties;
      }
    }

    [ClientProperty(ClientVisibility.Private)]
    public List<string> Tags => this.m_tags;

    internal void TestAgentAvailability(IVssRequestContext requestContext, BuildServiceHost host)
    {
      using (BuildAgentServiceProxy2010 serviceProxy2010 = new BuildAgentServiceProxy2010(requestContext, host.GetUrlForService(this.Uri), host.RequireClientCertificates))
      {
        try
        {
          serviceProxy2010.TestConnection();
        }
        catch (Exception ex)
        {
          if (this.Status == AgentStatus.Unavailable && !string.IsNullOrEmpty(this.StatusMessage))
            return;
          this.MarkAgentUnavailable(requestContext, ex);
        }
      }
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Common.Validation.CheckValidAgentName(this.Name, false);
      ArgumentValidation.CheckUri("ControllerUri", this.ControllerUri, "Controller", false, ResourceStrings.MissingController());
      ArgumentValidation.CheckUri("ServiceHostUri", this.ServiceHostUri, "ServiceHost", false, (string) null);
      string buildDirectory = this.BuildDirectory;
      ArgumentValidation.CheckBuildDirectory("BuildDirectory", ref buildDirectory, false);
      this.BuildDirectory = buildDirectory;
      Validation.CheckDescription("Description", this.Description, true);
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      ArgumentValidation.CheckArray<string>("Tags", (IList<string>) this.Tags, BuildAgent.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag ?? (BuildAgent.\u003C\u003EO.\u003C0\u003E__CheckBuildAgentTag = new Validate<string>(Validation.CheckBuildAgentTag)), false, (string) null);
    }

    private void MarkAgentOffline(IVssRequestContext requestContext, Exception e) => this.SetAgentStatus(requestContext, AgentStatus.Offline, e.Message);

    private void MarkAgentUnavailable(IVssRequestContext requestContext, Exception e) => this.SetAgentStatus(requestContext, AgentStatus.Unavailable, e.Message);

    private void MarkAgentAvailable(IVssRequestContext requestContext) => this.SetAgentStatus(requestContext, AgentStatus.Available, AdministrationResources.BuildAgentReenabled());

    internal void SetAgentStatus(
      IVssRequestContext requestContext,
      AgentStatus status,
      string statusMessage)
    {
      using (AdministrationComponent component = requestContext.CreateComponent<AdministrationComponent>("Build"))
        component.UpdateBuildAgents((IList<BuildAgentUpdateOptions>) new BuildAgentUpdateOptions[1]
        {
          new BuildAgentUpdateOptions()
          {
            Uri = this.Uri,
            Fields = BuildAgentUpdate.Status | BuildAgentUpdate.StatusMessage,
            Status = status,
            StatusMessage = AdministrationResources.BuildAgentStatusAutomaticallyChanged((object) statusMessage, (object) DateTime.UtcNow)
          }
        });
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildAgent Uri={0} Name={1} ServiceHostUri={2} ControllerUri={3} Status={4}]", (object) this.Uri, (object) this.Name, (object) this.ServiceHostUri, (object) this.ControllerUri, (object) this.Status);
  }
}
