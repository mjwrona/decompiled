// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.ConvertToJsonNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class ConvertToJsonNode : FunctionNode
  {
    protected override sealed object EvaluateCore(EvaluationContext context)
    {
      StringBuilder writer1 = new StringBuilder();
      MemoryCounter memory1 = new MemoryCounter((ExpressionNode) this, new int?(context.Options.MaxMemory));
      EvaluationResult result = this.Parameters[0].Evaluate(context);
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors1 = new Stack<ConvertToJsonNode.ICollectionEnumerator>();
      do
      {
        object collection;
        KeyValuePair<EvaluationResult, EvaluationResult> current;
        if (result.TryGetCollectionInterface(out collection))
        {
          switch (collection)
          {
            case IReadOnlyArray array:
              if (array.Count > 0)
              {
                this.WriteArrayStart(writer1, memory1, ancestors1);
                ConvertToJsonNode.ArrayEnumerator arrayEnumerator = new ConvertToJsonNode.ArrayEnumerator(context, result, array);
                arrayEnumerator.MoveNext();
                ancestors1.Push((ConvertToJsonNode.ICollectionEnumerator) arrayEnumerator);
                result = arrayEnumerator.Current;
                continue;
              }
              this.WriteEmptyArray(writer1, memory1, ancestors1);
              break;
            case IReadOnlyObject readOnlyObject:
              if (readOnlyObject.Count > 0)
              {
                this.WriteMappingStart(writer1, memory1, ancestors1);
                ConvertToJsonNode.ObjectEnumerator objectEnumerator = new ConvertToJsonNode.ObjectEnumerator(context, result, readOnlyObject);
                objectEnumerator.MoveNext();
                ancestors1.Push((ConvertToJsonNode.ICollectionEnumerator) objectEnumerator);
                EvaluationContext context1 = context;
                StringBuilder writer2 = writer1;
                MemoryCounter memory2 = memory1;
                current = objectEnumerator.Current;
                EvaluationResult key = current.Key;
                Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors2 = ancestors1;
                this.WriteMappingKey(context1, writer2, memory2, key, ancestors2);
                current = objectEnumerator.Current;
                result = current.Value;
                continue;
              }
              this.WriteEmptyMapping(writer1, memory1, ancestors1);
              break;
            default:
              throw new NotSupportedException("Unexpected type '" + collection?.GetType().FullName + "'");
          }
        }
        else
          this.WriteValue(context, writer1, memory1, result, ancestors1);
        do
        {
          if (ancestors1.Count > 0)
          {
            ConvertToJsonNode.ICollectionEnumerator collectionEnumerator = ancestors1.Peek();
            switch (collectionEnumerator)
            {
              case ConvertToJsonNode.ArrayEnumerator arrayEnumerator:
                if (arrayEnumerator.MoveNext())
                {
                  result = arrayEnumerator.Current;
                  goto label_22;
                }
                else
                {
                  ancestors1.Pop();
                  result = arrayEnumerator.Array;
                  this.WriteArrayEnd(writer1, memory1, ancestors1);
                  break;
                }
              case ConvertToJsonNode.ObjectEnumerator objectEnumerator:
                if (objectEnumerator.MoveNext())
                {
                  EvaluationContext context2 = context;
                  StringBuilder writer3 = writer1;
                  MemoryCounter memory3 = memory1;
                  current = objectEnumerator.Current;
                  EvaluationResult key = current.Key;
                  Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors3 = ancestors1;
                  this.WriteMappingKey(context2, writer3, memory3, key, ancestors3);
                  current = objectEnumerator.Current;
                  result = current.Value;
                  goto label_22;
                }
                else
                {
                  ancestors1.Pop();
                  result = objectEnumerator.Object;
                  this.WriteMappingEnd(writer1, memory1, ancestors1);
                  break;
                }
              default:
                throw new NotSupportedException("Unexpected type '" + collectionEnumerator?.GetType().FullName + "'");
            }
          }
          else
            result = (EvaluationResult) null;
        }
        while (result != null);
label_22:;
      }
      while (result != null);
      return (object) writer1.ToString();
    }

    private void WriteArrayStart(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = this.PrefixValue("[", ancestors);
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteMappingStart(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = this.PrefixValue("{", ancestors);
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteArrayEnd(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = "\n" + new string(' ', ancestors.Count * 2) + "]";
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteMappingEnd(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = "\n" + new string(' ', ancestors.Count * 2) + "}";
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteEmptyArray(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = this.PrefixValue("[]", ancestors);
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteEmptyMapping(
      StringBuilder writer,
      MemoryCounter memory,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = this.PrefixValue("{}", ancestors);
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteMappingKey(
      EvaluationContext context,
      StringBuilder writer,
      MemoryCounter memory,
      EvaluationResult key,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str = this.PrefixValue(JsonUtility.ToString((object) key.ConvertToString(context)), ancestors, true);
      memory.Add(str);
      writer.Append(str);
    }

    private void WriteValue(
      EvaluationContext context,
      StringBuilder writer,
      MemoryCounter memory,
      EvaluationResult value,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors)
    {
      string str1;
      switch (value.Kind)
      {
        case ValueKind.Array:
        case ValueKind.Object:
          str1 = "{}";
          break;
        case ValueKind.Boolean:
          str1 = (bool) value.Value ? "true" : "false";
          break;
        case ValueKind.DateTime:
        case ValueKind.String:
        case ValueKind.Version:
          str1 = JsonUtility.ToString((object) value.ConvertToString(context));
          break;
        case ValueKind.Number:
          str1 = value.ConvertToString(context);
          break;
        default:
          str1 = "null";
          break;
      }
      string str2 = this.PrefixValue(str1, ancestors);
      memory.Add(str2);
      writer.Append(str2);
    }

    private string PrefixValue(
      string value,
      Stack<ConvertToJsonNode.ICollectionEnumerator> ancestors,
      bool isMappingKey = false)
    {
      int count = ancestors.Count;
      ConvertToJsonNode.ICollectionEnumerator collectionEnumerator = count > 0 ? ancestors.Peek() : (ConvertToJsonNode.ICollectionEnumerator) null;
      if (!isMappingKey && collectionEnumerator is ConvertToJsonNode.ObjectEnumerator)
        return ": " + value;
      return count > 0 ? (collectionEnumerator.IsFirst ? string.Empty : ",") + "\n" + new string(' ', count * 2) + value : value;
    }

    private interface ICollectionEnumerator : IEnumerator
    {
      bool IsFirst { get; }
    }

    private sealed class ArrayEnumerator : ConvertToJsonNode.ICollectionEnumerator, IEnumerator
    {
      private readonly EvaluationContext m_context;
      private readonly IEnumerator<object> m_enumerator;
      private readonly EvaluationResult m_result;
      private EvaluationResult m_current;
      private int m_index = -1;

      public ArrayEnumerator(
        EvaluationContext context,
        EvaluationResult result,
        IReadOnlyArray array)
      {
        this.m_context = context;
        this.m_result = result;
        this.m_enumerator = array.GetEnumerator();
      }

      public EvaluationResult Array => this.m_result;

      public EvaluationResult Current => this.m_current;

      object IEnumerator.Current => (object) this.m_current;

      public bool IsFirst => this.m_index == 0;

      public bool MoveNext()
      {
        if (this.m_enumerator.MoveNext())
        {
          this.m_current = EvaluationResult.CreateIntermediateResult(this.m_context, this.m_enumerator.Current, out ResultMemory _);
          ++this.m_index;
          return true;
        }
        this.m_current = (EvaluationResult) null;
        return false;
      }

      public void Reset() => throw new NotSupportedException(nameof (Reset));
    }

    private sealed class ObjectEnumerator : ConvertToJsonNode.ICollectionEnumerator, IEnumerator
    {
      private readonly EvaluationContext m_context;
      private readonly IEnumerator<KeyValuePair<string, object>> m_enumerator;
      private readonly EvaluationResult m_result;
      private KeyValuePair<EvaluationResult, EvaluationResult> m_current;
      private int m_index = -1;

      public ObjectEnumerator(
        EvaluationContext context,
        EvaluationResult result,
        IReadOnlyObject obj)
      {
        this.m_context = context;
        this.m_result = result;
        this.m_enumerator = obj.GetEnumerator();
      }

      public KeyValuePair<EvaluationResult, EvaluationResult> Current => this.m_current;

      object IEnumerator.Current => (object) this.m_current;

      public bool IsFirst => this.m_index == 0;

      public EvaluationResult Object => this.m_result;

      public bool MoveNext()
      {
        if (this.m_enumerator.MoveNext())
        {
          ResultMemory conversionResultMemory;
          this.m_current = new KeyValuePair<EvaluationResult, EvaluationResult>(EvaluationResult.CreateIntermediateResult(this.m_context, (object) this.m_enumerator.Current.Key, out conversionResultMemory), EvaluationResult.CreateIntermediateResult(this.m_context, this.m_enumerator.Current.Value, out conversionResultMemory));
          ++this.m_index;
          return true;
        }
        this.m_current = new KeyValuePair<EvaluationResult, EvaluationResult>();
        return false;
      }

      public void Reset() => throw new NotSupportedException(nameof (Reset));
    }
  }
}
