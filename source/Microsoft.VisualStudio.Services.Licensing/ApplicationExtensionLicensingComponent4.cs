// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Licensing.ApplicationExtensionLicensingComponent4
// Assembly: Microsoft.VisualStudio.Services.Licensing, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1B3CC7E6-129F-4267-B8E5-2210320176C7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Licensing.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;

namespace Microsoft.VisualStudio.Services.Licensing
{
  internal class ApplicationExtensionLicensingComponent4 : ApplicationExtensionLicensingComponent3
  {
    private const string c_createTempUserExtensionLicenseTable = "stmt_CreateTempUserExtensionLicenseTable.sql";
    private const string c_insertTempUserExtensionLicense = "stmt_InsertTempUserExtensionLicense.sql";
    private const string c_readPartitionStatementTemplate = "SELECT {2} \nFROM   {0}.{1} WITH(NOLOCK) \nWHERE  PartitionId = @partitionId \nAND    CollectionId = @collectionId";

    public override void AssignExtensionLicenseToUserBatch(
      Guid scopeId,
      IEnumerable<Guid> userIds,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034141, nameof (AssignExtensionLicenseToUserBatch));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicenseBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuidTable(nameof (userIds), userIds);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.BindGuid("@collectionId", collectionId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034148, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034149, nameof (AssignExtensionLicenseToUserBatch));
      }
    }

    public override int UpdateExtensionsAssignedToUserBatchWithCount(
      Guid scopeId,
      Guid userId,
      IEnumerable<string> extensionIds,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034181, nameof (UpdateExtensionsAssignedToUserBatchWithCount));
        this.PrepareStoredProcedure("prc_UpdateExtensionsAssignedToUserBatch");
        this.BindGuid("@scopeId", scopeId);
        this.BindGuid("@userId", userId);
        this.BindStringTable("@extensionIds", extensionIds);
        this.BindByte("@source", (byte) source);
        this.BindGuid("@collectionId", collectionId);
        return (int) this.ExecuteScalar();
      }
      catch (Exception ex)
      {
        this.TraceException(1034188, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034189, nameof (UpdateExtensionsAssignedToUserBatchWithCount));
      }
    }

    internal override void AssignExtensionLicenseToUser(
      Guid scopeId,
      Guid userId,
      string extensionId,
      LicensingSource source,
      AssignmentSource assignmentSource,
      Guid collectionId)
    {
      try
      {
        this.TraceEnter(1034111, nameof (AssignExtensionLicenseToUser));
        this.PrepareStoredProcedure("prc_UpsertUserExtensionLicense");
        this.BindGuid("@userId", userId);
        this.BindGuid("@scopeId", scopeId);
        this.BindString("@extensionId", extensionId, 200, false, SqlDbType.NVarChar);
        this.BindByte("@source", (byte) source);
        this.BindGuid("@collectionId", collectionId);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.TraceException(1034118, ex);
        throw;
      }
      finally
      {
        this.TraceLeave(1034119, nameof (AssignExtensionLicenseToUser));
      }
    }

    internal void DeleteUserExtensionLicenseByCollection(Guid collectionId)
    {
      this.PrepareStoredProcedure("prc_DeleteUserExtensionLicenseByCollection");
      this.BindGuid("@collectionId", collectionId);
      this.ExecuteNonQuery();
    }

    internal void CreateTempUserExtensionLicenseTable()
    {
      this.Logger.Info("Create temp user extension license table for attach");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_CreateTempUserExtensionLicenseTable.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    internal void InsertTempUserExtensionLicense()
    {
      this.Logger.Info("Insert temp user extension license table to tbl_UserExtensionLicense");
      string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_InsertTempUserExtensionLicense.sql");
      this.PrepareSqlBatch(resourceAsString.Length);
      this.AddStatement(resourceAsString);
      this.ExecuteNonQuery();
    }

    public IList<UserExtensionLicense> ReadPartitionWithCollectionId(
      Table table,
      int targetPartitionId,
      Guid collectionId)
    {
      Dictionary<string, string> fieldExpressions = new Dictionary<string, string>()
      {
        {
          "PartitionId",
          targetPartitionId.ToString()
        }
      };
      string columnList = table.GetColumnList("", true, false, fieldExpressions);
      string sqlStatement = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SELECT {2} \nFROM   {0}.{1} WITH(NOLOCK) \nWHERE  PartitionId = @partitionId \nAND    CollectionId = @collectionId", (object) StringUtil.QuoteName(table.Schema), (object) StringUtil.QuoteName(table.Name), (object) columnList);
      this.PrepareSqlBatch(sqlStatement.Length);
      this.AddStatement(sqlStatement);
      this.BindGuid("@collectionId", collectionId);
      ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<UserExtensionLicense>((ObjectBinder<UserExtensionLicense>) new ExtensionBinders.UserExtensionLicenseBinder());
      return (IList<UserExtensionLicense>) resultCollection.GetCurrent<UserExtensionLicense>().Items;
    }
  }
}
