// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.FrameworkFileDiffEvaluator
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.DevSecOps.Common;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public class FrameworkFileDiffEvaluator
  {
    private ClientTraceData m_ctData;
    private IVssRequestContext m_requestContext;
    private IRemoteRepository m_remoteRepository;
    private IDiffChange[] m_fileChanges;
    private const string c_Layer = "FrameworkFileDiffEvaluator";

    public FrameworkFileDiffEvaluator(
      IVssRequestContext requestContext,
      IRemoteRepository remoteRepository,
      ClientTraceData ctData)
    {
      this.m_requestContext = requestContext;
      this.m_remoteRepository = remoteRepository;
      this.m_ctData = ctData;
    }

    public async Task<List<Violation>> GetFilteredResults(
      List<Violation> results,
      SourceContext resourceContext,
      Stream modifiedStream,
      Encoding modifiedEncoding = null,
      Stopwatch diffStopWatch = null)
    {
      if (results.Count == 0)
        return new List<Violation>();
      diffStopWatch?.Start();
      List<Violation> validCredScanViolations = new List<Violation>();
      DiffSummary diffSummary = (DiffSummary) null;
      using (Stream baseFileStream = await this.GetBaseFileStream(this.m_requestContext, this.m_remoteRepository, resourceContext.ResourceName, resourceContext.TargetBranchId))
        diffSummary = this.PerformDiff(this.m_requestContext, resourceContext, baseFileStream, modifiedStream, modifiedEncoding);
      if (diffSummary == null)
        return results;
      this.m_fileChanges = diffSummary.Changes;
      this.m_requestContext.TraceInfo(27009013, nameof (FrameworkFileDiffEvaluator), "Content difference between local file and remote version has been computed for file {0}.", (object) resourceContext.ResourceName);
      StringBuilder stringBuilder = new StringBuilder();
      foreach (Violation result in results)
      {
        if (!this.IsNewlyIntroducedCredential(result))
        {
          stringBuilder.AppendFormat("Credential with hash {0} at line {1} and column {2} is skipped because it's not newly introduced.\n", (object) result.MatchSecretHash, (object) result.LineNumber, (object) result.StartColumn);
          TelemetryHelper.AddToResults(this.m_ctData, "FilteredIssues", result.ToJson());
        }
        else
        {
          stringBuilder.AppendFormat("Credential with hash {0} at line {1} and column {2} has been flagged as a legitimate violation.\n", (object) result.MatchSecretHash, (object) result.LineNumber, (object) result.StartColumn);
          validCredScanViolations.Add(result);
        }
      }
      this.m_requestContext.TraceInfo(27009014, nameof (FrameworkFileDiffEvaluator), "After applying diff computation: {0}", (object) stringBuilder.ToString());
      this.AddToTelemetry(diffStopWatch);
      return validCredScanViolations;
    }

    internal DiffSummary PerformDiff(
      IVssRequestContext requestContext,
      SourceContext resourceContext,
      Stream originalStream,
      Stream modifiedFileStream,
      Encoding modifiedEncoding)
    {
      if (originalStream.Length == 0L)
      {
        this.m_requestContext.TraceInfo(27009012, nameof (FrameworkFileDiffEvaluator), "Skipping file difference computation because the remote file stream for file {0} was empty.", (object) resourceContext.ResourceName);
        return (DiffSummary) null;
      }
      return DiffUtil.Diff(originalStream, Encoding.UTF8, modifiedFileStream, modifiedEncoding ?? Encoding.UTF8, new DiffOptions()
      {
        Flags = DiffOptionFlags.IgnoreEndOfLineDifference | DiffOptionFlags.IgnoreWhiteSpace | DiffOptionFlags.EnablePreambleHandling | DiffOptionFlags.IgnoreEndOfFileEndOfLineDifference
      }, true);
    }

    public virtual async Task<Stream> GetBaseFileStream(
      IVssRequestContext requestContext,
      IRemoteRepository remoteRepository,
      string path,
      string branch)
    {
      return await remoteRepository.GetRemoteFileStream(branch);
    }

    private bool IsNewlyIntroducedCredential(Violation credScanViolation)
    {
      foreach (IDiffChange fileChange in this.m_fileChanges)
      {
        if ((fileChange.ChangeType == DiffChangeType.Insert || fileChange.ChangeType == DiffChangeType.Change) && fileChange.ModifiedStart + 1 <= credScanViolation.LineNumber && credScanViolation.LineNumber <= fileChange.ModifiedEnd)
          return true;
      }
      return false;
    }

    private void AddToTelemetry(Stopwatch stopWatch)
    {
      if (stopWatch == null)
        return;
      stopWatch.Stop();
      this.m_ctData.Add("DiffComputeDuration", (object) stopWatch.ElapsedMilliseconds);
    }
  }
}
