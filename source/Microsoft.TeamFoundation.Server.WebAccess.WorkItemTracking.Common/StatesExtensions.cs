// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.StatesExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  public static class StatesExtensions
  {
    public static IEnumerable<string> GetAllStateNames(this IEnumerable<State> states) => states.Select<State, string>((Func<State, string>) (s => s.Value));

    public static IEnumerable<string> GetFilteredStateNames(
      this IEnumerable<State> states,
      params StateTypeEnum[] typeFilter)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) typeFilter, nameof (typeFilter));
      return states.Where<State>((Func<State, bool>) (stateInfo => ((IEnumerable<StateTypeEnum>) typeFilter).Contains<StateTypeEnum>(stateInfo.Type))).Select<State, string>((Func<State, string>) (stateInfo => stateInfo.Value));
    }
  }
}
