// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildDetail2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType("BuildDetail")]
  public sealed class BuildDetail2010 : ICacheable
  {
    private string m_databaseUri;
    private StreamingCollection<BuildInformationNode2010> m_information = new StreamingCollection<BuildInformationNode2010>();

    public BuildDetail2010() => this.Reason = BuildReason2010.Manual;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string TeamProject { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildNumber { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildDefinitionUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime StartTime { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime FinishTime { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.Manual)]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildReason2010 Reason { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string Quality { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildPhaseStatus2010 CompilationStatus { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildPhaseStatus2010 TestStatus { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DropLocation { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string DropLocationRoot { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LogLocation { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string SourceGetVersion { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string RequestedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public DateTime LastChangedOn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LastChangedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool KeepForever { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LabelName { get; set; }

    [ClientProperty(ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly, PropertyName = "InternalInformation")]
    public StreamingCollection<BuildInformationNode2010> Information => this.m_information;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool IsDeleted { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string BuildAgentUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ConfigurationFolderUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ShelvesetName { get; set; }

    internal string DatabaseUri
    {
      get
      {
        if (this.m_databaseUri == null)
        {
          string dbId = DBHelper.ExtractDbId(this.Uri);
          int length = dbId.IndexOf("?queueId=", StringComparison.OrdinalIgnoreCase);
          this.m_databaseUri = length >= 0 ? DBHelper.CreateArtifactUri("Build", dbId.Substring(0, length)) : DBHelper.CreateArtifactUri("Build", dbId);
        }
        return this.m_databaseUri;
      }
    }

    internal int QueueId { get; set; }

    [XmlIgnore]
    public BuildDefinition2010 Definition { get; internal set; }

    int ICacheable.GetCachedSize() => 800;

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetail2010 Uri={0} BuildNumber={1} Status={2} CompilationStatus={3} TestStatus={4}]", (object) this.Uri, (object) this.BuildNumber, (object) this.Status, (object) this.CompilationStatus, (object) this.TestStatus);
  }
}
