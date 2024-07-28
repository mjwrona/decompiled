// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Operations.OperationsService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Operations
{
  internal class OperationsService : IOperationsService, IVssFrameworkService
  {
    private Dictionary<Guid, IOperationsPlugin> m_pluginResources;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
      {
        IDisposableReadOnlyList<IOperationsPlugin> extensions = systemRequestContext.GetService<IVssExtensionManagementService>().GetExtensions<IOperationsPlugin>(systemRequestContext, ExtensionLifetime.Service);
        this.m_pluginResources = new Dictionary<Guid, IOperationsPlugin>();
        foreach (IOperationsPlugin operationsPlugin in (IEnumerable<IOperationsPlugin>) extensions)
        {
          if (this.m_pluginResources.ContainsKey(operationsPlugin.OperationPluginId))
            throw new OperationPluginWithSameIdException(operationsPlugin.OperationPluginId);
          this.m_pluginResources.Add(operationsPlugin.OperationPluginId, operationsPlugin);
        }
      }
      else
        this.m_pluginResources = systemRequestContext.To(TeamFoundationHostType.Deployment).GetService<OperationsService>().m_pluginResources;
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public Operation GetOperation(
      IVssRequestContext requestContext,
      Guid pluginId,
      Guid operationId)
    {
      IOperationsPlugin operationsPlugin;
      if (!this.m_pluginResources.TryGetValue(pluginId, out operationsPlugin))
        throw new OperationPluginNotFoundException(pluginId);
      if (!operationsPlugin.HasPermission(requestContext, operationId))
        throw new OperationPluginNoPermission(pluginId, operationId);
      return operationsPlugin.GetOperation(requestContext, operationId);
    }
  }
}
