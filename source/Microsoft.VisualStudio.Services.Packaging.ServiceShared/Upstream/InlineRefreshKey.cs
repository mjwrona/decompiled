// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.InlineRefreshKey
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public sealed record InlineRefreshKey(
    IProtocol Protocol,
    FeedCore Feed,
    IPackageName PackageName,
    bool IsFromExplicitUserAction,
    string ConfigHash) : IPackageNameRequest, IFeedRequest, IProtocolAgnosticFeedRequest
  {
    public bool Equals(InlineRefreshKey? other) => (object) other != null && StringComparer.Ordinal.Equals(this.Protocol.CorrectlyCasedName, other.Protocol.CorrectlyCasedName) && this.Feed.Id == other.Feed.Id && PackageNameComparer.NormalizedName.Equals(this.PackageName, other.PackageName) && this.IsFromExplicitUserAction == other.IsFromExplicitUserAction && StringComparer.Ordinal.Equals(this.ConfigHash, other.ConfigHash);

    public override int GetHashCode() => (((StringComparer.Ordinal.GetHashCode(this.Protocol.CorrectlyCasedName) * 397 ^ this.Feed.Id.GetHashCode()) * 397 ^ PackageNameComparer.NormalizedName.GetHashCode(this.PackageName)) * 397 ^ this.IsFromExplicitUserAction.GetHashCode()) * 397 ^ StringComparer.Ordinal.GetHashCode(this.ConfigHash);

    string? IProtocolAgnosticFeedRequest.UserSuppliedFeedNameOrId => (string) null;

    string? IProtocolAgnosticFeedRequest.UserSuppliedProjectNameOrId => (string) null;
  }
}
