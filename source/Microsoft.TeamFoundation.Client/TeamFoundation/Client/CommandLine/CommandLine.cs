// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.CommandLine
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Diff;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Client;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.OAuth;
using Microsoft.VisualStudio.Services.Operations;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class CommandLine
  {
    public static readonly string NoPromptEnvironmentVariable = "TFSVC_NOPROMPT";
    public static readonly int ExpandEnvironmentVariablesMaximumLength = 32764;
    private static NoPromptState s_noPrompt = Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsEnvVarSet(Microsoft.TeamFoundation.Client.CommandLine.CommandLine.NoPromptEnvironmentVariable) || !Environment.UserInteractive ? NoPromptState.NoPrompt : NoPromptState.NotSpecified;
    private static readonly Encoding s_inputEncoding = (Encoding) null;
    private static readonly Encoding s_outputEncoding = (Encoding) null;
    private const string m_exitMarker = "%<<ENDOFSTREAM>>%";
    private const int c_maxLineBufferSize = 4096;
    private string m_commandLine;
    private bool m_showExitCode;
    private bool m_showExitMarker;
    private Dictionary<string, TfsTeamProjectCollection> m_tfServers = new Dictionary<string, TfsTeamProjectCollection>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private bool m_offline;
    private string[] m_helpCommandArgs;
    private TraceSwitch m_traceSwitch;
    private Stopwatch m_mainTimer;

    static CommandLine()
    {
      string environmentVariable1 = Environment.GetEnvironmentVariable("TF_INTERNAL_OUTPUT_CP");
      int result1;
      if (environmentVariable1 != null && int.TryParse(environmentVariable1, out result1))
        Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_outputEncoding = result1 == 65001 ? (Encoding) new UTF8Encoding(false) : Encoding.GetEncoding(result1);
      string environmentVariable2 = Environment.GetEnvironmentVariable("TF_INTERNAL_INTPUT_CP");
      int result2;
      if (environmentVariable2 == null || !int.TryParse(environmentVariable2, out result2))
        return;
      Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_inputEncoding = Encoding.GetEncoding(result2);
    }

    protected CommandLine()
    {
      this.m_commandLine = Environment.CommandLine;
      if (Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_outputEncoding != null)
        Console.OutputEncoding = Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_outputEncoding;
      if (Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_inputEncoding != null)
        Console.InputEncoding = Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_inputEncoding;
      Console.SetIn((TextReader) new StreamReader(Console.OpenStandardInput(4096), Console.InputEncoding, false, 4096));
    }

    protected void Run(ref string[] args)
    {
      try
      {
        ConsoleHost.Initialize(this.GetApplicationIcon());
        args = StringUtil.ParseCommandLine(this.CommandLineArgs);
        this.TraceStartUp();
        try
        {
          Console.CancelKeyPress += new ConsoleCancelEventHandler(this.Console_CancelKeyPress);
          this.Initialize(args);
          this.RunCommand(args);
          Console.CancelKeyPress -= new ConsoleCancelEventHandler(this.Console_CancelKeyPress);
        }
        catch (System.OperationCanceledException ex)
        {
          ConsoleHost.Canceled = false;
          UIHost.WriteError(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.CommandCanceled());
          ConsoleHost.Canceled = true;
        }
        finally
        {
          this.Shutdown();
        }
        this.TraceEnding();
      }
      catch (System.OperationCanceledException ex)
      {
      }
      catch (TypeInitializationException ex)
      {
        if (ex.InnerException is ConfigurationErrorsException)
          Console.WriteLine(Microsoft.TeamFoundation.Client.Internal.ClientResources.ConfigurationErrorsEncountered((object) ex));
        else
          throw;
      }
      catch (ConfigurationErrorsException ex)
      {
        Console.WriteLine(Microsoft.TeamFoundation.Client.Internal.ClientResources.ConfigurationErrorsEncountered((object) ex));
      }
    }

    protected TraceSwitch Tracing
    {
      get
      {
        if (this.m_traceSwitch == null)
          this.m_traceSwitch = new TraceSwitch("CommandSwitch", nameof (CommandLine));
        return this.m_traceSwitch;
      }
      set => this.m_traceSwitch = value;
    }

    protected bool TraceErrors => this.Tracing != null && this.Tracing.TraceError;

    protected bool TraceInformation => this.Tracing != null && this.Tracing.TraceInfo;

    protected bool TraceWarnings => this.Tracing != null && this.Tracing.TraceWarning;

    protected bool TraceVerbose => this.Tracing != null && this.Tracing.TraceVerbose;

    protected bool TraceOff => this.Tracing == null || this.Tracing.Level == TraceLevel.Off;

    private void TraceStartUp()
    {
      this.m_mainTimer = (Stopwatch) null;
      if (!this.TraceErrors)
        return;
      this.m_mainTimer = new Stopwatch();
      this.m_mainTimer.Start();
      Assembly assembly = this.GetType().Assembly;
    }

    private void TraceEnding()
    {
      Stopwatch mainTimer1 = this.m_mainTimer;
      Stopwatch mainTimer2 = this.m_mainTimer;
    }

    private void TraceRunCommandStart(string lineTag, string pseudoCmdPrompt)
    {
    }

    private void TraceRunCommandEnd(string lineTag, string[] args, long elapsedMilliseconds)
    {
    }

    public void LogException(Exception e)
    {
    }

    protected void RunCommand(string[] args)
    {
      bool flag = this.m_showExitCode;
      bool showExitCode = false;
      this.m_helpCommandArgs = args;
      try
      {
        if (args.Length != 0)
        {
          string[] strArray = new string[args.Length - 1];
          Array.Copy((Array) args, 1, (Array) strArray, 0, strArray.Length);
          if (args[0][0] == '@')
          {
            flag = false;
            this.RunMultipleCommands(args[0], strArray);
          }
          else
            this.RunCommand(args[0], strArray, out showExitCode);
        }
        else
          this.RunCommand(string.Empty, args, out showExitCode);
      }
      catch (System.OperationCanceledException ex)
      {
        throw;
      }
      catch (Exception ex)
      {
        this.LogException(ex);
        switch (ex)
        {
          case NullReferenceException _:
          case ArgumentNullException _:
            throw;
          default:
            UIHost.WriteError(this.LogCategory, ex.Message);
            Environment.ExitCode = !(ex is Command.UnrecognizedCommandException) ? (!(ex is Command.BlockedException) ? 100 : 3) : 2;
            break;
        }
      }
      if (flag | showExitCode)
        UIHost.WriteLine(LogCategory.General, Microsoft.TeamFoundation.Client.Internal.ClientResources.ExitCode((object) Environment.ExitCode));
      if (!this.m_showExitMarker)
        return;
      UIHost.WriteLine(LogCategory.General, "%<<ENDOFSTREAM>>%");
      UIHost.WriteError(LogCategory.General, "%<<ENDOFSTREAM>>%");
    }

    protected void UpdateHelpCommandArgs(bool commandGroupSpecified)
    {
      if (!commandGroupSpecified)
        return;
      this.m_helpCommandArgs = ((IEnumerable<string>) this.m_helpCommandArgs).Where<string>((Func<string, int, bool>) ((value, index) => index != 0)).ToArray<string>();
    }

    protected virtual void Initialize(string[] args)
    {
    }

    protected abstract void RunCommand(string commandName, string[] args, out bool showExitCode);

    protected abstract Icon GetApplicationIcon();

    protected abstract LogCategory LogCategory { get; }

    protected virtual void Shutdown()
    {
      UIHost.Shutdown();
      foreach (TfsConnection tfsConnection in this.m_tfServers.Values)
        tfsConnection.Dispose();
    }

    private void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e) => this.CancelKeyPressed(e);

    protected virtual void CancelKeyPressed(ConsoleCancelEventArgs e) => ConsoleHost.Canceled = true;

    protected void RunMultipleCommands(string arg, string[] parameters)
    {
      Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt = NoPromptState.NoPrompt;
      if (arg.Length == 1)
      {
        this.RunCommandsFromStream(Console.In, parameters, "<stdin>");
      }
      else
      {
        string str = arg.Substring(1);
        using (StreamReader stream = new StreamReader(str, FileTypeUtil.TryDetermineTextEncoding(str), false, 4096))
          this.RunCommandsFromStream((TextReader) stream, parameters, str);
      }
    }

    protected void RunCommandsFromStream(TextReader stream, string[] parameters, string fileName)
    {
      StringBuilder stringBuilder = new StringBuilder();
      int num = 0;
      List<string> stringList = new List<string>();
      while (true)
      {
        string[] strArray;
        bool flag;
        string str1;
        do
        {
          strArray = (string[]) null;
          string line = stream.ReadLine();
          flag = false;
          if (line != null || stringList.Count != 0)
          {
            if (line == null)
            {
              flag = true;
              goto label_18;
            }
            else
            {
              ++num;
              str1 = this.ExpandVariables(line, parameters);
              strArray = StringUtil.ParseCommandLine("\"" + Environment.GetCommandLineArgs()[0] + "\" " + str1);
              if (strArray.Length != 0 && (strArray.Length != 1 || strArray[0].Length != 0) && !strArray[0].StartsWith("#", StringComparison.Ordinal))
                goto label_7;
            }
          }
          else
            goto label_27;
        }
        while (stringList.Count <= 0);
        flag = true;
label_7:
        if (!flag)
        {
          string str2 = strArray[strArray.Length - 1];
          if (str2.EndsWith("\\", StringComparison.Ordinal))
          {
            for (int index = 0; index < strArray.Length; ++index)
            {
              if (index != strArray.Length - 1)
                stringList.Add(strArray[index]);
              else if (str2.Length > 1)
                stringList.Add(str2.Substring(0, str2.Length - 1));
            }
            int length = str1.LastIndexOf('\\');
            stringBuilder.Append(str1.Substring(0, length));
            stringBuilder.Append(" ");
            continue;
          }
          stringBuilder.Append(str1);
        }
label_18:
        if (flag || stringList.Count > 0)
        {
          if (!flag)
            stringList.AddRange((IEnumerable<string>) strArray);
          strArray = stringList.ToArray();
          stringList.Clear();
        }
        string str3 = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Environment.NewLine + "{0}> {1}", (object) Environment.CurrentDirectory, (object) string.Join(" ", strArray));
        UIHost.WriteLine(this.LogCategory, str3);
        string lineTag = fileName + ":" + num.ToString();
        this.TraceRunCommandStart(lineTag, str3);
        Stopwatch stopwatch = Stopwatch.StartNew();
        try
        {
          if (Environment.GetEnvironmentVariable("DD_SUITES") == null)
            this.RunCommand(strArray);
          else if (!this.RunExtendedCommand(strArray, stringBuilder.ToString(), parameters, fileName))
            this.RunCommand(strArray);
        }
        catch (System.OperationCanceledException ex)
        {
          throw;
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case ApplicationException _:
            case IOException _:
              UIHost.WriteError(this.LogCategory, ex.Message);
              break;
            default:
              throw;
          }
        }
        this.TraceRunCommandEnd(lineTag, strArray, stopwatch.ElapsedMilliseconds);
        stringBuilder.Length = 0;
      }
