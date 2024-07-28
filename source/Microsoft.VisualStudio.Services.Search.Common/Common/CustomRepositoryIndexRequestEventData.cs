// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.CustomRepositoryIndexRequestEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class CustomRepositoryIndexRequestEventData : ChangeEventData
  {
    private CustomRepositoryIndexRequestEventData()
    {
    }

    public CustomRepositoryIndexRequestEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember(Order = 0)]
    public Guid RequestId { get; set; }

    [DataMember(Order = 1)]
    public IEnumerable<string> Branches { get; set; }
  }
}
