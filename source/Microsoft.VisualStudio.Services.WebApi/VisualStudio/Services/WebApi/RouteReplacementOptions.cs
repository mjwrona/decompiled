// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.RouteReplacementOptions
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.ComponentModel;

namespace Microsoft.VisualStudio.Services.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Flags]
  public enum RouteReplacementOptions
  {
    None = 0,
    EscapeUri = 1,
    AppendUnusedAsQueryParams = 2,
    RequireExplicitRouteParams = 4,
    WildcardAsQueryParams = 8,
  }
}
