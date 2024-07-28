// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DisposableCollection`1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class DisposableCollection<T> : 
    IDisposableReadOnlyList<T>,
    IReadOnlyList<T>,
    IReadOnlyCollection<T>,
    IEnumerable<T>,
    IEnumerable,
    IDisposable
  {
    private readonly StackTrace m_constructorStackTrace;
    private IReadOnlyList<T> m_elements;
    private IDisposable[] m_disposableObjects;
    private static readonly string s_errorMessage = "DisposableCollection<" + typeof (T).ToString() + "> was not disposed!";

    public DisposableCollection(IReadOnlyList<T> elements)
      : this(elements, (IDisposable[]) null)
    {
    }

    public DisposableCollection(IReadOnlyList<T> elements, params IDisposable[] disposableObjects)
    {
      this.m_elements = elements;
      this.m_disposableObjects = disposableObjects;
      if (!TeamFoundationTracingService.IsRawTracingEnabled(261053250, TraceLevel.Info, "PluginLoader", nameof (DisposableCollection<T>), (string[]) null))
        return;
      this.m_constructorStackTrace = new StackTrace(false);
    }

    ~DisposableCollection()
    {
      int num = 0;
      if (this.m_elements != null)
      {
        for (int index = 0; index < this.m_elements.Count; ++index)
        {
          if ((object) this.m_elements[index] is IDisposable)
            ++num;
        }
      }
      if (this.m_disposableObjects != null)
      {
        for (int index = 0; index < this.m_disposableObjects.Length; ++index)
        {
          if (this.m_disposableObjects[index] != null)
            ++num;
        }
      }
      if (this.m_constructorStackTrace != null)
        TeamFoundationTracingService.TraceRaw(261053253, TraceLevel.Error, "PluginLoader", nameof (DisposableCollection<T>), "DisposableCollection was not disposed! - call stack: {0}, number of disposable objects: {1}", (object) this.m_constructorStackTrace, (object) num);
      else
        TeamFoundationTracingService.TraceRaw(261053255, TraceLevel.Error, "PluginLoader", nameof (DisposableCollection<T>), DisposableCollection<T>.s_errorMessage + string.Format(", number of disposable objects: {0}", (object) num));
    }

    public IEnumerator<T> GetEnumerator() => this.m_elements.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    public void Dispose()
    {
      if (this.m_disposableObjects != null)
      {
        for (int index = 0; index < this.m_disposableObjects.Length; ++index)
          this.m_disposableObjects[index]?.Dispose();
        this.m_disposableObjects = (IDisposable[]) null;
      }
      if (this.m_elements != null)
      {
        foreach (T element in (IEnumerable<T>) this.m_elements)
        {
          if (element is IDisposable disposable)
            disposable.Dispose();
        }
        this.m_elements = (IReadOnlyList<T>) null;
      }
      GC.SuppressFinalize((object) this);
    }

    public T this[int index] => this.m_elements[index];

    public int Count => this.m_elements.Count;
  }
}
