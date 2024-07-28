// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingTableValueParameter`1
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class WorkItemTrackingTableValueParameter<T> : 
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

    public WorkItemTrackingTableValueParameter(
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

    public void SetNullableBinary(SqlDataRecord record, int ordinal, byte[] value)
    {
      if (value == null)
        record.SetDBNull(ordinal);
      else
        record.SetBytes(ordinal, 0L, value, 0, value.Length);
    }

    public void SetString(
      SqlDataRecord record,
      int ordinal,
      string value,
      BindStringBehavior bindBehavior)
    {
      throw new NotImplementedException();
    }

    public void SetNullableString(SqlDataRecord record, int ordinal, string value) => throw new NotImplementedException();

    public void SetNullableStringAsEmpty(SqlDataRecord record, int ordinal, string value) => throw new NotImplementedException();

    public void SetNullableGuid(SqlDataRecord record, int ordinal, Guid value) => throw new NotImplementedException();

    public void SetNullableDateTime(SqlDataRecord record, int ordinal, DateTime value) => throw new NotImplementedException();

    public void SetNullableInt32(SqlDataRecord record, int ordinal, int? value) => throw new NotImplementedException();

    public void SetTimespanAsMicroseconds(SqlDataRecord record, int ordinal, TimeSpan value) => throw new NotImplementedException();
  }
}
