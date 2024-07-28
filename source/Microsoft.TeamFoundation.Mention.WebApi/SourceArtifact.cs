// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.WebApi.SourceArtifact
// Assembly: Microsoft.TeamFoundation.Mention.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A216308E-82C1-47B3-82AD-22E5F3709BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Mention.WebApi
{
  [DataContract]
  public class SourceArtifact
  {
    public const string TextFormat_Plain = "plain";
    public const string TextFormat_Rich = "rich";
    public const string TextFormat_Markdown = "markdown";

    [DataMember]
    public TeamProjectReference Project { get; set; }

    [DataMember]
    public string Type { get; set; }

    [DataMember]
    public EventScope Scope { get; set; }

    [DataMember]
    public string Identifier { get; set; }

    [DataMember]
    public string DisplayName { get; set; }

    [DataMember]
    public string GenericName { get; set; }

    [DataMember]
    public string Title { get; set; }

    [DataMember]
    public string Uri { get; set; }

    [DataMember]
    public string Text { get; set; }

    [DataMember]
    public string TextFormat { get; set; }

    [DataMember]
    public string FieldRefName { get; set; }

    [DataMember]
    public string FieldName { get; set; }
  }
}
