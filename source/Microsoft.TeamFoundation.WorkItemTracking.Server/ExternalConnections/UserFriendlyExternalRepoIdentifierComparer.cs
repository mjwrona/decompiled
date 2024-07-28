// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.UserFriendlyExternalRepoIdentifierComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class UserFriendlyExternalRepoIdentifierComparer : 
    IEqualityComparer<(string providerKey, string fullRepoName)>
  {
    public static UserFriendlyExternalRepoIdentifierComparer Instance = new UserFriendlyExternalRepoIdentifierComparer();

    protected UserFriendlyExternalRepoIdentifierComparer()
    {
    }

    public bool Equals(
      (string providerKey, string fullRepoName) x,
      (string providerKey, string fullRepoName) y)
    {
      return VssStringComparer.Hostname.Equals(x.providerKey, y.providerKey) && TFStringComparer.ExternalRepoName.Equals(x.fullRepoName, y.fullRepoName);
    }

    public int GetHashCode((string providerKey, string fullRepoName) obj) => (((obj.providerKey != null ? (long) obj.providerKey.GetHashCode() : 0L) << 32) + (obj.fullRepoName != null ? (long) obj.fullRepoName.GetHashCode() : 0L)).GetHashCode();
  }
}
