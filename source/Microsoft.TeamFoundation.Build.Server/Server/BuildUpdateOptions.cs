// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildUpdateOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using Microsoft.TeamFoundation.VersionControl.Server;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildUpdateOptions : IValidatable
  {
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildUpdate Fields { get; set; }

    [XmlAttribute]
    public string BuildNumber { get; set; }

    [XmlAttribute]
    public string DropLocation { get; set; }

    [XmlAttribute]
    public string LabelName { get; set; }

    [XmlAttribute]
    public string LogLocation { get; set; }

    [XmlAttribute]
    public BuildStatus Status { get; set; }

    [XmlAttribute]
    public string Quality { get; set; }

    [XmlAttribute]
    public BuildPhaseStatus CompilationStatus { get; set; }

    [XmlAttribute]
    public BuildPhaseStatus TestStatus { get; set; }

    [XmlAttribute]
    public bool KeepForever { get; set; }

    [XmlAttribute]
    public string SourceGetVersion { get; set; }

    [XmlIgnore]
    internal string DropLocationRoot { get; set; }

    [XmlIgnore]
    internal long? ContainerId { get; set; }

    [XmlIgnore]
    internal Guid ProjectId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Build", false, ResourceStrings.MissingUri());
      if ((this.Fields & BuildUpdate.BuildNumber) == BuildUpdate.BuildNumber)
        ArgumentValidation.CheckBuildNumber("BuildNumber", this.BuildNumber, false);
      if ((this.Fields & BuildUpdate.DropLocation) == BuildUpdate.DropLocation)
      {
        string dropLocation = this.DropLocation;
        ArgumentValidation.CheckDropLocation("DropLocation", ref dropLocation, false, (string) null);
        this.DropLocation = dropLocation;
      }
      if ((this.Fields & BuildUpdate.LogLocation) == BuildUpdate.LogLocation)
      {
        string logLocation = this.LogLocation;
        ArgumentValidation.CheckLogLocation("LogLocation", ref logLocation, false, (string) null);
        this.LogLocation = logLocation;
      }
      if ((this.Fields & BuildUpdate.Quality) == BuildUpdate.Quality)
        Validation.CheckQuality("Quality", this.Quality, true, (string) null);
      if ((this.Fields & BuildUpdate.SourceGetVersion) == BuildUpdate.SourceGetVersion)
        VersionSpec.ParseSingleSpec(this.SourceGetVersion, string.Empty);
      if ((this.Fields & BuildUpdate.Status) == BuildUpdate.Status && !Enum.IsDefined(typeof (BuildStatus), (object) this.Status))
        throw new ArgumentException(ResourceStrings.BuildStatusMustBeSingleValue((object) this.Status.ToString()));
      if ((this.Fields & BuildUpdate.LabelName) != BuildUpdate.LabelName)
        return;
      string labelName;
      string labelScope;
      LabelSpec.Parse(this.LabelName, (string) null, false, out labelName, out labelScope);
      if (!LabelSpec.IsLegalName(labelName, false))
        throw new ArgumentException(ResourceStrings.LabelNameInvalid((object) labelName));
      if (labelScope == null)
        throw new ArgumentException(ResourceStrings.LabelRequiresScope((object) this.LabelName));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetailUpdate Uri={0} BuildNumber={1} Fields={2}]", (object) this.Uri, (object) this.BuildNumber, (object) this.Fields);
  }
}
