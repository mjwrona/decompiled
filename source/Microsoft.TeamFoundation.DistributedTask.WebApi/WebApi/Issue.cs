// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.Issue
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  [DataContract]
  public class Issue
  {
    [DataMember(Name = "Data", EmitDefaultValue = false, Order = 4)]
    private IDictionary<string, string> m_serializedData;
    private IDictionary<string, string> m_data;

    public Issue()
    {
    }

    private Issue(Issue issueToBeCloned)
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

    [System.Runtime.Serialization.OnDeserialized]
    private void OnDeserialized(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_serializedData, ref this.m_data, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, true);

    [System.Runtime.Serialization.OnSerializing]
    private void OnSerializing(StreamingContext context) => SerializationHelper.Copy<string, string>(ref this.m_data, ref this.m_serializedData, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    [System.Runtime.Serialization.OnSerialized]
    private void OnSerialized(StreamingContext context) => this.m_serializedData = (IDictionary<string, string>) null;
  }
}
