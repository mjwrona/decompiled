// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataTierInfoBinder
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class DataTierInfoBinder : ObjectBinder<DataTierInfo>
  {
    protected SqlColumnBinder ConnectionString = new SqlColumnBinder(nameof (ConnectionString));
    protected SqlColumnBinder State = new SqlColumnBinder(nameof (State));
    protected SqlColumnBinder Tags = new SqlColumnBinder(nameof (Tags));

    internal DataTierInfoBinder()
    {
    }

    internal void Bind(out DataTierInfo result) => result = this.Bind();

    protected override DataTierInfo Bind()
    {
      Guid frameworkSigningKey = FrameworkServerConstants.FrameworkSigningKey;
      string connectionString = this.ConnectionString.GetString((IDataReader) this.Reader, false);
      DataTierState state = (DataTierState) Enum.ToObject(typeof (DataTierState), this.State.GetInt32((IDataReader) this.Reader));
      string tags = this.Tags.GetString((IDataReader) this.Reader, true);
      return new DataTierInfo(SqlConnectionInfoFactory.Create(connectionString), state, tags, frameworkSigningKey);
    }
  }
}
