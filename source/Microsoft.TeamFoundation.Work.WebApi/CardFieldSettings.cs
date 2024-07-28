// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Work.WebApi.CardFieldSettings
// Assembly: Microsoft.TeamFoundation.Work.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0C4CCFA0-0616-4E48-A4F0-952E1CB10B12
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Work.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Work.WebApi
{
  [DataContract]
  public class CardFieldSettings
  {
    [DataMember]
    public bool ShowId;
    [DataMember]
    public bool ShowAssignedTo;
    [DataMember]
    public IdentityDisplayFormat AssignedToDisplayFormat;
    [DataMember]
    public bool ShowState;
    [DataMember]
    public bool ShowTags;
    [DataMember]
    public bool ShowParent;
    [DataMember]
    public bool ShowEmptyFields;
    [DataMember]
    public bool ShowChildRollup;
    [DataMember]
    public IEnumerable<FieldInfo> AdditionalFields;
    [DataMember]
    public IEnumerable<FieldInfo> CoreFields;
  }
}
