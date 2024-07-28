// Decompiled with JetBrains decompiler
// Type: System.Collections.Generic.IAsyncEnumerator`1
// Assembly: Microsoft.Bcl.AsyncInterfaces, Version=7.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// MVID: 8B2E828D-BD93-4580-BC63-F76024589A76
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Bcl.AsyncInterfaces.dll

using System.Threading.Tasks;


#nullable enable
namespace System.Collections.Generic
{
  public interface IAsyncEnumerator<out T> : IAsyncDisposable
  {
    ValueTask<bool> MoveNextAsync();

    T Current { get; }
  }
}
