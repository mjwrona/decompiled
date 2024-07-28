// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionDataset
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  internal class ExternalConnectionDataset
  {
    public int DataspaceId { get; set; }

    public Guid ConnectionId { get; set; }

    public Guid ProviderId { get; set; }

    public string ProviderKey { get; set; }

    public string ConnectionName { get; set; }

    public Guid ServiceEndpointId { get; set; }

    public bool IsValid { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime CreatedDate { get; set; }

    public Guid CreatedBy { get; set; }

    public DateTime UpdatedDate { get; set; }

    public IEnumerable<ExternalConnectionRepository> Repos { get; set; }

    public string ConnectionMetadata { get; set; }
  }
}
