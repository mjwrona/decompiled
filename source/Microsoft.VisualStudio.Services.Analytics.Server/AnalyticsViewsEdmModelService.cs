// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.AnalyticsViewsEdmModelService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.OData.Edm;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure;
using System;

namespace Microsoft.VisualStudio.Services.Analytics
{
  internal class AnalyticsViewsEdmModelService : IAnalyticsViewsEdmModelService, IVssFrameworkService
  {
    public string GetLatestSupportedModelVersionForViews(IVssRequestContext requestContext)
    {
      int version = 2;
      ModelVersionExtension.VersionState versionState = requestContext.GetVersionState(version);
      string modelVersionForViews = string.Format("v{0}.0", (object) version);
      if (versionState.HasFlag((Enum) ModelVersionExtension.VersionState.Preview))
        modelVersionForViews += "-preview";
      return modelVersionForViews;
    }

    public IEdmModel GetEdmModel(
      IVssRequestContext requestContext,
      string modelVersion,
      ProjectInfo project = null)
    {
      this.EnsureRequestContextODataModelVersionIsSet(requestContext, modelVersion);
      return requestContext.GetService<AnalyticsMetadataService>().GetModel(requestContext, project).EdmModel;
    }

    private void EnsureRequestContextODataModelVersionIsSet(
      IVssRequestContext requestContext,
      string fallbackModelVersion)
    {
      try
      {
        requestContext.GetODataModelVersion();
      }
      catch (UnsupportedODataModelVersionException ex)
      {
        int odataModelVersion = ModelVersionExtension.ExtractODataModelVersion(requestContext, fallbackModelVersion);
        requestContext.SetODataModelVersion(odataModelVersion);
      }
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
