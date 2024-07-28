// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Models.ProjectMember
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Server.Core;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Models
{
  public class ProjectMember
  {
    private TeamFoundationIdentity m_ProjectGroup;
    private TeamFoundationIdentity m_UserId;

    public ProjectMember(
      TeamFoundationIdentity userId,
      TeamFoundationIdentity projectGroup,
      bool isError = false,
      string errorMessage = "")
    {
      this.m_UserId = userId;
      this.m_ProjectGroup = projectGroup;
      this.IsError = isError;
      if (!this.IsError)
        return;
      this.ErrorMessage = errorMessage;
    }

    public TeamFoundationIdentity ProjectGroup => this.m_ProjectGroup;

    public TeamFoundationIdentity UserId => this.m_UserId;

    public bool IsError { get; set; }

    public string ErrorMessage { get; set; }
  }
}
