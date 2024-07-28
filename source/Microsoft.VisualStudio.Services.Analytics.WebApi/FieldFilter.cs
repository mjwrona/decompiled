// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.WebApi.FieldFilter
// Assembly: Microsoft.VisualStudio.Services.Analytics.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F858B048-FFE8-4E5F-8EBC-9B25DAC23DF8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Analytics.WebApi
{
  [DataContract]
  public class FieldFilter
  {
    [DataMember]
    public string FieldName { get; set; }

    [DataMember]
    public FieldOperation FieldOperation { get; set; }

    [DataMember]
    public FilterValues<string> Value { get; set; }

    [DataMember]
    [Obsolete("Use Values instead")]
    public IList<string> FieldValue { get; set; }

    [DataMember(IsRequired = false)]
    [Obsolete("Use Values instead")]
    public FieldValuesOperator FieldValuesOperator { get; set; }

    private void PopulateFieldValuePropertyFromLegacyProperties()
    {
      if (this.FieldValue == null)
        return;
      this.Value = new FilterValues<string>();
      this.Value.Values = this.FieldValue;
      this.Value.Operator = this.FieldValuesOperator;
      this.FieldValue = (IList<string>) null;
    }

    [System.Runtime.Serialization.OnDeserialized]
    internal void OnDeserialized(StreamingContext context) => this.PopulateFieldValuePropertyFromLegacyProperties();
  }
}
