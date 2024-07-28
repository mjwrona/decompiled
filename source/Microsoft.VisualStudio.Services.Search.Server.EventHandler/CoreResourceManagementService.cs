// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.CoreResourceManagementService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Search.Platform.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityTypes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  public class CoreResourceManagementService : ICoreResourceManagementService, IVssFrameworkService
  {
    private IDisposableReadOnlyList<IJobResourceUtilizationController> m_jobResourceControllers;

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.CheckDeploymentRequestContext();
      this.m_jobResourceControllers = SearchPlatformHelper.GetExtensions<IJobResourceUtilizationController>(systemRequestContext, false);
      foreach (IJobResourceUtilizationController resourceController in (IEnumerable<IJobResourceUtilizationController>) this.m_jobResourceControllers)
        resourceController.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_jobResourceControllers == null)
        return;
      this.m_jobResourceControllers.Dispose();
      this.m_jobResourceControllers = (IDisposableReadOnlyList<IJobResourceUtilizationController>) null;
    }

    public IJobResourceUtilizationController GetJobResourceController(
      IVssRequestContext requestContext,
      IEntityType entityType)
    {
      return this.m_jobResourceControllers.FirstOrDefault<IJobResourceUtilizationController>((Func<IJobResourceUtilizationController, bool>) (controller => ((IEnumerable<IEntityType>) controller.SupportedEntityTypes).Contains<IEntityType>(entityType, (IEqualityComparer<IEntityType>) new EntityTypeComparer()))) ?? (IJobResourceUtilizationController) new CoreJobResourceUtilizationController();
    }
  }
}
