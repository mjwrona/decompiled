// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.WindowsErrorReporting.WatsonReport
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry.WindowsErrorReporting
{
  internal class WatsonReport
  {
    public const int DefaultWatsonSamplePercent = 10;
    public const int DefaultMaximumWatsonReportsPerSession = 10;
    public const int DefaultMinimumSecondsBetweenWatsonReports = 3600;
    internal const string UnknownBucketValue = "_";
    internal const int NumberOfBucketParameters = 10;
    internal const string ExceptionDataModelPrefix = "DataModel.Fault.Exception.";
    private static string[] bucketNames = new string[10]
    {
      "AppName",
      "AppVer",
      "TelemetryName",
      "FailureParam0",
      "FailureParam1",
      "FailureParam2",
      "FailureParam3",
      "FailureParam4",
      "NonFailureParam0",
      "NonFailureParam1"
    };
    public const int P0AppNameIndex = 0;
    public const int P1AppVersionIndex = 1;
    public const int P2TelemetryNameIndex = 2;
    public const int P3failureParam0Index = 3;
    public const int P3ExceptionTypeDefaultIndex = 3;
    public const int P4failureParam1Index = 4;
    public const int P4ModuleNameDefaultIndex = 4;
    public const int P5failureParam2Index = 5;
    public const int P5MethodNameDefaultIndex = 5;
    public const int P6failureParam3Index = 6;
    public const int P7failureParam4Index = 7;
    public const int P8nonFailureParam0Index = 8;
    public const int P9nonFailureParam1Index = 9;

    private FaultEvent FaultEvent { get; }

    private TelemetrySession TelemetrySession { get; }

    internal StringBuilder SBuilderErrorInfo { get; } = new StringBuilder();

    public WatsonReport(FaultEvent faultEvent, TelemetrySession telemetrySession)
    {
      faultEvent.RequiresArgumentNotNull<FaultEvent>(nameof (faultEvent));
      telemetrySession.RequiresArgumentNotNull<TelemetrySession>(nameof (telemetrySession));
      this.FaultEvent = faultEvent;
      this.TelemetrySession = telemetrySession;
      this.SBuilderErrorInfo.AppendLine("Error Information");
      this.SBuilderErrorInfo.AppendLine("AppInsightsEvent Name = " + this.FaultEvent.Name);
      this.SBuilderErrorInfo.AppendLine("          Description = " + this.FaultEvent.Description);
      this.SBuilderErrorInfo.AppendLine("     TelemetrySession = " + this.TelemetrySession.ToString());
      this.SBuilderErrorInfo.AppendLine("      WatsonEventType = " + this.FaultEvent.WatsonEventType);
      this.SBuilderErrorInfo.AppendLine("             UTC time = " + DateTime.UtcNow.ToString("s", (IFormatProvider) DateTimeFormatInfo.InvariantInfo));
      this.AddPropertiesForExceptionObject(this.FaultEvent.ExceptionObject);
      this.FaultEvent.Properties["VS.Fault.WatsonEventType"] = (object) this.FaultEvent.WatsonEventType;
      if (this.FaultEvent.WatsonEventType == "VisualStudioNonFatalErrors2" && Platform.IsWindows)
        this.SetInitialBucketParameters();
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Description", (object) this.FaultEvent.Description);
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.FaultSeverity", (object) this.FaultEvent.FaultSeverity.ToString());
    }

    public void PostWatsonReport(int maxReportsPerSession, int minSecondsBetweenReports)
    {
      bool? includedInWatsonSample = this.FaultEvent.IsIncludedInWatsonSample;
      bool flag = false;
      if (includedInWatsonSample.GetValueOrDefault() == flag & includedInWatsonSample.HasValue)
      {
        this.AddBucketParametersToEventProperties();
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.IsSampled", (object) this.FaultEvent.IsIncludedInWatsonSample);
        this.FaultEvent.Properties["VS.Fault.WatsonOptIn"] = (object) this.FaultEvent.UserOptInToWatson.ToString();
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "NotSampled");
        this.LogTelemetryAboutExtraDataAddedToFaultEvent();
      }
      else
      {
        try
        {
          if (this.FaultEvent.GatherEventDetails != null)
          {
            try
            {
              int num = this.FaultEvent.GatherEventDetails((IFaultUtility) this.FaultEvent);
              this.AddBucketParametersToEventProperties();
              if (num != 0)
              {
                this.FaultEvent.IsIncludedInWatsonSample = new bool?(false);
                this.FaultEvent.UserOptInToWatson = FaultEvent.FaultEventWatsonOptIn.CallbackOptOut;
                this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.IsSampled", (object) this.FaultEvent.IsIncludedInWatsonSample);
                this.FaultEvent.Properties["VS.Fault.WatsonOptIn"] = (object) this.FaultEvent.UserOptInToWatson.ToString();
                this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "CallbackOptedOut");
                this.LogTelemetryAboutExtraDataAddedToFaultEvent();
                return;
              }
              this.FaultEvent.UserOptInToWatson = FaultEvent.FaultEventWatsonOptIn.CallbackOptIn;
            }
            catch (Exception ex)
            {
              this.FaultEvent.UserOptInToWatson = FaultEvent.FaultEventWatsonOptIn.CallbackException;
              string str = ex.ToString();
              this.SBuilderErrorInfo.AppendLine("Fault Event Delegate threw exception.\r\n" + str);
              this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.DelegateException", (object) str);
            }
          }
          else
            this.AddBucketParametersToEventProperties();
          BucketFilter matchingBucketFilter = this.GetMatchingBucketFilter(this.TelemetrySession.BucketFiltersToAddDumpsToFaults, "ProcessDumpRequested");
          if (matchingBucketFilter != null)
          {
            using (Process currentProcess = Process.GetCurrentProcess())
            {
              this.FaultEvent.AddProcessDump(currentProcess.Id);
              string dumpType;
              if (matchingBucketFilter.AdditionalProperties.TryGetValue("DumpType", out dumpType))
              {
                WER_DUMP_TYPE dumpTypeFromString = FaultEvent.GetDumpTypeFromString(dumpType);
                if (dumpTypeFromString != WER_DUMP_TYPE.WerDumpTypeMax)
                {
                  this.FaultEvent.DumpCollectionType = dumpTypeFromString;
                  this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.DumpTypeRequestedByBucketFilterId", (object) matchingBucketFilter.Id);
                  this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.DumpTypeRequested", (object) dumpTypeFromString.ToString());
                }
              }
            }
          }
          this.LogTelemetryAboutExtraDataAddedToFaultEvent();
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.IsSampled", (object) this.FaultEvent.IsIncludedInWatsonSample);
          this.FaultEvent.Properties["VS.Fault.WatsonOptIn"] = (object) this.FaultEvent.UserOptInToWatson.ToString();
          if (!this.FaultEvent.AddedErrorInformation && !this.FaultEvent.AddedFile && !this.FaultEvent.AddedProcessDump && this.FaultEvent.UserOptInToWatson != FaultEvent.FaultEventWatsonOptIn.CallbackOptIn)
            this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "NoExtraDataAdded");
          else if (WatsonSessionChannel.NumberOfWatsonReportsThisSession >= maxReportsPerSession)
          {
            this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "MaxReportsPerSessionReached");
          }
          else
          {
            double totalSeconds = (DateTime.Now - WatsonSessionChannel.DateTimeOfLastWatsonReport).TotalSeconds;
            if (totalSeconds < (double) minSecondsBetweenReports)
            {
              this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "TooSoonSinceLastReport");
              this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.SecondsSinceLastReport", (object) totalSeconds);
            }
            else if (matchingBucketFilter == null && this.GetMatchingBucketFilter(this.TelemetrySession.BucketFiltersToEnableWatsonForFaults, "WatsonReportEnabled") == null)
            {
              this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WatsonNotSentReason", (object) "NoBucketFilter");
            }
            else
            {
              bool sendParams = this.FaultEvent.UserOptInToWatson == FaultEvent.FaultEventWatsonOptIn.PropertyOptIn || this.FaultEvent.UserOptInToWatson == FaultEvent.FaultEventWatsonOptIn.CallbackOptIn;
              bool fullCab = this.FaultEvent.IsIncludedInWatsonSample.Value;
              if (!sendParams && !fullCab)
                return;
              this.FaultEvent.Properties["VS.Fault.WatsonReportNumber"] = (object) WatsonSessionChannel.NumberOfWatsonReportsThisSession;
              ++WatsonSessionChannel.NumberOfWatsonReportsThisSession;
              WatsonSessionChannel.DateTimeOfLastWatsonReport = DateTime.Now;
              if (sendParams && !fullCab)
              {
                Version version = Environment.OSVersion.Version;
                if (version.Major < 6 || version.Major == 6 && version.Minor < 2)
                {
                  this.FaultEvent.Properties["VS.Fault.WatsonNotSentReason"] = (object) "oldOSVersion";
                  return;
                }
              }
              WER_SUBMIT_RESULT submitResult = WER_SUBMIT_RESULT.WerReportFailed;
              DateTime startTime = DateTime.MinValue;
              DateTime endTime = DateTime.MinValue;
              if (this.FaultEvent.SynchronousDumpCollection)
              {
                this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WerSubmitCurrentThread", (object) true);
                try
                {
                  submitResult = this.SendWatsonReport(fullCab, sendParams, out startTime, out endTime);
                }
                catch (Exception ex)
                {
                  this.LogExceptionToTelemetry(ex);
                }
              }
              else
              {
                this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WerSubmitCurrentThread", (object) false);
                SynchronizationContext current = SynchronizationContext.Current;
                try
                {
                  SynchronizationContext.SetSynchronizationContext(NoPumpSyncContext.Default);
                  using (ManualResetEvent mre = new ManualResetEvent(false))
                  {
                    ThreadPool.QueueUserWorkItem((WaitCallback) (state =>
                    {
                      try
                      {
                        submitResult = this.SendWatsonReport(fullCab, sendParams, out startTime, out endTime);
                      }
                      catch (Exception ex)
                      {
                        this.LogExceptionToTelemetry(ex);
                      }
                      finally
                      {
                        mre.Set();
                      }
                    }), (object) null);
                    mre.WaitOne();
                  }
                }
                finally
                {
                  SynchronizationContext.SetSynchronizationContext(current);
                }
              }
              switch (submitResult)
              {
                case WER_SUBMIT_RESULT.WerReportQueued:
                case WER_SUBMIT_RESULT.WerReportAsync:
                  if (!this.GetReportInfo(true, startTime, endTime))
                  {
                    this.GetReportInfo(false, startTime, endTime);
                    break;
                  }
                  break;
                case WER_SUBMIT_RESULT.WerReportUploaded:
                  this.GetReportInfo(false, startTime, endTime);
                  break;
              }
              if (!WatsonReport.ShouldSendSupplementalInfoTelemetryEvent())
                return;
              Guid correlationId = Guid.NewGuid();
              this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.CorrelationId", (object) correlationId);
              this.SendSupplementalInfoEvent(correlationId);
            }
          }
        }
        catch (Exception ex)
        {
          this.LogExceptionToTelemetry(ex);
        }
      }
    }

    private void LogTelemetryAboutExtraDataAddedToFaultEvent()
    {
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.CountOfDumpsRequested", (object) this.FaultEvent.ListProcessIdsToDump.Count);
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.CountOfFilesRequested", (object) this.FaultEvent.ListFilesToAdd.Count);
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.AddedErrorInformation", (object) this.FaultEvent.AddedErrorInformation);
    }

    private void LogExceptionToTelemetry(Exception ex)
    {
      try
      {
        this.TelemetrySession.PostEvent(new TelemetryEvent("VS/TelemetryError/FaultEvent")
        {
          Properties = {
            ["VS.TelemetryError.FaultEvent.ExceptionType"] = (object) ex.GetType().FullName
          }
        });
      }
      catch
      {
      }
    }

    private bool GetReportInfo(bool queue, DateTime startTime, DateTime endTime)
    {
      if (startTime == DateTime.MinValue || !WerStoreApi.IsStoreInterfacePresent)
        return false;
      using (WerStoreApi.IWerStore store = WerStoreApi.GetStore(queue ? WerStoreApi.REPORT_STORE_TYPES.MACHINE_QUEUE : WerStoreApi.REPORT_STORE_TYPES.MACHINE_ARCHIVE))
      {
        WerStoreApi.IWerReportData werReportData = store.GetReports().FirstOrDefault<WerStoreApi.IWerReportData>((Func<WerStoreApi.IWerReportData, bool>) (x =>
        {
          if (x.EventType != this.FaultEvent.WatsonEventType)
            return false;
          for (int index = 0; index < 10; ++index)
          {
            if (x.Parameters[index].Value != this.FaultEvent.BucketParameters[index] && (!(x.Parameters[index].Value == "_") || !string.IsNullOrEmpty(this.FaultEvent.BucketParameters[index])))
              return false;
          }
          return !(x.TimeStamp < startTime) && !(x.TimeStamp > endTime);
        }));
        if (werReportData != null)
        {
          IDictionary<string, object> properties1 = this.FaultEvent.Properties;
          Guid guid = werReportData.ReportId;
          string str1 = guid.ToString();
          properties1["VS.Fault.ReportID"] = (object) str1;
          if (!string.IsNullOrEmpty(werReportData.CabID))
            this.FaultEvent.Properties["VS.Fault.CabID"] = (object) werReportData.CabID;
          if (werReportData.BucketId != Guid.Empty)
          {
            IDictionary<string, object> properties2 = this.FaultEvent.Properties;
            guid = werReportData.BucketId;
            string str2 = guid.ToString();
            properties2["VS.Fault.BucketID"] = (object) str2;
          }
          return true;
        }
      }
      return false;
    }

    private WER_SUBMIT_RESULT SendWatsonReport(
      bool fullCab,
      bool sendParams,
      out DateTime startTime,
      out DateTime endTime)
    {
      if (!Platform.IsWindows)
      {
        startTime = DateTime.MinValue;
        endTime = DateTime.MinValue;
        return WER_SUBMIT_RESULT.WerDisabled;
      }
      using (WerReportBuilder reportBuilder = WerReportBuilder.Create(this.FaultEvent.WatsonEventType, WER_REPORT_TYPE.WerReportNonCritical))
      {
        if (reportBuilder == null)
        {
          startTime = DateTime.MinValue;
          endTime = DateTime.MinValue;
          return WER_SUBMIT_RESULT.WerReportFailed;
        }
        this.SetReportParameters(reportBuilder);
        if (fullCab)
        {
          this.SetDumpTypeFaultEventProperties();
          this.AddDumpsToReport(reportBuilder);
          this.AddFilesToReport(reportBuilder);
          this.AddErrorInfoFile(reportBuilder);
        }
        int watsonFlags;
        if (!fullCab & sendParams)
          watsonFlags = 4128;
        else if (fullCab)
        {
          watsonFlags = 32;
        }
        else
        {
          startTime = DateTime.MinValue;
          endTime = DateTime.MinValue;
          return WER_SUBMIT_RESULT.WerReportCancelled;
        }
        return this.SubmitReport(reportBuilder, watsonFlags, out startTime, out endTime);
      }
    }

    private void AddFilesToReport(WerReportBuilder reportBuilder)
    {
      try
      {
        int num1 = 0;
        long num2 = 0;
        foreach (string str in this.FaultEvent.ListFilesToAdd)
        {
          if (File.Exists(str))
          {
            if (reportBuilder.AddFile(str, 0))
            {
              ++num1;
              num2 += new FileInfo(str).Length;
            }
            else
              this.SBuilderErrorInfo.AppendLine("WerReportBuilder failed to add a file to the report: " + System.IO.Path.GetFileName(str));
          }
          else
            this.SBuilderErrorInfo.AppendLine("Failed to add file to report, file does not exist: " + System.IO.Path.GetFileName(str));
        }
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.CountOfFilesAdded", (object) num1);
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.SizeOfFilesAdded", (object) num2);
      }
      catch (Exception ex)
      {
        this.SBuilderErrorInfo.AppendLine("Exception thrown while adding files to report.\r\n" + ex.ToString());
      }
    }

    private WER_SUBMIT_RESULT SubmitReport(
      WerReportBuilder reportBuilder,
      int watsonFlags,
      out DateTime startTime,
      out DateTime endTime)
    {
      startTime = DateTime.UtcNow;
      WER_SUBMIT_RESULT werSubmitResult = reportBuilder.SubmitReport(WER_CONSENT.WerConsentNotAsked, watsonFlags);
      endTime = DateTime.UtcNow;
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WerSubmitDurationInMs", (object) (endTime - startTime).TotalMilliseconds);
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.WerSubmitResult", (object) werSubmitResult.ToString());
      return werSubmitResult;
    }

    private void SetReportParameters(WerReportBuilder reportBuilder)
    {
      bool flag = this.FaultEvent.WatsonEventType == "VisualStudioNonFatalErrors2";
      for (int paramId = 0; paramId < 10; ++paramId)
      {
        string paramValue = this.FaultEvent.BucketParameters[paramId];
        if (flag)
        {
          if (string.IsNullOrEmpty(paramValue))
            paramValue = "_";
          reportBuilder.SetParameter(paramId, WatsonReport.bucketNames[paramId], paramValue);
        }
        else if (!string.IsNullOrEmpty(paramValue))
          reportBuilder.SetParameter(paramId, (string) null, paramValue);
      }
    }

    private void AddDumpsToReport(WerReportBuilder reportBuilder)
    {
      int num = 0;
      foreach (int processId in this.FaultEvent.ListProcessIdsToDump)
      {
        try
        {
          using (Process processById = Process.GetProcessById(processId))
          {
            if (processById != null)
            {
              WER_DUMP_TYPE dumpCollectionType = this.FaultEvent.DumpCollectionType;
              this.SBuilderErrorInfo.AppendLine(string.Format("WerReportAddDump PID={0} {1} {2}", (object) processId, (object) processById.ProcessName, (object) dumpCollectionType));
              if (reportBuilder.AddDump(processById, dumpCollectionType))
                ++num;
            }
          }
        }
        catch (Exception ex)
        {
          this.SBuilderErrorInfo.AppendLine("Adding dump to report threw an exception.\r\n" + ex.ToString());
        }
      }
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.CountOfDumpsAdded", (object) num);
    }

    private void AddErrorInfoFile(WerReportBuilder reportBuilder)
    {
      int dwFileFlags = 2;
      if (this.FaultEvent.SBuilderAdditionalUserErrorInfo.Length > 0)
      {
        this.SBuilderErrorInfo.AppendLine("Additional Error info marked as not-anonymous (potentially contains PII)");
        this.SBuilderErrorInfo.Append((object) this.FaultEvent.SBuilderAdditionalUserErrorInfo);
        dwFileFlags = 0;
      }
      if (this.SBuilderErrorInfo.Length == 0)
        return;
      string str1 = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "VSFaultInfo", DateTime.Now.ToString("yyMMdd_hhmmss_fffffff"));
      if (!Directory.Exists(str1))
        ReparsePointAware.CreateDirectory(str1);
      string str2 = System.IO.Path.Combine(str1, "ErrorInformation.txt");
      if (File.Exists(str2))
        ReparsePointAware.DeleteFile(str2);
      string contents = this.SBuilderErrorInfo.ToString();
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.LengthOfErrorInformation", (object) contents.Length);
      ReparsePointAware.WriteAllText(str2, contents);
      reportBuilder.AddFile(str2, dwFileFlags);
    }

    private void SetDumpTypeFaultEventProperties()
    {
      WER_DUMP_TYPE result = WER_DUMP_TYPE.WerDumpTypeMiniDump;
      object obj;
      if (this.FaultEvent.Properties.TryGetValue("DumpCollectionType", out obj) && obj is string str)
      {
        if (Enum.TryParse<WER_DUMP_TYPE>(str, true, out result))
          this.FaultEvent.DumpCollectionType = result;
        else
          this.FaultEvent.Properties["ErrorInvalidDumpCollectionType"] = (object) str;
      }
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.DumpCollectionType", (object) this.FaultEvent.DumpCollectionType.ToString());
    }

    private void AddBucketParametersToEventProperties()
    {
      for (int index = 0; index < 10; ++index)
        this.FaultEvent.ReservedProperties.AddPrefixed(string.Format("{0}DataModel.Fault.BucketParam{1}", (object) "Reserved.", (object) (index + 1)), (object) this.FaultEvent.BucketParameters[index]);
    }

    internal BucketFilter GetMatchingBucketFilter(
      List<BucketFilter> bucketFilters,
      string bucketFilterTelemetryPropertyNamePrefix)
    {
      if (bucketFilters == null)
        return (BucketFilter) null;
      foreach (BucketFilter bucketFilter in bucketFilters)
      {
        if (bucketFilter.WatsonEventType == this.FaultEvent.WatsonEventType)
        {
          bool flag = false;
          for (int index = 0; index < 10; ++index)
          {
            string bucketParameterFilter = bucketFilter.BucketParameterFilters[index];
            if (bucketParameterFilter != null)
            {
              string bucketParameter = this.FaultEvent.BucketParameters[index];
              if (bucketParameter == null)
              {
                flag = true;
                break;
              }
              if (!Regex.Match(bucketParameter, bucketParameterFilter).Success)
              {
                flag = true;
                break;
              }
            }
          }
          if (!flag)
          {
            this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault." + bucketFilterTelemetryPropertyNamePrefix + "ByBucketFilterId", (object) bucketFilter.Id);
            return bucketFilter;
          }
        }
      }
      return (BucketFilter) null;
    }

    private void SendSupplementalInfoEvent(Guid correlationId)
    {
      try
      {
        TelemetryEvent telemetryEvent = new TelemetryEvent("VS/FaultEvent/AsyncDumpInfo");
        string str1 = "VS.Fault.";
        telemetryEvent.Properties[str1 + "CorrelationId"] = (object) correlationId;
        List<string> stringList1 = new List<string>();
        stringList1.Add("ReportID");
        stringList1.Add("CabID");
        stringList1.Add("BucketID");
        stringList1.Add("WatsonReportNumber");
        stringList1.Add("WatsonOptIn");
        List<string> stringList2 = new List<string>()
        {
          "CountOfDumpsAdded",
          "CountOfFilesAdded",
          "SizeOfFilesAdded",
          "ProcessDumpRequestedByBucketFilterId",
          "WerSubmitDurationInMs"
        };
        List<string> values = new List<string>();
        foreach (string str2 in stringList1)
        {
          string key = str1 + str2;
          object obj;
          if (this.FaultEvent.Properties.TryGetValue(key, out obj))
            telemetryEvent.Properties[key] = obj;
          else
            values.Add(str2);
        }
        foreach (string str3 in stringList2)
        {
          string key = "DataModel.Fault." + str3;
          object obj;
          if (this.FaultEvent.ReservedProperties.TryGetValue(key, out obj))
            telemetryEvent.ReservedProperties.AddPrefixed("Reserved." + key, obj);
          else
            values.Add(str3);
        }
        if (values.Count > 0)
          telemetryEvent.Properties[str1 + "PropertiesNotFound"] = (object) string.Join(";", (IEnumerable<string>) values);
        this.TelemetrySession.PostEvent(telemetryEvent);
      }
      catch (Exception ex)
      {
        this.LogExceptionToTelemetry(ex);
      }
    }

    private static bool ShouldSendSupplementalInfoTelemetryEvent() => WatsonReport.GetFaultEventDumpModeRemoteSettingValue() == 1;

    private static int GetFaultEventDumpModeRemoteSettingValue()
    {
      string name1 = "FaultEventDumpMode";
      int defaultValue = 0;
      string name2 = string.Format("Software\\Microsoft\\VisualStudio\\{0}.0_Remote\\FaultEvent", (object) Process.GetCurrentProcess().MainModule.FileVersionInfo.FileMajorPart);
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(name2))
          return registryKey == null ? defaultValue : (int) registryKey.GetValue(name1, (object) defaultValue);
      }
      catch
      {
      }
      return defaultValue;
    }

    private void TrySetBucketParameter(int bucketNum, string value)
    {
      if (!string.IsNullOrEmpty(this.FaultEvent.GetBucketParameter(bucketNum)))
        return;
      this.FaultEvent.SetBucketParameter(bucketNum, value);
    }

    private bool GetBlameFromFrame(
      StackFrame frame,
      ref string methodName,
      ref string fileName,
      ref string offset)
    {
      if (string.IsNullOrEmpty(frame.GetMethod()?.Module?.Name?.ToLowerInvariant()))
        return false;
      methodName = frame.GetMethod().DeclaringType.FullName + "." + frame.GetMethod().Name;
      fileName = frame.GetMethod().Module.Assembly.Location;
      offset = frame.GetNativeOffset().ToString();
      return true;
    }

    private void SetInitialBucketParameters()
    {
      try
      {
        using (Process currentProcess = Process.GetCurrentProcess())
        {
          this.TrySetBucketParameter(0, System.IO.Path.GetFileName(currentProcess.MainModule.FileName).ToLowerInvariant());
          string str = currentProcess.MainModule.FileVersionInfo.FileVersion;
          if (str != null)
          {
            int length = str.IndexOf(" built ", StringComparison.InvariantCultureIgnoreCase);
            if (length > 0)
              str = str.Substring(0, length);
          }
          this.TrySetBucketParameter(1, str);
          Assembly assembly = (Assembly) null;
          StackTrace stackTrace1 = (StackTrace) null;
          if (this.FaultEvent.ExceptionObject != null)
          {
            this.AddExceptionInfoToIncludedFile(this.FaultEvent.ExceptionObject);
            Exception innerMostException = this.GetInnerMostException(this.FaultEvent.ExceptionObject, (Func<Exception, bool>) (ex => ex.TargetSite != (MethodBase) null));
            MethodBase targetSite = innerMostException.TargetSite;
            if (targetSite != (MethodBase) null)
              assembly = targetSite.DeclaringType.Assembly;
            if (!string.IsNullOrEmpty(innerMostException.StackTrace))
              stackTrace1 = new StackTrace(innerMostException, false);
            this.TrySetBucketParameter(3, innerMostException.GetType().FullName);
          }
          string fileName = string.Empty;
          string empty = string.Empty;
          string offset = "0";
          int num = 0;
          if (assembly != (Assembly) null)
            fileName = assembly.Location;
          if (stackTrace1 == null)
            stackTrace1 = new StackTrace(false);
          StackFrame[] frames = stackTrace1.GetFrames();
          if (frames != null)
          {
            foreach (StackFrame frame in frames)
            {
              string name = frame.GetMethod()?.Module?.Name;
              num += name.GetHashCode();
              if (!string.Equals(name, "mscorlib.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "microsoft.visualstudio.telemetry.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "microsoft.visualstudio.telemetry.package.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "windowsbase.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "system.dll", StringComparison.OrdinalIgnoreCase) && this.GetBlameFromFrame(frame, ref empty, ref fileName, ref offset))
                break;
            }
            if (string.IsNullOrEmpty(empty))
            {
              foreach (StackFrame frame in frames)
              {
                string name = frame.GetMethod()?.Module?.Name;
                if (!string.Equals(name, "microsoft.visualstudio.telemetry.dll", StringComparison.OrdinalIgnoreCase) && !string.Equals(name, "microsoft.visualstudio.telemetry.package.dll", StringComparison.OrdinalIgnoreCase) && this.GetBlameFromFrame(frame, ref empty, ref fileName, ref offset))
                  break;
              }
            }
            if (this.FaultEvent.ExceptionObject == null || string.IsNullOrEmpty(this.FaultEvent.ExceptionObject.StackTrace))
            {
              IEnumerable<StackFrame> stackFrames = WatsonReport.RemoveFrames(frames);
              string stackTrace2 = WatsonReport.FormatStackTrace(stackFrames, shorten: true);
              string stack = WatsonReport.FormatStackTrace(stackFrames);
              this.AddStackToTelemetryReport(stackTrace2, exceptionStack: false);
              this.AddStackToFile("currently running Stack", stack, 0);
            }
            this.SetBucketParametersForModule(fileName, empty, offset);
            this.SBuilderErrorInfo.AppendLine(string.Format("CallStack Hash:{0:x}", (object) num));
          }
          else
            this.SBuilderErrorInfo.AppendLine("Failed to get stacktrace.");
        }
      }
      catch (Exception ex)
      {
        this.SBuilderErrorInfo.AppendLine("Exception getting module info" + WatsonReport.FormatException(ex));
      }
      this.TrySetBucketParameter(2, this.FaultEvent.Name.Replace("/", "."));
    }

    internal static IEnumerable<StackFrame> RemoveFrames(StackFrame[] frames)
    {
      List<StackFrame> stackFrameList = new List<StackFrame>();
      foreach (StackFrame frame in frames)
      {
        string lowerInvariant = frame.GetMethod().Module.ToString().ToLowerInvariant();
        if (!(lowerInvariant == "microsoft.visualstudio.telemetry.dll") && !(lowerInvariant == "microsoft.visualstudio.telemetry.package.dll"))
          stackFrameList.Add(frame);
      }
      return (IEnumerable<StackFrame>) stackFrameList;
    }

    internal static string FormatException(Exception ex)
    {
      string str1 = WatsonReport.FormatExceptionStack(ex);
      string str2 = string.IsNullOrEmpty(ex.Message) ? string.Empty : ex.Message + Environment.NewLine;
      return string.Format("{0}{1}{2}{3}", (object) ex.GetType(), (object) Environment.NewLine, (object) str2, (object) str1);
    }

    internal static string FormatExceptionStack(Exception ex, bool shorten = false)
    {
      string str1;
      if (ex.StackTrace == null)
      {
        str1 = string.Empty;
      }
      else
      {
        IEnumerable<StackFrame> frames = (IEnumerable<StackFrame>) new StackTrace(ex, false).GetFrames();
        if (frames == null || !frames.Any<StackFrame>())
        {
          Regex regex = new Regex("^   \\S+ (\\S+?)\\((.*?)\\)(?: .*)?$");
          string[] strArray = ex.StackTrace.Split(new char[2]
          {
            '\n',
            '\r'
          }, StringSplitOptions.RemoveEmptyEntries);
          new char[1][0] = ',';
          new char[1][0] = ' ';
          StringBuilder stringBuilder = new StringBuilder();
          foreach (string input in strArray)
          {
            Match match = regex.Match(input);
            if (match.Success)
            {
              stringBuilder.Append(match.Groups[1].Value + "(");
              string str2 = match.Groups[2].Value;
              stringBuilder.Append(str2);
              stringBuilder.AppendLine(")");
            }
            else if (input.Contains("End of stack trace from previous location where exception was thrown") || input.Contains("End of inner exception stack trace"))
              stringBuilder.AppendLine(input);
            else
              stringBuilder.AppendLine(" ");
          }
          str1 = stringBuilder.ToString();
        }
        else
          str1 = WatsonReport.FormatStackTrace(frames, shorten: shorten);
      }
      return str1;
    }

    internal static string FormatStackTrace(
      IEnumerable<StackFrame> stackFrames,
      int maxLength = 2147483647,
      bool shorten = false)
    {
      StringBuilder stringBuilder = new StringBuilder(512);
      foreach (StackFrame stackFrame in stackFrames)
      {
        string str = WatsonReport.FormatMethodName(stackFrame.GetMethod(), shorten);
        if (!string.IsNullOrEmpty(str))
        {
          if (stringBuilder.Length != 0)
            stringBuilder.Append(Environment.NewLine);
          stringBuilder.Append(str);
          if (stringBuilder.Length >= maxLength)
          {
            stringBuilder.Length = maxLength;
            break;
          }
        }
      }
      return stringBuilder.ToString();
    }

    internal static string FormatMethodName(MethodBase method, bool shortened = false)
    {
      if (method == (MethodBase) null)
        return (string) null;
      StringBuilder stringBuilder = new StringBuilder((int) byte.MaxValue);
      Type declaringType = method.DeclaringType;
      if (declaringType != (Type) null)
      {
        string str = declaringType.FullName.Replace('+', '.');
        stringBuilder.Append(str);
        stringBuilder.Append(".");
      }
      stringBuilder.Append(method.Name);
      MethodInfo methodInfo = method as MethodInfo;
      if (methodInfo != (MethodInfo) null && methodInfo.IsGenericMethod)
      {
        Type[] genericArguments = methodInfo.GetGenericArguments();
        stringBuilder.Append('[');
        for (int index = 0; index < genericArguments.Length; ++index)
        {
          if (index != 0)
            stringBuilder.Append(',');
          stringBuilder.Append(genericArguments[index].Name);
        }
        stringBuilder.Append(']');
      }
      stringBuilder.Append('(');
      ParameterInfo[] parameters = method.GetParameters();
      if (parameters != null)
      {
        for (int index = 0; index < parameters.Length; ++index)
        {
          if (index != 0)
          {
            stringBuilder.Append(",");
            if (!shortened)
              stringBuilder.Append(" ");
          }
          string str = "<UnknownType>";
          if (parameters[index].ParameterType != (Type) null)
            str = parameters[index].ParameterType.Name;
          stringBuilder.Append(str);
          if (!shortened)
            stringBuilder.Append(" " + parameters[index].Name);
        }
      }
      stringBuilder.Append(')');
      return stringBuilder.ToString();
    }

    private void SetBucketParametersForModule(string filename, string methodName, string offset)
    {
      string str = string.Empty;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (!string.IsNullOrEmpty(filename))
      {
        string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(filename);
        if (File.Exists(filename))
        {
          str = FileVersionInfo.GetVersionInfo(filename).FileVersion;
          int length = str.IndexOf(" built by", StringComparison.InvariantCultureIgnoreCase);
          if (length > 0)
            str = str.Substring(0, length);
        }
        this.TrySetBucketParameter(4, withoutExtension);
        this.FaultEvent.Properties["VS.Fault.Exception.ModuleVersion"] = (object) str;
      }
      this.TrySetBucketParameter(5, methodName);
      this.FaultEvent.Properties["VS.Fault.Exception.Offset"] = (object) offset;
    }

    internal void AddExceptionInfoToIncludedFile(Exception exceptionObject)
    {
      int indentLevel = 0;
      string desc = "Exception:";
      for (; exceptionObject != null; exceptionObject = exceptionObject.InnerException)
      {
        if (exceptionObject is AggregateException)
        {
          int num = 0;
          foreach (Exception innerException in ((AggregateException) exceptionObject).InnerExceptions)
            this.AddStackToFile(string.Format("Aggregate # {0}", (object) num++), WatsonReport.FormatException(innerException), indentLevel);
        }
        else
          this.AddStackToFile(desc, WatsonReport.FormatException(exceptionObject), indentLevel);
        ++indentLevel;
        desc = "Inner Exception:";
      }
    }

    private void AddStackToFile(string desc, string stack, int indentLevel)
    {
      string str1 = new string(' ', indentLevel * 4);
      this.SBuilderErrorInfo.AppendLine();
      if (!string.IsNullOrEmpty(desc))
        this.SBuilderErrorInfo.AppendLine(str1 + " " + desc);
      string str2 = stack;
      char[] separator = new char[2]{ '\r', '\n' };
      foreach (string str3 in str2.Split(separator, StringSplitOptions.RemoveEmptyEntries))
        this.SBuilderErrorInfo.AppendLine(str1 + " " + str3);
    }

    private void AddPropertiesForExceptionObject(Exception exceptionObject)
    {
      if (exceptionObject == null)
        return;
      Exception innerMostException = this.GetInnerMostException(exceptionObject, (Func<Exception, bool>) (ex => !string.IsNullOrEmpty(ex.StackTrace)));
      if (innerMostException != null)
        exceptionObject = innerMostException;
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.TypeString", (object) exceptionObject.GetType().FullName);
      this.AddStackToTelemetryReport(WatsonReport.FormatExceptionStack(exceptionObject, true), exceptionObject.Message);
      if (exceptionObject is TypeLoadException)
      {
        TypeLoadException typeLoadException = (TypeLoadException) exceptionObject;
        if (!string.IsNullOrEmpty(typeLoadException.TypeName))
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.TypeName", (object) typeLoadException.TypeName);
      }
      if (exceptionObject is ObjectDisposedException)
      {
        ObjectDisposedException disposedException = (ObjectDisposedException) exceptionObject;
        if (!string.IsNullOrEmpty(disposedException.ObjectName))
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.ObjectName", (object) disposedException.ObjectName);
      }
      if (exceptionObject is FileNotFoundException)
      {
        FileNotFoundException notFoundException = (FileNotFoundException) exceptionObject;
        if (!string.IsNullOrEmpty(notFoundException.FileName))
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.FileName", (object) notFoundException.FileName);
        if (!string.IsNullOrEmpty(notFoundException.FusionLog))
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.FusionLog", (object) notFoundException.FusionLog);
      }
      if (exceptionObject is ReflectionTypeLoadException)
      {
        ReflectionTypeLoadException typeLoadException = (ReflectionTypeLoadException) exceptionObject;
        if (typeLoadException.LoaderExceptions != null)
        {
          StringBuilder stringBuilder = new StringBuilder();
          foreach (Exception loaderException in typeLoadException.LoaderExceptions)
            stringBuilder.AppendLine(WatsonReport.FormatException(loaderException));
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.ExceptionArray", (object) stringBuilder.ToString());
        }
      }
      if (exceptionObject is ArgumentException)
      {
        ArgumentException argumentException = (ArgumentException) exceptionObject;
        if (!string.IsNullOrEmpty(argumentException.ParamName))
          this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.ParamName", (object) argumentException.ParamName);
      }
      this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.ErrorCode", (object) exceptionObject.HResult);
    }

    private void AddStackToTelemetryReport(string stackTrace, string message = null, bool exceptionStack = true)
    {
      string userName = Environment.UserName;
      if (userName != null)
        message = message?.Replace(userName, "[UserName]");
      if (message != null)
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.Message", (object) message);
      if (stackTrace == null)
        return;
      TelemetryComplexProperty telemetryComplexProperty = new TelemetryComplexProperty((object) stackTrace.Split(new char[2]
      {
        '\n',
        '\r'
      }, StringSplitOptions.RemoveEmptyEntries));
      if (exceptionStack)
        this.FaultEvent.ReservedProperties.AddPrefixed("Reserved.DataModel.Fault.Exception.StackTrace", (object) telemetryComplexProperty);
      else
        this.FaultEvent.Properties.Add("VS.Fault.CallerStackTrace", (object) telemetryComplexProperty);
    }

    private Exception GetInnerMostException(
      Exception exceptionObject,
      Func<Exception, bool> exceptionChainRestraint)
    {
      Exception innerMostException = exceptionObject;
      List<Exception> exceptionList = new List<Exception>()
      {
        exceptionObject
      };
      for (; exceptionObject.InnerException != null; exceptionObject = exceptionObject.InnerException)
        exceptionList.Add(exceptionObject.InnerException);
      if (exceptionList.Count != 1)
      {
        if (exceptionChainRestraint == null)
        {
          innerMostException = exceptionObject;
        }
        else
        {
          exceptionList.Reverse();
          foreach (Exception exception in exceptionList)
          {
            if (exceptionChainRestraint(exception))
            {
              innerMostException = exception;
              break;
            }
          }
        }
      }
      return innerMostException;
    }
  }
}
