// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.SearchPlatformException
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [Serializable]
  public class SearchPlatformException : SearchServiceException
  {
    public IEnumerable<SearchPlatformException.ErrorItem> ItemsWithErrors { get; }

    public SearchPlatformException()
    {
    }

    public SearchPlatformException(string message)
      : this(message, SearchServiceErrorCode.Unknown)
    {
    }

    public SearchPlatformException(
      string message,
      SearchServiceErrorCode errorCode,
      IEnumerable<SearchPlatformException.ErrorItem> itemsWithErrors = null)
      : this(message, (Exception) null, errorCode)
    {
      this.ItemsWithErrors = itemsWithErrors;
    }

    public SearchPlatformException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public SearchPlatformException(
      string message,
      Exception innerException,
      SearchServiceErrorCode errorCode)
      : base(message, innerException, errorCode)
    {
    }

    protected SearchPlatformException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.ItemsWithErrors = (IEnumerable<SearchPlatformException.ErrorItem>) info.GetValue(nameof (ItemsWithErrors), typeof (IEnumerable<SearchPlatformException.ErrorItem>));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("ItemsWithErrors", (object) this.ItemsWithErrors, typeof (IEnumerable<SearchPlatformException.ErrorItem>));
    }

    public class ErrorItem
    {
      public string Index { get; }

      public string Type { get; }

      public string Id { get; }

      public long Version { get; }

      public int Status { get; }

      public ErrorItem(string index, string type, string id, long version, int status)
      {
        this.Index = index;
        this.Type = type;
        this.Id = id;
        this.Version = version;
        this.Status = status;
      }
    }
  }
}
