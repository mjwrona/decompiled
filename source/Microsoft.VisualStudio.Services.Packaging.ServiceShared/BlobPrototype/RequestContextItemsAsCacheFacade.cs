// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.RequestContextItemsAsCacheFacade
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class RequestContextItemsAsCacheFacade : ICache<string, object>, IInvalidatableCache<string>
  {
    private readonly IVssRequestContext requestContext;

    public RequestContextItemsAsCacheFacade(IVssRequestContext requestContext) => this.requestContext = requestContext;

    public bool TryGet(string key, out object val)
    {
      string newKey;
      if (!this.ParseKeyAsPropertyKey(key, out newKey))
        return this.requestContext.Items.TryGetValue(key, out val);
      val = this.requestContext.GetPackagingTracesProperty(newKey);
      return val != null;
    }

    private bool ParseKeyAsPropertyKey(string key, out string newKey)
    {
      if (key.StartsWith("Packaging.Properties."))
      {
        newKey = key.Remove(0, "Packaging.Properties".Length + 1);
        return true;
      }
      newKey = (string) null;
      return false;
    }

    public bool Has(string key)
    {
      string newKey;
      return this.ParseKeyAsPropertyKey(key, out newKey) ? this.requestContext.GetPackagingTracesProperty(newKey) != null : this.requestContext.Items.ContainsKey(key);
    }

    public bool Set(string key, object val)
    {
      string newKey;
      if (this.ParseKeyAsPropertyKey(key, out newKey))
      {
        this.requestContext.AddPackagingTracesProperty(newKey, val);
        return true;
      }
      this.requestContext.Items[key] = val;
      return true;
    }

    public void Invalidate(string key)
    {
      string newKey;
      if (this.ParseKeyAsPropertyKey(key, out newKey))
        this.requestContext.AddPackagingTracesProperty(newKey, (object) null);
      this.requestContext.Items.Remove(key);
    }
  }
}
