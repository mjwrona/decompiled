// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildControllerUpdateOptions2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("BuildControllerUpdateOptions")]
  public sealed class BuildControllerUpdateOptions2010 : IValidatable
  {
    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildControllerUpdate2010 Fields { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Name { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Description { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string CustomAssemblyPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public int MaxConcurrentBuilds { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public ControllerStatus2010 Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public bool Enabled { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Controller", false, (string) null);
      if ((this.Fields & BuildControllerUpdate2010.Description) == BuildControllerUpdate2010.Description)
        Microsoft.TeamFoundation.Build.Server.Validation.CheckDescription("Description", this.Description, true);
      if ((this.Fields & BuildControllerUpdate2010.Name) == BuildControllerUpdate2010.Name)
        Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.Name, false);
      if ((this.Fields & BuildControllerUpdate2010.CustomAssemblyPath) == BuildControllerUpdate2010.CustomAssemblyPath)
      {
        string customAssemblyPath = this.CustomAssemblyPath;
        Microsoft.TeamFoundation.Build.Server.Validation.CheckVersionControlPath("CustomAssemblyPath", ref customAssemblyPath, true);
        this.CustomAssemblyPath = customAssemblyPath;
        if (!string.IsNullOrEmpty(this.CustomAssemblyPath) && VersionControlPath.Equals(this.CustomAssemblyPath, "$/"))
          throw new ArgumentException(ResourceStrings.InvalidCustomAssemblyPath());
      }
      if ((this.Fields & BuildControllerUpdate2010.MaxConcurrentBuilds) != BuildControllerUpdate2010.MaxConcurrentBuilds)
        return;
      ArgumentValidation.CheckBound("MaxConcurrentBuilds", this.MaxConcurrentBuilds, 0, int.MaxValue);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildControllerUpdate2010 Uri={0} Name={1} Fields={2}]", (object) this.Uri, (object) this.Name, (object) this.Fields);
  }
}
