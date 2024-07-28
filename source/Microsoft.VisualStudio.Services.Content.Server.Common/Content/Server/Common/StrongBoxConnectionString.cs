// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.StrongBoxConnectionString
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Content.Server.Common
{
  public class StrongBoxConnectionString : IEquatable<StrongBoxConnectionString>
  {
    private const string EmulatorUri = "UseDevelopmentStorage=true";

    public StrongBoxConnectionString(string connectionString, string strongboxKey)
    {
      if (string.IsNullOrWhiteSpace(connectionString))
        throw new ArgumentNullException(nameof (connectionString));
      if (string.IsNullOrWhiteSpace(strongboxKey))
        throw new ArgumentNullException(nameof (strongboxKey));
      this.ConnectionString = connectionString;
      this.StrongBoxItemKey = strongboxKey;
    }

    public StrongBoxConnectionString(string connectionString)
      : this(connectionString, connectionString)
    {
    }

    public string ConnectionString { get; private set; }

    public string StrongBoxItemKey { get; private set; }

    public bool IsDevelopment => string.Equals(this.ConnectionString, "UseDevelopmentStorage=true", StringComparison.OrdinalIgnoreCase);

    public static IEnumerable<StrongBoxConnectionString> ConvertFromConnectionStringEnumerable(
      IEnumerable<string> connStrings)
    {
      return connStrings.Select<string, StrongBoxConnectionString>((Func<string, StrongBoxConnectionString>) (cs => new StrongBoxConnectionString(cs, cs)));
    }

    public bool Equals(StrongBoxConnectionString other)
    {
      if ((object) other == null)
        return false;
      if ((object) this == (object) other)
        return true;
      return this.ConnectionString == other.ConnectionString && this.StrongBoxItemKey == other.StrongBoxItemKey;
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if ((object) this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((StrongBoxConnectionString) obj);
    }

    public override int GetHashCode() => (this.ConnectionString != null ? this.ConnectionString.GetHashCode() : 0) * 397 ^ (this.StrongBoxItemKey != null ? this.StrongBoxItemKey.GetHashCode() : 0);

    public static bool operator ==(StrongBoxConnectionString left, StrongBoxConnectionString right) => object.Equals((object) left, (object) right);

    public static bool operator !=(StrongBoxConnectionString left, StrongBoxConnectionString right) => !object.Equals((object) left, (object) right);
  }
}
