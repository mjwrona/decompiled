// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.ICustomTenantForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public interface ICustomTenantForwarder
  {
    bool ForwardRegisterTenantRequest(IVssRequestContext requestContext, CustomTenant tenant);

    IEnumerable<string> ForwardGetTenantCollectionNamesRequest(
      IVssRequestContext requestContext,
      string tenantName);
  }
}
