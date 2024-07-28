// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Client.AccountManagement.AccountStorageException
// Assembly: Microsoft.VisualStudio.Services.Client.Interactive, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 00B1FD41-439C-4B93-A417-9D1E4874E657
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Client.Interactive.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Client.AccountManagement
{
  [ExceptionMapping("0.0", "3.0", "AccountStorageException", "Microsoft.VisualStudio.Services.Client.AccountManagement.AccountStorageException, Microsoft.VisualStudio.Services.Client, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
  [Serializable]
  public class AccountStorageException : Exception
  {
    public AccountStorageException()
    {
    }

    public AccountStorageException(string message)
      : base(message)
    {
    }

    public AccountStorageException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    protected AccountStorageException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
