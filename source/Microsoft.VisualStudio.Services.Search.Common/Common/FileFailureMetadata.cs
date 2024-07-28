// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.FileFailureMetadata
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract(Namespace = "")]
  public class FileFailureMetadata : FailureMetadata
  {
    [DataMember(Order = 0)]
    public Branches Branches { get; set; }

    [DataMember(Order = 1)]
    public string FilePath { get; set; }

    public override string ToString() => FormattableString.Invariant(FormattableStringFactory.Create("Branches: {0}", (object) this.Branches));
  }
}
