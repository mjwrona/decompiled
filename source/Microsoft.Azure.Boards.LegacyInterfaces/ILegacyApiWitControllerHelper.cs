// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces.ILegacyApiWitControllerHelper
// Assembly: Microsoft.Azure.Boards.LegacyInterfaces, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C0E0C41-D39C-453E-A6CF-32A7C57153EE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.LegacyInterfaces.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.LegacyInterfaces
{
  [DefaultServiceImplementation("Microsoft.TeamFoundation.WorkItemTracking.LegacyTfsImplementations.LegacyApiWitControllerHelper, Microsoft.Tfs.WorkItemTracking.LegacyTfsImplementations")]
  public interface ILegacyApiWitControllerHelper : IVssFrameworkService
  {
    JsObject GetTeamProjectsInternal(
      IVssRequestContext requestContext,
      IEnumerable<object> projects,
      bool includeFieldDefinition = false,
      bool includeProcessInfo = true,
      bool includeWorkItemTypes = true);

    Microsoft.VisualStudio.Services.Identity.Identity[] ResolveAllIdentities(
      IVssRequestContext requestContext,
      Guid[] tfIds,
      Guid[] originIds,
      bool disallowInvitedUsers);
  }
}
