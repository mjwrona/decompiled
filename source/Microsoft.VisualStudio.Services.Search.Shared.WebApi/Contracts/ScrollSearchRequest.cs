// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts.ScrollSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 504F400B-CBC4-4007-9816-31A8DED1C3FC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts
{
  [DataContract]
  public class ScrollSearchRequest : EntitySearchRequestBase
  {
    private static readonly HashSet<string> s_validFilters = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "Project",
      "Repository",
      "Path",
      "Branch",
      "CodeElement"
    };
    public static readonly IReadOnlyDictionary<string, string> ParentOf = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["Path"] = "Repository",
      ["Repository"] = "Project"
    };

    [DataMember(Name = "$scrollId")]
    public string ScrollId { get; set; }

    [DataMember(Name = "$scrollSize")]
    public int ScrollSize { get; set; }

    public override void ValidateQuery()
    {
      if (!string.IsNullOrWhiteSpace(this.ScrollId))
        return;
      if (this.ScrollSize <= 0)
        throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchSharedWebApiResources.InvalidOrNotSetScrollSize));
      this.ValidateQuery(ScrollSearchRequest.ParentOf);
    }

    public override bool IsSupportedFilter(string filterName) => ScrollSearchRequest.s_validFilters.Contains(filterName);
  }
}
