// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions.LayoutExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions
{
  public static class LayoutExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control control)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control serverModel = new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control();
      serverModel.Id = control.Id;
      serverModel.ControlType = control.ControlType;
      serverModel.Label = control.Label;
      serverModel.Metadata = control.Metadata;
      serverModel.Rank = new int?();
      serverModel.ReadOnly = control.ReadOnly;
      serverModel.Visible = control.Visible;
      serverModel.Watermark = control.Watermark;
      Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution contribution = control.Contribution;
      serverModel.Contribution = contribution != null ? contribution.GetServerModel() : (Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution) null;
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution controlContribution)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution(controlContribution.ContributionId, controlContribution.ShowOnDeletedWorkItem, controlContribution.Inputs, controlContribution.Height);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group1 = new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group();
      group1.Id = group.Id;
      group1.Label = group.Label;
      group1.Rank = new int?();
      group1.Visible = group.Visible;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group serverModel = group1;
      if (group.Controls != null && group.Controls.Any<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>())
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control in group.Controls.Where<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control, bool>) (control => control != null)).Select<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control, Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control, Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>) (control => control.GetServerModel())))
          serverModel.Children.Add(control);
      }
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page(true);
      serverModel.Id = group.Id;
      serverModel.Label = group.Label;
      serverModel.Rank = new int?();
      serverModel.Visible = group.Visible;
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout GetWebApiModel(
      this Layout layout)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout()
      {
        Pages = (IList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>) layout.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>) (l => l.GetWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page>(),
        SystemControls = layout.SystemControls.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>) (c => c.GetWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>()
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page page)
    {
      Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page webApiModel = new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Page();
      webApiModel.Id = page.Id;
      webApiModel.Label = page.Label;
      webApiModel.Locked = page.Locked;
      webApiModel.Overridden = page.Overridden;
      webApiModel.PageType = LayoutExtensions.GetWebApiModel(page.PageType);
      webApiModel.Visible = page.Visible;
      webApiModel.Sections = (IList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section>) page.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section>) (s => s.GetWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section>();
      webApiModel.FromInheritedLayout = page.FromInheritedLayout;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = page.Contribution;
      webApiModel.Contribution = contribution != null ? contribution.GetWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution) null;
      return webApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section section)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Section()
      {
        Groups = (IList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>) section.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>) (g => g.GetWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group>(),
        Id = section.Id,
        Overridden = section.Overridden
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group webApiModel = new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Group();
      webApiModel.Controls = (IList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>) group.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>) (c => c.GetWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control>();
      webApiModel.FromInheritedLayout = group.FromInheritedLayout;
      webApiModel.Id = group.Id;
      webApiModel.Overridden = group.Overridden;
      webApiModel.Label = group.Label;
      webApiModel.Visible = group.Visible;
      webApiModel.Height = group.Height;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = group.Contribution;
      webApiModel.Contribution = contribution != null ? contribution.GetWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution) null;
      return webApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control)
    {
      Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control webApiModel = new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Control();
      webApiModel.Id = control.Id;
      webApiModel.ControlType = control.ControlType;
      webApiModel.Label = control.Label;
      webApiModel.Metadata = control.Metadata;
      webApiModel.ReadOnly = control.ReadOnly;
      webApiModel.Visible = control.Visible;
      webApiModel.Watermark = control.Watermark;
      webApiModel.FromInheritedLayout = control.FromInheritedLayout;
      webApiModel.Overridden = control.Overridden;
      webApiModel.Height = control.Height;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = control.Contribution;
      webApiModel.Contribution = contribution != null ? contribution.GetWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution) null;
      return webApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution controlContribution)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.WitContribution()
      {
        ContributionId = controlContribution.ContributionId,
        ShowOnDeletedWorkItem = controlContribution.ShowOnDeletedWorkItem,
        Inputs = controlContribution.Inputs,
        Height = controlContribution.Height
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Extension GetWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Extension extension)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.Extension() { Id = extension.Id };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.PageType GetWebApiModel(
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType type)
    {
      switch (type)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Custom:
          return Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.PageType.Custom;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.History:
          return Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.PageType.History;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Links:
          return Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.PageType.Links;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Attachments:
          return Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.PageType.Attachments;
        default:
          throw new NotImplementedException();
      }
    }
  }
}
