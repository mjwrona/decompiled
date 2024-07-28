// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.JobConstants
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using System;

namespace Microsoft.TeamFoundation.CodeSense.Server.Jobs
{
  public static class JobConstants
  {
    public static readonly Guid KeepupJobId = new Guid("978F0190-5520-4B23-A316-D19561573A36");
    public static readonly Guid CatchupJobId = new Guid("7397E059-09C0-475A-A373-A3F86530E57D");
    public static readonly Guid AggregatorJobId = new Guid("93D4DB79-9357-4B47-BF5A-ED4139FC87B5");
    public static readonly Guid CleanupJobId = new Guid("1501E05F-0130-4874-9760-03E54BF2927D");
    public static readonly Guid FrameworkCleanupJobId = new Guid("7A3E559E-8EB7-4E90-A4F7-B7A2515D52B9");
    public static readonly Guid MopupJobId = new Guid("0DAFF3C8-AB4F-43FA-8505-2D790A473605");
    public static readonly Guid FLICatchupJobId = new Guid("11C81B0D-2944-434B-9B68-CA762FC03154");
  }
}
