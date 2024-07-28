// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Policy.TeamFoundationGitPullRequestPolicySettings
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.Plugins.Policy;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Git.Server.Policy
{
  [DataContract]
  public class TeamFoundationGitPullRequestPolicySettings : ITeamFoundationGitPolicySettings
  {
    private string[] _filenamePatterns;

    [JsonProperty(Required = Required.Always)]
    [DataMember(Name = "scope", EmitDefaultValue = true)]
    public GitPolicyRefScope Scope { get; private set; }

    [DataMember(Name = "filenamePatterns", EmitDefaultValue = false)]
    public IReadOnlyList<string> FilenamePatterns
    {
      get => (IReadOnlyList<string>) this._filenamePatterns;
      private set
      {
        if (value != null && value.Any<string>())
          this._filenamePatterns = value.Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).ToArray<string>();
        else
          this._filenamePatterns = (string[]) null;
      }
    }

    [DataMember(Name = "ignoreIfSourceIsInScope", EmitDefaultValue = false)]
    public bool IgnoreIfSourceIsInScope { get; private set; }

    [DataMember(Name = "addedFilesOnly", EmitDefaultValue = false)]
    public bool AddedFilesOnly { get; private set; }

    GitScope ITeamFoundationGitPolicySettings.Scope => (GitScope) this.Scope;

    [JsonConstructor]
    protected TeamFoundationGitPullRequestPolicySettings(
      GitPolicyRefScope scope = null,
      IReadOnlyList<string> filenamePatterns = null,
      bool addedFilesOnly = false,
      bool ignoreIfSourceIsInScope = false)
    {
      this.Scope = scope;
      this.FilenamePatterns = filenamePatterns;
      this.AddedFilesOnly = addedFilesOnly;
      this.IgnoreIfSourceIsInScope = ignoreIfSourceIsInScope;
    }
  }
}
