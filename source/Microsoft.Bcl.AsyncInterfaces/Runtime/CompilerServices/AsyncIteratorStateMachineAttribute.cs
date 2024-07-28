// Decompiled with JetBrains decompiler
// Type: System.Runtime.CompilerServices.AsyncIteratorStateMachineAttribute
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll


#nullable enable
namespace System.Runtime.CompilerServices
{
  [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
  public sealed class AsyncIteratorStateMachineAttribute : StateMachineAttribute
  {
    public AsyncIteratorStateMachineAttribute(Type stateMachineType)
      : base(stateMachineType)
    {
    }
  }
}
