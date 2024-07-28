// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionRepository
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  public class ExternalConnectionRepository
  {
    public Guid ConnectionId { get; set; }

    public string ConnectionName { get; set; }

    public Guid RepositoryId { get; set; }

    public Guid ProviderId { get; set; }

    public string ProviderKey { get; set; }

    public string ExternalId { get; set; }

    public string RepositoryName { get; set; }

    public bool IsPrivate { get; set; }

    public string Url { get; set; }

    public string WebUrl { get; set; }

    public string AdditionalProperties { get; set; }

    public string Metadata { get; set; }

    public DateTime UpdatedDate { get; set; }
  }
}
