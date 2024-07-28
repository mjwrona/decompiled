// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Reporting.DataServices.DataServicesTimeoutPolicy
// Assembly: Microsoft.TeamFoundation.Reporting.DataServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0871DF71-209E-4628-905A-D95195A70FEC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Reporting.DataServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Reporting.DataServices
{
  public class DataServicesTimeoutPolicy
  {
    private static readonly TimeSpan TransformerDefaultTimeout = TimeSpan.FromSeconds(31.0);
    private static readonly TimeSpan DataQueryDefaultTimeout = TimeSpan.FromSeconds(31.0);
    public const string TransformerTimeoutRegistryPath = "/Service/DataServices/Settings/TransformerTimeout";
    public const string DataQueryTimeoutRegistryPath = "/Service/DataServices/Settings/DataQueryTimeout";

    public static TimeSpan GetTransformerTimeout(IVssRequestContext requestContext) => DataServicesTimeoutPolicy.GetTimeout(requestContext, "/Service/DataServices/Settings/TransformerTimeout", DataServicesTimeoutPolicy.TransformerDefaultTimeout);

    public static TimeSpan GetDataQueryTimeout(IVssRequestContext requestContext) => DataServicesTimeoutPolicy.GetTimeout(requestContext, "/Service/DataServices/Settings/DataQueryTimeout", DataServicesTimeoutPolicy.DataQueryDefaultTimeout);

    public static TimeSpan GetTimeout(
      IVssRequestContext requestContext,
      string timeoutRegistryPath,
      TimeSpan defaultTimeout)
    {
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      return service == null ? defaultTimeout : service.GetValue<TimeSpan>(requestContext, (RegistryQuery) timeoutRegistryPath, defaultTimeout);
    }
  }
}
