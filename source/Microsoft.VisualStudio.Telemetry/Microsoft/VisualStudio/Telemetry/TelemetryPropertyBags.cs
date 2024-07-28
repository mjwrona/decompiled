// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetryPropertyBags
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Telemetry
{
  public static class TelemetryPropertyBags
  {
    internal static readonly StringComparer KeyComparer = StringComparer.OrdinalIgnoreCase;

    public static bool HasProperties<TValue>(this ITelemetryPropertyBag<TValue> bag) => bag.Count > 0;

    public class Concurrent<TValue> : 
      ConcurrentDictionary<string, TValue>,
      ITelemetryPropertyBag<TValue>,
      IDictionary<string, TValue>,
      ICollection<KeyValuePair<string, TValue>>,
      IEnumerable<KeyValuePair<string, TValue>>,
      IEnumerable
    {
      public Concurrent()
        : base((IEqualityComparer<string>) TelemetryPropertyBags.KeyComparer)
      {
      }
    }

    internal sealed class NotConcurrent<TValue> : 
      Dictionary<string, TValue>,
      ITelemetryPropertyBag<TValue>,
      IDictionary<string, TValue>,
      ICollection<KeyValuePair<string, TValue>>,
      IEnumerable<KeyValuePair<string, TValue>>,
      IEnumerable
    {
      public NotConcurrent()
        : base((IEqualityComparer<string>) TelemetryPropertyBags.KeyComparer)
      {
      }
    }

    internal sealed class PrefixedConcurrent<TValue> : 
      PrefixedPropertyBag<TValue>,
      ITelemetryPropertyBag<TValue>,
      IDictionary<string, TValue>,
      ICollection<KeyValuePair<string, TValue>>,
      IEnumerable<KeyValuePair<string, TValue>>,
      IEnumerable
    {
      public PrefixedConcurrent(string prefix)
        : base((IDictionary<string, TValue>) new ConcurrentDictionary<string, TValue>((IEqualityComparer<string>) TelemetryPropertyBags.KeyComparer), prefix)
      {
      }
    }

    internal sealed class PrefixedNotConcurrent<TValue> : 
      PrefixedPropertyBag<TValue>,
      ITelemetryPropertyBag<TValue>,
      IDictionary<string, TValue>,
      ICollection<KeyValuePair<string, TValue>>,
      IEnumerable<KeyValuePair<string, TValue>>,
      IEnumerable
    {
      public PrefixedNotConcurrent(string prefix)
        : base((IDictionary<string, TValue>) new Dictionary<string, TValue>((IEqualityComparer<string>) TelemetryPropertyBags.KeyComparer), prefix)
      {
      }
    }
  }
}
