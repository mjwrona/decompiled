// Decompiled with JetBrains decompiler
// Type: Microsoft.OData.ODataError
// Assembly: Microsoft.TeamFoundation.OData.Core, Version=7.6.3.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 6619C7F6-E44A-4143-AE77-6D570F968D9A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.OData.Core.dll

using Microsoft.OData.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;

namespace Microsoft.OData
{
  [DebuggerDisplay("{ErrorCode}: {Message}")]
  public sealed class ODataError : ODataAnnotatable
  {
    public string ErrorCode { get; set; }

    public string Message { get; set; }

    public string Target { get; set; }

    public ICollection<ODataErrorDetail> Details { get; set; }

    public ODataInnerError InnerError { get; set; }

    [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "We want to allow the same instance annotation collection instance to be shared across ODataLib OM instances.")]
    public ICollection<ODataInstanceAnnotation> InstanceAnnotations
    {
      get => this.GetInstanceAnnotations();
      set => this.SetInstanceAnnotations(value);
    }

    public override string ToString() => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{{\"error\":{{\"code\":\"{0}\",\"message\":\"{1}\",\"target\":\"{2}\",\"details\":{3},\"innererror\":{4} }}}}", this.ErrorCode == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.ErrorCode), this.Message == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.Message), this.Target == null ? (object) "" : (object) JsonValueUtils.GetEscapedJsonString(this.Target), this.Details == null ? (object) "{}" : (object) this.GetJsonStringForDetails(), this.InnerError == null ? (object) "{}" : (object) this.InnerError.ToJson());

    private string GetJsonStringForDetails() => "[" + string.Join(",", this.Details.Select<ODataErrorDetail, string>((Func<ODataErrorDetail, string>) (i => i.ToJson())).ToArray<string>()) + "]";
  }
}
