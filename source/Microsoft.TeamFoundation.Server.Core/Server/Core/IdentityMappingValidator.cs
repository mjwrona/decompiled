// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.Core.IdentityMappingValidator
// Assembly: Microsoft.TeamFoundation.Server.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9DD3208E-87CF-4F7C-8D96-8880BDAD13B2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.Core.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.Core
{
  internal class IdentityMappingValidator
  {
    private TeamFoundationIdentity[][] m_resolvedIdentities;
    private IList<string> m_collectionTableIdentities;
    private IdentityMappingCollection m_mappings;
    private IVssRequestContext m_organizationRequestContext;
    private ISqlConnectionInfo m_collectionConnectionInfo;
    private char[] m_invalidCharacters;

    public IdentityMappingValidator(
      IVssRequestContext organizationRequestContext,
      ISqlConnectionInfo collectionConnectionInfo,
      IdentityMappingCollection mappings)
    {
      this.m_organizationRequestContext = organizationRequestContext;
      this.m_collectionConnectionInfo = collectionConnectionInfo;
      this.m_mappings = mappings;
    }

    public bool Run(out IList<Exception> errors)
    {
      errors = (IList<Exception>) new List<Exception>();
      try
      {
        List<IdentityMappingEntry> list1 = this.m_mappings.Where<IdentityMappingEntry>((Func<IdentityMappingEntry, bool>) (i => !string.Equals(i.LocalIdentity, IdentityMappingEntry.NoMappingToken, StringComparison.OrdinalIgnoreCase))).ToList<IdentityMappingEntry>();
        List<IdentityMappingEntry> list2 = list1.Where<IdentityMappingEntry>((Func<IdentityMappingEntry, bool>) (i => string.IsNullOrWhiteSpace(i.LocalIdentity))).ToList<IdentityMappingEntry>();
        if (list2.Count > 0)
        {
          foreach (IdentityMappingEntry identityMappingEntry in list2)
            errors.Add(new Exception(FrameworkResources.AttachHostedCollectionMappingFileMappingLocalIdentityNotSpecified((object) identityMappingEntry.CloudIdentity)));
          list1 = list1.Where<IdentityMappingEntry>((Func<IdentityMappingEntry, bool>) (i => !string.IsNullOrWhiteSpace(i.LocalIdentity))).ToList<IdentityMappingEntry>();
        }
        if (this.m_resolvedIdentities == null)
        {
          this.m_resolvedIdentities = this.m_organizationRequestContext.GetService<TeamFoundationIdentityService>().ReadIdentities(this.m_organizationRequestContext, IdentitySearchFactor.AccountName, list1.Select<IdentityMappingEntry, string>((Func<IdentityMappingEntry, string>) (i => i.LocalIdentity)).ToArray<string>(), MembershipQuery.None, ReadIdentityOptions.IncludeReadFromSource, (IEnumerable<string>) null);
          using (DetachedHostedCollectionComponent componentRaw = this.m_collectionConnectionInfo.CreateComponentRaw<DetachedHostedCollectionComponent>())
          {
            componentRaw.PartitionId = 1;
            this.m_collectionTableIdentities = (IList<string>) componentRaw.GetAccountNamesForClaimsIdentities();
          }
        }
        this.ValidateAllMappingsResolved(this.m_resolvedIdentities, (IList<IdentityMappingEntry>) list1, errors);
        this.ValidateOneResolvedIdentity(this.m_resolvedIdentities, (IList<IdentityMappingEntry>) list1, errors);
        this.ValidateOneCloudIdentityPerLocalIdentity(this.m_resolvedIdentities, (IList<IdentityMappingEntry>) list1, errors);
        this.ValidateNoDuplicateCloudIdentities(this.m_mappings, errors);
        this.ValidateMappingsMatchIdentityTable((IList<IdentityMappingEntry>) this.m_mappings, this.m_collectionTableIdentities, errors);
        if (errors.Count == 0)
          this.ValidateNoInvalidCharacters(list1, new Func<char[]>(this.GetInvalidCharacters), errors);
      }
      catch (Exception ex)
      {
        errors.Add((Exception) new AttachCollectionException(FrameworkResources.AttachHostedCollectionUnexpectedMappingValidationException((object) ex.ToString())));
      }
      return errors.Count == 0;
    }

    public void ValidateNoDuplicateCloudIdentities(
      IdentityMappingCollection mappings,
      IList<Exception> errors)
    {
      Dictionary<string, IdentityMappingEntry> dictionary = new Dictionary<string, IdentityMappingEntry>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (IdentityMappingEntry mapping in (List<IdentityMappingEntry>) mappings)
      {
        IdentityMappingEntry item = mapping;
        if (!dictionary.ContainsKey(item.CloudIdentity))
          dictionary[item.CloudIdentity] = item;
        else
          this.TryValidation(errors, (Action) (() =>
          {
            throw new AttachCollectionException(FrameworkResources.AttachHostedCollectionDuplicateMappingFileEntries((object) item.CloudIdentity));
          }));
      }
    }

    public void ValidateOneCloudIdentityPerLocalIdentity(
      TeamFoundationIdentity[][] resolvedIdentities,
      IList<IdentityMappingEntry> itemsToMap,
      IList<Exception> errors)
    {
      Dictionary<string, string> sidToCloudMappings = new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      for (int i = 0; i < resolvedIdentities.Length; i++)
      {
        for (int index = 0; index < resolvedIdentities[i].Length; ++index)
        {
          string sid = resolvedIdentities[i][index].Descriptor.Identifier;
          if (sidToCloudMappings.ContainsKey(sid))
            this.TryValidation(errors, (Action) (() =>
            {
              throw new AttachCollectionException(FrameworkResources.AttachHostedCollectionDuplicateLocalIdentities((object) itemsToMap[i].CloudIdentity, (object) sidToCloudMappings[sid], (object) sid));
            }));
          else
            sidToCloudMappings[sid] = itemsToMap[i].CloudIdentity;
        }
      }
    }

    public void ValidateOneResolvedIdentity(
      TeamFoundationIdentity[][] resolvedIdentities,
      IList<IdentityMappingEntry> itemsToMap,
      IList<Exception> errors)
    {
      for (int i = 0; i < resolvedIdentities.Length; i++)
        this.TryValidation(errors, (Action) (() =>
        {
          if (resolvedIdentities[i].Length > 1)
            throw new AttachCollectionException(FrameworkResources.AttachHostedCollectionMultipleResolvedLocalMappings((object) itemsToMap[i].CloudIdentity, (object) itemsToMap[i].LocalIdentity));
        }));
    }

    public void ValidateAllMappingsResolved(
      TeamFoundationIdentity[][] results,
      IList<IdentityMappingEntry> itemsToMap,
      IList<Exception> errors)
    {
      for (int i = 0; i < results.Length; i++)
        this.TryValidation(errors, (Action) (() =>
        {
          if (results[i].Length == 0)
            throw new AttachCollectionException(FrameworkResources.AttachHostedCollectionCannotResolveLocalIdentity((object) itemsToMap[i].LocalIdentity));
        }));
    }

    public void ValidateMappingsMatchIdentityTable(
      IList<IdentityMappingEntry> mappings,
      IList<string> collectionTableCloudIdentities,
      IList<Exception> errors)
    {
      this.TryValidation(errors, (Action) (() =>
      {
        this.ValidateSetContainment(errors, (IEnumerable<string>) collectionTableCloudIdentities, mappings.Select<IdentityMappingEntry, string>((Func<IdentityMappingEntry, string>) (m => m.CloudIdentity)), (Func<string, string>) (s => FrameworkResources.AttachHostedCollectionMappingFileMissingEntries((object) s)));
        this.ValidateSetContainment(errors, mappings.Select<IdentityMappingEntry, string>((Func<IdentityMappingEntry, string>) (m => m.CloudIdentity)), (IEnumerable<string>) collectionTableCloudIdentities, (Func<string, string>) (s => FrameworkResources.AttachHostedCollectionMappingFileExtraClouldIdentity((object) s)));
      }));
    }

    private void ValidateNoInvalidCharacters(
      List<IdentityMappingEntry> itemsToMap,
      Func<char[]> invalidCharactersProvider,
      IList<Exception> errors)
    {
      for (int index = 0; index < itemsToMap.Count; ++index)
      {
        IdentityMappingEntry itemsTo = itemsToMap[index];
        string localIdentity = itemsTo.LocalIdentity;
        string str = (string) null;
        foreach (char c in itemsTo.LocalIdentity)
        {
          if ((c < 'a' || c > 'z') && (c < 'A' || c > 'Z') && (c < '0' || c > '9') && c != '\\' && c != '@' && c != '-' && c != '_' && c != '.')
          {
            if (this.m_invalidCharacters == null)
              this.m_invalidCharacters = invalidCharactersProvider();
            if (Array.BinarySearch<char>(this.m_invalidCharacters, c) >= 0)
            {
              if (str == null)
                str = new string(c, 1);
              else if (str.IndexOf(c) < 0)
                str += c.ToString();
            }
          }
        }
        if (str != null)
        {
          AttachCollectionException collectionException = new AttachCollectionException(FrameworkResources.AttachHostedCollectionIdentityNameContainsInvalidCharacters((object) localIdentity, (object) str));
          errors.Add((Exception) collectionException);
        }
      }
    }

    private void ValidateSetContainment(
      IList<Exception> errors,
      IEnumerable<string> listA,
      IEnumerable<string> listB,
      Func<string, string> formattedError)
    {
      HashSet<string> stringSet = new HashSet<string>(listA, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      HashSet<string> setB = new HashSet<string>(listB, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string str in stringSet)
      {
        string item = str;
        this.TryValidation(errors, (Action) (() =>
        {
          if (!setB.Contains(item))
            throw new AttachCollectionException(formattedError(item));
        }));
      }
    }

    private char[] GetInvalidCharacters()
    {
      using (SqlScriptResourceComponent componentRaw = this.m_collectionConnectionInfo.CreateComponentRaw<SqlScriptResourceComponent>(logger: (ITFLogger) new NullLogger()))
      {
        string resourceAsString = EmbeddedResourceUtil.GetResourceAsString("stmt_QueryInvalidCharacters.sql");
        return ((string) componentRaw.ExecuteStatementScalar(resourceAsString, 900)).ToCharArray();
      }
    }

    private void TryValidation(IList<Exception> errors, Action action)
    {
      try
      {
        action();
      }
      catch (Exception ex)
      {
        errors.Add(ex);
      }
    }
  }
}
