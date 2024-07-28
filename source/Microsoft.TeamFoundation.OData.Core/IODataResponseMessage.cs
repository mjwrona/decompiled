// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.IODataResponseMessage
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Microsoft.OData
{
  public interface IODataResponseMessage
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Users will never have to instantiate these; the rule does not apply.")]
    IEnumerable<KeyValuePair<string, string>> Headers { get; }

    int StatusCode { get; set; }

    string GetHeader(string headerName);

    void SetHeader(string headerName, string headerValue);

    [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "This is intentionally a method.")]
    Stream GetStream();
  }
}
