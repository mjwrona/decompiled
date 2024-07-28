// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildRequest2010
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
  [XmlType("BuildRequest")]
  public sealed class BuildRequest2010 : IValidatable
  {
    public BuildRequest2010()
    {
      this.GetOption = GetOption2010.LatestOnBuild;
      this.MaxQueuePosition = int.MaxValue;
      this.Priority = QueuePriority2010.Normal;
      this.Reason = BuildReason2010.Manual;
    }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildControllerUri { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
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
    [DefaultValue(GetOption2010.LatestOnBuild)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public GetOption2010 GetOption { get; set; }

    [XmlAttribute]
    [DefaultValue(2147483647)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public int MaxQueuePosition { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public bool Postponed { get; set; }

    [XmlAttribute]
    [DefaultValue(QueuePriority2010.Normal)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public QueuePriority2010 Priority { get; set; }

    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string ProcessParameters { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.Manual)]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public BuildReason2010 Reason { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string RequestedFor { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string ShelvesetName { get; set; }

    [XmlAttribute]
    [ClientType(typeof (Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal, Direction = ClientPropertySerialization.ClientToServerOnly)]
    public string BuildAgentUri { get; set; }

    internal string BuildUri { get; set; }

    internal string RequestedBy { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("BuildControllerUri", this.BuildControllerUri, "Controller", true, (string) null);
      ArgumentValidation.CheckUri("BuildDefinitionUri", this.BuildDefinitionUri, "Definition", false, ResourceStrings.MissingDefinition());
      ArgumentValidation.CheckBound("MaxQueuePosition", this.MaxQueuePosition, 0);
      Microsoft.TeamFoundation.Build.Server.Validation.CheckIdentityName("RequestedFor", this.RequestedFor, true);
      string dropLocation = this.DropLocation;
      ArgumentValidation.CheckDropLocation("DropLocation", ref dropLocation, true, (string) null);
      this.DropLocation = dropLocation;
      if (this.GetOption == GetOption2010.Custom && string.IsNullOrEmpty(this.CustomGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionRequired((object) "Custom"));
      if (this.GetOption != GetOption2010.Custom && !string.IsNullOrEmpty(this.CustomGetVersion))
        throw new ArgumentException(ResourceStrings.BuildRequestCustomGetVersionIgnored((object) "Custom"));
      if (this.Reason != BuildReason2010.Manual && this.Reason != BuildReason2010.ValidateShelveset && this.Reason != BuildReason2010.CheckInShelveset)
      {
        if (this.Reason == BuildReason2010.IndividualCI || this.Reason == BuildReason2010.BatchedCI || this.Reason == BuildReason2010.Schedule || this.Reason == BuildReason2010.ScheduleForced)
          throw new ArgumentException(ResourceStrings.BuildRequestReasonReserved((object) this.Reason.ToString()));
        throw new ArgumentException(ResourceStrings.BuildRequestReasonInvalid((object) this.Reason.ToString()));
      }
      if (this.Reason == BuildReason2010.ValidateShelveset || this.Reason == BuildReason2010.CheckInShelveset)
      {
        ArgumentValidation.CheckShelvesetName("ShelvesetName", this.ShelvesetName, false);
        if (this.Reason == BuildReason2010.CheckInShelveset && this.GetOption != GetOption2010.LatestOnBuild)
          throw new ArgumentException(ResourceStrings.BuildRequestGetOptionInvalid((object) "LatestOnBuild", (object) "CheckInShelveset"));
      }
      else if (!string.IsNullOrEmpty(this.ShelvesetName))
        throw new ArgumentException(ResourceStrings.BuildRequestShelvesetIgnored((object) "CheckInShelveset", (object) "ValidateShelveset"));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildRequest2010 BuildDefinitionUri={0} BuildControllerUri={1} Priority={2} Reason={3}]", (object) this.BuildDefinitionUri, (object) this.BuildControllerUri, (object) this.Priority, (object) this.Reason);
  }
}
