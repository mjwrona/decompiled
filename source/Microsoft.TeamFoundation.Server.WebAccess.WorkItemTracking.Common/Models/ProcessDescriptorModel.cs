// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.ProcessDescriptorModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.Azure.Boards.ProcessTemplates;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  [DataContract]
  public class ProcessDescriptorModel : BaseSecuredObjectModel
  {
    [DataMember]
    public Guid Id { get; set; }

    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public bool IsInherited { get; set; }

    [DataMember]
    public bool IsSystem { get; set; }

    [DataMember]
    public bool CanEditProcess { get; set; }

    public ProcessDescriptorModel(
      IVssRequestContext requestContext,
      ProcessDescriptor processDescriptor,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      ITeamFoundationProcessService service = requestContext.GetService<ITeamFoundationProcessService>();
      this.Id = processDescriptor.TypeId;
      this.Name = processDescriptor.Name;
      this.IsInherited = processDescriptor.IsDerived;
      this.IsSystem = processDescriptor.IsSystem;
      this.CanEditProcess = service.HasProcessPermission(requestContext, 1, processDescriptor);
    }

    public JsObject ToJson()
    {
      JsObject json = new JsObject();
      json.Add("id", (object) this.Id);
      json.Add("name", (object) this.Name);
      json.Add("isInherited", (object) this.IsInherited);
      json.Add("isSystem", (object) this.IsSystem);
      json.Add("canEditProcess", (object) this.CanEditProcess);
      return json;
    }
  }
}
