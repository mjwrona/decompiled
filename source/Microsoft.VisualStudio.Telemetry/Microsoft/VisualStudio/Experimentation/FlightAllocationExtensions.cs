// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.FlightAllocationExtensions
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Experimentation
{
  internal static class FlightAllocationExtensions
  {
    internal static HashSet<FlightAllocation> UnionWithFlights(
      this IEnumerable<FlightAllocation> flights,
      IEnumerable<FlightAllocation> otherFlights,
      StringComparer comparer)
    {
      Dictionary<string, string> dictionary = flights.ToDictionary<FlightAllocation, string, string>((Func<FlightAllocation, string>) (f => f.FlightName), (Func<FlightAllocation, string>) (f => f.AllocationId), (IEqualityComparer<string>) comparer);
      foreach (FlightAllocation otherFlight in otherFlights)
      {
        if (!dictionary.ContainsKey(otherFlight.FlightName) || otherFlight.AllocationId != null)
          dictionary[otherFlight.FlightName] = otherFlight.AllocationId;
      }
      return new HashSet<FlightAllocation>(dictionary.Select<KeyValuePair<string, string>, FlightAllocation>((Func<KeyValuePair<string, string>, FlightAllocation>) (kv => new FlightAllocation(kv.Key, kv.Value))));
    }

    internal static void ExceptWithFlights(
      this HashSet<FlightAllocation> flights,
      IEnumerable<FlightAllocation> excludedFlights,
      StringComparer comparer)
    {
      HashSet<string> excludedFlightNames = new HashSet<string>(excludedFlights.Select<FlightAllocation, string>((Func<FlightAllocation, string>) (f => f.FlightName)), (IEqualityComparer<string>) comparer);
      flights.RemoveWhere((Predicate<FlightAllocation>) (f => excludedFlightNames.Contains(f.FlightName)));
    }
  }
}
