// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.MemoryCacheMultiSubscriber
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class MemoryCacheMultiSubscriber
  {
    public static IMemoryCacheSubscriber<TKey, TValue> Create<TKey, TValue>(
      IMemoryCacheSubscriber<TKey, TValue> inst1,
      IMemoryCacheSubscriber<TKey, TValue> inst2)
    {
      ArgumentUtility.CheckForNull<IMemoryCacheSubscriber<TKey, TValue>>(inst1, nameof (inst1));
      ArgumentUtility.CheckForNull<IMemoryCacheSubscriber<TKey, TValue>>(inst2, nameof (inst2));
      return (IMemoryCacheSubscriber<TKey, TValue>) new MemoryCacheMultiSubscriber<TKey, TValue>((IEnumerable<IMemoryCacheSubscriber<TKey, TValue>>) new IMemoryCacheSubscriber<TKey, TValue>[2]
      {
        inst1,
        inst2
      });
    }
  }
}
