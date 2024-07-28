// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Authorization.Server.DefinitionExtensionService
// Assembly: Microsoft.Azure.Pipelines.Authorization.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B31FF9-0E6B-45B0-A4F8-77598802CAB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Authorization.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Authorization.Server
{
  internal class DefinitionExtensionService : IDefinitionExtensionService, IVssFrameworkService
  {
    private DefinitionExtensionCache m_definitionExtensionCache;

    public IDefinitionPlugin GetDefinitionPlugin() => this.m_definitionExtensionCache.DefinitionPlugin;

    public IPipelineDefinitionPlugin GetPipelineDefinitionPlugin() => this.m_definitionExtensionCache.PipelineDefinitionPlugin;

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_definitionExtensionCache != null && this.m_definitionExtensionCache is IDisposable)
        ((IDisposable) this.m_definitionExtensionCache).Dispose();
      this.m_definitionExtensionCache = (DefinitionExtensionCache) null;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext) => this.m_definitionExtensionCache = new DefinitionExtensionCache(systemRequestContext.GetExtension<IDefinitionPlugin>(), systemRequestContext.GetExtension<IPipelineDefinitionPlugin>());
  }
}
