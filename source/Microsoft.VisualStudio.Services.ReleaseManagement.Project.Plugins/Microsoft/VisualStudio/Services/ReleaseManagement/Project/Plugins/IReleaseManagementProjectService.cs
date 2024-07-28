// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins.IReleaseManagementProjectService
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B731B647-F193-4F67-AC15-FCB72E92BCAA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Project.Plugins
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.ReleaseManagement.Server.Services.ProjectService, Microsoft.VisualStudio.Services.ReleaseManagement2.Server")]
  public interface IReleaseManagementProjectService : IVssFrameworkService
  {
    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "1#", Justification = "Is of type string")]
    void DeleteProject(IVssRequestContext requestContext, string projectUri);
  }
}
