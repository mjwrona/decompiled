// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Helpers.WitApiVersionHelpers
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common.Helpers
{
  internal static class WitApiVersionHelpers
  {
    public static bool SupportsVersion(IVssRequestContext requestContext, Version version)
    {
      bool flag = true;
      ApiResourceVersion apiResourceVersion = (ApiResourceVersion) null;
      if (requestContext.TryGetItem<ApiResourceVersion>("WitApiResourceVersion", out apiResourceVersion))
        flag = apiResourceVersion.ApiVersion >= version;
      return flag;
    }
  }
}
