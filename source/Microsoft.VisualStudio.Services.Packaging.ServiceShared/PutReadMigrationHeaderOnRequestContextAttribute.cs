// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.PutReadMigrationHeaderOnRequestContextAttribute
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  internal class PutReadMigrationHeaderOnRequestContextAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!(actionContext?.ControllerContext?.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      KeyValuePair<string, IEnumerable<string>>? nullable1;
      if (actionContext == null)
      {
        nullable1 = new KeyValuePair<string, IEnumerable<string>>?();
      }
      else
      {
        HttpRequestMessage request = actionContext.Request;
        if (request == null)
        {
          nullable1 = new KeyValuePair<string, IEnumerable<string>>?();
        }
        else
        {
          HttpRequestHeaders headers = request.Headers;
          nullable1 = headers != null ? new KeyValuePair<string, IEnumerable<string>>?(headers.FirstOrDefault<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (header => header.Key.Equals(CodeOnlyDeploymentsConstants.ReadMigrationHeader, StringComparison.OrdinalIgnoreCase)))) : new KeyValuePair<string, IEnumerable<string>>?();
        }
      }
      KeyValuePair<string, IEnumerable<string>>? nullable2 = nullable1;
      if (!nullable2.HasValue)
        return;
      KeyValuePair<string, IEnumerable<string>> keyValuePair = nullable2.Value;
      if (keyValuePair.Equals((object) new KeyValuePair<string, IEnumerable<string>>()))
        return;
      keyValuePair = nullable2.Value;
      if (keyValuePair.Value.Count<string>() != 1)
        return;
      IDictionary<string, object> items = tfsRequestContext.Items;
      string migrationHeaderKey = CodeOnlyDeploymentsConstants.ReadMigrationHeaderKey;
      keyValuePair = nullable2.Value;
      string str = keyValuePair.Value.First<string>();
      items[migrationHeaderKey] = (object) str;
    }
  }
}
