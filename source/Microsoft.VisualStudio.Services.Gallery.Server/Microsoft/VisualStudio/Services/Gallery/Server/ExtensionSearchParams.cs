// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.ExtensionSearchParams
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server
{
  internal class ExtensionSearchParams
  {
    public IList<SearchCriteria> CriteriaList { get; set; }

    public int PageSize { get; set; }

    public int PageNumber { get; set; }

    public int SortBy { get; set; }

    public int SortOrder { get; set; }

    internal string RawQuery { get; set; }

    internal ExtensionQueryResultMetadataFlags MetadataFlags { get; set; }

    internal string Product { get; set; }

    internal SearchFeatureFlags FeatureFlags { get; set; }
  }
}
