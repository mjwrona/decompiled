// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScope
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScope
  {
    internal AuthorizationScope(
      AuthorizationScopeUri scopeUri,
      AuthorizationScopeUri[] patternUris,
      IReadOnlyDictionary<string, AuthorizationScopeDescription> scopeDescriptions,
      AuthorizationScopeAvailability availability = AuthorizationScopeAvailability.None,
      string inheritsFrom = null,
      bool isScopeImplmentedThroughAces = false,
      bool publicScopeLookupFromPrivateScope = false)
    {
      if (scopeUri == null)
        throw new ArgumentNullException("scope URI");
      if (patternUris == null)
        throw new ArgumentNullException("pattern URI set");
      if (scopeDescriptions == null)
        throw new ArgumentNullException("scope descriptions");
      if (!scopeUri.IsWellKnownScope)
        throw new ArgumentException("Scope URI must be in the well-known format: '" + scopeUri.ToString() + "'");
      if (!((IEnumerable<AuthorizationScopeUri>) patternUris).Where<AuthorizationScopeUri>((Func<AuthorizationScopeUri, bool>) (patternUri => patternUri != null && !patternUri.IsWellKnownScope)).Any<AuthorizationScopeUri>())
        throw new ArgumentException("Pattern URI set did not contain any valid patterns.");
      this.ScopeDescriptions = scopeDescriptions;
      this.PatternUris = (IReadOnlyList<AuthorizationScopeUri>) patternUris;
      this.ScopeUri = scopeUri;
      this.InheritsFrom = inheritsFrom;
      this.Availability = availability;
      this.IsInternalScope = this.IsSystem();
      this.IsScopeImplmentedThroughAces = isScopeImplmentedThroughAces;
      this.PublicScopeLookupFromPrivateScope = publicScopeLookupFromPrivateScope;
    }

    public AuthorizationScopeAvailability Availability { get; protected set; }

    public bool IsSystem() => this.Availability.HasFlag((Enum) AuthorizationScopeAvailability.System);

    public bool IsPrivate() => !this.IsPublic();

    public bool IsDeprecated() => this.Availability.HasFlag((Enum) AuthorizationScopeAvailability.Deprecated);

    public bool IsPublic() => this.Availability.HasFlag((Enum) AuthorizationScopeAvailability.Public);

    public bool IsInternalScope { get; protected set; }

    public string InheritsFrom { get; protected set; }

    public IReadOnlyList<AuthorizationScopeUri> PatternUris { get; protected set; }

    public IReadOnlyDictionary<string, AuthorizationScopeDescription> ScopeDescriptions { get; protected set; }

    public AuthorizationScopeUri ScopeUri { get; protected set; }

    public bool IsScopeImplmentedThroughAces { get; protected set; }

    public bool PublicScopeLookupFromPrivateScope { get; protected set; }

    public AuthorizationScopeDescription GetDescription(string market = null)
    {
      AuthorizationScopeDescription description = (AuthorizationScopeDescription) null;
      if (this.ScopeDescriptions != null)
      {
        if (market != null)
          this.ScopeDescriptions.TryGetValue(market, out description);
        if (description == null)
          this.ScopeDescriptions.TryGetValue(AuthorizationScopeDescription.FallbackMarket, out description);
      }
      return description;
    }

    public bool Match(string uri, HttpMethod method) => this.PatternUris.Any<AuthorizationScopeUri>((Func<AuthorizationScopeUri, bool>) (patternUri => patternUri.Match(uri, method)));

    public static bool TryCreate(
      string scope,
      string[] patterns,
      AuthorizationScopeAvailability availability,
      string inheritsFrom,
      AuthorizationScopeDescription[] descriptions,
      out AuthorizationScope authorizationScope,
      bool isScopeImplmentedThroughAces = false,
      bool publicScopeLookupFromPrivateScope = false)
    {
      authorizationScope = (AuthorizationScope) null;
      if (string.IsNullOrWhiteSpace(scope) || patterns == null || patterns.Length == 0)
        return false;
      AuthorizationScopeUri scopeUri = new AuthorizationScopeUri(scope);
      AuthorizationScopeUri[] array = ((IEnumerable<string>) patterns).Select<string, AuthorizationScopeUri>((Func<string, AuthorizationScopeUri>) (pattern => AuthorizationScope.TryCreateScopeUri(pattern))).Where<AuthorizationScopeUri>((Func<AuthorizationScopeUri, bool>) (patternUri => patternUri != null)).ToArray<AuthorizationScopeUri>();
      Dictionary<string, AuthorizationScopeDescription> scopeDescriptions = new Dictionary<string, AuthorizationScopeDescription>();
      foreach (AuthorizationScopeDescription description in descriptions)
        scopeDescriptions[description.Market] = (AuthorizationScopeDescription) new AuthorizationSafeScopeDescription(description.Market, description.Title, description.Description);
      try
      {
        authorizationScope = new AuthorizationScope(scopeUri, array, (IReadOnlyDictionary<string, AuthorizationScopeDescription>) scopeDescriptions, availability, inheritsFrom, isScopeImplmentedThroughAces, publicScopeLookupFromPrivateScope);
      }
      catch
      {
        return false;
      }
      return true;
    }

    private static AuthorizationScopeUri TryCreateScopeUri(string uri)
    {
      AuthorizationScopeUri scopeUri = (AuthorizationScopeUri) null;
      AuthorizationScopeUri.TryCreate(uri, out scopeUri);
      return scopeUri;
    }
  }
}
