// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache.IWorkItemFieldCacheService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 79AB9CC0-954C-4F8E-A45A-BE8292FA9E70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache
{
  [DefaultServiceImplementation(typeof (WorkItemFieldCacheService))]
  public interface IWorkItemFieldCacheService : IVssFrameworkService
  {
    bool TryGetFieldByRefName(
      IVssRequestContext requestContext,
      string fieldRefName,
      out WorkItemField fieldData,
      bool forceUpdate = false);

    bool TryGetFieldByName(
      IVssRequestContext requestContext,
      string fieldName,
      out IEnumerable<WorkItemField> fieldsData);

    IEnumerable<WorkItemField> GetFieldsList(IVssRequestContext requestContext);

    HashSet<string> GetIdentityFieldsList(IVssRequestContext requestContext);
  }
}
