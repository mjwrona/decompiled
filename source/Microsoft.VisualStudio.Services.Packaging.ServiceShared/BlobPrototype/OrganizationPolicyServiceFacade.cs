// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.OrganizationPolicyServiceFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Organization;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class OrganizationPolicyServiceFacade : IOrganizationPolicies
  {
    private readonly IVssRequestContext requestContext;
    private readonly IOrganizationPolicyService service;

    public OrganizationPolicyServiceFacade(IVssRequestContext requestContext)
    {
      this.requestContext = requestContext;
      this.service = requestContext.GetService<IOrganizationPolicyService>();
    }

    public T GetPolicyValue<T>(string policyName, T defaultValue)
    {
      try
      {
        return this.service.GetPolicy<T>(this.requestContext, policyName, defaultValue).EffectiveValue;
      }
      catch (Exception ex)
      {
        return defaultValue;
      }
    }
  }
}
