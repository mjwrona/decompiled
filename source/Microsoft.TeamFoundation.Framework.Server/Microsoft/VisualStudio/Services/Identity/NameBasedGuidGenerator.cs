// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.NameBasedGuidGenerator
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Identity
{
  internal static class NameBasedGuidGenerator
  {
    private static readonly byte[] VstsAadNamespaceBytes = new Guid("{1203A0A8-803F-4C35-932C-E62EC5F79811}").ToByteArray();
    private static readonly byte[] VstsMsaNamespaceBytes = new Guid("{0F157860-B5E0-4F4A-B8CA-BE7F61DA6D6A}").ToByteArray();
    private static readonly byte[] CspPartnerNamespaceBytes = new Guid("{C3278185-D00D-480D-AF58-0A21127C1ADE}").ToByteArray();

    public static Guid NewAadGuid(byte[] nameBytes, NameBasedGuidVersion nameBasedGuidVersion) => GuidUtils.GetNewGuidInternal(NameBasedGuidGenerator.VstsAadNamespaceBytes, nameBytes, nameBasedGuidVersion);

    public static Guid NewMsaGuid(byte[] nameBytes, NameBasedGuidVersion nameBasedGuidVersion) => GuidUtils.GetNewGuidInternal(NameBasedGuidGenerator.VstsMsaNamespaceBytes, nameBytes, nameBasedGuidVersion);

    public static Guid NewCspPartnerGuid(
      byte[] nameBytes,
      NameBasedGuidVersion nameBasedGuidVersion)
    {
      return GuidUtils.GetNewGuidInternal(NameBasedGuidGenerator.CspPartnerNamespaceBytes, nameBytes, nameBasedGuidVersion);
    }
  }
}
