// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionFilter`1
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Newtonsoft.Json.Linq;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public abstract class ContributionFilter<T> : IContributionFilter
  {
    public abstract string Name { get; }

    public abstract T ParseProperties(JObject properties);

    public abstract bool ApplyConstraint(
      IVssRequestContext requestContext,
      T properties,
      Contribution contribution);

    public object ProcessProperties(JObject properties) => (object) this.ParseProperties(properties);

    public bool ApplyFilter(
      IVssRequestContext requestContext,
      object properties,
      Contribution contribution)
    {
      return this.ApplyConstraint(requestContext, (T) properties, contribution);
    }
  }
}
