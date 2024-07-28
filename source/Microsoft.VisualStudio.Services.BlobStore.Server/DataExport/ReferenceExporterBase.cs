// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DataExport.ReferenceExporterBase
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common;
using Microsoft.VisualStudio.Services.BlobStore.Server.Common.Models;
using Microsoft.VisualStudio.Services.Content.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.DataExport
{
  public abstract class ReferenceExporterBase : 
    IBlobIdReferenceProcessor,
    IBlobIdReferenceRowVisitor,
    IDedupMetadataEntryProcessor
  {
    private const string BaseRegistryPath = "/Configuration/BlobStore/DataExport/IdReferences";
    public const string BatchExportCountSetting = "BatchExportCount";
    private const int TracePoint = 5701910;
    private const int DefaultBatchExportCount = 100000;
    protected readonly Microsoft.VisualStudio.Services.Content.Server.Common.Tracer tracer;
    private readonly IDiagnosticsExportService exportService;
    private readonly string logFileName;
    private readonly int batchExportCount;
    private List<string> logResults = new List<string>();
    private bool wasErrorEncounteredDuringProcessing;
    private bool wasErrorEncounteredDuringExport;

    protected abstract string TraceMethod { get; }

    protected abstract string LogFolder { get; }

    protected abstract string RegistryFolder { get; }

    internal abstract string ExportFeatureName { get; }

    protected virtual string LogFileExtension { get; } = ".csv";

    protected abstract void ProcessFileIdReference(IdBlobReferenceRow idBlobReferenceRow);

    protected abstract void ProcessChunkReference(DedupMetadataEntry dedupMetadataEntry);

    public bool IsEnabled { get; protected set; }

    public ReferenceExporterBase(IVssRequestContext requestContext, JobParameters runParameters)
    {
      IVssRequestContext requestContext1 = requestContext;
      TraceData data = new TraceData();
      data.Area = "BlobStore";
      data.Layer = "DiagnosticsExport";
      string traceMethod = this.TraceMethod;
      this.tracer = Microsoft.VisualStudio.Services.Content.Server.Common.Tracer.Enter(requestContext1, data, 5701910, traceMethod);
      this.exportService = requestContext.To(TeamFoundationHostType.Deployment).GetService<IDiagnosticsExportService>();
      this.logFileName = this.CreateLogFileName(requestContext.ServiceHost.InstanceId, runParameters);
      this.batchExportCount = requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) this.BatchExportCountRegistryPath(), 100000);
      this.IsEnabled = requestContext.IsFeatureEnabled(this.ExportFeatureName);
    }

    protected string RegistryPath(string settingName) => "/Configuration/BlobStore/DataExport/IdReferences/" + this.RegistryFolder + "/" + settingName;

    internal string BatchExportCountRegistryPath() => this.RegistryPath("BatchExportCount");

    private string CreateLogFileName(Guid hostId, JobParameters parameters) => this.LogFolder + "/" + parameters.RunDateTime.ToString("yyyy-MM-d") + "/" + hostId.ToString() + "_" + parameters.PartitionId.ToString() + "_" + parameters.RunId + this.LogFileExtension;

    public void VisitIdReference(IdBlobReferenceRow idBlobReferenceRow)
    {
      try
      {
        this.ProcessFileIdReference(idBlobReferenceRow);
        if (this.logResults.Count < this.batchExportCount)
          return;
        this.FlushToLogAsync(this.logResults).ConfigureAwait(false).GetAwaiter().GetResult();
        this.logResults = new List<string>();
      }
      catch (Exception ex)
      {
        this.wasErrorEncounteredDuringProcessing = true;
        this.tracer.TraceException(ex);
      }
    }

    public void VisitEntry(DedupMetadataEntry dedupMetadataEntry)
    {
      try
      {
        this.ProcessChunkReference(dedupMetadataEntry);
        if (this.logResults.Count < this.batchExportCount)
          return;
        this.FlushToLogAsync(this.logResults).ConfigureAwait(false).GetAwaiter().GetResult();
        this.logResults = new List<string>();
      }
      catch (Exception ex)
      {
        this.wasErrorEncounteredDuringProcessing = true;
        this.tracer.TraceException(ex);
      }
    }

    protected void AppendLog(string log) => this.logResults.Add(log);

    public void Complete()
    {
      this.FlushToLogAsync(this.logResults).ConfigureAwait(false).GetAwaiter().GetResult();
      if ((this.wasErrorEncounteredDuringProcessing ? 0 : (!this.wasErrorEncounteredDuringExport ? 1 : 0)) != 0)
        this.tracer.TraceAlways("[Result] Successful export completed.");
      else
        this.tracer.TraceAlways("[Result] Export completed with errors encountered. " + string.Format("ErrorDuringProcessing: {0}, ErrorDuringExport: {1}. ", (object) this.wasErrorEncounteredDuringProcessing, (object) this.wasErrorEncounteredDuringExport));
    }

    private async Task FlushToLogAsync(List<string> resultsToFlush)
    {
      if (resultsToFlush.Count == 0)
        return;
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      cancellationTokenSource.CancelAfter(TimeSpan.FromMinutes(2.0));
      try
      {
        StringBuilder stringBuilder = new StringBuilder();
        foreach (string str in resultsToFlush)
        {
          stringBuilder.Append(str);
          stringBuilder.Append('\n');
        }
        await this.exportService.AppendLogAsync("artifactservicesdataexport", this.logFileName, new ArraySegment<byte>(Encoding.UTF8.GetBytes(stringBuilder.ToString())), cancellationTokenSource.Token).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        this.wasErrorEncounteredDuringExport = true;
        this.tracer.TraceException(ex);
      }
    }
  }
}
