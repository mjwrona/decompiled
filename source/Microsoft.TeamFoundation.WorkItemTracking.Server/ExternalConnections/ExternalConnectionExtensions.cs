// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public static class ExternalConnectionExtensions
  {
    public static Guid GetRepoInternalId(
      this ExternalConnection externalConnection,
      string repoNodeId)
    {
      IEnumerable<ExternalGitRepo> externalGitRepos = externalConnection.ExternalGitRepos;
      Guid? nullable;
      if (externalGitRepos == null)
      {
        nullable = new Guid?();
      }
      else
      {
        ExternalGitRepo artifact = externalGitRepos.Where<ExternalGitRepo>((Func<ExternalGitRepo, bool>) (r => r.NodeId() == repoNodeId)).FirstOrDefault<ExternalGitRepo>();
        nullable = artifact != null ? new Guid?(artifact.GetRepoInternalId()) : new Guid?();
      }
      return nullable ?? Guid.Empty;
    }
  }
}
