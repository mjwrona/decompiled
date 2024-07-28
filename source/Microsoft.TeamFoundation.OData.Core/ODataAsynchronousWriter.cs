// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataAsynchronousWriter
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Microsoft.OData
{
  public sealed class ODataAsynchronousWriter : IODataOutputInStreamErrorListener
  {
    private readonly ODataRawOutputContext rawOutputContext;
    private readonly IServiceProvider container;
    private bool responseMessageCreated;

    internal ODataAsynchronousWriter(ODataRawOutputContext rawOutputContext)
    {
      this.rawOutputContext = rawOutputContext;
      this.container = rawOutputContext.Container;
      this.rawOutputContext.InitializeRawValueWriter();
    }

    public ODataAsynchronousResponseMessage CreateResponseMessage()
    {
      this.VerifyCanCreateResponseMessage(true);
      return this.CreateResponseMessageImplementation();
    }

    public Task<ODataAsynchronousResponseMessage> CreateResponseMessageAsync()
    {
      this.VerifyCanCreateResponseMessage(false);
      return TaskUtils.GetTaskForSynchronousOperation<ODataAsynchronousResponseMessage>((Func<ODataAsynchronousResponseMessage>) (() => this.CreateResponseMessageImplementation()));
    }

    public void Flush()
    {
      this.VerifyCanFlush(true);
      this.rawOutputContext.Flush();
    }

    public Task FlushAsync()
    {
      this.VerifyCanFlush(false);
      return this.rawOutputContext.FlushAsync();
    }

    void IODataOutputInStreamErrorListener.OnInStreamError()
    {
      this.rawOutputContext.VerifyNotDisposed();
      this.rawOutputContext.TextWriter.Flush();
      throw new ODataException(Strings.ODataAsyncWriter_CannotWriteInStreamErrorForAsync);
    }

    private void ValidateWriterNotDisposed() => this.rawOutputContext.VerifyNotDisposed();

    private void VerifyCallAllowed(bool synchronousCall)
    {
      if (synchronousCall)
      {
        if (!this.rawOutputContext.Synchronous)
          throw new ODataException(Strings.ODataAsyncWriter_SyncCallOnAsyncWriter);
      }
      else if (this.rawOutputContext.Synchronous)
        throw new ODataException(Strings.ODataAsyncWriter_AsyncCallOnSyncWriter);
    }

    private void VerifyCanFlush(bool synchronousCall)
    {
      this.rawOutputContext.VerifyNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
    }

    private void VerifyCanCreateResponseMessage(bool synchronousCall)
    {
      this.ValidateWriterNotDisposed();
      this.VerifyCallAllowed(synchronousCall);
      if (!this.rawOutputContext.WritingResponse)
        throw new ODataException(Strings.ODataAsyncWriter_CannotCreateResponseWhenNotWritingResponse);
      if (this.responseMessageCreated)
        throw new ODataException(Strings.ODataAsyncWriter_CannotCreateResponseMoreThanOnce);
    }

    private ODataAsynchronousResponseMessage CreateResponseMessageImplementation()
    {
      ODataAsynchronousResponseMessage messageForWriting = ODataAsynchronousResponseMessage.CreateMessageForWriting(this.rawOutputContext.OutputStream, new Action<ODataAsynchronousResponseMessage>(this.WriteInnerEnvelope), this.container);
      this.responseMessageCreated = true;
      return messageForWriting;
    }

    private void WriteInnerEnvelope(ODataAsynchronousResponseMessage responseMessage)
    {
      string statusMessage = HttpUtils.GetStatusMessage(responseMessage.StatusCode);
      this.rawOutputContext.TextWriter.WriteLine("{0} {1} {2}", (object) "HTTP/1.1", (object) responseMessage.StatusCode, (object) statusMessage);
      if (responseMessage.Headers != null)
      {
        foreach (KeyValuePair<string, string> header in responseMessage.Headers)
          this.rawOutputContext.TextWriter.WriteLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}: {1}", new object[2]
          {
            (object) header.Key,
            (object) header.Value
          }));
      }
      this.rawOutputContext.TextWriter.WriteLine();
      this.rawOutputContext.TextWriter.Flush();
    }
  }
}
