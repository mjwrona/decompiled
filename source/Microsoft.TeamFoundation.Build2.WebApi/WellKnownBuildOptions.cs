// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.WellKnownBuildOptions
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using System;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [Obsolete("No longer used.")]
  public static class WellKnownBuildOptions
  {
    public static readonly Guid CreateDrop = Guid.Parse("{E8B30F6F-039D-4D34-969C-449BBE9C3B9E}");
    public static readonly Guid CopyToStagingFolder = Guid.Parse("{82F9A3E8-3930-482E-AC62-AE3276F284D5}");
  }
}
