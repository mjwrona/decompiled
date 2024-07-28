// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LobParameterComponent
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class LobParameterComponent : TeamFoundationSqlResourceComponent
  {
    protected int m_parameterId;
    private static Dictionary<int, SqlExceptionFactory> s_sqlExceptionFactories = new Dictionary<int, SqlExceptionFactory>();

    static LobParameterComponent() => LobParameterComponent.s_sqlExceptionFactories.Add(800058, new SqlExceptionFactory(typeof (InvalidLobParameterException)));

    internal void Append(string value)
    {
      this.PrepareStoredProcedure("prc_UpdateLobParameter");
      this.BindString("@token", this.Token, -1, true, SqlDbType.NVarChar);
      SqlParameter sqlParameter = this.BindInt("@parameterId", this.m_parameterId);
      sqlParameter.Direction = ParameterDirection.InputOutput;
      this.BindXml("@parameterValue", value);
      this.ExecuteNonQuery();
      this.m_parameterId = (int) sqlParameter.Value;
    }

    internal string Token { get; set; }

    internal Stream GetLobParameter(int parameterId)
    {
      this.PrepareStoredProcedure("prc_GetLobParameter");
      this.BindInt("@parameterId", parameterId);
      SqlDataReader reader = this.ExecuteReader(CommandBehavior.SequentialAccess);
      return reader.Read() ? (Stream) new LobParameterComponent.LobStream(reader) : (Stream) null;
    }

    internal int ParameterId
    {
      get => this.m_parameterId;
      set => this.m_parameterId = value;
    }

    protected override IDictionary<int, SqlExceptionFactory> TranslatedExceptions => (IDictionary<int, SqlExceptionFactory>) LobParameterComponent.s_sqlExceptionFactories;

    private sealed class LobStream : Stream
    {
      private long m_currentIndex;
      private SqlDataReader m_dataReader;

      internal LobStream(SqlDataReader reader) => this.m_dataReader = reader;

      public override bool CanRead => true;

      public override bool CanSeek => false;

      public override bool CanWrite => false;

      public override int Read(byte[] buffer, int offset, int count)
      {
        long bytes = this.m_dataReader.GetBytes(0, this.m_currentIndex, buffer, offset, count);
        this.m_currentIndex += bytes;
        return (int) bytes;
      }

      public override void Flush() => throw new NotImplementedException();

      public override long Length => throw new NotImplementedException();

      public override long Position
      {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
      }

      public override long Seek(long offset, SeekOrigin origin) => throw new NotImplementedException();

      public override void SetLength(long value) => throw new NotImplementedException();

      public override void Write(byte[] buffer, int offset, int count) => throw new NotImplementedException();
    }
  }
}
