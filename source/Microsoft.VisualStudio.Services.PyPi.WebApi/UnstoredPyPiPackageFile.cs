// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.PyPi.WebApi.Types.UnstoredPyPiPackageFile
// Assembly: Microsoft.VisualStudio.Services.PyPi.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17E1C323-94FE-4FF1-903A-ED51BA3159D2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.PyPi.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.PyPi.WebApi.Types
{
  [DataContract]
  public class UnstoredPyPiPackageFile
  {
    [DataMember]
    public string PyPiDistType { get; }

    [DataMember]
    public string Path { get; }

    [DataMember]
    public IReadOnlyCollection<KeyValuePair<string, string>> HashTypeAndValueCollection { get; }

    [DataMember]
    public long Size { get; }

    [DataMember]
    public DateTime DateAdded { get; }

    public UnstoredPyPiPackageFile(
      string pyPiDistType,
      string path,
      List<KeyValuePair<string, string>> hashTypeAndValueCollection,
      long size,
      DateTime dateAdded)
    {
      this.PyPiDistType = pyPiDistType;
      this.Path = path;
      this.HashTypeAndValueCollection = (IReadOnlyCollection<KeyValuePair<string, string>>) hashTypeAndValueCollection;
      this.Size = size;
      this.DateAdded = dateAdded;
    }
  }
}
