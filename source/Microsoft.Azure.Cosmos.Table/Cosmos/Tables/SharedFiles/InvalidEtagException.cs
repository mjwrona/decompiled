// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Tables.SharedFiles.InvalidEtagException
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using System;

namespace Microsoft.Azure.Cosmos.Tables.SharedFiles
{
  internal class InvalidEtagException : Exception
  {
    private string etag;

    public InvalidEtagException()
    {
    }

    public InvalidEtagException(string message)
      : base(message)
    {
    }

    public InvalidEtagException(string message, string etag)
      : base(message)
    {
      this.etag = etag;
    }

    public override string Message => base.Message + " Etag=" + (this.etag ?? string.Empty);
  }
}
