// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.QueryInformationController
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.DataImport;
using Microsoft.VisualStudio.Services.Cloud.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Cloud
{
  [ControllerApiVersion(5.2)]
  [ClientInternalUseOnly(true)]
  [VersionedApiControllerCustomName(Area = "DataImportQuery", ResourceName = "SupportedMilestones")]
  public class QueryInformationController : TfsApiController
  {
    [HttpGet]
    public DataImportInformation GetSupportedImportInformation()
    {
      QueryInformationController.CheckPermission(this.TfsRequestContext, QueryInformationPermissions.Read);
      SupportedMilestoneHelper supportedMilestoneHelper = new SupportedMilestoneHelper(this.TfsRequestContext, (string) null);
      string[] array = supportedMilestoneHelper.SupportedValues.Union<string>(supportedMilestoneHelper.AdditionalValues).ToArray<string>();
      return new DataImportInformation()
      {
        SupportedMilestones = array
      };
    }

    internal static void CheckPermission(
      IVssRequestContext requestContext,
      QueryInformationPermissions permission)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, DataImportQueryInformationSecurityConstants.QueryInformationNamespaceId);
      if (securityNamespace == null)
      {
        requestContext.Trace(15080244, TraceLevel.Warning, "DataImportQueryInformation", nameof (QueryInformationController), "Failing permission check because the security namespace isn't available yet");
        securityNamespace.ThrowAccessDeniedException(requestContext, "QueryInformation", (int) permission);
      }
      securityNamespace.CheckPermission(requestContext, "QueryInformation", (int) permission, false);
    }
  }
}
