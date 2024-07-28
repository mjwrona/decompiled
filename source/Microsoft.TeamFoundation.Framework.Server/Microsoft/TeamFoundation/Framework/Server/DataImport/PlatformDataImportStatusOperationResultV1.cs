// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.DataImport.PlatformDataImportStatusOperationResultV1
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Framework.Server.DataImport
{
  [DataContract]
  public class PlatformDataImportStatusOperationResultV1
  {
    [DataMember]
    public string StatusPageTitle { get; set; }

    [DataMember]
    public string StatusPageSubtitle { get; set; }

    [DataMember]
    public DateTime StatusPageLastUpdated { get; set; }

    [DataMember]
    public List<DataImportFileTransferProgress> FileTransferProgress { get; set; }

    [DataMember]
    public List<DataImportTableTransferProgress> TableTransferProgress { get; set; }
  }
}
