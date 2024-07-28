// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationTableValueParameter`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete]
  public abstract class TeamFoundationTableValueParameter<T> : 
    ITeamFoundationTableValueParameter<T>,
    IEnumerable<SqlDataRecord>,
    IEnumerable,
    IEnumerator<SqlDataRecord>,
    IDisposable,
    IEnumerator
  {
    protected SqlDataRecord m_record;
    protected IEnumerator<T> m_enumerator;
    protected IEnumerable<T> m_rows;
    protected bool m_hasMoreRows;
    private SqlMetaData[] m_metadata;
    protected bool m_omitNullEntries;

    public TeamFoundationTableValueParameter(
      IEnumerable<T> rows,
      string typeName,
      SqlMetaData[] metadata,
      bool omitNullEntries = false)
    {
      this.TypeName = typeName;
      this.m_rows = rows;
      this.m_hasMoreRows = false;
      this.m_metadata = metadata;
      this.m_omitNullEntries = omitNullEntries;
      if (this.m_rows == null)
        return;
      this.m_enumerator = this.m_rows.GetEnumerator();
      while (this.m_enumerator.MoveNext())
      {
        if (!this.m_omitNullEntries || !EqualityComparer<T>.Default.Equals(this.m_enumerator.Current, default (T)))
        {
          this.m_record = new SqlDataRecord(metadata);
          this.m_hasMoreRows = true;
          break;
        }
      }
    }

    public bool IsNullOrEmpty => !this.m_hasMoreRows;

    public string TypeName { get; private set; }

    public abstract void SetRecord(T t, SqlDataRecord record);

    public virtual IEnumerator<SqlDataRecord> GetEnumerator() => (IEnumerator<SqlDataRecord>) this;

    public SqlDataRecord Current
    {
      get
      {
        if (this.m_enumerator == null)
          throw new InvalidOperationException("Enumerator is disposed or not positioned on a row");
        return this.m_record;
      }
    }

    public void Dispose()
    {
      if (this.m_enumerator == null)
        return;
      this.m_enumerator.Dispose();
      this.m_enumerator = (IEnumerator<T>) null;
      this.m_hasMoreRows = false;
    }

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    object IEnumerator.Current => throw new NotImplementedException();

    public bool MoveNext()
    {
      if (!this.m_hasMoreRows)
      {
        this.Dispose();
        return false;
      }
      bool hasMoreRows = this.m_hasMoreRows;
      this.m_record = new SqlDataRecord(this.m_metadata);
      this.SetRecord(this.m_enumerator.Current, this.m_record);
      this.m_hasMoreRows = false;
      while (this.m_enumerator.MoveNext())
      {
        if (!this.m_omitNullEntries || !EqualityComparer<T>.Default.Equals(this.m_enumerator.Current, default (T)))
        {
          this.m_hasMoreRows = true;
          break;
        }
      }
      return hasMoreRows;
    }

    public virtual void Reset()
    {
      this.Dispose();
      this.m_enumerator = this.m_rows.GetEnumerator();
      this.m_enumerator.Reset();
      this.m_hasMoreRows = this.m_enumerator.MoveNext();
    }

    public void SetString(
      SqlDataRecord record,
      int ordinal,
      string value,
      BindStringBehavior bindBehavior)
    {
      record.SetString(ordinal, value, bindBehavior);
    }

    public void SetNullableString(SqlDataRecord record, int ordinal, string value) => record.SetNullableString(ordinal, value);

    public void SetNullableStringAsEmpty(SqlDataRecord record, int ordinal, string value) => record.SetNullableStringAsEmpty(ordinal, value);

    public void SetNullableGuid(SqlDataRecord record, int ordinal, Guid value) => record.SetNullableGuid(ordinal, value);

    public void SetNullableDateTime(SqlDataRecord record, int ordinal, DateTime value) => record.SetNullableDateTime(ordinal, value);

    public void SetNullableInt32(SqlDataRecord record, int ordinal, int? value) => record.SetNullableInt32(ordinal, value);

    public void SetNullableBinary(SqlDataRecord record, int ordinal, byte[] value) => record.SetNullableBinary(ordinal, value);

    public void SetTimespanAsMicroseconds(SqlDataRecord record, int ordinal, TimeSpan value) => record.SetTimespanAsMicroseconds(ordinal, value);
  }
}
