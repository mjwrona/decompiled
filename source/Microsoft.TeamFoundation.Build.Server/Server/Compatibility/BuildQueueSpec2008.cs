// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildQueueSpec2008
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [XmlType("BuildQueueSpec")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  public sealed class BuildQueueSpec2008 : IValidatable
  {
    private BuildAgentSpec2008 m_agentSpec;
    private BuildDefinitionSpec2010 m_definitionSpec;
    private QueryOptions2010 m_options;
    private QueueStatus2010 m_statusFlags;
    private int m_completedAge;

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public BuildAgentSpec2008 AgentSpec
    {
      get => this.m_agentSpec;
      set => this.m_agentSpec = value;
    }

    [XmlElement]
    [ClientProperty(ClientVisibility.Private)]
    public BuildDefinitionSpec2010 DefinitionSpec
    {
      get => this.m_definitionSpec;
      set => this.m_definitionSpec = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "QueryOptions", Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueryOptions2010 Options
    {
      get => this.m_options;
      set => this.m_options = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, PropertyName = "Status", Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueueStatus2010 StatusFlags
    {
      get => this.m_statusFlags;
      set => this.m_statusFlags = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public int CompletedAge
    {
      get => this.m_completedAge;
      set => this.m_completedAge = value;
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "AgentSpec", (IValidatable) this.m_agentSpec, false, ValidationContext.Query);
      Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, "DefinitionSpec", (IValidatable) this.m_definitionSpec, false, ValidationContext.Query);
      ArgumentValidation.CheckBound("CompletedAge", this.m_completedAge, -1);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildQueueSpec2008 StatusFlags={0} Options={1} DefinitionSpec={2} AgentSpec={3}]", (object) this.StatusFlags, (object) this.Options, (object) this.DefinitionSpec, (object) this.AgentSpec);
  }
}
