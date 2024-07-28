// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementPointService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementPointService))]
  public interface ITeamFoundationTestManagementPointService : IVssFrameworkService
  {
    TestPointsQuery GetPointsByQuery(
      IVssRequestContext requestContext,
      string projectId,
      TestPointsQuery query,
      int skip,
      int top,
      bool includeNames = false);

    List<TestPoint> GetPoints(
      IVssRequestContext tfsRequestContext,
      string projectName,
      int planId,
      List<int> pointIds,
      string[] testCaseProperties);

    List<TestPointRecord> QueryTestPointsByOutcomeMigrationDate(
      IVssRequestContext requestContext,
      int batchSize,
      TestPointWatermark fromWatermark,
      out TestPointWatermark toWatermark,
      TestArtifactSource dataSource);

    List<TestPointHistoryRecord> QueryTestPointHistoryByWatermarkDate(
      IVssRequestContext requestContext,
      int batchSize,
      TestPointHistoryWatermark fromWatermark,
      out TestPointHistoryWatermark toWatermark,
      TestArtifactSource dataSource);
  }
}
