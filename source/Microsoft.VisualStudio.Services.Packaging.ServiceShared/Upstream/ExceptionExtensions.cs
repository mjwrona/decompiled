// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.ExceptionExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class ExceptionExtensions
  {
    public static string RelatedUpstreamSourceKey = "Microsoft.Azure.Artifacts.Packaging.RelatedUpstreamSource";

    public static void SetRelatedUpstreamSource(this Exception ex, UpstreamSource upstream) => ex.Data[(object) ExceptionExtensions.RelatedUpstreamSourceKey] = (object) upstream.Id;

    public static bool HasRelatedUpstreamSource(this Exception ex) => ex.GetRelatedUpstreamSourceIdOrDefault().HasValue;

    public static Guid? GetRelatedUpstreamSourceIdOrDefault(this Exception ex) => (Guid?) ex.Data[(object) ExceptionExtensions.RelatedUpstreamSourceKey];
  }
}
