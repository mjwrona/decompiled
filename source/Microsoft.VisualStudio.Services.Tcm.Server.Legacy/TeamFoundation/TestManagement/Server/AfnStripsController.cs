// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AfnStripsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Tcm;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "AfnStrips", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class AfnStripsController : TcmControllerBase
  {
    [HttpGet]
    [ClientLocationId("E95B7842-152C-4A62-AD35-6C428CDDDE07")]
    public List<AfnStrip> GetAfnStrips([ClientParameterAsIEnumerable(typeof (int), ',')] string testCaseIds = "")
    {
      List<AfnStrip> defaultAfnStrips = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().GetDefaultAfnStrips(this.TestManagementRequestContext, this.ProjectId, this.GetIds(testCaseIds, nameof (testCaseIds)));
      AfnStripsController.PopulateStoredInField(defaultAfnStrips, "tcm");
      return defaultAfnStrips;
    }

    [HttpPost]
    [ClientLocationId("E95B7842-152C-4A62-AD35-6C428CDDDE07")]
    public AfnStrip CreateAfnStrip(AfnStrip afnStrip)
    {
      NewProjectStepsPerformer.InitializeNewProject(this.TestManagementRequestContext, this.ProjectId);
      return this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().CreateAfnStrip(this.TestManagementRequestContext, this.ProjectId, afnStrip);
    }

    [HttpPatch]
    [ClientLocationId("E95B7842-152C-4A62-AD35-6C428CDDDE07")]
    public void UpdateDefaultAfnStrips(IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings) => this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().UpdateDefaultStrip(this.TestManagementRequestContext, this.ProjectId, bindings);

    private IList<int> GetIds(string testCaseIds, string parameterName)
    {
      try
      {
        return (IList<int>) ((IEnumerable<string>) testCaseIds.Split(new string[1]
        {
          ","
        }, StringSplitOptions.RemoveEmptyEntries)).Select<string, int>((Func<string, int>) (token => int.Parse(token))).ToList<int>();
      }
      catch (Exception ex)
      {
        switch (ex)
        {
          case ArgumentNullException _:
          case OverflowException _:
          case FormatException _:
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, string.Format(ServerResources.InValidListOfIds, (object) parameterName)));
          default:
            throw;
        }
      }
    }

    private static void PopulateStoredInField(List<AfnStrip> afnStrips, string serviceName)
    {
      foreach (AfnStrip afnStrip in (IEnumerable<AfnStrip>) afnStrips ?? Enumerable.Empty<AfnStrip>())
        afnStrip.StoredIn = serviceName;
    }
  }
}
