// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.Server.NpmPackageName
// Assembly: Microsoft.VisualStudio.Services.Npm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2F4F0262-1C1B-42F0-BCA7-1385424A0D51
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Npm.WebApi.Exceptions;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Microsoft.VisualStudio.Services.Npm.Server
{
  public class NpmPackageName : IPackageName, IEquatable<NpmPackageName>
  {
    public static readonly Regex AllowedNamePattern = new Regex("^(?:@(?<packageScope>[^/@]+?)/)?(?<packageName>[^/@]+?)$");

    public NpmPackageName(string packageScope, string unscopedPackageName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(unscopedPackageName, nameof (unscopedPackageName));
      if (unscopedPackageName.Contains<char>('@') || unscopedPackageName.Contains<char>('/'))
        throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) this.FullName));
      this.Scope = packageScope == null || !packageScope.Contains<char>('@') && !packageScope.Contains<char>('/') ? packageScope : throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) this.FullName));
      this.UnscopedName = unscopedPackageName;
    }

    public NpmPackageName(string packageName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(packageName, nameof (packageName));
      switch (packageName.Count<char>((Func<char, bool>) (c => c == '/')))
      {
        case 0:
          this.UnscopedName = !packageName.Contains<char>('@') ? packageName : throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) packageName));
          break;
        case 1:
          string[] strArray = packageName.Split('/');
          string str = strArray[0];
          string source1 = strArray[1];
          string source2 = !source1.Contains<char>('@') && str.StartsWith("@") ? str.Substring(1) : throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) this.FullName));
          this.Scope = !source2.Contains<char>('@') ? source2 : throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) this.FullName));
          this.UnscopedName = source1;
          break;
        default:
          throw new InvalidPackageException(Resources.Error_InvalidPackageName((object) packageName));
      }
    }

    public string Scope { get; }

    public bool IsScoped => this.Scope != null;

    public string UnscopedName { get; }

    public string FullName => !this.IsScoped ? this.UnscopedName : "@" + this.Scope + "/" + this.UnscopedName;

    public string UriEscapedFullName => !this.IsScoped ? this.UnscopedName : "@" + this.Scope + "%2f" + this.UnscopedName;

    string IPackageName.DisplayName => this.FullName;

    string IPackageName.NormalizedName => this.FullName;

    public IProtocol Protocol => (IProtocol) Microsoft.VisualStudio.Services.Npm.Server.Protocol.npm;

    public override string ToString() => this.FullName;

    public override bool Equals(object obj) => this.Equals(obj as NpmPackageName);

    public bool Equals(NpmPackageName other) => other != null && other.FullName.Equals(this.FullName, StringComparison.Ordinal);

    public override int GetHashCode() => this.FullName.GetHashCode();
  }
}
