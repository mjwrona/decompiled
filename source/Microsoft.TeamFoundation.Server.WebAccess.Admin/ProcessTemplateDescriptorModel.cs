// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.ProcessTemplateDescriptorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public class ProcessTemplateDescriptorModel : IComparable<ProcessTemplateDescriptorModel>
  {
    public ProcessTemplateDescriptorModel(ProcessDescriptor processDescriptor, bool isDefault)
    {
      this.Id = processDescriptor.IntegerId;
      this.Name = processDescriptor.Name;
      this.Description = processDescriptor.Description;
      this.IsDefault = isDefault;
      this.TypeId = processDescriptor.TypeId;
      this.IsSystemTemplate = processDescriptor.Scope == ProcessScope.Deployment;
      this.Inherits = processDescriptor.Inherits;
    }

    public string Name { get; set; }

    public string Description { get; set; }

    public bool IsDefault { get; set; }

    public int Id { get; set; }

    public Guid TypeId { get; set; }

    public Guid Inherits { get; set; }

    public bool IsSystemTemplate { get; set; }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json["Name"] = (object) this.Name;
      json["Description"] = (object) this.Description;
      json["IsDefault"] = (object) this.IsDefault;
      json["Id"] = (object) this.Id;
      json["TypeId"] = (object) this.TypeId;
      json["Inherits"] = (object) this.Inherits;
      return json;
    }

    public int CompareTo(ProcessTemplateDescriptorModel other)
    {
      if (other == null)
        return -1;
      return this.IsSystemTemplate ? (!other.IsSystemTemplate ? -1 : this.Name.CompareTo(other.Name)) : (!other.IsSystemTemplate ? this.Name.CompareTo(other.Name) : 1);
    }
  }
}
