// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TenantPolicy.Policy
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.TenantPolicy
{
  public class Policy
  {
    public Policy(string name, bool value, IDictionary<string, string> properties)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.Value = value;
      this.Properties = properties != null ? properties.Copy<string, string>((IDictionary<string, string>) new Dictionary<string, string>()) : (IDictionary<string, string>) null;
    }

    internal Policy(string name, bool value = false)
    {
      ArgumentUtility.CheckForNull<string>(name, nameof (name));
      this.Name = name;
      this.Value = value;
      this.Properties = (IDictionary<string, string>) new Dictionary<string, string>();
    }

    public string Name { get; }

    public bool Value { get; }

    public IDictionary<string, string> Properties { get; }
  }
}
