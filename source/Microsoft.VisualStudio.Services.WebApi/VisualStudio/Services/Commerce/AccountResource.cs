// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.AccountResource
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class AccountResource : Resource, IEquatable<AccountResource>
  {
    public AccountResource(string id, string name, string type)
      : base(id, name, type)
    {
    }

    public Dictionary<string, string> properties { get; set; } = new Dictionary<string, string>();

    bool IEquatable<AccountResource>.Equals(AccountResource other) => this.Equals((object) other) && this.properties.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) other.properties).Count<KeyValuePair<string, string>>() == 0 && other.properties.Except<KeyValuePair<string, string>>((IEnumerable<KeyValuePair<string, string>>) this.properties).Count<KeyValuePair<string, string>>() == 0;
  }
}
