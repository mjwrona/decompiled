// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions.SecretScanSubscriberBase`1
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
using System.Linq;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.Extensions
{
  public abstract class SecretScanSubscriberBase<TEventArg> where TEventArg : class
  {
    private static readonly Type[] SubscribedTypesArray = new Type[1]
    {
      typeof (TEventArg)
    };

    public SecretScanSubscriberBase(string serviceArea, string serviceFeature)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceArea, nameof (serviceArea));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(serviceFeature, nameof (serviceFeature));
      this.ServiceArea = serviceArea;
      this.ServiceFeature = serviceFeature;
      this.ServiceLayer = this.GetType().Name;
      this.Name = this.GetType().Name;
      this.Priority = SubscriberPriority.Normal;
      this.ClientTraceData = new ClientTraceData();
    }

    public string Name { get; }

    public SubscriberPriority Priority { get; }

    public string ServiceArea { get; }

    public string ServiceFeature { get; }

    public string ServiceLayer { get; }

    protected ClientTraceData ClientTraceData { get; private set; }

    protected string SecretScanCredentialLocationsPropertyName => "Microsoft.AzureDevOps.SecretScan.CredentialLocations";

    protected abstract string ComposeErrorMessage(
      IVssRequestContext requestContext,
      TEventArg eventArg,
      IReadOnlyList<string> userFacingLocations,
      bool usePrescriptiveBlockMessage = false);

    protected abstract bool IsBlockingEnabled(IVssRequestContext requestContext);

    protected abstract BypassType DetermineBypassType(TEventArg eventArg);

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

    protected abstract ScanResult PerformScan(IVssRequestContext requestContext, TEventArg eventArg);

    protected virtual bool IsPrescriptiveBlockingEnabled(IVssRequestContext requestContext) => false;

    protected virtual bool ShouldScan(IVssRequestContext requestContext, TEventArg eventArg) => requestContext.ExecutionEnvironment.IsHostedDeployment && !requestContext.IsServicingContext && requestContext.IsMicrosoftTenant() && (object) eventArg != null;

    public virtual TEventArg CloneAndModifyEvent(TEventArg eventArg) => eventArg;

    public EventNotificationStatus ProcessEvent(
      IVssRequestContext requestContext,
      NotificationType notificationType,
      object notificationEventArgs,
      out int statusCode,
      out string statusMessage,
      out ExceptionPropertyCollection properties)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceEnter(27009027, this.ServiceArea, this.ServiceLayer, nameof (ProcessEvent));
      statusCode = 0;
      properties = (ExceptionPropertyCollection) null;
      statusMessage = (string) null;
      TEventArg eventArg = this.CloneAndModifyEvent(notificationEventArgs as TEventArg);
      EventNotificationStatus status = EventNotificationStatus.ActionPermitted;
      this.ClientTraceData = new ClientTraceData();
      try
      {
        bool flag1 = this.ShouldScan(requestContext, eventArg);
        this.ClientTraceData.Add("ShouldScan", (object) flag1);
        this.ClientTraceData.Add("NotificationType", (object) notificationType);
        if (!flag1 || notificationType != NotificationType.DecisionPoint)
          return EventNotificationStatus.ActionPermitted;
        ScanResult scanResult = this.PerformScan(requestContext, eventArg);
        BlockType blockType = BlockType.None;
        BypassType bypassType = this.DetermineBypassType(eventArg);
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
              userFacingLocations.Add(userFacingLocation + ": " + violation.RuleName);
              bool flag3 = HighConfidenceSecretKinds.IsHighConfidenceSecretKind(violation);
              if (flag3)
                containsHighConfidenceSecretKinds |= flag3;
            }
            else if (!violation.MatchState.Equals("NotSuppressed", StringComparison.Ordinal))
              source2.Add(violation);
          }
          bool flag4 = bypassType != 0;
          if (((!isPrescriptiveBlockingEnabled ? 0 : (bypassType == BypassType.Normal ? 1 : 0)) & (containsHighConfidenceSecretKinds ? 1 : 0)) != 0)
            flag4 = false;
          if (source1.Any<Violation>() & flag2 && !flag4)
          {
            statusMessage = this.ComposeErrorMessage(requestContext, eventArg, (IReadOnlyList<string>) userFacingLocations, isPrescriptiveBlockingEnabled & containsHighConfidenceSecretKinds);
            if (properties == null)
              properties = new ExceptionPropertyCollection();
            properties.Set(this.SecretScanCredentialLocationsPropertyName, source1.Select<Violation, string>((Func<Violation, string>) (v => v.LogicalLocation)).ToArray<string>());
            if (isPrescriptiveBlockingEnabled)
              this.ClientTraceData.Add("PrescriptiveBlockingPerformed", (object) containsHighConfidenceSecretKinds);
            statusCode = -1;
            blockType = HighConfidenceSecretKinds.GetBlockType(isPrescriptiveBlockingEnabled, containsHighConfidenceSecretKinds, bypassType);
            status = EventNotificationStatus.ActionDenied;
          }
          this.ClientTraceData.AddRangeToResults("Issues", (IEnumerable<string>) source1.Select<Violation, string>((Func<Violation, string>) (v => v.ToJson())).ToArray<string>());
          this.ClientTraceData.AddRangeToResults("Issues", (IEnumerable<string>) source2.Select<Violation, string>((Func<Violation, string>) (v => v.ToJson())).ToArray<string>());
        }
        this.ClientTraceData.Add("BlockType", (object) blockType.ToString());
        this.ClientTraceData.Add("BypassType", (object) bypassType.ToString());
        this.ClientTraceData.Add("IsBlockingEnabled", (object) flag2);
        this.ClientTraceData.Add("PrescriptiveBlockingEnabled", (object) isPrescriptiveBlockingEnabled);
      }
      catch (Exception ex)
      {
        status = EventNotificationStatus.ActionPermitted;
        requestContext.TraceException(27009027, this.ServiceArea, this.ServiceLayer, ex);
      }
      finally
      {
        requestContext.TraceLeave(27009027, this.ServiceArea, this.ServiceLayer);
        this.PublishClientTrace(requestContext, status);
      }
      return status;
    }

    private void PublishClientTrace(
      IVssRequestContext requestContext,
      EventNotificationStatus status)
    {
      if (!requestContext.IsMicrosoftTenant())
        return;
      this.ClientTraceData.Add("EventNotificationStatus", (object) status.ToString());
      requestContext.GetService<ClientTraceService>().Publish(requestContext, this.ServiceArea, this.ServiceFeature, this.ClientTraceData);
    }

    public Type[] SubscribedTypes() => SecretScanSubscriberBase<TEventArg>.SubscribedTypesArray;
  }
}
