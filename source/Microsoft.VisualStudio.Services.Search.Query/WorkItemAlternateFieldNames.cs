// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.WorkItemAlternateFieldNames
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Query
{
  public static class WorkItemAlternateFieldNames
  {
    private static IReadOnlyDictionary<string, ISet<string>> s_alternateFieldNames = (IReadOnlyDictionary<string, ISet<string>>) new Dictionary<string, ISet<string>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      ["System.Assignedto"] = (ISet<string>) new HashSet<string>()
      {
        "a",
        "Owner"
      },
      ["System.Createdby"] = (ISet<string>) new HashSet<string>()
      {
        "c"
      },
      ["System.State"] = (ISet<string>) new HashSet<string>()
      {
        "s"
      },
      ["System.WorkItemType"] = (ISet<string>) new HashSet<string>()
      {
        "t"
      },
      ["System.AreaPath"] = (ISet<string>) new HashSet<string>()
      {
        "Area"
      },
      ["System.IterationPath"] = (ISet<string>) new HashSet<string>()
      {
        "Iteration"
      },
      ["System.History"] = (ISet<string>) new HashSet<string>()
      {
        "Discussion"
      },
      ["System.TeamProject"] = (ISet<string>) new HashSet<string>()
      {
        "Project",
        "p",
        "proj"
      }
    };

    public static IReadOnlyDictionary<string, ISet<string>> AlternateFieldNames => WorkItemAlternateFieldNames.s_alternateFieldNames;
  }
}
