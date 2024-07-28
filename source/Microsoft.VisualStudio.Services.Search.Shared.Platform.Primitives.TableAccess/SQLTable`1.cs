// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.SQLTable`1
// Assembly: Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3998EAE-13E8-421A-93CB-363047218BB4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common.Utils;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.TableAccess
{
  public abstract class SQLTable<T> : TeamFoundationSqlResourceComponent, ITable<T>, IDisposable
  {
    protected static readonly SqlMetaData[] BigIntTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.BigInt)
    };
    protected static readonly SqlMetaData[] TinyIntTable = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.TinyInt)
    };

    public SQLTable(bool bindPartitionId = true)
    {
      if (!bindPartitionId)
        return;
      this.SelectedFeatures = SqlResourceComponentFeatures.BindPartitionIdByDefault;
    }

    protected SQLTable(string connectionString, int partitionId)
      : this()
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.PartitionId = partitionId;
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    protected SQLTable(string connectionString, int partitionId, IVssRequestContext requestContext)
      : this()
    {
      ISqlConnectionInfo connectionInfo = SqlConnectionInfoFactory.Create(connectionString);
      this.PartitionId = partitionId;
      this.Initialize(connectionInfo, 3600, 20, 1, 1, this.GetDefaultLogger(), (CircuitBreakerDatabaseProperties) null);
    }

    protected void ValidateNotNull<T1>(string propertyName, T1 value)
    {
      if ((object) value == null)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentNullException(propertyName));
    }

    protected void ValidateNotNullOrEmptyList<T1>(string propertyName, IList<T1> objectList)
    {
      if (objectList == null || objectList.Count == 0)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException(string.Format("{0} is null or empty", (object) propertyName)));
    }

    protected void ValidateNotNullOrEmptyString(string propertyName, string value)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException(string.Format("{0} is null or empty", (object) propertyName)));
    }

    protected void ValidateNotEmptyGuid(string propertyName, Guid value)
    {
      if (value == Guid.Empty)
        throw new TableAccessException(TableAcessErrorCodeEnum.INVALID_ARGUMENTS, (Exception) new ArgumentException(string.Format("{0} is empty", (object) propertyName)));
    }

    internal static string ToString(object obj, Type type) => Serializers.ToXmlString(obj, type);

    internal static string ToString(object obj, Type type, IEnumerable<Type> knownTypes) => Serializers.ToXmlString(obj, type, knownTypes);

    internal static string ToJsonString(object obj, Type type) => Serializers.ToJsonString(obj, type);

    internal static string ToJsonString(object obj, Type type, IEnumerable<Type> knownTypes) => Serializers.ToJsonString(obj, type, knownTypes);

    internal static object FromJsonString(string str, Type type) => Serializers.FromJsonString(str, type);

    internal static object FromJsonString(string str, Type type, IEnumerable<Type> knownTypes) => Serializers.FromJsonString(str, type, knownTypes);

    internal static Stream ToStream(object obj, Type type)
    {
      DataContractSerializer contractSerializer = new DataContractSerializer(type);
      MemoryStream stream = new MemoryStream();
      MemoryStream memoryStream = stream;
      object graph = obj;
      contractSerializer.WriteObject((Stream) memoryStream, graph);
      stream.Flush();
      stream.Position = 0L;
      return (Stream) stream;
    }

    internal static object FromString(string str, Type type, IEnumerable<Type> knownTypes) => Serializers.FromXmlString(str, type, knownTypes);

    internal static object FromString(string str, Type type) => Serializers.FromXmlString(str, type);

    public virtual T Insert(T entity) => throw new NotImplementedException();

    public virtual void Delete(T entity) => throw new NotImplementedException();

    public virtual T Update(T entity) => throw new NotImplementedException();

    public virtual int CheckAndUpdate(T oldEntity, T newEntity) => throw new NotImplementedException();

    public virtual T RetriveTableEntity(TableEntityFilterList filterList) => throw new NotImplementedException();

    public virtual List<T> AddTableEntityBatch(List<T> azurePlatformEntityList, bool merge) => throw new NotImplementedException();

    public virtual List<T> RetriveTableEntityList(int count, TableEntityFilterList filterList) => throw new NotImplementedException();

    protected SqlParameter BindParameterTable<DataType>(
      string parameterName,
      IEnumerable<DataType> rows,
      SqlDbType sqlDbType,
      string columnName)
    {
      rows = rows ?? Enumerable.Empty<DataType>();
      string typeName = "typ_" + sqlDbType.ToString() + "Table";
      SqlMetaData[] bindingTable = new SqlMetaData[1]
      {
        new SqlMetaData(columnName, sqlDbType)
      };
      System.Func<DataType, SqlDataRecord> selector = (System.Func<DataType, SqlDataRecord>) (value =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(bindingTable);
        this.SetParameter<DataType>(sqlDataRecord, value, sqlDbType);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, typeName, rows.Select<DataType, SqlDataRecord>(selector));
    }

    private void SetParameter<DataType>(
      SqlDataRecord sqlDataRecord,
      DataType value,
      SqlDbType sqlDbType)
    {
      if (sqlDbType != SqlDbType.TinyInt)
        throw new NotImplementedException(string.Format("Not implemented for {0}", (object) typeof (DataType)));
      sqlDataRecord.SetByte(0, (byte) (object) value);
    }
  }
}
