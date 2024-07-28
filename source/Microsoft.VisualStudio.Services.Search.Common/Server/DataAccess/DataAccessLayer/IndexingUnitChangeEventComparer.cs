// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer.IndexingUnitChangeEventComparer
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer
{
  public class IndexingUnitChangeEventComparer : IEqualityComparer<IndexingUnitChangeEvent>
  {
    public bool Equals(IndexingUnitChangeEvent x, IndexingUnitChangeEvent y)
    {
      if (x == null && y == null)
        return true;
      return x != null && y != null && !(x.GetType() != y.GetType()) && x.Id == y.Id;
    }

    public int GetHashCode(IndexingUnitChangeEvent obj) => obj != null ? (int) obj.Id : throw new ArgumentNullException(nameof (obj));
  }
}
