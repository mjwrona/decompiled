// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalGitRepoProvisionResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ExternalGitRepoProvisionResult
  {
    public ExternalGitRepoProvisionResult(
      ExternalGitRepo repo,
      bool isSuccess,
      bool isAdd,
      string message = null)
    {
      this.Repo = repo;
      this.IsSuccess = isSuccess;
      this.Message = message;
      this.IsAdd = isAdd;
    }

    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public ExternalGitRepo Repo { get; set; }

    public bool IsAdd { get; set; }
  }
}
