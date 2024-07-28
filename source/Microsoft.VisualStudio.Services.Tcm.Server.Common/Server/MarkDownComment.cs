// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.MarkDownComment
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class MarkDownComment
  {
    private int _iterationId;
    private string _iterationUrl;
    private string _coverageReportUrl;
    private CoverageMetrics _coverageMetrics;
    private readonly int _maxFilesToShowInComment;
    private readonly int _maxFilesInPullRequest;
    private Dictionary<string, FolderCoverageResult> _folderCoverageResults;
    public const string HorizontalLine = "---";

    public MarkDownComment(
      TestManagementRequestContext tcmRequestContext,
      CoverageMetrics coverageMetrics,
      int iterationId,
      string iterationUrl,
      string coverageReportUrl)
    {
      this._coverageMetrics = coverageMetrics;
      this._iterationId = iterationId;
      this._iterationUrl = iterationUrl;
      this._coverageReportUrl = coverageReportUrl;
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      this._maxFilesInPullRequest = coverageConfiguration.GetMaxFilesInPullRequest(tcmRequestContext.RequestContext);
      this._maxFilesToShowInComment = coverageConfiguration.GetCoverageMetricsTableMaxRows(tcmRequestContext.RequestContext);
    }

    public MarkDownComment(
      TestManagementRequestContext tcmRequestContext,
      CoverageMetrics coverageMetrics,
      int iterationId,
      string iterationUrl,
      string coverageReportUrl,
      Dictionary<string, FolderCoverageResult> folderCoverageResults)
    {
      this._coverageMetrics = coverageMetrics;
      this._iterationId = iterationId;
      this._iterationUrl = iterationUrl;
      this._coverageReportUrl = coverageReportUrl;
      this._folderCoverageResults = folderCoverageResults;
      CoverageConfiguration coverageConfiguration = new CoverageConfiguration();
      this._maxFilesInPullRequest = coverageConfiguration.GetMaxFilesInPullRequest(tcmRequestContext.RequestContext);
      this._maxFilesToShowInComment = coverageConfiguration.GetCoverageMetricsTableMaxRows(tcmRequestContext.RequestContext);
    }

    public string CreateComment()
    {
      StringBuilder stringBuilder = new StringBuilder();
      string coverageMarkdown = this.GetDiffCoverageMarkdown();
      if (string.IsNullOrEmpty(coverageMarkdown))
        return coverageMarkdown;
      stringBuilder.AppendLine(coverageMarkdown);
      stringBuilder.AppendLine();
      if (this._coverageMetrics != null && this._coverageMetrics.FileCoverageResults != null && this._coverageMetrics.FileCoverageResults.Count > 0)
      {
        stringBuilder.AppendLine("<details>");
        stringBuilder.AppendLine("<summary> Details </summary>");
        stringBuilder.AppendLine();
        string str1 = this.GetCoverageTableMarkdown().ToString().Replace(" ", "&nbsp;");
        stringBuilder.AppendLine(str1);
        stringBuilder.AppendLine(this.GetCoverageReportMarkdown());
        stringBuilder.AppendLine();
        stringBuilder.AppendLine("</details>");
        if (this._folderCoverageResults != null && this._folderCoverageResults.Count > 0)
        {
          stringBuilder.AppendLine("<details>");
          stringBuilder.AppendLine("<summary> Folder Level Coverage Details </summary>");
          stringBuilder.AppendLine();
          string str2 = this.GetFolderCoverageTableMarkdown().ToString().Replace(" ", "&nbsp;");
          stringBuilder.AppendLine(str2);
          stringBuilder.AppendLine("</details>");
        }
      }
      return stringBuilder.ToString();
    }

    private StringBuilder GetFolderCoverageTableMarkdown()
    {
      string[] strArray = new string[2]
      {
        CoverageResources.FolderPath,
        CoverageResources.ChangedLinesCoveredText
      };
      StringBuilder coverageTableMarkdown = new StringBuilder();
      coverageTableMarkdown.AppendLine(MarkDownComment.GetTableHeaderOrContentRow(strArray));
      coverageTableMarkdown.AppendLine(MarkDownComment.GetTableSeparatorRow(strArray.Length));
      List<MarkDownComment.CoverageEntry> coverageEntryList = new List<MarkDownComment.CoverageEntry>();
      foreach (KeyValuePair<string, FolderCoverageResult> folderCoverageResult in this._folderCoverageResults)
      {
        string str1 = folderCoverageResult.Key;
        string str2 = this._iterationUrl + "&path=" + HttpUtility.UrlEncode(str1);
        if (str1.StartsWith("/") && str1.Length > 1)
          str1 = str1.Substring(1);
        string str3 = "[" + str1 + "](" + str2 + ")";
        string empty = string.Empty;
        string str4 = !folderCoverageResult.Value.CumulativeDiffCoverage.NoExecutableChanges ? (!folderCoverageResult.Value.CumulativeDiffCoverage.CoverageDataNotFound ? MarkDownComment.GetFolderCoverageCellText(folderCoverageResult.Value) : CoverageResources.CoverageDataNotFound) : CoverageResources.NoExecutableChanges;
        coverageEntryList.Add(new MarkDownComment.CoverageEntry()
        {
          Coverage = folderCoverageResult.Value.CumulativeDiffCoverage.Coverage,
          NoExecutableChanges = folderCoverageResult.Value.CumulativeDiffCoverage.NoExecutableChanges,
          CoverageDataNotFound = folderCoverageResult.Value.CumulativeDiffCoverage.CoverageDataNotFound,
          TableRowMarkdown = MarkDownComment.GetTableHeaderOrContentRow(str3, str4)
        });
      }
      coverageEntryList.Sort();
      int num = 0;
      bool flag = false;
      foreach (MarkDownComment.CoverageEntry coverageEntry in coverageEntryList)
      {
        coverageTableMarkdown.AppendLine(coverageEntry.TableRowMarkdown);
        ++num;
        if (num > this._maxFilesToShowInComment)
        {
          flag = true;
          break;
        }
      }
      if (flag)
        coverageTableMarkdown.AppendLine(MarkDownComment.GetTableHeaderOrContentRow(Enumerable.Repeat<string>("...", strArray.Length).ToArray<string>()));
      return coverageTableMarkdown;
    }

    private StringBuilder GetCoverageTableMarkdown()
    {
      string[] strArray = new string[3]
      {
        CoverageResources.ChangedFilesText,
        CoverageResources.LinesCoveredText,
        CoverageResources.ChangedLinesCoveredText
      };
      StringBuilder coverageTableMarkdown = new StringBuilder();
      coverageTableMarkdown.AppendLine(MarkDownComment.GetTableHeaderOrContentRow(strArray));
      coverageTableMarkdown.AppendLine(MarkDownComment.GetTableSeparatorRow(strArray.Length));
      List<MarkDownComment.CoverageEntry> coverageEntryList = new List<MarkDownComment.CoverageEntry>();
      foreach (FileCoverageResult fileCoverageResult in this._coverageMetrics.FileCoverageResults)
      {
        string str1 = fileCoverageResult.FilePath;
        string str2 = this._iterationUrl + "&path=" + HttpUtility.UrlEncode(fileCoverageResult.FilePath);
        if (str1.StartsWith("/") && str1.Length > 1)
          str1 = str1.Substring(1);
        string str3 = "[" + str1 + "](" + str2 + ")";
        string empty1 = string.Empty;
        string str4 = !fileCoverageResult.OverallCoverage.CoverageDataNotFound ? (!fileCoverageResult.OverallCoverage.NoExecutableChanges ? MarkDownComment.GetCoverageCellText(fileCoverageResult.OverallCoverage) : CoverageResources.NoExecutableChanges) : CoverageResources.CoverageDataNotFound;
        string empty2 = string.Empty;
        string str5 = !fileCoverageResult.DiffCoverage.NoExecutableChanges ? (!fileCoverageResult.DiffCoverage.CoverageDataNotFound ? MarkDownComment.GetCoverageCellText(fileCoverageResult.DiffCoverage) : CoverageResources.CoverageDataNotFound) : CoverageResources.NoExecutableChanges;
        coverageEntryList.Add(new MarkDownComment.CoverageEntry()
        {
          Coverage = fileCoverageResult.DiffCoverage.Coverage,
          NoExecutableChanges = fileCoverageResult.DiffCoverage.NoExecutableChanges,
          CoverageDataNotFound = fileCoverageResult.DiffCoverage.CoverageDataNotFound,
          TableRowMarkdown = MarkDownComment.GetTableHeaderOrContentRow(str3, str4, str5)
        });
      }
      coverageEntryList.Sort();
      int num = 0;
      bool flag = false;
      foreach (MarkDownComment.CoverageEntry coverageEntry in coverageEntryList)
      {
        coverageTableMarkdown.AppendLine(coverageEntry.TableRowMarkdown);
        ++num;
        if (num > this._maxFilesToShowInComment)
        {
          flag = true;
          break;
        }
      }
      if (flag)
        coverageTableMarkdown.AppendLine(MarkDownComment.GetTableHeaderOrContentRow(Enumerable.Repeat<string>("...", strArray.Length).ToArray<string>()));
      return coverageTableMarkdown;
    }

    private string GetDiffCoverageMarkdown()
    {
      string empty = string.Empty;
      string iterationMarkdown = this.GetIterationMarkdown();
      if (this._coverageMetrics == null || this._coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.NoCodeFilesInPullRequest)
        return "> " + iterationMarkdown + ": " + CoverageResources.NoCodeCoverageText;
      if (this._coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.FailedToGetPullRequestChanges)
        return "> " + iterationMarkdown + ": " + CoverageResources.FailedToGetPullRequestChanges;
      if (this._coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.MaxFilesInPullRequestExceeded)
      {
        string str = string.Format(CoverageResources.MaxFilesExceeded, (object) this._maxFilesInPullRequest);
        return "> " + iterationMarkdown + ": " + str;
      }
      if (this._coverageMetrics.CoverageEvaluationStatus == FileCoverageEvaluationStatus.FilePathNotFound)
        return "> " + iterationMarkdown + ": " + CoverageResources.FailedToFindFileAtSamePath;
      CoverageStatusCheckState statusCheckState = CoverageStatusCheckState.Error;
      CoverageStatusCheckResult statusCheckResult1 = this._coverageMetrics.CoverageStatusCheckResult;
      CoverageStatusCheckResult statusCheckResult2;
      if (statusCheckResult1 == null)
      {
        statusCheckResult2 = (CoverageStatusCheckResult) null;
      }
      else
      {
        List<CoverageStatusCheckResult> subResults = statusCheckResult1.SubResults;
        statusCheckResult2 = subResults != null ? subResults.Where<CoverageStatusCheckResult>((Func<CoverageStatusCheckResult, bool>) (x => x.Name == "DiffCoverageStatusCheck")).FirstOrDefault<CoverageStatusCheckResult>() : (CoverageStatusCheckResult) null;
      }
      CoverageStatusCheckResult statusCheckResult3 = statusCheckResult2;
      if (statusCheckResult3 == null)
      {
        string message = "Diff coverage status check result not available.";
        statusCheckState = CoverageStatusCheckState.Error;
        throw new Exception(message);
      }
      string description = statusCheckResult3.Description;
      CoverageStatusCheckState state = statusCheckResult3.State;
      string str1;
      if (this._coverageMetrics.AggregatedDiffCoverage.CoverageDataNotFound)
        str1 = iterationMarkdown + ": " + CoverageResources.DiffCoverageMessageCoverageNotFound;
      else if (this._coverageMetrics.AggregatedDiffCoverage.NoExecutableChanges)
        str1 = iterationMarkdown + ": " + CoverageResources.DiffCoverageMessageNoExecutableChanges;
      else if (!this._coverageMetrics.AggregatedDiffCoverage.Coverage.HasValue)
      {
        str1 = iterationMarkdown + ": " + CoverageResources.DiffCoverageMessageCannotDetermine;
      }
      else
      {
        string textFormatString = CoverageResources.DiffCoverageLinesCoveredTextFormatString;
        object[] objArray = new object[4]
        {
          (object) this._coverageMetrics.AggregatedDiffCoverage.CoveredLines,
          (object) this._coverageMetrics.AggregatedDiffCoverage.TotalLines,
          null,
          null
        };
        ref double? local = ref this._coverageMetrics.AggregatedDiffCoverage.Coverage;
        objArray[2] = (object) (local.HasValue ? local.GetValueOrDefault().ToString("P", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null);
        objArray[3] = (object) iterationMarkdown;
        str1 = string.Format("{0} {1}", (object) string.Format(textFormatString, objArray), state == CoverageStatusCheckState.Error ? (object) string.Empty : (object) description);
      }
      string str2 = "";
      switch (state)
      {
        case CoverageStatusCheckState.Error:
          str2 = description ?? "";
          break;
        case CoverageStatusCheckState.Failed:
          str2 = CoverageResources.DiffCoverageCheckFailed;
          break;
        case CoverageStatusCheckState.Succeeded:
          str2 = CoverageResources.DiffCoverageCheckSucceeded;
          break;
        case CoverageStatusCheckState.NotApplicable:
          str2 = CoverageResources.DiffCoverageCheckNotApplicable;
          break;
      }
      if (this._folderCoverageResults != null && this._folderCoverageResults.Count > 0 && this._folderCoverageResults.Where<KeyValuePair<string, FolderCoverageResult>>((Func<KeyValuePair<string, FolderCoverageResult>, bool>) (x => x.Value.CoverageStatusCheck == CoverageStatusCheckState.Failed)).Any<KeyValuePair<string, FolderCoverageResult>>())
        str2 = CoverageResources.DiffCoverageMessageFolderLevelFailed;
      return string.Format("> {0}\n>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;{1}", (object) str2, (object) str1);
    }

    private string GetIterationMarkdown()
    {
      string iterationMarkdown = string.Format("Update {0}", (object) this._iterationId);
      if (!string.IsNullOrEmpty(this._iterationUrl))
        iterationMarkdown = "[" + iterationMarkdown + "](" + this._iterationUrl + ")";
      return iterationMarkdown;
    }

    private string GetCoverageReportMarkdown()
    {
      string str = CoverageResources.ViewFullCoverageReport;
      if (!string.IsNullOrEmpty(this._coverageReportUrl))
        str = "[" + str + "](" + this._coverageReportUrl + ")";
      return str ?? "";
    }

    private static string GetCoverageCellText(LineCoverageResult lineCoverageResult)
    {
      if (!lineCoverageResult.Coverage.HasValue)
        return string.Empty;
      // ISSUE: variable of a boxed type
      __Boxed<uint> coveredLines = (ValueType) lineCoverageResult.CoveredLines;
      // ISSUE: variable of a boxed type
      __Boxed<uint> totalLines = (ValueType) lineCoverageResult.TotalLines;
      ref double? local = ref lineCoverageResult.Coverage;
      string str = local.HasValue ? local.GetValueOrDefault().ToString("P", (IFormatProvider) CultureInfo.InvariantCulture) : (string) null;
      return string.Format("{0}/{1}  ({2})", (object) coveredLines, (object) totalLines, (object) str);
    }

    private static string GetFolderCoverageCellText(FolderCoverageResult folderCoverageResult)
    {
      if (folderCoverageResult.CumulativeDiffCoverage == null)
        return string.Empty;
      return folderCoverageResult.CoverageStatusCheck == CoverageStatusCheckState.Succeeded ? string.Format(CoverageResources.DiffCoverageFolderLevelStringFormatPassed, (object) folderCoverageResult.CumulativeDiffCoverage.CoveredLines, (object) folderCoverageResult.CumulativeDiffCoverage.TotalLines, (object) folderCoverageResult.CumulativeDiffCoverage.Coverage.Value.ToString("P"), (object) folderCoverageResult.target) : string.Format(CoverageResources.DiffCoverageFolderLevelStringFormatFailed, (object) folderCoverageResult.CumulativeDiffCoverage.CoveredLines, (object) folderCoverageResult.CumulativeDiffCoverage.TotalLines, (object) folderCoverageResult.CumulativeDiffCoverage.Coverage.Value.ToString("P"), (object) folderCoverageResult.target);
    }

    private static string GetTableHeaderOrContentRow(params string[] values)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < values.Length; ++index)
      {
        string str = values[index];
        stringBuilder.Append("|" + str);
        if (index == values.Length - 1)
          stringBuilder.Append("|");
      }
      return stringBuilder.ToString();
    }

    private static string GetTableSeparatorRow(int columnCount)
    {
      StringBuilder stringBuilder = new StringBuilder();
      for (int index = 0; index < columnCount; ++index)
      {
        stringBuilder.Append("|:");
        stringBuilder.Append("---");
        if (index == columnCount - 1)
          stringBuilder.Append("|");
      }
      return stringBuilder.ToString();
    }

    internal class CoverageEntry : IComparable
    {
      public string TableRowMarkdown { get; set; }

      public double? Coverage { get; set; }

      public bool CoverageDataNotFound { get; set; }

      public bool NoExecutableChanges { get; set; }

      public int CompareTo(object obj)
      {
        MarkDownComment.CoverageEntry coverageEntry = (MarkDownComment.CoverageEntry) obj;
        if (this.CoverageDataNotFound && coverageEntry.CoverageDataNotFound)
          return 0;
        if (this.CoverageDataNotFound)
          return 1;
        if (coverageEntry.CoverageDataNotFound)
          return -1;
        if (this.NoExecutableChanges && coverageEntry.NoExecutableChanges)
          return 0;
        if (this.NoExecutableChanges)
          return 1;
        if (coverageEntry.NoExecutableChanges)
          return -1;
        if (!this.Coverage.HasValue && !coverageEntry.Coverage.HasValue)
          return 0;
        if (!this.Coverage.HasValue)
          return -1;
        if (!coverageEntry.Coverage.HasValue)
          return 1;
        double? coverage1 = this.Coverage;
        double? coverage2 = coverageEntry.Coverage;
        if (coverage1.GetValueOrDefault() < coverage2.GetValueOrDefault() & coverage1.HasValue & coverage2.HasValue)
          return -1;
        coverage2 = this.Coverage;
        double? coverage3 = coverageEntry.Coverage;
        return coverage2.GetValueOrDefault() > coverage3.GetValueOrDefault() & coverage2.HasValue & coverage3.HasValue ? 1 : 0;
      }
    }
  }
}
