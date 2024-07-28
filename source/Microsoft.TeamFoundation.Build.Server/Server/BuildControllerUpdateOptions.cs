// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildControllerUpdateOptions
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType(Namespace = "http://schemas.microsoft.com/TeamFoundation/2010/Build")]
  public sealed class BuildControllerUpdateOptions : IValidatable, IUpdatePropertyProvider
  {
    private List<PropertyValue> m_attachedProperties = new List<PropertyValue>();

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public BuildControllerUpdate Fields { get; set; }

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
    public ControllerStatus Status { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public string StatusMessage { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public bool Enabled { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal)]
    public List<PropertyValue> AttachedProperties
    {
      get => this.m_attachedProperties;
      set => this.m_attachedProperties = value;
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.CheckUri("Uri", this.Uri, "Controller", false, (string) null);
      if ((this.Fields & BuildControllerUpdate.Description) == BuildControllerUpdate.Description)
        Validation.CheckDescription("Description", this.Description, true);
      if ((this.Fields & BuildControllerUpdate.Name) == BuildControllerUpdate.Name)
        Microsoft.TeamFoundation.Build.Common.Validation.CheckValidControllerName(this.Name, false);
      if ((this.Fields & BuildControllerUpdate.CustomAssemblyPath) == BuildControllerUpdate.CustomAssemblyPath)
      {
        string customAssemblyPath = this.CustomAssemblyPath;
        if (BuildSourceProviders.GitProperties.IsGitUri(customAssemblyPath))
        {
          if (!BuildSourceProviders.GitProperties.ParseGitPath(customAssemblyPath, out string _, out string _, out string _))
            throw new ArgumentException(ResourceStrings.InvalidCustomAssemblyPath());
        }
        else
          Validation.CheckVersionControlPath("CustomAssemblyPath", ref customAssemblyPath, true);
        this.CustomAssemblyPath = customAssemblyPath;
        if (!string.IsNullOrEmpty(this.CustomAssemblyPath) && VersionControlPath.Equals(this.CustomAssemblyPath, "$/"))
          throw new ArgumentException(ResourceStrings.InvalidCustomAssemblyPath());
      }
      if ((this.Fields & BuildControllerUpdate.MaxConcurrentBuilds) != BuildControllerUpdate.MaxConcurrentBuilds)
        return;
      ArgumentValidation.CheckBound("MaxConcurrentBuilds", this.MaxConcurrentBuilds, 0, int.MaxValue);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[BuildControllerUpdate Uri={0} Name={1} Fields={2}]", (object) this.Uri, (object) this.Name, (object) this.Fields);
  }
}
