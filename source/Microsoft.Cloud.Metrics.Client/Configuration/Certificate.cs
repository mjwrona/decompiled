// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Configuration.Certificate
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Cloud.Metrics.Client.Configuration
{
  [Obsolete]
  public sealed class Certificate : IPermission
  {
    [JsonConstructor]
    public Certificate(string identity, Role role)
    {
      this.Identity = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof (identity));
      this.Role = role;
    }

    public string Identity { get; }

    public Role Role { get; set; }
  }
}
