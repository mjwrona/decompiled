// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Admin.FeatureType
// Assembly: Microsoft.TeamFoundation.Admin.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4DC7473-FE52-49C1-BB5D-1E769BB5001D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Admin.Common.dll

using System;

namespace Microsoft.TeamFoundation.Admin
{
  [Flags]
  public enum FeatureType
  {
    None = 0,
    ApplicationTier = 1,
    VersionControlProxy = 4,
    Tools = 8,
    ObjectModel = 64, // 0x00000040
    Search = 256, // 0x00000100
  }
}
