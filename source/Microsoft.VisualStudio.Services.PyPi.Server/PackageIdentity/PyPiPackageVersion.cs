// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.PyPiPackageVersion
// Assembly: Microsoft.VisualStudio.Services.PyPi.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC58CC2C-9A83-4CAE-B2C4-C90763B36046
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity.VersionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Microsoft.VisualStudio.Services.PyPi.Server.PackageIdentity
{
  public class PyPiPackageVersion : 
    IPackageVersion,
    IComparable<PyPiPackageVersion>,
    IEquatable<PyPiPackageVersion>,
    IComparable
  {
    private string canonicalVersion;
    private string normalizedVersion;
    private string sortableVersion;

    public PyPiPackageVersion(
      string rawVersion,
      BigInteger epoch,
      BigInteger[] release,
      string preLabel,
      BigInteger? preNumber,
      string postLabel,
      BigInteger? postNumber,
      string devLabel,
      BigInteger? devNumber,
      PyPiLocalVersionLabel local)
    {
      string str1 = rawVersion;
      string str2 = str1 != null ? str1 : throw new ArgumentNullException(nameof (rawVersion));
      this.DisplayVersion = str1;
      this.DisplayVersion = str2;
      this.Epoch = epoch;
      this.Release = release;
      this.PreLabel = preLabel;
      this.PreNumber = preNumber;
      this.PostLabel = postLabel;
      this.PostNumber = postNumber;
      this.DevLabel = devLabel;
      this.DevNumber = devNumber;
      this.Local = local;
    }

    public int CompareTo(PyPiPackageVersion other)
    {
      ArgumentUtility.CheckForNull<PyPiPackageVersion>(other, nameof (other));
      return string.Compare(this.SortableVersion, other?.SortableVersion, StringComparison.Ordinal);
    }

    public bool Equals(PyPiPackageVersion other)
    {
      ArgumentUtility.CheckForNull<PyPiPackageVersion>(other, nameof (other));
      return this.NormalizedVersion.Equals(other?.NormalizedVersion, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => this.Equals(obj as PyPiPackageVersion);

    public override int GetHashCode() => this.NormalizedVersion.GetHashCode();

    public int CompareTo(object obj) => this.CompareTo(obj as PyPiPackageVersion);

    public string DisplayVersion { get; }

    public override string ToString() => this.DisplayVersion + " (" + this.NormalizedVersion + ")";

    public string CanonicalVersion => this.canonicalVersion ?? (this.canonicalVersion = this.BuildCanonicalVersion(false));

    public string NormalizedVersion => this.normalizedVersion ?? (this.normalizedVersion = this.BuildCanonicalVersion(true));

    public string SortableVersion
    {
      get
      {
        if (this.sortableVersion.IsNullOrEmpty<char>())
          this.sortableVersion = new PyPiSortableVersionConverter().Convert(this);
        return this.sortableVersion;
      }
    }

    public BigInteger Epoch { get; }

    public BigInteger[] Release { get; }

    public string PreLabel { get; }

    public BigInteger? PreNumber { get; }

    public string PostLabel { get; }

    public BigInteger? PostNumber { get; }

    public string DevLabel { get; }

    public BigInteger? DevNumber { get; }

    public PyPiLocalVersionLabel Local { get; }

    public bool IsRelease
    {
      get
      {
        if (this.PreLabel != null && this.PreNumber.HasValue)
          return false;
        return this.DevLabel == null || !this.DevNumber.HasValue;
      }
    }

    private string BuildCanonicalVersion(bool removeReleaseTrailingZeroes)
    {
      StringBuilder stringBuilder1 = new StringBuilder();
      if (this.Epoch != 0L)
        stringBuilder1.Append(string.Format("{0}!", (object) this.Epoch));
      if (this.Release == null)
        throw new InvalidOperationException("PyPI version does not contain a release segment");
      string str = removeReleaseTrailingZeroes ? string.Join<BigInteger>(".", PyPiPackageVersion.RemoveTrailingZeroes((IEnumerable<BigInteger>) this.Release)) : string.Join<BigInteger>(".", (IEnumerable<BigInteger>) this.Release);
      if (removeReleaseTrailingZeroes && str.Equals(string.Empty))
        stringBuilder1.Append("0");
      stringBuilder1.Append(str);
      BigInteger? nullable;
      if (this.PreLabel != null)
      {
        nullable = this.PreNumber;
        if (nullable.HasValue)
        {
          stringBuilder1.Append(this.PreLabel);
          StringBuilder stringBuilder2 = stringBuilder1;
          nullable = this.PreNumber;
          // ISSUE: variable of a boxed type
          __Boxed<BigInteger> local = (ValueType) nullable.Value;
          stringBuilder2.Append((object) local);
        }
      }
      if (this.PostLabel != null)
      {
        nullable = this.PostNumber;
        if (nullable.HasValue)
        {
          stringBuilder1.Append(".");
          stringBuilder1.Append(this.PostLabel);
          StringBuilder stringBuilder3 = stringBuilder1;
          nullable = this.PostNumber;
          // ISSUE: variable of a boxed type
          __Boxed<BigInteger> local = (ValueType) nullable.Value;
          stringBuilder3.Append((object) local);
        }
      }
      if (this.DevLabel != null)
      {
        nullable = this.DevNumber;
        if (nullable.HasValue)
        {
          stringBuilder1.Append(".");
          stringBuilder1.Append(this.DevLabel);
          StringBuilder stringBuilder4 = stringBuilder1;
          nullable = this.DevNumber;
          // ISSUE: variable of a boxed type
          __Boxed<BigInteger> local = (ValueType) nullable.Value;
          stringBuilder4.Append((object) local);
        }
      }
      if ((object) this.Local != null)
      {
        stringBuilder1.Append("+");
        stringBuilder1.Append(this.Local.ToString());
      }
      return stringBuilder1.ToString();
    }

    internal static IEnumerable<BigInteger> RemoveTrailingZeroes(IEnumerable<BigInteger> original) => original.Reverse<BigInteger>().SkipWhile<BigInteger>((Func<BigInteger, bool>) (x => x == 0L)).Reverse<BigInteger>();
  }
}
