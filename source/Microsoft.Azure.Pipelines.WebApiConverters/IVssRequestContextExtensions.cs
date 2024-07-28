// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Pipelines.WebApiConverters.IVssRequestContextExtensions
// Assembly: Microsoft.Azure.Pipelines.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 42DDCCD8-4E0C-44F8-A5D2-AEF894388AED
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Pipelines.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.Azure.Pipelines.WebApiConverters
{
  public static class IVssRequestContextExtensions
  {
    public static T CreateVersionedObject<T>(
      this IVssRequestContext requestContext,
      int resourceVersion)
      where T : class, new()
    {
      return (T) VersionedObjectFactoryCache.Instance.GetFactory<T>().GetCreator(resourceVersion).Create();
    }
  }
}
