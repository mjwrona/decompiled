// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IResultRetentionSettingsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (ResultRetentionSettingsService))]
  public interface IResultRetentionSettingsService : IVssFrameworkService
  {
    ResultRetentionSettings Create(
      TestManagementRequestContext context,
      GuidAndString projectId,
      ResultRetentionSettings settings,
      bool skipPermissionsCheck = false);

    ResultRetentionSettings Get(TestManagementRequestContext context, GuidAndString projectId);

    ResultRetentionSettings Update(
      TestManagementRequestContext context,
      GuidAndString projectId,
      ResultRetentionSettings settings,
      bool skipPermissionsCheck = false);

    void CreateResultRetentionSettingsForNewProject(
      TestManagementRequestContext context,
      GuidAndString projectId);

    void CreateResultRetentionSettingsForExistingProjects(TestManagementRequestContext context);
  }
}
