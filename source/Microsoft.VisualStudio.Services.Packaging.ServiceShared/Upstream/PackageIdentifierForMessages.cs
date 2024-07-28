// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.PackageIdentifierForMessages
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System.Diagnostics.CodeAnalysis;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public readonly struct PackageIdentifierForMessages
  {
    public static readonly PackageIdentifierForMessages None;

    [MemberNotNullWhen(true, "PackageDisplayString")]
    public bool HasValue
    {
      [MemberNotNullWhen(true, "PackageDisplayString")] get => this.PackageDisplayString != null;
    }

    public string? PackageDisplayString { get; }

    public string? FileDisplayString { get; }

    public static PackageIdentifierForMessages From(IPackageName packageName) => new PackageIdentifierForMessages(packageName);

    public static PackageIdentifierForMessages From(
      IPackageIdentity packageIdentity,
      IPackageFileName? fileName = null)
    {
      return new PackageIdentifierForMessages(packageIdentity, fileName);
    }

    public PackageIdentifierForMessages(IPackageName packageName)
    {
      this.FileDisplayString = (string) null;
      this.PackageDisplayString = packageName.DisplayName;
    }

    public PackageIdentifierForMessages(
      IPackageIdentity packageIdentity,
      IPackageFileName? packageFileName = null)
    {
      this.PackageDisplayString = packageIdentity.DisplayStringForMessages;
      this.FileDisplayString = packageFileName?.Path;
    }
  }
}
