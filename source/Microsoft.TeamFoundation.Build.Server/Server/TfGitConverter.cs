// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TfGitConverter
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class TfGitConverter : IProjectGuidConverter, IProjectNameConverter
  {
    public void ConvertToProjectGuid(IVssRequestContext requestContext)
    {
      if (this.SourceProvider == null || !BuildSourceProviders.IsTfGit(this.SourceProvider.Name) || this.ProjectInfo == null || !((RepoScope) this.RepoKey != (RepoScope) null))
        return;
      NameValueField nameValueField1 = this.SourceProvider.Fields.Where<NameValueField>((Func<NameValueField, bool>) (x => string.Equals(x.Name, BuildSourceProviders.GitProperties.RepositoryName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<NameValueField>();
      NameValueField nameValueField2 = this.SourceProvider.Fields.Where<NameValueField>((Func<NameValueField, bool>) (x => string.Equals(x.Name, BuildSourceProviders.GitProperties.RepositoryUrl, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<NameValueField>();
      if (nameValueField1 != null)
        nameValueField1.Value = BuildSourceProviders.GitProperties.CreateUniqueRepoName(this.ProjectInfo.Id.ToString("D"), this.RepoKey.RepoId.ToString("D"));
      if (nameValueField2 == null)
        return;
      this.SourceProvider.Fields.Remove(nameValueField2);
    }

    public void ConvertToProjectName(IVssRequestContext requestContext)
    {
      if (this.SourceProvider == null || !BuildSourceProviders.IsTfGit(this.SourceProvider.Name) || this.ProjectInfo == null || this.RepoName == null || this.RepoUri == null)
        return;
      NameValueField nameValueField1 = this.SourceProvider.Fields.Where<NameValueField>((Func<NameValueField, bool>) (x => string.Equals(x.Name, BuildSourceProviders.GitProperties.RepositoryName, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<NameValueField>();
      NameValueField nameValueField2 = this.SourceProvider.Fields.Where<NameValueField>((Func<NameValueField, bool>) (x => string.Equals(x.Name, BuildSourceProviders.GitProperties.RepositoryUrl, StringComparison.OrdinalIgnoreCase))).FirstOrDefault<NameValueField>();
      if (nameValueField1 == null)
        return;
      nameValueField1.Value = BuildSourceProviders.GitProperties.CreateUniqueRepoName(this.ProjectInfo.Name, this.RepoName);
      if (nameValueField2 == null)
      {
        nameValueField2 = new NameValueField(BuildSourceProviders.GitProperties.RepositoryUrl, "");
        this.SourceProvider.Fields.Add(nameValueField2);
      }
      nameValueField2.Value = this.RepoUri;
    }

    public ProjectInfo ProjectInfo { get; set; }

    public BuildDefinitionSourceProvider SourceProvider { get; set; }

    public RepoKey RepoKey { get; set; }

    public string RepoName { get; set; }

    public string RepoUri { get; set; }
  }
}
