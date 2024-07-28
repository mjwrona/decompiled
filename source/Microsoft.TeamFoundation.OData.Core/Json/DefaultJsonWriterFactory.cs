// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.Json.DefaultJsonWriterFactory
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.IO;

namespace Microsoft.OData.Json
{
  public sealed class DefaultJsonWriterFactory : IJsonWriterFactory
  {
    private ODataStringEscapeOption stringEscapeOption;

    public DefaultJsonWriterFactory()
      : this(ODataStringEscapeOption.EscapeNonAscii)
    {
    }

    public DefaultJsonWriterFactory(ODataStringEscapeOption stringEscapeOption) => this.stringEscapeOption = stringEscapeOption;

    [CLSCompliant(false)]
    public IJsonWriter CreateJsonWriter(TextWriter textWriter, bool isIeee754Compatible) => (IJsonWriter) new JsonWriter(textWriter, isIeee754Compatible, this.stringEscapeOption);
  }
}
