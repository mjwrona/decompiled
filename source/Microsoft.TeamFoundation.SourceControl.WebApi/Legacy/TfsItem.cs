// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebApi.Legacy.TfsItem
// Assembly: Microsoft.TeamFoundation.SourceControl.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 663B2C57-EC74-4E67-8BD7-7AC09B503305
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.SourceControl.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.SourceControl.WebApi.Legacy
{
  [DataContract]
  public class TfsItem : ItemModel
  {
    [DataMember(Name = "id", EmitDefaultValue = false)]
    public int Id { get; set; }

    [DataMember(Name = "changeset", EmitDefaultValue = false)]
    public int ChangesetVersion { get; set; }

    [DataMember(Name = "deletionId", EmitDefaultValue = false)]
    public int DeletionId { get; set; }

    [DataMember(Name = "isBranch", EmitDefaultValue = false)]
    public bool IsBranch { get; set; }

    [DataMember(Name = "isPendingChange", EmitDefaultValue = false)]
    public bool IsPendingChange { get; set; }

    [IgnoreDataMember]
    public int Encoding { get; set; }

    [IgnoreDataMember]
    public int FileId { get; set; }
  }
}
