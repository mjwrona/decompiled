// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionPropertyProviderTypesService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionPropertyProviderTypesService : IVssFrameworkService
  {
    private IDictionary<string, Type> m_providerTypes;
    private const string s_area = "ContributionService";
    private const string s_layer = "PropertyProviderTypesService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10015551, "ContributionService", "PropertyProviderTypesService", nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        using (IDisposableReadOnlyList<IContributionPropertyProvider> extensions = systemRequestContext.GetExtensions<IContributionPropertyProvider>(throwOnError: true))
        {
          this.m_providerTypes = (IDictionary<string, Type>) new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (IContributionPropertyProvider propertyProvider in (IEnumerable<IContributionPropertyProvider>) extensions)
            this.m_providerTypes[propertyProvider.Name] = propertyProvider.GetType();
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(10015552, "ContributionService", "PropertyProviderTypesService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IDictionary<string, Type> GetPropertyProviderTypes() => this.m_providerTypes;
  }
}
