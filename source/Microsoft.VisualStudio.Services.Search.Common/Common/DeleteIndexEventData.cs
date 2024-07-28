// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.DeleteIndexEventData
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Search.Common.Enums;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  [DataContract]
  [Export(typeof (ChangeEventData))]
  public class DeleteIndexEventData : ChangeEventData
  {
    internal DeleteIndexEventData()
    {
    }

    public DeleteIndexEventData(ExecutionContext executionContext)
      : base(executionContext)
    {
    }

    [DataMember]
    public string IndexName { get; set; }

    [DataMember]
    public string ESConnectionString { get; set; }

    [DataMember]
    public IEnumerable<DocumentContractType> DocumentContractTypes { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckStringForNullOrEmpty(this.IndexName, "IndexName");
      ArgumentUtility.CheckStringForNullOrEmpty(this.ESConnectionString, "ESConnectionString");
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) this.DocumentContractTypes, "DocumentContractTypes");
    }
  }
}
