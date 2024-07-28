// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildDetailDictionary
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class BuildDetailDictionary : Dictionary<string, BuildDetail>
  {
    public BuildDetailDictionary()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public BuildDetailDictionary(IEnumerable<BuildDetail> collection)
      : this()
    {
      foreach (BuildDetail buildDetail in collection)
        this[buildDetail.Uri] = buildDetail;
    }

    public void Add(BuildDetail item) => this[item.Uri] = item;
  }
}
