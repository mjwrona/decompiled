// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeFieldInstanceBase
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public abstract class WorkItemTypeFieldInstanceBase : WorkItemFieldReference
  {
    private WorkItemFieldReference m_fieldReference;

    public WorkItemTypeFieldInstanceBase()
    {
    }

    public WorkItemTypeFieldInstanceBase(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public WorkItemFieldReference Field
    {
      get
      {
        if (this.m_fieldReference == null)
          this.m_fieldReference = new WorkItemFieldReference()
          {
            Name = this.Name,
            ReferenceName = this.ReferenceName,
            Url = this.Url
          };
        return this.m_fieldReference;
      }
      set => this.m_fieldReference = value;
    }

    [DataMember(EmitDefaultValue = false)]
    public string HelpText { get; set; }

    [DataMember]
    public bool AlwaysRequired { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public IEnumerable<WorkItemFieldReference> DependentFields { get; set; }

    public bool IsIdentity { get; set; }

    public override int GetHashCode() => base.GetHashCode();

    public override bool Equals(object obj)
    {
      WorkItemTypeFieldInstance typeFieldInstance = obj as WorkItemTypeFieldInstance;
      return base.Equals(obj) && typeFieldInstance != null && typeFieldInstance.HelpText == this.HelpText && typeFieldInstance.AlwaysRequired == this.AlwaysRequired;
    }
  }
}
