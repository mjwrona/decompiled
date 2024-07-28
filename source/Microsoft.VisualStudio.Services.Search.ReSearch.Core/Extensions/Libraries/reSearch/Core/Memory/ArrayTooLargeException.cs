// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory.ArrayTooLargeException
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Core.Memory
{
  [Serializable]
  public class ArrayTooLargeException : Exception
  {
    public ArrayTooLargeException()
    {
    }

    public ArrayTooLargeException(string message)
      : base(message)
    {
    }

    public ArrayTooLargeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected ArrayTooLargeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
