// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board.BoardSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Board
{
  [DataContract]
  public class BoardSearchRequest : EntitySearchRequest
  {
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Collection",
      "Project",
      "boardType"
    };
    public static readonly IReadOnlyDictionary<string, string> ParentOf = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["Project"] = "Collection"
    };
    public const string BoardType = "boardType";

    public override bool IsSupportedFilter(string filterName) => BoardSearchRequest.s_validFilters.Contains(filterName);

    public void ValidateFilterHierarchy() => this.ValidateQuery(BoardSearchRequest.ParentOf);

    public override void ValidateQuery() => base.ValidateQuery();
  }
}
