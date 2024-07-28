// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.PluginServicingExecutionHandlerProvider
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class PluginServicingExecutionHandlerProvider : 
    IServicingExecutionHandlerProvider,
    IDisposable
  {
    private IDisposableReadOnlyList<IServicingStepGroupExecutionHandler> m_servicingStepgGroupExecutionHandlerPlugins;
    private IDisposableReadOnlyList<IServicingOperationExecutionHandler> m_servicingOperationsExecutionHandlerPlugins;
    private Dictionary<string, IServicingStepGroupExecutionHandler> m_stepGroupHandlers = new Dictionary<string, IServicingStepGroupExecutionHandler>();
    private Dictionary<string, IServicingOperationExecutionHandler> m_operationHandlers = new Dictionary<string, IServicingOperationExecutionHandler>();

    public PluginServicingExecutionHandlerProvider(string pluginDirectory)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(pluginDirectory, nameof (pluginDirectory));
      this.LoadHandlers(pluginDirectory);
    }

    public IServicingStepGroupExecutionHandler GetServicingStepGroupExecutionHandler(
      string fullTypeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fullTypeName, nameof (fullTypeName));
      IServicingStepGroupExecutionHandler executionHandler;
      if (!this.m_stepGroupHandlers.TryGetValue(fullTypeName, out executionHandler))
        throw new ServicingStepGroupHandlerNotFoundException(FrameworkResources.ServicingStepGroupHandlerNotFound((object) fullTypeName));
      return executionHandler;
    }

    public IServicingOperationExecutionHandler GetServicingOperationExecutionHandler(
      string fullTypeName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(fullTypeName, nameof (fullTypeName));
      IServicingOperationExecutionHandler executionHandler;
      if (!this.m_operationHandlers.TryGetValue(fullTypeName, out executionHandler))
        throw new ServicingOperationHandlerNotFoundException(FrameworkResources.ServicingOperationHandlerNotFound((object) fullTypeName));
      return executionHandler;
    }

    public void Dispose()
    {
      if (this.m_servicingOperationsExecutionHandlerPlugins != null)
      {
        this.m_servicingOperationsExecutionHandlerPlugins.Dispose();
        this.m_servicingOperationsExecutionHandlerPlugins = (IDisposableReadOnlyList<IServicingOperationExecutionHandler>) null;
      }
      if (this.m_servicingStepgGroupExecutionHandlerPlugins == null)
        return;
      this.m_servicingStepgGroupExecutionHandlerPlugins.Dispose();
      this.m_servicingStepgGroupExecutionHandlerPlugins = (IDisposableReadOnlyList<IServicingStepGroupExecutionHandler>) null;
    }

    private void LoadHandlers(string pluginDirectory)
    {
      this.m_servicingStepgGroupExecutionHandlerPlugins = VssExtensionManagementService.GetExtensionsRaw<IServicingStepGroupExecutionHandler>(pluginDirectory);
      foreach (IServicingStepGroupExecutionHandler executionHandlerPlugin in (IEnumerable<IServicingStepGroupExecutionHandler>) this.m_servicingStepgGroupExecutionHandlerPlugins)
        this.m_stepGroupHandlers[executionHandlerPlugin.GetType().FullName] = executionHandlerPlugin;
      this.m_servicingOperationsExecutionHandlerPlugins = VssExtensionManagementService.GetExtensionsRaw<IServicingOperationExecutionHandler>(pluginDirectory);
      foreach (IServicingOperationExecutionHandler executionHandlerPlugin in (IEnumerable<IServicingOperationExecutionHandler>) this.m_servicingOperationsExecutionHandlerPlugins)
        this.m_operationHandlers[executionHandlerPlugin.GetType().FullName] = executionHandlerPlugin;
    }
  }
}
