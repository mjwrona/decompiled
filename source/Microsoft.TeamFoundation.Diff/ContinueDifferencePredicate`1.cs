// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Diff.ContinueDifferencePredicate`1
// Assembly: Microsoft.TeamFoundation.Diff, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F647AACF-6EF1-4C0C-AB27-20317A054A39
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Diff.dll

using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Diff
{
  public delegate bool ContinueDifferencePredicate<T>(
    int originalIndex,
    IList<T> originalSequence,
    int longestMatchSoFar);
}
