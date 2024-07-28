// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.JsonLight.ReorderingJsonReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData.JsonLight
{
  internal sealed class ReorderingJsonReader : BufferingJsonReader
  {
    internal ReorderingJsonReader(IJsonReader innerReader, int maxInnerErrorDepth)
      : base(innerReader, "error", maxInnerErrorDepth)
    {
    }

    public override Stream CreateReadStream()
    {
      Stream readStream;
      try
      {
        readStream = (Stream) new MemoryStream(this.Value == null ? new byte[0] : Convert.FromBase64String((string) this.Value));
      }
      catch (FormatException ex)
      {
        throw new ODataException(Strings.JsonReader_InvalidBinaryFormat(this.Value));
      }
      this.Read();
      return readStream;
    }

    public override TextReader CreateTextReader()
    {
      if (this.NodeType == JsonNodeType.Property)
        throw new ODataException("Reading JSON Streams not supported for beta.");
      TextReader textReader = (TextReader) new StringReader(this.Value == null ? "" : (string) this.Value);
      this.Read();
      return textReader;
    }

    public override bool CanStream() => this.Value is string || this.Value == null;

    protected override void ProcessObjectValue()
    {
      Stack<ReorderingJsonReader.BufferedObject> bufferedObjectStack = new Stack<ReorderingJsonReader.BufferedObject>();
      while (true)
      {
        string annotationName;
        do
        {
          switch (this.currentBufferedNode.NodeType)
          {
            case JsonNodeType.StartObject:
              ReorderingJsonReader.BufferedObject bufferedObject1 = new ReorderingJsonReader.BufferedObject()
              {
                ObjectStart = this.currentBufferedNode
              };
              bufferedObjectStack.Push(bufferedObject1);
              base.ProcessObjectValue();
              this.currentBufferedNode = bufferedObject1.ObjectStart;
              this.ReadInternal();
              continue;
            case JsonNodeType.EndObject:
              ReorderingJsonReader.BufferedObject bufferedObject2 = bufferedObjectStack.Pop();
              if (bufferedObject2.CurrentProperty != null)
                bufferedObject2.CurrentProperty.EndOfPropertyValueNode = this.currentBufferedNode.Previous;
              bufferedObject2.Reorder();
              if (bufferedObjectStack.Count == 0)
                return;
              this.ReadInternal();
              continue;
            case JsonNodeType.Property:
              ReorderingJsonReader.BufferedObject bufferedObject3 = bufferedObjectStack.Peek();
              if (bufferedObject3.CurrentProperty != null)
                bufferedObject3.CurrentProperty.EndOfPropertyValueNode = this.currentBufferedNode.Previous;
              ReorderingJsonReader.BufferedProperty bufferedProperty = new ReorderingJsonReader.BufferedProperty();
              bufferedProperty.PropertyNameNode = this.currentBufferedNode;
              string propertyName;
              this.ReadPropertyName(out propertyName, out annotationName);
              bufferedProperty.PropertyAnnotationName = annotationName;
              bufferedObject3.AddBufferedProperty(propertyName, annotationName, bufferedProperty);
              continue;
            default:
              goto label_12;
          }
        }
        while (annotationName == null);
        this.BufferValue();
        continue;
label_12:
        this.ReadInternal();
      }
    }

    private void ReadPropertyName(out string propertyName, out string annotationName)
    {
      string propertyName1 = this.GetPropertyName();
      this.ReadInternal();
      if (propertyName1.StartsWith("@", StringComparison.Ordinal))
      {
        propertyName = (string) null;
        annotationName = propertyName1.Substring(1);
        if (annotationName.IndexOf('.') != -1)
          return;
        annotationName = "odata." + annotationName;
      }
      else
      {
        int length = propertyName1.IndexOf('@');
        if (length > 0)
        {
          propertyName = propertyName1.Substring(0, length);
          annotationName = propertyName1.Substring(length + 1);
        }
        else if (propertyName1.IndexOf('.') < 0)
        {
          propertyName = propertyName1;
          annotationName = (string) null;
        }
        else
        {
          if (!ODataJsonLightUtils.IsMetadataReferenceProperty(propertyName1))
            throw new ODataException(Strings.JsonReaderExtensions_UnexpectedInstanceAnnotationName((object) propertyName1));
          propertyName = (string) null;
          annotationName = propertyName1;
        }
      }
    }

    private void BufferValue()
    {
      int num = 0;
      do
      {
        switch (this.NodeType)
        {
          case JsonNodeType.StartObject:
          case JsonNodeType.StartArray:
            ++num;
            break;
          case JsonNodeType.EndObject:
          case JsonNodeType.EndArray:
            --num;
            break;
        }
        this.ReadInternal();
      }
      while (num > 0);
    }

    private sealed class BufferedObject
    {
      private readonly Dictionary<string, object> propertyCache;
      private readonly HashSet<string> dataProperties;
      private readonly List<KeyValuePair<string, string>> propertyNamesWithAnnotations;

      internal BufferedObject()
      {
        this.propertyNamesWithAnnotations = new List<KeyValuePair<string, string>>();
        this.dataProperties = new HashSet<string>((IEqualityComparer<string>) StringComparer.Ordinal);
        this.propertyCache = new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.Ordinal);
      }

      internal BufferingJsonReader.BufferedNode ObjectStart { get; set; }

      internal ReorderingJsonReader.BufferedProperty CurrentProperty { get; private set; }

      internal void AddBufferedProperty(
        string propertyName,
        string annotationName,
        ReorderingJsonReader.BufferedProperty bufferedProperty)
      {
        this.CurrentProperty = bufferedProperty;
        string key = propertyName ?? annotationName;
        if (propertyName == null)
          this.propertyNamesWithAnnotations.Add(new KeyValuePair<string, string>(annotationName, (string) null));
        else if (!this.dataProperties.Contains(propertyName))
        {
          if (annotationName == null)
            this.dataProperties.Add(propertyName);
          this.propertyNamesWithAnnotations.Add(new KeyValuePair<string, string>(propertyName, annotationName));
        }
        object obj;
        if (this.propertyCache.TryGetValue(key, out obj))
        {
          List<ReorderingJsonReader.BufferedProperty> bufferedPropertyList;
          if (obj is ReorderingJsonReader.BufferedProperty bufferedProperty1)
          {
            bufferedPropertyList = new List<ReorderingJsonReader.BufferedProperty>(4);
            bufferedPropertyList.Add(bufferedProperty1);
            this.propertyCache[key] = (object) bufferedPropertyList;
          }
          else
            bufferedPropertyList = (List<ReorderingJsonReader.BufferedProperty>) obj;
          bufferedPropertyList.Add(bufferedProperty);
        }
        else
          this.propertyCache.Add(key, (object) bufferedProperty);
      }

      internal void Reorder()
      {
        BufferingJsonReader.BufferedNode node = this.ObjectStart;
        foreach (string sortPropertyName in this.SortPropertyNames())
        {
          object bufferedProperties = this.propertyCache[sortPropertyName];
          if (bufferedProperties is ReorderingJsonReader.BufferedProperty bufferedProperty1)
          {
            bufferedProperty1.InsertAfter(node);
            node = bufferedProperty1.EndOfPropertyValueNode;
          }
          else
          {
            foreach (ReorderingJsonReader.BufferedProperty bufferedProperty in ReorderingJsonReader.BufferedObject.SortBufferedProperties((IList<ReorderingJsonReader.BufferedProperty>) bufferedProperties))
            {
              bufferedProperty.InsertAfter(node);
              node = bufferedProperty.EndOfPropertyValueNode;
            }
          }
        }
      }

      private static IEnumerable<ReorderingJsonReader.BufferedProperty> SortBufferedProperties(
        IList<ReorderingJsonReader.BufferedProperty> bufferedProperties)
      {
        List<ReorderingJsonReader.BufferedProperty> delayedProperties = (List<ReorderingJsonReader.BufferedProperty>) null;
        int i;
        for (i = 0; i < bufferedProperties.Count; ++i)
        {
          ReorderingJsonReader.BufferedProperty bufferedProperty = bufferedProperties[i];
          string propertyAnnotationName = bufferedProperty.PropertyAnnotationName;
          if (propertyAnnotationName == null || !ReorderingJsonReader.BufferedObject.IsODataInstanceAnnotation(propertyAnnotationName))
          {
            if (delayedProperties == null)
              delayedProperties = new List<ReorderingJsonReader.BufferedProperty>();
            delayedProperties.Add(bufferedProperty);
          }
          else
            yield return bufferedProperty;
        }
        if (delayedProperties != null)
        {
          for (i = 0; i < delayedProperties.Count; ++i)
            yield return delayedProperties[i];
        }
      }

      private static bool IsODataInstanceAnnotation(string annotationName) => annotationName.StartsWith("odata.", StringComparison.Ordinal);

      private static bool IsODataContextAnnotation(string annotationName) => string.CompareOrdinal("odata.context", annotationName) == 0;

      private static bool IsODataRemovedAnnotation(string annotationName) => string.CompareOrdinal("odata.removed", annotationName) == 0;

      private static bool IsODataTypeAnnotation(string annotationName) => string.CompareOrdinal("odata.type", annotationName) == 0;

      private static bool IsODataIdAnnotation(string annotationName) => string.CompareOrdinal("odata.id", annotationName) == 0;

      private static bool IsODataETagAnnotation(string annotationName) => string.CompareOrdinal("odata.etag", annotationName) == 0;

      private IEnumerable<string> SortPropertyNames()
      {
        string str1 = (string) null;
        string removedAnnotationName = (string) null;
        string typeAnnotationName = (string) null;
        string idAnnotationName = (string) null;
        string etagAnnotationName = (string) null;
        List<string> odataAnnotationNames = (List<string>) null;
        List<string> otherNames = (List<string>) null;
        foreach (KeyValuePair<string, string> namesWithAnnotation in this.propertyNamesWithAnnotations)
        {
          string key = namesWithAnnotation.Key;
          if (namesWithAnnotation.Value == null || !this.dataProperties.Contains(key))
          {
            this.dataProperties.Add(key);
            if (ReorderingJsonReader.BufferedObject.IsODataContextAnnotation(key))
              str1 = key;
            else if (ReorderingJsonReader.BufferedObject.IsODataRemovedAnnotation(key))
              removedAnnotationName = key;
            else if (ReorderingJsonReader.BufferedObject.IsODataTypeAnnotation(key))
              typeAnnotationName = key;
            else if (ReorderingJsonReader.BufferedObject.IsODataIdAnnotation(key))
              idAnnotationName = key;
            else if (ReorderingJsonReader.BufferedObject.IsODataETagAnnotation(key))
              etagAnnotationName = key;
            else if (ReorderingJsonReader.BufferedObject.IsODataInstanceAnnotation(key))
            {
              if (odataAnnotationNames == null)
                odataAnnotationNames = new List<string>();
              odataAnnotationNames.Add(key);
            }
            else
            {
              if (otherNames == null)
                otherNames = new List<string>();
              otherNames.Add(key);
            }
          }
        }
        if (str1 != null)
          yield return str1;
        if (removedAnnotationName != null)
          yield return removedAnnotationName;
        if (typeAnnotationName != null)
          yield return typeAnnotationName;
        if (idAnnotationName != null)
          yield return idAnnotationName;
        if (etagAnnotationName != null)
          yield return etagAnnotationName;
        if (odataAnnotationNames != null)
        {
          foreach (string str2 in odataAnnotationNames)
            yield return str2;
        }
        if (otherNames != null)
        {
          foreach (string str3 in otherNames)
            yield return str3;
        }
      }
    }

    private sealed class BufferedProperty
    {
      internal string PropertyAnnotationName { get; set; }

      internal BufferingJsonReader.BufferedNode PropertyNameNode { get; set; }

      internal BufferingJsonReader.BufferedNode EndOfPropertyValueNode { get; set; }

      internal void InsertAfter(BufferingJsonReader.BufferedNode node)
      {
        BufferingJsonReader.BufferedNode previous = this.PropertyNameNode.Previous;
        BufferingJsonReader.BufferedNode next1 = this.EndOfPropertyValueNode.Next;
        previous.Next = next1;
        next1.Previous = previous;
        BufferingJsonReader.BufferedNode next2 = node.Next;
        node.Next = this.PropertyNameNode;
        this.PropertyNameNode.Previous = node;
        this.EndOfPropertyValueNode.Next = next2;
        if (next2 == null)
          return;
        next2.Previous = this.EndOfPropertyValueNode;
      }
    }
  }
}