label_27:;
    }

    protected virtual bool RunExtendedCommand(
      string[] args,
      string fullCommandLine,
      string[] parameters,
      string fileName)
    {
      string upperInvariant = args[0].ToUpperInvariant();
      if (upperInvariant != null)
      {
        switch (upperInvariant.Length)
        {
          case 2:
            switch (upperInvariant[0])
            {
              case 'C':
                if (upperInvariant == "CD")
                  break;
                goto label_69;
              case 'M':
                if (upperInvariant == "MD")
                  goto label_54;
                else
                  goto label_69;
              case 'R':
                if (upperInvariant == "RD")
                  goto label_57;
                else
                  goto label_69;
              default:
                goto label_69;
            }
            break;
          case 3:
            switch (upperInvariant[0])
            {
              case 'C':
                if (upperInvariant == "CMD")
                {
                  this.RunOSCommand(fullCommandLine);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'R':
                if (upperInvariant == "REM")
                  goto label_70;
                else
                  goto label_69;
              default:
                goto label_69;
            }
          case 4:
            switch (upperInvariant[1])
            {
              case 'O':
                if (upperInvariant == "COPY")
                {
                  this.CheckNumArgs(args, 2);
                  FileSpec.CopyFile(Path.GetFullPath(args[1]), Path.GetFullPath(args[2]));
                  goto label_70;
                }
                else
                  goto label_69;
              case 'R':
                if (upperInvariant == "TREE")
                {
                  this.CheckNumArgs(args, 1);
                  this.DisplayTree(args[1], 0, new StringBuilder(), false);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'U':
                if (upperInvariant == "QUIT")
                  break;
                goto label_69;
              case 'X':
                if (upperInvariant == "EXIT")
                  break;
                goto label_69;
              case 'Y':
                if (upperInvariant == "TYPE")
                {
                  this.CheckNumArgs(args, 1);
                  this.DisplayFileContents(args[1]);
                  goto label_70;
                }
                else
                  goto label_69;
              default:
                goto label_69;
            }
            this.CheckNumArgs(args, 0);
            throw new System.OperationCanceledException();
          case 5:
            switch (upperInvariant[1])
            {
              case 'A':
                if (upperInvariant == "PAUSE")
                {
                  this.CheckNumArgs(args, 0);
                  UIHost.Write(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.PressEnterKey());
                  ConsoleHost.ReadLine();
                  goto label_70;
                }
                else
                  goto label_69;
              case 'H':
                if (upperInvariant == "CHDIR")
                  break;
                goto label_69;
              case 'K':
                if (upperInvariant == "MKDIR")
                  goto label_54;
                else
                  goto label_69;
              case 'L':
                if (upperInvariant == "SLEEP")
                {
                  this.CheckNumArgs(args, 1);
                  this.Sleep(args[1]);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'M':
                if (upperInvariant == "RMDIR")
                  goto label_57;
                else
                  goto label_69;
              case 'O':
                if (upperInvariant == "TOUCH")
                {
                  this.CheckNumArgs(args, 1, 2);
                  this.CreateFile(args);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'R':
                if (upperInvariant == "ERASE")
                {
                  this.CheckNumArgs(args, 1);
                  FileSpec.DeleteFile(Path.GetFullPath(args[1]));
                  goto label_70;
                }
                else
                  goto label_69;
              case 'V':
                if (upperInvariant == "MVDIR")
                  goto label_55;
                else
                  goto label_69;
              default:
                goto label_69;
            }
            break;
          case 6:
            switch (upperInvariant[1])
            {
              case 'E':
                if (upperInvariant == "SETENV")
                {
                  this.CheckNumArgs(args, 2);
                  Environment.SetEnvironmentVariable(args[1], args[2]);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'P':
                if (upperInvariant == "APPEND")
                {
                  this.CheckNumArgs(args, 2, 3);
                  this.AppendLine(args);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'T':
                if (upperInvariant == "ATTRIB")
                {
                  this.CheckNumArgs(args, 2);
                  this.AttribFile(args);
                  goto label_70;
                }
                else
                  goto label_69;
              default:
                goto label_69;
            }
          case 7:
            if (upperInvariant == "MOVEDIR")
              goto label_55;
            else
              goto label_69;
          case 9:
            switch (upperInvariant[0])
            {
              case 'C':
                if (upperInvariant == "CNT_FILES")
                {
                  this.CheckNumArgs(args, 1);
                  UIHost.WriteLine(this.LogCategory, string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}", (object) this.CountFiles(args[1])));
                  goto label_70;
                }
                else
                  goto label_69;
              case 'G':
                if (upperInvariant == "GO_ONLINE")
                {
                  this.m_offline = false;
                  this.m_tfServers.Clear();
                  goto label_70;
                }
                else
                  goto label_69;
              default:
                goto label_69;
            }
          case 10:
            if (upperInvariant == "GO_OFFLINE")
            {
              this.m_offline = true;
              this.m_tfServers.Clear();
              goto label_70;
            }
            else
              goto label_69;
          case 11:
            if (upperInvariant == "SETNOPROMPT")
            {
              this.CheckNumArgs(args, 1);
              this.SetNoPrompt(args[1]);
              goto label_70;
            }
            else
              goto label_69;
          case 12:
            switch (upperInvariant[0])
            {
              case 'S':
                if (upperInvariant == "SHOWEXITCODE")
                {
                  this.CheckNumArgs(args, 1);
                  this.SetShowExitCode(args[1]);
                  goto label_70;
                }
                else
                  goto label_69;
              case 'T':
                if (upperInvariant == "TREEWITHDATE")
                {
                  this.CheckNumArgs(args, 1);
                  this.DisplayTree(args[1], 0, new StringBuilder(), true);
                  goto label_70;
                }
                else
                  goto label_69;
              default:
                goto label_69;
            }
          case 14:
            switch (upperInvariant[0])
            {
              case 'D':
                if (upperInvariant == "DEBUGGER.BREAK")
                {
                  Debugger.Break();
                  goto label_70;
                }
                else
                  goto label_69;
              case 'S':
                if (upperInvariant == "SHOWEXITMARKER")
                {
                  this.CheckNumArgs(args, 1);
                  this.SetShowExitMarker(args[1]);
                  goto label_70;
                }
                else
                  goto label_69;
              default:
                goto label_69;
            }
          case 15:
            if (upperInvariant == "DEBUGGER.LAUNCH")
            {
              Debugger.Launch();
              goto label_70;
            }
            else
              goto label_69;
          case 17:
            if (upperInvariant == "RENAMETEAMPROJECT")
            {
              this.CheckNumArgs(args, 2);
              this.RenameTeamProject(args[1], args[2]);
              goto label_70;
            }
            else
              goto label_69;
          default:
            goto label_69;
        }
        this.CheckNumArgs(args, 1);
        Environment.CurrentDirectory = args[1];
        goto label_70;
label_54:
        this.CheckNumArgs(args, 1);
        Directory.CreateDirectory(Path.GetFullPath(args[1]));
        goto label_70;
label_55:
        this.CheckNumArgs(args, 2);
        Directory.Move(args[1], args[2]);
        goto label_70;
label_57:
        this.CheckNumArgs(args, 1);
        if (Microsoft.TeamFoundation.Common.Internal.NativeMethods.SHFileOperationDelete(Path.GetFullPath(args[1]), false) != 0)
          FileSpec.DeleteDirectory(Path.GetFullPath(args[1]), true);
label_70:
        return true;
      }
label_69:
      return false;
    }

    private void RenameTeamProject(string origName, string newName)
    {
      TfsTeamProjectCollection projectCollection = ((IEnumerable<TfsTeamProjectCollection>) this.GetConnectedServers()).First<TfsTeamProjectCollection>();
      ProjectHttpClient client1 = projectCollection.GetClient<ProjectHttpClient>();
      TeamProject result1 = client1.GetProject(origName).Result;
      result1.Name = newName;
      result1.Capabilities = (Dictionary<string, Dictionary<string, string>>) null;
      result1.Links = (ReferenceLinks) null;
      result1.Url = (string) null;
      result1.State = ProjectState.Unchanged;
      OperationReference result2 = client1.UpdateProject(result1.Id, result1).Result;
      OperationsHttpClient client2 = projectCollection.GetClient<OperationsHttpClient>();
      for (int index = 0; index < 200; ++index)
      {
        Operation result3 = client2.GetOperation(result2.Id).Result;
        if (result3.Status != OperationStatus.InProgress && result3.Status != OperationStatus.Queued)
        {
          Console.WriteLine("Project Rename Status: {0}", (object) result3.Status);
          return;
        }
        Thread.Sleep(100);
      }
      throw new Exception("The team project rename didn't finish after 20 seconds.  Is your job agent running?");
    }

    protected string ExpandVariables(string line, string[] parameters)
    {
      StringBuilder stringBuilder;
      if (line.Length > Microsoft.TeamFoundation.Client.CommandLine.CommandLine.ExpandEnvironmentVariablesMaximumLength)
      {
        stringBuilder = new StringBuilder(line);
        if (line.IndexOf('%') != -1)
          UIHost.WriteWarning(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.CommandFileLineTooLongWarning((object) Microsoft.TeamFoundation.Client.CommandLine.CommandLine.ExpandEnvironmentVariablesMaximumLength));
      }
      else
        stringBuilder = new StringBuilder(Environment.ExpandEnvironmentVariables(line));
      int length = parameters.Length;
      while (length-- > 0)
        stringBuilder.Replace("%" + (length + 1).ToString(), parameters[length]);
      return stringBuilder.ToString();
    }

    protected void CheckNumArgs(string[] args, int numArgs)
    {
      if (args.Length != numArgs + 1)
        throw new ApplicationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.WrongNumberOfArgsForBuiltIn((object) args[0]));
    }

    protected void CheckNumArgs(string[] args, int minArgs, int maxArgs)
    {
      if (args.Length < minArgs + 1 || args.Length > maxArgs + 1)
        throw new ApplicationException(Microsoft.TeamFoundation.Client.Internal.ClientResources.WrongNumberOfArgsForBuiltIn((object) args[0]));
    }

    protected void DisplayTree(string path, int level, StringBuilder prefix, bool showDate)
    {
      if (level == 0)
        path = FileSpec.GetFullPath(path);
      prefix.Length = level * 2;
      UIHost.WriteLine(this.LogCategory, prefix.Length != 0 ? prefix.ToString(0, prefix.Length - 1) + "+-+" + FileSpec.GetFileName(path) : string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0}:", (object) path));
      string[] files = Directory.GetFiles(path);
      string[] directories = Directory.GetDirectories(path);
      List<string> stringList = new List<string>();
      foreach (string path1 in directories)
      {
        DirectoryInfo directoryInfo = new DirectoryInfo(path1);
        if (!directoryInfo.Exists || (directoryInfo.Attributes & System.IO.FileAttributes.Hidden) != System.IO.FileAttributes.Hidden)
          stringList.Add(path1);
      }
      string message1 = prefix?.ToString() + (stringList.Count > 0 ? " | " : "   ");
      foreach (string path2 in files)
      {
        string message2 = message1 + FileSpec.GetFileName(path2);
        if (showDate)
          message2 = message2 + " " + System.IO.File.GetLastWriteTime(path2).ToString("F");
        UIHost.WriteLine(this.LogCategory, message2);
      }
      if (files.Length != 0 && stringList.Count > 0)
        UIHost.WriteLine(this.LogCategory, message1);
      prefix.Append(" |");
      for (int index = 0; index < stringList.Count; ++index)
      {
        if (index == stringList.Count - 1 && prefix.Length > 0)
        {
          prefix[level * 2] = ' ';
          prefix[level * 2 + 1] = ' ';
        }
        this.DisplayTree(stringList[index], level + 1, prefix, showDate);
      }
    }

    protected int CountFiles(string folderPath)
    {
      int num1 = 0;
      folderPath = FileSpec.GetFullPath(folderPath);
      string[] files = Directory.GetFiles(folderPath);
      int num2 = num1 + files.Length;
      foreach (string directory in Directory.GetDirectories(folderPath))
        num2 += this.CountFiles(directory);
      return num2;
    }

    protected void DisplayFileContents(string fileName)
    {
      fileName = Path.GetFullPath(fileName);
      if (!Directory.Exists(Path.GetDirectoryName(fileName)))
        Directory.GetFiles(fileName);
      using (StreamReader streamReader = new StreamReader(fileName, Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_outputEncoding ?? Encoding.Default, true, 4096))
        UIHost.Write(LogCategory.General, streamReader.ReadToEnd());
      UIHost.WriteLine(LogCategory.General);
    }

    protected void CreateFile(string[] args)
    {
      using (StreamWriter streamWriter = new StreamWriter(args[1]))
      {
        if (args.Length != 3)
          return;
        streamWriter.WriteLine(args[2]);
      }
    }

    protected void Sleep(string arg) => Thread.Sleep(int.Parse(arg, (IFormatProvider) CultureInfo.InvariantCulture) * 1000);

    protected void AttribFile(string[] args)
    {
      string path = args[2];
      System.IO.FileAttributes attributes = System.IO.File.GetAttributes(path);
      if (string.Equals(args[1], "-R", StringComparison.OrdinalIgnoreCase))
        attributes &= ~System.IO.FileAttributes.ReadOnly;
      else if (string.Equals(args[1], "+R", StringComparison.OrdinalIgnoreCase))
        attributes |= System.IO.FileAttributes.ReadOnly;
      System.IO.File.SetAttributes(path, attributes);
    }

    protected void AppendLine(string[] args)
    {
      if (string.Equals(args[1], "/front", StringComparison.OrdinalIgnoreCase) || string.Equals(args[1], "-front", StringComparison.OrdinalIgnoreCase))
      {
        string path = args[2];
        string str1 = args[3];
        string str2 = string.Empty;
        if (System.IO.File.Exists(path))
        {
          using (StreamReader streamReader = new StreamReader(path))
            str2 = streamReader.ReadToEnd();
        }
        using (StreamWriter streamWriter = new StreamWriter(path))
        {
          streamWriter.WriteLine(str1);
          streamWriter.Write(str2);
        }
      }
      else
      {
        string path = args[1];
        string str = args[2];
        using (StreamWriter streamWriter = new StreamWriter(path, true))
          streamWriter.WriteLine(str);
      }
    }

    protected void RunOSCommand(string commandLine)
    {
      int startPos = 0;
      StringUtil.ParseCommandLineArgument(commandLine, ref startPos);
      System.Diagnostics.Process process = System.Diagnostics.Process.Start(new ProcessStartInfo(StringUtil.ParseCommandLineArgument(commandLine, ref startPos), commandLine.Substring(startPos))
      {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        RedirectStandardError = true
      });
      string end1 = process.StandardOutput.ReadToEnd();
      if (!string.IsNullOrEmpty(end1))
        UIHost.WriteLine(this.LogCategory, end1);
      string end2 = process.StandardError.ReadToEnd();
      if (!string.IsNullOrEmpty(end2))
        UIHost.WriteError(this.LogCategory, end2);
      process.WaitForExit();
    }

    public TfsTeamProjectCollection[] GetConnectedServers()
    {
      TfsTeamProjectCollection[] array = new TfsTeamProjectCollection[this.m_tfServers.Count];
      this.m_tfServers.Values.CopyTo(array, 0);
      return array;
    }

    private static string CanonicalizeTfsName(string tfsName)
    {
      try
      {
        return TfsTeamProjectCollection.GetFullyQualifiedUriForName(tfsName).AbsoluteUri;
      }
      catch (TeamFoundationInvalidServerNameException ex)
      {
        return tfsName;
      }
    }

    public TfsTeamProjectCollection CreateTfs(string tfsName, Command command)
    {
      string userName;
      string password;
      command.GetUserNameAndPassword(out userName, out password);
      string impersonationUserName;
      command.GetImpersonationUserName(out impersonationUserName);
      Options.Value obj = Options.Value.Ntlm;
      if (!this.m_offline && Environment.GetEnvironmentVariable("L2_TRUN") != null)
      {
        obj = Options.Value.OAuth;
        if (string.IsNullOrEmpty(userName))
        {
          userName = Environment.GetEnvironmentVariable("L2_USERNAME");
          password = Environment.GetEnvironmentVariable("L2_PASSWORD");
        }
      }
      if (command.IsOptionSpecified(Options.ID.LoginType))
      {
        obj = command.ParseOption(Options.ID.LoginType, new Options.Value[3]
        {
          Options.Value.Ntlm,
          Options.Value.OAuth,
          Options.Value.ServiceIdentity
        }, Options.Value.Ntlm);
        if (Options.Value.ServiceIdentity == obj && string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
          throw new Command.ArgumentListException(TFCommonResources.UserNameAndPasswordRequiredForLoginTypeServiceIdentity());
        if (Options.Value.OAuth == obj && string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
          throw new Command.ArgumentListException(TFCommonResources.UserNameAndPasswordRequiredForLoginTypeOAuth());
      }
      string str = Microsoft.TeamFoundation.Client.CommandLine.CommandLine.CanonicalizeTfsName(tfsName);
      string key = (userName == null ? "<null>" : userName) + (impersonationUserName == null ? "<noimpers>" : impersonationUserName) + obj.ToString() + "<>" + str;
      TfsTeamProjectCollection tfs;
      if (this.m_tfServers.ContainsKey(key))
      {
        tfs = this.m_tfServers[key];
      }
      else
      {
        Uri qualifiedUriForName = TfsTeamProjectCollection.GetFullyQualifiedUriForName(tfsName);
        VssCredentials credentials1 = VssClientCredentials.LoadCachedCredentials(qualifiedUriForName, false, CredentialPromptType.DoNotPrompt);
        if (!string.IsNullOrEmpty(password))
        {
          if (obj == Options.Value.ServiceIdentity)
            credentials1 = (VssCredentials) new VssClientCredentials(new Microsoft.VisualStudio.Services.Common.WindowsCredential(false), (Microsoft.VisualStudio.Services.Common.FederatedCredential) new VssServiceIdentityCredential(userName, password));
          else if (obj == Options.Value.OAuth)
            credentials1 = (VssCredentials) new VssClientCredentials(new Microsoft.VisualStudio.Services.Common.WindowsCredential(false), (Microsoft.VisualStudio.Services.Common.FederatedCredential) new VssOAuthAccessTokenCredential(password));
        }
        else if (!command.IsNoPromptSpecified())
          credentials1.PromptType = CredentialPromptType.PromptIfNeeded;
        if (obj == Options.Value.Ntlm)
        {
          ICredentials credentials2 = (ICredentials) this.GetCredentials(userName, password);
          if (credentials2 != null)
            credentials1.Windows.Credentials = credentials2;
        }
        tfs = new TfsTeamProjectCollection(qualifiedUriForName, credentials1);
        if (!string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(password))
          tfs.Authenticate();
        if (impersonationUserName != null)
          tfs = new TfsTeamProjectCollection(tfs.Uri, tfs.ClientCredentials, (tfs.GetService<IIdentityManagementService>().ReadIdentity(IdentitySearchFactor.General, impersonationUserName, MembershipQuery.None, ReadIdentityOptions.None) ?? throw new IdentityNotFoundException(TFCommonResources.IdentityNotFoundException((object) impersonationUserName))).Descriptor);
        this.m_tfServers[key] = tfs;
      }
      return tfs;
    }

    private NetworkCredential GetCredentials(string userName, string password)
    {
      if (this.m_offline && Environment.GetEnvironmentVariable("DD_SUITES") != null)
        return new NetworkCredential("qLkxRAQzrZEA", "sPPVgFwCILva", "LFrjnDqkwehT");
      if (string.IsNullOrEmpty(userName))
        return CredentialCache.DefaultNetworkCredentials;
      string domain;
      UserNameUtil.Parse(UserNameUtil.Complete(userName), out userName, out domain);
      return new NetworkCredential(userName, password, domain);
    }

    public static bool IsEnvVarSet(string envVar) => VssStringComparer.EnvVar.Equals(Environment.GetEnvironmentVariable(envVar), "1");

    private void SetShowExitCode(string setting)
    {
      if (string.Equals(setting, "on", StringComparison.OrdinalIgnoreCase))
        this.m_showExitCode = true;
      else if (string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase))
        this.m_showExitCode = false;
      else
        UIHost.WriteError(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.CouldNotParseParameter((object) "showexitcode", (object) setting));
    }

    private void SetShowExitMarker(string setting)
    {
      if (string.Equals(setting, "on", StringComparison.OrdinalIgnoreCase))
        this.m_showExitMarker = true;
      else if (string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase))
        this.m_showExitMarker = false;
      else
        UIHost.WriteError(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.CouldNotParseParameter((object) "showexitmarker", (object) setting));
    }

    public virtual string CommandLineArgs
    {
      get => this.m_commandLine;
      set => this.m_commandLine = value;
    }

    public bool IsNoPromptSet() => Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt == NoPromptState.NoPrompt;

    public NoPromptState NoPrompt => Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt;

    private void SetNoPrompt(string setting)
    {
      if (string.Equals(setting, bool.TrueString, StringComparison.OrdinalIgnoreCase) || string.Equals(setting, "on", StringComparison.OrdinalIgnoreCase) || string.Equals(setting, "1", StringComparison.OrdinalIgnoreCase))
        Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt = NoPromptState.NoPrompt;
      else if (string.Equals(setting, bool.FalseString, StringComparison.OrdinalIgnoreCase) || string.Equals(setting, "off", StringComparison.OrdinalIgnoreCase) || string.Equals(setting, "0", StringComparison.OrdinalIgnoreCase))
        Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt = NoPromptState.Prompt;
      else if (string.Equals(setting, "default", StringComparison.OrdinalIgnoreCase))
        Microsoft.TeamFoundation.Client.CommandLine.CommandLine.s_noPrompt = NoPromptState.NotSpecified;
      else
        UIHost.WriteError(this.LogCategory, Microsoft.TeamFoundation.Client.Internal.ClientResources.CouldNotParseParameter((object) "noprompt", (object) setting));
    }

    public bool HelpRequested()
    {
      if (this.m_helpCommandArgs.Length == 0 || Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsHelpText(this.m_helpCommandArgs[0]))
        return true;
      for (int index = 0; index < this.m_helpCommandArgs.Length; ++index)
      {
        if (Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsHelpAlias(this.m_helpCommandArgs[index]))
          return true;
      }
      return false;
    }

    public bool MsdnRequested() => this.m_helpCommandArgs.Length != 0 && Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsMsdnText(this.m_helpCommandArgs[0]);

    public string GetHelpCommandName()
    {
      foreach (string helpCommandArg in this.m_helpCommandArgs)
      {
        if (!Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsHelpText(helpCommandArg) && !Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsOption(helpCommandArg))
          return helpCommandArg;
      }
      return string.Empty;
    }

    public string GetMsdnCommandName()
    {
      foreach (string helpCommandArg in this.m_helpCommandArgs)
      {
        if (!Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsMsdnText(helpCommandArg) && !Microsoft.TeamFoundation.Client.CommandLine.CommandLine.IsOption(helpCommandArg))
          return helpCommandArg;
      }
      return string.Empty;
    }

    protected static bool IsHelpText(string text) => string.Equals(text, "help", StringComparison.OrdinalIgnoreCase);

    private static bool IsMsdnText(string text) => string.Equals(text, "msdn", StringComparison.OrdinalIgnoreCase);

    protected static bool IsHelpAlias(string text) => text.Equals("/?") || text.Equals("-?") || string.Equals(text, "/h", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "-h", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "/help", StringComparison.OrdinalIgnoreCase) || string.Equals(text, "-help", StringComparison.OrdinalIgnoreCase);

    private static bool IsOption(string text) => text.StartsWith("/", StringComparison.Ordinal) || text.StartsWith("-", StringComparison.Ordinal);
  }
}
