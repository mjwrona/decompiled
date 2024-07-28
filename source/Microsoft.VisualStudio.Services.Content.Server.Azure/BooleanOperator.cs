// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Azure.BooleanOperator
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Azure, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7823E4AE-BEB6-4A7C-9914-276DEAE1FB1F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Azure.dll

namespace Microsoft.VisualStudio.Services.Content.Server.Azure
{
  public struct BooleanOperator
  {
    public static readonly BooleanOperator And = new BooleanOperator("and");
    public static readonly BooleanOperator Or = new BooleanOperator("or");
    public readonly string OperatorString;

    private BooleanOperator(string operatorString) => this.OperatorString = " " + operatorString.Trim() + " ";
  }
}
