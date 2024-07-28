// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ExternalConnectionMetadata
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [DataContract]
  public class ExternalConnectionMetadata
  {
    public ExternalConnectionMetadata() => this.RepositoriesWithMappingToDifferentOrganization = (ISet<string>) new HashSet<string>();

    [DataMember]
    public Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.ConnectionErrorType? ConnectionErrorType { get; set; }

    public IEnumerable<string> UnreachableRepositories { get; set; }

    [DataMember]
    public ISet<string> RepositoriesWithMappingToDifferentOrganization { get; set; }
  }
}
