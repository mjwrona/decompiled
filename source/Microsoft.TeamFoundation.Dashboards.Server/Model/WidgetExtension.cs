// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Model.WidgetExtension
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Services;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Dashboards.Model
{
  public static class WidgetExtension
  {
    public static void Validate(this Widget widget, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<WidgetSize>(widget.Size, "Size");
      if (string.IsNullOrWhiteSpace(widget.Name))
        throw new EmptyWidgetNameException();
      if (string.IsNullOrWhiteSpace(widget.ContributionId))
        throw new WidgetTypeDoesNotExistException(Microsoft.TeamFoundation.Dashboards.DashboardResources.ErrorEmptyContributionId());
      if (widget.Name.Length > 256)
        throw new WidgetNameLengthExceededException(256, widget.Name.Length);
      if (widget.Settings != null && widget.Settings.Length > 16000)
        throw new WidgetSettingsLengthExceededException(16000, widget.Settings.Length);
      WidgetExtension.ValidateSettingsVersion(widget, requestContext);
      if (widget.Size.RowSpan > 10 || widget.Size.RowSpan < 1)
        throw new WidgetSizeDimensionExceededException("RowSpan", 1, 10, widget.Size.RowSpan);
      if (widget.Size.ColumnSpan > 10 || widget.Size.ColumnSpan < 1)
        throw new WidgetSizeDimensionExceededException("ColumnSpan", 1, 10, widget.Size.ColumnSpan);
      if (widget.IsPositioned())
      {
        if (widget.Position.Row > DashboardSettings.GetMaxRowNumber(requestContext) || widget.Position.Row < 1)
          throw new WidgetPositionDimensionExceededException("Row", 0, DashboardSettings.GetMaxRowNumber(requestContext), widget.Position.Row);
        if (widget.Position.Column > DashboardSettings.GetMaxColumnNumber(requestContext) || widget.Position.Column < 1)
          throw new WidgetPositionDimensionExceededException("Column", 0, DashboardSettings.GetMaxColumnNumber(requestContext), widget.Position.Column);
      }
      int length = string.IsNullOrEmpty(widget.ContributionId) ? 0 : widget.ContributionId.Length;
      if (length > 1000 || length <= 0)
        throw new WidgetTypeLengthExceededException(1000, length);
    }

    public static void ValidateFromMetadata(this Widget widget, WidgetMetadata metadata)
    {
      if (!metadata.AllowedSizes.Any<WidgetSize>((Func<WidgetSize, bool>) (ws => ws.ColumnSpan == widget.Size.ColumnSpan && ws.RowSpan == widget.Size.RowSpan)))
        throw new WidgetSizeNotSupportedException(widget.Size.ColumnSpan, widget.Size.RowSpan);
    }

    private static void ValidateSettingsVersion(Widget widget, IVssRequestContext requestContext)
    {
      if (widget.SettingsVersion == null)
        return;
      int num1 = uint.TryParse(widget.SettingsVersion.Major.ToString(), out uint _) ? 1 : 0;
      bool flag1 = uint.TryParse(widget.SettingsVersion.Minor.ToString(), out uint _);
      bool flag2 = uint.TryParse(widget.SettingsVersion.Patch.ToString(), out uint _);
      int num2 = flag1 ? 1 : 0;
      if ((num1 & num2 & (flag2 ? 1 : 0)) == 0)
        throw new WidgetSettingsVersionInvalidException(widget.SettingsVersion.ToString());
    }

    public static void AddLinks(
      this Widget response,
      IVssRequestContext TfsRequestContext,
      UrlHelper Url,
      Guid groupId,
      Guid dashboardId)
    {
      if (response == null)
        return;
      response.Url = DashboardExtension.GetWidgetUrl(TfsRequestContext, Url, new Guid?(response.Id.Value));
      response.Links = new ReferenceLinks();
      response.Links.AddLink("self", response.Url, (ISecuredObject) response);
      response.Links.AddLink("group", DashboardExtension.GetWidgetUrl(TfsRequestContext, Url, new Guid?()), (ISecuredObject) response);
      response.Links.AddLink("dashboard", DashboardExtension.GetDashboardUrl(TfsRequestContext, Url, new Guid?(dashboardId)), (ISecuredObject) response);
    }
  }
}
