// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Ingestion.CargoPackageMetadataDependency
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using Microsoft.VisualStudio.Services.Cargo.Server.PackageIdentity;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Ingestion
{
  public record CargoPackageMetadataDependency(
    CargoPackageName ActualPackageName,
    string VersionRequirement,
    IImmutableList<string> Features,
    bool Optional,
    bool DefaultFeaturesEnabled,
    string? TargetPlatform,
    string Kind,
    string? RegistryIndex,
    string? DeclaredName)
  {
    public bool IsRenamed => !string.IsNullOrWhiteSpace(this.DeclaredName);

    [CompilerGenerated]
    protected virtual bool PrintMembers(StringBuilder builder)
    {
      RuntimeHelpers.EnsureSufficientExecutionStack();
      builder.Append("ActualPackageName = ");
      builder.Append((object) this.ActualPackageName);
      builder.Append(", VersionRequirement = ");
      builder.Append((object) this.VersionRequirement);
      builder.Append(", Features = ");
      builder.Append((object) this.Features);
      builder.Append(", Optional = ");
      builder.Append(this.Optional.ToString());
      builder.Append(", DefaultFeaturesEnabled = ");
      builder.Append(this.DefaultFeaturesEnabled.ToString());
      builder.Append(", TargetPlatform = ");
      builder.Append((object) this.TargetPlatform);
      builder.Append(", Kind = ");
      builder.Append((object) this.Kind);
      builder.Append(", RegistryIndex = ");
      builder.Append((object) this.RegistryIndex);
      builder.Append(", DeclaredName = ");
      builder.Append((object) this.DeclaredName);
      builder.Append(", IsRenamed = ");
      builder.Append(this.IsRenamed.ToString());
      return true;
    }
  }
}
