// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.LayoutConstants
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common.Provision;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout
{
  public class LayoutConstants
  {
    public const int DefaultMaxControlsPerGroupAllowed = 32;
    public const int DefaultMaxGroupsPerSectionAllowed = 32;
    public const int DefaultMaxPagesAllowed = 16;
    public const string FormLayoutServiceRegistryRootPath = "/Service/FormLayout/Settings";
    public const string MaxControlsAllowed = "/Service/FormLayout/Settings/MaxControlsAllowed";
    public const string MaxGroupsAllowed = "/Service/FormLayout/Settings/MaxGroupsAllowed";
    public const string MaxPagesAllowed = "/Service/FormLayout/Settings/MaxPagesAllowed";

    public static HashSet<string> WideControls => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.HtmlControl,
      WellKnownControlNames.WebpageControl,
      WellKnownControlNames.AssociatedAutomationControl,
      WellKnownControlNames.LinksControl
    };

    public static HashSet<string> SystemControls => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.LinksControl,
      WellKnownControlNames.AttachmentsControl,
      WellKnownControlNames.WorkItemLogControl
    };

    public static HashSet<string> FieldsNotInjectedDuringLegacyFormDeserialization => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.AreaId",
      "System.AreaPath",
      "System.Reason",
      "System.AssignedTo",
      "System.AttachedFileCount",
      "System.ChangedBy",
      "System.ChangedDate",
      "System.ExternalLinkCount",
      "System.History",
      "System.HyperLinkCount",
      "System.Id",
      "System.IterationId",
      "System.IterationPath",
      "System.Links.LinkType",
      "System.NodeName",
      "System.Rev",
      "System.RevisedDate",
      "System.State",
      "System.Tags",
      "System.Title"
    };

    public static HashSet<string> HeaderFieldsExistInBody => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.Reason"
    };

    public static HashSet<string> NonCustomizableFields => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "System.AreaId",
      "System.AttachedFileCount",
      "System.ExternalLinkCount",
      "System.History",
      "System.HyperLinkCount",
      "System.Id",
      "System.IterationId",
      "System.Links.LinkType",
      "System.NodeName",
      "System.Rev",
      "System.RevisedDate",
      "System.Tags"
    };

    public static HashSet<string> NonFieldControls => new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      WellKnownControlNames.LinksControl,
      WellKnownControlNames.LabelControl,
      WellKnownControlNames.AttachmentsControl,
      WellKnownControlNames.AssociatedAutomationControl,
      WellKnownControlNames.DeploymentsControl
    };
  }
}
