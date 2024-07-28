// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.FormInput.InputValuesQuery
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.FormInput
{
  [DataContract]
  public class InputValuesQuery
  {
    [DataMember]
    public object Resource { get; set; }

    [DataMember]
    public IList<Microsoft.VisualStudio.Services.FormInput.InputValues> InputValues { get; set; }

    [DataMember]
    public IDictionary<string, string> CurrentValues { get; set; }
  }
}
