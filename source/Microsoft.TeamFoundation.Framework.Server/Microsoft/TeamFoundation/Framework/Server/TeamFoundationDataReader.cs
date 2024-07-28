// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.TeamFoundationDataReader
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class TeamFoundationDataReader : IDisposable, IEnumerable
  {
    private IEnumerable<IDisposable> m_disposableObjects;
    private List<object> m_objectCollection;
    private int m_currentObjectIndex;
    private bool? m_disposed;
    private readonly StackTracer m_constructorStackTrace;

    public TeamFoundationDataReader(params object[] dataValues)
      : this((IDisposable) null, dataValues)
    {
    }

    public TeamFoundationDataReader(IDisposable disposableObject, params object[] dataValues)
    {
      IDisposable[] disposableObjects;
      if (disposableObject == null)
        disposableObjects = (IDisposable[]) null;
      else
        disposableObjects = new IDisposable[1]
        {
          disposableObject
        };
      // ISSUE: explicit constructor call
      this.\u002Ector((IEnumerable<IDisposable>) disposableObjects, dataValues);
    }

    public TeamFoundationDataReader(
      IEnumerable<IDisposable> disposableObjects,
      params object[] dataValues)
    {
      this.m_disposableObjects = disposableObjects;
      this.m_objectCollection = new List<object>((IEnumerable<object>) dataValues);
      if (TeamFoundationTracingService.IsRawTracingEnabled(1149504683, TraceLevel.Info, "Streaming", nameof (TeamFoundationDataReader), (string[]) null))
        this.m_constructorStackTrace = new StackTracer();
      this.m_disposed = new bool?(false);
    }

    ~TeamFoundationDataReader()
    {
      bool? disposed = this.m_disposed;
      bool flag = false;
      if (!(disposed.GetValueOrDefault() == flag & disposed.HasValue))
        return;
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(2047463046, TraceLevel.Error, "Streaming", nameof (TeamFoundationDataReader), "TeamFoundationDataReader has not been disposed before finalization - Call stack {0}", (object) this.m_constructorStackTrace);
      else
        TeamFoundationTracingService.TraceRaw(2047463046, TraceLevel.Error, "Streaming", nameof (TeamFoundationDataReader), "TeamFoundationDataReader has not been disposed before finalization");
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    public void Dispose(bool disposing)
    {
      bool? disposed = this.m_disposed;
      bool flag = false;
      if (!(disposed.GetValueOrDefault() == flag & disposed.HasValue))
        return;
      if (disposing)
      {
        if (this.m_objectCollection != null)
        {
          foreach (object obj in this.m_objectCollection)
          {
            if (obj is IDisposable disposable)
              disposable.Dispose();
          }
          this.m_objectCollection = (List<object>) null;
        }
        if (this.m_disposableObjects != null)
        {
          foreach (IDisposable disposableObject in this.m_disposableObjects)
            disposableObject?.Dispose();
          this.m_disposableObjects = (IEnumerable<IDisposable>) null;
        }
      }
      this.m_disposed = new bool?(true);
    }

    public bool TryMoveNext()
    {
      if (this.m_currentObjectIndex >= this.m_objectCollection.Count - 1)
        return false;
      ++this.m_currentObjectIndex;
      return true;
    }

    public void MoveNext()
    {
      if (!this.TryMoveNext())
        throw new InvalidOperationException();
    }

    public T Current<T>()
    {
      if (this.m_currentObjectIndex >= this.m_objectCollection.Count)
        throw new InvalidOperationException();
      return (T) this.m_objectCollection[this.m_currentObjectIndex];
    }

    public IEnumerable<T> CurrentEnumerable<T>() => this.Current<IEnumerable<T>>();

    public IEnumerator GetEnumerator()
    {
      IEnumerable enumerable = this.Current<IEnumerable>();
      return enumerable != null ? enumerable.GetEnumerator() : (IEnumerator) Enumerable.Empty<object>().GetEnumerator();
    }
  }
}
