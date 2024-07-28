// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions.ProcessLayoutExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions
{
  public static class ProcessLayoutExtensions
  {
    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control control)
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
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution contribution = control.Contribution;
      serverModel.Contribution = contribution != null ? contribution.GetServerModel() : (Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution) null;
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution controlContribution)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution(controlContribution.ContributionId, controlContribution.ShowOnDeletedWorkItem, controlContribution.Inputs, controlContribution.Height);
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page serverModel = new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page(true);
      serverModel.Id = group.Id;
      serverModel.Label = group.Label;
      serverModel.Rank = new int?();
      serverModel.Visible = group.Visible;
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group GetServerModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group1 = new Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group();
      group1.Id = group.Id;
      group1.Label = group.Label;
      group1.Rank = new int?();
      group1.Visible = group.Visible;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group serverModel = group1;
      if (group.Controls != null && group.Controls.Any<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>())
      {
        foreach (Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control in group.Controls.Where<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control, bool>) (control => control != null)).Select<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control, Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control, Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control>) (control => control.GetServerModel())))
          serverModel.Children.Add(control);
      }
      return serverModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution controlContribution)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution()
      {
        ContributionId = controlContribution.ContributionId,
        ShowOnDeletedWorkItem = controlContribution.ShowOnDeletedWorkItem,
        Inputs = controlContribution.Inputs,
        Height = controlContribution.Height
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group group)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group processWebApiModel = new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group();
      processWebApiModel.Controls = (IList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) group.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) (c => c.GetProcessWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>();
      processWebApiModel.FromInheritedLayout = group.FromInheritedLayout;
      processWebApiModel.Id = group.Id;
      processWebApiModel.Overridden = group.Overridden;
      processWebApiModel.Label = group.Label;
      processWebApiModel.Visible = group.Visible;
      processWebApiModel.Height = group.Height;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = group.Contribution;
      processWebApiModel.Contribution = contribution != null ? contribution.GetProcessWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution) null;
      return processWebApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control control)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control processWebApiModel = new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control();
      processWebApiModel.Id = control.Id;
      processWebApiModel.ControlType = control.ControlType;
      processWebApiModel.Label = control.Label;
      processWebApiModel.Metadata = control.Metadata;
      processWebApiModel.ReadOnly = control.ReadOnly;
      processWebApiModel.Visible = control.Visible;
      processWebApiModel.Watermark = control.Watermark;
      processWebApiModel.FromInheritedLayout = control.FromInheritedLayout;
      processWebApiModel.Overridden = control.Overridden;
      processWebApiModel.Height = control.Height;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = control.Contribution;
      processWebApiModel.Contribution = contribution != null ? contribution.GetProcessWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution) null;
      return processWebApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section section)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section()
      {
        Groups = (IList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>) section.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Group, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>) (g => g.GetProcessWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Group>(),
        Id = section.Id,
        Overridden = section.Overridden
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.PageType GetProcessWebApiModel(
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType type)
    {
      switch (type)
      {
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Custom:
          return Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.PageType.Custom;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.History:
          return Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.PageType.History;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Links:
          return Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.PageType.Links;
        case Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.PageType.Attachments:
          return Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.PageType.Attachments;
        default:
          throw new NotImplementedException();
      }
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page page)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page processWebApiModel = new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page();
      processWebApiModel.Id = page.Id;
      processWebApiModel.Label = page.Label;
      processWebApiModel.Locked = page.Locked;
      processWebApiModel.Overridden = page.Overridden;
      processWebApiModel.PageType = ProcessLayoutExtensions.GetProcessWebApiModel(page.PageType);
      processWebApiModel.Visible = page.Visible;
      processWebApiModel.Sections = (IList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section>) page.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Section, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section>) (s => s.GetProcessWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Section>();
      processWebApiModel.FromInheritedLayout = page.FromInheritedLayout;
      Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.WitContribution contribution = page.Contribution;
      processWebApiModel.Contribution = contribution != null ? contribution.GetProcessWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.WitContribution) null;
      return processWebApiModel;
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.FormLayout GetProcessWebApiModel(
      this Layout layout)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.FormLayout()
      {
        Pages = (IList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>) layout.Children.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Page, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>) (l => l.GetProcessWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Page>(),
        SystemControls = layout.SystemControls.Select<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>((Func<Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Control, Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>) (c => c.GetProcessWebApiModel())).ToList<Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Control>()
      };
    }

    public static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Extension GetProcessWebApiModel(
      this Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout.Extension extension)
    {
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.Extension() { Id = extension.Id };
    }
  }
}
