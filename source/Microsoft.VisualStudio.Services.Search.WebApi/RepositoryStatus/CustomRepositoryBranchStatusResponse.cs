// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.CustomRepositoryBranchStatusResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus
{
  [DataContract]
  public class CustomRepositoryBranchStatusResponse : SearchSecuredV2Object
  {
    public CustomRepositoryBranchStatusResponse()
    {
      this.LatestChangeId = -1L;
      this.LastIndexedChangeId = -1L;
      this.LatestChangeIdChangeTime = DateTime.MinValue;
      this.LastIndexedChangeIdChangeTime = DateTime.MinValue;
    }

    [DataMember(Name = "latestChangeId")]
    public long LatestChangeId { get; set; }

    [DataMember(Name = "lastIndexedChangeId")]
    public long LastIndexedChangeId { get; set; }

    [DataMember(Name = "latestChangeIdChangeTime")]
    public DateTime LatestChangeIdChangeTime { get; set; }

    [DataMember(Name = "lastIndexedChangeIdChangeTime")]
    public DateTime LastIndexedChangeIdChangeTime { get; set; }
  }
}
