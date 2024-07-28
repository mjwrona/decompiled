// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.RecentActivity.RecentActivityScope
// Assembly: Microsoft.Azure.Boards.RecentActivity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 684DCFA4-4764-4794-94A6-960AF811434C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.RecentActivity.dll

using System;

namespace Microsoft.Azure.Boards.RecentActivity
{
  [Flags]
  public enum RecentActivityScope
  {
    User = 1,
    Project = 2,
  }
}
