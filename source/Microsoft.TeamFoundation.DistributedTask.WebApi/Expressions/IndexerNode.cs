// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Expressions.IndexerNode
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Expressions
{
  internal sealed class IndexerNode : ContainerNode
  {
    private static Lazy<JsonSerializer> s_serializer = new Lazy<JsonSerializer>((Func<JsonSerializer>) (() => JsonUtility.CreateJsonSerializer()));
    private static Lazy<MethodInfo> s_tryGetValueTemplate = new Lazy<MethodInfo>((Func<MethodInfo>) (() => typeof (IndexerNode).GetTypeInfo().GetMethod("TryGetValue", BindingFlags.Static | BindingFlags.NonPublic)));

    internal IndexerNode() => this.Name = "indexer";

    protected override sealed bool TraceFullyRealized => true;

    internal override sealed string ConvertToExpression() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}[{1}]", (object) this.Parameters[0].ConvertToExpression(), (object) this.Parameters[1].ConvertToExpression());

    internal override sealed string ConvertToRealizedExpression(EvaluationContext context)
    {
      string str;
      return context.TryGetTraceResult((ExpressionNode) this, out str) ? str : this.ConvertToExpression();
    }

    protected override sealed object EvaluateCore(
      EvaluationContext context,
      out ResultMemory resultMemory)
    {
      EvaluationResult evaluationResult1 = this.Parameters[0].Evaluate(context);
      if (context.Options.UseCollectionInterfaces)
      {
        object collection;
        if (!evaluationResult1.TryGetCollectionInterface(out collection))
        {
          if (this.Parameters.Count > 2)
          {
            resultMemory = (ResultMemory) null;
            return (object) new IndexerNode.FilteredArray();
          }
          resultMemory = (ResultMemory) null;
          return (object) null;
        }
        switch (collection)
        {
          case IndexerNode.FilteredArray filteredArray:
            return this.HandleFilteredArray(context, filteredArray, out resultMemory);
          case IReadOnlyObject readOnlyObject:
            return this.HandleObject(context, readOnlyObject, this.Parameters[0].Name, out resultMemory);
          case IReadOnlyArray array:
            return this.HandleArray(context, array, out resultMemory);
          default:
            resultMemory = (ResultMemory) null;
            return (object) null;
        }
      }
      else
      {
        object core = (object) null;
        if (evaluationResult1.Kind == ValueKind.Array && evaluationResult1.Value is JArray)
        {
          JArray jarray = evaluationResult1.Value as JArray;
          EvaluationResult evaluationResult2 = this.Parameters[1].Evaluate(context);
          if (evaluationResult2.Kind == ValueKind.Number)
          {
            Decimal num = (Decimal) evaluationResult2.Value;
            if (num >= 0M && num < (Decimal) jarray.Count && num == Math.Floor(num))
              core = (object) jarray[(int) num];
          }
          else
          {
            Decimal result;
            if (evaluationResult2.Kind == ValueKind.String && !string.IsNullOrEmpty(evaluationResult2.Value as string) && evaluationResult2.TryConvertToNumber(context, out result) && result >= 0M && result < (Decimal) jarray.Count && result == Math.Floor(result))
              core = (object) jarray[(int) result];
          }
        }
        else if (evaluationResult1.Kind == ValueKind.Object)
        {
          if (evaluationResult1.Value is JObject)
          {
            JObject jobject = evaluationResult1.Value as JObject;
            string result;
            if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result))
              core = (object) jobject[result];
          }
          else if (evaluationResult1.Value is IDictionary<string, string>)
          {
            IDictionary<string, string> dictionary = evaluationResult1.Value as IDictionary<string, string>;
            string result;
            if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result))
            {
              string str;
              core = dictionary.TryGetValue(result, out str) ? (object) str : (object) null;
            }
          }
          else if (evaluationResult1.Value is IDictionary<string, object>)
          {
            IDictionary<string, object> dictionary = evaluationResult1.Value as IDictionary<string, object>;
            string result;
            if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result) && !dictionary.TryGetValue(result, out core))
              core = (object) null;
          }
          else if (evaluationResult1.Value is IReadOnlyDictionary<string, string>)
          {
            IReadOnlyDictionary<string, string> readOnlyDictionary = evaluationResult1.Value as IReadOnlyDictionary<string, string>;
            string result;
            if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result))
            {
              string str;
              core = readOnlyDictionary.TryGetValue(result, out str) ? (object) str : (object) null;
            }
          }
          else if (evaluationResult1.Value is IReadOnlyDictionary<string, object>)
          {
            IReadOnlyDictionary<string, object> readOnlyDictionary = evaluationResult1.Value as IReadOnlyDictionary<string, object>;
            string result;
            if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result) && !readOnlyDictionary.TryGetValue(result, out core))
              core = (object) null;
          }
          else
          {
            switch (IndexerNode.s_serializer.Value.ContractResolver.ResolveContract(evaluationResult1.Value.GetType()))
            {
              case JsonObjectContract jsonObjectContract:
                string result1;
                if (this.Parameters[1].Evaluate(context).TryConvertToString(context, out result1))
                {
                  JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty(result1);
                  if (closestMatchProperty != null)
                  {
                    core = jsonObjectContract.Properties[closestMatchProperty.PropertyName].ValueProvider.GetValue(evaluationResult1.Value);
                    break;
                  }
                  break;
                }
                break;
              case JsonDictionaryContract dictionaryContract:
                string result2;
                if (dictionaryContract.DictionaryKeyType == typeof (string) && this.Parameters[1].Evaluate(context).TryConvertToString(context, out result2))
                {
                  MethodInfo methodInfo = IndexerNode.s_tryGetValueTemplate.Value.MakeGenericMethod(dictionaryContract.DictionaryValueType);
                  resultMemory = (ResultMemory) null;
                  object[] parameters = new object[2]
                  {
                    evaluationResult1.Value,
                    (object) result2
                  };
                  return methodInfo.Invoke((object) null, parameters);
                }
                break;
            }
          }
        }
        resultMemory = (ResultMemory) null;
        return core;
      }
    }

    private object HandleFilteredArray(
      EvaluationContext context,
      IndexerNode.FilteredArray filteredArray,
      out ResultMemory resultMemory)
    {
      IndexerNode.IndexHelper indexHelper = new IndexerNode.IndexHelper(this.Parameters[1].Evaluate(context), context);
      bool flag;
      if (this.Parameters.Count > 2)
      {
        flag = true;
        if (!string.Equals(indexHelper.StringIndex, '*'.ToString(), StringComparison.Ordinal))
          throw new InvalidOperationException("Unexpected filter '" + indexHelper.StringIndex + "'");
      }
      else
        flag = false;
      IndexerNode.FilteredArray filteredArray1 = new IndexerNode.FilteredArray();
      MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) this, new int?(context.Options.MaxMemory));
      foreach (object filtered in filteredArray)
      {
        object collection;
        if (EvaluationResult.CreateIntermediateResult(context, filtered, out ResultMemory _).TryGetCollectionInterface(out collection))
        {
          if (collection is IReadOnlyObject readOnlyObject)
          {
            if (flag)
            {
              foreach (object o in readOnlyObject.Values)
              {
                filteredArray1.Add(o);
                memoryCounter.Add(IntPtr.Size);
              }
            }
            else
            {
              object o;
              if (indexHelper.HasStringIndex && readOnlyObject.TryGetValue(indexHelper.StringIndex, out o))
              {
                filteredArray1.Add(o);
                memoryCounter.Add(IntPtr.Size);
              }
            }
          }
          else if (collection is IReadOnlyArray readOnlyArray)
          {
            if (flag)
            {
              foreach (object o in (IEnumerable<object>) readOnlyArray)
              {
                filteredArray1.Add(o);
                memoryCounter.Add(IntPtr.Size);
              }
            }
            else if (indexHelper.HasIntegerIndex && indexHelper.IntegerIndex < readOnlyArray.Count)
            {
              filteredArray1.Add(readOnlyArray[indexHelper.IntegerIndex]);
              memoryCounter.Add(IntPtr.Size);
            }
          }
        }
      }
      resultMemory = new ResultMemory()
      {
        Bytes = new int?(memoryCounter.CurrentBytes)
      };
      return (object) filteredArray1;
    }

    private object HandleObject(
      EvaluationContext context,
      IReadOnlyObject obj,
      string objName,
      out ResultMemory resultMemory)
    {
      IndexerNode.IndexHelper indexHelper = new IndexerNode.IndexHelper(this.Parameters[1].Evaluate(context), context);
      if (indexHelper.HasStringIndex)
      {
        if (this.Parameters.Count > 2)
        {
          IndexerNode.FilteredArray filteredArray = new IndexerNode.FilteredArray();
          MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) this, new int?(context.Options.MaxMemory));
          foreach (object o in obj.Values)
          {
            filteredArray.Add(o);
            memoryCounter.Add(IntPtr.Size);
          }
          resultMemory = new ResultMemory()
          {
            Bytes = new int?(memoryCounter.CurrentBytes)
          };
          return (object) filteredArray;
        }
        if (this.IsStrictlyIndexed(context, objName))
        {
          resultMemory = (ResultMemory) null;
          return obj[indexHelper.StringIndex];
        }
        object obj1;
        if (obj.TryGetValue(indexHelper.StringIndex, out obj1))
        {
          resultMemory = (ResultMemory) null;
          return obj1;
        }
      }
      resultMemory = (ResultMemory) null;
      return (object) null;
    }

    private bool IsStrictlyIndexed(EvaluationContext context, string objName)
    {
      ICollection<string> strictlyIndexedObjects = context.Options.StrictlyIndexedObjects;
      return strictlyIndexedObjects != null && strictlyIndexedObjects.Any<string>((Func<string, bool>) (s => string.Equals(s, objName, StringComparison.OrdinalIgnoreCase)));
    }

    private object HandleArray(
      EvaluationContext context,
      IReadOnlyArray array,
      out ResultMemory resultMemory)
    {
      IndexerNode.IndexHelper indexHelper = new IndexerNode.IndexHelper(this.Parameters[1].Evaluate(context), context);
      if (this.Parameters.Count > 2)
      {
        IndexerNode.FilteredArray filteredArray = new IndexerNode.FilteredArray();
        MemoryCounter memoryCounter = new MemoryCounter((ExpressionNode) this, new int?(context.Options.MaxMemory));
        foreach (object o in (IEnumerable<object>) array)
        {
          filteredArray.Add(o);
          memoryCounter.Add(IntPtr.Size);
        }
        resultMemory = new ResultMemory()
        {
          Bytes = new int?(memoryCounter.CurrentBytes)
        };
        return (object) filteredArray;
      }
      if (indexHelper.HasIntegerIndex && indexHelper.IntegerIndex < array.Count)
      {
        resultMemory = (ResultMemory) null;
        return array[indexHelper.IntegerIndex];
      }
      resultMemory = (ResultMemory) null;
      return (object) null;
    }

    private static object TryGetValue<TValue>(IDictionary<string, TValue> dictionary, string key)
    {
      TValue obj;
      return !dictionary.TryGetValue(key, out obj) ? (object) null : (object) obj;
    }

    private class FilteredArray : 
      IReadOnlyArray,
      IReadOnlyList<object>,
      IReadOnlyCollection<object>,
      IEnumerable<object>,
      IEnumerable
    {
      private readonly IList<object> m_list;

      public FilteredArray() => this.m_list = (IList<object>) new List<object>();

      public void Add(object o) => this.m_list.Add(o);

      public int Count => this.m_list.Count;

      public object this[int index] => this.m_list[index];

      public IEnumerator<object> GetEnumerator() => this.m_list.GetEnumerator();

      IEnumerator IEnumerable.GetEnumerator() => (IEnumerator) this.m_list.GetEnumerator();
    }

    private class IndexHelper
    {
      private Lazy<Tuple<bool, int>> m_integerIndex;
      private Lazy<Tuple<bool, string>> m_stringIndex;
      private readonly EvaluationResult m_result;
      private readonly EvaluationContext m_context;

      public bool HasIntegerIndex => this.m_integerIndex.Value.Item1;

      public int IntegerIndex => this.m_integerIndex.Value.Item2;

      public bool HasStringIndex => this.m_stringIndex.Value.Item1;

      public string StringIndex => this.m_stringIndex.Value.Item2;

      public IndexHelper(EvaluationResult result, EvaluationContext context)
      {
        this.m_result = result;
        this.m_context = context;
        Decimal result1;
        this.m_integerIndex = new Lazy<Tuple<bool, int>>((Func<Tuple<bool, int>>) (() => this.m_result.TryConvertToNumber(this.m_context, out result1) && result1 >= 0M ? new Tuple<bool, int>(true, (int) Math.Floor(result1)) : new Tuple<bool, int>(false, 0)));
        string result2;
        this.m_stringIndex = new Lazy<Tuple<bool, string>>((Func<Tuple<bool, string>>) (() => this.m_result.TryConvertToString(this.m_context, out result2) ? new Tuple<bool, string>(true, result2) : new Tuple<bool, string>(false, (string) null)));
      }
    }
  }
}
