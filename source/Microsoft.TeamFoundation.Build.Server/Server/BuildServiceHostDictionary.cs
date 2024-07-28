// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.BuildServiceHostDictionary
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build.Server
{
  internal class BuildServiceHostDictionary : Dictionary<string, BuildServiceHost>
  {
    public BuildServiceHostDictionary()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
    }

    public BuildServiceHostDictionary(IEnumerable<BuildServiceHost> collection)
      : this()
    {
      foreach (BuildServiceHost buildServiceHost in collection)
        this[buildServiceHost.Uri] = buildServiceHost;
    }
  }
}
