// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Location.Server.ExtendedLocationDataService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Location.Server
{
  internal class ExtendedLocationDataService : IVssFrameworkService
  {
    private int m_apiCacheVersion = -1;
    private ServiceDefinitionCollection m_pluginDefinitions;
    private ServiceDefinitionCollection m_allDefinitions;

    public void ServiceStart(IVssRequestContext systemRequestContext) => systemRequestContext.CheckDeploymentRequestContext();

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IEnumerable<ServiceDefinition> GetDefinitions(IVssRequestContext requestContext)
    {
      requestContext.CheckDeploymentRequestContext();
      this.EnsureInitialized(requestContext);
      return this.m_allDefinitions.GetDefinitions();
    }

    private void EnsureInitialized(IVssRequestContext requestContext)
    {
      if (this.m_pluginDefinitions == null)
      {
        ServiceDefinitionCollection definitions = new ServiceDefinitionCollection(requestContext.ExecutionEnvironment.IsHostedDeployment);
        IVssRequestContext requestContext1 = requestContext.Elevate();
        using (IDisposableReadOnlyList<ILocationDefinitionProvider> extensions = requestContext.GetExtensions<ILocationDefinitionProvider>())
        {
          foreach (ILocationDefinitionProvider definitionProvider in (IEnumerable<ILocationDefinitionProvider>) extensions)
            definitionProvider.RegisterDefinitions(requestContext1, definitions);
        }
        this.m_pluginDefinitions = definitions;
      }
      int apiCacheVersion = this.m_apiCacheVersion;
      int changeVersion = VersionedApiResourceRegistration.ChangeVersion;
      if (this.m_allDefinitions != null && apiCacheVersion == changeVersion)
        return;
      ServiceDefinitionCollection definitionCollection = new ServiceDefinitionCollection(this.m_pluginDefinitions, requestContext.ExecutionEnvironment.IsHostedDeployment);
      TeamFoundationHostType[] foundationHostTypeArray = new TeamFoundationHostType[3]
      {
        TeamFoundationHostType.Deployment,
        TeamFoundationHostType.Application,
        TeamFoundationHostType.ProjectCollection
      };
      foreach (TeamFoundationHostType foundationHostType in foundationHostTypeArray)
      {
        foreach (ApiResourceLocation allLocation in VersionedApiResourceRegistration.GetLocationsForHostType(foundationHostType).GetAllLocations())
          definitionCollection.RegisterDefinition(foundationHostType, allLocation.ToServiceDefinition((InheritLevel) foundationHostType), true);
      }
      if (apiCacheVersion != Interlocked.CompareExchange(ref this.m_apiCacheVersion, changeVersion, apiCacheVersion) && this.m_allDefinitions != null)
        return;
      this.m_allDefinitions = definitionCollection;
    }
  }
}
