// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.BranchInfo
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus
{
  [DataContract]
  public class BranchInfo : SearchSecuredV2Object
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [ClientInternalUseOnly(true)]
    [DataMember(Name = "lastIndexedChangeId")]
    public string LastIndexedChangeId { get; set; }

    [ClientInternalUseOnly(true)]
    [DataMember(Name = "lastProcessedTime")]
    public DateTime LastProcessedTime { get; set; }

    public BranchInfo(string name, string lastIndexedChangeId, DateTime lastProcessedTime)
    {
      this.Name = name;
      this.LastIndexedChangeId = lastIndexedChangeId;
      this.LastProcessedTime = lastProcessedTime;
    }

    public BranchInfo(string name)
    {
      this.Name = name;
      this.LastIndexedChangeId = "";
      this.LastProcessedTime = ContractConstants.DefaultLastProcessedTime;
    }
  }
}
