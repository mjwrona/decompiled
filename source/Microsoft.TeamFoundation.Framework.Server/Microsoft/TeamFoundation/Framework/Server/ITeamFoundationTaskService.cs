// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ITeamFoundationTaskService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationTaskService))]
  public interface ITeamFoundationTaskService : IVssFrameworkService
  {
    void AddTask(IVssRequestContext requestContext, TeamFoundationTaskCallback callback);

    void AddTask(IVssRequestContext requestContext, TeamFoundationTask task);

    void AddTask(Guid instanceId, TeamFoundationTask task);

    void RemoveTask(IVssRequestContext requestContext, TeamFoundationTaskCallback callback);

    void RemoveTask(IVssRequestContext requestContext, TeamFoundationTask task);

    void RemoveTask(Guid instanceId, TeamFoundationTask task);

    void RemoveAllTasks(Guid instanceId);

    void AddTask(int databaseId, TeamFoundationTask<int> task);

    void RemoveTask(int databaseId, TeamFoundationTask<int> task);
  }
}
