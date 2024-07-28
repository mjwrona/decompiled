// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.CodeAnalysis.MemberNotNullAttribute
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll


#nullable enable
namespace System.Diagnostics.CodeAnalysis
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  [ExcludeFromCodeCoverage]
  [DebuggerNonUserCode]
  internal sealed class MemberNotNullAttribute : Attribute
  {
    public string[] Members { get; }

    public MemberNotNullAttribute(string member) => this.Members = new string[1]
    {
      member
    };

    public MemberNotNullAttribute(params string[] members) => this.Members = members;
  }
}
