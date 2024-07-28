// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Common.Internal.WatsonReport
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace Microsoft.TeamFoundation.Common.Internal
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class WatsonReport
  {
    internal const string UnmappedGeneralException = "Unmapped";
    public const string UnhandledGeneralException = "General";
    public const string UnhandledDatabaseException = "Database";
    public const string ConfigurationException = "Configuration";
    internal const string UnhandledUnknownException = "Unknown";
    private const string m_exceptionEventType = "TeamFoundationUE";
    private StringCollection m_reportDataFiles = new StringCollection();
    private bool m_keepReportFiles;
    private string m_applicationInformation;
    private string m_exceptionCategory;
    private bool m_reportSynchronous = true;
    private bool m_includeEnvironmentalInformation;
    private bool m_includeHttpRequestInformation;
    private string m_appName;
    private static Dictionary<int, int> s_reportsFiled = new Dictionary<int, int>();
    private static object s_cacheLock = new object();
    private StringCollection m_eventParameters = new StringCollection();
    private string m_eventType;
    private int m_maxExceptionTypeStringLength = 24;
    private string m_eventLogSource;
    private Exception m_exceptionToReport;
    private WatsonReport.WatsonReportType m_reportType;
    private int m_loggingFlags;
    private int m_reportingFlags;
    private int m_userInterfaceFlags;
    private CultureInfo m_uiCulture = CultureInfo.InvariantCulture;
    private WatsonReport.WatsonDisplayInformation m_displayInformation = new WatsonReport.WatsonDisplayInformation();
    private Assembly m_assembly;
    private AppDomain m_appDomain;
    private static int s_maxParameterCount = 10;
    private static string s_dw20Location = Environment.ExpandEnvironmentVariables("%CommonProgramFiles%\\Microsoft Shared\\DW\\DW20.exe");
    private string[] s_illegalWords = new string[23]
    {
      "CON",
      "PRN",
      "AUX",
      "CLOCK$",
      "NUL",
      "COM1",
      "COM2",
      "COM3",
      "COM4",
      "COM5",
      "COM6",
      "COM7",
      "COM8",
      "COM9",
      "LPT1",
      "LPT2",
      "LPT3",
      "LPT4",
      "LPT5",
      "LPT6",
      "LPT7",
      "LPT8",
      "LPT9"
    };
    private Version m_tfVersion = new Version(1, 0, 0, 0);
    private const int m_major = 1;
    private const int m_minor = 0;
    private const int m_build = 0;
    private const int m_majRevision = 0;
    private const int m_minRevision = 0;
    private const int m_defaultEventParameterLength = 25;
    private const string FileSeparator = "|";
    private static int s_reportInterval = 10;

    private WatsonReport(
      WatsonReport.WatsonReportType reportType,
      string eventCategory,
      Exception exceptionToReport)
    {
      this.m_exceptionToReport = exceptionToReport;
      this.m_reportType = reportType;
      this.m_eventType = "TeamFoundationUE";
      this.m_exceptionCategory = eventCategory;
    }

    public static bool VerifyConfiguration() => true;

    public static WatsonReport CreateReport(
      WatsonReport.WatsonReportType reportType,
      string eventType,
      Exception exceptionToReport)
    {
      return new WatsonReport(reportType, eventType, exceptionToReport);
    }

    public void FileReport()
    {
      List<string> filesToKeep = new List<string>();
      List<string> filesToDelete = new List<string>();
      foreach (string reportDataFile in this.m_reportDataFiles)
        File.Open(reportDataFile, FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.ReadWrite).Close();
      if (this.m_reportType == WatsonReport.WatsonReportType.ServerDefault)
        this.SetServerDefaults();
      if (this.OmitReport(this.ReportSignature, out int _))
        return;
      if (this.m_includeHttpRequestInformation || this.m_includeEnvironmentalInformation || !string.IsNullOrEmpty(this.m_applicationInformation))
      {
        StringBuilder stringBuilder = new StringBuilder();
        string tempFileName = FileSpec.GetTempFileName();
        filesToDelete.Add(tempFileName);
        if (this.m_includeEnvironmentalInformation)
          this.GatherEnvironmentData(stringBuilder);
        if (!string.IsNullOrEmpty(this.m_applicationInformation))
        {
          stringBuilder.AppendLine();
          stringBuilder.AppendLine("Application Provided Information");
          stringBuilder.AppendLine(this.m_applicationInformation);
        }
        this.WriteFileData(tempFileName, stringBuilder);
      }
      if (this.m_exceptionToReport != null)
      {
        StringBuilder stringBuilder = new StringBuilder();
        string tempFileName = FileSpec.GetTempFileName();
        filesToDelete.Add(tempFileName);
        this.GatherExceptionData(this.m_exceptionToReport, stringBuilder);
        for (Exception innerException = this.m_exceptionToReport.InnerException; innerException != null; innerException = innerException.InnerException)
        {
          stringBuilder.AppendLine("Inner Exception Details");
          this.GatherExceptionData(innerException, stringBuilder);
        }
        this.WriteFileData(tempFileName, stringBuilder);
      }
      foreach (string reportDataFile in this.m_reportDataFiles)
      {
        if (this.m_keepReportFiles)
          filesToKeep.Add(reportDataFile);
        else
          filesToDelete.Add(reportDataFile);
      }
      if (this.m_assembly != (Assembly) null || this.m_appDomain != null)
      {
        string tempFileName = FileSpec.GetTempFileName();
        filesToDelete.Add(tempFileName);
        StringBuilder stringBuilder = new StringBuilder();
        if (this.m_appDomain != null)
        {
          stringBuilder.AppendLine("Application Domain Information");
          foreach (Assembly assembly in this.m_appDomain.GetAssemblies())
            this.GatherAssemblyInformation(assembly, stringBuilder);
        }
        if (this.m_assembly != (Assembly) null)
        {
          stringBuilder.AppendLine("Reporting Assembly Information");
          this.GatherAssemblyInformation(this.m_assembly, stringBuilder);
        }
        this.WriteFileData(tempFileName, stringBuilder);
      }
      string watsonManifest = this.GenerateWatsonManifest(filesToKeep, filesToDelete);
      filesToDelete.Add(watsonManifest);
      this.FileWerReport(filesToKeep, filesToDelete);
    }

    private void FileWerReport(List<string> filesToKeep, List<string> filesToDelete)
    {
      using (WindowsErrorReport windowsErrorReport = new WindowsErrorReport())
      {
        try
        {
          WindowsErrorReport.WER_REPORT_INFORMATION reportInformation = new WindowsErrorReport.WER_REPORT_INFORMATION();
          reportInformation.dwSize = (uint) Marshal.SizeOf<WindowsErrorReport.WER_REPORT_INFORMATION>(reportInformation);
          reportInformation.wzApplicationName = this.ApplicationName;
          reportInformation.wzDescription = this.DisplayInformation.mainIntroBold;
          reportInformation.wzFriendlyEventName = this.DisplayInformation.queuedEventDescription;
          windowsErrorReport.WerReportCreate(this.m_eventType, WindowsErrorReport.WER_REPORT_TYPE.WerReportNonCritical, reportInformation);
          for (int index = 0; index < this.m_eventParameters.Count && index < WatsonReport.s_maxParameterCount; ++index)
            windowsErrorReport.WerReportSetParameter(index, string.Format("P{0}", (object) (index + 1)), this.m_eventParameters[index].ToString());
          foreach (string path in filesToKeep)
            windowsErrorReport.WerReportAddFile(path, WindowsErrorReport.WER_FILE_TYPE.WerFileTypeOther, (WindowsErrorReport.WER_FILE_FLAGS) 0);
          foreach (string path in filesToDelete)
            windowsErrorReport.WerReportAddFile(path, WindowsErrorReport.WER_FILE_TYPE.WerFileTypeOther, WindowsErrorReport.WER_FILE_FLAGS.WER_FILE_DELETE_WHEN_DONE);
          int num = (int) windowsErrorReport.WerReportSubmit(WindowsErrorReport.WER_CONSENT.WerConsentNotAsked, this.m_reportSynchronous ? WindowsErrorReport.WER_SUBMIT_FLAGS.WER_SUBMIT_OUTOFPROCESS : WindowsErrorReport.WER_SUBMIT_FLAGS.WER_SUBMIT_OUTOFPROCESS_ASYNC);
        }
        catch (Exception ex)
        {
          TeamFoundationTrace.TraceException("Filing WER report.", nameof (FileWerReport), ex);
        }
      }
    }

    private string GenerateWatsonManifest(List<string> filesToKeep, List<string> filesToDelete)
    {
      StringWriter contents = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
      contents.WriteLine();
      contents.WriteLine("Version=131072");
      contents.WriteLine("General_AppName={0}", (object) this.m_appName);
      contents.WriteLine("EventType={0}", (object) this.m_eventType);
      contents.WriteLine("LoggingFlags={0}", (object) this.m_loggingFlags);
      contents.WriteLine("UIFlags={0}", (object) this.m_userInterfaceFlags);
      if (this.m_eventLogSource != null)
        contents.WriteLine("EventLogSource={0}", (object) this.m_eventLogSource);
      if (this.m_uiCulture != null)
        contents.WriteLine("UI LCID={0}", (object) this.m_uiCulture.LCID);
      for (int index = 0; index < this.m_eventParameters.Count && index < WatsonReport.s_maxParameterCount; ++index)
        contents.WriteLine("P{0}={1}", (object) (index + 1), (object) this.m_eventParameters[index]);
      if (filesToDelete.Count > 0)
      {
        this.m_reportingFlags |= 1;
        contents.WriteLine("FilesToDelete={0}", (object) this.CreateFileString(filesToDelete));
      }
      if (filesToKeep.Count > 0)
        contents.WriteLine("FilesToKeep={0}", (object) this.CreateFileString(filesToKeep));
      contents.WriteLine("Main_Intro_Bold={0}", (object) (this.m_displayInformation.mainIntroBold ?? string.Empty));
      contents.WriteLine("Main_Intro_Reg={0}", (object) (this.m_displayInformation.mainIntroRegular ?? string.Empty));
      contents.WriteLine("Main_Plea_Bold={0}", (object) (this.m_displayInformation.mainPleaBold ?? string.Empty));
      contents.WriteLine("Main_Plea_Reg={0}", (object) (this.m_displayInformation.mainPleaRegular ?? string.Empty));
      contents.WriteLine("Queued_EventDescription={0}", (object) (this.m_displayInformation.queuedEventDescription ?? string.Empty));
      string tempFileName = FileSpec.GetTempFileName();
      this.WriteFileData(tempFileName, contents);
      return tempFileName;
    }

    private void WaitForProcess(object parm)
    {
      Process process = (Process) parm;
      try
      {
        process.WaitForExit();
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.Error("Exception while waiting for watson process to complete", ex);
      }
      finally
      {
        process.Close();
      }
    }

    private void SetServerDefaults()
    {
      this.m_eventLogSource = "Team Foundation Error Reporting";
      this.m_userInterfaceFlags = 1;
      this.m_reportingFlags = 14;
      this.m_displayInformation.mainIntroBold = TFCommonResources.WatsonMainIntroBold();
      this.m_displayInformation.mainIntroRegular = TFCommonResources.WatsonMainIntroRegular();
      this.m_displayInformation.mainPleaBold = TFCommonResources.WatsonMainPleaBold();
      this.m_displayInformation.mainPleaRegular = TFCommonResources.WatsonMainPleaRegular();
      this.m_displayInformation.queuedEventDescription = TFCommonResources.WatsonEventDescription((object) this.m_exceptionToReport.Message);
      this.m_includeEnvironmentalInformation = true;
      this.m_includeHttpRequestInformation = true;
      this.m_appDomain = AppDomain.CurrentDomain;
      this.m_reportSynchronous = false;
      this.m_eventParameters.Add(this.TeamFoundationVersion);
      this.m_eventParameters.Add(this.TeamFoundationComponent);
      this.m_eventParameters.Add(this.ReportingAssemblyProductVersion);
      this.m_eventParameters.Add(this.ReportingAssemblyVersion);
      if (this.m_exceptionToReport == null)
        return;
      if (!string.IsNullOrEmpty(this.m_exceptionCategory))
        this.m_eventParameters.Add(this.m_exceptionCategory);
      else
        this.m_eventParameters.Add("Unknown");
      this.m_eventParameters.Add(this.ExceptionType);
      this.m_eventParameters.Add(this.ExceptionSource);
      if (!(this.m_exceptionToReport.TargetSite != (MethodBase) null))
        return;
      this.m_eventParameters.Add(this.ExceptionTargetSite);
    }

    private string GetExceptionTypeString(Exception ex)
    {
      string str1 = ex.GetType().ToString();
      int num = str1.LastIndexOf('.');
      if (num >= 0)
        str1 = str1.Substring(num + 1);
      string str2 = (string) null;
      if (ex is SqlException)
      {
        SqlException sqlException = (SqlException) ex;
        str2 = "." + sqlException.Class.ToString((IFormatProvider) CultureInfo.InvariantCulture) + "." + sqlException.Number.ToString((IFormatProvider) CultureInfo.InvariantCulture);
      }
      return str1.Substring(0, Math.Min(str1.Length, this.m_maxExceptionTypeStringLength)) + str2;
    }

    private string EventParameterString(string parameter) => this.EventParameterString(parameter, 25);

    private string EventParameterString(string parameter, int maxParameterLength) => parameter.Length <= maxParameterLength ? parameter : parameter.Substring(parameter.Length - maxParameterLength, maxParameterLength);

    internal WatsonReport.WatsonDisplayInformation DisplayInformation => this.m_displayInformation;

    internal bool IncludeEnvironmentalInformation
    {
      set => this.m_includeEnvironmentalInformation = value;
    }

    internal AppDomain AppDomain
    {
      set => this.m_appDomain = value;
    }

    internal StringCollection EventParameters => this.m_eventParameters;

    internal string EventType
    {
      set => this.m_eventType = value;
    }

    public StringCollection ReportDataFiles => this.m_reportDataFiles;

    public string ApplicationInformation
    {
      set => this.m_applicationInformation = value;
    }

    public bool KeepReportFiles
    {
      set => this.m_keepReportFiles = value;
    }

    internal int LoggingFlags
    {
      set => this.m_loggingFlags = value;
    }

    internal int ReportingFlags
    {
      set => this.m_reportingFlags = value;
    }

    internal int UserInterfaceFlags
    {
      set => this.m_userInterfaceFlags = value;
    }

    internal CultureInfo UserInterfaceCulture
    {
      set => this.m_uiCulture = value;
    }

    public string ApplicationName
    {
      get => this.m_appName;
      set => this.m_appName = value;
    }

    internal string EventLogSource
    {
      set => this.m_eventLogSource = value;
    }

    internal string ExceptionCategory
    {
      set => this.m_exceptionCategory = value;
    }

    internal Assembly Assembly
    {
      set => this.m_assembly = value;
    }

    internal bool ReportSynchronously
    {
      set => this.m_reportSynchronous = value;
    }

    private bool OmitReport(int signature, out int occurrences)
    {
      occurrences = 0;
      if (signature == 0)
        return false;
      lock (WatsonReport.s_cacheLock)
      {
        int num;
        occurrences = !WatsonReport.s_reportsFiled.TryGetValue(signature, out num) ? 1 : num + 1;
        WatsonReport.s_reportsFiled[signature] = occurrences;
      }
      return occurrences != 1 && occurrences % WatsonReport.s_reportInterval != 0;
    }

    private int ReportSignature
    {
      get
      {
        int reportSignature = 0;
        foreach (string eventParameter in this.m_eventParameters)
          reportSignature += eventParameter.GetHashCode();
        return reportSignature;
      }
    }

    private string CreateFileString(List<string> filenameList)
    {
      StringBuilder stringBuilder = new StringBuilder();
      if (filenameList.Count > 0)
      {
        stringBuilder.Append(filenameList[0]);
        for (int index = 1; index < filenameList.Count; ++index)
        {
          stringBuilder.Append("|");
          stringBuilder.Append(filenameList[index]);
        }
      }
      return stringBuilder.ToString();
    }

    private void GatherAssemblyInformation(Assembly assembly, StringBuilder assemblyInfo)
    {
      try
      {
        if (!(assembly.ManifestModule.GetType().Namespace != "System.Reflection.Emit"))
          return;
        assemblyInfo.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assembly Name={0}", (object) assembly.FullName));
        assemblyInfo.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assembly CLR Version={0}", (object) assembly.ImageRuntimeVersion));
        assemblyInfo.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assembly Version={0}", (object) assembly.GetName().Version.ToString()));
        assemblyInfo.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Assembly Location={0}", (object) assembly.Location));
        if (string.IsNullOrEmpty(assembly.Location))
          return;
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
        assemblyInfo.AppendLine("Assembly File Version:");
        assemblyInfo.AppendLine(versionInfo.ToString());
      }
      catch (NotSupportedException ex)
      {
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Gathering assembly info for Watson report", (string) null, ex);
      }
    }

    private void WriteFileData(string filename, StringWriter contents)
    {
      using (StreamWriter streamWriter = new StreamWriter(filename, false, Encoding.Unicode))
        streamWriter.Write(contents.ToString());
    }

    private void WriteFileData(string filename, StringBuilder contents)
    {
      using (StreamWriter streamWriter = new StreamWriter(filename, false, Encoding.Unicode))
        streamWriter.Write(contents.ToString());
    }

    private void GatherEnvironmentData(StringBuilder data)
    {
      data.AppendLine("System Values");
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "OS Version Information={0}", (object) Environment.OSVersion));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "CLR Version Information={0}", (object) Environment.Version));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Machine Name={0} Processor Count={1}", (object) Environment.MachineName, (object) Environment.ProcessorCount));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Working Set={0}", (object) Environment.WorkingSet));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "System Directory={0}", (object) Environment.SystemDirectory));
      data.AppendLine("Process Values");
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "ExitCode={0}", (object) Environment.ExitCode.ToString((IFormatProvider) CultureInfo.InvariantCulture)));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Interactive={0}", (object) Environment.UserInteractive.ToString()));
      data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Has Shutdown Started={0}", (object) Environment.HasShutdownStarted.ToString()));
      data.AppendLine("Process Environment Variables");
      foreach (DictionaryEntry environmentVariable in Environment.GetEnvironmentVariables())
        data.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} = {1}", environmentVariable.Key, environmentVariable.Value));
      data.AppendLine();
    }

    private void GatherExceptionData(Exception ex, StringBuilder exContents)
    {
      exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Exception: {0}", (object) ex.GetType().Name));
      exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Message: {0}", (object) ex.Message));
      if (ex is SqlException)
      {
        SqlException sqlException = (SqlException) ex;
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Class: {0}", (object) sqlException.Class));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Number: {0}", (object) sqlException.Number));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Server: {0}", (object) sqlException.Server));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Source: {0}", (object) sqlException.Source));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception State: {0}", (object) sqlException.State));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Procedure: {0}", (object) sqlException.Procedure));
        exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Exception Line Number: {0}", (object) sqlException.LineNumber));
        for (int index = 1; index < sqlException.Errors.Count; ++index)
        {
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "SQL Error: {0}", (object) sqlException.Errors[index].ToString()));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tClass: {0}", (object) sqlException.Errors[index].Class));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tNumber: {0}", (object) sqlException.Errors[index].Number));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tServer: {0}", (object) sqlException.Errors[index].Server));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tSource: {0}", (object) sqlException.Errors[index].Source));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tState: {0}", (object) sqlException.Errors[index].State));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tProcedure: {0}", (object) sqlException.Errors[index].Procedure));
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "\tLine Number: {0}", (object) sqlException.Errors[index].LineNumber));
        }
      }
      if (ex.Data != null && ex.Data.Count > 0)
      {
        exContents.AppendLine("Exception Data Dictionary follows");
        foreach (DictionaryEntry dictionaryEntry in ex.Data)
          exContents.AppendLine(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0} = {1}", dictionaryEntry.Key, dictionaryEntry.Value));
      }
      exContents.AppendLine("Stack Trace:");
      exContents.AppendLine(ex.StackTrace);
    }

    private string TeamFoundationVersion => this.m_tfVersion.ToString();

    private string TeamFoundationComponent => this.EventParameterString(this.m_appName);

    private string ReportingAssemblyProductVersion
    {
      get
      {
        try
        {
          return FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
        }
        catch
        {
          return Assembly.GetExecutingAssembly().ImageRuntimeVersion;
        }
      }
    }

    private string ReportingAssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

    private string ExceptionType => this.GetExceptionTypeString(this.m_exceptionToReport);

    private string ExceptionSource
    {
      get
      {
        string str = this.m_exceptionToReport.StackTrace.GetHashCode().ToString("X", (IFormatProvider) CultureInfo.InvariantCulture);
        return this.m_exceptionToReport.Source != null ? str + this.m_exceptionToReport.Source.ToString().GetHashCode().ToString("X", (IFormatProvider) CultureInfo.InvariantCulture) : str;
      }
    }

    private string ExceptionTargetSite => this.m_exceptionToReport.TargetSite.Name.ToString().GetHashCode().ToString("X", (IFormatProvider) CultureInfo.InvariantCulture);

    private enum ReturnCodes
    {
      Success = 0,
      Failure = 1,
      Debug = 16, // 0x00000010
    }

    [Flags]
    internal enum WatsonReportingFlags
    {
      fDwrDeleteFiles = 1,
      fDwrIgnoreHKCU = 2,
      fDwrForceToAdminQueue = 4,
      fDwrForceOfflineMode = 8,
      fDwrDenyOfflineMode = 16, // 0x00000010
      fDwrNoHeapCollection = 32, // 0x00000020
      fDwrNoSecondLevelCollection = 64, // 0x00000040
      fDwrNeverUpload = 128, // 0x00000080
      fDwrDontPromptIfCantReport = 256, // 0x00000100
      fDwrNoDefaultCabLimit = 512, // 0x00000200
    }

    [Flags]
    internal enum WatsonUserInterfaceFlags
    {
      fDwuNoEventUI = 1,
      fDwuNoQueueUI = 2,
      fDwuShowFeedbackLink = 4,
      fDwuUseIE = 8,
      fDwuManifestDebug = 32, // 0x00000020
      fDwuDenySuspend = 64, // 0x00000040
    }

    [Flags]
    internal enum WatsonLoggingFlags
    {
      fDwlNoParameterLog = 1,
      fDwlNoBucketLog = 2,
      fDwlResponseLog = 4,
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public enum WatsonReportType
    {
      ServerDefault,
      None,
    }

    internal class WatsonDisplayInformation
    {
      internal string mainIntroBold;
      internal string mainIntroRegular;
      internal string mainPleaBold;
      internal string mainPleaRegular;
      internal string queuedEventDescription;
    }
  }
}
