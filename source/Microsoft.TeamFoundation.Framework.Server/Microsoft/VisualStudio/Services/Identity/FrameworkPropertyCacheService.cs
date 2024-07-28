// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.FrameworkPropertyCacheService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  internal class FrameworkPropertyCacheService : IPropertyCacheService, IVssFrameworkService
  {
    private const string s_Area = "PropertyCache";
    private const string s_Layer = "FrameworkPropertyCacheService";

    public void ServiceStart(IVssRequestContext requestContext) => requestContext.CheckHostedDeployment();

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public string Cache<T>(IVssRequestContext requestContext, T value) where T : class
    {
      try
      {
        requestContext.TraceEnter(1095000, "PropertyCache", nameof (FrameworkPropertyCacheService), nameof (Cache));
        return this.GetHttpClient(requestContext).Cache<T>(value).SyncResult<string>();
      }
      finally
      {
        requestContext.TraceLeave(1095001, "PropertyCache", nameof (FrameworkPropertyCacheService), nameof (Cache));
      }
    }

    public T Get<T>(IVssRequestContext requestContext, string key) where T : class
    {
      ArgumentUtility.CheckStringForNullOrEmpty(key, nameof (key));
      return requestContext.GetClient<CacheHttpClient>().GetAsync(key).Result as T;
    }

    public void Delete(IVssRequestContext requestContext, string cacheKey)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(cacheKey, nameof (cacheKey));
      requestContext.GetClient<CacheHttpClient>().DeleteAsync(cacheKey).SyncResult();
    }

    internal virtual PropertyCacheHttpClient GetHttpClient(IVssRequestContext requestContext) => requestContext.GetClient<PropertyCacheHttpClient>();
  }
}
