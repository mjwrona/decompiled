// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Graph.SubjectTypeMapper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Graph
{
  public class SubjectTypeMapper
  {
    public static readonly SubjectTypeMapper Instance = new SubjectTypeMapper();
    private string[] typeNameFromId;
    private IReadOnlyDictionary<string, byte> typeIdFromName = (IReadOnlyDictionary<string, byte>) new ReadOnlyDictionary<string, byte>((IDictionary<string, byte>) new Dictionary<string, byte>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "aad",
        (byte) 20
      },
      {
        "msa",
        (byte) 21
      },
      {
        "unusr",
        (byte) 22
      },
      {
        "aadgp",
        (byte) 40
      },
      {
        "vssgp",
        (byte) 41
      },
      {
        "ungrp",
        (byte) 42
      },
      {
        "bnd",
        (byte) 23
      },
      {
        "win",
        (byte) 24
      },
      {
        "uauth",
        (byte) 25
      },
      {
        "svc",
        (byte) 26
      },
      {
        "agg",
        (byte) 27
      },
      {
        "imp",
        (byte) 28
      },
      {
        "tst",
        (byte) 30
      },
      {
        "scp",
        (byte) 50
      },
      {
        "csp",
        (byte) 29
      },
      {
        "aadsp",
        (byte) 32
      },
      {
        "s2s",
        (byte) 254
      },
      {
        "slic",
        (byte) 253
      },
      {
        "sscp",
        (byte) 252
      },
      {
        "acs",
        (byte) 31
      },
      {
        "ukn",
        byte.MaxValue
      }
    });

    public SubjectTypeMapper()
    {
      this.typeNameFromId = new string[256];
      foreach (KeyValuePair<string, byte> keyValuePair in (IEnumerable<KeyValuePair<string, byte>>) this.typeIdFromName)
        this.typeNameFromId[(int) keyValuePair.Value] = keyValuePair.Key;
    }

    public byte GetTypeIdFromName(string typeName)
    {
      ArgumentUtility.CheckForNull<string>(typeName, nameof (typeName));
      byte maxValue;
      if (!this.typeIdFromName.TryGetValue(typeName, out maxValue))
        maxValue = byte.MaxValue;
      return maxValue;
    }

    public string GetTypeNameFromId(byte typeId) => this.typeNameFromId[(int) typeId] ?? "ukn";

    public bool IsValidTypeName(string typeName) => !string.IsNullOrEmpty(typeName) && this.GetTypeIdFromName(typeName) != byte.MaxValue;

    private enum SubjectTypeId : byte
    {
      AadUser = 20, // 0x14
      MsaUser = 21, // 0x15
      UnknownUser = 22, // 0x16
      BindPendingUser = 23, // 0x17
      WindowsIdentity = 24, // 0x18
      UnauthenticatedIdentity = 25, // 0x19
      ServiceIdentity = 26, // 0x1A
      AggregateIdentity = 27, // 0x1B
      ImportedIdentity = 28, // 0x1C
      CspPartnerIdentity = 29, // 0x1D
      ServerTestIdentity = 30, // 0x1E
      AcsServiceIdentity = 31, // 0x1F
      AadServicePrincipal = 32, // 0x20
      AadGroup = 40, // 0x28
      VstsGroup = 41, // 0x29
      UnknownGroup = 42, // 0x2A
      GroupScopeType = 50, // 0x32
      SystemScope = 252, // 0xFC
      SystemLicense = 253, // 0xFD
      SystemServicePrincipal = 254, // 0xFE
      Unknown = 255, // 0xFF
    }
  }
}
