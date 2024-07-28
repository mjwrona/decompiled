// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity.CargoPackageName
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity
{
  public sealed class CargoPackageName : IPackageName, IEquatable<CargoPackageName>
  {
    internal CargoPackageName(string displayName, string normalizedName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(displayName, nameof (displayName));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(normalizedName, nameof (normalizedName));
      this.DisplayName = displayName;
      this.NormalizedName = normalizedName;
    }

    public string DisplayName { get; }

    public string NormalizedName { get; }

    IProtocol IPackageName.Protocol => (IProtocol) Protocol.Cargo;

    public override string ToString() => this.DisplayName;

    public bool Equals(CargoPackageName? other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || this.NormalizedName == other.NormalizedName;
    }

    public override bool Equals(object? obj)
    {
      if ((object) this == obj)
        return true;
      CargoPackageName other = obj as CargoPackageName;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => this.NormalizedName.GetHashCode();

    public static bool operator ==(CargoPackageName? left, CargoPackageName? right) => object.Equals((object) left, (object) right);

    public static bool operator !=(CargoPackageName? left, CargoPackageName? right) => !object.Equals((object) left, (object) right);
  }
}
