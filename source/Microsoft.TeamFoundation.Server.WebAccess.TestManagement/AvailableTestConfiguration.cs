// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.AvailableTestConfiguration
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  public class AvailableTestConfiguration
  {
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "values")]
    public string Values { get; set; }

    [DataMember(Name = "isActive")]
    public bool IsActive { get; set; }

    [DataMember(Name = "isAssigned")]
    public bool IsAssigned { get; set; }

    public AvailableTestConfiguration(int id, string name, string values, bool isActive)
    {
      this.Id = id;
      this.Name = name;
      this.Values = values;
      this.IsActive = isActive;
      this.IsAssigned = false;
    }
  }
}
