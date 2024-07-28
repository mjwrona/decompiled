// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.Client.GraphSubjectQuery
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Graph.Client
{
  [DataContract]
  public class GraphSubjectQuery
  {
    [DataMember]
    public string Query { get; private set; }

    [DataMember]
    public IEnumerable<string> SubjectKind { get; private set; }

    [DataMember]
    public SubjectDescriptor ScopeDescriptor { get; private set; }

    public GraphSubjectQuery(
      string query,
      IEnumerable<string> subjectKind,
      SubjectDescriptor scopeDescriptor)
    {
      this.Query = query;
      this.SubjectKind = subjectKind;
      this.ScopeDescriptor = scopeDescriptor;
    }
  }
}
