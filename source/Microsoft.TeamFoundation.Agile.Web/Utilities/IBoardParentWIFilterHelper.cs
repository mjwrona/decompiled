// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Web.Utilities.IBoardParentWIFilterHelper
// Assembly: Microsoft.TeamFoundation.Agile.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDD05019-96D4-4BDC-9BC2-86F688BB0A99
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Web.dll

using Microsoft.TeamFoundation.Agile.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Agile.Web.Utilities
{
  public interface IBoardParentWIFilterHelper
  {
    ICollection<ParentChildWIMap> GetParentChildWIMap(
      IVssRequestContext requestContext,
      IAgileSettings teamAgileSettings,
      BacklogLevelConfiguration parentBacklogLevel,
      int[] workitemIds);
  }
}
