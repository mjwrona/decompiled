// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.StringExtensions
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Common.Internal
{
  public static class StringExtensions
  {
    public static int GetStableHashCode(this string str) => !Environment.Is64BitProcess ? ClrHashUtil.GetStringHashOrcas32(str) : ClrHashUtil.GetStringHashOrcas64(str);
  }
}
