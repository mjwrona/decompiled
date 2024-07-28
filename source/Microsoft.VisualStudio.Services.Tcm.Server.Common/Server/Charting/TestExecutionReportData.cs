// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Charting.TestExecutionReportData
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace Microsoft.TeamFoundation.TestManagement.Server.Charting
{
  [CLSCompliant(false)]
  public class TestExecutionReportData
  {
    private List<DatedTestFieldData> m_reportData;

    public TestExecutionReportData() => this.m_reportData = new List<DatedTestFieldData>();

    private static string GetStringForOutcome(Microsoft.TeamFoundation.TestManagement.Client.TestOutcome outcome)
    {
      string empty = string.Empty;
      string stringForOutcome;
      switch (outcome)
      {
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Unspecified;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_None;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Passed:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Passed;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Failed:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Failed;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Inconclusive:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Inconclusive;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Timeout:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Timeout;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Aborted:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Aborted;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Blocked:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Blocked;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotExecuted:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_NotExecuted;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Warning:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Warning;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Error:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Error;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotApplicable:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_NotApplicable;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Paused:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_Paused;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.InProgress:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_InProgress;
          break;
        case Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.NotImpacted:
          stringForOutcome = Microsoft.TeamFoundation.TestManagement.Server.ServerResources.TestOutcome_NotImpacted;
          break;
        default:
          stringForOutcome = outcome.ToString();
          break;
      }
      return stringForOutcome;
    }

    public static string MapDimensionValue(
      IVssRequestContext context,
      string dimensionName,
      object dimensionValue)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(context, nameof (context));
      ArgumentUtility.CheckStringForNullOrEmpty(dimensionName, nameof (dimensionName));
      int result1 = 0;
      string empty = string.Empty;
      bool flag = dimensionValue != null && dimensionValue.ToString() != SqlString.Null.ToString();
      string str;
      if (dimensionName != null)
      {
        switch (dimensionName.Length)
        {
          case 5:
            if (dimensionName == "RunBy")
              break;
            goto label_16;
          case 6:
            if (dimensionName == "Tester")
              break;
            goto label_16;
          case 7:
            if (dimensionName == "Outcome")
            {
              str = !flag || !int.TryParse(dimensionValue.ToString(), out result1) ? Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotRun : TestExecutionReportData.GetStringForOutcome((Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) result1);
              goto label_17;
            }
            else
              goto label_16;
          case 8:
            if (dimensionName == "Priority")
            {
              str = !flag || !int.TryParse(dimensionValue.ToString(), out result1) ? Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotRun : (result1 != (int) byte.MaxValue ? dimensionValue.ToString() : Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotAvailable);
              goto label_17;
            }
            else
              goto label_16;
          case 10:
            if (dimensionName == "Resolution")
              goto label_15;
            else
              goto label_16;
          case 11:
            if (dimensionName == "FailureType")
              goto label_15;
            else
              goto label_16;
          case 18:
            if (dimensionName == "RunTypeIsAutomated")
            {
              str = !flag ? Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotRun : (dimensionValue.ToString().ToLowerInvariant() == "false" ? Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionRunTypeManual : Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionRunTypeAutomated);
              goto label_17;
            }
            else
              goto label_16;
          default:
            goto label_16;
        }
        str = dimensionName == "RunBy" ? Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotRun : Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotAvailable;
        Guid result2;
        if (flag && Guid.TryParse(dimensionValue.ToString(), out result2))
        {
          string name = Microsoft.TeamFoundation.TestManagement.Server.IdentityHelper.ResolveIdentityToName(new TestManagementRequestContext(context), result2, true);
          if (!string.IsNullOrEmpty(name))
          {
            str = name;
            goto label_17;
          }
          else
            goto label_17;
        }
        else
          goto label_17;
label_15:
        str = flag ? dimensionValue.ToString() : Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNone;
        goto label_17;
      }
label_16:
      str = flag ? dimensionValue.ToString() : Microsoft.TeamFoundation.TestManagement.Server.ServerResources.ChartDimensionValueNotAvailable;
label_17:
      return str;
    }

    public void AddReportDatarow(Dictionary<string, object> dimensionValues, long measure)
    {
      ArgumentUtility.CheckForNull<Dictionary<string, object>>(dimensionValues, nameof (dimensionValues));
      this.m_reportData.Add(new DatedTestFieldData(DateTime.Now, new TestFieldData(dimensionValues, measure)));
    }

    public IEnumerable<DatedTestFieldData> GetReportTable() => (IEnumerable<DatedTestFieldData>) this.m_reportData;

    public Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData ToWebApiModel()
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData webApiModel = new Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData()
      {
        ReportData = new List<Microsoft.TeamFoundation.TestManagement.WebApi.DatedTestFieldData>()
      };
      foreach (DatedTestFieldData datedTestFieldData in this.m_reportData)
        webApiModel.ReportData.Add(datedTestFieldData.ToWebApiModel());
      return webApiModel;
    }

    public static TestExecutionReportData FromWebApiModel(Microsoft.TeamFoundation.TestManagement.WebApi.TestExecutionReportData webApiModel)
    {
      TestExecutionReportData executionReportData = new TestExecutionReportData();
      if (webApiModel != null && webApiModel.ReportData != null)
      {
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.DatedTestFieldData datedTestFieldData in webApiModel.ReportData)
        {
          if (datedTestFieldData != null && datedTestFieldData.Value != null)
            executionReportData.AddReportDatarow(datedTestFieldData.Value.Dimensions, datedTestFieldData.Value.Measure);
        }
      }
      return executionReportData;
    }
  }
}
