// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.RepositoryStatusResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus
{
  [DataContract]
  public class RepositoryStatusResponse : SearchSecuredV2Object
  {
    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "indexedBranches")]
    public IEnumerable<BranchInfo> IndexedBranches { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<BranchInfo> indexedBranches = this.IndexedBranches;
      this.IndexedBranches = indexedBranches != null ? (IEnumerable<BranchInfo>) indexedBranches.Select<BranchInfo, BranchInfo>((Func<BranchInfo, BranchInfo>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<BranchInfo>() : (IEnumerable<BranchInfo>) null;
    }
  }
}
