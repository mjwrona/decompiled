// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build.Server.Compatibility.WorkspaceTemplate2010
// Assembly: Microsoft.TeamFoundation.Build.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 50E8BB1D-C69C-4DD2-83BE-A8FFBFFA6298
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;

namespace Microsoft.TeamFoundation.Build.Server.Compatibility
{
  [ClassVisibility(ClientVisibility.Internal, ClientVisibility.Internal)]
  [XmlType("WorkspaceTemplate")]
  public sealed class WorkspaceTemplate2010 : IValidatable
  {
    private List<WorkspaceMapping2010> m_mappings = new List<WorkspaceMapping2010>();

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Internal)]
    public string DefinitionUri { get; set; }

    [ClientProperty(ClientVisibility.Internal, ClientVisibility.Internal, PropertyName = "InternalMappings")]
    public List<WorkspaceMapping2010> Mappings => this.m_mappings;

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public DateTime LastModifiedDate { get; set; }

    [XmlAttribute]
    [ClientProperty(ClientVisibility.Public)]
    public string LastModifiedBy { get; set; }

    internal int WorkspaceId { get; set; }

    internal Guid ProjectId { get; set; }

    void IValidatable.Validate(IVssRequestContext requestContext, ValidationContext context)
    {
      ArgumentValidation.Check("Mappings", (object) this.m_mappings, false);
      if (this.m_mappings.Count == 0)
        throw new ArgumentException(ResourceStrings.WorkspaceTemplateMustDefineMapping());
      ArgumentValidation.CheckUri("DefinitionUri", this.DefinitionUri, "Definition", true, (string) null);
      Dictionary<string, object> dictionary1 = new Dictionary<string, object>((IEqualityComparer<string>) VssStringComparer.FilePath);
      Dictionary<string, object> dictionary2 = new Dictionary<string, object>((IEqualityComparer<string>) TFStringComparer.VersionControlPath);
      for (int index = 0; index < this.m_mappings.Count; ++index)
      {
        Microsoft.TeamFoundation.Build.Server.Validation.CheckValidatable(requestContext, string.Empty, (IValidatable) this.m_mappings[index], false, context);
        if (dictionary2.ContainsKey(this.m_mappings[index].ServerItem))
          throw new ArgumentException(ResourceStrings.DuplicateWorkspaceMappingDefinition((object) this.m_mappings[index].ServerItem));
        if (!string.IsNullOrEmpty(this.m_mappings[index].LocalItem))
        {
          if (dictionary1.ContainsKey(this.m_mappings[index].LocalItem))
            throw new ArgumentException(ResourceStrings.DuplicateWorkspaceMappingDefinition((object) this.m_mappings[index].LocalItem));
          dictionary1.Add(this.m_mappings[index].LocalItem, (object) null);
        }
        dictionary2.Add(this.m_mappings[index].ServerItem, (object) null);
      }
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "[WorkspaceTemplate2010 WorkspaceId={0} DefinitionUri={1} Mappings={2}]", (object) this.WorkspaceId, (object) this.DefinitionUri, (object) this.Mappings.ListItems<WorkspaceMapping2010>());
  }
}
