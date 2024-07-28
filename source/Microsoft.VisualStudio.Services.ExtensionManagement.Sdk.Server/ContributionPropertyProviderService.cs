// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.ContributionPropertyProviderService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class ContributionPropertyProviderService : 
    IContributionPropertyProviderService,
    IVssFrameworkService
  {
    private IDictionary<string, Type> m_providerTypes;
    private const string s_propertyProviderRequestContextKey = "PropertyProviders";
    private const string s_area = "ContributionService";
    private const string s_layer = "PropertyProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.TraceEnter(10015541, "ContributionService", "PropertyProviderService", nameof (ServiceStart));
      try
      {
        this.m_providerTypes = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<ContributionPropertyProviderTypesService>().GetPropertyProviderTypes();
      }
      finally
      {
        systemRequestContext.TraceLeave(10015542, "ContributionService", "PropertyProviderService", nameof (ServiceStart));
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public ContributionPropertyProviderResult GetProperties(
      IVssRequestContext requestContext,
      ContributionNode propertyProviderContribution,
      ContributionNode targetContribution)
    {
      ContributionPropertyProviderResult properties = new ContributionPropertyProviderResult();
      using (PerformanceTimer.StartMeasure(requestContext, "ContributionPropertyProviderService.GetProperties", propertyProviderContribution.Id))
      {
        string property = propertyProviderContribution.Contribution.GetProperty<string>("name");
        if (string.IsNullOrEmpty(property))
        {
          properties.Error = string.Format("Property provider contribution '{0}' is missing property '{1}'.", (object) propertyProviderContribution.Id, (object) "name");
        }
        else
        {
          IContributionPropertyProvider propertyProvider = this.GetPropertyProvider(requestContext, property);
          if (propertyProvider != null)
          {
            try
            {
              properties.Properties = propertyProvider.GetProperties(requestContext, propertyProviderContribution, targetContribution);
            }
            catch (Exception ex)
            {
              properties.Error = ex.Message;
              requestContext.TraceException(10015543, "ContributionService", "PropertyProviderService", ex);
            }
          }
          else
            properties.Error = string.Format("Property provider contribution '{0}' specifies a provider '{1}' which does not exist.", (object) propertyProviderContribution.Id, (object) property);
        }
      }
      return properties;
    }

    private IContributionPropertyProvider GetPropertyProvider(
      IVssRequestContext requestContext,
      string providerName)
    {
      IContributionPropertyProvider propertyProvider = (IContributionPropertyProvider) null;
      Dictionary<string, IContributionPropertyProvider> dictionary;
      if (!requestContext.Items.TryGetValue<Dictionary<string, IContributionPropertyProvider>>("PropertyProviders", out dictionary))
      {
        dictionary = new Dictionary<string, IContributionPropertyProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        requestContext.Items["PropertyProviders"] = (object) dictionary;
      }
      Type type;
      if (!dictionary.TryGetValue(providerName, out propertyProvider) && this.m_providerTypes.TryGetValue(providerName, out type))
      {
        propertyProvider = (IContributionPropertyProvider) Activator.CreateInstance(type);
        if (propertyProvider != null)
          dictionary[providerName] = propertyProvider;
      }
      return propertyProvider;
    }
  }
}
