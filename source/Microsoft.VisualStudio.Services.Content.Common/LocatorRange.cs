// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.LocatorRange
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  public class LocatorRange
  {
    public LocatorRangeBoundary LowerBound { get; }

    public LocatorRangeBoundary UpperBound { get; }

    public LocatorRange(LocatorRangeBoundary lowerBound, LocatorRangeBoundary upperBound)
    {
      ArgumentUtility.CheckForNull<LocatorRangeBoundary>(lowerBound, nameof (lowerBound));
      ArgumentUtility.CheckForNull<LocatorRangeBoundary>(upperBound, nameof (upperBound));
      this.LowerBound = lowerBound;
      this.UpperBound = upperBound;
    }

    public ShardableLocatorRange ToShardable(Func<Locator, ShardableLocator> conversion)
    {
      ArgumentUtility.CheckForNull<Func<Locator, ShardableLocator>>(conversion, nameof (conversion));
      return new ShardableLocatorRange(this.LowerBound.ToShardable(conversion), this.UpperBound.ToShardable(conversion));
    }
  }
}
