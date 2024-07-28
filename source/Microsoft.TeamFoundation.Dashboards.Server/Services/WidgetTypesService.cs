// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.WidgetTypesService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Model;
using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class WidgetTypesService : IWidgetTypesService, IVssFrameworkService
  {
    public void ValidateWidgets(IVssRequestContext requestContext, IEnumerable<Widget> widgets)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017402, nameof (WidgetTypesService), "WidgetTypesService.ValidateWidgets"))
      {
        IEnumerable<WidgetMetadata> widgetTypesImpl = this.GetWidgetTypesImpl(requestContext);
        this.ValidateWidgetsImpl(requestContext, widgets, widgetTypesImpl);
      }
    }

    public IEnumerable<WidgetMetadata> GetAllWidgetsMetadata(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017400, nameof (WidgetTypesService), "WidgetTypesService.GetWidgetTypes"))
        return this.GetWidgetTypesImpl(requestContext);
    }

    public IEnumerable<WidgetMetadata> GetFilteredWidgetsMetadata(
      IVssRequestContext requestContext,
      Func<WidgetMetadata, bool> filter)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (TelemetryCollector.TraceMonitor(requestContext, 10017410, nameof (WidgetTypesService), "WidgetTypesService.GetFilteredWidgetMetadata"))
        return this.GetWidgetTypesImpl(requestContext).Where<WidgetMetadata>((Func<WidgetMetadata, bool>) (o => filter(o)));
    }

    public void ValidateWidgetsImpl(
      IVssRequestContext requestContext,
      IEnumerable<Widget> widgets,
      IEnumerable<WidgetMetadata> metadataSet)
    {
      Dictionary<string, WidgetMetadata> dictionary = metadataSet.ToDictionary<WidgetMetadata, string, WidgetMetadata>((Func<WidgetMetadata, string>) (o => o.ContributionId), (Func<WidgetMetadata, WidgetMetadata>) (o => o));
      foreach (Widget widget in widgets)
      {
        if (widget.ContributionId == null)
          throw new WidgetTypeDoesNotExistException(Microsoft.TeamFoundation.Dashboards.DashboardResources.ErrorEmptyContributionId());
        WidgetMetadata metadata;
        if (!dictionary.TryGetValue(widget.ContributionId, out metadata))
          throw new WidgetTypeDoesNotExistException(Microsoft.TeamFoundation.Dashboards.DashboardResources.ErrorWidgetTypeDoesNotExistFormat((object) widget.ContributionId));
        widget.ValidateFromMetadata(metadata);
      }
    }

    private IEnumerable<WidgetMetadata> GetWidgetTypesImpl(IVssRequestContext requestContext)
    {
      List<WidgetMetadata> source1 = new List<WidgetMetadata>();
      IWidgetMetadataService service = requestContext.GetService<IWidgetMetadataService>();
      source1.AddRange(service.GetWidgets(requestContext));
      List<WidgetMetadata> list1 = source1.Where<WidgetMetadata>((Func<WidgetMetadata, bool>) (o => o.IsValid() && o.IsEnabled)).ToList<WidgetMetadata>();
      List<WidgetConfigurationMetadata> list2 = service.GetWidgetConfigurations(requestContext).ToList<WidgetConfigurationMetadata>();
      foreach (WidgetMetadata widgetMetadata in list1)
      {
        foreach (string target1 in widgetMetadata.Targets)
        {
          string target = target1;
          IEnumerable<WidgetConfigurationMetadata> source2 = list2.Where<WidgetConfigurationMetadata>((Func<WidgetConfigurationMetadata, bool>) (o => o.ContributionId == target));
          if (source2 != null && source2.Count<WidgetConfigurationMetadata>() == 1)
          {
            widgetMetadata.ConfigurationContributionId = target;
            widgetMetadata.ConfigurationContributionRelativeId = source2.First<WidgetConfigurationMetadata>().TypeId;
          }
        }
      }
      return (IEnumerable<WidgetMetadata>) list1;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
