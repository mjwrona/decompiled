// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.ClassificationNodeIdComparator
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public class ClassificationNodeIdComparator : IEqualityComparer<ClassificationNode>
  {
    public bool Equals(ClassificationNode x, ClassificationNode y)
    {
      if (x == null && y == null)
        return true;
      if (x == null || y == null)
        return false;
      return x == y || x.Id == y.Id;
    }

    public int GetHashCode(ClassificationNode obj) => obj != null ? obj.GetHashCode() : throw new ArgumentNullException(nameof (obj));
  }
}
