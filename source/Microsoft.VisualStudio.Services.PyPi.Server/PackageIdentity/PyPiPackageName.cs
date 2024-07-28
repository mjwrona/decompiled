// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiPackageName
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Text.RegularExpressions;


#nullable enable
namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public sealed class PyPiPackageName : IPackageName, IEquatable<PyPiPackageName>
  {
    public static string MakeNameSafe(string inputName) => Regex.Replace(inputName.ToLowerInvariant(), "[-_.]+", "-");

    public PyPiPackageName(string name)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(name, nameof (name));
      this.DisplayName = name;
      this.NormalizedName = PyPiPackageName.MakeNameSafe(name);
    }

    public string DisplayName { get; }

    public string NormalizedName { get; }

    public IProtocol Protocol => (IProtocol) Microsoft.VisualStudio.Services.PyPi.Server.Protocol.PyPi;

    public override string ToString() => this.DisplayName;

    public bool Equals(PyPiPackageName? other)
    {
      if ((object) other == null)
        return false;
      return (object) this == (object) other || this.NormalizedName == other.NormalizedName;
    }

    public override bool Equals(object? obj)
    {
      if ((object) this == obj)
        return true;
      PyPiPackageName other = obj as PyPiPackageName;
      return (object) other != null && this.Equals(other);
    }

    public override int GetHashCode() => this.NormalizedName.GetHashCode();

    public static bool operator ==(PyPiPackageName left, PyPiPackageName right) => object.Equals((object) left, (object) right);

    public static bool operator !=(PyPiPackageName left, PyPiPackageName right) => !object.Equals((object) left, (object) right);
  }
}
