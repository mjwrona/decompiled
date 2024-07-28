// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.Capture
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Common
{
  public static class Capture
  {
    public static Capture<T> Create<T>(T value, Predicate<T> valueCheck = null) where T : struct => new Capture<T>(value, valueCheck);
  }
}
