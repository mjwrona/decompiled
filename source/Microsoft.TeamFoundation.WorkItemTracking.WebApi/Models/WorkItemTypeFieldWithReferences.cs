// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItemTypeFieldWithReferences
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models
{
  [DataContract]
  public class WorkItemTypeFieldWithReferences : WorkItemTypeFieldInstanceBase
  {
    public WorkItemTypeFieldWithReferences()
    {
    }

    public WorkItemTypeFieldWithReferences(ISecuredObject securedObject)
      : base(securedObject)
    {
    }

    public WorkItemTypeFieldWithReferences(WorkItemTypeFieldInstance fieldInstance)
      : base((ISecuredObject) fieldInstance)
    {
      this.Name = fieldInstance.Name;
      this.ReferenceName = fieldInstance.ReferenceName;
      this.Url = fieldInstance.Url;
      this.HelpText = fieldInstance.HelpText;
      this.Field = fieldInstance.Field;
      this.AlwaysRequired = fieldInstance.AlwaysRequired;
      this.DefaultValue = (object) fieldInstance.DefaultValue;
      this.DependentFields = fieldInstance.DependentFields;
      this.AllowedValues = (object[]) fieldInstance.AllowedValues;
      this.IsIdentity = fieldInstance.IsIdentity;
    }

    [DataMember]
    [JsonConverter(typeof (IdentityRefOrJValueConverter))]
    public object DefaultValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    [JsonConverter(typeof (ValueArrayConverter))]
    public object[] AllowedValues { get; set; }
  }
}
