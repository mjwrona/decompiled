// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement.ContributedFeatureValuePlugin`1
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.FeatureManagement;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement
{
  public abstract class ContributedFeatureValuePlugin<T> : IContributedFeatureValuePlugin
  {
    public abstract string Name { get; }

    public abstract T ParseProperties(IDictionary<string, object> properties);

    public abstract ContributedFeatureEnabledValue GetEnabledValue(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      T properties,
      IDictionary<string, string> scopeValues,
      out string reason);

    public object ProcessProperties(IDictionary<string, object> properties) => (object) this.ParseProperties(properties);

    public ContributedFeatureEnabledValue ComputeEnabledValue(
      IVssRequestContext requestContext,
      ContributedFeature feature,
      object properties,
      IDictionary<string, string> scopeValues,
      out string reason)
    {
      return this.GetEnabledValue(requestContext, feature, (T) properties, scopeValues, out reason);
    }
  }
}
