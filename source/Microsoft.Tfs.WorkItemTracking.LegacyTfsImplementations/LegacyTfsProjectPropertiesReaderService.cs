// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyTfsProjectPropertiesReaderService
// Assembly: Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6D9A1E77-52F6-4366-807D-D0FABA8CDE81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations
{
  public class LegacyTfsProjectPropertiesReaderService : 
    ILegacyProjectPropertiesReaderService,
    IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext requestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public IEnumerable<ProjectInfo> PopulateProperties(
      IEnumerable<ProjectInfo> projects,
      IVssRequestContext requestContext,
      params string[] filters)
    {
      return projects.PopulateProperties(requestContext, filters);
    }
  }
}
