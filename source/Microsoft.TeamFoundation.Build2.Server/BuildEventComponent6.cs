// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildEventComponent6
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal class BuildEventComponent6 : BuildEventComponent5
  {
    public override long GetBuildEventQueueData()
    {
      this.TraceEnter(0, nameof (GetBuildEventQueueData));
      this.PrepareStoredProcedure("Build.prc_GetBuildEventQueueData");
      long buildEventQueueData = (long) this.ExecuteScalar();
      this.TraceLeave(0, nameof (GetBuildEventQueueData));
      return buildEventQueueData;
    }
  }
}
