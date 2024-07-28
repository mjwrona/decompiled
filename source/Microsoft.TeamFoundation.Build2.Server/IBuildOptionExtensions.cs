// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IBuildOptionExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IBuildOptionExtensions
  {
    public static Guid GetId(this IBuildOption feature)
    {
      BuildOptionAttribute featureAttribute = feature.GetFeatureAttribute();
      return featureAttribute == null ? Guid.Empty : featureAttribute.Id;
    }

    public static int GetOrdinal(this IBuildOption feature)
    {
      BuildOptionAttribute featureAttribute = feature.GetFeatureAttribute();
      return featureAttribute == null ? int.MaxValue : featureAttribute.Ordinal;
    }

    private static BuildOptionAttribute GetFeatureAttribute(this IBuildOption feature) => ((IEnumerable<object>) feature.GetType().GetCustomAttributes(typeof (BuildOptionAttribute), false)).FirstOrDefault<object>() as BuildOptionAttribute;
  }
}
