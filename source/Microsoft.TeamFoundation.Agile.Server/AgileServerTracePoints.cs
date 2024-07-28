// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Agile.Server.AgileServerTracePoints
// Assembly: Microsoft.TeamFoundation.Agile.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B4912F51-3FCA-4D2B-A7B5-CF15E2F3B46B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Agile.Server.dll

namespace Microsoft.TeamFoundation.Agile.Server
{
  public static class AgileServerTracePoints
  {
    private const int Base = 6000000;
    private const int AgileServiceBase = 6000000;
    private const int AgileSettingBase = 6000100;
    private const int KanbanBase = 6000200;
    private const int ExceptionBase = 6000900;
    public const int AgileSettingsGetStart = 6000101;
    public const int AgileSettingsGetEnd = 6000102;
    public const int AgileSettingsGetTeam = 6000103;
    public const int ProvisionKanbanBoardDeleteColumn = 6000201;
    public const int ProvisionKanbanBoardDeleteRow = 6000202;
    public const int GetBacklogsStart = 6000001;
    public const int GetBacklogsEnd = 6000002;
    public const int GetBacklogsByProjectStart = 6000003;
    public const int GetBacklogsByProjectEnd = 6000004;
    public const int GetBacklogStart = 6000005;
    public const int GetBacklogEnd = 6000006;
    public const int DeleteBacklogStart = 6000007;
    public const int DeleteBacklogEnd = 6000008;
    public const int CreateBacklogStart = 6000009;
    public const int CreateBacklogEnd = 6000010;
    public const int UpdateBacklogStart = 6000011;
    public const int UpdateBacklogEnd = 6000012;
    public const int CreateBacklogFieldValuesStart = 6000013;
    public const int CreateBacklogFieldValuesEnd = 6000014;
    public const int CreateBacklogIterationPathsStart = 6000015;
    public const int CreateBacklogIterationPathsEnd = 6000016;
    public const int CreateBacklogMembersStart = 6000017;
    public const int CreateBacklogMembersEnd = 6000018;
    public const int CreateBacklogCapacityStart = 6000019;
    public const int CreateBacklogCapacityEnd = 6000020;
    public const int UpdateBacklogNameStart = 6000021;
    public const int UpdateBacklogNameEnd = 6000022;
    public const int CreateBacklogException = 6000901;
    public const int DeleteBacklogException = 6000902;
    public const int CreateBacklogIterationPathsException = 6000903;
    public const int CreateBacklogMembersException = 6000904;
    public const int CreateBacklogCapacityException = 6000905;
    public const int GetBacklogException = 6000906;
    public const int GetBacklogsException = 6000907;
    public const int UpdateBacklogNameException = 6000908;
  }
}
