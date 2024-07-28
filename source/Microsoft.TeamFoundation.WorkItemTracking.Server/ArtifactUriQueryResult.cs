// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ArtifactUriQueryResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class ArtifactUriQueryResult
  {
    private IDictionary<int, string> m_workitemToTokenLookup;

    internal ArtifactUriQueryResult()
    {
    }

    public ArtifactUriQueryResult(
      string artifactUri,
      IEnumerable<int> workItemIds,
      IDictionary<int, string> workItemToTokenLookup)
    {
      this.ArtifactUri = artifactUri;
      this.WorkItemIds = workItemIds;
      this.m_workitemToTokenLookup = workItemToTokenLookup;
    }

    public string ArtifactUri { get; internal set; }

    public IEnumerable<int> WorkItemIds { get; internal set; }

    public string GetTokenWorkItem(int workItemId) => this.m_workitemToTokenLookup[workItemId];

    public Guid GetNamespaceIdWorkItem() => AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid;

    public int GetRequiredPermissionsWorkItem() => 16;
  }
}
