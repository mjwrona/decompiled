// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ThreadLocalRandom
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public static class ThreadLocalRandom
  {
    private static int seed;
    private static readonly ThreadLocal<Random> threadLocal = new ThreadLocal<Random>((Func<Random>) (() => new Random(Interlocked.Increment(ref ThreadLocalRandom.seed))));

    static ThreadLocalRandom() => ThreadLocalRandom.seed = new Random().Next();

    public static Random Generator => ThreadLocalRandom.threadLocal.Value;
  }
}
