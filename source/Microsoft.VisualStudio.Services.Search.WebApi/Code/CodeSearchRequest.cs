// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code.CodeSearchRequest
// Assembly: Microsoft.VisualStudio.Services.Search.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9E496DAE-109A-4A16-A97B-F7DEDEC6CB20
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.WebApi.dll

using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Contracts;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.WebApi.Contracts.Code
{
  [DataContract]
  public class CodeSearchRequest : EntitySearchRequest
  {
    private static HashSet<string> s_sortSupportedFields = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "path",
      "fileName"
    };
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

    [DataMember(Name = "includeSnippet", IsRequired = false)]
    public bool IncludeSnippet { get; set; }

    public override void ValidateQuery()
    {
      this.ValidateQuery(CodeSearchRequest.ParentOf);
      if (this.OrderBy == null)
        return;
      foreach (SortOption sortOption in this.OrderBy)
      {
        if (!CodeSearchRequest.s_sortSupportedFields.Contains(sortOption.Field))
          throw new InvalidQueryException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, SearchWebApiResources.SortingOnFieldNotSupportedMessageFormat, (object) sortOption.Field));
      }
    }

    public override bool IsSupportedFilter(string filterName) => CodeSearchRequest.s_validFilters.Contains(filterName);
  }
}
