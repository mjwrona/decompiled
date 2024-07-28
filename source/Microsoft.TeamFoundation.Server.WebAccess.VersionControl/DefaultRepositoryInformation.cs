// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.VersionControl.DefaultRepositoryInformation
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.VersionControl, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEC6FD7F-E72C-4C65-8428-206D27D3BF89
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.VersionControl.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.VersionControl
{
  [DataContract]
  public class DefaultRepositoryInformation : VersionControlSecuredObject
  {
    private string m_defaultGitRepoUrl;
    private IVssRequestContext m_requestContext;

    public DefaultRepositoryInformation(IVssRequestContext requestContext) => this.m_requestContext = requestContext;

    [DataMember(EmitDefaultValue = false)]
    public Guid DefaultRepoId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DefaultRepoIsGit { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DefaultRepoCanFork { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool DefaultRepoIsFork { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultGitRepoName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultGitRepoUrl
    {
      get
      {
        if (this.m_defaultGitRepoUrl == null && this.DefaultRepoIsGit)
          this.m_defaultGitRepoUrl = Microsoft.TeamFoundation.SourceControl.WebServer.GitUrlGenerator.GetActionUrl(this.m_requestContext, this.DefaultGitRepoName, "index");
        return this.m_defaultGitRepoUrl;
      }
      set => this.m_defaultGitRepoUrl = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public string DefaultGitBranchName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool SupportsTfvc { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string NotFoundRepoName { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string DisabledRepoName { get; set; }
  }
}
