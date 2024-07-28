// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Common.CompletedOperation`1
// Assembly: Microsoft.TeamFoundation.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E643B42-FE11-4FC2-A9D6-79417E26CF92
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Framework.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public sealed class CompletedOperation<TResult> : AsyncOperation
  {
    private TResult m_result;

    public CompletedOperation(TResult result, AsyncCallback callback, object state)
      : base(callback, state)
    {
      this.m_result = result;
      this.Complete(true);
    }

    public static TResult End(IAsyncResult result) => AsyncOperation.End<CompletedOperation<TResult>>(result).m_result;
  }
}
