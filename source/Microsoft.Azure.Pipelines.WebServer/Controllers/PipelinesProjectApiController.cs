// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebServer.Controllers.PipelinesProjectApiController
// Assembly: Microsoft.Azure.Pipelines.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 734D9167-50DB-4E4F-ADE9-FD0A74DAB1E7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebServer.dll

using Microsoft.Azure.Pipelines.WebApiConverters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.Azure.Pipelines.WebServer.Controllers
{
  public class PipelinesProjectApiController : TfsProjectApiController
  {
    private int? m_resourceVersion;

    protected int GetResourceVersion()
    {
      if (!this.m_resourceVersion.HasValue)
      {
        ApiResourceVersion apiResourceVersion = this.Request.GetApiResourceVersion(VssRestApiVersionsRegistry.GetLatestReleasedVersion());
        if (apiResourceVersion.ResourceVersion > 0)
        {
          this.m_resourceVersion = new int?(apiResourceVersion.ResourceVersion);
        }
        else
        {
          VersionedApiControllerCustomNameAttribute customNameAttribute = this.GetType().GetCustomAttributes(typeof (VersionedApiControllerCustomNameAttribute), true).Cast<VersionedApiControllerCustomNameAttribute>().OrderByDescending<VersionedApiControllerCustomNameAttribute, int>((Func<VersionedApiControllerCustomNameAttribute, int>) (a => a.ResourceVersion)).FirstOrDefault<VersionedApiControllerCustomNameAttribute>();
          this.m_resourceVersion = new int?(customNameAttribute != null ? customNameAttribute.ResourceVersion : 1);
        }
      }
      return this.m_resourceVersion.Value;
    }

    protected TConverter GetClientConverter<TConverter>() where TConverter : class, new() => this.TfsRequestContext.CreateVersionedObject<TConverter>(this.GetResourceVersion());

    protected void CheckUrlSignatureValidated(IVssRequestContext requestContext)
    {
      bool flag;
      if (!requestContext.TryGetItem<bool>(RequestContextItemsKeys.RequestUriSignatureValidated, out flag) || !flag)
        throw new NotSupportedException(WebServerResources.RequestUrlSignatureValidationRequired());
    }
  }
}
