// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.IInstalledFeatureService
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Admin
{
  internal interface IInstalledFeatureService
  {
    IEnumerable<IFeature> InstalledFeatures { get; }

    IFeature GetFeature(FeatureType type);

    IFeature TryGetFeature(FeatureType type);

    bool IsInstalled(FeatureType type);

    event EventHandler FeatureStateChanged;
  }
}
