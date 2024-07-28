// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.BuildGroupItem2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System.Diagnostics;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [XmlInclude(typeof (BuildDefinition))]
  [XmlInclude(typeof (BuildDetail))]
  [XmlType("BuildGroupItem")]
  public abstract class BuildGroupItem2010 : IValidatable
  {
    private string m_name;

    [XmlAttribute]
    [ClientType(typeof (System.Uri))]
    [ClientProperty(ClientVisibility.Public, ClientVisibility.Internal)]
    public string Uri { get; set; }

    [XmlIgnore]
    public string Name
    {
      get
      {
        if (this.m_name == null)
          this.m_name = BuildPath.GetItemName(this.FullPath);
        return this.m_name;
      }
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Private)]
    public string FullPath { get; set; }

    internal TeamProject TeamProject { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context) => this.Validate(requestContext, context);

    internal void Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      if (context == ValidationContext.Update)
        ArgumentValidation.CheckUri("Uri", this.Uri, false, ResourceStrings.MissingUri());
      string fullPath = this.FullPath;
      ArgumentValidation.CheckItemPath("FullPath", ref fullPath, false, false);
      this.FullPath = fullPath;
      this.AssignTeamProject(requestContext);
    }

    internal virtual void AssignTeamProject(IVssRequestContext requestContext)
    {
      this.TeamProject = requestContext.GetService<IProjectService>().GetTeamProjectFromGuidOrName(requestContext, BuildPath.GetTeamProject(this.FullPath));
      requestContext.Trace(0, TraceLevel.Info, "Build", "Service", "Set team project '{0}' on build group item '{1}'", (object) this.TeamProject.Name, (object) this.Uri);
    }
  }
}
