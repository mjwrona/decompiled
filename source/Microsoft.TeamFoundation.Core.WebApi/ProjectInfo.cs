// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.ProjectInfo
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi.Internal;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [ServiceEventObject]
  [DataContract]
  public class ProjectInfo : IEquatable<object>
  {
    public const int c_maxProjectAbbreviationLength = 3;

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Abbreviation { get; set; }

    [DataMember]
    public string Description { get; set; }

    [DataMember]
    public ProjectVisibility Visibility { get; set; }

    [DataMember]
    public ProjectState State { get; set; }

    [DataMember]
    public IList<ProjectProperty> Properties { get; set; }

    [DataMember]
    public long Revision { get; set; }

    [IgnoreDataMember]
    public Guid UserId { get; set; }

    [DataMember]
    public DateTime LastUpdateTime { get; set; }

    [DataMember]
    public long Version
    {
      set
      {
      }
      get => 0;
    }

    [IgnoreDataMember]
    public IList<string> KnownNames { get; set; }

    [IgnoreDataMember]
    public bool IsProjectForUpdate { get; private set; }

    [IgnoreDataMember]
    public bool IsSoftDeleted => this.Visibility == ProjectVisibility.SystemPrivate && ProjectInfo.IsDGuid(this.Name);

    public ProjectInfo()
    {
      this.Properties = (IList<ProjectProperty>) new List<ProjectProperty>();
      this.KnownNames = (IList<string>) new List<string>();
      this.LastUpdateTime = DateTime.UtcNow;
    }

    public ProjectInfo(
      Guid id,
      string name,
      ProjectState state,
      string abbreviation = null,
      string description = null)
    {
      this.Uri = ProjectInfo.GetProjectUri(id);
      this.Id = id;
      this.Name = name;
      this.Abbreviation = abbreviation;
      this.Description = description;
      this.State = state;
      this.Properties = (IList<ProjectProperty>) new List<ProjectProperty>();
      this.KnownNames = (IList<string>) new List<string>()
      {
        name
      };
      this.LastUpdateTime = DateTime.UtcNow;
    }

    public ProjectInfo(
      Guid id,
      string name,
      ProjectState state,
      ProjectVisibility visibility,
      string abbreviation = null,
      string description = null)
      : this(id, name, state, abbreviation, description)
    {
      this.Visibility = visibility;
    }

    public ProjectInfo Clone()
    {
      ProjectInfo projectInfo = new ProjectInfo()
      {
        Id = this.Id,
        Uri = ProjectInfo.GetProjectUri(this.Id),
        Name = this.Name,
        Abbreviation = this.Abbreviation,
        Description = this.Description,
        State = this.State,
        Properties = (IList<ProjectProperty>) null,
        KnownNames = (IList<string>) null,
        LastUpdateTime = this.LastUpdateTime,
        Revision = this.Revision,
        UserId = this.UserId,
        Visibility = this.Visibility
      };
      if (this.Properties != null)
      {
        projectInfo.Properties = (IList<ProjectProperty>) new List<ProjectProperty>(this.Properties.Count);
        foreach (ProjectProperty property in (IEnumerable<ProjectProperty>) this.Properties)
          projectInfo.Properties.Add(property == null ? (ProjectProperty) null : property.Clone());
      }
      if (this.KnownNames != null)
        projectInfo.KnownNames = (IList<string>) new List<string>((IEnumerable<string>) this.KnownNames);
      return projectInfo;
    }

    public override string ToString() => this.Name;

    public override bool Equals(object that) => that != null && that is ProjectInfo && this.Id == ((ProjectInfo) that).Id;

    public override int GetHashCode() => this.Id.GetHashCode();

    public void SetProjectUri() => this.Uri = ProjectInfo.GetProjectUri(this.Id);

    public static string GetProjectUri(string id)
    {
      Guid result;
      if (!Guid.TryParse(id, out result))
        throw new ArgumentException(WebApiResources.ProjectUriIdError((object) id));
      return ProjectInfo.GetProjectUri(result);
    }

    public static string GetProjectUri(Guid id) => LinkingUtilities.EncodeUri(new ArtifactId()
    {
      Tool = "Classification",
      ArtifactType = "TeamProject",
      ToolSpecificId = id.ToString()
    });

    public static Guid GetProjectId(string uri)
    {
      ArtifactId artifactId = LinkingUtilities.DecodeUri(uri.Trim());
      Guid result;
      if (StringComparer.OrdinalIgnoreCase.Equals(artifactId.Tool, "Classification") && StringComparer.OrdinalIgnoreCase.Equals(artifactId.ArtifactType, "TeamProject") && Guid.TryParse(artifactId.ToolSpecificId, out result))
        return result;
      throw new ArgumentException(WebApiResources.CSS_INVALID_URI((object) "URI", (object) uri));
    }

    public static ProjectInfo GetProjectToUpdate(Guid id) => new ProjectInfo()
    {
      Id = id,
      Uri = ProjectInfo.GetProjectUri(id),
      State = ProjectState.Unchanged,
      Properties = (IList<ProjectProperty>) null,
      IsProjectForUpdate = true,
      Visibility = ProjectVisibility.Unchanged
    };

    public static string NormalizeProjectUri(string projectUri)
    {
      projectUri = projectUri != null ? projectUri.Trim() : throw new ArgumentNullException(nameof (projectUri));
      return !string.IsNullOrEmpty(projectUri) ? projectUri : throw new ArgumentException(WebApiResources.CSS_EMPTY_ARGUMENT((object) nameof (projectUri)));
    }

    public static string NormalizeProjectName(
      string projectName,
      string parameterName,
      bool allowGuid = false,
      bool checkValidity = false)
    {
      projectName = projectName != null ? projectName.Trim() : throw new ArgumentNullException(parameterName);
      if (string.IsNullOrEmpty(projectName))
        throw new ArgumentException(WebApiResources.CSS_EMPTY_ARGUMENT((object) parameterName));
      if (!allowGuid && Guid.TryParse(projectName, out Guid _))
        throw new InvalidProjectNameException(projectName);
      return !checkValidity || CssUtils.IsValidProjectName(projectName) ? projectName : throw new InvalidProjectNameException(projectName);
    }

    public static string NormalizeProjectAbbreviation(
      string projectAbbreviation,
      string parameterName)
    {
      if (projectAbbreviation == null)
        return (string) null;
      projectAbbreviation = projectAbbreviation.Trim();
      return projectAbbreviation.Length <= 3 ? projectAbbreviation : throw new ArgumentException(string.Format("Project abbreviation cannot be longer than {0} characters.", (object) 3));
    }

    private static bool IsDGuid(string projectName) => projectName != null && projectName.Length == 37 && (projectName[0].Equals('D') || projectName[0].Equals('d')) && Guid.TryParse(projectName.Substring(1), out Guid _);
  }
}
