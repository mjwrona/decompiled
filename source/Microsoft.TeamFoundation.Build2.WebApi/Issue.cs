// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.Issue
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  public class Issue : BaseSecuredObject
  {
    [DataMember(Name = "Data", EmitDefaultValue = false, Order = 4)]
    private IDictionary<string, string> m_data;

    public Issue()
    {
    }

    internal Issue(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    private Issue(Issue issueToBeCloned)
      : base((ISecuredObject) issueToBeCloned)
    {
      this.Type = issueToBeCloned.Type;
      this.Category = issueToBeCloned.Category;
      this.Message = issueToBeCloned.Message;
      if (issueToBeCloned.m_data == null)
        return;
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) issueToBeCloned.m_data)
        this.Data.Add(keyValuePair);
    }

    [DataMember(Order = 1)]
    public IssueType Type { get; set; }

    [DataMember(Order = 2)]
    public string Category { get; set; }

    [DataMember(Order = 3)]
    public string Message { get; set; }

    public IDictionary<string, string> Data
    {
      get
      {
        if (this.m_data == null)
          this.m_data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.m_data;
      }
    }

    public Issue Clone() => new Issue(this);
  }
}
