// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.VssLazy`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class VssLazy<T> : Lazy<T>
  {
    public VssLazy() => throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());

    public VssLazy(bool isThreadSafe)
      : base(isThreadSafe)
    {
      if (isThreadSafe)
        throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());
    }

    public VssLazy(Func<T> valueFactory)
      : base(valueFactory)
    {
      throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());
    }

    public VssLazy(LazyThreadSafetyMode mode)
      : base(mode)
    {
      if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
        throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());
    }

    public VssLazy(Func<T> valueFactory, bool isThreadSafe)
      : base(valueFactory, isThreadSafe)
    {
      if (isThreadSafe)
        throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());
    }

    public VssLazy(Func<T> valueFactory, LazyThreadSafetyMode mode)
      : base(valueFactory, mode)
    {
      if (mode == LazyThreadSafetyMode.ExecutionAndPublication)
        throw new ArgumentException(FrameworkResources.UnsupportedLazyThreadSafetyMode());
    }
  }
}
