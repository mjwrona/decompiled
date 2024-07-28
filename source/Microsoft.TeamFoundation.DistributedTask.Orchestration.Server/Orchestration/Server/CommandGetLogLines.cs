// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.CommandGetLogLines
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.DataAccess;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.FileContainer;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class CommandGetLogLines : Command
  {
    private TaskLog m_log;
    private long m_endLine;
    private long m_startLine;
    private long m_linesToRead;
    private long m_linesToSkip;
    private TaskHub m_taskHub;
    private TaskLogPage m_currentPage;
    private StreamReader m_pageReader;
    private IEnumerator<TaskLogPage> m_enumerator;
    private TaskOrchestrationPlanReference m_planData;
    private IVssRequestContext m_requestContext;
    private ISecuredObject m_securedObject;

    public CommandGetLogLines(
      IVssRequestContext requestContext,
      TaskHub taskHub,
      ISecuredObject securedObject)
      : base(requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_taskHub = taskHub;
      this.m_securedObject = securedObject;
    }

    public StreamingCollection<string> Lines { get; private set; }

    public void Execute(
      Guid scopeIdentifier,
      Guid planId,
      int logId,
      ref long startLine,
      ref long endLine,
      out long totalLines)
    {
      totalLines = 0L;
      if (endLine >= 0L && endLine < startLine)
        throw new ArgumentException("Startline cannot be greater than endline");
      IList<TaskLogPage> pages = (IList<TaskLogPage>) Array.Empty<TaskLogPage>();
      this.m_requestContext.GetService<DistributedTaskHubService>();
      this.m_planData = this.m_taskHub.EnsurePlanData(this.m_requestContext, scopeIdentifier, planId);
      if (this.m_taskHub.Extension.HasReadPermission(this.m_requestContext, this.m_planData.ScopeIdentifier, this.m_planData.ArtifactUri))
      {
        using (TaskTrackingComponent component = this.m_requestContext.CreateComponent<TaskTrackingComponent>(this.m_taskHub.DataspaceCategory))
          this.m_log = component.GetLog(this.m_planData.ScopeIdentifier, this.m_planData.PlanId, logId, out pages);
        if (this.m_log != null)
        {
          totalLines = this.m_log.LineCount;
          if (endLine < 0L)
          {
            startLine = Math.Max(totalLines + endLine, 0L) + 1L;
            endLine = totalLines;
          }
          if (startLine < 0L)
            startLine = 0L;
        }
        long originalEndLine = endLine;
        long originalStartLine = startLine;
        pages = (IList<TaskLogPage>) pages.Where<TaskLogPage>((Func<TaskLogPage, bool>) (x => x.StartLine <= originalEndLine && x.EndLine >= originalStartLine && x.State != 0)).OrderBy<TaskLogPage, int>((Func<TaskLogPage, int>) (x => x.PageId)).ToList<TaskLogPage>();
      }
      this.Lines = new StreamingCollection<string>((Command) this, 240);
      if (pages.Count == 0)
      {
        totalLines = 0L;
        this.Lines.IsComplete = true;
      }
      else
      {
        endLine = Math.Min(endLine, this.m_log.LineCount);
        this.m_endLine = endLine;
        this.m_startLine = startLine;
        this.m_enumerator = pages.GetEnumerator();
        this.ContinueExecution();
        if (!this.IsCacheFull)
          return;
        this.RequestContext.PartialResultsReady();
      }
    }

    public override void ContinueExecution()
    {
      do
      {
        if (this.m_linesToRead == 0L || this.m_pageReader == null || this.m_pageReader.EndOfStream)
        {
          if (this.m_pageReader != null)
          {
            this.m_pageReader.Dispose();
            this.m_pageReader = (StreamReader) null;
          }
          if (this.m_enumerator.MoveNext())
          {
            this.m_currentPage = this.m_enumerator.Current;
            this.m_linesToSkip = 0L;
            this.m_linesToRead = this.m_currentPage.EndLine - this.m_currentPage.StartLine + 1L;
            if (this.m_currentPage.StartLine < this.m_startLine)
              this.m_linesToSkip = this.m_startLine - this.m_currentPage.StartLine;
            if (this.m_currentPage.EndLine > this.m_endLine)
              this.m_linesToRead = this.m_endLine - this.m_currentPage.StartLine + 1L;
            this.m_pageReader = new StreamReader(this.GetPage(this.m_requestContext, this.m_currentPage), Encoding.UTF8);
          }
          else
          {
            this.Lines.IsComplete = true;
            break;
          }
        }
        string str;
        for (; !this.IsCacheFull && this.m_linesToRead > 0L && (str = this.m_pageReader.ReadLine()) != null; --this.m_linesToRead)
        {
          if (this.m_linesToSkip > 0L)
            --this.m_linesToSkip;
          else
            this.Lines.Enqueue(str);
        }
      }
      while (!this.IsCacheFull);
    }

    protected override void Dispose(bool disposing)
    {
      if (this.m_pageReader == null)
        return;
      this.m_pageReader.Dispose();
      this.m_pageReader = (StreamReader) null;
    }

    private Stream GetPage(IVssRequestContext requestContext, TaskLogPage page)
    {
      if (page.BlobFileId != null)
      {
        IBlobStoreLogService blobLogService = requestContext.GetService<IBlobStoreLogService>();
        return requestContext.RunSynchronously<Stream>((Func<Task<Stream>>) (() => blobLogService.GetLogStreamAsync(requestContext, page.BlobFileId)));
      }
      FileContainerItem fileContainerItem = requestContext.GetService<TeamFoundationFileContainerService>().QueryItems(requestContext.Elevate(), this.m_planData.ContainerId, this.m_log.GetPagePath(page), this.m_planData.ScopeIdentifier, false, false).FirstOrDefault<FileContainerItem>();
      if (fileContainerItem == null)
        return (Stream) null;
      TeamFoundationFileService service = requestContext.GetService<TeamFoundationFileService>();
      byte[] hashValue;
      long contentLength;
      CompressionType compressionType;
      if (this.m_securedObject == null)
        return service.RetrieveFile(requestContext.Elevate(), (long) fileContainerItem.FileId, false, out hashValue, out contentLength, out compressionType);
      using (requestContext.AllowPublicUserWrites(this.m_securedObject))
        return service.RetrieveFile(requestContext.Elevate(), (long) fileContainerItem.FileId, false, out hashValue, out contentLength, out compressionType);
    }
  }
}
