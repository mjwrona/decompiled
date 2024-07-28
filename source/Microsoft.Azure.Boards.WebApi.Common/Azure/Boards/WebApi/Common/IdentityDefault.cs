// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.IdentityDefault
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using System;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WebApi.Common
{
  [DataContract]
  public class IdentityDefault : Default
  {
    [DataMember(Name = "Vsid")]
    public Guid Vsid { get; set; }
  }
}
