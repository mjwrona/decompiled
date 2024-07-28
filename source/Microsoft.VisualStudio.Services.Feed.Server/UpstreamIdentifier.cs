// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.UpstreamIdentifier
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Feed.Server
{
  public class UpstreamIdentifier
  {
    private string Name { get; }

    private string Location { get; }

    public UpstreamIdentifier(string name, string location)
    {
      this.Name = name;
      this.Location = location;
    }

    public override string ToString() => this.Name + ";" + this.Location;

    public override int GetHashCode()
    {
      int hashCode1 = this.Name.GetHashCode();
      int hashCode2 = this.Location.GetHashCode();
      return (hashCode1 << 5) + hashCode1 ^ hashCode2;
    }

    public bool CompareName(UpstreamIdentifier obj) => string.Equals(this.Name, obj.Name, StringComparison.OrdinalIgnoreCase);

    public bool CompareLocation(UpstreamIdentifier obj) => string.Equals(this.Location, obj.Location, StringComparison.OrdinalIgnoreCase);

    public void CheckForMismatch(UpstreamIdentifier obj)
    {
      if (this.CompareName(obj) != this.CompareLocation(obj))
        throw new InvalidUpstreamSourceException(Resources.Error_UpstreamSourceNameLocationMismatch((object) this, (object) obj));
    }

    public int GetHashCode(UpstreamIdentifier obj) => throw new NotImplementedException();
  }
}
