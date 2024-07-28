// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.ArtifactSource
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.FormInput;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public class ArtifactSource
  {
    public int Id { get; set; }

    public string ArtifactTypeId { get; set; }

    public string SourceId { get; set; }

    public Dictionary<string, InputValue> SourceData { get; set; }

    public string Alias { get; set; }

    public string SourceBranch { get; set; }

    public bool IsPrimary { get; set; }

    public bool IsRetained { get; set; }

    public ArtifactSource() => this.SourceData = new Dictionary<string, InputValue>();

    public InputValue IsMultiDefinitionTypeData
    {
      get
      {
        InputValue inputValue;
        return this.SourceData.TryGetValue("IsMultiDefinitionType", out inputValue) ? inputValue : (InputValue) null;
      }
    }

    public InputValue ProjectData
    {
      get
      {
        InputValue inputValue;
        return this.SourceData.TryGetValue("project", out inputValue) ? inputValue : (InputValue) null;
      }
    }

    public InputValue DefinitionsData
    {
      get
      {
        string key = "definition";
        if (this.IsMultiDefinitionType && this.SourceData.ContainsKey("definitions") && this.SourceData["definitions"] != null && !this.SourceData["definitions"].Value.IsNullOrEmpty<char>())
          key = "definitions";
        InputValue inputValue;
        return this.SourceData.TryGetValue(key, out inputValue) ? inputValue : (InputValue) null;
      }
    }

    public Guid TeamProjectId
    {
      get
      {
        Guid result;
        return this.ProjectData == null || !Guid.TryParse(this.ProjectData.Value, out result) ? Guid.Empty : result;
      }
    }

    public bool IsMultiDefinitionType
    {
      get
      {
        bool result;
        return this.IsMultiDefinitionTypeData != null && bool.TryParse(this.IsMultiDefinitionTypeData.Value, out result) && result;
      }
    }

    public bool IsBuildArtifact => string.Equals(this.ArtifactTypeId, "Build", StringComparison.OrdinalIgnoreCase);

    public bool IsPackageManagementArtifact => string.Equals(this.ArtifactTypeId, "PackageManagement", StringComparison.OrdinalIgnoreCase);

    public bool IsGitArtifact => string.Equals(this.ArtifactTypeId, "Git", StringComparison.OrdinalIgnoreCase);

    public bool IsGitHubArtifact => string.Equals(this.ArtifactTypeId, "GitHub", StringComparison.OrdinalIgnoreCase);

    public bool IsDockerHubArtifact => string.Equals(this.ArtifactTypeId, "DockerHub", StringComparison.OrdinalIgnoreCase);

    public bool SupportsWorkItemLinking => this.IsBuildArtifact;

    public bool IsAzureContainerRepositoryArtifact => string.Equals(this.ArtifactTypeId, "AzureContainerRepository", StringComparison.OrdinalIgnoreCase);

    public void CopySourceDataFrom(ArtifactSource originArtifactSource, bool overwriteExistingKeys)
    {
      if (originArtifactSource == null)
        return;
      foreach (KeyValuePair<string, InputValue> keyValuePair in originArtifactSource.SourceData)
      {
        bool flag = this.SourceData.ContainsKey(keyValuePair.Key);
        if (!flag || flag & overwriteExistingKeys)
          this.SourceData[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public string GetSourceBranchName()
    {
      if (!this.SourceBranch.IsNullOrEmpty<char>())
        return this.SourceBranch;
      return this.SourceData != null && this.SourceData.ContainsKey("branch") && this.SourceData["branch"] != null ? this.SourceData["branch"].ToString() : string.Empty;
    }

    public ArtifactSource Clone()
    {
      ArtifactSource artifactSource = new ArtifactSource();
      artifactSource.ArtifactTypeId = this.ArtifactTypeId;
      artifactSource.SourceId = this.SourceId;
      artifactSource.Id = this.Id;
      artifactSource.Alias = this.Alias;
      artifactSource.IsPrimary = this.IsPrimary;
      artifactSource.CopySourceDataFrom(this, true);
      return artifactSource;
    }

    public bool IsXamlBuildArtifact(IVssRequestContext requestContext)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      InputValue inputValue1 = (InputValue) null;
      if (this.SourceData.ContainsKey("IsXamlBuildArtifactType"))
        this.SourceData.TryGetValue("IsXamlBuildArtifactType", out inputValue1);
      else if (this.SourceData.ContainsKey("isXaml"))
        this.SourceData.TryGetValue("isXaml", out inputValue1);
      bool result1;
      if (inputValue1 != null && bool.TryParse(inputValue1.Value, out result1))
        return result1;
      int result2 = 0;
      InputValue inputValue2;
      if (this.SourceData.TryGetValue("definition", out inputValue2) && !inputValue2.Value.IsNullOrEmpty<char>())
        result2 = int.TryParse(inputValue2.Value, out result2) ? result2 : 0;
      try
      {
        return requestContext.Elevate().GetClient<XamlBuildHttpClient>().GetDefinitionAsync(this.TeamProjectId, result2).GetAwaiter().GetResult().Type == DefinitionType.Xaml;
      }
      catch (DefinitionNotFoundException ex)
      {
        return false;
      }
    }
  }
}
