// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.UriParser.ODataUnrecognizedPathException
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.OData.UriParser
{
  [DebuggerDisplay("{Message}")]
  public sealed class ODataUnrecognizedPathException : ODataException
  {
    public ODataUnrecognizedPathException()
      : this(Strings.ODataUriParserException_GeneralError, (Exception) null)
    {
    }

    public ODataUnrecognizedPathException(string message)
      : this(message, (Exception) null)
    {
    }

    public ODataUnrecognizedPathException(string message, Exception innerException)
      : base(message, innerException)
    {
    }

    public IEnumerable<ODataPathSegment> ParsedSegments { get; set; }

    public string CurrentSegment { get; set; }

    public IEnumerable<string> UnparsedSegments { get; set; }
  }
}
