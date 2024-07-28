// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataErrorDetail
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using System;
using System.Globalization;

namespace Microsoft.OData
{
  public sealed class ODataErrorDetail
  {
    public string ErrorCode { get; set; }

    public string Message { get; set; }

    public string Target { get; set; }

    internal string ToJson() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{ \"errorcode\": \"{0}\", \"message\": \"{1}\", \"target\": \"{2}\" }}", new object[3]
    {
      (object) (this.ErrorCode ?? ""),
      (object) (this.Message ?? ""),
      (object) (this.Target ?? "")
    });
  }
}
