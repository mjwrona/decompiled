// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Documents.CorrelationManager
// Assembly: Microsoft.Azure.DocumentDB.Core, Version=2.10.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 20BB0EB7-1465-494C-8F4E-F898448B85D9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DocumentDB.Core.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.Azure.Documents
{
  internal class CorrelationManager
  {
    private readonly AsyncLocal<Guid> _activityId = new AsyncLocal<Guid>();
    private readonly AsyncLocal<CorrelationManager.StackNode> _stack = new AsyncLocal<CorrelationManager.StackNode>();
    private readonly Stack _stackWrapper;

    internal CorrelationManager() => this._stackWrapper = (Stack) new CorrelationManager.AsyncLocalStackWrapper(this._stack);

    public Stack LogicalOperationStack => this._stackWrapper;

    public void StartLogicalOperation() => this.StartLogicalOperation((object) Guid.NewGuid());

    public void StopLogicalOperation() => this._stackWrapper.Pop();

    public Guid ActivityId
    {
      get => this._activityId.Value;
      set => this._activityId.Value = value;
    }

    public void StartLogicalOperation(object operationId)
    {
      if (operationId == null)
        throw new ArgumentNullException(nameof (operationId));
      this._stackWrapper.Push(operationId);
    }

    private sealed class StackNode
    {
      internal StackNode(object value, CorrelationManager.StackNode prev = null)
      {
        this.Value = value;
        this.Prev = prev;
        this.Count = prev != null ? prev.Count + 1 : 1;
      }

      internal int Count { get; }

      internal object Value { get; }

      internal CorrelationManager.StackNode Prev { get; }
    }

    private sealed class AsyncLocalStackWrapper : Stack
    {
      private readonly AsyncLocal<CorrelationManager.StackNode> _stack;

      internal AsyncLocalStackWrapper(AsyncLocal<CorrelationManager.StackNode> stack) => this._stack = stack;

      public override void Clear() => this._stack.Value = (CorrelationManager.StackNode) null;

      public override object Clone() => (object) new CorrelationManager.AsyncLocalStackWrapper(this._stack);

      public override int Count
      {
        get
        {
          CorrelationManager.StackNode stackNode = this._stack.Value;
          return stackNode == null ? 0 : stackNode.Count;
        }
      }

      public override IEnumerator GetEnumerator() => this.GetEnumerator(this._stack.Value);

      public override object Peek() => this._stack.Value?.Value;

      public override bool Contains(object obj)
      {
        for (CorrelationManager.StackNode prev = this._stack.Value; prev != null; prev = prev.Prev)
        {
          if (obj == null)
          {
            if (prev.Value == null)
              return true;
          }
          else if (obj.Equals(prev.Value))
            return true;
        }
        return false;
      }

      public override void CopyTo(Array array, int index)
      {
        for (CorrelationManager.StackNode prev = this._stack.Value; prev != null; prev = prev.Prev)
          array.SetValue(prev.Value, index++);
      }

      private IEnumerator GetEnumerator(CorrelationManager.StackNode n)
      {
        for (; n != null; n = n.Prev)
          yield return n.Value;
      }

      public override object Pop()
      {
        CorrelationManager.StackNode stackNode = this._stack.Value;
        if (stackNode == null)
          base.Pop();
        this._stack.Value = stackNode.Prev;
        return stackNode.Value;
      }

      public override void Push(object obj) => this._stack.Value = new CorrelationManager.StackNode(obj, this._stack.Value);

      public override object[] ToArray()
      {
        CorrelationManager.StackNode prev = this._stack.Value;
        if (prev == null)
          return Array.Empty<object>();
        List<object> objectList = new List<object>();
        do
        {
          objectList.Add(prev.Value);
          prev = prev.Prev;
        }
        while (prev != null);
        return objectList.ToArray();
      }
    }
  }
}
