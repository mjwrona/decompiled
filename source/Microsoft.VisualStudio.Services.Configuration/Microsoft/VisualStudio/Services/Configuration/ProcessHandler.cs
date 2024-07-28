// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.ProcessHandler
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using Microsoft.TeamFoundation.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class ProcessHandler
  {
    private readonly string m_displayArguments;
    private readonly string m_standardInputArguments;
    private readonly ProcessHandlerOptions m_options;
    private readonly ProcessStartInfo m_startInfo;

    public ProcessHandler(string fileName, string arguments, string displayArguments)
      : this(fileName, arguments, displayArguments, ProcessHandlerOptions.None)
    {
    }

    public ProcessHandler(
      string fileName,
      string arguments,
      string displayArguments,
      ProcessHandlerOptions options)
      : this(new ProcessStartInfo()
      {
        FileName = fileName,
        Arguments = arguments
      }, displayArguments, options)
    {
    }

    public ProcessHandler(
      ProcessStartInfo startInfo,
      string displayArguments,
      ProcessHandlerOptions options)
    {
      this.m_startInfo = startInfo;
      this.m_startInfo.UseShellExecute = false;
      this.m_startInfo.RedirectStandardInput = false;
      this.m_startInfo.RedirectStandardError = true;
      this.m_startInfo.RedirectStandardOutput = true;
      this.m_startInfo.CreateNoWindow = true;
      this.m_startInfo.ErrorDialog = false;
      this.m_displayArguments = displayArguments;
      this.m_options = options;
    }

    public ProcessHandler(
      ProcessStartInfo startInfo,
      string displayArguments,
      string standardInputArguments,
      ProcessHandlerOptions options)
      : this(startInfo, displayArguments, options)
    {
      this.m_startInfo.RedirectStandardInput = true;
      this.m_standardInputArguments = standardInputArguments;
    }

    public ProcessOutput Run(ITFLogger logger)
    {
      logger.Info(ConfigurationResources.ProcessStarting((object) this.m_startInfo.FileName, (object) this.m_displayArguments));
      List<OutputLine> output = new List<OutputLine>();
      object outputLock = new object();
      TimeSpan elapsed;
      int exitCode;
      using (Process process = new Process())
      {
        bool processExited = false;
        process.StartInfo = this.m_startInfo;
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += (DataReceivedEventHandler) ((sender, e) =>
        {
          if (e.Data == null)
            return;
          string str = e.Data;
          if (this.m_startInfo.FileName.EndsWith("\\installutil.exe", StringComparison.OrdinalIgnoreCase) && str.IndexOf("password = ", StringComparison.OrdinalIgnoreCase) >= 0)
            str = Regex.Replace(str, "password = .+", "password = [password removed from log]");
          lock (outputLock)
            output.Add(new OutputLine(OutputType.StdOut, str));
        });
        process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, e) =>
        {
          if (e.Data == null)
            return;
          lock (outputLock)
            output.Add(new OutputLine(OutputType.StdErr, e.Data));
        });
        process.Exited += (EventHandler) ((sender, e) => processExited = true);
        Stopwatch stopwatch = Stopwatch.StartNew();
        process.Start();
        if (this.m_startInfo.RedirectStandardInput && !string.IsNullOrEmpty(this.m_standardInputArguments))
        {
          byte[] bytes = Convert.FromBase64String(this.m_standardInputArguments);
          process.StandardInput.WriteLine(Encoding.UTF8.GetString(bytes));
        }
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        int num = 0;
        while (!process.WaitForExit(1000))
        {
          ++num;
          if (num % 30 == 0)
            logger.Info("Waiting on process {0} ({1} seconds elapsed)", (object) process.Id, (object) num);
          if (processExited)
            break;
        }
        process.WaitForExit();
        stopwatch.Stop();
        elapsed = stopwatch.Elapsed;
        exitCode = process.ExitCode;
        logger.Info(ConfigurationResources.ProcessFinished((object) this.m_startInfo.FileName, (object) this.m_displayArguments, (object) process.ExitCode, (object) stopwatch.ElapsedMilliseconds));
      }
      lock (outputLock)
      {
        this.WriteProcessOutput((IEnumerable<OutputLine>) output, logger);
        return new ProcessOutput(exitCode, output.ToArray(), elapsed);
      }
    }

    public Task<ProcessOutput> RunAsync(ITFLogger logger, CancellationToken cancellationToken = default (CancellationToken))
    {
      logger.Info(ConfigurationResources.ProcessStarting((object) this.m_startInfo.FileName, (object) this.m_displayArguments));
      BlockingCollection<OutputLine> output = new BlockingCollection<OutputLine>();
      Process process = new Process()
      {
        StartInfo = this.m_startInfo,
        EnableRaisingEvents = true
      };
      process.OutputDataReceived += (DataReceivedEventHandler) ((sender, eventArgs) =>
      {
        if (eventArgs.Data == null)
          return;
        string str = eventArgs.Data;
        if (this.m_startInfo.FileName.EndsWith("\\installutil.exe", StringComparison.OrdinalIgnoreCase) && str.IndexOf("password = ", StringComparison.OrdinalIgnoreCase) >= 0)
          str = Regex.Replace(str, "password = .+", "password = [password removed from log]");
        output.Add(new OutputLine(OutputType.StdOut, str));
      });
      process.ErrorDataReceived += (DataReceivedEventHandler) ((sender, eventArgs) =>
      {
        if (eventArgs.Data == null)
          return;
        output.Add(new OutputLine(OutputType.StdErr, eventArgs.Data));
      });
      TaskCompletionSource<ProcessOutput> tcs = new TaskCompletionSource<ProcessOutput>();
      cancellationToken.Register((Action) (() =>
      {
        tcs.TrySetCanceled();
        process.Kill();
      }));
      cancellationToken.ThrowIfCancellationRequested();
      Stopwatch stopwatch = Stopwatch.StartNew();
      process.Exited += (EventHandler) ((sender, eventArgs) =>
      {
        stopwatch.Stop();
        logger.Info(ConfigurationResources.ProcessFinished((object) this.m_startInfo.FileName, (object) this.m_displayArguments, (object) process.ExitCode, (object) stopwatch.ElapsedMilliseconds));
        OutputLine[] array = output.ToArray();
        this.WriteProcessOutput((IEnumerable<OutputLine>) array, logger);
        tcs.TrySetResult(new ProcessOutput(process.ExitCode, array, stopwatch.Elapsed));
      });
      process.Start();
      process.BeginOutputReadLine();
      process.BeginErrorReadLine();
      return tcs.Task;
    }

    public static ProcessOutput RunExe(
      string command,
      CommandLineBuilder args,
      ITFLogger logger,
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return ProcessHandler.Perform_RunExe(new ProcessStartInfo()
      {
        FileName = command,
        Arguments = args.ToString()
      }, args.ToHiddenString(), logger, logOptions);
    }

    public static ProcessOutput RunExe(
      string command,
      string args,
      ITFLogger logger,
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return ProcessHandler.Perform_RunExe(new ProcessStartInfo()
      {
        FileName = command,
        Arguments = args
      }, args, logger, logOptions);
    }

    public static ProcessOutput RunExe(
      string filename,
      string arguments,
      string displayArguments,
      ITFLogger logger,
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return ProcessHandler.Perform_RunExe(new ProcessStartInfo()
      {
        FileName = filename,
        Arguments = arguments
      }, displayArguments, logger, logOptions);
    }

    public static ProcessOutput RunExe(
      ProcessStartInfo startInfo,
      string displayArguments,
      ITFLogger logger,
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return ProcessHandler.Perform_RunExe(startInfo, displayArguments, logger, logOptions);
    }

    public static ProcessOutput RunExe(
      ProcessStartInfo startInfo,
      string displayArguments,
      string standardInputArguments,
      ITFLogger logger,
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return ProcessHandler.Perform_RunExe(startInfo, displayArguments, standardInputArguments, logger, logOptions);
    }

    public static Task<ProcessOutput> RunExeAsync(
      ProcessStartInfo startInfo,
      string displayArguments,
      ITFLogger logger,
      CancellationToken cancellationToken = default (CancellationToken),
      ProcessHandlerOptions logOptions = ProcessHandlerOptions.None)
    {
      return new ProcessHandler(startInfo, displayArguments, logOptions).RunAsync(logger, cancellationToken);
    }

    private static ProcessOutput Perform_RunExe(
      ProcessStartInfo startInfo,
      string displayArguments,
      ITFLogger logger,
      ProcessHandlerOptions logOptions)
    {
      return new ProcessHandler(startInfo, displayArguments, logOptions).Run(logger);
    }

    private static ProcessOutput Perform_RunExe(
      ProcessStartInfo startInfo,
      string displayArguments,
      string standardInputArguements,
      ITFLogger logger,
      ProcessHandlerOptions logOptions)
    {
      return new ProcessHandler(startInfo, displayArguments, standardInputArguements, logOptions).Run(logger);
    }

    private void WriteProcessOutput(IEnumerable<OutputLine> outputLines, ITFLogger logger)
    {
      foreach (OutputLine outputLine in outputLines)
      {
        if (outputLine.OutputType == OutputType.StdErr)
        {
          if (this.m_options.HasFlag((Enum) ProcessHandlerOptions.LogErrorsAsWarnings))
            logger.Warning(outputLine.Data);
          else if (this.m_options.HasFlag((Enum) ProcessHandlerOptions.LogErrorsAsInfo))
            logger.Info(outputLine.Data);
          else
            logger.Error(outputLine.Data);
        }
        else
          logger.Info(outputLine.Data);
      }
    }
  }
}
