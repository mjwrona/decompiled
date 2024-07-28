// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.PredicateRule
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F602B8A6-9B4B-4971-8764-E3FEAFAB8CD5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions.dll

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Extensions
{
  public class PredicateRule
  {
    public string inputName { get; set; }

    public string condition { get; set; }

    public string expectedValue { get; set; }

    public PredicateRule(string inputName, string condition, string expectedValue)
    {
      this.inputName = inputName;
      this.condition = condition;
      this.expectedValue = expectedValue;
    }
  }
}
