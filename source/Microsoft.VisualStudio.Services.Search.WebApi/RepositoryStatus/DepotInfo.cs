// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus.DepotInfo
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.RepositoryStatus
{
  [DataContract]
  public class DepotInfo : SearchSecuredV2Object
  {
    [DataMember(Name = "name")]
    public string Name { get; set; }

    [ClientInternalUseOnly(true)]
    [DataMember(Name = "indexedBranches")]
    public IEnumerable<BranchInfo> IndexedBranches { get; set; }

    public DepotInfo(string name, IEnumerable<BranchInfo> indexedBranches)
    {
      this.Name = name;
      this.IndexedBranches = indexedBranches;
    }

    public DepotInfo(string name)
    {
      this.Name = name;
      this.IndexedBranches = (IEnumerable<BranchInfo>) null;
    }

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
