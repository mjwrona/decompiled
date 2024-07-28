// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DelegatedAuthorization.AuthorizationScopeDefinition
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.VisualStudio.Services.DelegatedAuthorization
{
  public class AuthorizationScopeDefinition
  {
    public string scope { get; set; }

    public string title { get; set; }

    public string description { get; set; }

    public AuthorizationScopeLocalization[] localizations { get; set; }

    public string[] patterns { get; set; }

    public AuthorizationScopeAvailability availability { get; set; }

    public string availabilityFeatureFlag { get; set; }

    public string inheritsFrom { get; set; }

    public string groupingKey { get; set; }

    public string firstPartyExtensionName { get; set; }

    public bool isScopeImplmentedThroughAces { get; set; }

    public bool publicScopeLookupFromPrivateScope { get; set; }
  }
}
