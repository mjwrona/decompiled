// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.CodeAnalysis.MemberNotNullWhenAttribute
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

namespace System.Diagnostics.CodeAnalysis
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  internal sealed class MemberNotNullWhenAttribute : Attribute
  {
    public MemberNotNullWhenAttribute(bool returnValue, string member)
    {
      this.ReturnValue = returnValue;
      this.Members = new string[1]{ member };
    }

    public MemberNotNullWhenAttribute(bool returnValue, params string[] members)
    {
      this.ReturnValue = returnValue;
      this.Members = members;
    }

    public bool ReturnValue { get; }

    public string[] Members { get; }
  }
}
