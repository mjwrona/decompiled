// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.Checks.Server.ChecksExtensionService
// Assembly: Microsoft.Azure.Pipelines.Checks.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D08C7285-654E-4A4D-BA46-748F0D1E96AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.Checks.Server.dll

using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Pipelines.Checks.Server
{
  internal sealed class ChecksExtensionService : IChecksExtensionService, IVssFrameworkService
  {
    private ChecksExtensionCache m_checkExtensionCache;

    public void ServiceStart(IVssRequestContext requestContext) => this.m_checkExtensionCache = new ChecksExtensionCache(requestContext.GetExtensions<ICheckType>());

    public void ServiceEnd(IVssRequestContext requestContext)
    {
      this.m_checkExtensionCache?.Dispose();
      this.m_checkExtensionCache = (ChecksExtensionCache) null;
    }

    public ICheckType GetCheckInstance(CheckType checkType)
    {
      if (checkType != null)
      {
        if (checkType.Id != Guid.Empty)
          return this.GetCheckType(checkType.Id);
        if (!string.IsNullOrWhiteSpace(checkType.Name))
          return this.GetCheckType(checkType.Name);
      }
      throw new InvalidCheckTypeException(PipelineChecksResources.InvalidCheckTypeException());
    }

    private ICheckType GetCheckType(Guid checkTypeId)
    {
      ICheckType checkType;
      if (this.m_checkExtensionCache.TryGetValue(checkTypeId, out checkType))
        return checkType;
      throw new InvalidCheckTypeException(PipelineChecksResources.CheckTypeIdNotFound((object) checkTypeId));
    }

    private ICheckType GetCheckType(string checkTypeName)
    {
      ICheckType checkType;
      if (this.m_checkExtensionCache.TryGetValue(checkTypeName, out checkType))
        return checkType;
      throw new InvalidCheckTypeException(PipelineChecksResources.CheckTypeNameNotFound((object) checkTypeName));
    }
  }
}
