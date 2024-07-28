// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.EstimatedExecutionPlanUtil
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public static class EstimatedExecutionPlanUtil
  {
    public static bool ReviewEstimatedExecutionPlans(
      string connectionString,
      EstimatedExecutionPlanTestLevel testLevel,
      IList<string> errorList,
      string outFolder,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<string>(connectionString, nameof (connectionString));
      return EstimatedExecutionPlanUtil.ReviewEstimatedExecutionPlans(SqlConnectionInfoFactory.Create(connectionString), testLevel, errorList, outFolder, logger);
    }

    public static bool ReviewEstimatedExecutionPlans(
      ISqlConnectionInfo connectionInfo,
      EstimatedExecutionPlanTestLevel testLevel,
      IList<string> errorList,
      string outFolder,
      ITFLogger logger)
    {
      ArgumentUtility.CheckForNull<IList<string>>(errorList, nameof (errorList));
      ArgumentUtility.CheckForNull<ITFLogger>(logger, nameof (logger));
      logger.Info("Starting ReviewEstimatedExecutionPlans for {0}", (object) ConnectionStringUtility.MaskPassword(connectionInfo.ConnectionString));
      List<EstimatedExecutionPlanComponent.EstimateableItem> estimateableItems;
      using (EstimatedExecutionPlanComponent componentRaw = connectionInfo.CreateComponentRaw<EstimatedExecutionPlanComponent>(logger: logger))
      {
        EstimatedExecutionPlanComponent executionPlanComponent = componentRaw;
        bool flag = testLevel == EstimatedExecutionPlanTestLevel.ModifiedOnly;
        DateTime? modifiedAfter = new DateTime?();
        int num = flag ? 1 : 0;
        estimateableItems = executionPlanComponent.GetEstimateableItems(modifiedAfter: modifiedAfter, useDatabaseCreation: num != 0);
      }
      List<EstimatedExecutionPlanComponent.EstimateableItem> errorItemList1;
      int num1 = EstimatedExecutionPlanUtil.ProcessExecutionPlanItems(connectionInfo, errorList, estimateableItems, outFolder, logger, out errorItemList1);
      int num2 = 0;
      if (errorItemList1 != null && num1 < errorItemList1.Count)
      {
        logger.Info("The previous run for estimated execution plan has encounter a SQL Server Error: that is retry able (701 or 1750)");
        bool flag = true;
        while (errorItemList1 != null)
        {
          EstimatedExecutionPlanUtil.LogCurrentStoredProcedureCacheState(connectionInfo, logger);
          using (SqlScriptResourceComponent componentRaw = connectionInfo.CreateComponentRaw<SqlScriptResourceComponent>(logger: logger))
            componentRaw.ExecuteStatement("DBCC FREEPROCCACHE", 300);
          List<EstimatedExecutionPlanComponent.EstimateableItem> errorItemList2;
          num1 = EstimatedExecutionPlanUtil.ProcessExecutionPlanItems(connectionInfo, errorList, errorItemList1, outFolder, logger, out errorItemList2);
          ++num2;
          if (errorItemList2 != null)
          {
            if (errorItemList2.Count < errorItemList1.Count)
            {
              logger.Info("Failed item reduced from {0} to {1}, we will try failed items again", (object) errorItemList1.Count, (object) errorItemList2.Count);
              errorItemList1 = errorItemList2;
            }
            else
            {
              flag = false;
              logger.Error("Retry of estimated execution plan failed with {0} items.", (object) errorItemList2.Count);
              EstimatedExecutionPlanUtil.LogCurrentStoredProcedureCacheState(connectionInfo, logger);
              errorItemList1 = (List<EstimatedExecutionPlanComponent.EstimateableItem>) null;
            }
          }
          else
            errorItemList1 = (List<EstimatedExecutionPlanComponent.EstimateableItem>) null;
        }
        EventLog.WriteEntry("Application", string.Format("Estimated Execution Plan has some retriable errors. Retried {0} times, success ({1})", (object) num2, (object) flag), EventLogEntryType.Error, 12345);
      }
      return num1 == 0;
    }

    private static int ProcessExecutionPlanItems(
      ISqlConnectionInfo connectionInfo,
      IList<string> errorList,
      List<EstimatedExecutionPlanComponent.EstimateableItem> items,
      string outFolder,
      ITFLogger logger,
      out List<EstimatedExecutionPlanComponent.EstimateableItem> errorItemList)
    {
      errorItemList = (List<EstimatedExecutionPlanComponent.EstimateableItem>) null;
      ConcurrentBag<EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion> bag = new ConcurrentBag<EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion>();
      ParallelOptions parallelOptions = new ParallelOptions()
      {
        MaxDegreeOfParallelism = 4
      };
      if (!string.IsNullOrEmpty(outFolder))
      {
        Directory.CreateDirectory(Path.Combine(outFolder, "Sprocs"));
        Directory.CreateDirectory(Path.Combine(outFolder, "Functions"));
        Directory.CreateDirectory(Path.Combine(outFolder, "Views"));
      }
      Parallel.ForEach<EstimatedExecutionPlanComponent.EstimateableItem, EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion>((IEnumerable<EstimatedExecutionPlanComponent.EstimateableItem>) items, parallelOptions, (Func<EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion>) (() => new EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion(connectionInfo, logger)), (Func<EstimatedExecutionPlanComponent.EstimateableItem, ParallelLoopState, EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion, EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion>) ((item, loopState, portion) =>
      {
        try
        {
          string estimatedExecutionPlan = portion.Component.GetEstimatedExecutionPlan(item);
          if (!string.IsNullOrEmpty(outFolder))
          {
            XDocument xdocument = XDocument.Parse(estimatedExecutionPlan);
            string path2;
            switch (item)
            {
              case EstimatedExecutionPlanComponent.StoredProcedureDescription _:
                path2 = "Sprocs";
                break;
              case EstimatedExecutionPlanComponent.FunctionDescription _:
                path2 = "Functions";
                break;
              case EstimatedExecutionPlanComponent.ViewDescription _:
                path2 = "Views";
                break;
              default:
                throw new InvalidOperationException("Unexpected " + item.GetType().FullName);
            }
            string fileName = Path.Combine(outFolder, path2, item.Schema + "." + item.Name + ".sqlplan");
            xdocument.Save(fileName, SaveOptions.None);
          }
          ++portion.Succeeded;
        }
        catch (Exception ex)
        {
          portion.Failures.Add(new Tuple<EstimatedExecutionPlanComponent.EstimateableItem, Exception>(item, ex));
        }
        return portion;
      }), (Action<EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion>) (portion =>
      {
        portion.Component.Dispose();
        portion.Component = (EstimatedExecutionPlanComponent) null;
        bag.Add(portion);
      }));
      errorList?.Add(string.Join("\t", new string[5]
      {
        "Item Type",
        "Item Name",
        "Execution Statement",
        "SQL Error Message",
        ""
      }));
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      int num5 = 0;
      foreach (EstimatedExecutionPlanUtil.EsimtatedExecutionPlanPortion executionPlanPortion in bag)
      {
        num1 += executionPlanPortion.Succeeded;
        foreach (Tuple<EstimatedExecutionPlanComponent.EstimateableItem, Exception> failure in executionPlanPortion.Failures)
        {
          EstimatedExecutionPlanComponent.EstimateableItem estimateableItem = failure.Item1;
          Exception exception1 = failure.Item2;
          if (!(estimateableItem.WellFormedName == "[dbo].[prc_TransferIdentityRights]"))
          {
            if (estimateableItem.Whitelisted)
            {
              ++num4;
            }
            else
            {
              StringBuilder stringBuilder1 = new StringBuilder();
              StringBuilder stringBuilder2 = new StringBuilder();
              stringBuilder2.AppendFormat("{0}\t{1}\t{2}\t", (object) estimateableItem.GetType().Name, (object) estimateableItem.WellFormedName, (object) estimateableItem.ExecutionStatement);
              stringBuilder1.AppendFormat("Failed to compute an Estimated Execution Plan for {0} {1}.", (object) estimateableItem.GetType().Name, (object) estimateableItem.WellFormedName);
              stringBuilder1.AppendLine();
              stringBuilder1.AppendLine(estimateableItem.ExecutionStatement);
              bool flag1 = false;
              bool flag2 = false;
              bool flag3 = false;
              Exception exception2 = exception1;
              while (true)
              {
                switch (exception2)
                {
                  case null:
                    goto label_20;
                  case SqlException _:
                    SqlException sqlException = (SqlException) exception2;
                    if (sqlException.Number == 701)
                    {
                      stringBuilder1.AppendFormat("Encounter error number 701 (insufficient system memory) when executing {0}.\r\nAssuming we are making progress, we will try to re-run this {1}.", (object) estimateableItem.WellFormedName, (object) estimateableItem.GetType().Name);
                      flag3 = true;
                    }
                    if (sqlException.Number == 1750)
                    {
                      stringBuilder1.AppendFormat("Encounter error number 1750 (Could not create constraint. See previous errors) when executing {0}.\r\nAssuming we are making progress, we will try to re-run this {1}.", (object) estimateableItem.WellFormedName, (object) estimateableItem.GetType().Name);
                      flag3 = true;
                    }
                    stringBuilder2.AppendFormat("{0}\t", (object) sqlException.Message.Replace('\t', '~'));
                    stringBuilder1.AppendLine(sqlException.Message);
                    stringBuilder1.AppendFormat("Number: {0}, ErrorCode: {1}", (object) sqlException.Number, (object) sqlException.ErrorCode);
                    flag1 = true;
                    break;
                  case EstimatedExecutionPlanComponent.NotFullyDefinedException _:
                    stringBuilder2.AppendFormat("{0}\t", (object) exception1.Message.Replace('\t', '~'));
                    stringBuilder1.AppendLine(exception1.Message);
                    flag2 = true;
                    flag1 = true;
                    break;
                }
                exception2 = exception2.InnerException;
              }
label_20:
              if (flag3)
                ++num5;
              if (!flag1)
              {
                stringBuilder2.AppendFormat("{0}\t", (object) exception1.Message.Replace('\t', '~'));
                stringBuilder1.Append(exception1.ToReadableStackTrace());
              }
              errorList?.Add(stringBuilder2.ToString());
              if (flag3)
                logger.Warning(stringBuilder1.ToString());
              else
                logger.Error(stringBuilder1.ToString());
              if (flag2)
              {
                ++num3;
              }
              else
              {
                if (errorItemList == null)
                  errorItemList = new List<EstimatedExecutionPlanComponent.EstimateableItem>();
                errorItemList.Add(estimateableItem);
                ++num2;
              }
            }
          }
        }
      }
      int num6 = num2 - num5;
      string message = string.Format("Done ReviewEstimatedExecutionPlans. succeeded: {0}, failed: {1}, untested: {2}, whitelisted: {3}, retriable: {4}", (object) num1, (object) num6, (object) num3, (object) num4, (object) num5);
      if (num6 > 0)
      {
        logger.Error(message);
        logger.Warning("\r\nIf an item has valid SQL but is failing it can be whitelisted to do so include the following as a comment in its definition\r\n--EEPR WHITELISTED:\r\nfollowed by a reason for whitelisting it, for example\r\n--EEPR WHITELISTED:Uses #propertiesToDelete.IX_propertiesToDelete_Id\r\nSome common reasons for this to happen is conditionally access Tables/Columns/Indexes that do not always exist within an existence check, and using Indexes on temporary tables.\r\nSelecting into a temporary table is another possible cause however that should be replace by creating the temporary table first.");
      }
      else
        logger.Info(message);
      return num6;
    }

    private static void LogCurrentStoredProcedureCacheState(
      ISqlConnectionInfo connectionInfo,
      ITFLogger logger)
    {
      logger.Info("Current Stored Procedure Cache State");
      logger.Info("num proc buffs, num proc buffs used, num proc buffs active, proc cache size, proc cache used, proc cache active");
      using (SqlScriptResourceComponent componentRaw = connectionInfo.CreateComponentRaw<SqlScriptResourceComponent>(logger: logger))
        componentRaw.ExecuteStatement("DBCC PROCCACHE", 300);
    }

    private class EsimtatedExecutionPlanPortion
    {
      public EsimtatedExecutionPlanPortion(ISqlConnectionInfo connectionInfo, ITFLogger logger)
      {
        this.Component = connectionInfo.CreateComponentRaw<EstimatedExecutionPlanComponent>(logger: logger);
        this.Failures = new List<Tuple<EstimatedExecutionPlanComponent.EstimateableItem, Exception>>();
      }

      public EstimatedExecutionPlanComponent Component { get; set; }

      public int Succeeded { get; set; }

      public List<Tuple<EstimatedExecutionPlanComponent.EstimateableItem, Exception>> Failures { get; private set; }
    }
  }
}
