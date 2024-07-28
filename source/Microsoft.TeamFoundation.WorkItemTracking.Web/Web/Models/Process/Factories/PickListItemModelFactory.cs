// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.PickListItemModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class PickListItemModelFactory
  {
    public static IList<string> CreateList(IList<WorkItemPickListMember> members)
    {
      ArgumentUtility.CheckForNull<IList<WorkItemPickListMember>>(members, nameof (members));
      return (IList<string>) members.Select<WorkItemPickListMember, string>((Func<WorkItemPickListMember, string>) (item => item.Value)).ToList<string>();
    }
  }
}
