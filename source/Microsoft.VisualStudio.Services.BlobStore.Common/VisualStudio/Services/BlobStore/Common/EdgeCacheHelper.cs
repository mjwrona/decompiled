// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.EdgeCacheHelper
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class EdgeCacheHelper
  {
    public static EdgeCache GetEdgeCacheEnvVar(string envVarPrefix)
    {
      string environmentVariable = Environment.GetEnvironmentVariable(envVarPrefix + "AllowEdge");
      int result;
      return string.IsNullOrWhiteSpace(environmentVariable) || int.TryParse(environmentVariable, out result) && result == 1 ? EdgeCache.Allowed : EdgeCache.NotAllowed;
    }
  }
}
