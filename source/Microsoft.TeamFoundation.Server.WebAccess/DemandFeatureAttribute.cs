// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DemandFeatureAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DemandFeatureAttribute : ActionFilterAttribute
  {
    private Guid[] m_featureIds;
    protected bool m_doNotAllowAdvertisingMode;

    public DemandFeatureAttribute(string feature, bool doNotAllowAdvertisingMode = false)
      : this(new Guid(feature), doNotAllowAdvertisingMode)
    {
    }

    public DemandFeatureAttribute(string[] features, bool doNotAllowAdvertisingMode = false)
      : this(((IEnumerable<string>) features).Select<string, Guid>((Func<string, Guid>) (id => new Guid(id))).ToArray<Guid>(), doNotAllowAdvertisingMode)
    {
    }

    public DemandFeatureAttribute(Guid feature, bool doNotAllowAdvertisingMode = false)
      : this(new Guid[1]{ feature }, doNotAllowAdvertisingMode)
    {
    }

    public DemandFeatureAttribute(Guid[] features, bool doNotAllowAdvertisingMode = false)
    {
      this.m_featureIds = features;
      this.m_doNotAllowAdvertisingMode = doNotAllowAdvertisingMode;
      this.Order = 100;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      using (WebPerformanceTimer.StartMeasure(filterContext.RequestContext, "Controller.Attributes.DemandFeature"))
      {
        base.OnActionExecuting(filterContext);
        IVssRequestContext tfsRequestContext = filterContext.HttpContext.TfsRequestContext();
        bool flag = filterContext.Controller is TfsController controller && controller.TfsWebContext.IsHosted && !this.m_doNotAllowAdvertisingMode;
        IList<string> stringList = (IList<string>) null;
        FeatureContext featureContext = tfsRequestContext.FeatureContext();
        foreach (Guid featureId in this.m_featureIds)
        {
          if (featureId == new Guid("CEDD6BE8-B717-4a0a-8BFD-C4E9B4CAA071") || featureId == new Guid("EC7545A3-E5DB-40E8-B0D0-F64DF7619BBA") || featureContext.IsFeatureAvailable(featureId) || flag && featureContext.IsFeatureInAdvertisingMode(featureId))
            return;
          if (stringList == null)
            stringList = (IList<string>) new List<string>();
          string name;
          try
          {
            name = featureContext.GetLicenseFeature(featureId).Name;
          }
          catch (ArgumentException ex)
          {
            featureId.ToString();
            return;
          }
          stringList.Add(name);
        }
        Microsoft.TeamFoundation.Server.Core.DemandFeatureAttribute.RecordLicenseExceptionTelemetry(tfsRequestContext, "InvalidLicenseException", stringList, controller.TraceArea);
        throw new HttpInvalidLicenseException(string.Join(", ", (IEnumerable<string>) stringList));
      }
    }
  }
}
