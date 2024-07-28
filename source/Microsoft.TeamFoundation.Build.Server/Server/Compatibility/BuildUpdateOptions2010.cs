// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildUpdateOptions2010
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

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildUpdateOptions")]
  public sealed class BuildUpdateOptions2010 : Microsoft.TeamFoundation.Build.Server.IValidatable
  {
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildUpdate2010 Fields { get; set; }

    [XmlAttribute]
    public string BuildNumber { get; set; }

    [XmlAttribute]
    public string DropLocation { get; set; }

    [XmlAttribute]
    public string LabelName { get; set; }

    [XmlAttribute]
    public string LogLocation { get; set; }

    [XmlAttribute]
    public BuildStatus2010 Status { get; set; }

    [XmlAttribute]
    public string Quality { get; set; }

    [XmlAttribute]
    public BuildPhaseStatus2010 CompilationStatus { get; set; }

    [XmlAttribute]
    public BuildPhaseStatus2010 TestStatus { get; set; }

    [XmlAttribute]
    public bool KeepForever { get; set; }

    [XmlAttribute]
    public string SourceGetVersion { get; set; }

    void Microsoft.TeamFoundation.Build.Server.IValidatable.Validate(
      IVssRequestContext requestContext,
      ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Build", false, ResourceStrings.MissingUri());
      if ((this.Fields & BuildUpdate2010.BuildNumber) == BuildUpdate2010.BuildNumber)
        ArgumentValidation.CheckBuildNumber("BuildNumber", this.BuildNumber, false);
      if ((this.Fields & BuildUpdate2010.DropLocation) == BuildUpdate2010.DropLocation)
      {
        string dropLocation = this.DropLocation;
        ArgumentValidation.CheckDropLocation("DropLocation", ref dropLocation, false, (string) null);
        this.DropLocation = dropLocation;
      }
      if ((this.Fields & BuildUpdate2010.LogLocation) == BuildUpdate2010.LogLocation)
      {
        string logLocation = this.LogLocation;
        ArgumentValidation.CheckLogLocation("LogLocation", ref logLocation, false, (string) null);
        this.LogLocation = logLocation;
      }
      if ((this.Fields & BuildUpdate2010.Quality) == BuildUpdate2010.Quality)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckQuality("Quality", this.Quality, true, (string) null);
      if ((this.Fields & BuildUpdate2010.SourceGetVersion) == BuildUpdate2010.SourceGetVersion)
        VersionSpec.ParseSingleSpec(this.SourceGetVersion, ".");
      if ((this.Fields & BuildUpdate2010.Status) == BuildUpdate2010.Status && !Enum.IsDefined(typeof (BuildStatus2010), (object) this.Status))
        throw new ArgumentException(ResourceStrings.BuildStatusMustBeSingleValue((object) this.Status.ToString()));
      if ((this.Fields & BuildUpdate2010.LabelName) != BuildUpdate2010.LabelName)
        return;
      string labelName;
      string labelScope;
      LabelSpec.Parse(this.LabelName, (string) null, false, out labelName, out labelScope);
      if (!LabelSpec.IsLegalName(labelName, false))
        throw new ArgumentException(ResourceStrings.LabelNameInvalid((object) labelName));
      if (labelScope == null)
        throw new ArgumentException(ResourceStrings.LabelRequiresScope((object) this.LabelName));
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildDetailUpdate2010 Uri={0} BuildNumber={1} Fields={2}]", (object) this.Uri, (object) this.BuildNumber, (object) this.Fields);
  }
}
