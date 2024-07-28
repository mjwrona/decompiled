// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TuningRecommendationBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class TuningRecommendationBinder : ObjectBinder<TuningRecommendation>
  {
    private SqlColumnBinder m_name = new SqlColumnBinder("name");
    private SqlColumnBinder m_type = new SqlColumnBinder("type");
    private SqlColumnBinder m_reason = new SqlColumnBinder("reason");
    private SqlColumnBinder m_validSince = new SqlColumnBinder("valid_since");
    private SqlColumnBinder m_lastRefresh = new SqlColumnBinder("last_refresh");
    private SqlColumnBinder m_state = new SqlColumnBinder("state");
    private SqlColumnBinder m_isExecutableAction = new SqlColumnBinder("is_executable_action");
    private SqlColumnBinder m_isRevertableAction = new SqlColumnBinder("is_revertable_action");
    private SqlColumnBinder m_executeActionStartTime = new SqlColumnBinder("execute_action_start_time");
    private SqlColumnBinder m_executeActionDuration = new SqlColumnBinder("execute_action_duration");
    private SqlColumnBinder m_executeActionInitiatedBy = new SqlColumnBinder("execute_action_initiated_by");
    private SqlColumnBinder m_executeActionInitiatedTime = new SqlColumnBinder("execute_action_initiated_time");
    private SqlColumnBinder m_revertActionStartTime = new SqlColumnBinder("revert_action_start_time");
    private SqlColumnBinder m_revertActionDuration = new SqlColumnBinder("revert_action_duration");
    private SqlColumnBinder m_revertActionInitiatedBy = new SqlColumnBinder("revert_action_initiated_by");
    private SqlColumnBinder m_revertActionInitatedTime = new SqlColumnBinder("revert_action_initiated_time");
    private SqlColumnBinder m_score = new SqlColumnBinder("score");
    private SqlColumnBinder m_details = new SqlColumnBinder("details");

    private static DateTime Max(DateTime t1, DateTime t2) => !(t1 > t2) ? t2 : t1;

    protected override TuningRecommendation Bind()
    {
      DateTime t2 = new DateTime(1901, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      return new TuningRecommendation()
      {
        Name = this.m_name.GetString((IDataReader) this.Reader, false),
        Type = this.m_type.GetString((IDataReader) this.Reader, false),
        Reason = this.m_reason.GetString((IDataReader) this.Reader, true),
        ValidSince = this.m_validSince.GetDateTime((IDataReader) this.Reader),
        LastRefresh = this.m_lastRefresh.GetDateTime((IDataReader) this.Reader),
        State = this.m_state.GetString((IDataReader) this.Reader, true),
        IsExecutableAction = this.m_isExecutableAction.GetBoolean((IDataReader) this.Reader),
        IsRevertableAction = this.m_isRevertableAction.GetBoolean((IDataReader) this.Reader),
        ExecuteAction = new TuningAction()
        {
          StartTime = TuningRecommendationBinder.Max(this.m_executeActionStartTime.GetDateTime((IDataReader) this.Reader), t2),
          Duration = this.m_executeActionDuration.GetTimeSpan(this.Reader, TimeSpan.Zero),
          InitiatedBy = this.m_executeActionInitiatedBy.GetString((IDataReader) this.Reader, true),
          InitiatedTime = TuningRecommendationBinder.Max(this.m_executeActionInitiatedTime.GetDateTime((IDataReader) this.Reader), t2)
        },
        RevertAction = new TuningAction()
        {
          StartTime = TuningRecommendationBinder.Max(this.m_revertActionStartTime.GetDateTime((IDataReader) this.Reader), t2),
          Duration = this.m_revertActionDuration.GetTimeSpan(this.Reader, TimeSpan.Zero),
          InitiatedBy = this.m_revertActionInitiatedBy.GetString((IDataReader) this.Reader, true),
          InitiatedTime = TuningRecommendationBinder.Max(this.m_revertActionInitatedTime.GetDateTime((IDataReader) this.Reader), t2)
        },
        Score = this.m_score.GetInt32((IDataReader) this.Reader),
        Details = this.m_details.GetString((IDataReader) this.Reader, false)
      };
    }
  }
}
