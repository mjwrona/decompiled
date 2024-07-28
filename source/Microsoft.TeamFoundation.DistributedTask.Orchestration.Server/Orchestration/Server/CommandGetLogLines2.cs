// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.CommandGetLogLines2
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.FileContainer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal sealed class CommandGetLogLines2 : Command
  {
    private TaskLog m_log;
    private long m_endLine;
    private long m_startLine;
    private long m_linesToRead;
    private long m_linesToSkip;
    private TaskLogPage m_currentPage;
    private StreamReader m_pageReader;
    private IEnumerator<TaskLogPage> m_enumerator;
    private TaskOrchestrationPlanReference m_planData;
    private IVssRequestContext m_requestContext;

    public StreamingCollection<string> Lines { get; private set; }

    public CommandGetLogLines2(
      IVssRequestContext requestContext,
      TaskOrchestrationPlanReference planData,
      TaskLog log,
      IList<TaskLogPage> pages)
      : base(requestContext)
    {
      this.m_requestContext = requestContext;
      this.m_log = log ?? throw new ArgumentNullException(nameof (log));
      this.m_startLine = 0L;
      this.m_endLine = log.LineCount;
      this.m_enumerator = pages.GetEnumerator();
      this.m_planData = planData ?? throw new ArgumentNullException(nameof (planData));
      this.Lines = new StreamingCollection<string>((Command) this, 240);
    }

    public void Execute()
    {
      this.ContinueExecution();
      if (!this.IsCacheFull)
        return;
      this.RequestContext.PartialResultsReady();
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
      FileContainerItem fileContainerItem = requestContext.GetService<TeamFoundationFileContainerService>().QueryItems(requestContext, this.m_planData.ContainerId, this.m_log.GetPagePath(page), this.m_planData.ScopeIdentifier, false, false).FirstOrDefault<FileContainerItem>();
      return fileContainerItem == null ? (Stream) null : requestContext.GetService<TeamFoundationFileService>().RetrieveFile(requestContext, (long) fileContainerItem.FileId, false, out byte[] _, out long _, out CompressionType _);
    }
  }
}
