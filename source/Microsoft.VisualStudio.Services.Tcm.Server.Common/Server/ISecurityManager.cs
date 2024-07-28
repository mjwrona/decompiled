// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ISecurityManager
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface ISecurityManager
  {
    bool HasGenericReadPermission(TestManagementRequestContext context);

    void CheckManageTestControllersPermission(TestManagementRequestContext context);

    bool HasManageTestControllersPermission(TestManagementRequestContext context);

    void CheckManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri);

    bool HasManageTestEnvironmentsPermission(
      TestManagementRequestContext context,
      string projectUri);

    void CheckTeamProjectCreatePermission(TestManagementRequestContext context);

    bool HasTeamProjectCreatePermission(TestManagementRequestContext context);

    void CheckTeamProjectDeletePermission(
      TestManagementRequestContext context,
      string teamProjectUri);

    bool HasTeamProjectDeletePermission(TestManagementRequestContext context, string teamProjectUri);

    void CheckProjectWritePermission(TestManagementRequestContext context, string projectUri);

    bool HasProjectWritePermission(TestManagementRequestContext context, string projectUri);

    bool HasProjectReadPermission(TestManagementRequestContext context, string projectUri);

    void CheckProjectReadPermission(TestManagementRequestContext context, string projectUri);

    void CheckProjectMigrationPermissions(TestManagementRequestContext context);

    void CheckRetentionSettingsModifyPermissions(IVssRequestContext context, string projectUri);

    bool CheckProjectSettingsPermission(TestManagementRequestContext context, string projectUri);

    void CheckViewTestResultsPermission(TestManagementRequestContext context, string projectUri);

    bool HasViewTestResultsPermission(TestManagementRequestContext context, string projectUri);

    void CheckPublishTestResultsPermission(TestManagementRequestContext context, string projectUri);

    bool HasPublishTestResultsPermission(TestManagementRequestContext context, string projectUri);

    void CheckDeleteTestResultsPermission(TestManagementRequestContext context, string projectUri);

    bool HasDeleteTestResultsPermission(TestManagementRequestContext context, string projectUri);

    bool CanViewTestResult(TestManagementRequestContext context, string testCaseArea);

    void CheckManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri);

    bool HasManageTestConfigurationsPermission(
      TestManagementRequestContext context,
      string projectUri);

    void CheckManageTestPlansPermission(TestManagementRequestContext context, string areaUri);

    void CheckManageTestSuitesPermission(TestManagementRequestContext context, string areaUri);

    void CheckForViewNodeAndThrow(
      TestManagementRequestContext context,
      string areaUri,
      string exceptionMessage);

    bool IsJobAgent(TestManagementRequestContext context);

    bool IsServiceAccount(TestManagementRequestContext context);

    void CheckServiceAccount(TestManagementRequestContext context);

    void CheckWorkItemWritePermission(TestManagementRequestContext context, string areaUri);

    void CheckWorkItemReadPermission(TestManagementRequestContext context, string areaUri);

    bool HasWorkItemReadPermission(TestManagementRequestContext context, string areaUri);

    List<T> FilterViewWorkItemOnAreaPath<T>(
      TestManagementRequestContext context,
      IEnumerable<KeyValuePair<int, T>> items,
      ITestManagementWorkItemCacheService workItemCacheService);

    void FilterViewWorkItemOptional<T>(TestManagementRequestContext context, IList<T> list);

    bool HasTestManagementPermission(TestManagementRequestContext context);

    void CheckTestManagementPermission(TestManagementRequestContext context);
  }
}
