// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd.ServerNodeHasher
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.VisualStudio.Services.Cloud.SmartRouter.Common;
using Microsoft.VisualStudio.Services.Common;
using System.Security.Cryptography;
using System.Text;


#nullable enable
namespace Microsoft.VisualStudio.Services.Cloud.SmartRouter.BackEnd
{
  internal class ServerNodeHasher : SmartRouterBase, IServerNodeHasher
  {
    private const string c_hashSalt = "4df32948-bc19-49bc-82f4-33ac9cdbe615";

    public ServerNodeHasher()
      : base(SmartRouterBase.TraceLayer.BackEnd)
    {
    }

    public ServerNodeWithHash CreateWithHash(ServerNode server)
    {
      server = server.CheckArgumentIsNotNull<ServerNode>(nameof (server));
      using (HashAlgorithm hashAlgorithm = ServerNodeHasher.CreateHashAlgorithm())
      {
        string hashTextWithSalt = ServerNodeHasher.GetHashTextWithSalt(server);
        byte[] hash = ServerNodeHasher.ComputeHash(hashAlgorithm, hashTextWithSalt);
        string hex = ServerNodeHasher.BytesToHex(hash);
        return new ServerNodeWithHash(server, hash, hex);
      }
    }

    private static HashAlgorithm CreateHashAlgorithm() => (HashAlgorithm) SHA256.Create();

    private static string GetHashTextWithSalt(ServerNode server)
    {
      StringBuilder stringBuilder = new StringBuilder("4df32948-bc19-49bc-82f4-33ac9cdbe615".Length + server.RoleInstance.Length + server.IPAddress.Length + 2);
      stringBuilder.Append("4df32948-bc19-49bc-82f4-33ac9cdbe615");
      stringBuilder.Append('|');
      stringBuilder.Append(server.RoleInstance);
      stringBuilder.Append('|');
      stringBuilder.Append(server.IPAddress);
      return stringBuilder.ToString();
    }

    private static byte[] ComputeHash(HashAlgorithm hash, string hashText)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(hashText);
      return hash.ComputeHash(bytes);
    }

    private static string BytesToHex(byte[] bytes) => HexConverter.ToStringLowerCase(bytes);
  }
}
