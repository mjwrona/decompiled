// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.WorkItemTypeExtensionUtils
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class WorkItemTypeExtensionUtils
  {
    public static WorkItemTypeExtension GetExtension(
      this IWorkItemTypeExtensionService service,
      IVssRequestContext requestContext,
      Guid id,
      bool ignoreCache = false)
    {
      return service.GetExtensions(requestContext, (IEnumerable<Guid>) new Guid[1]
      {
        id
      }, (ignoreCache ? 1 : 0) != 0).FirstOrDefault<WorkItemTypeExtension>();
    }
  }
}
