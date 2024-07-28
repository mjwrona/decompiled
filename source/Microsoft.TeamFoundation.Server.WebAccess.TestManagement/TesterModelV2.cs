// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TesterModelV2
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract(Name = "TesterModel")]
  internal class TesterModelV2
  {
    public TesterModelV2(Guid id, string displayName, string uniqueName)
    {
      this.DisplayName = displayName;
      this.Id = id;
      this.UniqueName = uniqueName;
    }

    [DataMember(Name = "id")]
    public Guid Id { get; set; }

    [DataMember(Name = "displayName")]
    public string DisplayName { get; set; }

    [DataMember(Name = "uniqueName")]
    public string UniqueName { get; set; }

    [DataMember(Name = "entityId")]
    public string EntityId { get; set; }
  }
}
