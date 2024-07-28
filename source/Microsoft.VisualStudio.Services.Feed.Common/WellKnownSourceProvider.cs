// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Common.WellKnownSourceProvider
// Assembly: Microsoft.VisualStudio.Services.Feed.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AAC6BCA4-7F6C-4DFE-8058-1CCDD886477F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Common
{
  public sealed class WellKnownSourceProvider
  {
    private readonly IReadOnlyDictionary<string, WellKnownUpstreamSource> knownSourcesByHost;

    public IReadOnlyList<WellKnownUpstreamSource> KnownSources { get; }

    public static WellKnownSourceProvider Instance { get; } = new WellKnownSourceProvider(WellKnownSources.All);

    public WellKnownSourceProvider(
      IReadOnlyList<WellKnownUpstreamSource> knownSources)
    {
      this.KnownSources = knownSources;
      this.knownSourcesByHost = (IReadOnlyDictionary<string, WellKnownUpstreamSource>) knownSources.ToDictionary<WellKnownUpstreamSource, string>((Func<WellKnownUpstreamSource, string>) (x => x.Location.Host), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    public WellKnownUpstreamSource? GetWellKnownSourceOrDefault(Uri location)
    {
      ArgumentUtility.CheckForNull<Uri>(location, nameof (location));
      if (!location.IsAbsoluteUri)
        throw new ArgumentException(Resources.Error_UpstreamLocationMustBeAbsolute(), nameof (location));
      WellKnownUpstreamSource knownUpstreamSource;
      return location.Scheme == Uri.UriSchemeHttps && this.knownSourcesByHost.TryGetValue(location.Host, out knownUpstreamSource) ? knownUpstreamSource : (WellKnownUpstreamSource) null;
    }

    public WellKnownUpstreamSource? GetWellKnownSourceOrDefault(string location)
    {
      Uri result;
      return string.IsNullOrWhiteSpace(location) || !Uri.TryCreate(location, UriKind.Absolute, out result) ? (WellKnownUpstreamSource) null : this.GetWellKnownSourceOrDefault(result);
    }

    public WellKnownUpstreamSource? GetWellKnownSourceOrDefault(UpstreamSource upstreamSource) => this.GetWellKnownSourceOrDefault(upstreamSource.Location);

    public WellKnownUpstreamSource? GetWellKnownSourceByTagNameOrDefault(string tagName) => this.KnownSources.FirstOrDefault<WellKnownUpstreamSource>((Func<WellKnownUpstreamSource, bool>) (x => x.TagName.Equals(tagName, StringComparison.OrdinalIgnoreCase)));
  }
}
