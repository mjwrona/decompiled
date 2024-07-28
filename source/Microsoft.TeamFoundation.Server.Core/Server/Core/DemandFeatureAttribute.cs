// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.DemandFeatureAttribute
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Server.Core
{
  public class DemandFeatureAttribute : ActionFilterAttribute
  {
    private bool checkForLicenseInHostedDeployment;

    public DemandFeatureAttribute(params string[] featureIds)
      : this(((IEnumerable<string>) featureIds).Select<string, Guid>((Func<string, Guid>) (id => new Guid(id))).ToArray<Guid>())
    {
    }

    public DemandFeatureAttribute(string featureId, bool checkForLicenseInHosted = false)
      : this(new Guid(featureId))
    {
      this.checkForLicenseInHostedDeployment = checkForLicenseInHosted;
    }

    public DemandFeatureAttribute(params Guid[] features) => this.FeatureIds = features;

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      base.OnActionExecuting(actionContext);
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (tfsRequestContext.ExecutionEnvironment.IsHostedDeployment && !this.checkForLicenseInHostedDeployment)
        return;
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = tfsRequestContext.GetUserIdentity();
      if (userIdentity == null || !IdentityHelper.IsServiceIdentity(tfsRequestContext, (IReadOnlyVssIdentity) userIdentity))
      {
        ITeamFoundationLicensingService service = tfsRequestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>();
        IList<string> stringList = (IList<string>) null;
        foreach (Guid featureId in this.FeatureIds)
        {
          if (service.IsFeatureSupported(tfsRequestContext, featureId))
            return;
          if (stringList == null)
            stringList = (IList<string>) new List<string>();
          string name;
          try
          {
            name = service.GetLicenseFeature(tfsRequestContext, featureId).Name;
          }
          catch (ArgumentException ex)
          {
            featureId.ToString();
            return;
          }
          stringList.Add(name);
        }
        DemandFeatureAttribute.RecordLicenseExceptionTelemetry(tfsRequestContext, "MissingLicenseException", stringList, controller.TraceArea);
        throw new MissingLicenseException(string.Join(", ", (IEnumerable<string>) stringList));
      }
    }

    public static void RecordLicenseExceptionTelemetry(
      IVssRequestContext tfsRequestContext,
      string licenseExceptionMessage,
      IList<string> missingFeatures,
      string traceArea = null)
    {
      try
      {
        ILicenseType[] licensesForUser = tfsRequestContext.To(TeamFoundationHostType.Application).GetService<ITeamFoundationLicensingService>().GetLicensesForUser(tfsRequestContext, tfsRequestContext.UserContext);
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add("Licenses", (object) ((IEnumerable<ILicenseType>) licensesForUser).Select<ILicenseType, string>((Func<ILicenseType, string>) (license => license.Name)));
        properties.Add("MissingFeatures", (object) missingFeatures);
        properties.Add("ServiceName", tfsRequestContext.ServiceName);
        properties.Add("RequestPath", tfsRequestContext.RequestPath());
        properties.Add("Title", tfsRequestContext.Title());
        properties.Add("TraceArea", traceArea);
        tfsRequestContext.GetService<CustomerIntelligenceService>().Publish(tfsRequestContext, CustomerIntelligenceArea.Licensing, licenseExceptionMessage, properties);
      }
      catch (Exception ex)
      {
        TeamFoundationTrace.TraceException("Fail to record CustomerIntelligence", nameof (RecordLicenseExceptionTelemetry), ex);
      }
    }

    private Guid[] FeatureIds { get; set; }
  }
}
