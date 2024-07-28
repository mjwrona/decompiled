// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.IdentityParser
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Microsoft.VisualStudio.Services.Identity
{
  public static class IdentityParser
  {
    private static readonly IList<IdentityDescriptor> s_emptyIdentityDescriptorList = (IList<IdentityDescriptor>) new List<IdentityDescriptor>().AsReadOnly();
    private static readonly IList<Guid> s_emptyGuidList = (IList<Guid>) new List<Guid>().AsReadOnly();

    public static IList<IdentityDescriptor> GetDescriptorsFromString(string descriptors)
    {
      if (string.IsNullOrEmpty(descriptors))
        return IdentityParser.s_emptyIdentityDescriptorList;
      return (IList<IdentityDescriptor>) ((IEnumerable<string>) descriptors.Split(',')).Where<string>((Func<string, bool>) (descriptor => !string.IsNullOrEmpty(descriptor))).Select<string, IdentityDescriptor>((Func<string, IdentityDescriptor>) (descriptor => IdentityParser.GetDescriptorFromString(descriptor))).ToList<IdentityDescriptor>();
    }

    public static IdentityDescriptor GetDescriptorFromString(string descriptor)
    {
      if (descriptor == null)
        return (IdentityDescriptor) null;
      string[] strArray = descriptor.Split(new char[1]
      {
        ';'
      }, 2);
      if (strArray.Length == 2 && !string.IsNullOrEmpty(strArray[0]) && !string.IsNullOrEmpty(strArray[1]))
        return new IdentityDescriptor(strArray[0], HttpUtility.UrlDecode(strArray[1]));
      return strArray.Length == 1 && !string.IsNullOrEmpty(strArray[0]) ? new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", HttpUtility.UrlDecode(strArray[0])) : (IdentityDescriptor) null;
    }

    public static IList<Guid> GetIdentityIdsFromString(string identityIds)
    {
      if (string.IsNullOrWhiteSpace(identityIds))
        return IdentityParser.s_emptyGuidList;
      try
      {
        return (IList<Guid>) ((IEnumerable<string>) identityIds.Split(',')).Where<string>((Func<string, bool>) (identityId => !string.IsNullOrEmpty(identityId))).Select<string, Guid>((Func<string, Guid>) (identityId => new Guid(identityId))).ToList<Guid>();
      }
      catch (Exception ex)
      {
        TeamFoundationEventLog.Default.LogException(ex.Message, ex);
        throw;
      }
    }

    public static IEnumerable<string> GetPropertyFiltersFromString(string properties)
    {
      if (string.IsNullOrWhiteSpace(properties))
        return Enumerable.Empty<string>();
      return ((IEnumerable<string>) properties.Split(',')).Where<string>((Func<string, bool>) (x => !string.IsNullOrEmpty(x)));
    }
  }
}
