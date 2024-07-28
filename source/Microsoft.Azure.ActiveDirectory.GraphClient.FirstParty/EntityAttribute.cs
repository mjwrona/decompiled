// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.ActiveDirectory.GraphClient.EntityAttribute
// Assembly: Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty, Version=1.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 7AAC33C3-FEBB-470D-AB81-20ABE7F3618F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.ActiveDirectory.GraphClient.FirstParty.dll

using System;

namespace Microsoft.Azure.ActiveDirectory.GraphClient
{
  [AttributeUsage(AttributeTargets.Class)]
  public class EntityAttribute : Attribute
  {
    public EntityAttribute(string setName, string[] odataTypes)
    {
      this.SetName = setName;
      this.ODataTypes = odataTypes;
    }

    public string SetName { get; set; }

    public string[] ODataTypes { get; set; }
  }
}
