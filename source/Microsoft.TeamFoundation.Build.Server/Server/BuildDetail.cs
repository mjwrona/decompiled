// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDetail
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [RequiredClientService("BuildServer")]
  [CallOnDeserialization("AfterDeserialize")]
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildDetail : IValidatable, ICacheable
  {
    private List<int> m_queueIds = new List<int>();
    private StreamingCollection<BuildInformationNode> m_information = new StreamingCollection<BuildInformationNode>();
    private Dictionary<string, object> m_extendedProperties;

    public BuildDetail() => this.Reason = BuildReason.Manual;

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
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildReason Reason { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string Quality { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildPhaseStatus CompilationStatus { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public BuildPhaseStatus TestStatus { get; set; }

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
    public DateTime LastChangedOn { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LastChangedBy { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LastChangedByDisplayName { get; set; }

    public Guid LastChangeByTFID { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool KeepForever { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public string LabelName { get; set; }

    [ClientProperty(ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public List<int> QueueIds => this.m_queueIds;

    [ClientProperty(ClientVisibility.Internal, Direction = ClientPropertySerialization.ServerToClientOnly, PropertyName = "InternalInformation")]
    public StreamingCollection<BuildInformationNode> Information => this.m_information;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public bool IsDeleted { get; set; }

    [ClientProperty(ClientVisibility.Public, ClientVisibility.Private, Direction = ClientPropertySerialization.ServerToClientOnly)]
    public long? ContainerId { get; set; }

    [XmlIgnore]
    public BuildDefinition Definition { get; internal set; }

    [XmlIgnore]
    public Dictionary<string, object> ExtendedProperties
    {
      get
      {
        if (this.m_extendedProperties == null)
          this.m_extendedProperties = new Dictionary<string, object>();
        return this.m_extendedProperties;
      }
    }

    [XmlIgnore]
    public Guid ProjectId { get; set; }

    int ICacheable.GetCachedSize() => 800;

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckBuildNumber("BuildNumber", this.BuildNumber, false);
      ArgumentValidation.CheckUri("BuildDefinitionUri", this.BuildDefinitionUri, "Definition", false, ResourceStrings.MissingDefinition());
      ArgumentValidation.CheckUri("BuildControllerUri", this.BuildControllerUri, "Controller", false, ResourceStrings.MissingController());
      ArgumentValidation.Check("Quality", this.Quality, false, ResourceStrings.MissingQuality());
    }

    internal void MarkBuildComplete(IVssRequestContext requestContext, BuildStatus status) => this.MarkBuildComplete(requestContext, requestContext.UserContext, status);

    internal void MarkBuildComplete(
      IVssRequestContext requestContext,
      IdentityDescriptor requestedFor,
      BuildStatus status)
    {
      requestContext.TraceEnter(0, "Build", "Service", nameof (MarkBuildComplete));
      requestContext.GetService<TeamFoundationBuildService>().UpdateBuilds(requestContext, (IList<BuildUpdateOptions>) new BuildUpdateOptions[1]
      {
        new BuildUpdateOptions()
        {
          Uri = this.Uri,
          Status = status,
          Fields = BuildUpdate.FinishTime | BuildUpdate.Status
        }
      }, new Guid());
      requestContext.TraceLeave(0, "Build", "Service", nameof (MarkBuildComplete));
    }

    internal void ReplaceInformation(List<BuildInformationNode> information)
    {
      this.m_information = new StreamingCollection<BuildInformationNode>();
      information.ForEach((Action<BuildInformationNode>) (x => this.m_information.Enqueue(x)));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetail Uri={0} BuildNumber={1} Status={2} CompilationStatus={3} TestStatus={4}]", (object) this.Uri, (object) this.BuildNumber, (object) this.Status, (object) this.CompilationStatus, (object) this.TestStatus);
  }
}
