// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Legacy.AfnStripsHelper
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server.Legacy
{
  public class AfnStripsHelper : RestApiHelper
  {
    private MergeTcmDataHelper m_mergeDataHelper;
    private TfsTestManagementRequestContext m_tfsTestManagementRequestContext;

    public AfnStripsHelper(TfsTestManagementRequestContext context)
      : base((TestManagementRequestContext) context)
    {
      this.m_tfsTestManagementRequestContext = context;
    }

    public AfnStrip CreateAfnStrip(Guid projectId, AfnStrip afnStrip)
    {
      TfsTestManagementRequestContext managementRequestContext = this.m_tfsTestManagementRequestContext;
      AfnStrip createdAfnStrip;
      return managementRequestContext.LegacyTcmServiceHelper.TryCreateAfnStrip(managementRequestContext.RequestContext, projectId, afnStrip, out createdAfnStrip) ? createdAfnStrip : managementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().CreateAfnStrip((TestManagementRequestContext) managementRequestContext, projectId, afnStrip);
    }

    public List<AfnStrip> GetDefaultAfnStrips(Guid projectId, IList<int> testCaseIds)
    {
      TfsTestManagementRequestContext managementRequestContext = this.m_tfsTestManagementRequestContext;
      List<AfnStrip> afnStrips;
      managementRequestContext.LegacyTcmServiceHelper.TryGetDefaultAfnStrips(managementRequestContext.RequestContext, projectId, testCaseIds, out afnStrips);
      AfnStripsHelper.PopulateStoredInField(afnStrips, "tcm");
      List<AfnStrip> defaultAfnStrips = managementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().GetDefaultAfnStrips((TestManagementRequestContext) managementRequestContext, projectId, testCaseIds);
      AfnStripsHelper.PopulateStoredInField(defaultAfnStrips, "tfs");
      return this.MergeDataHelper.MergeDefaultAfnStrips(afnStrips, defaultAfnStrips);
    }

    public List<AfnStrip> GetDefaultAfnStrips(string projectName, IList<int> testCaseIds) => this.GetDefaultAfnStrips(this.GetProjectReference(projectName).Id, testCaseIds);

    public void UpdateDefaultStrip(
      TestManagementRequestContext context,
      Guid projectId,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings)
    {
      ITcmServiceHelper tcmServiceHelper = context.TcmServiceHelper;
      Dictionary<bool, List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>> dictionary = bindings.GroupBy<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, bool>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding, bool>) (binding => tcmServiceHelper.IsTestRunInTCM(context.RequestContext, binding.TestRunId, false))).ToDictionary<IGrouping<bool, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>, bool, List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>>((Func<IGrouping<bool, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>, bool>) (grouping => grouping.Key), (Func<IGrouping<bool, Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>, List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>>) (grouping => grouping.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>()));
      if (dictionary.ContainsKey(true))
        context.TcmServiceHelper.TryUpdateDefaultStrips(context.RequestContext, projectId, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) dictionary[true]);
      if (!dictionary.ContainsKey(false))
        return;
      context.RequestContext.GetService<ITeamFoundationTestManagementAfnStripService>().UpdateDefaultStrip(context, projectId, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding>) dictionary[false]);
    }

    public void UpdateDefaultStrip(
      TestManagementRequestContext context,
      string projectName,
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.DefaultAfnStripBinding> bindings)
    {
      Guid id = this.GetProjectReference(projectName).Id;
      this.UpdateDefaultStrip(context, id, bindings);
    }

    public virtual IMergeDataHelper MergeDataHelper
    {
      get
      {
        if (this.m_mergeDataHelper == null)
          this.m_mergeDataHelper = new MergeTcmDataHelper();
        return (IMergeDataHelper) this.m_mergeDataHelper;
      }
    }

    public static void PopulateStoredInField(List<AfnStrip> afnStrips, string serviceName)
    {
      foreach (AfnStrip afnStrip in (IEnumerable<AfnStrip>) afnStrips ?? Enumerable.Empty<AfnStrip>())
        afnStrip.StoredIn = serviceName;
    }
  }
}
