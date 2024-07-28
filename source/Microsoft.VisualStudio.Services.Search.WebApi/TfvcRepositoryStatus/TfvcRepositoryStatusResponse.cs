// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.TfvcRepositoryStatus.TfvcRepositoryStatusResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.TfvcRepositoryStatus
{
  [DataContract]
  public class TfvcRepositoryStatusResponse : SearchSecuredV2Object
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "indexingInformation")]
    public IEnumerable<BranchInfo> IndexingInformation { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<BranchInfo> indexingInformation = this.IndexingInformation;
      this.IndexingInformation = indexingInformation != null ? (IEnumerable<BranchInfo>) indexingInformation.Select<BranchInfo, BranchInfo>((Func<BranchInfo, BranchInfo>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<BranchInfo>() : (IEnumerable<BranchInfo>) null;
    }
  }
}
