// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Common.ConcurrentIteratorInvalidCursorException
// Assembly: Microsoft.VisualStudio.Services.Content.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DC45E7D4-4445-41B3-8FA2-C13CD848D0F1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Content.Common
{
  [Serializable]
  public class ConcurrentIteratorInvalidCursorException : Exception
  {
    public ConcurrentIteratorInvalidCursorException()
    {
    }

    public ConcurrentIteratorInvalidCursorException(string message)
      : base(message)
    {
    }

    public ConcurrentIteratorInvalidCursorException(string message, Exception inner)
      : base(message, inner)
    {
    }

    protected ConcurrentIteratorInvalidCursorException(
      SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }
  }
}
