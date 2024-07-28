// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio.FilterCriteriaComparer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio
{
  internal class FilterCriteriaComparer : IComparer<FilterCriteria>
  {
    public int Compare(FilterCriteria x, FilterCriteria y)
    {
      if (x == y)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      if (x.FilterType < y.FilterType)
        return -1;
      return x.FilterType > y.FilterType ? 1 : string.Compare(x.Value, y.Value, StringComparison.OrdinalIgnoreCase);
    }
  }
}
