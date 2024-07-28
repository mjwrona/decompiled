// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitRepositoryPolicySettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  [DataContract]
  public class TeamFoundationGitRepositoryPolicySettings : 
    ITeamFoundationGitRepositoryPolicySettings,
    ITeamFoundationGitPolicySettings
  {
    [JsonProperty(Required = Required.Always)]
    public GitPolicyRepositoryScope Scope { get; private set; }

    GitScope ITeamFoundationGitPolicySettings.Scope => (GitScope) this.Scope;

    [JsonConstructor]
    internal TeamFoundationGitRepositoryPolicySettings(GitPolicyRepositoryScope scope = null) => this.Scope = scope;
  }
}
