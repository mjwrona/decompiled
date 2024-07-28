// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata.PackageIdentityBase`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Common;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata
{
  public abstract class PackageIdentityBase<TName, TVersion> : 
    IPackageIdentity<TName, TVersion>,
    IPackageIdentity
    where TName : IPackageName
    where TVersion : IPackageVersion
  {
    protected PackageIdentityBase(TName name, TVersion version)
    {
      ArgumentUtility.CheckGenericForNull((object) name, nameof (name));
      ArgumentUtility.CheckGenericForNull((object) version, nameof (version));
      this.Name = name;
      this.Version = version;
    }

    public TVersion Version { get; }

    public TName Name { get; }

    IPackageName IPackageIdentity.Name => (IPackageName) this.Name;

    IPackageVersion IPackageIdentity.Version => (IPackageVersion) this.Version;

    public virtual string DisplayStringForMessages => this.Name.DisplayName + " " + this.Version.DisplayVersion;

    public override string ToString() => this.DisplayStringForMessages;
  }
}
