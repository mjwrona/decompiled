// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.ProcessTemplate2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.VersionControl.Common;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal)]
  [RequiredClientService("BuildServer")]
  [XmlType("ProcessTemplate")]
  public sealed class ProcessTemplate2010 : IValidatable
  {
    private Microsoft.TeamFoundation.Build.Server.TeamProject m_teamProjectObj;
    private string m_teamProject;

    public ProcessTemplate2010()
    {
      this.SupportedReasons = BuildReason2010.Manual | BuildReason2010.IndividualCI | BuildReason2010.BatchedCI | BuildReason2010.Schedule | BuildReason2010.ScheduleForced | BuildReason2010.UserCreated;
      this.TemplateType = ProcessTemplateType2010.Custom;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string TeamProject
    {
      get => this.m_teamProjectObj != null ? this.m_teamProjectObj.Name : this.m_teamProject;
      set => this.m_teamProject = value;
    }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string ServerPath { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string Description { get; set; }

    [XmlAttribute]
    [DefaultValue(BuildReason2010.Manual | BuildReason2010.IndividualCI | BuildReason2010.BatchedCI | BuildReason2010.Schedule | BuildReason2010.ScheduleForced | BuildReason2010.UserCreated)]
    [ClientProperty(ClientVisibility.Public)]
    public BuildReason2010 SupportedReasons { get; set; }

    [XmlAttribute]
    [DefaultValue(ProcessTemplateType2010.Custom)]
    [ClientProperty(ClientVisibility.Public)]
    public ProcessTemplateType2010 TemplateType { get; set; }

    [XmlAttribute]
    [DefaultValue(-1)]
    [ClientProperty(ClientVisibility.Public)]
    public int Id { get; set; }

    [XmlElement]
    [ClientProperty(ClientVisibility.Public)]
    public string Parameters { get; set; }

    [ClientProperty(ClientVisibility.Public)]
    public bool FileExists { get; set; }

    internal Microsoft.TeamFoundation.Build.Server.TeamProject TeamProjectObj
    {
      get => this.m_teamProjectObj;
      set
      {
        this.m_teamProjectObj = value;
        if (this.m_teamProjectObj == null)
          return;
        this.m_teamProject = this.m_teamProjectObj.Name;
      }
    }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      this.ServerPath = !string.IsNullOrEmpty(this.ServerPath) ? VersionControlPath.GetFullPath(this.ServerPath) : throw new ArgumentException(BuildTypeResource.InvalidInputParameterNull((object) "ServerPath"));
      ProjectInfo project = requestContext.GetService<IProjectService>().GetProject(requestContext, this.m_teamProject);
      this.m_teamProjectObj = new Microsoft.TeamFoundation.Build.Server.TeamProject(project.Uri, project.Name);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[ProcessTemplate2010 Id={0} ServerPath={1} TeamProject={2} TemplateType={3}]", (object) this.Id, (object) this.ServerPath, (object) this.TeamProject, (object) this.TemplateType);
  }
}
