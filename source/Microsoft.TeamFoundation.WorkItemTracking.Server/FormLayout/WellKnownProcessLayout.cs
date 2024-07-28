// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WellKnownProcessLayout
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.Process;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public static class WellKnownProcessLayout
  {
    private static readonly Guid AgileProcessTypeId = new Guid("ADCC42AB-9882-485E-A3ED-7678F01F66BC");
    private static string AgileProcessBugReferenceName = "Microsoft.VSTS.WorkItemTypes.Bug";

    public static Layout GetAgileBugLayout(IVssRequestContext requestContext)
    {
      Layout agileBugLayout = (Layout) null;
      requestContext.TraceEnter(909811, "FormLayout", "FormTransformsLayer", nameof (GetAgileBugLayout));
      try
      {
        ProcessWorkDefinition processWorkDefinition = requestContext.GetService<ILegacyWorkItemTrackingProcessService>().GetProcessWorkDefinition(requestContext, WellKnownProcessLayout.AgileProcessTypeId);
        ProcessWorkItemTypeDefinition itemTypeDefinition1;
        if (processWorkDefinition == null)
        {
          itemTypeDefinition1 = (ProcessWorkItemTypeDefinition) null;
        }
        else
        {
          IReadOnlyCollection<ProcessWorkItemTypeDefinition> itemTypeDefinitions = processWorkDefinition.WorkItemTypeDefinitions;
          itemTypeDefinition1 = itemTypeDefinitions != null ? itemTypeDefinitions.FirstOrDefault<ProcessWorkItemTypeDefinition>((Func<ProcessWorkItemTypeDefinition, bool>) (w => w.ReferenceName != null && w.ReferenceName.Equals(WellKnownProcessLayout.AgileProcessBugReferenceName, StringComparison.OrdinalIgnoreCase))) : (ProcessWorkItemTypeDefinition) null;
        }
        ProcessWorkItemTypeDefinition itemTypeDefinition2 = itemTypeDefinition1;
        if (itemTypeDefinition2 != null)
          agileBugLayout = itemTypeDefinition2.Form;
        else
          requestContext.Trace(909812, TraceLevel.Verbose, "FormLayout", "FormTransformsLayer", "Agile bug workitem type shouldnt be null");
        return agileBugLayout;
      }
      finally
      {
        requestContext.TraceLeave(909813, "FormLayout", "FormTransformsLayer", nameof (GetAgileBugLayout));
      }
    }

    public static Group GetDeploymentLinksControlGroup(Section linksSection)
    {
      if (linksSection != null && linksSection.Children.Count > 0)
        return linksSection.Children.FirstOrDefault<Group>((Func<Group, bool>) (g => g.Children.Any<Control>((Func<Control, bool>) (c => WellKnownControlNames.DeploymentsControl.Equals(c.ControlType, StringComparison.OrdinalIgnoreCase) && c.Id.Equals(WebLayoutXmlHelper.DeploymentControlId)))));
      throw new ArgumentNullException(nameof (linksSection));
    }

    public static Group GetDevelopmentLinksControlGroup(Section linksSection)
    {
      if (linksSection != null && linksSection.Children.Count > 0)
        return linksSection.Children.FirstOrDefault<Group>((Func<Group, bool>) (g => g.Children.Any<Control>((Func<Control, bool>) (c => WellKnownControlNames.LinksControl.Equals(c.ControlType, StringComparison.OrdinalIgnoreCase) && c.Id.Equals(WebLayoutXmlHelper.DevelopmentControlId)))));
      throw new ArgumentNullException(nameof (linksSection));
    }

    public static Group GetRelatedWorkItemLinksControlGroup(Section linksSection)
    {
      if (linksSection == null || linksSection.Children.Count <= 0)
        throw new ArgumentNullException(nameof (linksSection));
      return linksSection == null ? (Group) null : linksSection.Children.FirstOrDefault<Group>((Func<Group, bool>) (g => g.Children.Any<Control>((Func<Control, bool>) (c => WellKnownControlNames.LinksControl.Equals(c.ControlType, StringComparison.OrdinalIgnoreCase) && c.Id.Equals(WebLayoutXmlHelper.RelatedWorkControlId)))));
    }
  }
}
