// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityTypeMapper
// Assembly: Microsoft.VisualStudio.Services.Identity, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 1372DA81-8681-4BFE-8E91-D1AB4333F834
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Identity.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.VisualStudio.Services.Identity
{
  public class IdentityTypeMapper
  {
    public static readonly IdentityTypeMapper Instance = new IdentityTypeMapper();
    private string[] typeNameFromId;
    private IReadOnlyDictionary<string, byte> typeIdFromName = (IReadOnlyDictionary<string, byte>) new ReadOnlyDictionary<string, byte>((IDictionary<string, byte>) new Dictionary<string, byte>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      {
        "System.Security.Principal.WindowsIdentity",
        (byte) 3
      },
      {
        "Microsoft.TeamFoundation.Identity",
        (byte) 4
      },
      {
        "Microsoft.IdentityModel.Claims.ClaimsIdentity",
        (byte) 5
      },
      {
        "Microsoft.TeamFoundation.BindPendingIdentity",
        (byte) 6
      },
      {
        "Microsoft.TeamFoundation.UnauthenticatedIdentity",
        (byte) 7
      },
      {
        "Microsoft.TeamFoundation.ServiceIdentity",
        (byte) 8
      },
      {
        "Microsoft.TeamFoundation.AggregateIdentity",
        (byte) 9
      },
      {
        "Microsoft.VisualStudio.Services.Identity.ServerTestIdentity",
        (byte) 100
      },
      {
        "Microsoft.TeamFoundation.ImportedIdentity",
        (byte) 10
      },
      {
        "Microsoft.VisualStudio.Services.Graph.GraphScope",
        (byte) 70
      },
      {
        "Microsoft.TeamFoundation.Claims.CspPartnerIdentity",
        (byte) 11
      },
      {
        "Microsoft.VisualStudio.Services.Claims.AadServicePrincipal",
        (byte) 12
      },
      {
        "System:ServicePrincipal",
        (byte) 254
      },
      {
        "System:License",
        (byte) 253
      },
      {
        "System:Scope",
        (byte) 252
      }
    });

    public IdentityTypeMapper()
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

    public string GetTypeNameFromId(byte typeId) => this.typeNameFromId[(int) typeId] ?? "Microsoft.VisualStudio.Services.Identity.UnknownIdentity";

    public bool IsValidTypeName(string typeName) => !string.IsNullOrEmpty(typeName) && this.GetTypeIdFromName(typeName) != byte.MaxValue;

    private enum IdentityTypeId : byte
    {
      WindowsIdentity = 3,
      TeamFoundationIdentity = 4,
      ClaimsIdentity = 5,
      BindPendingIdentity = 6,
      UnauthenticatedIdentity = 7,
      ServiceIdentity = 8,
      AggregateIdentity = 9,
      ImportedIdentity = 10, // 0x0A
      CspPartnerIdentity = 11, // 0x0B
      AadServicePrincipal = 12, // 0x0C
      GroupScope = 70, // 0x46
      ServerTestIdentity = 100, // 0x64
      System_Scope = 252, // 0xFC
      System_License = 253, // 0xFD
      System_ServicePrincipal = 254, // 0xFE
      Unknown = 255, // 0xFF
    }
  }
}
