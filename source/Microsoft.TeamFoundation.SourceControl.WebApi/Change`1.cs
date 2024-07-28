// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Change`1
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  [KnownType(typeof (TfvcChange))]
  [KnownType(typeof (GitChange))]
  public class Change<T> : VersionControlSecuredObject
  {
    [DataMember(EmitDefaultValue = false)]
    public T Item { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string SourceServerItem { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public VersionControlChangeType ChangeType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ItemContent NewContent { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Url { get; set; }

    public override void SetSecuredObject(ISecuredObject securedObject)
    {
      base.SetSecuredObject(securedObject);
      if (this.Item is VersionControlSecuredObject controlSecuredObject)
        controlSecuredObject.SetSecuredObject(securedObject);
      this.NewContent?.SetSecuredObject(securedObject);
    }
  }
}
