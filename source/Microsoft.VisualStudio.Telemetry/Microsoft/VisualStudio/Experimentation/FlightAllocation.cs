// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.FlightAllocation
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using System;

namespace Microsoft.VisualStudio.Experimentation
{
  internal class FlightAllocation : IEquatable<FlightAllocation>
  {
    private static readonly char RegistrySplitCharacter = '~';
    private static readonly char NetworkSplitCharacter = ':';
    private readonly string registryFormat;

    public string FlightName { get; private set; }

    public string AllocationId { get; private set; }

    public static FlightAllocation CreateFromNetworkString(string rawNetworkFlight) => FlightAllocation.CreationHelper(rawNetworkFlight, FlightAllocation.NetworkSplitCharacter);

    public static FlightAllocation CreateFromRegistryString(string rawRegistryFlight) => FlightAllocation.CreationHelper(rawRegistryFlight, FlightAllocation.RegistrySplitCharacter);

    public FlightAllocation(string flight)
      : this(flight, (string) null)
    {
    }

    public FlightAllocation(string flight, string allocation)
    {
      this.FlightName = !string.IsNullOrWhiteSpace(flight) ? flight.Trim() : throw new ArgumentException("Invalid flight name", nameof (flight));
      if (!string.IsNullOrWhiteSpace(allocation))
      {
        this.AllocationId = allocation.Trim();
        this.registryFormat = this.FlightName + (object) FlightAllocation.RegistrySplitCharacter + this.AllocationId;
      }
      else
      {
        this.AllocationId = (string) null;
        this.registryFormat = this.FlightName;
      }
    }

    public FlightAllocation ToLowerInvariant() => new FlightAllocation(this.FlightName.ToLowerInvariant(), this.AllocationId?.ToLowerInvariant());

    public string ToRegistryString() => this.registryFormat;

    public override string ToString() => string.IsNullOrWhiteSpace(this.AllocationId) ? this.FlightName : this.FlightName + (object) FlightAllocation.NetworkSplitCharacter + this.AllocationId;

    public bool Equals(FlightAllocation other) => string.Equals(this.registryFormat, other.registryFormat, StringComparison.Ordinal);

    public override int GetHashCode() => this.registryFormat.GetHashCode();

    private static FlightAllocation CreationHelper(string flightString, char separator)
    {
      string[] strArray = !string.IsNullOrWhiteSpace(flightString) ? flightString.Split(separator) : throw new ArgumentException("Invalid flight name", nameof (flightString));
      return new FlightAllocation(strArray[0], strArray.Length == 2 ? strArray[1] : (string) null);
    }
  }
}
