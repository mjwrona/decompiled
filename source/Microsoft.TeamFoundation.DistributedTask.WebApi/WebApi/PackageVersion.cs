// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.PackageVersion
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class PackageVersion : IComparable<PackageVersion>, IEquatable<PackageVersion>
  {
    public PackageVersion()
    {
    }

    public PackageVersion(string version)
    {
      int major;
      int minor;
      int patch;
      VersionParser.ParseVersion(version, out major, out minor, out patch, out string _);
      this.Major = major;
      this.Minor = minor;
      this.Patch = patch;
    }

    public static bool TryParse(string versionStr, out PackageVersion version)
    {
      version = (PackageVersion) null;
      try
      {
        version = new PackageVersion(versionStr);
        return true;
      }
      catch
      {
        return false;
      }
    }

    private PackageVersion(PackageVersion versionToClone)
    {
      this.Major = versionToClone.Major;
      this.Minor = versionToClone.Minor;
      this.Patch = versionToClone.Patch;
    }

    [DataMember]
    public int Major { get; set; }

    [DataMember]
    public int Minor { get; set; }

    [DataMember]
    public int Patch { get; set; }

    public PackageVersion Clone() => new PackageVersion(this);

    public static implicit operator string(PackageVersion version) => version.ToString();

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}.{2}", (object) this.Major, (object) this.Minor, (object) this.Patch);

    public override int GetHashCode() => this.ToString().GetHashCode();

    public int CompareTo(PackageVersion other)
    {
      int num = this.Major.CompareTo(other.Major);
      if (num == 0)
      {
        num = this.Minor.CompareTo(other.Minor);
        if (num == 0)
          num = this.Patch.CompareTo(other.Patch);
      }
      return num;
    }

    public bool Equals(PackageVersion other) => this.CompareTo(other) == 0;
  }
}
