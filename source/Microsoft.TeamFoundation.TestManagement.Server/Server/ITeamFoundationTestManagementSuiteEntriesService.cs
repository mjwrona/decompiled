// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementSuiteEntriesService
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementSuiteEntriesService))]
  public interface ITeamFoundationTestManagementSuiteEntriesService : IVssFrameworkService
  {
    IEnumerable<SuiteEntry> GetSuiteEntriesFromSuite(
      IVssRequestContext requestContext,
      Guid projectId,
      int suiteId);

    IEnumerable<SuiteEntry> ReorderSuiteEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      int suiteId,
      IEnumerable<SuiteEntryUpdateModel> suiteEntries);
  }
}
