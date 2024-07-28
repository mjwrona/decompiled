// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.JobPriorityLevel
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

namespace Microsoft.TeamFoundation.Framework.Common
{
  public enum JobPriorityLevel
  {
    None = 0,
    Idle = 1,
    Lowest = 2,
    BelowNormal = 3,
    Normal = 4,
    Highest = 6,
  }
}
