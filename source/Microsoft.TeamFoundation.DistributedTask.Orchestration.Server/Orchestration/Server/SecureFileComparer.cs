// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.SecureFileComparer
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  internal class SecureFileComparer : IEqualityComparer<SecureFile>
  {
    public bool Equals(SecureFile secureFile1, SecureFile secureFile2)
    {
      bool flag = false;
      if (secureFile1 == null && secureFile2 == null)
        flag = true;
      if (secureFile1 != null && secureFile2 != null & secureFile1.Id == secureFile2.Id)
        flag = true;
      return flag;
    }

    public int GetHashCode(SecureFile secureFile)
    {
      Guid guid = Guid.Empty;
      if (secureFile != null)
        guid = secureFile.Id;
      return guid.GetHashCode();
    }
  }
}
