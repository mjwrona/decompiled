// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UserHiveRegistryExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class UserHiveRegistryExtensions
  {
    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path)
    {
      return registryService.GetValue(requestContext, identity.Id, path);
    }

    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path)
    {
      return registryService.GetValue(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), false, (string) null);
    }

    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path,
      bool fallThru,
      string defaultValue)
    {
      return registryService.GetValue(requestContext, identity.Id, path, fallThru, defaultValue);
    }

    public static string GetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path,
      bool fallThru,
      string defaultValue)
    {
      return registryService.GetValue(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), fallThru, defaultValue);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path)
    {
      return registryService.GetValue<T>(requestContext, identity.Id, path);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path)
    {
      return registryService.GetValue<T>(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), false, default (T));
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path,
      T defaultValue)
    {
      return registryService.GetValue<T>(requestContext, identity.Id, path, defaultValue);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path,
      T defaultValue)
    {
      return registryService.GetValue<T>(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), defaultValue);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path,
      bool fallThru,
      T defaultValue)
    {
      return registryService.GetValue<T>(requestContext, identity.Id, path, fallThru, defaultValue);
    }

    public static T GetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path,
      bool fallThru,
      T defaultValue)
    {
      return registryService.GetValue<T>(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), fallThru, defaultValue);
    }

    public static RegistryEntryCollection ReadEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string registryPathPattern)
    {
      return registryService.ReadEntries(requestContext, identity.Id, registryPathPattern);
    }

    public static RegistryEntryCollection ReadEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string registryPathPattern)
    {
      string userRootPath = UserHiveRegistryExtensions.GetUserRootPath(identityId);
      return UserHiveRegistryExtensions.MakeRelative(registryService.ReadEntries(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(userRootPath, registryPathPattern)), userRootPath);
    }

    public static RegistryEntryCollection ReadEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string registryPathPattern,
      bool includeFolders)
    {
      return registryService.ReadEntries(requestContext, identity.Id, registryPathPattern, includeFolders);
    }

    public static RegistryEntryCollection ReadEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string registryPathPattern,
      bool includeFolders)
    {
      string userRootPath = UserHiveRegistryExtensions.GetUserRootPath(identityId);
      return UserHiveRegistryExtensions.MakeRelative(registryService.ReadEntries(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(userRootPath, registryPathPattern), includeFolders), userRootPath);
    }

    public static RegistryEntryCollection ReadEntriesFallThru(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string registryPathPattern)
    {
      return registryService.ReadEntriesFallThru(requestContext, identity.Id, registryPathPattern);
    }

    public static RegistryEntryCollection ReadEntriesFallThru(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string registryPathPattern)
    {
      string userRootPath = UserHiveRegistryExtensions.GetUserRootPath(identityId);
      return UserHiveRegistryExtensions.MakeRelative(registryService.ReadEntriesFallThru(requestContext, (RegistryQuery) RegistryHelpers.CombinePath(userRootPath, registryPathPattern)), userRootPath);
    }

    public static void SetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path,
      string value)
    {
      registryService.SetValue(requestContext, identity.Id, path, value);
    }

    public static void SetValue(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path,
      string value)
    {
      registryService.SetValue<string>(requestContext, RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), value);
    }

    public static void SetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      string path,
      T value)
    {
      registryService.SetValue<T>(requestContext, identity.Id, path, value);
    }

    public static void SetValue<T>(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      string path,
      T value)
    {
      registryService.SetValue<T>(requestContext, RegistryHelpers.CombinePath(UserHiveRegistryExtensions.GetUserRootPath(identityId), path), value);
    }

    public static void WriteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      IEnumerable<RegistryEntry> registryEntries)
    {
      registryService.WriteEntries(requestContext, identity.Id, registryEntries);
    }

    public static void WriteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      IEnumerable<RegistryEntry> registryEntries)
    {
      ArgumentUtility.CheckForNull<IEnumerable<RegistryEntry>>(registryEntries, nameof (registryEntries));
      string userRootPath = UserHiveRegistryExtensions.GetUserRootPath(identityId);
      registryService.WriteEntries(requestContext, registryEntries.Select<RegistryEntry, RegistryEntry>((Func<RegistryEntry, RegistryEntry>) (s => new RegistryEntry(RegistryHelpers.CombinePath(userRootPath, s.Path), s.Value))));
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Identity.Identity identity,
      params string[] registryPathPatterns)
    {
      return registryService.DeleteEntries(requestContext, identity.Id, registryPathPatterns);
    }

    public static int DeleteEntries(
      this IVssRegistryService registryService,
      IVssRequestContext requestContext,
      Guid identityId,
      params string[] registryPathPatterns)
    {
      ArgumentUtility.CheckForNull<string[]>(registryPathPatterns, nameof (registryPathPatterns));
      string userRootPath = UserHiveRegistryExtensions.GetUserRootPath(identityId);
      string[] strArray = new string[registryPathPatterns.Length];
      for (int index = 0; index < registryPathPatterns.Length; ++index)
        strArray[index] = RegistryHelpers.CombinePath(userRootPath, registryPathPatterns[index]);
      return registryService.DeleteEntries(requestContext, strArray);
    }

    private static string GetUserRootPath(Guid identityId)
    {
      ArgumentUtility.CheckForEmptyGuid(identityId, nameof (identityId));
      return string.Format((IFormatProvider) CultureInfo.InvariantCulture, "/Users/{0}", (object) identityId);
    }

    private static RegistryEntryCollection MakeRelative(
      RegistryEntryCollection registryEntries,
      string relativeRoot)
    {
      foreach (RegistryEntry registryEntry in registryEntries)
        registryEntry.Path = RegistryHelpers.MakeRelative(registryEntry.Path, relativeRoot);
      return registryEntries;
    }
  }
}
