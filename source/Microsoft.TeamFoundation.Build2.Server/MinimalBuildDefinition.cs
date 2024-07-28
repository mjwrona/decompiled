// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.MinimalBuildDefinition
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build2.Server
{
  [DataContract]
  public class MinimalBuildDefinition
  {
    private Uri m_uri;

    public MinimalBuildDefinition()
    {
      this.Type = DefinitionType.Build;
      this.QueueStatus = DefinitionQueueStatus.Enabled;
      this.JobAuthorizationScope = BuildAuthorizationScope.ProjectCollection;
    }

    public string GetToken() => BuildDefinition.GetToken(this.ProjectId, this.Path, this.Id);

    [IgnoreDataMember]
    public Guid ProjectId { get; set; }

    [DataMember]
    public int Id { get; set; }

    [DataMember]
    public int? Revision { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public DefinitionType Type { get; set; }

    [DataMember]
    public DefinitionQueueStatus QueueStatus { get; set; }

    [DataMember]
    public BuildAuthorizationScope JobAuthorizationScope { get; set; }

    [DataMember]
    public Uri Uri
    {
      get
      {
        if (this.m_uri == (Uri) null)
          this.m_uri = new Uri(LinkingUtilities.EncodeUri(new ArtifactId()
          {
            ToolSpecificId = this.Id.ToString((IFormatProvider) CultureInfo.InvariantCulture),
            ArtifactType = "Definition",
            Tool = "Build"
          }), UriKind.Absolute);
        return this.m_uri;
      }
      set => this.m_uri = value;
    }
  }
}
