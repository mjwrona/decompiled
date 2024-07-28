// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.ForwardedSshCommandExtension
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.Ssh.Server.Core;
using SBSSHClient;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public class ForwardedSshCommandExtension
  {
    private const string c_layer = "ForwardedSshCommandExtension";

    public void Execute(
      IVssRequestContext rc,
      GitSshCommandInfo commandInfo,
      IPipeSshChannel srcChannel,
      ISshSession session,
      SshOptions options)
    {
      try
      {
        rc.TraceEnter(23002020, "Ssh", nameof (ForwardedSshCommandExtension), nameof (Execute));
        Stopwatch stopwatch1 = Stopwatch.StartNew();
        ClientTraceData properties = new ClientTraceData();
        properties.Add("Action", (object) "RoutedSshGitCommand");
        properties.Add("Command", (object) commandInfo.StandardizedCommand);
        Stopwatch stopwatch2 = Stopwatch.StartNew();
        string host;
        int port;
        string targetAudience;
        this.ParseClientRequestParameters(rc, out host, out port, out targetAudience);
        properties.Add("ParsingTimeMs", (object) stopwatch2.ElapsedMilliseconds);
        properties.Add("Host", (object) host);
        properties.Add("Port", (object) port);
        properties.Add("Collection", (object) commandInfo.Collection);
        properties.Add("TargetAudience", (object) targetAudience);
        string forwardCredentials = session.AuthenticationProvider.GenerateForwardCredentials(rc, session.SessionUser, targetAudience);
        Stopwatch stopwatch3 = Stopwatch.StartNew();
        EldosSshCommandClient2 sshCommandClient2 = (EldosSshCommandClient2) null;
        try
        {
          TElSSHClient sshClient = TElSSHClientFactory.Create(options, rc.RequestTracer);
          try
          {
            sshCommandClient2 = new EldosSshCommandClient2(rc, sshClient, options);
            sshClient = (TElSSHClient) null;
          }
          finally
          {
            sshClient?.Dispose();
          }
          sshCommandClient2.RunCommand(host, port, commandInfo.Collection, forwardCredentials, commandInfo.StandardizedCommand, (Action<IPipeSshChannel>) (tgtChannel =>
          {
            try
            {
              Task.WaitAll(LinkAsync(srcChannel.DataIn, tgtChannel.DataOut), LinkAsync(srcChannel.ExtendedDataIn, tgtChannel.ExtendedDataOut), LinkAsync(tgtChannel.DataIn, srcChannel.DataOut), LinkAsync(tgtChannel.ExtendedDataIn, srcChannel.ExtendedDataOut));
            }
            catch (AggregateException ex)
            {
              SshAsyncUtil.RethrowNonCanceled(rc.RequestTracer, 23002021, nameof (ForwardedSshCommandExtension), ex);
            }
          }));
        }
        finally
        {
          sshCommandClient2?.Dispose();
          properties.Add("RoutedCommandExecutionTimeMs", (object) stopwatch3.ElapsedMilliseconds);
          properties.Add("ElapsedTimeMs", (object) stopwatch1.ElapsedMilliseconds);
          rc.GetService<ClientTraceService>().Publish(rc, "SSH", "SSH_ARR", properties);
        }
      }
      finally
      {
        rc.TraceLeave(23002021, "Ssh", nameof (ForwardedSshCommandExtension), nameof (Execute));
      }

      static async Task LinkAsync(PipeReader from, PipeWriter to)
      {
        try
        {
          TaskCompletionSource<ReadResult> toReaderCompletedTaskSrc = new TaskCompletionSource<ReadResult>();
          to.OnReaderCompleted((Action<Exception, object>) ((ex, state) =>
          {
            if (ex != null)
              ((TaskCompletionSource<ReadResult>) state).TrySetException(ex);
            else
              ((TaskCompletionSource<ReadResult>) state).TrySetResult(new ReadResult());
          }), (object) toReaderCompletedTaskSrc);
          ReadResult readResult;
          do
          {
            Task<ReadResult> task = await Task.WhenAny<ReadResult>(from.ReadAsync().AsTask(), toReaderCompletedTaskSrc.Task);
            if (task == toReaderCompletedTaskSrc.Task)
            {
              to.Complete();
              from.Complete();
              return;
            }
            readResult = task.Result;
            ReadOnlySequence<byte> readOnlySequence = !readResult.IsCanceled ? readResult.Buffer : throw new System.OperationCanceledException("ReadResult");
            foreach (ReadOnlyMemory<byte> readOnlyMemory in readOnlySequence)
            {
              FlushResult flushResult = await to.WriteAsync(readOnlyMemory, new CancellationToken());
              if (flushResult.IsCanceled)
                throw new System.OperationCanceledException("FlushResult");
              if (flushResult.IsCompleted)
              {
                to.Complete();
                from.Complete();
                return;
              }
            }
            PipeReader pipeReader = from;
            readOnlySequence = readResult.Buffer;
            SequencePosition end = readOnlySequence.End;
            pipeReader.AdvanceTo(end);
          }
          while (!readResult.IsCompleted);
          from.Complete();
          to.Complete();
          toReaderCompletedTaskSrc = (TaskCompletionSource<ReadResult>) null;
          readResult = new ReadResult();
        }
        catch (Exception ex)
        {
          from.Complete(ex);
          to.Complete(ex);
          throw;
        }
      }
    }

    private void ParseClientRequestParameters(
      IVssRequestContext requestContext,
      out string host,
      out int port,
      out string targetAudience)
    {
      HostProxyData hostProxyData;
      if (!requestContext.TryGetItem<HostProxyData>(RequestContextItemsKeys.HostProxyData, out hostProxyData))
        throw ForwardedSshCommandParsingException.GetNewException("Couldn't read HostProxyData from requestContext.", ForwardedSshCommandParsingException.FieldType.ProxyData);
      Uri uri = new Uri(hostProxyData.TargetInstanceUrl);
      host = uri.Host;
      TeamFoundationExecutionEnvironment executionEnvironment = requestContext.ExecutionEnvironment;
      port = !executionEnvironment.IsDevFabricDeployment ? 22 : (!hostProxyData.TargetPublicUrl.ToLower().Contains("tfs1") ? (!hostProxyData.TargetPublicUrl.ToLower().Contains("tfs2") ? -1 : 26) : 24);
      targetAudience = hostProxyData.TargetInstanceId.ToString();
      if (string.IsNullOrEmpty(targetAudience))
        throw ForwardedSshCommandParsingException.GetNewException(targetAudience, ForwardedSshCommandParsingException.FieldType.TargetAudience);
    }
  }
}
