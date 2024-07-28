// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.KeyVault.UriExtensions
// Assembly: Microsoft.Azure.KeyVault, Version=3.0.5.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 594DACFC-3846-4701-8E31-E06E75D35FE9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.KeyVault.dll

using System;

namespace Microsoft.Azure.KeyVault
{
  internal static class UriExtensions
  {
    public static string FullAuthority(this Uri uri)
    {
      string str = uri.Authority;
      if (!str.Contains(":") && uri.Port > 0)
        str = string.Format("{0}:{1}", (object) uri.Authority, (object) uri.Port.ToString());
      return str;
    }
  }
}
