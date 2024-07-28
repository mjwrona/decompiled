// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.InvalidFilterException
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal class InvalidFilterException : Exception
  {
    private string filterParam;

    public InvalidFilterException()
    {
    }

    public InvalidFilterException(string message)
      : base(message)
    {
    }

    public InvalidFilterException(string message, string filterParam)
      : base(message)
    {
      this.filterParam = filterParam;
    }

    public InvalidFilterException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public override string Message => string.Format("{0} FilterParam={1}", (object) base.Message, (object) (this.filterParam ?? string.Empty));
  }
}
