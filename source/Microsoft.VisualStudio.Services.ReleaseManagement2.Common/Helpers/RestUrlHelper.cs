// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers.RestUrlHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C3F75541-7C8A-4AF6-A47E-709CEEE7550D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers
{
  public static class RestUrlHelper
  {
    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "//TODO: Yadu")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "//TODO: Yadu")]
    public static string GetRestUrlForRelease(
      IVssRequestContext requestContext,
      Guid projectId,
      int releaseId)
    {
      try
      {
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Release", ReleaseManagementApiConstants.ReleasesLocationGuid, projectId, (object) new
        {
          releaseId = releaseId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972016, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to get the Resource Url. Exception {0}", (object) ex);
        return string.Empty;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "//TODO: Yadu")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "//TODO: Yadu")]
    public static string GetRestUrlForReleaseDefinition(
      IVssRequestContext requestContext,
      Guid projectId,
      int definitionId)
    {
      try
      {
        return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "Release", ReleaseManagementApiConstants.ReleaseDefinitionsLocationId, projectId, (object) new
        {
          definitionId = definitionId
        }).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972016, TraceLevel.Error, "ReleaseManagementService", "Events", "Failed to get the Resource Url. Exception {0}", (object) ex);
        return string.Empty;
      }
    }

    [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", Justification = "This is not shown to the user and is for tracing")]
    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO:DIBATHOJ to come up with custom exception")]
    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "As per design")]
    public static string GetRestUrlForArtifacts(
      IVssRequestContext requestContext,
      string serviceArea,
      Guid locationId,
      Guid projectId,
      object routeValues)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, serviceArea, locationId, projectId, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1972016, TraceLevel.Error, "Pipeline", "Events", "Failed to get the Resource Url. Exception {0}", (object) ex);
        return string.Empty;
      }
    }

    [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "TODO:dibathoj to come up generic exception")]
    [SuppressMessage("Microsoft.Design", "CA1055:UriReturnValuesShouldNotBeStrings", Justification = "As per design")]
    public static string GetResourceUrl(
      IVssRequestContext requestContext,
      string serviceArea,
      Guid locationId,
      object routeValues)
    {
      ILocationService service = requestContext.GetService<ILocationService>();
      try
      {
        return service.GetResourceUri(requestContext, serviceArea, locationId, routeValues).AbsoluteUri;
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex, TeamFoundationEventId.DefaultWarningEventId, EventLogEntryType.Warning);
        return string.Empty;
      }
    }
  }
}
