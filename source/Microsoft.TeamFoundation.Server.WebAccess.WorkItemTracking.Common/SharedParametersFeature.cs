// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.SharedParametersFeature
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common
{
  internal class SharedParametersFeature : ProjectFeatureBase, INotifyProjectFeatureProvisioned
  {
    private const string c_SharedParameterLinkTypeReferenceName = "Microsoft.VSTS.TestCase.SharedParameterReferencedBy";
    private const string c_SharedParameterLinkTypeDefinition = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<LinkTypes>\r\n  <LinkType ReferenceName=\"Microsoft.VSTS.TestCase.SharedParameterReferencedBy\" ForwardName=\"{0}\" ReverseName=\"{1}\" Topology=\"Dependency\" />\r\n</LinkTypes>";

    public SharedParametersFeature()
      : base(Resources.SharedParametersFeatureName)
    {
    }

    public override ProjectFeatureState GetState(IProjectMetadata projectMetadata) => projectMetadata.GetWorkItemTypeCategory(WitCategoryRefName.SharedDataSet) != null ? ProjectFeatureState.FullyConfigured : ProjectFeatureState.NotConfigured;

    public override void Process(IProjectProvisioningContext context) => this.EnsureCategory(context, WitCategoryRefName.SharedDataSet, ProjectFeatureBase.EnsureCategoryMode.DoNotOverwrite, ProjectFeatureBase.EnsureWorkItemMode.AddIfMissingDoNotOverwriteButWarnIfExists);

    void INotifyProjectFeatureProvisioned.OnProvisioned(
      IVssRequestContext requestContext,
      string projectUri)
    {
      requestContext.TraceEnter(1004050, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "SharedParametersFeature.OnProvisioned");
      try
      {
        if (!this.IsSharedParameterLinkTypeImported(requestContext))
        {
          IProvisioningService service = requestContext.GetService<IProvisioningService>();
          string linkTypeDefinition = this.GetSharedParameterLinkTypeDefinition();
          IVssRequestContext requestContext1 = requestContext;
          string definition = linkTypeDefinition;
          service.ImportWorkItemLinkType(requestContext1, definition, true);
        }
        else
          requestContext.Trace(1004053, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "SharedParameterLinkType already imported for project uri:" + projectUri);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(1004052, TraceLevel.Info, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, ex);
      }
      requestContext.TraceLeave(1004051, "ProjectFeatureProvisioning", TfsTraceLayers.BusinessLogic, "SharedParametersFeature.OnProvisioned");
    }

    private bool IsSharedParameterLinkTypeImported(IVssRequestContext requestContext)
    {
      foreach (WorkItemLinkType linkType in (ReadOnlyCollection<WorkItemLinkType>) requestContext.GetService<WebAccessWorkItemService>().GetLinkTypes(requestContext))
      {
        if (TFStringComparer.WorkItemLinkTypeReferenceName.Equals("Microsoft.VSTS.TestCase.SharedParameterReferencedBy", linkType.ReferenceName))
          return true;
      }
      return false;
    }

    private string GetSharedParameterLinkTypeDefinition() => string.Format((IFormatProvider) CultureInfo.CurrentCulture, "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<LinkTypes>\r\n  <LinkType ReferenceName=\"Microsoft.VSTS.TestCase.SharedParameterReferencedBy\" ForwardName=\"{0}\" ReverseName=\"{1}\" Topology=\"Dependency\" />\r\n</LinkTypes>", (object) Resources.SharedParametersLinkTypeForwardName, (object) Resources.SharedParametersLinkTypeReverseName);
  }
}
