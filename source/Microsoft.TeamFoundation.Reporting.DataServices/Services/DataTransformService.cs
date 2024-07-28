// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.Services.DataTransformService
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Charting.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Reporting.DataServices.Services
{
  public class DataTransformService : IDataTransformService, IVssFrameworkService
  {
    public IDataServicesWriter GetWriter(IVssRequestContext requestContext)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      }
      catch (ArgumentException ex)
      {
        throw new InvalidTransformOptionsException((Exception) ex);
      }
      requestContext.TraceEnter(1017550, "Reporting", nameof (DataTransformService), nameof (GetWriter));
      try
      {
        return (IDataServicesWriter) new JsonDataTableWriter()
        {
          RequestContext = requestContext
        };
      }
      finally
      {
        requestContext.TraceLeave(1017560, "Reporting", nameof (DataTransformService), nameof (GetWriter));
      }
    }

    public IEnumerable<TransformResult> GetResults(
      IVssRequestContext requestContext,
      string scope,
      Guid? projectId,
      IEnumerable<TransformOptions> options)
    {
      this.ValidateInput(requestContext, options);
      requestContext.TraceEnter(1017500, "Reporting", nameof (DataTransformService), nameof (GetResults));
      try
      {
        options = this.EnforceCleanTransforms(requestContext, scope, options);
        TransformSecurityInformation transformSecurityInformation = this.PrepareTransformSecurityInformation(requestContext, scope, projectId);
        IProvideChartingData provideChartingData = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope);
        if (!provideChartingData.IsEnabled(requestContext))
          throw new ChartProviderNotEnabledException(provideChartingData.GetCapabilityProvider(requestContext).GetArtifactPluralName(requestContext));
        foreach (TransformOptions option in options)
          DataTransformService.SetProjectNameFromProjectGuidInFilterContext(requestContext, option.FilterContext, transformSecurityInformation.ProjectId);
        List<TransformResult> results = new List<TransformResult>(provideChartingData.GetTransformProvider(requestContext).GetResults(requestContext, transformSecurityInformation, options));
        results.ForEach((Action<TransformResult>) (o => o.SetSecuredObject(transformSecurityInformation.SharedSecuredObject)));
        return (IEnumerable<TransformResult>) results;
      }
      catch (Exception ex)
      {
        TelemetryHelper.PublishChartingRequestFailure(requestContext, scope, nameof (GetResults), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(1017540, "Reporting", nameof (DataTransformService), nameof (GetResults));
      }
    }

    public IInterpretQueryText GetQueryTextInterpreter(
      IVssRequestContext requestContext,
      string scope)
    {
      IProvideChartingData provideChartingData = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope);
      return provideChartingData.IsEnabled(requestContext) ? provideChartingData.GetTransformProvider(requestContext).GetQueryTextInterpreter() : throw new ChartProviderNotEnabledException(provideChartingData.GetCapabilityProvider(requestContext).GetArtifactPluralName(requestContext));
    }

    private TransformSecurityInformation PrepareTransformSecurityInformation(
      IVssRequestContext requestContext,
      string scope,
      Guid? projectId)
    {
      ISecuredObject sharedSecuredObject = (ISecuredObject) null;
      if (requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope).GetSecurityProvider(requestContext) is IDataServicesSecurityProvider2 securityProvider && projectId.HasValue)
        sharedSecuredObject = securityProvider.GetTransformSecuredObject(projectId.Value);
      return new TransformSecurityInformation(projectId, sharedSecuredObject);
    }

    private IEnumerable<TransformOptions> EnforceCleanTransforms(
      IVssRequestContext requestContext,
      string scope,
      IEnumerable<TransformOptions> options)
    {
      requestContext.TraceEnter(1017912, "Reporting", nameof (DataTransformService), "EnforceTransformOptions");
      try
      {
        List<TransformOptions> transformOptionsList = new List<TransformOptions>();
        IDataServicesSecurityProvider securityProvider = requestContext.GetService<IFeatureProviderRegistryService>().SelectProvider(requestContext, scope).GetSecurityProvider(requestContext);
        if (options.Count<TransformOptions>() == 1)
        {
          foreach (TransformOptions option in options)
          {
            DataTransformService.ValidateTransformOptions(requestContext, option);
            securityProvider.EnsureTransformPermissions(option);
            transformOptionsList.Add(option);
          }
        }
        else
        {
          if (options.Count<TransformOptions>() > 50)
            throw new TooManyOptionsPerTransformException();
          foreach (TransformOptions option in options)
          {
            try
            {
              DataTransformService.ValidateTransformOptions(requestContext, option);
              securityProvider.EnsureTransformPermissions(option);
              transformOptionsList.Add(option);
            }
            catch (Exception ex)
            {
              requestContext.TraceException(1017914, TraceLevel.Verbose, "Reporting", nameof (DataTransformService), ex);
            }
          }
        }
        return (IEnumerable<TransformOptions>) transformOptionsList;
      }
      finally
      {
        requestContext.TraceLeave(1017913, "Reporting", nameof (DataTransformService), "EnforceTransformOptions");
      }
    }

    public static void ValidateTransformOptions(
      IVssRequestContext requestContext,
      TransformOptions transformOptions)
    {
      try
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<TransformOptions>(transformOptions, nameof (transformOptions));
        ArgumentUtility.CheckStringForNullOrWhiteSpace(transformOptions.Filter, "Filter");
        if (string.IsNullOrEmpty(transformOptions.HistoryRange))
          ArgumentUtility.CheckStringForNullOrWhiteSpace(transformOptions.GroupBy, "GroupBy");
        ArgumentUtility.CheckForNull<OrderBy>(transformOptions.OrderBy, "transformOptionsOrderBy");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(transformOptions.OrderBy.PropertyName, "OrderByProperty");
        ArgumentUtility.CheckStringForNullOrWhiteSpace(transformOptions.OrderBy.Direction, "OrderByDirection");
        ArgumentUtility.CheckForNull<Measure>(transformOptions.Measure, "transformOptionsMeasure");
        AggregationMediator.CheckMeasureParameters(requestContext, transformOptions.Measure);
        if (transformOptions.OrderBy.Direction != "ascending" && transformOptions.OrderBy.Direction != "descending")
          throw new InvalidTransformOptionsException(ReportingResources.InvalidOrderDirection((object) transformOptions.OrderBy.Direction));
      }
      catch (ArgumentException ex)
      {
        throw new InvalidTransformOptionsException((Exception) ex);
      }
    }

    public static void SetProjectNameFromProjectGuidInFilterContext(
      IVssRequestContext requestContext,
      IDictionary filterContext,
      Guid? requestProjectId)
    {
      if (!requestProjectId.HasValue && filterContext == null || !filterContext.Contains((object) "projectId"))
        return;
      string input = filterContext[(object) "projectId"] as string;
      Guid result = Guid.Empty;
      if (string.IsNullOrWhiteSpace(input) || !Guid.TryParse(input, out result))
        return;
      if (requestProjectId.HasValue && requestProjectId.Value != result)
        throw new InvalidTransformOptionsException("Project, when specified in filter Context must refer to same project as requested Project Scope .");
      IProjectService service = requestContext.GetService<IProjectService>();
      filterContext[(object) "project"] = (object) service.GetProject(requestContext, result).Name;
    }

    private void ValidateInput(
      IVssRequestContext requestContext,
      IEnumerable<TransformOptions> options)
    {
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<TransformOptions>>(options, nameof (options));
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) options, nameof (options));
      }
      catch (ArgumentException ex)
      {
        throw new InvalidTransformOptionsException((Exception) ex);
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
