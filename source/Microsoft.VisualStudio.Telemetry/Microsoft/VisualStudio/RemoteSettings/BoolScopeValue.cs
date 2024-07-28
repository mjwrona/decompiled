// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.RemoteSettings.BoolScopeValue
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

namespace Microsoft.VisualStudio.RemoteSettings
{
  public sealed class BoolScopeValue : ScopeValue
  {
    private bool value;

    public BoolScopeValue(bool value) => this.value = value;

    internal override AsyncScopeParser.AsyncOperand GetAsyncOperand() => (AsyncScopeParser.AsyncOperand) new AsyncScopeParser.BoolOperand(this.value);

    internal override ScopeParser.Operand GetOperand() => (ScopeParser.Operand) new ScopeParser.BoolOperand(this.value);
  }
}
