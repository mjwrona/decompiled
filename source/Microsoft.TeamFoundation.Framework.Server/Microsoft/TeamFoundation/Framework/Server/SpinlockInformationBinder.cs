// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.SpinlockInformationBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class SpinlockInformationBinder : ObjectBinder<SpinlockInformation>
  {
    private SqlColumnBinder m_name = new SqlColumnBinder("name");
    private SqlColumnBinder m_collisions = new SqlColumnBinder("collisions");
    private SqlColumnBinder m_spins = new SqlColumnBinder("spins");
    private SqlColumnBinder m_spinsPerCollision = new SqlColumnBinder("spins_per_collision");
    private SqlColumnBinder m_sleepTime = new SqlColumnBinder("sleep_time");
    private SqlColumnBinder m_backoffs = new SqlColumnBinder("backoffs");

    protected override SpinlockInformation Bind()
    {
      if (this.m_backoffs.GetObject((IDataReader) this.Reader).GetType() == typeof (int))
        return new SpinlockInformation()
        {
          Name = this.m_name.GetString((IDataReader) this.Reader, false),
          Collisions = this.m_collisions.GetInt64((IDataReader) this.Reader),
          Spins = this.m_spins.GetInt64((IDataReader) this.Reader),
          SpinsPerCollision = this.m_spinsPerCollision.GetFloat((IDataReader) this.Reader),
          SleepTime = this.m_sleepTime.GetInt64((IDataReader) this.Reader),
          Backoffs = (long) this.m_backoffs.GetInt32((IDataReader) this.Reader)
        };
      return new SpinlockInformation()
      {
        Name = this.m_name.GetString((IDataReader) this.Reader, false),
        Collisions = this.m_collisions.GetInt64((IDataReader) this.Reader),
        Spins = this.m_spins.GetInt64((IDataReader) this.Reader),
        SpinsPerCollision = this.m_spinsPerCollision.GetFloat((IDataReader) this.Reader),
        SleepTime = this.m_sleepTime.GetInt64((IDataReader) this.Reader),
        Backoffs = (long) (int) this.m_backoffs.GetInt64((IDataReader) this.Reader)
      };
    }
  }
}
