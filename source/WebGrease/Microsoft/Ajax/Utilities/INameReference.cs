// Decompiled with JetBrains decompiler
// Type: Microsoft.Ajax.Utilities.INameReference
// Assembly: WebGrease, Version=1.6.5135.21930, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 86487675-C393-48D4-AFEC-7657DB09B21F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\WebGrease.dll

namespace Microsoft.Ajax.Utilities
{
  public interface INameReference
  {
    ActivationObject VariableScope { get; }

    bool IsAssignment { get; }

    AstNode AssignmentValue { get; }

    JSVariableField VariableField { get; }

    string Name { get; }

    long Index { get; }

    AstNode Parent { get; }
  }
}
