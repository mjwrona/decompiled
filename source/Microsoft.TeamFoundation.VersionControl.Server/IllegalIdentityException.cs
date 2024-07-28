// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.VersionControl.Server.IllegalIdentityException
// Assembly: Microsoft.TeamFoundation.VersionControl.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C5D9EE74-8805-4E00-959F-39760D2358B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.VersionControl.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;

namespace Microsoft.TeamFoundation.VersionControl.Server
{
  [Serializable]
  public class IllegalIdentityException : ServerException
  {
    internal string m_name;

    public IllegalIdentityException(string name)
      : base(TFCommonResources.IllegalIdentityException((object) IllegalIdentityException.TruncateIdentityName(name)))
    {
      this.m_name = name;
    }

    private static string TruncateIdentityName(string name)
    {
      if (name.Length <= 256)
        return name;
      return Resources.Format("TruncatedIdentityName", (object) name.Substring(0, 256));
    }

    public override void SetFailureInfo(Failure failure)
    {
      base.SetFailureInfo(failure);
      failure.IdentityName = this.m_name;
    }
  }
}
