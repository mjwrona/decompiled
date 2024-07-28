// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Models.ProjectMembershipModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Models
{
  public class ProjectMembershipModel
  {
    private List<ProjectMember> m_ProjectUserAdditions;
    private List<ProjectMember> m_ProjectUserDeletions;

    public ProjectMembershipModel(
      List<ProjectMember> projectUserAdditions,
      List<ProjectMember> projectUserDeletions)
    {
      this.m_ProjectUserAdditions = projectUserAdditions;
      this.m_ProjectUserDeletions = projectUserDeletions;
    }

    public List<ProjectMember> ProjectUserAdditions => this.m_ProjectUserAdditions;

    public List<ProjectMember> ProjectUserDeletions => this.m_ProjectUserDeletions;
  }
}
