// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Agile.Service.IRequestedTeamIterationService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Agile, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 577172B7-1034-4DD0-9CB1-238BFF966AC0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Agile.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;

namespace Microsoft.TeamFoundation.Server.WebAccess.Agile.Service
{
  public interface IRequestedTeamIterationService
  {
    TreeNode GetRequestedIterationNode(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      bool throwIfNotFound = true,
      bool throwIfTeamNotSubscribed = false);

    TreeNode GetRequestedIterationNode(
      IVssRequestContext requestContext,
      IAgileSettings settings,
      string pathOrId,
      bool throwIfNotFound = true,
      bool throwIfTeamNotSubscribed = false);

    string GetRequestedIterationRouteValue(IVssRequestContext requestContext);
  }
}
