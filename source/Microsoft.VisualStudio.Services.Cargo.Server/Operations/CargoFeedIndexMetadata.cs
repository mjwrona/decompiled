// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cargo.Server.Operations.CargoFeedIndexMetadata
// Assembly: Microsoft.VisualStudio.Services.Cargo.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 148B8823-9815-48AA-B93D-5DED42B9B7A4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cargo.Server.dll

using System.Collections.Immutable;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cargo.Server.Operations
{
  public record CargoFeedIndexMetadata(
    ImmutableDictionary<string, ImmutableArray<string>> Features,
    IImmutableList<string> Authors,
    string? DocumentationUrl,
    string? HomepageUrl,
    string? ReadmeText,
    string? ReadmeFilePath,
    IImmutableList<string> Categories,
    string? LicenseExpression,
    string? LicenseFilePath,
    string? RepositoryUrl,
    string? Links)
  {
    public const int SchemaVersion = 1;
  }
}
