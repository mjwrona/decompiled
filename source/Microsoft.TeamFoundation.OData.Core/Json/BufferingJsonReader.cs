// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.BufferingJsonReader
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.JsonLight;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.OData.Json
{
  internal class BufferingJsonReader : IJsonStreamReader, IJsonReader
  {
    protected BufferingJsonReader.BufferedNode bufferedNodesHead;
    protected BufferingJsonReader.BufferedNode currentBufferedNode;
    private readonly IJsonReader innerReader;
    private readonly int maxInnerErrorDepth;
    private readonly string inStreamErrorPropertyName;
    private bool isBuffering;
    private bool removeOnNextRead;
    private bool parsingInStreamError;
    private bool disableInStreamErrorDetection;

    internal BufferingJsonReader(
      IJsonReader innerReader,
      string inStreamErrorPropertyName,
      int maxInnerErrorDepth)
    {
      this.innerReader = innerReader;
      this.inStreamErrorPropertyName = inStreamErrorPropertyName;
      this.maxInnerErrorDepth = maxInnerErrorDepth;
      this.bufferedNodesHead = (BufferingJsonReader.BufferedNode) null;
      this.currentBufferedNode = (BufferingJsonReader.BufferedNode) null;
    }

    public JsonNodeType NodeType
    {
      get
      {
        if (this.bufferedNodesHead == null)
          return this.innerReader.NodeType;
        return this.isBuffering ? this.currentBufferedNode.NodeType : this.bufferedNodesHead.NodeType;
      }
    }

    public object Value
    {
      get
      {
        if (this.bufferedNodesHead == null)
          return this.innerReader.Value;
        return this.isBuffering ? this.currentBufferedNode.Value : this.bufferedNodesHead.Value;
      }
    }

    public bool IsIeee754Compatible => this.innerReader.IsIeee754Compatible;

    internal bool DisableInStreamErrorDetection
    {
      get => this.disableInStreamErrorDetection;
      set => this.disableInStreamErrorDetection = value;
    }

    internal bool IsBuffering => this.isBuffering;

    public virtual Stream CreateReadStream()
    {
      IJsonStreamReader innerReader = this.innerReader as IJsonStreamReader;
      if (!this.isBuffering && innerReader != null)
        return innerReader.CreateReadStream();
      Stream readStream = (Stream) new MemoryStream(this.Value == null ? new byte[0] : Convert.FromBase64String((string) this.Value));
      this.innerReader.Read();
      return readStream;
    }

    public virtual TextReader CreateTextReader()
    {
      IJsonStreamReader innerReader = this.innerReader as IJsonStreamReader;
      if (!this.isBuffering && innerReader != null)
        return innerReader.CreateTextReader();
      TextReader textReader = (TextReader) new StringReader(this.Value == null ? "" : (string) this.Value);
      this.innerReader.Read();
      return textReader;
    }

    public virtual bool CanStream()
    {
      IJsonStreamReader innerReader = this.innerReader as IJsonStreamReader;
      if (!this.isBuffering && innerReader != null)
        return innerReader.CanStream();
      return this.Value is string || this.Value == null || this.NodeType == JsonNodeType.StartArray || this.NodeType == JsonNodeType.StartObject;
    }

    public bool Read() => this.ReadInternal();

    internal void StartBuffering()
    {
      if (this.bufferedNodesHead == null)
        this.bufferedNodesHead = new BufferingJsonReader.BufferedNode(this.innerReader.NodeType, this.innerReader.Value);
      else
        this.removeOnNextRead = false;
      if (this.currentBufferedNode == null)
        this.currentBufferedNode = this.bufferedNodesHead;
      this.isBuffering = true;
    }

    internal object BookmarkCurrentPosition() => (object) this.currentBufferedNode;

    internal void MoveToBookmark(object bookmark) => this.currentBufferedNode = bookmark as BufferingJsonReader.BufferedNode;

    internal void StopBuffering()
    {
      this.isBuffering = false;
      this.removeOnNextRead = true;
      this.currentBufferedNode = (BufferingJsonReader.BufferedNode) null;
    }

    internal bool StartBufferingAndTryToReadInStreamErrorPropertyValue(out ODataError error)
    {
      error = (ODataError) null;
      this.StartBuffering();
      this.parsingInStreamError = true;
      try
      {
        return this.TryReadInStreamErrorPropertyValue(out error);
      }
      finally
      {
        this.StopBuffering();
        this.parsingInStreamError = false;
      }
    }

    protected bool ReadInternal()
    {
      if (this.removeOnNextRead)
      {
        this.RemoveFirstNodeInBuffer();
        this.removeOnNextRead = false;
      }
      bool flag;
      if (this.isBuffering)
      {
        if (this.currentBufferedNode.Next != this.bufferedNodesHead)
        {
          this.currentBufferedNode = this.currentBufferedNode.Next;
          flag = true;
        }
        else if (this.parsingInStreamError)
        {
          flag = this.innerReader.Read();
          BufferingJsonReader.BufferedNode bufferedNode = new BufferingJsonReader.BufferedNode(this.innerReader.NodeType, this.innerReader.Value);
          bufferedNode.Previous = this.bufferedNodesHead.Previous;
          bufferedNode.Next = this.bufferedNodesHead;
          this.bufferedNodesHead.Previous.Next = bufferedNode;
          this.bufferedNodesHead.Previous = bufferedNode;
          this.currentBufferedNode = bufferedNode;
        }
        else
          flag = this.ReadNextAndCheckForInStreamError();
      }
      else if (this.bufferedNodesHead == null)
      {
        flag = this.parsingInStreamError ? this.innerReader.Read() : this.ReadNextAndCheckForInStreamError();
      }
      else
      {
        flag = this.bufferedNodesHead.NodeType != JsonNodeType.EndOfInput;
        this.removeOnNextRead = true;
      }
      return flag;
    }

    protected virtual void ProcessObjectValue()
    {
      ODataError error = (ODataError) null;
      if (this.DisableInStreamErrorDetection)
        return;
      this.ReadInternal();
      bool flag = false;
      while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
      {
        if (string.CompareOrdinal(this.inStreamErrorPropertyName, (string) this.currentBufferedNode.Value) != 0 | flag)
          return;
        flag = true;
        this.ReadInternal();
        if (!this.TryReadInStreamErrorPropertyValue(out error))
          return;
      }
      if (flag)
        throw new ODataErrorException(error);
    }

    private bool ReadNextAndCheckForInStreamError()
    {
      this.parsingInStreamError = true;
      try
      {
        bool flag = this.ReadInternal();
        if (this.innerReader.NodeType == JsonNodeType.StartObject)
        {
          bool isBuffering = this.isBuffering;
          BufferingJsonReader.BufferedNode bufferedNode = (BufferingJsonReader.BufferedNode) null;
          if (this.isBuffering)
            bufferedNode = this.currentBufferedNode;
          else
            this.StartBuffering();
          this.ProcessObjectValue();
          if (isBuffering)
            this.currentBufferedNode = bufferedNode;
          else
            this.StopBuffering();
        }
        return flag;
      }
      finally
      {
        this.parsingInStreamError = false;
      }
    }

    private bool TryReadInStreamErrorPropertyValue(out ODataError error)
    {
      error = (ODataError) null;
      if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
        return false;
      this.ReadInternal();
      error = new ODataError();
      ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField = ODataJsonLightReaderUtils.ErrorPropertyBitMask.None;
      while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
      {
        switch ((string) this.currentBufferedNode.Value)
        {
          case "code":
            string stringValue1;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Code) || !this.TryReadErrorStringPropertyValue(out stringValue1))
              return false;
            error.ErrorCode = stringValue1;
            break;
          case "message":
            string stringValue2;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Message) || !this.TryReadErrorStringPropertyValue(out stringValue2))
              return false;
            error.Message = stringValue2;
            break;
          case "target":
            string stringValue3;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Target) || !this.TryReadErrorStringPropertyValue(out stringValue3))
              return false;
            error.Target = stringValue3;
            break;
          case "details":
            ICollection<ODataErrorDetail> details;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Details) || !this.TryReadErrorDetailsPropertyValue(out details))
              return false;
            error.Details = details;
            break;
          case "innererror":
            ODataInnerError innerError;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.InnerError) || !this.TryReadInnerErrorPropertyValue(out innerError, 0))
              return false;
            error.InnerError = innerError;
            break;
          default:
            return false;
        }
        this.ReadInternal();
      }
      this.ReadInternal();
      return propertiesFoundBitField != 0;
    }

    private bool TryReadErrorDetailsPropertyValue(out ICollection<ODataErrorDetail> details)
    {
      this.ReadInternal();
      if (this.currentBufferedNode.NodeType != JsonNodeType.StartArray)
      {
        details = (ICollection<ODataErrorDetail>) null;
        return false;
      }
      this.ReadInternal();
      details = (ICollection<ODataErrorDetail>) new List<ODataErrorDetail>();
      ODataErrorDetail detail;
      if (this.TryReadErrorDetail(out detail))
        details.Add(detail);
      this.ReadInternal();
      return true;
    }

    private bool TryReadErrorDetail(out ODataErrorDetail detail)
    {
      if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
      {
        detail = (ODataErrorDetail) null;
        return false;
      }
      this.ReadInternal();
      detail = new ODataErrorDetail();
      ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField = ODataJsonLightReaderUtils.ErrorPropertyBitMask.None;
      while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
      {
        switch ((string) this.currentBufferedNode.Value)
        {
          case "code":
            string stringValue1;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Code) || !this.TryReadErrorStringPropertyValue(out stringValue1))
              return false;
            detail.ErrorCode = stringValue1;
            break;
          case "target":
            string stringValue2;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.Target) || !this.TryReadErrorStringPropertyValue(out stringValue2))
              return false;
            detail.Target = stringValue2;
            break;
          case "message":
            string stringValue3;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.MessageValue) || !this.TryReadErrorStringPropertyValue(out stringValue3))
              return false;
            detail.Message = stringValue3;
            break;
          default:
            this.SkipValueInternal();
            break;
        }
        this.ReadInternal();
      }
      return true;
    }

    private bool TryReadInnerErrorPropertyValue(out ODataInnerError innerError, int recursionDepth)
    {
      ValidationUtils.IncreaseAndValidateRecursionDepth(ref recursionDepth, this.maxInnerErrorDepth);
      this.ReadInternal();
      if (this.currentBufferedNode.NodeType != JsonNodeType.StartObject)
      {
        innerError = (ODataInnerError) null;
        return false;
      }
      this.ReadInternal();
      innerError = new ODataInnerError();
      ODataJsonLightReaderUtils.ErrorPropertyBitMask propertiesFoundBitField = ODataJsonLightReaderUtils.ErrorPropertyBitMask.None;
      while (this.currentBufferedNode.NodeType == JsonNodeType.Property)
      {
        switch ((string) this.currentBufferedNode.Value)
        {
          case "message":
            string stringValue1;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.MessageValue) || !this.TryReadErrorStringPropertyValue(out stringValue1))
              return false;
            innerError.Message = stringValue1;
            break;
          case "type":
            string stringValue2;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.TypeName) || !this.TryReadErrorStringPropertyValue(out stringValue2))
              return false;
            innerError.TypeName = stringValue2;
            break;
          case "stacktrace":
            string stringValue3;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.StackTrace) || !this.TryReadErrorStringPropertyValue(out stringValue3))
              return false;
            innerError.StackTrace = stringValue3;
            break;
          case "internalexception":
            ODataInnerError innerError1;
            if (!ODataJsonLightReaderUtils.ErrorPropertyNotFound(ref propertiesFoundBitField, ODataJsonLightReaderUtils.ErrorPropertyBitMask.InnerError) || !this.TryReadInnerErrorPropertyValue(out innerError1, recursionDepth))
              return false;
            innerError.InnerError = innerError1;
            break;
          default:
            this.SkipValueInternal();
            break;
        }
        this.ReadInternal();
      }
      return true;
    }

    private bool TryReadErrorStringPropertyValue(out string stringValue)
    {
      this.ReadInternal();
      stringValue = this.currentBufferedNode.Value as string;
      if (this.currentBufferedNode.NodeType != JsonNodeType.PrimitiveValue)
        return false;
      return this.currentBufferedNode.Value == null || stringValue != null;
    }

    private void SkipValueInternal()
    {
      int num = 0;
      do
      {
        switch (this.currentBufferedNode.NodeType)
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

    private void RemoveFirstNodeInBuffer()
    {
      if (this.bufferedNodesHead.Next == this.bufferedNodesHead)
      {
        this.bufferedNodesHead = (BufferingJsonReader.BufferedNode) null;
      }
      else
      {
        this.bufferedNodesHead.Previous.Next = this.bufferedNodesHead.Next;
        this.bufferedNodesHead.Next.Previous = this.bufferedNodesHead.Previous;
        this.bufferedNodesHead = this.bufferedNodesHead.Next;
      }
    }

    protected internal sealed class BufferedNode
    {
      private readonly JsonNodeType nodeType;
      private readonly object nodeValue;

      internal BufferedNode(JsonNodeType nodeType, object value)
      {
        this.nodeType = nodeType;
        this.nodeValue = value;
        this.Previous = this;
        this.Next = this;
      }

      internal JsonNodeType NodeType => this.nodeType;

      internal object Value => this.nodeValue;

      internal BufferingJsonReader.BufferedNode Previous { get; set; }

      internal BufferingJsonReader.BufferedNode Next { get; set; }
    }
  }
}
