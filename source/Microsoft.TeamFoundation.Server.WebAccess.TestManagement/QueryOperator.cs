// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.QueryOperator
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  public class QueryOperator
  {
    public string RawValue { get; private set; }

    public string DisplayName { get; private set; }

    public QueryOperator(string rawValue, string displayName)
    {
      this.RawValue = rawValue;
      this.DisplayName = displayName;
    }
  }
}
