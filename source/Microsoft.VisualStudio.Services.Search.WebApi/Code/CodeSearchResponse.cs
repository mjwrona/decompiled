// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeSearchResponse
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code
{
  public class CodeSearchResponse : EntitySearchResponse
  {
    [DataMember(Name = "count")]
    public int Count { get; set; }

    [DataMember(Name = "results")]
    public IEnumerable<CodeResult> Results { get; set; }

    public override void SetSecuredObject(Guid namespaceId, int requiredPermissions, string token)
    {
      base.SetSecuredObject(namespaceId, requiredPermissions, token);
      IEnumerable<CodeResult> results = this.Results;
      this.Results = results != null ? (IEnumerable<CodeResult>) results.Select<CodeResult, CodeResult>((Func<CodeResult, CodeResult>) (i =>
      {
        i.SetSecuredObject(namespaceId, requiredPermissions, token);
        return i;
      })).ToList<CodeResult>() : (IEnumerable<CodeResult>) null;
    }
  }
}
