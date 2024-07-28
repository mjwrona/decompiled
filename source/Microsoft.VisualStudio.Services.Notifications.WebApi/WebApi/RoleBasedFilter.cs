// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.RoleBasedFilter
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  [DataContract]
  public abstract class RoleBasedFilter : ExpressionFilter
  {
    private IList<string> m_inclusions;
    private IList<string> m_exclusions;

    public RoleBasedFilter(string eventType, ExpressionFilterModel filterModel)
      : base(eventType, filterModel)
    {
    }

    [DataMember]
    [JsonProperty]
    public IList<string> Inclusions
    {
      get
      {
        if (this.m_inclusions == null)
          this.m_inclusions = (IList<string>) new List<string>();
        return this.m_inclusions;
      }
      internal set => this.m_inclusions = value;
    }

    [DataMember]
    [JsonProperty]
    public IList<string> Exclusions
    {
      get
      {
        if (this.m_exclusions == null)
          this.m_exclusions = (IList<string>) new List<string>();
        return this.m_exclusions;
      }
      internal set => this.m_exclusions = value;
    }
  }
}
