// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Ssh.Server.External.Eldos.EldosSshKeyWrapper
// Assembly: Microsoft.TeamFoundation.Ssh.Server.External.Eldos, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 76A7154E-5D66-408C-AA1C-E130B17CCD4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Ssh.Server.External.Eldos.dll

using SBSSHKeyStorage;

namespace Microsoft.TeamFoundation.Ssh.Server.External.Eldos
{
  public class EldosSshKeyWrapper : IEldosSshKey
  {
    private readonly TElSSHKey m_eldosSshKey;

    internal EldosSshKeyWrapper(TElSSHKey eldosSshKey) => this.m_eldosSshKey = eldosSshKey;

    public int SavePublicKeyToBlob(
      ref string algorithmName,
      byte[] buffer,
      int startIndex,
      ref int count)
    {
      return this.m_eldosSshKey.SavePublicKeyToBlob(ref algorithmName, buffer, startIndex, ref count);
    }

    public static IEldosSshKey Create() => (IEldosSshKey) new EldosSshKeyWrapper(new TElSSHKey());
  }
}
