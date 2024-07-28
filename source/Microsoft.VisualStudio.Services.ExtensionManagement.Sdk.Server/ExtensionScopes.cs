// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionScopes
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.DelegatedAuthorization;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class ExtensionScopes
  {
    public static List<Scope> GetScopesForExtension(
      IVssRequestContext requestContext,
      ExtensionManifest extensionManifest)
    {
      List<Scope> scopesForExtension = new List<Scope>();
      if (extensionManifest != null && extensionManifest.Scopes != null)
        scopesForExtension = ((IEnumerable<AuthorizationScopeDefinition>) AuthorizationScopeDefinitions.Default.scopes).Where<AuthorizationScopeDefinition>((Func<AuthorizationScopeDefinition, bool>) (x => x.availability == AuthorizationScopeAvailability.Public && extensionManifest.Scopes.Contains<string>(x.scope, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))).Select<AuthorizationScopeDefinition, Scope>((Func<AuthorizationScopeDefinition, Scope>) (x => new Scope()
        {
          Description = x.description,
          Title = x.title,
          Value = x.scope
        })).OrderBy<Scope, string>((Func<Scope, string>) (x => x.Title)).ToList<Scope>();
      return scopesForExtension;
    }
  }
}
