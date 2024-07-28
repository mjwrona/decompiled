// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types.SessionRequest
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 92A332A6-CB54-4593-8984-3FC6CEE8C30D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.dll

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.WebApi.Types
{
  [DataContract]
  public class SessionRequest
  {
    [DataMember(EmitDefaultValue = false)]
    [MaxLength(50)]
    public string Source = "Custom";

    [DataMember(EmitDefaultValue = false)]
    public IDictionary<string, string> Data { get; set; }

    [DataMember(IsRequired = true)]
    public string Feed { get; set; }
  }
}
