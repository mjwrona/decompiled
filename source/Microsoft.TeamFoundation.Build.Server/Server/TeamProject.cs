// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.TeamProject
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class TeamProject
  {
    private Guid m_id;
    private string m_uri;
    private string m_name;
    private string m_securityToken;

    internal TeamProject(string uri, string name)
    {
      this.m_uri = uri;
      this.m_name = name;
      this.m_id = Guid.Parse(LinkingUtilities.DecodeUri(uri).ToolSpecificId);
    }

    public Guid Id => this.m_id;

    public string Uri => this.m_uri;

    public string Name => this.m_name;

    public string SecurityToken
    {
      get
      {
        if (this.m_securityToken == null)
          this.m_securityToken = LinkingUtilities.DecodeUri(this.m_uri).ToolSpecificId;
        return this.m_securityToken;
      }
    }

    public bool MatchesScope(Guid scopeId) => scopeId == Guid.Empty || scopeId == this.Id;

    public override bool Equals(object obj) => obj is TeamProject && this.Equals(obj as TeamProject);

    public bool Equals(TeamProject project) => string.Equals(this.m_uri, project.Uri, StringComparison.OrdinalIgnoreCase);

    public override int GetHashCode() => this.m_uri.GetHashCode();
  }
}
