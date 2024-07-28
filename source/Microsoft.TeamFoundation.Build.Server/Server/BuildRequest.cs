// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildRequest
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [CallOnSerialization("BeforeSerialize")]
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildRequest : IValidatable
  {
    public BuildRequest()
    {
      this.BatchId = BuildWellKnownBatchIds.DynamicBatch;
      this.GetOption = GetOption.LatestOnBuild;
      this.MaxQueuePosition = int.MaxValue;
      this.Priority = QueuePriority.Normal;
      this.Reason = BuildReason.Manual;
    }

    public BuildRequest(
      string buildControllerUri,
      string buildDefinitionUri,
      string dropLocation,
      QueuePriority priority,
      string processParameters,
      BuildReason reason)
    {
      this.BatchId = BuildWellKnownBatchIds.DynamicBatch;
      this.GetOption = GetOption.LatestOnBuild;
      this.MaxQueuePosition = int.MaxValue;
      this.BuildControllerUri = buildControllerUri;
      this.BuildDefinitionUri = buildDefinitionUri;
      this.DropLocation = dropLocation;
      this.Priority = priority;
      this.ProcessParameters = processParameters;
      this.Reason = reason;
    }

    [XmlAttribute]
    [DefaultValue(typeof (Guid), "00000000-0000-0000-0000-000000000000")]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public Guid BatchId { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildDefinitionUri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly, PropertyName = "GatedCheckInTicket")]
    public string CheckInTicket { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string CustomGetVersion { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string DropLocation { get; set; }

    [XmlAttribute]
    [DefaultValue(GetOption.LatestOnBuild)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public GetOption GetOption { get; set; }

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxQueuePosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Postponed { get; set; }

    [XmlAttribute]
    [DefaultValue(QueuePriority.Normal)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuePriority Priority { get; set; }

    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason.Manual)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildReason Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string ShelvesetName { get; set; }

    internal string BuildUri { get; set; }

    internal Guid ProjectId { get; set; }

    internal TeamFoundationIdentity RequestedForIdentity { get; private set; }

    internal TeamFoundationIdentity RequestedByIdentity { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("BuildControllerUri", this.BuildControllerUri, "Controller", true, (string) null);
      ArgumentValidation.CheckUri("BuildDefinitionUri", this.BuildDefinitionUri, "Definition", false, ResourceStrings.MissingDefinition());
      ArgumentValidation.CheckBound("MaxQueuePosition", this.MaxQueuePosition, 0);
      string path = this.DropLocation;
      if (!string.IsNullOrEmpty(path) && VersionControlPath.IsServerItem(path))
        path = VersionControlPath.GetFullPath(path);
      else if (BuildContainerPath.IsServerPath(path))
        path = BuildContainerPath.GetFullPath(path);
      else
        ArgumentValidation.CheckDropLocation("DropLocation", ref path, true, (string) null);
      this.DropLocation = path;
      this.RequestedByIdentity = requestContext.GetService<TeamFoundationIdentityService>().ReadRequestIdentity(requestContext);
      if (string.IsNullOrEmpty(this.RequestedFor))
      {
        this.RequestedForIdentity = this.RequestedByIdentity;
      }
      else
      {
        this.RequestedForIdentity = Validation.ResolveIdentity(requestContext, this.RequestedFor);
        if (this.RequestedForIdentity == null)
          throw new ArgumentException(ResourceStrings.InvalidIdentityNotFound((object) this.RequestedFor));
      }
      if (this.GetOption == GetOption.Custom && string.IsNullOrEmpty(this.CustomGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionRequired((object) "Custom"));
      if (this.GetOption != GetOption.Custom && !string.IsNullOrEmpty(this.CustomGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionIgnored((object) "Custom"));
      if (!string.IsNullOrEmpty(this.CustomGetVersion))
        VersionSpec.ParseSingleSpec(this.CustomGetVersion, this.RequestedByIdentity.DisplayName, this.RequestedByIdentity.UniqueName);
      if (this.Reason != BuildReason.Manual && this.Reason != BuildReason.ValidateShelveset && this.Reason != BuildReason.CheckInShelveset)
      {
        if (this.Reason == BuildReason.IndividualCI || this.Reason == BuildReason.BatchedCI || this.Reason == BuildReason.Schedule || this.Reason == BuildReason.ScheduleForced)
          throw new ArgumentException(ResourceStrings.BuildRequestReasonReserved((object) this.Reason.ToString()));
        throw new ArgumentException(ResourceStrings.BuildRequestReasonInvalid((object) this.Reason.ToString()));
      }
      if (this.Reason == BuildReason.ValidateShelveset || this.Reason == BuildReason.CheckInShelveset)
      {
        ArgumentValidation.CheckShelvesetName("ShelvesetName", this.ShelvesetName, false);
        if (this.Reason == BuildReason.CheckInShelveset && this.GetOption != GetOption.LatestOnBuild)
          throw new ArgumentException(ResourceStrings.BuildRequestGetOptionInvalid((object) "LatestOnBuild", (object) "CheckInShelveset"));
      }
      else if (!string.IsNullOrEmpty(this.ShelvesetName))
        throw new ArgumentException(ResourceStrings.BuildRequestShelvesetIgnored((object) "CheckInShelveset", (object) "ValidateShelveset"));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildRequest BuildDefinitionUri={0} BuildControllerUri={1} BatchId={2} Priority={3} Reason={4}]", (object) this.BuildDefinitionUri, (object) this.BuildControllerUri, (object) this.BatchId, (object) this.Priority, (object) this.Reason);
  }
}
