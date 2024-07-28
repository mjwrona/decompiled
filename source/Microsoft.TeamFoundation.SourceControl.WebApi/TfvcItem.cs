// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.TfvcItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi
{
  [DataContract]
  public class TfvcItem : ItemModel
  {
    [DataMember(Name = "version", EmitDefaultValue = false)]
    public int ChangesetVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int DeletionId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsBranch { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool IsPendingChange { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime ChangeDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public long Size { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string HashValue { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int Encoding { get; set; }

    [IgnoreDataMember]
    public int Id { get; set; }

    [IgnoreDataMember]
    public int FileId { get; set; }
  }
}
