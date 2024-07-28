// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.CodeAnalysis.NotNullIfNotNullAttribute
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

namespace System.Diagnostics.CodeAnalysis
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
  internal sealed class NotNullIfNotNullAttribute : Attribute
  {
    public NotNullIfNotNullAttribute(string parameterName) => this.ParameterName = parameterName;

    public string ParameterName { get; }
  }
}
