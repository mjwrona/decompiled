// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess.CounterVariableBinder
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess
{
  internal sealed class CounterVariableBinder : ObjectBinder<CounterVariable>
  {
    private SqlColumnBinder m_prefix = new SqlColumnBinder("Prefix");
    private SqlColumnBinder m_seed = new SqlColumnBinder("Seed");
    private SqlColumnBinder m_value = new SqlColumnBinder("Value");

    protected override CounterVariable Bind()
    {
      string prefix = this.m_prefix.GetString((IDataReader) this.Reader, true);
      int int32_1 = this.m_seed.GetInt32((IDataReader) this.Reader);
      int int32_2 = this.m_value.GetInt32((IDataReader) this.Reader);
      int seed = int32_1;
      int num = int32_2;
      return new CounterVariable(prefix, seed, num);
    }
  }
}
