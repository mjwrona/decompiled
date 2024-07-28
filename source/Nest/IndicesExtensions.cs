// Decompiled with JetBrains decompiler
// Type: Nest.IndicesExtensions
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;

namespace Nest
{
  public static class IndicesExtensions
  {
    public static string Resolve(this Indices marker, IConnectionSettingsValues connectionSettings)
    {
      if (marker == (Indices) null)
        return (string) null;
      connectionSettings.ThrowIfNull<IConnectionSettingsValues>(nameof (connectionSettings));
      return connectionSettings.Inferrer.Resolve((IUrlParameter) marker);
    }
  }
}
