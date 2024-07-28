// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.SqlSerializer`1
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class SqlSerializer<T> : 
    IEnumerable<SqlDataRecord>,
    IEnumerable,
    IEnumerator<SqlDataRecord>,
    IDisposable,
    IEnumerator
  {
    private IEnumerable<T> m_collection;
    private IEnumerator<T> m_enumerator;
    private SqlDataRecord m_currentRecord;
    private SqlMetaData[] m_metadata;
    private MethodInfo[] m_getters;
    private string m_typeName;

    public SqlSerializer(IEnumerable<T> collection)
    {
      PropertyInfo[] properties = typeof (T).GetProperties();
      this.m_typeName = typeof (T).Name;
      this.m_metadata = new SqlMetaData[properties.Length];
      this.m_getters = new MethodInfo[properties.Length];
      object[] customAttributes = typeof (T).GetCustomAttributes(typeof (TeamFoundationMetadataAttribute), false);
      if (customAttributes != null && customAttributes.Length != 0)
        this.m_typeName = ((TeamFoundationMetadataAttribute) customAttributes[0]).TypeName;
      for (int index = 0; index < properties.Length; ++index)
      {
        PropertyInfo propertyInfo = properties[index];
        SqlDbType dbTypeFromClrType = this.GetSqlDbTypeFromClrType(propertyInfo.PropertyType);
        switch (dbTypeFromClrType)
        {
          case SqlDbType.NVarChar:
          case SqlDbType.VarBinary:
            this.m_metadata[index] = new SqlMetaData(propertyInfo.Name, dbTypeFromClrType, -1L);
            break;
          default:
            this.m_metadata[index] = new SqlMetaData(propertyInfo.Name, dbTypeFromClrType);
            break;
        }
        MethodInfo getMethod = propertyInfo.GetGetMethod();
        this.m_getters[index] = getMethod;
      }
      this.m_collection = collection;
      ICollection collection1 = collection as ICollection;
      if (collection != null && (collection1 == null || collection1.Count > 0))
        this.m_enumerator = collection.GetEnumerator();
      this.m_currentRecord = new SqlDataRecord(this.m_metadata);
    }

    public string TypeName => this.m_typeName;

    public IEnumerator<SqlDataRecord> GetEnumerator() => (IEnumerator<SqlDataRecord>) this;

    public bool IsNull => this.m_enumerator == null;

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    private SqlDbType GetSqlDbTypeFromClrType(Type t)
    {
      if (t == typeof (byte))
        return SqlDbType.TinyInt;
      if (t == typeof (short))
        return SqlDbType.SmallInt;
      if (t == typeof (int))
        return SqlDbType.Int;
      if (t == typeof (long))
        return SqlDbType.BigInt;
      if (t == typeof (string))
        return SqlDbType.NVarChar;
      if (t == typeof (Guid))
        return SqlDbType.UniqueIdentifier;
      if (t == typeof (DateTime))
        return SqlDbType.DateTime2;
      throw new NotImplementedException(string.Format("The Type {0} is not supported!", (object) t.Name));
    }

    public SqlDataRecord Current => this.m_currentRecord;

    public void Dispose()
    {
    }

    object IEnumerator.Current => (object) this.m_currentRecord;

    public bool MoveNext()
    {
      if (this.m_enumerator == null || !this.m_enumerator.MoveNext())
        return false;
      T current = this.m_enumerator.Current;
      for (int ordinal = 0; ordinal < this.m_metadata.Length; ++ordinal)
        this.m_currentRecord.SetValue(ordinal, this.m_getters[ordinal].Invoke((object) current, (object[]) null));
      return true;
    }

    public void Reset() => throw new NotImplementedException();
  }
}
