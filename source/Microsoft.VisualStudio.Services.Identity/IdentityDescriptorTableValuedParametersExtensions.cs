// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityDescriptorTableValuedParametersExtensions
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityDescriptorTableValuedParametersExtensions
  {
    private static readonly SqlMetaData[] typ_DescriptorTable2 = new SqlMetaData[3]
    {
      new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
      new SqlMetaData("Type", SqlDbType.VarChar, 64L),
      new SqlMetaData("OrderId", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_DescriptorTable3 = new SqlMetaData[3]
    {
      new SqlMetaData("Sid", SqlDbType.VarChar, 256L),
      new SqlMetaData("TypeId", SqlDbType.TinyInt),
      new SqlMetaData("OrderId", SqlDbType.Int)
    };

    public static SqlParameter BindOrderedDescriptorTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityDescriptor> rows,
      bool omitNullEntries = false)
    {
      rows = rows ?? Enumerable.Empty<IdentityDescriptor>();
      Func<IdentityDescriptor, int, IEnumerable<SqlDataRecord>> selector = (Func<IdentityDescriptor, int, IEnumerable<SqlDataRecord>>) ((descriptor, index) =>
      {
        if (omitNullEntries && descriptor == (IdentityDescriptor) null)
          return Enumerable.Empty<SqlDataRecord>();
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IdentityDescriptorTableValuedParametersExtensions.typ_DescriptorTable2);
        sqlDataRecord.SetString(0, descriptor.Identifier);
        sqlDataRecord.SetString(1, descriptor.IdentityType);
        sqlDataRecord.SetInt32(2, index);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          sqlDataRecord
        };
      });
      return component.BindTable(parameterName, "typ_DescriptorTable2", rows.SelectMany<IdentityDescriptor, SqlDataRecord>(selector));
    }

    public static SqlParameter BindOrderedDescriptorTable2ForIdentityComponent16OrLater(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityDescriptor> rows,
      bool omitNullEntries = false)
    {
      rows = rows ?? Enumerable.Empty<IdentityDescriptor>();
      Func<IdentityDescriptor, int, IEnumerable<SqlDataRecord>> selector = (Func<IdentityDescriptor, int, IEnumerable<SqlDataRecord>>) ((descriptor, index) =>
      {
        if (omitNullEntries && descriptor == (IdentityDescriptor) null)
          return Enumerable.Empty<SqlDataRecord>();
        SqlDataRecord sqlDataRecord = new SqlDataRecord(IdentityDescriptorTableValuedParametersExtensions.typ_DescriptorTable3);
        sqlDataRecord.SetString(0, descriptor.Identifier);
        sqlDataRecord.SetByte(1, IdentityTypeMapper.Instance.GetTypeIdFromName(descriptor.IdentityType));
        sqlDataRecord.SetInt32(2, index);
        return (IEnumerable<SqlDataRecord>) new SqlDataRecord[1]
        {
          sqlDataRecord
        };
      });
      return component.BindTable(parameterName, "typ_DescriptorTable3", rows.SelectMany<IdentityDescriptor, SqlDataRecord>(selector));
    }
  }
}
