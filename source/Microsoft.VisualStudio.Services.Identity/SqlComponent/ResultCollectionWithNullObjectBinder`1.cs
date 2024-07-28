// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.SqlComponent.ResultCollectionWithNullObjectBinder`1
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Data;

namespace Microsoft.VisualStudio.Services.Identity.SqlComponent
{
  public class ResultCollectionWithNullObjectBinder<T> : ResultCollection
  {
    private bool NullObjectBinder = true;

    public override bool TryNextResult()
    {
      if (this.m_binders.Count == 0)
        return false;
      if (this.NullObjectBinder && this.m_binders[this.m_currentBinderIndex] is Microsoft.VisualStudio.Services.Identity.SqlComponent.NullObjectBinder<T>)
      {
        ++this.m_currentBinderIndex;
        return true;
      }
      this.NullObjectBinder = false;
      if (this.m_binders.Count <= this.m_currentBinderIndex + 1 || !(this.m_binders[this.m_currentBinderIndex + 1] is Microsoft.VisualStudio.Services.Identity.SqlComponent.NullObjectBinder<T>))
        return base.TryNextResult();
      ++this.m_currentBinderIndex;
      return true;
    }

    public ResultCollectionWithNullObjectBinder(
      IDataReader reader,
      string storedProcedureName,
      IVssRequestContext requestContext)
      : base(reader, storedProcedureName, requestContext)
    {
    }

    public ResultCollectionWithNullObjectBinder(
      IDataReader reader,
      int maximumRows,
      string storedProcedureName,
      SqlExceptionHandler exceptionHandler,
      IVssRequestContext requestContext)
      : base(reader, maximumRows, storedProcedureName, exceptionHandler, requestContext)
    {
    }
  }
}
