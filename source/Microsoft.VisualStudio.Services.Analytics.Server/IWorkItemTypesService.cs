// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.IWorkItemTypesService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics
{
  [DefaultServiceImplementation(typeof (WorkItemTypesService))]
  public interface IWorkItemTypesService : IVssFrameworkService
  {
    List<string> GetWorkItemTypesNotInTheHiddenCategory(
      IVssRequestContext requestContext,
      IList<Guid> projectIds);

    List<string> GetBugWorkItemTypes(IVssRequestContext requestContext, IList<Guid> projectIds);

    List<string> GetWorkItemTypesForBacklogs(
      IVssRequestContext requestContext,
      IList<Guid> projects,
      IList<string> backlogs);
  }
}
