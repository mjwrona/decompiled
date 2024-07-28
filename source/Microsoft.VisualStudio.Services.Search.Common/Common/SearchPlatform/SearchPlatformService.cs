// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchPlatform.SearchPlatformService
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common.SearchPlatform
{
  public class SearchPlatformService : ISearchPlatformService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      if (systemRequestContext == null)
        throw new ArgumentNullException(nameof (systemRequestContext));
      if (!systemRequestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        throw new UnsupportedHostTypeException();
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public KeyValuePair<string, string> GetCredentials(IVssRequestContext requestContext) => new BasicAuthCredentialsHelper().GetCredentials(requestContext);
  }
}
