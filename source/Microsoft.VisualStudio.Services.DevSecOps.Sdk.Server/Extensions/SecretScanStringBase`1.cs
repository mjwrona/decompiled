// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.SecretScanStringBase`1
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.DevSecOps.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public abstract class SecretScanStringBase<TEventArg> where TEventArg : class
  {
    public SecretScanStringBase(
      string serviceArea,
      string serviceFeature,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceArea, nameof (serviceArea));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceFeature, nameof (serviceFeature));
      this.ServiceArea = serviceArea;
      this.ServiceFeature = serviceFeature;
      this.ServiceLayer = this.GetType().Name;
      this.Name = this.GetType().Name;
      this.ClientTraceData = new ClientTraceData();
      this.CancellationToken = cancellationToken;
    }

    public string Name { get; }

    public string ServiceArea { get; }

    public string ServiceFeature { get; }

    public string ServiceLayer { get; }

    protected ClientTraceData ClientTraceData { get; private set; }

    protected CancellationToken CancellationToken { get; private set; }

    protected abstract string ComposeErrorMessage(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      IReadOnlyList<string> userFacingLocations);

    protected abstract bool IsBlockingEnabled(IVssRequestContext requestContext);

    protected abstract BypassType DetermineBypassType(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      string content);

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
      TEventArg eventArg,
      string content);

    protected virtual bool IsPrescriptiveBlockingEnabled(IVssRequestContext requestContext) => false;

    protected virtual bool ShouldScan(IVssRequestContext requestContext, TEventArg eventArg) => requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsServicingContext && requestContext.IsMicrosoftTenant() && (object) eventArg != null;

    public void Process(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      string content,
      out string exceptionMessage)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(27009032, this.ServiceArea, this.ServiceLayer, "ProcessEvent");
      exceptionMessage = (string) null;
      try
      {
        bool flag1 = this.ShouldScan(requestContext, eventArg);
        this.ClientTraceData.Add("ShouldScan", (object) flag1);
        if (!flag1)
          return;
        ScanResult scanResult = this.PerformScan(requestContext, eventArg, content);
        BlockType blockType = BlockType.None;
        BypassType bypassType = this.DetermineBypassType(requestContext, eventArg, content);
        bool flag2 = this.IsBlockingEnabled(requestContext);
        bool isPrescriptiveBlockingEnabled = this.IsPrescriptiveBlockingEnabled(requestContext);
        if (scanResult != null && scanResult.Violations.Count > 0)
        {
          bool containsHighConfidenceSecretKinds = false;
          List<string> userFacingLocations = new List<string>();
          List<Violation> source1 = new List<Violation>();
          List<Violation> source2 = new List<Violation>();
          foreach (Violation violation in (IEnumerable<Violation>) scanResult.Violations)
          {
            string userFacingLocation;
            if (this.IsReportableLocation(requestContext, eventArg, violation, out userFacingLocation))
            {
              source1.Add(violation);
              userFacingLocations.Add(userFacingLocation);
              bool flag3 = HighConfidenceSecretKinds.IsHighConfidenceSecretKind(violation);
              if (flag3)
                containsHighConfidenceSecretKinds |= flag3;
            }
            else
              source2.Add(violation);
          }
          bool flag4 = bypassType != 0;
          if (((!isPrescriptiveBlockingEnabled ? 0 : (bypassType == BypassType.Normal ? 1 : 0)) & (containsHighConfidenceSecretKinds ? 1 : 0)) != 0)
            flag4 = false;
          if (source1.Any<Violation>() & flag2 && !flag4)
          {
            exceptionMessage = this.ComposeErrorMessage(requestContext, eventArg, (IReadOnlyList<string>) userFacingLocations);
            if (isPrescriptiveBlockingEnabled)
              this.ClientTraceData.Add("PrescriptiveBlockingPerformed", (object) containsHighConfidenceSecretKinds);
            blockType = HighConfidenceSecretKinds.GetBlockType(isPrescriptiveBlockingEnabled, containsHighConfidenceSecretKinds, bypassType);
          }
          this.ClientTraceData.AddRangeToResults("Issues", (IEnumerable<string>) source1.Select<Violation, string>((Func<Violation, string>) (v => v.ToJson())).ToArray<string>());
          this.ClientTraceData.AddRangeToResults("NonReportableViolationIssues", (IEnumerable<string>) source2.Select<Violation, string>((Func<Violation, string>) (v => v.ToJson())).ToArray<string>());
        }
        this.ClientTraceData.Add("BlockType", (object) blockType.ToString());
        this.ClientTraceData.Add("BypassType", (object) bypassType.ToString());
        this.ClientTraceData.Add("IsBlockingEnabled", (object) flag2);
        this.ClientTraceData.Add("PrescriptiveBlockingEnabled", (object) isPrescriptiveBlockingEnabled);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(27009032, this.ServiceArea, this.ServiceLayer, ex);
      }
      finally
      {
        requestContext.TraceLeave(27009032, this.ServiceArea, this.ServiceLayer);
        this.PublishClientTrace(requestContext);
      }
    }

    private void PublishClientTrace(IVssRequestContext requestContext)
    {
      if (!requestContext.IsMicrosoftTenant())
        return;
      requestContext.GetService<ClientTraceService>().Publish(requestContext, this.ServiceArea, this.ServiceFeature, this.ClientTraceData);
    }
  }
}
