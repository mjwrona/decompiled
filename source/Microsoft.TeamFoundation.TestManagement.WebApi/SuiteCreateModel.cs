// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.WebApi.SuiteCreateModel
// Assembly: Microsoft.TeamFoundation.TestManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10F0A812-3ECA-42B4-865D-429941F99EBE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.TestManagement.WebApi.dll

using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.TestManagement.WebApi
{
  [DataContract]
  public class SuiteCreateModel
  {
    public SuiteCreateModel(
      string suiteType,
      string name = "",
      string queryString = "",
      int[] requirementIds = null)
    {
      this.SuiteType = suiteType;
      this.QueryString = queryString;
      this.RequirementIds = requirementIds;
      this.Name = name;
    }

    [DataMember(Name = "name")]
    public string Name { get; private set; }

    [DataMember(Name = "suiteType")]
    public string SuiteType { get; private set; }

    [DataMember(Name = "queryString")]
    public string QueryString { get; private set; }

    [DataMember(Name = "requirementIds")]
    public int[] RequirementIds { get; private set; }
  }
}
