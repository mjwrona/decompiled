// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Common.BackoffTimerHelper
// Assembly: Microsoft.VisualStudio.Services.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9C46641-2F1F-44C4-9C2E-4328CD5FFC63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Common.dll

using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public static class BackoffTimerHelper
  {
    private static readonly ThreadLocal<Random> s_rnd = new ThreadLocal<Random>((Func<Random>) (() => new Random()));

    public static TimeSpan GetRandomBackoff(
      TimeSpan minBackoff,
      TimeSpan maxBackoff,
      TimeSpan? previousBackoff = null)
    {
      return TimeSpan.FromMilliseconds((double) (!previousBackoff.HasValue ? new Random() : new Random((int) previousBackoff.Value.TotalMilliseconds)).Next((int) minBackoff.TotalMilliseconds, (int) maxBackoff.TotalMilliseconds));
    }

    public static TimeSpan GetExponentialBackoff(
      int attempt,
      TimeSpan minBackoff,
      TimeSpan maxBackoff,
      TimeSpan deltaBackoff,
      double radix)
    {
      ArgumentUtility.CheckForOutOfRange<double>(radix, nameof (radix), 1.0);
      double num1 = BackoffTimerHelper.s_rnd.Value.NextDouble();
      double num2 = deltaBackoff == TimeSpan.Zero ? 0.0 : deltaBackoff.TotalMilliseconds * (0.8 + num1 * 0.4);
      double num3 = attempt < 0 ? Math.Pow(radix, (double) attempt) * num2 : (Math.Pow(radix, (double) attempt) - 1.0) * num2;
      return TimeSpan.FromMilliseconds(Math.Min(minBackoff.TotalMilliseconds + num3, maxBackoff.TotalMilliseconds));
    }

    public static TimeSpan GetExponentialBackoff(
      int attempt,
      TimeSpan minBackoff,
      TimeSpan maxBackoff,
      TimeSpan deltaBackoff)
    {
      return BackoffTimerHelper.GetExponentialBackoff(attempt, minBackoff, maxBackoff, deltaBackoff, 2.0);
    }
  }
}
