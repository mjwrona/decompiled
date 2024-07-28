// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.IRemoteRepository
// Assembly: Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AEA81E2B-AB47-44C0-8043-66C0E1018997
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server.dll

using System.IO;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.DevSecOps.Sdk.Server
{
  public interface IRemoteRepository
  {
    Task<Stream> GetRemoteFileStream(string path = null, string branch = null, bool throwErrorIfNotFound = false);
  }
}
