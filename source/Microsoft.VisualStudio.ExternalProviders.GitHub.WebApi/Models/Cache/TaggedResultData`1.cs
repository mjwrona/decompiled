// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Cache.TaggedResultData`1
// Assembly: Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4FE25D33-B783-4B98-BAFC-7E522D8D8D08
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.dll

namespace Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi.Models.Cache
{
  public class TaggedResultData<T>
  {
    public string[] Tags { get; set; }

    public string PageUrl { get; set; }

    public T Value { get; set; }

    public TaggedResultData(string[] tags, string page, T value)
    {
      this.Tags = tags;
      this.PageUrl = page;
      this.Value = value;
    }
  }
}
