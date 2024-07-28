// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Utils.UpstreamSourceValidator
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Feed.Server.Utils
{
  public abstract class UpstreamSourceValidator
  {
    public static readonly IReadOnlyList<string> SupportedUpstreamProtocols = (IReadOnlyList<string>) new string[7]
    {
      "NuGet",
      "npm",
      "PyPI",
      "Maven",
      "UPack",
      "Cargo",
      "Conda"
    };
    private UpstreamNameValidator nameValidator;
    protected IVssRequestContext requestContext;

    public UpstreamSource Source { get; private set; }

    public static UpstreamSourceValidator Create(
      IVssRequestContext requestContext,
      UpstreamSource source)
    {
      if (source.UpstreamSourceType == UpstreamSourceType.Public)
        return (UpstreamSourceValidator) new PublicSourceValidator(requestContext, source);
      return source.UpstreamSourceType == UpstreamSourceType.Internal ? (UpstreamSourceValidator) new InternalSourceValidator(requestContext, source) : throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstreamSourceType((object) source.UpstreamSourceType));
    }

    protected UpstreamSourceValidator(IVssRequestContext requestContext, UpstreamSource source)
    {
      this.Source = source;
      this.requestContext = requestContext;
      this.nameValidator = new UpstreamNameValidator();
    }

    public virtual async Task ValidateAsync()
    {
      if (this.Source.Id == Guid.Empty)
        throw new InvalidOperationException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceIdIsInvalid((object) this.Source.Id));
      string errorMessage;
      if (!this.nameValidator.IsValidName(this.Source.Name, out errorMessage))
        throw new InvalidUpstreamSourceException(errorMessage);
      ITeamFoundationFeatureAvailabilityService service = this.requestContext.GetService<ITeamFoundationFeatureAvailabilityService>();
      if (((service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UPackSameCollectionUpstreams") ? 1 : (service.IsFeatureEnabled(this.requestContext, "Packaging.Feed.UPackCrossCollectionUpstreams") ? 1 : 0)) == 0 || !StringComparer.OrdinalIgnoreCase.Equals(this.Source.Protocol, "UPack")) && !UpstreamSourceValidator.SupportedUpstreamProtocols.Contains<string>(this.Source.Protocol, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_InvalidUpstreamProtocolValue((object) string.Join(", ", (IEnumerable<string>) UpstreamSourceValidator.SupportedUpstreamProtocols)));
      await this.ValidateUpstreamLocationAsync();
    }

    public string ValidateUpstreamNameAgainstOtherSources(
      Dictionary<string, HashSet<string>> PerProtocolNamesSet)
    {
      if (PerProtocolNamesSet[this.Source.Protocol].Contains(this.Source.Name))
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceNameNotDistinct((object) this.Source.Name));
      return this.Source.Name;
    }

    public string ValidateUpstreamLocationAgainstOtherSources(
      Dictionary<string, HashSet<string>> PerProtocolLocationSet)
    {
      string normalizedLocation = this.GetNormalizedLocation();
      if (PerProtocolLocationSet[this.Source.Protocol].Contains(normalizedLocation))
        throw new InvalidUpstreamSourceException(Microsoft.VisualStudio.Services.Feed.Server.Resources.Error_UpstreamSourceLocationNotDistinct((object) this.Source.Location));
      return normalizedLocation;
    }

    public UpstreamIdentifier CheckUpstreamNameLocationMatchOtherSource(
      HashSet<UpstreamIdentifier> NameAndLocationSet)
    {
      UpstreamIdentifier upstreamIdentifier = new UpstreamIdentifier(this.Source.Name, this.GetNormalizedLocation());
      if (!NameAndLocationSet.Contains(upstreamIdentifier))
        NameAndLocationSet.ForEach<UpstreamIdentifier>((Action<UpstreamIdentifier>) (key => upstreamIdentifier.CheckForMismatch(key)));
      return upstreamIdentifier;
    }

    public abstract string GetNormalizedLocation();

    protected abstract Task ValidateUpstreamLocationAsync();
  }
}
