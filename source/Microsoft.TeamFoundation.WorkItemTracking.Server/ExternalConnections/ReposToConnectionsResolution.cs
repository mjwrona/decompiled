// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ReposToConnectionsResolution
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ReposToConnectionsResolution
  {
    public IDictionary<Guid, ExternalConnection> RepoInternalIdToConnectionLookup;
    public IDictionary<Guid, ExternalGitRepo> RepoInternalIdToRepoLookup;
    public IDictionary<Guid, string> RepoInternalIdToExternalIdLookup;
    public IDictionary<Tuple<string, string>, Guid> RepoNameToInternalIdLookup;

    public ReposToConnectionsResolution()
    {
      this.RepoInternalIdToConnectionLookup = (IDictionary<Guid, ExternalConnection>) new Dictionary<Guid, ExternalConnection>();
      this.RepoInternalIdToRepoLookup = (IDictionary<Guid, ExternalGitRepo>) new Dictionary<Guid, ExternalGitRepo>();
      this.RepoInternalIdToExternalIdLookup = (IDictionary<Guid, string>) new Dictionary<Guid, string>();
      this.RepoNameToInternalIdLookup = (IDictionary<Tuple<string, string>, Guid>) new Dictionary<Tuple<string, string>, Guid>((IEqualityComparer<Tuple<string, string>>) CaseInsensitiveTupleComparer.Instance);
    }
  }
}
