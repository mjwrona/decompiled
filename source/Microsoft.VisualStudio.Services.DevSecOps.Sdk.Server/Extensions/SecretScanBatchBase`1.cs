// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.SecretScanBatchBase`1
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public abstract class SecretScanBatchBase<TEventArg> where TEventArg : class
  {
    public SecretScanBatchBase(
      string serviceArea,
      string serviceFeature,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceArea, nameof (serviceArea));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceFeature, nameof (serviceFeature));
      this.ServiceArea = serviceArea;
      this.ServiceFeature = serviceFeature;
      this.ServiceLayer = this.GetType().Name;
      this.ClientTraceData = new ClientTraceData();
      this.CancellationToken = cancellationToken;
    }

    public string ServiceArea { get; }

    public string ServiceFeature { get; }

    public string ServiceLayer { get; }

    protected ClientTraceData ClientTraceData { get; private set; }

    protected CancellationToken CancellationToken { get; private set; }

    protected abstract string ComposeErrorMessge(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      IReadOnlyList<string> userFacingLocations);

    protected abstract bool IsBlockingEnabled(
      IVssRequestContext requestContext,
      IDictionary<int, TEventArg> eventArgs);

    protected abstract BypassType DetermineBypassType(
      IVssRequestContext requestContext,
      TEventArg eventArg);

    protected internal BypassType DetermineBypassTypeFromComment(string comment)
    {
      if (comment != null && comment.IndexOf("**BYPASS_SECRET_SCANNING**", StringComparison.OrdinalIgnoreCase) >= 0)
        return BypassType.Normal;
      return comment == null || comment.IndexOf("4CE71094-6DCC-41B0-A1FA-CC3EF3228F4E", StringComparison.OrdinalIgnoreCase) < 0 ? BypassType.None : BypassType.BreakGlass;
    }

    protected abstract bool IsReportableLocation(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      Violation violation,
      out string userFacingLocation);

    protected abstract ScanResult PerformScan(
      IVssRequestContext requestContext,
      IDictionary<int, TEventArg> eventArgs,
      IDictionary<int, object> requestContent);

    protected abstract object GetSerializableScanContent(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      out int contentSize);

    protected virtual bool IsPrescriptiveBlockingEnabled(IVssRequestContext requestContext) => false;

    protected virtual bool ShouldScan(
      IVssRequestContext requestContext,
      IDictionary<int, TEventArg> eventArgs)
    {
      return requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsServicingContext && requestContext.IsMicrosoftTenant() && eventArgs != null;
    }

    public IDictionary<int, string> ProcessBatch(
      IVssRequestContext requestContext,
      IDictionary<int, TEventArg> eventArgs,
      int maxBatchContentLength)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(27009028, this.ServiceArea, this.ServiceLayer, nameof (ProcessBatch));
      Dictionary<int, string> dictionary1 = (Dictionary<int, string>) null;
      this.ClientTraceData = new ClientTraceData();
      try
      {
        if (eventArgs.IsNullOrEmpty<KeyValuePair<int, TEventArg>>())
          return (IDictionary<int, string>) dictionary1;
        this.ClientTraceData.Add("NumberOfEventArgs", (object) eventArgs.Keys.Count);
        bool flag = this.ShouldScan(requestContext, eventArgs);
        this.ClientTraceData.Add("ShouldScan", (object) flag);
        if (!flag)
          return (IDictionary<int, string>) dictionary1;
        bool isBlockingEnabled = this.IsBlockingEnabled(requestContext, eventArgs);
        this.ClientTraceData.Add("IsBlockingEnabled", (object) isBlockingEnabled);
        bool isPrescriptiveBlockingEnabled = this.IsPrescriptiveBlockingEnabled(requestContext);
        this.ClientTraceData.Add("PrescriptiveBlockingEnabled", (object) isPrescriptiveBlockingEnabled);
        int num1 = 0;
        int num2 = 0;
        Dictionary<int, object> dictionary2 = new Dictionary<int, object>();
        foreach (int key in (IEnumerable<int>) eventArgs.Keys)
        {
          TEventArg eventArg = eventArgs[key];
          if ((object) eventArg != null)
          {
            int contentSize;
            object serializableScanContent = this.GetSerializableScanContent(requestContext, eventArg, out contentSize);
            if (serializableScanContent != null && contentSize > 0)
            {
              if (num2 + contentSize > maxBatchContentLength && !dictionary2.IsNullOrEmpty<KeyValuePair<int, object>>())
              {
                ++num1;
                IDictionary<int, string> enumerable = this.ProcessBatchScan(requestContext, eventArgs, (IDictionary<int, object>) dictionary2, isBlockingEnabled, isPrescriptiveBlockingEnabled);
                if (isBlockingEnabled && !enumerable.IsNullOrEmpty<KeyValuePair<int, string>>())
                {
                  dictionary1 = dictionary1 ?? new Dictionary<int, string>();
                  dictionary1.TryAddRange<int, string, Dictionary<int, string>>((IEnumerable<KeyValuePair<int, string>>) enumerable);
                }
                num2 = 0;
                dictionary2 = new Dictionary<int, object>();
              }
              if (contentSize <= maxBatchContentLength)
              {
                dictionary2.Add(key, serializableScanContent);
                num2 += contentSize;
              }
              else
                TelemetryHelper.AddToResults(this.ClientTraceData, "ExceedsMaxContentSizeLimit", string.Format("{0}:{1}", (object) key, (object) contentSize));
            }
          }
        }
        if (!dictionary2.IsNullOrEmpty<KeyValuePair<int, object>>())
        {
          ++num1;
          IDictionary<int, string> enumerable = this.ProcessBatchScan(requestContext, eventArgs, (IDictionary<int, object>) dictionary2, isBlockingEnabled, isPrescriptiveBlockingEnabled);
          if (isBlockingEnabled && !enumerable.IsNullOrEmpty<KeyValuePair<int, string>>())
          {
            dictionary1 = dictionary1 ?? new Dictionary<int, string>();
            dictionary1.TryAddRange<int, string, Dictionary<int, string>>((IEnumerable<KeyValuePair<int, string>>) enumerable);
          }
        }
        this.ClientTraceData.Add("NumberOfBatches", (object) num1);
      }
      catch (Exception ex) when (this.CancellationToken.IsCancellationRequested)
      {
        requestContext.TraceException(27009029, this.ServiceArea, this.ServiceLayer, ex);
      }
      catch (Exception ex)
      {
        dictionary1 = (Dictionary<int, string>) null;
        requestContext.TraceException(27009028, this.ServiceArea, this.ServiceLayer, ex);
      }
      finally
      {
        requestContext.TraceLeave(27009028, this.ServiceArea, this.ServiceLayer);
        stopwatch.Stop();
        this.ClientTraceData.Add("ProcessBatchDurationInMilliseconds", (object) stopwatch.ElapsedMilliseconds);
        this.PublishClientTrace(requestContext);
      }
      return (IDictionary<int, string>) dictionary1;
    }

    private IDictionary<int, string> ProcessBatchScan(
      IVssRequestContext requestContext,
      IDictionary<int, TEventArg> eventArgs,
      IDictionary<int, object> requestContent,
      bool isBlockingEnabled,
      bool isPrescriptiveBlockingEnabled)
    {
      IDictionary<int, string> dictionary1 = (IDictionary<int, string>) null;
      ScanResult scanResult = this.PerformScan(requestContext, eventArgs, requestContent);
      if (scanResult != null && scanResult.Violations.Count > 0)
      {
        List<Violation> source = new List<Violation>();
        Dictionary<int, List<string>> dictionary2 = new Dictionary<int, List<string>>();
        Dictionary<int, HashSet<BlockType>> blockTypeMapping = new Dictionary<int, HashSet<BlockType>>();
        Dictionary<int, HashSet<BypassType>> bypassTypeMapping = new Dictionary<int, HashSet<BypassType>>();
        foreach (Violation violation in (IEnumerable<Violation>) scanResult.Violations)
        {
          int index;
          if (this.TryGetIndexFromLocation(violation.LogicalLocation, out index) && eventArgs.ContainsKey(index))
          {
            TEventArg eventArg = eventArgs[index];
            string userFacingLocation;
            if ((object) eventArg != null && this.IsReportableLocation(requestContext, eventArg, violation, out userFacingLocation))
            {
              BypassType bypassType = this.DetermineBypassType(requestContext, eventArg);
              SecretScanBatchBase<TEventArg>.ProcessViolation(isPrescriptiveBlockingEnabled, dictionary2, blockTypeMapping, bypassTypeMapping, violation, index, userFacingLocation, bypassType);
              source.Add(violation);
            }
          }
        }
        if (isBlockingEnabled && !dictionary2.IsNullOrEmpty<KeyValuePair<int, List<string>>>())
        {
          foreach (int key in dictionary2.Keys)
          {
            TEventArg eventArg = eventArgs[key];
            string str = this.ComposeErrorMessge(requestContext, eventArg, (IReadOnlyList<string>) dictionary2[key]);
            dictionary1 = dictionary1 ?? (IDictionary<int, string>) new Dictionary<int, string>();
            dictionary1.Add(key, str);
          }
        }
        if (bypassTypeMapping.Count > 0)
        {
          foreach (KeyValuePair<int, HashSet<BypassType>> keyValuePair in bypassTypeMapping)
            TelemetryHelper.AddToResults(this.ClientTraceData, "BypassType", SecretScanBatchBase<TEventArg>.ComposeBypassTypeInformation(keyValuePair.Key, keyValuePair.Value));
        }
        if (blockTypeMapping.Count > 0)
        {
          foreach (KeyValuePair<int, HashSet<BlockType>> keyValuePair in blockTypeMapping)
            TelemetryHelper.AddToResults(this.ClientTraceData, "BlockType", SecretScanBatchBase<TEventArg>.ComposeBlockTypeInformation(keyValuePair.Key, keyValuePair.Value));
        }
        if (source != null && source.Count > 0)
          this.ClientTraceData.AddRangeToResults("Issues", (IEnumerable<string>) source.Select<Violation, string>((Func<Violation, string>) (v => v.ToJson())).ToArray<string>());
      }
      return dictionary1;
    }

    internal static void ProcessViolation(
      bool isPrescriptiveBlockingEnabled,
      Dictionary<int, List<string>> blockingLocations,
      Dictionary<int, HashSet<BlockType>> blockTypeMapping,
      Dictionary<int, HashSet<BypassType>> bypassTypeMapping,
      Violation violation,
      int index,
      string reportableLocation,
      BypassType bypassType)
    {
      bool flag = bypassType != 0;
      bool containsHighConfidenceSecretKinds = HighConfidenceSecretKinds.IsHighConfidenceSecretKind(violation);
      BlockType blockType = HighConfidenceSecretKinds.GetBlockType(isPrescriptiveBlockingEnabled, containsHighConfidenceSecretKinds, bypassType);
      if (((!isPrescriptiveBlockingEnabled ? 0 : (bypassType == BypassType.Normal ? 1 : 0)) & (containsHighConfidenceSecretKinds ? 1 : 0)) != 0)
      {
        flag = false;
        bypassType = BypassType.None;
      }
      if (!bypassTypeMapping.ContainsKey(index))
        bypassTypeMapping.Add(index, new HashSet<BypassType>());
      if (!blockTypeMapping.ContainsKey(index))
        blockTypeMapping.Add(index, new HashSet<BlockType>());
      blockTypeMapping[index].Add(blockType);
      bypassTypeMapping[index].Add(bypassType);
      if (flag)
        return;
      if (!blockingLocations.ContainsKey(index))
        blockingLocations.Add(index, new List<string>());
      blockingLocations[index].Add(reportableLocation);
    }

    private bool TryGetIndexFromLocation(string location, out int index)
    {
      if (!string.IsNullOrWhiteSpace(location))
      {
        int length = location.IndexOf('[');
        int result;
        if (length > 0 && int.TryParse(location.Substring(0, length), out result))
        {
          index = result;
          return true;
        }
        TelemetryHelper.AddToResults(this.ClientTraceData, "InvalidGetIndexFromLocation", location);
      }
      index = -1;
      return false;
    }

    private void PublishClientTrace(IVssRequestContext requestContext)
    {
      if (!requestContext.IsMicrosoftTenant())
        return;
      requestContext.GetService<ClientTraceService>().Publish(requestContext, this.ServiceArea, this.ServiceFeature, this.ClientTraceData);
    }

    internal static string ComposeBypassTypeInformation(int index, HashSet<BypassType> bypassTypes)
    {
      if (bypassTypes.Any<BypassType>((Func<BypassType, bool>) (bt => bt == BypassType.PolicySetting)))
        return string.Format("{0}-{1}", (object) index, (object) BypassType.PolicySetting);
      if (bypassTypes.Any<BypassType>((Func<BypassType, bool>) (bt => bt == BypassType.BreakGlass)))
        return string.Format("{0}-{1}", (object) index, (object) BypassType.BreakGlass);
      return bypassTypes.Any<BypassType>((Func<BypassType, bool>) (bt => bt == BypassType.Normal)) ? string.Format("{0}-{1}", (object) index, (object) BypassType.Normal) : string.Format("{0}-{1}", (object) index, (object) BypassType.None);
    }

    internal static string ComposeBlockTypeInformation(int index, HashSet<BlockType> blockTypes)
    {
      if (blockTypes.Any<BlockType>((Func<BlockType, bool>) (bt => bt == BlockType.Prescriptive)))
        return string.Format("{0}-{1}", (object) index, (object) BlockType.Prescriptive);
      return blockTypes.Any<BlockType>((Func<BlockType, bool>) (bt => bt == BlockType.Normal)) ? string.Format("{0}-{1}", (object) index, (object) BlockType.Normal) : string.Format("{0}-{1}", (object) index, (object) BlockType.None);
    }
  }
}
