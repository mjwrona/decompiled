// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.FaultEvent
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.VisualStudio.Telemetry
{
  public sealed class FaultEvent : TelemetryEvent, IFaultUtility
  {
    internal const string InternalFaultEventName = "VS/Telemetry/InternalFault";
    internal const int WatsonMaxParamLength = 255;
    [Obsolete("This property is obsolete. Use TelemetrySession.BucketFiltersToEnableWatsonForFaults to enable Watson reports for fault events. They are disabled by default.", false)]
    public static List<BucketFilter> BucketFiltersToDisableWatsonReport = new List<BucketFilter>();
    [Obsolete("This property is obsolete. Use TelemetrySession.BucketFiltersToAddDumpsToFaults to add process dumps to fault events. They are disabled by default.", false)]
    public static List<BucketFilter> BucketFiltersToAddProcessDump = new List<BucketFilter>();
    internal const string watsonEventTypeVisualStudioNonFatalErrors2 = "VisualStudioNonFatalErrors2";
    internal FaultEvent.FaultEventWatsonOptIn UserOptInToWatson;

    public static int? WatsonSamplePercent { get; set; }

    public static int? MaximumWatsonReportsPerSession { get; set; }

    public static int? MinimumSecondsBetweenWatsonReports { get; set; }

    public bool? IsIncludedInWatsonSample { get; set; }

    public WER_DUMP_TYPE DumpCollectionType { get; set; } = WER_DUMP_TYPE.WerDumpTypeMiniDump;

    public string WatsonEventType { get; set; } = "VisualStudioNonFatalErrors2";

    public bool SynchronousDumpCollection { get; set; }

    internal string Description { get; }

    internal Exception ExceptionObject { get; }

    internal Func<IFaultUtility, int> GatherEventDetails { get; }

    internal FaultSeverity FaultSeverity { get; set; }

    internal bool PostThisEventToTelemetry { get; set; } = true;

    internal string[] BucketParameters { get; set; } = new string[10];

    internal List<int> ListProcessIdsToDump { get; } = new List<int>();

    internal List<string> ListFilesToAdd { get; } = new List<string>();

    internal StringBuilder SBuilderAdditionalUserErrorInfo { get; } = new StringBuilder();

    internal bool AddedErrorInformation { get; set; }

    internal bool AddedProcessDump { get; set; }

    internal bool AddedFile { get; set; }

    public FaultEvent(
      string eventName,
      string description,
      Exception exceptionObject = null,
      Func<IFaultUtility, int> gatherEventDetails = null)
      : this(eventName, description, FaultSeverity.Uncategorized, exceptionObject, gatherEventDetails)
    {
    }

    public FaultEvent(
      string eventName,
      string description,
      FaultSeverity faultSeverity,
      Exception exceptionObject = null,
      Func<IFaultUtility, int> gatherEventDetails = null)
      : base(eventName, TelemetrySeverity.High, DataModelEventType.Fault)
    {
      this.Description = description ?? string.Empty;
      this.ExceptionObject = exceptionObject;
      this.GatherEventDetails = gatherEventDetails;
      this.FaultSeverity = faultSeverity;
      this.UserOptInToWatson = FaultEvent.FaultEventWatsonOptIn.Unspecified;
      DataModelEventNameHelper.SetProductFeatureEntityName(this);
    }

    public void AddErrorInformation(string information)
    {
      if (string.IsNullOrEmpty(information))
        return;
      this.AddedErrorInformation = true;
      string[] array = information.Split(new char[2]
      {
        '\r',
        '\n'
      }, StringSplitOptions.RemoveEmptyEntries);
      this.SBuilderAdditionalUserErrorInfo.AppendLine();
      Action<string> action = (Action<string>) (line => this.SBuilderAdditionalUserErrorInfo.AppendLine("   " + line));
      Array.ForEach<string>(array, action);
    }

    public void AddProcessDump(int pid)
    {
      this.AddedProcessDump = true;
      this.ListProcessIdsToDump.Add(pid);
    }

    public void SetBucketParameter(int bucketNumber, string newBucketValue)
    {
      if (bucketNumber < 0 || bucketNumber >= 10)
        return;
      if (string.IsNullOrEmpty(newBucketValue))
        newBucketValue = string.Empty;
      this.BucketParameters[bucketNumber] = FaultEvent.TrucateToMaxWatsonParamLength(newBucketValue.Trim());
    }

    public void SetAppName(string appName) => this.SetBucketParameter(0, appName);

    public void SetAppVersion(string appVersion) => this.SetBucketParameter(1, appVersion);

    public void SetFailureParameters(
      string failureParameter0 = null,
      string failureParameter1 = null,
      string failureParameter2 = null,
      string failureParameter3 = null,
      string failureParameter4 = null)
    {
      List<string> stringList = new List<string>()
      {
        failureParameter0,
        failureParameter1,
        failureParameter2,
        failureParameter3,
        failureParameter4
      };
      for (int index = 0; index < stringList.Count; ++index)
      {
        int bucketNumber = index + 3;
        string newBucketValue = stringList[index] ?? this.GetBucketParameter(bucketNumber);
        this.SetBucketParameter(bucketNumber, newBucketValue);
      }
    }

    public void SetNonFailureParameters(string nonFailureParameter0 = null, string nonFailureParameter1 = null)
    {
      this.SetBucketParameter(8, nonFailureParameter0 ?? this.GetBucketParameter(8));
      this.SetBucketParameter(9, nonFailureParameter1 ?? this.GetBucketParameter(9));
    }

    internal static string TrucateToMaxWatsonParamLength(string input) => input.Length > (int) byte.MaxValue ? input.Substring(0, (int) byte.MaxValue) : input;

    public string GetBucketParameter(int bucketNumber)
    {
      string bucketParameter = string.Empty;
      if (bucketNumber >= 0 && bucketNumber < 10)
        bucketParameter = this.BucketParameters[bucketNumber];
      return bucketParameter;
    }

    public void AddFile(string fullPathFileName)
    {
      this.AddedFile = true;
      this.ListFilesToAdd.Add(fullPathFileName);
    }

    public override string ToString() => string.Format("{0} IsSampled = {1}", (object) base.ToString(), (object) this.IsIncludedInWatsonSample);

    internal static WER_DUMP_TYPE GetDumpTypeFromString(string dumpType)
    {
      dumpType = dumpType.ToUpper();
      switch (dumpType)
      {
        case "MICRO":
          return WER_DUMP_TYPE.WerDumpTypeMicroDump;
        case "MINI":
          return WER_DUMP_TYPE.WerDumpTypeMiniDump;
        case "HEAP":
          return WER_DUMP_TYPE.WerDumpTypeHeapDump;
        case "TRIAGE":
          return WER_DUMP_TYPE.WerDumpTypeTriageDump;
        default:
          return WER_DUMP_TYPE.WerDumpTypeMax;
      }
    }

    internal enum FaultEventWatsonOptIn
    {
      Unspecified,
      PropertyOptIn,
      CallbackOptIn,
      PropertyOptOut,
      CallbackOptOut,
      CallbackException,
    }
  }
}
