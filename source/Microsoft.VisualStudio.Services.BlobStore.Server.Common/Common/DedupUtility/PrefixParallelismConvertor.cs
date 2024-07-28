// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility.PrefixParallelismConvertor
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupUtility
{
  public static class PrefixParallelismConvertor
  {
    public static IEnumerable<string> GetPrefixesFromParallelism(int value)
    {
      if (value < 16)
        return (IEnumerable<string>) new string[1]
        {
          string.Empty
        };
      if (value < 256)
        return Enumerable.Range(0, 16).Select<int, string>((Func<int, string>) (i => string.Format("{0:X1}", (object) i)));
      return value < 4096 ? Enumerable.Range(0, 256).Select<int, string>((Func<int, string>) (i => string.Format("{0:X2}", (object) i))) : Enumerable.Range(0, 4096).Select<int, string>((Func<int, string>) (i => string.Format("{0:X3}", (object) i)));
    }
  }
}
