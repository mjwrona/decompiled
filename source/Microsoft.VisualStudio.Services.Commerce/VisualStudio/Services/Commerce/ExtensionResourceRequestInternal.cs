// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ExtensionResourceRequestInternal
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal class ExtensionResourceRequestInternal : ExtensionResourceRequest
  {
    private readonly string resourceProviderNamespace;
    private const string RootResourceType = "account";
    private const string ResourceType = "extension";

    public Guid SubscriptionId { get; set; }

    public string ResourceGroupName { get; set; }

    public string RootResourceName { get; set; }

    public string ResourceName { get; set; }

    public ExtensionResourceRequestInternal()
    {
    }

    public ExtensionResourceRequestInternal(
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      string resourceProviderNamespace)
    {
      this.SubscriptionId = subscriptionId;
      this.ResourceGroupName = resourceGroupName;
      this.RootResourceName = accountResourceName;
      this.ResourceName = extensionResourceName;
      this.resourceProviderNamespace = resourceProviderNamespace;
    }

    public ExtensionResourceRequestInternal(
      ExtensionResourceRequest request,
      Guid subscriptionId,
      string resourceGroupName,
      string accountResourceName,
      string extensionResourceName,
      string resourceProviderNamespace)
      : this(subscriptionId, resourceGroupName, accountResourceName, extensionResourceName, resourceProviderNamespace)
    {
      this.Properties = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.Properties.AddRange<KeyValuePair<string, string>, Dictionary<string, string>>((IEnumerable<KeyValuePair<string, string>>) request.Properties);
      this.Location = request.Location;
      this.Tags = request.Tags;
      this.Plan = request.Plan;
    }

    public string GetResourcePath() => string.Format("/subscriptions/{0}/resourcegroups/{1}/providers/{2}", (object) this.SubscriptionId, (object) this.ResourceGroupName, (object) this.resourceProviderNamespace) + "/account/" + this.RootResourceName + "/extension/" + this.ResourceName;

    public override string ToString()
    {
      StringBuilder seed = new StringBuilder();
      seed.AppendLine("Location:" + this.Location + "; ");
      seed.AppendLine(string.Format("Plan: {0}", (object) this.Plan));
      if (this.Tags != null)
      {
        seed.Append("Tags[");
        this.Tags.Aggregate<KeyValuePair<string, string>, StringBuilder>(seed, (Func<StringBuilder, KeyValuePair<string, string>, StringBuilder>) ((b, t) => b.Append(t.Key + ":" + t.Value + "; ")));
        seed.Append("]; ");
      }
      if (this.Properties != null)
      {
        seed.Append("Properties[");
        this.Properties.Aggregate<KeyValuePair<string, string>, StringBuilder>(seed, (Func<StringBuilder, KeyValuePair<string, string>, StringBuilder>) ((b, p) => b.Append(p.Key + ":" + p.Value + "; ")));
        seed.Append("]; ");
      }
      return seed.ToString();
    }
  }
}
