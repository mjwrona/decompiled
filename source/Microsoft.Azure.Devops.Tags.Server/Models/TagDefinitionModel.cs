// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Tags.Server.Models.TagDefinitionModel
// Assembly: Microsoft.Azure.Devops.Tags.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10FDC8E3-D1DB-4668-B2F2-04DAA10A7143
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Devops.Tags.Server.dll

using Microsoft.TeamFoundation.Server.WebAccess;
using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Devops.Tags.Server.Models
{
  [DataContract]
  public class TagDefinitionModel
  {
    public TagDefinitionModel(TagDefinition td)
      : this(td.TagId, td.Name, td.Scope, td.LastUpdated)
    {
    }

    public TagDefinitionModel(Guid tagId, string name, Guid scope, DateTime lastUpdated)
    {
      this.TagId = tagId;
      this.Name = name;
      this.Scope = scope;
      this.LastUpdated = lastUpdated;
    }

    [DataMember(Name = "tagId", EmitDefaultValue = false)]
    public Guid TagId { get; set; }

    [DataMember(Name = "name", EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(Name = "projectScope", EmitDefaultValue = false)]
    public Guid Scope { get; set; }

    [DataMember(Name = "lastUpdated", EmitDefaultValue = false)]
    public DateTime LastUpdated { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["tagId"] = (object) this.TagId;
      json["name"] = (object) this.Name;
      json["projectScope"] = (object) this.Scope;
      json["lastUpdated"] = (object) this.LastUpdated;
      return json;
    }
  }
}
