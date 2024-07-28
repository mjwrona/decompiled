// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildRequest2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public class BuildRequest2008 : IValidatable
  {
    private string m_buildAgentUri;
    private string m_buildDefinitionUri;
    private QueuePriority2010 m_priority = QueuePriority2010.Normal;
    private bool m_postponed;
    private string m_dropLocation;
    private GetOption2010 m_getOption = GetOption2010.LatestOnBuild;
    private string m_customGetVersion;
    private int m_maxQueuePosition = int.MaxValue;
    private string m_requestedFor;
    private string m_requestedBy;
    private string m_commandLineArguments;

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildAgentUri
    {
      get => this.m_buildAgentUri;
      set => this.m_buildAgentUri = value;
    }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildDefinitionUri
    {
      get => this.m_buildDefinitionUri;
      set => this.m_buildDefinitionUri = value;
    }

    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string CommandLineArguments
    {
      get => this.m_commandLineArguments;
      set => this.m_commandLineArguments = value;
    }

    [XmlAttribute]
    [DefaultValue(QueuePriority2010.Normal)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuePriority2010 Priority
    {
      get => this.m_priority;
      set => this.m_priority = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Postponed
    {
      get => this.m_postponed;
      set => this.m_postponed = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string DropLocation
    {
      get => this.m_dropLocation;
      set => this.m_dropLocation = value;
    }

    [XmlAttribute]
    [DefaultValue(GetOption2010.LatestOnBuild)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public GetOption2010 GetOption
    {
      get => this.m_getOption;
      set => this.m_getOption = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string CustomGetVersion
    {
      get => this.m_customGetVersion;
      set => this.m_customGetVersion = value;
    }

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxQueuePosition
    {
      get => this.m_maxQueuePosition;
      set => this.m_maxQueuePosition = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor
    {
      get => this.m_requestedFor;
      set => this.m_requestedFor = value;
    }

    internal string RequestedBy
    {
      get => this.m_requestedBy;
      set => this.m_requestedBy = value;
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("BuildAgentUri", this.m_buildAgentUri, false, ResourceStrings.MissingController());
      ArgumentValidation.CheckUri("BuildDefinitionUri", this.m_buildDefinitionUri, "Definition", false, ResourceStrings.MissingDefinition());
      ArgumentValidation.CheckBound("MaxQueuePosition", this.m_maxQueuePosition, 0);
      ArgumentValidation.CheckDropLocation("DropLocation", ref this.m_dropLocation, true, (string) null);
      Microsoft.TeamFoundation.Build.Server.Validation.CheckIdentityName("RequestedFor", this.m_requestedFor, true);
      if (this.m_getOption == GetOption2010.Custom && string.IsNullOrEmpty(this.m_customGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionRequired((object) "Custom"));
      if (this.m_getOption != GetOption2010.Custom && !string.IsNullOrEmpty(this.m_customGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionIgnored((object) "Custom"));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildRequest2008 BuildDefinitionUri={0} BuildAgentrUri={1} Priority={2}]", (object) this.BuildDefinitionUri, (object) this.BuildAgentUri, (object) this.Priority);
  }
}
