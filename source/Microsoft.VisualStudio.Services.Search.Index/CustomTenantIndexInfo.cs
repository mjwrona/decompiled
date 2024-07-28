// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.CustomTenantIndexInfo
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public class CustomTenantIndexInfo
  {
    public CustomTenantIndexInfo()
    {
      this.CollectionIds = (IList<string>) new List<string>();
      this.SearchRoutings = (IEnumerable<string>) new List<string>();
    }

    public IList<string> CollectionIds { get; set; }

    public IEnumerable<string> SearchRoutings { get; set; }

    public IDictionary<string, List<string>> Filters => (IDictionary<string, List<string>>) new Dictionary<string, List<string>>()
    {
      {
        "collectionId",
        this.CollectionIds.Select<string, string>((Func<string, string>) (a => a.ToLower())).ToList<string>()
      }
    };
  }
}
