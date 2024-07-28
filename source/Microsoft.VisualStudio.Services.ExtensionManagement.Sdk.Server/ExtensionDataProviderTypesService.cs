// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ExtensionDataProviderTypesService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ExtensionDataProviderTypesService : IVssFrameworkService
  {
    private IDictionary<string, Type> m_dataProviderTypes;
    private const string s_area = "ContributionService";
    private const string s_layer = "DataProviderTypesService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10013521, "ContributionService", "DataProviderTypesService", nameof (ServiceStart));
      systemRequestContext.CheckDeploymentRequestContext();
      try
      {
        using (IDisposableReadOnlyList<IExtensionDataProvider> extensions = systemRequestContext.GetExtensions<IExtensionDataProvider>(throwOnError: true))
        {
          this.m_dataProviderTypes = (IDictionary<string, Type>) new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (IExtensionDataProvider extensionDataProvider in (IEnumerable<IExtensionDataProvider>) extensions)
            this.m_dataProviderTypes[extensionDataProvider.Name] = extensionDataProvider.GetType();
        }
      }
      finally
      {
        systemRequestContext.TraceLeave(10013522, "ContributionService", "DataProviderTypesService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual IDictionary<string, Type> GetDataProviderTypes() => this.m_dataProviderTypes;
  }
}
