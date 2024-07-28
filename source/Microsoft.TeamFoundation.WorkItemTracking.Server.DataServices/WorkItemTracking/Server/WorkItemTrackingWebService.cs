// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItemTrackingWebService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 929F0284-16B2-4277-9F4A-B615689A77D1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataServices.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core.WebServices;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services.Protocols;
using System.Xml;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public abstract class WorkItemTrackingWebService : TeamFoundationWebService
  {
    private Lazy<ApiStackRouter> m_callRouter;
    private static readonly IReadOnlyCollection<string> LegacyProcessUpdateActions = (IReadOnlyCollection<string>) new HashSet<string>((IEnumerable<string>) new string[28]
    {
      "DestroyGlobalList",
      "DeleteWorkItemTypeCategoryMember",
      "InsertWorkItemTypeCategoryMember",
      "DestroyWorkItemTypeCategory",
      "UpdateWorkItemTypeCategory",
      "InsertWorkItemTypeCategory",
      "DeleteWorkItemLinkType",
      "UpdateWorkItemLinkType",
      "InsertWorkItemLinkType",
      "UpdateRule",
      "InsertRule",
      "UpdateTreeProperty",
      "InsertTreeProperty",
      "UpdateTree",
      "InsertTree",
      "UpdateFieldUsage",
      "InsertFieldUsage",
      "DeleteField",
      "UpdateField",
      "InsertField",
      "InsertConstantSet",
      "UpdateConstantSet",
      "UpdateWorkItemTypeUsage",
      "InsertWorkItemTypeUsage",
      "RenameWorkItemType",
      "DestroyWorkItemType",
      "UpdateWorkItemType",
      "InsertWorkItemType"
    }, (IEqualityComparer<string>) TFStringComparer.UpdateAction);
    private RequestHeader m_requestHeader;
    private readonly int m_clientVersion;

    internal ApiStackRouter CallRouter => this.m_callRouter.Value;

    public RequestHeader requestHeader
    {
      get => this.m_requestHeader;
      set
      {
        this.m_requestHeader = value;
        this.RequestContext.Items["IsClientOm"] = (object) true;
        this.RequestContext.Items["UseLegacyIdentityString"] = (object) !this.UseDisambiguatedIdentityString;
      }
    }

    protected string RequestId => this.requestHeader == null ? (string) null : this.requestHeader.Id;

    protected bool UseDisambiguatedIdentityString => this.requestHeader != null && this.requestHeader.UseDisambiguatedIdentityString;

    public WorkItemTrackingWebService()
    {
      this.RequestContext.ServiceName = "WorkItem Tracking";
      this.RequestContext.GetService<WorkItemTrackingConfigurationSettingService>();
      this.m_callRouter = new Lazy<ApiStackRouter>((Func<ApiStackRouter>) (() => new ApiStackRouter(this.RequestContext)));
    }

    public WorkItemTrackingWebService(int clientVersion)
      : this()
    {
      this.RequestContext.Items[nameof (ClientVersion)] = (object) clientVersion;
      this.m_clientVersion = clientVersion;
    }

    protected override Exception HandleException(Exception exception)
    {
      if (this.RequestContext != null)
        this.RequestContext.Status = exception;
      if (!(exception is SoapException))
        ExceptionManager.ThrowProperSoapException(this.RequestContext, exception);
      return exception;
    }

    protected int ClientVersion => this.m_clientVersion;

    protected void ExecuteWebMethod(
      MethodInformation methodInformation,
      string traceLayer,
      int startTracepoint,
      int endTracepoint,
      AccessIntent intent,
      Action methodCodeBlock,
      Action<Exception> catchCodeBlock = null)
    {
      ArgumentUtility.CheckForNull<Action>(methodCodeBlock, "originalCall");
      this.RequestContext.TraceEnter(startTracepoint, "WebServices", traceLayer, methodInformation.Name);
      this.EnterMethod(methodInformation);
      try
      {
        Guid requestIdGuid;
        bool requestId = RequestCancelableScope.TryParseRequestId(this.RequestId, out requestIdGuid);
        try
        {
          if (requestId)
          {
            using (new RequestCancelableScope(requestIdGuid, this.RequestContext))
              this.CallRouter.Run(methodCodeBlock);
          }
          else
            this.CallRouter.Run(methodCodeBlock);
        }
        catch (Exception ex)
        {
          if (catchCodeBlock != null)
            catchCodeBlock(ex);
          throw;
        }
      }
      catch (Exception ex)
      {
        throw this.HandleException(ex);
      }
      finally
      {
        this.LeaveMethod();
        this.RequestContext.TraceLeave(endTracepoint, "WebServices", traceLayer, methodInformation.Name);
      }
    }

    protected void ExecuteWebMethod(
      string methodName,
      MethodType methodType,
      EstimatedMethodCost methodCost,
      string traceLayer,
      int startTracepoint,
      int endTracepoint,
      AccessIntent intent,
      Action methodCodeBlock)
    {
      this.ExecuteWebMethod(new MethodInformation(methodName, methodType, methodCost), traceLayer, startTracepoint, endTracepoint, intent, methodCodeBlock);
    }

    internal void CheckAndBlockWitSoapAccess(XmlElement updatePackage = null)
    {
      if (updatePackage == null || !this.HasLegacyProcessUpdateOperation(updatePackage))
        return;
      this.CheckAndBlockLegacyProcessUpdates(updatePackage);
    }

    private void CheckAndBlockLegacyProcessUpdates(XmlElement updatePackage)
    {
      if (!this.HasLegacyProcessUpdateOperation(updatePackage))
        return;
      WorkItemTrackingFeatureFlags.CheckLegacyProcessUpdateInCustomizationModeEnabled(this.RequestContext);
    }

    private bool HasLegacyProcessUpdateOperation(XmlElement updatePackage)
    {
      bool? nullable;
      if (updatePackage == null)
      {
        nullable = new bool?();
      }
      else
      {
        XmlNodeList childNodes = updatePackage.ChildNodes;
        nullable = childNodes != null ? new bool?(childNodes.Cast<XmlNode>().Any<XmlNode>((Func<XmlNode, bool>) (n => n != null && n.Name != null && WorkItemTrackingWebService.LegacyProcessUpdateActions.Contains<string>(n.Name)))) : new bool?();
      }
      return nullable.GetValueOrDefault();
    }
  }
}
