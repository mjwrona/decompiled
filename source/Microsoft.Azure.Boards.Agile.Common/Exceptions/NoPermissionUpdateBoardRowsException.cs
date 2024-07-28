// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.Agile.Common.Exceptions.NoPermissionUpdateBoardRowsException
// Assembly: Microsoft.Azure.Boards.Agile.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BB24C713-7980-4A88-9344-6289BC767DB3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.Agile.Common.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.Azure.Boards.Agile.Common.Exceptions
{
  [Serializable]
  public class NoPermissionUpdateBoardRowsException : VssServiceException
  {
    public NoPermissionUpdateBoardRowsException(string message)
      : base(message)
    {
    }
  }
}
