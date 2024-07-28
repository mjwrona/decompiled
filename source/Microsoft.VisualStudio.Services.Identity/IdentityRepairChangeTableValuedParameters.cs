// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityRepairChangeTableValuedParameters
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class IdentityRepairChangeTableValuedParameters
  {
    private static readonly SqlMetaData[] typ_IdentityRepairChangeTable = new SqlMetaData[13]
    {
      new SqlMetaData("RequiredById", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ChangedId", SqlDbType.UniqueIdentifier),
      new SqlMetaData("ChangedDate", SqlDbType.DateTime),
      new SqlMetaData("PreviousTypeId", SqlDbType.TinyInt),
      new SqlMetaData("UpdatedTypeId", SqlDbType.TinyInt),
      new SqlMetaData("PreviousSid", SqlDbType.VarChar, 256L),
      new SqlMetaData("UpdatedSid", SqlDbType.VarChar, 256L),
      new SqlMetaData("PreviousUpn", SqlDbType.NVarChar, 256L),
      new SqlMetaData("UpdatedUpn", SqlDbType.NVarChar, 256L),
      new SqlMetaData("PreviousOid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("UpdatedOid", SqlDbType.UniqueIdentifier),
      new SqlMetaData("PreviousPuid", SqlDbType.VarChar, 100L),
      new SqlMetaData("UpdatedPuid", SqlDbType.VarChar, 100L)
    };

    public static SqlParameter BindIdentityRepairChangeTable(
      this TeamFoundationSqlResourceComponent component,
      string parameterName,
      IEnumerable<IdentityRepairChange> rows)
    {
      return component.Bind<IdentityRepairChange>(parameterName, rows, IdentityRepairChangeTableValuedParameters.typ_IdentityRepairChangeTable, "typ_IdentityRepairChangeTable", (Action<SqlDataRecord, IdentityRepairChange>) ((record, repairChange) =>
      {
        // ISSUE: reference to a compiler-generated field
        if (IdentityRepairChangeTableValuedParameters.\u003C\u003Eo__1.\u003C\u003Ep__0 == null)
        {
          // ISSUE: reference to a compiler-generated field
          IdentityRepairChangeTableValuedParameters.\u003C\u003Eo__1.\u003C\u003Ep__0 = CallSite<System.Action<CallSite, Type, SqlDataRecord, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.ResultDiscarded, "BindIdentityRepairChange", (IEnumerable<Type>) null, typeof (IdentityRepairChangeTableValuedParameters), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[3]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType | CSharpArgumentInfoFlags.IsStaticType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        IdentityRepairChangeTableValuedParameters.\u003C\u003Eo__1.\u003C\u003Ep__0.Target((CallSite) IdentityRepairChangeTableValuedParameters.\u003C\u003Eo__1.\u003C\u003Ep__0, typeof (IdentityRepairChangeTableValuedParameters), record, (object) repairChange);
      }));
    }

    private static void BindIdentityRepairChangeBaseProperties(
      SqlDataRecord record,
      IdentityRepairChange repairChange)
    {
      record.SetGuid(0, repairChange.RequiredById);
      record.SetGuid(1, repairChange.ChangedId);
      record.SetDateTime(2, repairChange.ChangedDate);
      record.SetNullableByte(3, repairChange.PreviousTypeId);
      record.SetNullableByte(4, repairChange.UpdatedTypeId);
      record.SetNullableString(5, repairChange.PreviousIdentifier);
      record.SetNullableString(6, repairChange.UpdatedIdentifier);
    }

    private static void BindIdentityRepairChange(
      SqlDataRecord record,
      IdentityUpnRepairChange upnRepairChange)
    {
      IdentityRepairChangeTableValuedParameters.BindIdentityRepairChangeBaseProperties(record, (IdentityRepairChange) upnRepairChange);
      record.SetNullableString(7, upnRepairChange.PreviousUpn);
      record.SetNullableString(8, upnRepairChange.UpdatedUpn);
    }

    private static void BindIdentityRepairChange(
      SqlDataRecord record,
      IdentityOidRepairChange oidRepairChange)
    {
      IdentityRepairChangeTableValuedParameters.BindIdentityRepairChangeBaseProperties(record, (IdentityRepairChange) oidRepairChange);
      record.SetNullableGuid(9, oidRepairChange.PreviousOid);
      record.SetNullableGuid(10, oidRepairChange.UpdatedOid);
    }

    private static void BindIdentityRepairChange(
      SqlDataRecord record,
      IdentityPuidRepairChange puidRepairChange)
    {
      IdentityRepairChangeTableValuedParameters.BindIdentityRepairChangeBaseProperties(record, (IdentityRepairChange) puidRepairChange);
      record.SetNullableString(11, puidRepairChange.PreviousPuid);
      record.SetNullableString(12, puidRepairChange.UpdatedPuid);
    }

    private static void BindIdentityRepairChange(
      SqlDataRecord record,
      IdentityRepairChange repairChange)
    {
      throw new NotImplementedException();
    }

    private static byte? TypeId(this IdentityDescriptor descriptor) => new byte?(IdentityTypeMapper.Instance.GetTypeIdFromName(descriptor.IdentityType));
  }
}
