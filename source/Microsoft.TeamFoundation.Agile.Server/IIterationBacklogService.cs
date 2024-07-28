// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.IIterationBacklogService
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

using Microsoft.Azure.Boards.CssNodes;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Server
{
  [DefaultServiceImplementation(typeof (IterationBacklogService))]
  public interface IIterationBacklogService : IVssFrameworkService
  {
    IReadOnlyCollection<LinkQueryResultEntry> GetTaskBoardQueryResults(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      string iterationPath,
      IDictionary queryContext = null);

    IReadOnlyCollection<LinkQueryResultEntry> GetTaskBoardQueryResults(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo projectInfo,
      WebApiTeam team,
      Guid iterationId,
      IDictionary queryContext = null);

    string GetTaskBoardQuery(
      IVssRequestContext requestContext,
      IAgileSettings agileSettings,
      string iterationPath,
      IEnumerable<string> fields);

    string GetTaskBoardQuery(
      IVssRequestContext requestContext,
      CommonStructureProjectInfo projectInfo,
      WebApiTeam team,
      Guid iterationId,
      IEnumerable<string> fields);
  }
}
