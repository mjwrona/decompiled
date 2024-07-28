// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Maven.Server.Models.MavenPackageVersion
// Assembly: Microsoft.VisualStudio.Services.Maven.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AEBE02E-FDD2-41D8-89F7-5C54445DBFA7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Maven.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Maven.Server.Version;
using Microsoft.VisualStudio.Services.Maven.Server.Version.Helpers;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Versioning;
using System;
using System.Numerics;

namespace Microsoft.VisualStudio.Services.Maven.Server.Models
{
  public class MavenPackageVersion : 
    IPackageVersion,
    IEquatable<MavenPackageVersion>,
    IComparable<MavenPackageVersion>,
    IComparable
  {
    public MavenPackageVersion(string version)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(version, nameof (version));
      this.Parser = !version.Equals("versionlist", StringComparison.OrdinalIgnoreCase) ? new MavenVersionParser(version) : throw new ArgumentException(Resources.Error_VersionListIsReservedName(), nameof (version));
      try
      {
        LexicalBase64Converter lexicalBase64Converter = new LexicalBase64Converter();
        this.SortableVersion = new MavenSortableVersionConverter((IConverter<BigInteger, string>) new VersionBigIntegerToPaddedStringConverter((IConverter<ArraySegment<byte>, string>) lexicalBase64Converter), (IConverter<ArraySegment<byte>, string>) lexicalBase64Converter).Convert(this.Parser);
      }
      catch (InvalidVersionException ex)
      {
        this.SortableVersion = this.NormalizedVersion;
        this.ExceptionComputingSortableVersion = (Exception) ex;
      }
    }

    public MavenVersionParser Parser { get; }

    public string DisplayVersion => this.Parser.RawVersion;

    public string NormalizedVersion => this.DisplayVersion.ToLowerInvariant();

    public string CanonicalizedVersion => this.Parser.CanonicalVersion;

    public string SortableVersion { get; }

    public Exception ExceptionComputingSortableVersion { get; }

    public bool Equals(MavenPackageVersion other)
    {
      ArgumentUtility.CheckForNull<MavenPackageVersion>(other, nameof (other));
      return this.NormalizedVersion.Equals(other.NormalizedVersion, StringComparison.Ordinal);
    }

    public override bool Equals(object other)
    {
      ArgumentUtility.CheckForNull<object>(other, nameof (other));
      return other is MavenPackageVersion other1 && this.Equals(other1);
    }

    public int CompareTo(MavenPackageVersion other)
    {
      ArgumentUtility.CheckForNull<MavenPackageVersion>(other, nameof (other));
      return string.Compare(this.SortableVersion, other.SortableVersion, StringComparison.Ordinal);
    }

    public int CompareTo(object obj) => this.CompareTo(obj as MavenPackageVersion);

    public override int GetHashCode() => this.NormalizedVersion.GetHashCode();

    public override string ToString() => this.DisplayVersion;
  }
}
