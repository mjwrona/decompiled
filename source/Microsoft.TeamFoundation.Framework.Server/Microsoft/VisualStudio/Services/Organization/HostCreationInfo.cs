// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Organization.HostCreationInfo
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.Organization
{
  public class HostCreationInfo
  {
    [JsonProperty]
    public Guid HostId { get; set; }

    [JsonProperty]
    public Guid PoolId { get; set; }

    [JsonProperty]
    public HostCreationType HostCreationType { get; set; }

    [JsonProperty]
    public AssignmentStatus AssignmentStatus { get; set; }

    [JsonProperty]
    public DateTime LastUpdated { get; set; }
  }
}
