// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ServicingComponent13
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class ServicingComponent13 : ServicingComponent12
  {
    private static readonly SqlMetaData[] typ_ServicingOperation = new SqlMetaData[3]
    {
      new SqlMetaData("ServicingOperation", SqlDbType.VarChar, 128L),
      new SqlMetaData("Handlers", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ServicingTarget", SqlDbType.TinyInt)
    };
    private static readonly SqlMetaData[] typ_ServicingOperationGroup = new SqlMetaData[3]
    {
      new SqlMetaData("ServicingOperation", SqlDbType.NVarChar, 128L),
      new SqlMetaData("GroupName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("GroupOrderNumber", SqlDbType.Int)
    };
    private static readonly SqlMetaData[] typ_ServicingStepGroup = new SqlMetaData[2]
    {
      new SqlMetaData("GroupName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Handlers", SqlDbType.NVarChar, -1L)
    };
    private static readonly SqlMetaData[] typ_ServicingStep = new SqlMetaData[7]
    {
      new SqlMetaData("StepName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("GroupName", SqlDbType.NVarChar, 128L),
      new SqlMetaData("OrderNumber", SqlDbType.Int),
      new SqlMetaData("StepPerformer", SqlDbType.NVarChar, 128L),
      new SqlMetaData("StepType", SqlDbType.NVarChar, 128L),
      new SqlMetaData("Options", SqlDbType.Int),
      new SqlMetaData("StepData", SqlDbType.NVarChar, -1L)
    };

    public override ServicingOperation[] GetServicingOperations()
    {
      this.PrepareStoredProcedure("prc_QueryServicingOperations");
      List<ServicingOperationData> items1;
      List<ServicingOperationGroupData> items2;
      List<ServicingStepGroupData> items3;
      List<ServicingStepData> items4;
      using (ResultCollection resultCollection = new ResultCollection((IDataReader) this.ExecuteReader(), this.ProcedureName, this.RequestContext))
      {
        resultCollection.AddBinder<ServicingOperationData>((ObjectBinder<ServicingOperationData>) new ServicingOperationDataBinder());
        resultCollection.AddBinder<ServicingOperationGroupData>((ObjectBinder<ServicingOperationGroupData>) new ServicingOperationGroupDataBinder());
        resultCollection.AddBinder<ServicingStepGroupData>((ObjectBinder<ServicingStepGroupData>) new ServicingStepGroupDataBinder());
        resultCollection.AddBinder<ServicingStepData>((ObjectBinder<ServicingStepData>) new ServicingStepDataBinder());
        items1 = resultCollection.GetCurrent<ServicingOperationData>().Items;
        resultCollection.NextResult();
        items2 = resultCollection.GetCurrent<ServicingOperationGroupData>().Items;
        resultCollection.NextResult();
        items3 = resultCollection.GetCurrent<ServicingStepGroupData>().Items;
        resultCollection.NextResult();
        items4 = resultCollection.GetCurrent<ServicingStepData>().Items;
      }
      return ServicingOperationConverter.ToServicingOperations((IList<ServicingOperationData>) items1, (IList<ServicingOperationGroupData>) items2, (IList<ServicingStepGroupData>) items3, (IList<ServicingStepData>) items4);
    }

    public void UploadServicingOperations(
      List<ServicingOperationData> operations,
      List<ServicingOperationGroupData> operationGroups,
      List<ServicingStepGroupData> stepGroups,
      List<ServicingStepData> steps)
    {
      this.PrepareStoredProcedure("prc_UploadServicingOperations");
      this.BindOperationsTable("@operations", operations);
      this.BindOperationGroupsTable("@operationGroups", operationGroups);
      this.BindStepGroupsTable("@stepGroups", stepGroups);
      this.BindStepsTable("@steps", steps);
      this.ExecuteNonQuery();
    }

    protected SqlParameter BindOperationsTable(
      string parameterName,
      List<ServicingOperationData> operations)
    {
      System.Func<ServicingOperationData, SqlDataRecord> selector = (System.Func<ServicingOperationData, SqlDataRecord>) (operation =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent13.typ_ServicingOperation);
        sqlDataRecord.SetString(0, operation.ServicingOperation);
        sqlDataRecord.SetString(1, operation.Handlers);
        sqlDataRecord.SetByte(2, (byte) operation.ServicingTarget);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingOperation", operations.Select<ServicingOperationData, SqlDataRecord>(selector));
    }

    protected SqlParameter BindOperationGroupsTable(
      string parameterName,
      List<ServicingOperationGroupData> groups)
    {
      System.Func<ServicingOperationGroupData, SqlDataRecord> selector = (System.Func<ServicingOperationGroupData, SqlDataRecord>) (operationGroup =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent13.typ_ServicingOperationGroup);
        sqlDataRecord.SetString(0, operationGroup.ServicingOperation);
        sqlDataRecord.SetString(1, operationGroup.GroupName);
        sqlDataRecord.SetInt32(2, operationGroup.GroupOrderNumber);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingOperationGroup", groups.Select<ServicingOperationGroupData, SqlDataRecord>(selector));
    }

    protected SqlParameter BindStepGroupsTable(
      string parameterName,
      List<ServicingStepGroupData> groups)
    {
      System.Func<ServicingStepGroupData, SqlDataRecord> selector = (System.Func<ServicingStepGroupData, SqlDataRecord>) (stepGroup =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent13.typ_ServicingStepGroup);
        sqlDataRecord.SetString(0, stepGroup.GroupName);
        sqlDataRecord.SetString(1, stepGroup.Handlers);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingStepGroup", groups.Select<ServicingStepGroupData, SqlDataRecord>(selector));
    }

    protected SqlParameter BindStepsTable(string parameterName, List<ServicingStepData> steps)
    {
      System.Func<ServicingStepData, SqlDataRecord> selector = (System.Func<ServicingStepData, SqlDataRecord>) (step =>
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ServicingComponent13.typ_ServicingStep);
        sqlDataRecord.SetString(0, step.StepName);
        sqlDataRecord.SetString(1, step.GroupName);
        sqlDataRecord.SetInt32(2, step.OrderNumber);
        sqlDataRecord.SetString(3, step.StepPerformer);
        sqlDataRecord.SetString(4, step.StepType);
        sqlDataRecord.SetInt32(5, (int) step.Options);
        if (step.StepData != null)
          sqlDataRecord.SetString(6, step.StepData);
        return sqlDataRecord;
      });
      return this.BindTable(parameterName, "typ_ServicingStep", steps.Select<ServicingStepData, SqlDataRecord>(selector));
    }
  }
}
