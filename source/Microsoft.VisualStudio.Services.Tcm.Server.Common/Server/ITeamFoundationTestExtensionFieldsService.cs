// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestExtensionFieldsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestExtensionFieldsService))]
  public interface ITeamFoundationTestExtensionFieldsService : IVssFrameworkService
  {
    IList<TestExtensionFieldDetails> AddFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<TestExtensionFieldDetails> fields);

    IList<TestExtensionFieldDetails> QueryFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<string> fieldNamesFilter = null,
      CustomTestFieldScope scopeFilter = CustomTestFieldScope.None);

    IList<TestExtensionFieldDetails> DeleteFields(
      TestManagementRequestContext context,
      Guid projectId,
      IList<string> fieldNames = null);

    void CreateDefaultTestExtensionFieldsForExistingProjects(TestManagementRequestContext context);

    void CreateDefaultTestExtensionFieldsForProject(
      TestManagementRequestContext context,
      Guid projectId);
  }
}
