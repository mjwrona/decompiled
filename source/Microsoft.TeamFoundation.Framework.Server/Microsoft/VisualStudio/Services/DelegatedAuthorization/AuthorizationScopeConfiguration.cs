// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScopeConfiguration
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScopeConfiguration
  {
    public const string ScopeDelimiter = " ";
    private const char ScopeDelimiterCharacter = ' ';

    internal AuthorizationScopeConfiguration()
    {
    }

    internal AuthorizationScopeConfiguration(AuthorizationScopeDefinitions scopeDefinitions)
    {
      IList<AuthorizationScope> source = (IList<AuthorizationScope>) new List<AuthorizationScope>();
      IDictionary<string, AuthorizationScopeDefinition> scopeDefinitionsMap = (IDictionary<string, AuthorizationScopeDefinition>) new Dictionary<string, AuthorizationScopeDefinition>();
      foreach (AuthorizationScopeDefinition scope in scopeDefinitions.scopes)
      {
        if (!string.IsNullOrEmpty(scope.scope))
          scopeDefinitionsMap.Add(scope.scope, scope);
      }
      foreach (AuthorizationScopeDefinition scope in scopeDefinitions.scopes)
      {
        AuthorizationScope authorizationScope = (AuthorizationScope) null;
        List<AuthorizationScopeDescription> scopeDescriptionList = scope.localizations == null ? new List<AuthorizationScopeDescription>() : ((IEnumerable<AuthorizationScopeLocalization>) scope.localizations).Select<AuthorizationScopeLocalization, AuthorizationScopeDescription>((Func<AuthorizationScopeLocalization, AuthorizationScopeDescription>) (localization => AuthorizationSafeScopeDescription.TryCreate(localization))).Where<AuthorizationScopeDescription>((Func<AuthorizationScopeDescription, bool>) (description => description != null)).ToList<AuthorizationScopeDescription>();
        if (!string.IsNullOrEmpty(scope.title))
        {
          if (!string.IsNullOrEmpty(scope.description))
          {
            try
            {
              scopeDescriptionList.Add((AuthorizationScopeDescription) new AuthorizationSafeScopeDescription(AuthorizationScopeDescription.FallbackMarket, scope.title, scope.description));
            }
            catch
            {
            }
          }
        }
        string[] strArray = scope.patterns;
        string[] inheritedPatterns = AuthorizationScopeConfiguration.GetInheritedPatterns(scopeDefinitionsMap, scope.scope);
        if (inheritedPatterns.Length != 0)
          strArray = ((IEnumerable<string>) inheritedPatterns).Union<string>((IEnumerable<string>) strArray).ToArray<string>();
        if (AuthorizationScope.TryCreate(scope.scope, strArray, scope.availability, scope.inheritsFrom, scopeDescriptionList.ToArray(), out authorizationScope, scope.isScopeImplmentedThroughAces, scope.publicScopeLookupFromPrivateScope))
          source.Add(authorizationScope);
      }
      this.Scopes = (IReadOnlyList<AuthorizationScope>) source.ToArray<AuthorizationScope>();
    }

    internal static string[] GetInheritedPatterns(
      IDictionary<string, AuthorizationScopeDefinition> scopeDefinitionsMap,
      string scope)
    {
      IList<string> source = (IList<string>) new List<string>();
      AuthorizationScopeDefinition authorizationScopeDefinition1 = (AuthorizationScopeDefinition) null;
      if (scopeDefinitionsMap.TryGetValue(scope, out authorizationScopeDefinition1) && !string.IsNullOrEmpty(authorizationScopeDefinition1.inheritsFrom))
      {
        AuthorizationScopeDefinition authorizationScopeDefinition2 = (AuthorizationScopeDefinition) null;
        if (scopeDefinitionsMap.TryGetValue(authorizationScopeDefinition1.inheritsFrom, out authorizationScopeDefinition2))
          source = (IList<string>) ((IEnumerable<string>) authorizationScopeDefinition2.patterns).Union<string>((IEnumerable<string>) AuthorizationScopeConfiguration.GetInheritedPatterns(scopeDefinitionsMap, authorizationScopeDefinition2.scope)).ToList<string>();
      }
      return source.ToArray<string>();
    }

    public virtual bool IsEnabled => this.Scopes.Count > 0;

    public virtual IReadOnlyList<AuthorizationScope> Scopes { get; protected set; }

    public virtual AuthorizationScope[] GetScopes(string normalizedScopes) => this.GetScopes(normalizedScopes.Split(' '));

    public virtual AuthorizationScope[] GetScopes(params string[] normalizedScopes) => this.Scopes.Where<AuthorizationScope>((Func<AuthorizationScope, bool>) (scope => ((IEnumerable<string>) normalizedScopes).Any<string>((Func<string, bool>) (normalizedScope => scope.ScopeUri.Match(normalizedScope))))).ToArray<AuthorizationScope>();

    public virtual bool HasScopePatternMatch(
      string normalizedScopes,
      string uri,
      HttpMethod method,
      bool overrideScopeImplementedThroughAces = false)
    {
      return this.MatchFirstScopePattern(normalizedScopes, uri, method, overrideScopeImplementedThroughAces) != null;
    }

    public virtual bool HasImpersonationScope(
      string normalizedScopes,
      string uri,
      HttpMethod method)
    {
      AuthorizationScope[] array = ((IEnumerable<string>) normalizedScopes.Split(' ')).Select<string, AuthorizationScope>((Func<string, AuthorizationScope>) (x => this.MatchScope(x, true))).Where<AuthorizationScope>((Func<AuthorizationScope, bool>) (x => x != null)).ToArray<AuthorizationScope>();
      return ((IEnumerable<AuthorizationScope>) array).Any<AuthorizationScope>() && ((IEnumerable<AuthorizationScope>) array).Any<AuthorizationScope>((Func<AuthorizationScope, bool>) (x => x.ScopeUri.IsWellKnownScope && "user_impersonation".Equals(x.ScopeUri.Uri.OriginalString)));
    }

    public virtual AuthorizationScope MatchFirstScopePattern(
      string normalizedScopes,
      string uri,
      HttpMethod method,
      bool overrideScopeImplementedThroughAces = false)
    {
      return this.MatchFirstScopePattern(uri, method, (overrideScopeImplementedThroughAces ? 1 : 0) != 0, normalizedScopes.Split(' '));
    }

    public virtual AuthorizationScope MatchFirstScopePattern(
      string uri,
      HttpMethod method,
      bool overrideScopeImplementedThroughAces = false,
      params string[] normalizedScopes)
    {
      return ((IEnumerable<string>) normalizedScopes).Select<string, AuthorizationScope>((Func<string, AuthorizationScope>) (normalizedScope => this.MatchScope(normalizedScope, true))).Where<AuthorizationScope>((Func<AuthorizationScope, bool>) (scope => scope.IsScopeImplmentedThroughAces && !overrideScopeImplementedThroughAces || scope.Match(uri, method))).FirstOrDefault<AuthorizationScope>();
    }

    public virtual AuthorizationScope MatchPattern(string uri, HttpMethod method) => this.Scopes.FirstOrDefault<AuthorizationScope>((Func<AuthorizationScope, bool>) (scope => scope.Match(uri, method)));

    public virtual AuthorizationScope MatchScope(string uri, bool matchSystemScopes = false) => this.Scopes.FirstOrDefault<AuthorizationScope>((Func<AuthorizationScope, bool>) (scope => (matchSystemScopes || !scope.IsSystem()) && scope.ScopeUri.Match(uri)));

    public virtual string CompareRequestedScopes(string scopes, string requestedScopes)
    {
      if (scopes == null)
        throw new ArgumentNullException(nameof (scopes));
      if (requestedScopes == null)
        throw new ArgumentNullException(nameof (requestedScopes));
      Dictionary<string, string> dictionary = this.SplitScopes(scopes).ToDictionary<(string, string), string, string>((Func<(string, string), string>) (k => k.lower), (Func<(string, string), string>) (v => v.original));
      List<(string original, string lower)> list = this.SplitScopes(requestedScopes).ToList<(string, string)>();
      string empty = string.Empty;
      foreach ((string original, string lower) tuple in list)
      {
        if (!dictionary.ContainsKey(tuple.lower))
        {
          if (empty.Length > 0)
            empty += " ";
          empty += tuple.original;
        }
      }
      if (empty.Length > 0)
        throw new ArgumentException("Scope(s) is not valid: '" + empty + "'");
      return requestedScopes;
    }

    public virtual string NormalizeScopes(
      string scopes,
      out bool usingUriScopes,
      bool matchInternalScopes = false)
    {
      return string.Join(" ", this.SplitScopes(scopes, matchInternalScopes, out usingUriScopes));
    }

    private IEnumerable<(string original, string lower)> SplitScopes(string scopes) => (IEnumerable<(string, string)>) ((IEnumerable<string>) scopes.Split(new char[1]
    {
      ' '
    }, StringSplitOptions.RemoveEmptyEntries)).Select<string, (string, string)>((Func<string, (string, string)>) (scope => (scope, scope.ToLowerInvariant()))).OrderBy<(string, string), string>((Func<(string, string), string>) (scope => scope.lower), (IComparer<string>) StringComparer.InvariantCulture);

    public virtual string[] SplitScopes(
      string scopes,
      bool matchInternalScopes,
      out bool usingUriScopes)
    {
      usingUriScopes = false;
      bool usingModernScopes = false;
      if (scopes == null)
        throw new ArgumentNullException(nameof (scopes));
      IEnumerable<(string original, string lower)> source = !string.IsNullOrWhiteSpace(scopes) ? this.SplitScopes(scopes) : throw new ArgumentException("Scopes are required: '" + scopes + "'");
      foreach ((string original, string lower) tuple in source)
      {
        if (this.MatchScope(tuple.lower, matchInternalScopes) == null)
        {
          if (!matchInternalScopes && this.MatchScope(tuple.lower, true) != null)
            throw new ArgumentException("Scope is not valid: '" + tuple.original + "'");
          usingModernScopes = true;
        }
        else
          usingUriScopes = true;
        if (usingModernScopes & usingUriScopes)
          throw new ArgumentException("Scope is not valid. Cannot mix uri based and modern scopes: '" + tuple.original + "'");
      }
      return source.Select<(string, string), string>((Func<(string, string), string>) (s => !usingModernScopes ? s.lower : s.original)).ToArray<string>();
    }
  }
}
