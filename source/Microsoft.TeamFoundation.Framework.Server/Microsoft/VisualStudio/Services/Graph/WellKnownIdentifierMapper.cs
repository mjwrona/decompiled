// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.WellKnownIdentifierMapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Graph
{
  public struct WellKnownIdentifierMapper
  {
    private const string c_prefix = "S-1-9-1551374245-";
    private const int c_maxDigitsPerPart = 10;
    private const int c_maxSidLength = 61;

    public WellKnownIdentifierMapper(Guid domainId) => this.DomainSid = WellKnownIdentifierMapper.GetDomainSid(domainId);

    public SubjectDescriptor MapFromWellKnownIdentifier(SubjectDescriptor descriptor)
    {
      if (descriptor.IsVstsGroupType())
      {
        string identifier = this.MapFromWellKnownIdentifier(descriptor.Identifier);
        if ((object) descriptor.Identifier != (object) identifier)
          descriptor = new SubjectDescriptor("vssgp", identifier);
      }
      return descriptor;
    }

    public string MapFromWellKnownIdentifier(string identifier) => WellKnownIdentifierMapper.MapFromWellKnownIdentifier(identifier, this.DomainSid);

    internal static string MapFromWellKnownIdentifier(string identifier, string domainSid)
    {
      if (identifier.StartsWith(SidIdentityHelper.WellKnownSidPrefix, StringComparison.OrdinalIgnoreCase))
        identifier = domainSid + SidIdentityHelper.WellKnownSidType + identifier.Substring(SidIdentityHelper.WellKnownSidPrefix.Length);
      return identifier;
    }

    internal static unsafe string GetDomainSid(Guid domainId)
    {
      uint* numPtr = (uint*) &domainId;
      uint num1 = WellKnownIdentifierMapper.SwitchEndianness(*numPtr);
      uint num2 = WellKnownIdentifierMapper.SwitchEndianness(numPtr[1]);
      uint num3 = WellKnownIdentifierMapper.SwitchEndianness(numPtr[2]);
      uint num4 = WellKnownIdentifierMapper.SwitchEndianness(numPtr[3]);
      char* pSrc1 = stackalloc char[10];
      char* pSrc2 = stackalloc char[10];
      char* pSrc3 = stackalloc char[10];
      char* pSrc4 = stackalloc char[10];
      WellKnownIdentifierMapper.WriteDigits(num1, pSrc1 + 10);
      WellKnownIdentifierMapper.WriteDigits(num2, pSrc2 + 10);
      WellKnownIdentifierMapper.WriteDigits(num3, pSrc3 + 10);
      WellKnownIdentifierMapper.WriteDigits(num4, pSrc4 + 10);
      char* chPtr1 = stackalloc char[61];
      fixed (char* source = "S-1-9-1551374245-")
        Buffer.MemoryCopy((void*) source, (void*) chPtr1, 122L, (long) ("S-1-9-1551374245-".Length * 2));
      int length = "S-1-9-1551374245-".Length;
      int num5 = WellKnownIdentifierMapper.CopyDigits(pSrc1, chPtr1, length);
      char* chPtr2 = chPtr1;
      int num6 = num5;
      int tgtIndex1 = num6 + 1;
      IntPtr num7 = (IntPtr) num6 * 2;
      *(short*) ((IntPtr) chPtr2 + num7) = (short) 45;
      int num8 = WellKnownIdentifierMapper.CopyDigits(pSrc2, chPtr1, tgtIndex1);
      char* chPtr3 = chPtr1;
      int num9 = num8;
      int tgtIndex2 = num9 + 1;
      IntPtr num10 = (IntPtr) num9 * 2;
      *(short*) ((IntPtr) chPtr3 + num10) = (short) 45;
      int num11 = WellKnownIdentifierMapper.CopyDigits(pSrc3, chPtr1, tgtIndex2);
      char* chPtr4 = chPtr1;
      int num12 = num11;
      int tgtIndex3 = num12 + 1;
      IntPtr num13 = (IntPtr) num12 * 2;
      *(short*) ((IntPtr) chPtr4 + num13) = (short) 45;
      int index = WellKnownIdentifierMapper.CopyDigits(pSrc4, chPtr1, tgtIndex3);
      chPtr1[index] = char.MinValue;
      return new string(chPtr1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static uint SwitchEndianness(uint input) => (uint) (((int) input & (int) byte.MaxValue) << 24 | ((int) input & 65280) << 8) | (input & 16711680U) >> 8 | (input & 4278190080U) >> 24;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int WriteDigits(uint value, char* digits)
    {
      int num = 0;
      do
      {
        *--digits = (char) (48U + value % 10U);
        value /= 10U;
        ++num;
      }
      while (value != 0U);
      for (int index = num; index < 10; ++index)
        *--digits = char.MinValue;
      return num;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static unsafe int CopyDigits(char* pSrc, char* pTgt, int tgtIndex)
    {
      int index = 0;
      while (pSrc[index] == char.MinValue)
        ++index;
      Buffer.MemoryCopy((void*) (pSrc + index), (void*) (pTgt + tgtIndex), (long) ((61 - tgtIndex) * 2), (long) ((10 - index) * 2));
      return tgtIndex + (10 - index);
    }

    private string DomainSid { get; }
  }
}
