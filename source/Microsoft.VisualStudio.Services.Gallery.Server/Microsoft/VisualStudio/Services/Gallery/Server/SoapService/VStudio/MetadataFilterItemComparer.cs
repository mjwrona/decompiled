// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio.MetadataFilterItemComparer
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Microsoft.VisualStudio.Services.Gallery.Server.SoapService.Models;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Server.SoapService.VStudio
{
  internal class MetadataFilterItemComparer : IComparer<MetadataFilterItem>
  {
    public int Compare(MetadataFilterItem x, MetadataFilterItem y)
    {
      if (x == y)
        return 0;
      if (x == null)
        return -1;
      if (y == null)
        return 1;
      int num = string.Compare(x.Key, y.Key, StringComparison.OrdinalIgnoreCase);
      if (num != 0)
        return num;
      return x.Operator == y.Operator ? string.Compare(x.Value, y.Value, StringComparison.OrdinalIgnoreCase) : x.Operator.CompareTo((object) y.Operator);
    }
  }
}
