// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.PlatformMustacheExtensions
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.Web;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  public static class PlatformMustacheExtensions
  {
    private const string s_area = "PlatformMustacheExtensions";
    private const string s_layer = "Contributions";
    private const string c_TeamFoundationRequestContext = "IVssRequestContext";

    static PlatformMustacheExtensions()
    {
      PlatformMustacheExtensions.Parser = new MustacheTemplateParser();
      PlatformMustacheExtensions.Parser.RegisterPlatformHelpers();
    }

    public static MustacheTemplateParser Parser { get; private set; }

    public static void RegisterPlatformHelpers(this MustacheTemplateParser templateParser)
    {
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("ServiceUrl", PlatformMustacheExtensions.\u003C\u003EO.\u003C0\u003E__ServiceUrlHelper ?? (PlatformMustacheExtensions.\u003C\u003EO.\u003C0\u003E__ServiceUrlHelper = new MustacheTemplateHelperMethod(PlatformMustacheExtensions.ServiceUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("DeploymentUrl", PlatformMustacheExtensions.\u003C\u003EO.\u003C1\u003E__DeploymentUrlHelper ?? (PlatformMustacheExtensions.\u003C\u003EO.\u003C1\u003E__DeploymentUrlHelper = new MustacheTemplateHelperMethod(PlatformMustacheExtensions.DeploymentUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("ContentRootUrl", PlatformMustacheExtensions.\u003C\u003EO.\u003C2\u003E__ContentRootUrlHelper ?? (PlatformMustacheExtensions.\u003C\u003EO.\u003C2\u003E__ContentRootUrlHelper = new MustacheTemplateHelperMethod(PlatformMustacheExtensions.ContentRootUrlHelper)));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      templateParser.RegisterHelper("ResourceAreaUrl", PlatformMustacheExtensions.\u003C\u003EO.\u003C3\u003E__ResourceAreaUrlHelper ?? (PlatformMustacheExtensions.\u003C\u003EO.\u003C3\u003E__ResourceAreaUrlHelper = new MustacheTemplateHelperMethod(PlatformMustacheExtensions.ResourceAreaUrlHelper)));
    }

    private static string ServiceUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      IVssRequestContext vssRequestContext;
      if (context.AdditionalEvaluationData == null || !context.AdditionalEvaluationData.TryGetValue<IVssRequestContext>("IVssRequestContext", out vssRequestContext))
        vssRequestContext = HttpContext.Current != null && HttpContext.Current.Items != null ? HttpContext.Current.Items[(object) "IVssRequestContext"] as IVssRequestContext : throw new InvalidMustacheTemplateContextException(ExtMgmtResources.MustacheTemplateInvalidContext());
      string input = expression.Expression;
      if (vssRequestContext == null)
        return string.Empty;
      TeamFoundationHostType result1 = TeamFoundationHostType.Unknown;
      Guid result2 = Guid.Empty;
      if (!string.IsNullOrEmpty(input))
      {
        vssRequestContext.Trace(10013565, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Expression: {0}", (object) input);
        string[] strArray = input.Trim().Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 2)
        {
          input = strArray[0].Trim('\'').Trim('"');
          vssRequestContext.Trace(10013570, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Service owner: {0}", (object) input);
          string str = strArray[1].Trim('\'').Trim('"');
          vssRequestContext.Trace(10013575, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "HostType string: {0}", (object) str);
          if (!Enum.TryParse<TeamFoundationHostType>(str, true, out result1))
          {
            vssRequestContext.Trace(10013580, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Invalid host type Expression: {0}", (object) str);
            return string.Empty;
          }
          vssRequestContext.Trace(10013585, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "HostType: {0}", (object) result1);
        }
        else if (strArray.Length == 1)
        {
          string str = strArray[0].Trim('\'').Trim('"');
          vssRequestContext.Trace(10013590, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Only one argument: {0}", (object) str);
          if (Enum.TryParse<TeamFoundationHostType>(str, true, out result1))
            input = PlatformMustacheExtensions.GetServiceInstanceTypeFromMustacheContext(context);
        }
      }
      else
      {
        vssRequestContext.Trace(10013595, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "No arguments specified.  Defaulting service owner to current service.");
        input = PlatformMustacheExtensions.GetServiceInstanceTypeFromMustacheContext(context);
      }
      Guid.TryParse(input, out result2);
      if (vssRequestContext.ExecutionEnvironment.IsHostedDeployment && result1 == TeamFoundationHostType.Application)
      {
        vssRequestContext.Trace(10013600, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Changing the requested host type from Application to ProjectCollection.");
        result1 = TeamFoundationHostType.ProjectCollection;
      }
      vssRequestContext.Trace(10013605, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Current request context host type: {0}", (object) vssRequestContext.ServiceHost.HostType);
      if (result1 != TeamFoundationHostType.Unknown && !vssRequestContext.ServiceHost.Is(result1))
      {
        if (result1 > vssRequestContext.ServiceHost.HostType)
        {
          vssRequestContext.Trace(10013610, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Unable to convert to requested host type");
          return string.Empty;
        }
        vssRequestContext.Trace(10013615, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Changing from host type {0} to {1}.", (object) vssRequestContext.ServiceHost.HostType, (object) result1);
        vssRequestContext = vssRequestContext.To(result1);
      }
      string locationServiceUrl = vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, result2, AccessMappingConstants.ClientAccessMappingMoniker);
      vssRequestContext.Trace(10013620, TraceLevel.Info, nameof (PlatformMustacheExtensions), "Contributions", "Evaulated template: {0}", (object) locationServiceUrl);
      return locationServiceUrl;
    }

    private static string ContentRootUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      IVssRequestContext vssRequestContext;
      if (context.AdditionalEvaluationData == null || !context.AdditionalEvaluationData.TryGetValue<IVssRequestContext>("IVssRequestContext", out vssRequestContext))
        vssRequestContext = HttpContext.Current != null && HttpContext.Current.Items != null ? HttpContext.Current.Items[(object) "IVssRequestContext"] as IVssRequestContext : throw new InvalidMustacheTemplateContextException(ExtMgmtResources.MustacheTemplateInvalidContext());
      string input = expression.Expression;
      if (vssRequestContext == null)
        return string.Empty;
      TeamFoundationHostType hostType = vssRequestContext.ExecutionEnvironment.IsOnPremisesDeployment ? TeamFoundationHostType.Deployment : TeamFoundationHostType.ProjectCollection;
      Guid result = Guid.Empty;
      if (!string.IsNullOrEmpty(input))
      {
        string[] strArray = input.Trim().Split(new char[1]
        {
          ' '
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length == 1)
          input = strArray[0].Trim('\'').Trim('"');
      }
      else
        input = PlatformMustacheExtensions.GetServiceInstanceTypeFromMustacheContext(context);
      Guid.TryParse(input, out result);
      if (hostType != TeamFoundationHostType.Unknown && !vssRequestContext.ServiceHost.Is(hostType))
      {
        if (hostType > vssRequestContext.ServiceHost.HostType)
          return string.Empty;
        vssRequestContext = vssRequestContext.To(hostType);
      }
      return vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, result, AccessMappingConstants.ClientAccessMappingMoniker);
    }

    private static string DeploymentUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string str = expression.Expression;
      IVssRequestContext vssRequestContext1;
      if (context.AdditionalEvaluationData == null || !context.AdditionalEvaluationData.TryGetValue<IVssRequestContext>("IVssRequestContext", out vssRequestContext1))
        vssRequestContext1 = HttpContext.Current.Items[(object) "IVssRequestContext"] as IVssRequestContext;
      if (vssRequestContext1 != null)
      {
        Guid result = Guid.Empty;
        if (string.IsNullOrEmpty(str))
          str = PlatformMustacheExtensions.GetServiceInstanceTypeFromMustacheContext(context);
        if (!string.IsNullOrEmpty(str))
        {
          if (Guid.TryParse(str.Trim('\'').Trim('"'), out result))
          {
            Guid instanceId = vssRequestContext1.ServiceHost.InstanceId;
            IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
            HostInstanceMapping hostInstanceMapping = vssRequestContext2.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext2, instanceId, result, true);
            if (hostInstanceMapping == null || hostInstanceMapping.ServiceInstance == null || hostInstanceMapping.ServiceInstance.PublicUri == (Uri) null)
              throw new ExtensionTemplateException(ExtMgmtResources.DeploymentInstanceNotFoundException((object) instanceId, (object) result));
            return hostInstanceMapping.ServiceInstance.PublicUri.ToString();
          }
        }
      }
      return string.Empty;
    }

    private static string ResourceAreaUrlHelper(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      string expression1 = expression.Expression;
      IVssRequestContext vssRequestContext1;
      if (context.AdditionalEvaluationData == null || !context.AdditionalEvaluationData.TryGetValue<IVssRequestContext>("IVssRequestContext", out vssRequestContext1))
        vssRequestContext1 = HttpContext.Current.Items[(object) "IVssRequestContext"] as IVssRequestContext;
      if (vssRequestContext1 != null)
      {
        if (string.IsNullOrEmpty(expression1))
          throw new ExtensionTemplateException(ExtMgmtResources.ResourceAreaNotSet());
        Guid result = Guid.Empty;
        if (Guid.TryParse(expression1.Trim('\'').Trim('"'), out result))
        {
          Guid instanceId = vssRequestContext1.ServiceHost.InstanceId;
          ResourceArea resourceArea = vssRequestContext1.GetService<IResourceAreaService>().GetResourceArea(vssRequestContext1, result, true);
          if (resourceArea == null)
            throw new ExtensionTemplateException(ExtMgmtResources.ResourceAreaNotFoundException((object) instanceId, (object) result));
          IVssRequestContext vssRequestContext2 = vssRequestContext1.To(TeamFoundationHostType.Deployment);
          HostInstanceMapping hostInstanceMapping = vssRequestContext2.GetService<IInstanceManagementService>().GetHostInstanceMapping(vssRequestContext2, instanceId, resourceArea.ParentService, true);
          if (hostInstanceMapping == null || hostInstanceMapping.ServiceInstance == null || hostInstanceMapping.ServiceInstance.PublicUri == (Uri) null)
            throw new ExtensionTemplateException(ExtMgmtResources.DeploymentInstanceNotFoundException((object) instanceId, (object) resourceArea.ParentService));
          return hostInstanceMapping.ServiceInstance.PublicUri.ToString();
        }
      }
      return string.Empty;
    }

    private static string GetServiceInstanceTypeFromMustacheContext(
      MustacheEvaluationContext context)
    {
      if (context.ReplacementObject == null)
        return (string) null;
      if (!(context.ReplacementObject is JToken jtoken))
        jtoken = JToken.FromObject(context.ReplacementObject);
      return jtoken[(object) "$ServiceInstanceType"]?.ToString();
    }
  }
}
