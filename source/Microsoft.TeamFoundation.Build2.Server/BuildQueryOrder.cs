// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildQueryOrder
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  public enum BuildQueryOrder
  {
    Ascending = 2,
    Descending = 3,
    QueueTimeDescending = 4,
    QueueTimeAscending = 5,
    StartTimeDescending = 6,
    StartTimeAscending = 7,
    FinishTimeDescending = 8,
    FinishTimeAscending = 9,
  }
}
