// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestConfigurationModel
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.TeamFoundation.TestManagement.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  [DataContract]
  internal class TestConfigurationModel
  {
    public TestConfigurationModel(string name, int id)
    {
      this.Name = name;
      this.Id = id;
    }

    public TestConfigurationModel(TestConfiguration config)
    {
      this.Name = config.Name;
      this.Id = config.Id;
      this.Variables = config.Values;
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "variables")]
    public List<NameValuePair> Variables { get; set; }

    public override int GetHashCode() => this.Id;

    public override bool Equals(object obj) => obj is TestConfigurationModel configurationModel && this.Id == configurationModel.Id && this.Name.Equals(configurationModel.Name, StringComparison.InvariantCultureIgnoreCase);
  }
}
