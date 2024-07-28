// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Charts.IBoardsChartService
// Assembly: Microsoft.Azure.Boards.Charts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EABADF19-3537-403E-8E3C-4185CE6D1F3B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Charts.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.IO;

namespace Microsoft.Azure.Boards.Charts
{
  [DefaultServiceImplementation(typeof (BoardsChartService))]
  public interface IBoardsChartService : IVssFrameworkService
  {
    Stream GenerateBoardChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      WebApiTeam team,
      string backlogLevelId,
      int width,
      int height,
      bool showDetails,
      string title);

    Stream GenerateIterationChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      Guid iterationId,
      WebApiTeam team,
      int width,
      int height,
      bool showDetails,
      string title);

    Stream GenerateIterationsChartImage(
      IVssRequestContext requestContext,
      string name,
      IAgileSettings agileSettings,
      Guid projectId,
      int iterationsNumber,
      int width,
      int height,
      bool showDetails,
      string title);
  }
}
