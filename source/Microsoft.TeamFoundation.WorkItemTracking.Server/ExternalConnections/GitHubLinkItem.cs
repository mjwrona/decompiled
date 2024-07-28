// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections.GitHubLinkItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.ExternalConnections
{
  [DataContract]
  public class GitHubLinkItem
  {
    private object item;

    [DataMember]
    [JsonConverter(typeof (StringEnumConverter))]
    public GitHubLinkItemType ItemType { get; set; }

    [DataMember]
    public string ProviderKey { get; set; }

    [DataMember]
    public string ConnectionAuthenticationType { get; set; }

    [DataMember]
    public Guid RepoInternalId { get; set; }

    [DataMember]
    public string RepoNameWithOwner { get; set; }

    [IgnoreDataMember]
    public string RepoExternalId { get; set; }

    [DataMember]
    public string NumberOrSHA { get; set; }

    [DataMember]
    public Author Author { get; set; }

    [DataMember]
    public string State { get; set; }

    [DataMember]
    public string ReviewDecision { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? Date { get; set; }

    [DataMember]
    public string Url { get; set; }

    [DataMember]
    public string ErrorMessage { get; set; }

    public T GetItem<T>() where T : IExternalArtifact => (T) this.item;

    public void SetItem(IExternalArtifact item) => this.item = (object) item;
  }
}
