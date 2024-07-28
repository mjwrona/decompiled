// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.RuntimeEnvironmentFlags
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;

namespace Microsoft.TeamFoundation.Client
{
  [Flags]
  public enum RuntimeEnvironmentFlags
  {
    None = 0,
    Windows = 1,
    Vsip = 2,
    Console = 4,
    Office = 8,
  }
}
