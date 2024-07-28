// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.ExternalUserDatasetComparer
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts.DataModels;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalArtifacts
{
  internal class ExternalUserDatasetComparer : IEqualityComparer<ExternalUserDataset>
  {
    public bool Equals(ExternalUserDataset a, ExternalUserDataset b) => TFStringComparer.ExternalProviderKey.Equals(a.ProviderKey, b.ProviderKey) && a.UserId == b.UserId;

    public int GetHashCode(ExternalUserDataset dataset) => dataset.ProviderKey.GetHashCode() ^ dataset.UserId.GetHashCode();

    public static ExternalUserDatasetComparer Instance { get; } = new ExternalUserDatasetComparer();
  }
}
