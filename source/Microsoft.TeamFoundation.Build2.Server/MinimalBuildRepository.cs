// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.MinimalBuildRepository
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class MinimalBuildRepository
  {
    private string m_type;

    [DataMember]
    public string Type
    {
      get => this.m_type;
      set
      {
        if (string.Equals(value, "TfsVersionControl", StringComparison.OrdinalIgnoreCase))
          this.m_type = "TfsVersionControl";
        else if (string.Equals(value, "TfsGit", StringComparison.OrdinalIgnoreCase))
          this.m_type = "TfsGit";
        else if (string.Equals(value, "Git", StringComparison.OrdinalIgnoreCase))
          this.m_type = "Git";
        else if (string.Equals(value, "GitHub", StringComparison.OrdinalIgnoreCase))
          this.m_type = "GitHub";
        else if (string.Equals(value, "GitHubEnterprise", StringComparison.OrdinalIgnoreCase))
          this.m_type = "GitHubEnterprise";
        else if (string.Equals(value, "Bitbucket", StringComparison.OrdinalIgnoreCase))
          this.m_type = "Bitbucket";
        else if (string.Equals(value, "Svn", StringComparison.OrdinalIgnoreCase))
          this.m_type = "Svn";
        else
          this.m_type = value;
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public string Id { get; set; }
  }
}
