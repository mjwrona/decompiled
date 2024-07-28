// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.WitResourceNotFoundException
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [Serializable]
  public class WitResourceNotFoundException : TeamFoundationServiceException
  {
    public WitResourceNotFoundException(string message)
      : base(message)
    {
    }

    public WitResourceNotFoundException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
