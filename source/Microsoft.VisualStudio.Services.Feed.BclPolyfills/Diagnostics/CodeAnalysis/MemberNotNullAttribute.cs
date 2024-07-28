// Decompiled with JetBrains decompiler
// Type: System.Diagnostics.CodeAnalysis.MemberNotNullAttribute
// Assembly: Microsoft.VisualStudio.Services.Feed.BclPolyfills, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 22B1AF8C-B841-48B3-9E2E-229EAA8AFCE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.BclPolyfills.dll


#nullable enable
namespace System.Diagnostics.CodeAnalysis
{
  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, AllowMultiple = true, Inherited = false)]
  public sealed class MemberNotNullAttribute : Attribute
  {
    public MemberNotNullAttribute(params string[] members)
    {
      this.Members = members;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }

    public string[] Members { get; }

    public MemberNotNullAttribute(string member)
      : this(new string[1]{ member })
    {
    }
  }
}
