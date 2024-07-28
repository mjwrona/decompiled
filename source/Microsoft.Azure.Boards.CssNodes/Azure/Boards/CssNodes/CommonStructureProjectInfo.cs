// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.CssNodes.CommonStructureProjectInfo
// Assembly: Microsoft.Azure.Boards.CssNodes, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D887A041-2C68-42E5-BA83-E261159AB40A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.CssNodes.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Azure.Boards.CssNodes
{
  [XmlType("ProjectInfo")]
  [ClientType("ProjectInfo")]
  [ClassVisibility(ClientVisibility.Public, ClientVisibility.Public)]
  [ClassNotSealed]
  [Serializable]
  public class CommonStructureProjectInfo
  {
    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Uri { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public string Name { get; set; }

    [ClientProperty(ClientVisibility.Private, ClientVisibility.Private, UseClientDefinedProperty = true)]
    public CommonStructureProjectState Status { get; set; }

    [XmlIgnore]
    internal DateTime LastUpdateTime { get; set; }

    public CommonStructureProjectInfo(string uri, string name, CommonStructureProjectState status)
    {
      this.Uri = uri;
      this.Name = name;
      this.Status = status;
      this.LastUpdateTime = DateTime.UtcNow;
    }

    public CommonStructureProjectInfo() => this.LastUpdateTime = DateTime.UtcNow;

    public override string ToString() => this.Name;

    public override bool Equals(object that) => that != null && that is CommonStructureProjectInfo && this.Uri == ((CommonStructureProjectInfo) that).Uri;

    public override int GetHashCode() => this.Uri.GetHashCode();

    public TeamProjectReference ToProjectReference()
    {
      Guid id;
      CommonStructureUtils.TryParseUri(this.Uri, out id, true);
      return new TeamProjectReference()
      {
        Id = id,
        Name = this.Name,
        State = CommonStructureProjectInfo.ConvertToProjectState(this.Status)
      };
    }

    public static CommonStructureProjectInfo[] ConvertProjectInfo(IList<ProjectInfo> projects)
    {
      CommonStructureProjectInfo[] structureProjectInfoArray = new CommonStructureProjectInfo[projects.Count];
      for (int index = 0; index < projects.Count; ++index)
        structureProjectInfoArray[index] = CommonStructureProjectInfo.ConvertProjectInfo(projects[index]);
      return structureProjectInfoArray;
    }

    public static CommonStructureProjectInfo ConvertProjectInfo(ProjectInfo project) => new CommonStructureProjectInfo(project.Uri, project.Name, CommonStructureProjectInfo.ConvertProjectState(project.State))
    {
      LastUpdateTime = project.LastUpdateTime
    };

    public virtual ProjectInfo ToProjectInfo()
    {
      Guid id;
      CommonStructureUtils.TryParseProjectUri(this.Uri, out id);
      return new ProjectInfo(id, this.Name, CommonStructureProjectInfo.ConvertToProjectState(this.Status))
      {
        LastUpdateTime = this.LastUpdateTime
      };
    }

    public static CommonStructureProjectState ConvertProjectState(ProjectState state)
    {
      switch (state)
      {
        case ProjectState.New:
          return CommonStructureProjectState.New;
        case ProjectState.WellFormed:
          return CommonStructureProjectState.WellFormed;
        case ProjectState.Deleting:
          return CommonStructureProjectState.Deleting;
        case ProjectState.CreatePending:
          return CommonStructureProjectState.CreatePending;
        default:
          throw new ArgumentException();
      }
    }

    public static ProjectState ConvertToProjectState(CommonStructureProjectState state)
    {
      switch (state)
      {
        case CommonStructureProjectState.New:
          return ProjectState.New;
        case CommonStructureProjectState.WellFormed:
          return ProjectState.WellFormed;
        case CommonStructureProjectState.Deleting:
          return ProjectState.Deleting;
        case CommonStructureProjectState.CreatePending:
          return ProjectState.CreatePending;
        default:
          throw new ArgumentException();
      }
    }
  }
}
