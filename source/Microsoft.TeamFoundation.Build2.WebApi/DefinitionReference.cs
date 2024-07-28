// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.WebApi.DefinitionReference
// Assembly: Microsoft.TeamFoundation.Build2.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0683407D-7C61-4505-8CA6-86AD7E3B6BCA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Build2.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Build.WebApi
{
  [DataContract]
  [KnownType(typeof (BuildDefinition))]
  [KnownType(typeof (BuildDefinitionReference))]
  [KnownType(typeof (XamlBuildDefinition))]
  [JsonConverter(typeof (DefinitionReferenceJsonConverter))]
  public class DefinitionReference : ShallowReference, ISecuredObject
  {
    private int m_requiredPermissions = BuildPermissions.ViewBuildDefinition;
    private string m_nestingSecurityToken = string.Empty;

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new int Id
    {
      get => base.Id;
      set => base.Id = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Name
    {
      get => base.Name;
      set => base.Name = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public new string Url
    {
      get => base.Url;
      set => base.Url = value;
    }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri Uri { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string Path { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public DefinitionType Type { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = true)]
    public DefinitionQueueStatus QueueStatus { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Revision { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime CreatedDate { get; set; }

    [DataMember(EmitDefaultValue = false, Order = 1000)]
    public TeamProjectReference Project { get; set; }

    Guid ISecuredObject.NamespaceId => Security.BuildNamespaceId;

    int ISecuredObject.RequiredPermissions => this.m_requiredPermissions;

    string ISecuredObject.GetToken() => !string.IsNullOrEmpty(this.m_nestingSecurityToken) ? this.m_nestingSecurityToken : DefinitionReference.GetToken(this.Project, this.Path, this.Id);

    internal void SetRequiredPermissions(int newValue) => this.m_requiredPermissions = newValue;

    internal void SetNestingSecurityToken(string tokenValue)
    {
      if (this is BuildDefinitionReference)
        this.m_nestingSecurityToken = string.Empty;
      else
        this.m_nestingSecurityToken = tokenValue;
    }

    internal static string GetToken(TeamProjectReference project, string path, int definitionId) => DefinitionReference.GetToken(project?.Id, path, definitionId);

    internal static string GetToken(Guid? projectId, string path, int definitionId) => (projectId?.ToString("D") ?? string.Empty) + Security.GetSecurityTokenPath(path ?? string.Empty) + (object) definitionId;
  }
}
