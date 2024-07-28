// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.IAnalyticsSecurityService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [DefaultServiceImplementation(typeof (AnalyticsSecurityService))]
  public interface IAnalyticsSecurityService : IVssFrameworkService
  {
    ICollection<ProjectInfo> GetAccessibleProjects(IVssRequestContext requestContext);

    bool HasReadPermission(IVssRequestContext requestContext, Guid projectId);

    bool HasExecuteUnrestrictedQueryPermission(IVssRequestContext requestContext);

    bool HasStagingPermission(IVssRequestContext requestContext);

    bool HasReadEuiiPermission(IVssRequestContext requestContext);
  }
}
