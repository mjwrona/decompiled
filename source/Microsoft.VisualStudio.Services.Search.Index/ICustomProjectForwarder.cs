// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Index.ICustomProjectForwarder
// Assembly: Microsoft.VisualStudio.Services.Search.Index, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 75C10BA9-319D-46E2-AA64-F18680226A42
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Index.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Index
{
  public interface ICustomProjectForwarder
  {
    IEnumerable<string> ForwardGetProjectsRequest(IVssRequestContext requestContext);
  }
}
