// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.ProjectComponent9
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class ProjectComponent9 : ProjectComponent8
  {
    protected override void BindDescription(string parameterName, string projectDescription) => this.BindString(parameterName, projectDescription, int.MaxValue, BindStringBehavior.Unchanged, SqlDbType.NVarChar);

    protected override ObjectBinder<ProjectInfo> CreateProjectInfoColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectInfoColumns3();

    protected override ObjectBinder<ProjectInfo> CreateProjectWatermarkColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectWatermarkColumns2();

    protected override ObjectBinder<ProjectInfo> CreateProjectHistoryColumnsBinder() => (ObjectBinder<ProjectInfo>) new ProjectHistoryColumns3();
  }
}
