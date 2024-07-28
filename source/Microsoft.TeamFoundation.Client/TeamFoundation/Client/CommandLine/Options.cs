// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Client.CommandLine.Options
// Assembly: Microsoft.TeamFoundation.Client, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 03892C75-AE2B-482B-8E0D-B14588A2C857
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Client.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Client.CommandLine
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public abstract class Options
  {
    public static bool MatchesValue(string inputValue, Options.Value val) => inputValue != null && string.Equals(val.ToString(), inputValue, StringComparison.OrdinalIgnoreCase);

    public static void GetIDAndStyle(string alias, out Options.ID id, out Options.Style style)
    {
      id = Options.GetIDFromAlias(alias);
      if (id != Options.ID.UnknownOption)
        style = Options.GetStyleFromID(id);
      else
        style = Options.Style.NoValue;
    }

    public static Options.ID GetIDFromAlias(string alias)
    {
      if (string.IsNullOrEmpty(alias))
        return Options.ID.UnknownOption;
      alias = alias.ToUpperInvariant();
      if (alias.Length == 1)
      {
        switch (alias[0])
        {
          case '?':
          case 'H':
            return Options.ID.Help;
          case 'B':
            return Options.ID.BuildUri;
          case 'C':
            return Options.ID.Comment;
          case 'D':
            return Options.ID.Delete;
          case 'F':
            return Options.ID.Format;
          case 'I':
            return Options.ID.NoPrompt;
          case 'K':
            return Options.ID.Lock;
          case 'M':
            return Options.ID.Computer;
          case 'N':
            return Options.ID.NewName;
          case 'O':
            return Options.ID.Owner;
          case 'P':
            return Options.ID.Force;
          case 'R':
            return Options.ID.Recursive;
          case 'S':
            return Options.ID.Server;
          case 'T':
            return Options.ID.Template;
          case 'U':
            return Options.ID.User;
          case 'V':
            return Options.ID.Version;
          case 'W':
            return Options.ID.Workspace;
          case 'X':
            return Options.ID.SlotMode;
          case 'Y':
            return Options.ID.Login;
        }
      }
      else
      {
        try
        {
          return (Options.ID) Enum.Parse(typeof (Options.ID), alias, true);
        }
        catch (ArgumentException ex)
        {
        }
      }
      return Options.ID.UnknownOption;
    }

    public static Options.Style GetStyleFromID(Options.ID id)
    {
      if (id <= Options.ID.Machine)
      {
        if (id <= Options.ID.Diff)
        {
          if (id <= Options.ID.Cloak)
          {
            if (id <= Options.ID.Baseless)
            {
              if (id <= Options.ID.ApplyFilters)
              {
                if (id <= Options.ID.AgeInDays)
                {
                  if (id <= Options.ID.Adds)
                  {
                    if (id != Options.ID.Abort && id != Options.ID.Adds)
                      goto label_335;
                  }
                  else if ((uint) (id - 50) <= 2U || id == Options.ID.AgeInDays)
                    goto label_330;
                  else
                    goto label_335;
                }
                else if (id <= Options.ID.Allow)
                {
                  if ((uint) (id - 99) > 1U && id == Options.ID.Allow)
                    goto label_332;
                }
                else if (id != Options.ID.AppTierUri)
                {
                  if (id != Options.ID.AreaPath)
                  {
                    if (id != Options.ID.ApplyFilters)
                      goto label_335;
                  }
                  else
                    goto label_332;
                }
                else
                  goto label_330;
              }
              else if (id <= Options.ID.AutoProvision)
              {
                if (id <= Options.ID.Attachment)
                {
                  if (id != Options.ID.AssignFailuresToUser)
                  {
                    switch (id - 228)
                    {
                      case Options.ID.UnknownOption:
                        break;
                      case (Options.ID) 2:
                        goto label_332;
                      case (Options.ID) 3:
                        goto label_330;
                      default:
                        goto label_335;
                    }
                  }
                  else
                    goto label_330;
                }
                else if (id == Options.ID.Auto || id == Options.ID.AutoProvision)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id <= Options.ID.AutoSelect)
              {
                if (id != Options.ID.AutoResolveAll)
                {
                  if (id != Options.ID.AutoSelect)
                    goto label_335;
                }
                else
                  goto label_330;
              }
              else if (id != Options.ID.Author && id != Options.ID.Authority)
              {
                if (id != Options.ID.Baseless)
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.BuildVerification)
            {
              if (id <= Options.ID.BuildController)
              {
                if (id <= Options.ID.Branch)
                {
                  if (id == Options.ID.BlobId || id == Options.ID.Branch)
                    goto label_330;
                  else
                    goto label_335;
                }
                else if (id == Options.ID.Build || (uint) (id - 600) <= 1U)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id <= Options.ID.BuildDir)
              {
                if (id == Options.ID.BuildDefinition || (uint) (id - 800) <= 1U)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id != Options.ID.BuildProcess && (uint) (id - 900) > 1U)
              {
                if (id != Options.ID.BuildVerification)
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.Changeset)
            {
              if (id <= Options.ID.Candidate)
              {
                if (id != Options.ID.Bypass && id != Options.ID.Candidate)
                  goto label_335;
              }
              else if (id != Options.ID.Category)
              {
                if (id != Options.ID.Change && id == Options.ID.Changeset)
                  goto label_330;
              }
              else
                goto label_333;
            }
            else if (id <= Options.ID.Checkin)
            {
              if (id != Options.ID.Changesets)
              {
                if (id != Options.ID.Checkin)
                  goto label_335;
              }
              else
                goto label_332;
            }
            else if ((uint) (id - 1200) > 1U)
            {
              if (id != Options.ID.Clean && id != Options.ID.Cloak)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id <= Options.ID.Controller)
          {
            if (id <= Options.ID.CommitId)
            {
              if (id <= Options.ID.CollectionDb)
              {
                if (id <= Options.ID.CloneChildren)
                {
                  if (id != Options.ID.Clone && id != Options.ID.CloneChildren)
                    goto label_335;
                }
                else if (id != Options.ID.CloneRequirements && id == Options.ID.CollectionDb)
                  goto label_330;
              }
              else if (id <= Options.ID.CollectionId)
              {
                if (id == Options.ID.Collection || id == Options.ID.CollectionId)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id != Options.ID.CollectionName)
              {
                if (id != Options.ID.Comment)
                {
                  if (id == Options.ID.CommitId)
                    goto label_330;
                  else
                    goto label_335;
                }
                else
                  goto label_333;
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.ConfigurationFolder)
            {
              if (id <= Options.ID.ConfigurationDb)
              {
                if (id == Options.ID.Computer || id == Options.ID.ConfigurationDb)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id != Options.ID.ConfigDBOnly && id == Options.ID.ConfigurationFolder)
                goto label_330;
            }
            else if (id <= Options.ID.Conservative)
            {
              switch (id - 1700)
              {
                case Options.ID.UnknownOption:
                case (Options.ID) 3:
                case (Options.ID) 7:
                  break;
                case (Options.ID) 1:
                case (Options.ID) 2:
                  goto label_330;
                case (Options.ID) 4:
                case (Options.ID) 5:
                case (Options.ID) 6:
                case (Options.ID) 8:
                case (Options.ID) 9:
                  goto label_335;
                case (Options.ID) 10:
                  goto label_333;
                default:
                  if (id == Options.ID.Conservative)
                    break;
                  goto label_335;
              }
            }
            else if (id != Options.ID.Console && id != Options.ID.Continue && id == Options.ID.Controller)
              goto label_330;
          }
          else if (id <= Options.ID.Delete)
          {
            if (id <= Options.ID.Days)
            {
              if (id <= Options.ID.Create)
              {
                if (id != Options.ID.ConvertToType)
                {
                  if (id != Options.ID.Create)
                    goto label_335;
                }
                else
                  goto label_330;
              }
              else
              {
                switch (id - 2000)
                {
                  case Options.ID.UnknownOption:
                  case (Options.ID) 1:
                  case (Options.ID) 2:
                  case (Options.ID) 5:
                  case (Options.ID) 7:
                  case (Options.ID) 10:
                    goto label_330;
                  case (Options.ID) 3:
                  case (Options.ID) 4:
                  case (Options.ID) 6:
                  case (Options.ID) 8:
                  case (Options.ID) 9:
                    goto label_335;
                  default:
                    if (id == Options.ID.Days)
                      goto label_330;
                    else
                      goto label_335;
                }
              }
            }
            else if (id <= Options.ID.Default)
            {
              if (id != Options.ID.Decloak && id == Options.ID.Default)
                goto label_330;
            }
            else if (id != Options.ID.DefaultCollection)
            {
              if (id != Options.ID.DefaultMailAddress)
              {
                if (id != Options.ID.Delete)
                  goto label_335;
              }
              else
                goto label_330;
            }
          }
          else if (id <= Options.ID.Description)
          {
            if (id <= Options.ID.Deletes)
            {
              if (id != Options.ID.DeleteAll)
              {
                switch (id - 2400)
                {
                  case Options.ID.UnknownOption:
                  case (Options.ID) 5:
                    break;
                  case (Options.ID) 1:
                    goto label_330;
                  case (Options.ID) 2:
                    goto label_332;
                  default:
                    goto label_335;
                }
              }
            }
            else if (id != Options.ID.DeleteValues && id != Options.ID.Deny)
            {
              if (id == Options.ID.Description)
                goto label_333;
              else
                goto label_335;
            }
            else
              goto label_332;
          }
          else if (id <= Options.ID.DestroyCodeIndex)
          {
            if ((uint) (id - 2625) > 3U)
            {
              if (id != Options.ID.DestroyCodeIndex)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id != Options.ID.Detach && id != Options.ID.Detect && id != Options.ID.Diff)
            goto label_335;
        }
        else if (id <= Options.ID.GetKeyFingerprint)
        {
          if (id <= Options.ID.Export)
          {
            if (id <= Options.ID.DropLocation)
            {
              if (id <= Options.ID.Display)
              {
                if (id <= Options.ID.Disable)
                {
                  if (id != Options.ID.Directory)
                  {
                    if (id != Options.ID.Disable)
                      goto label_335;
                  }
                  else
                    goto label_330;
                }
                else if (id != Options.ID.Discard && id != Options.ID.Display)
                  goto label_335;
              }
              else if (id <= Options.ID.DnsSuffix)
              {
                if (id != Options.ID.Dns && id == Options.ID.DnsSuffix)
                  goto label_333;
              }
              else if (id != Options.ID.Downgrade && id != Options.ID.DropExistingDatabases && id == Options.ID.DropLocation)
                goto label_330;
            }
            else if (id <= Options.ID.EventFilter)
            {
              if (id <= Options.ID.Edit)
              {
                if (id != Options.ID.DumpLog && id != Options.ID.Edit)
                  goto label_335;
              }
              else if (id != Options.ID.EmailAddress)
              {
                switch (id - 3200)
                {
                  case Options.ID.UnknownOption:
                  case (Options.ID) 2:
                  case (Options.ID) 3:
                    goto label_330;
                  case (Options.ID) 1:
                    break;
                  default:
                    goto label_335;
                }
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.Exclude)
            {
              if (id != Options.ID.Encoding)
              {
                if (id == Options.ID.Exclude)
                  goto label_332;
                else
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id != Options.ID.ExitCode)
            {
              if (id != Options.ID.ExpectedResultsField)
              {
                if (id != Options.ID.Export)
                  goto label_335;
              }
              else
                goto label_330;
            }
          }
          else if (id <= Options.ID.FilterLocalPathsOnly)
          {
            if (id <= Options.ID.FeatureFlag)
            {
              if (id <= Options.ID.Extended)
              {
                if (id != Options.ID.Extend && id != Options.ID.Extended)
                  goto label_335;
              }
              else if (id != Options.ID.External && id == Options.ID.FeatureFlag)
                goto label_330;
            }
            else if (id <= Options.ID.FileCount)
            {
              if (id != Options.ID.File && id == Options.ID.FileCount)
                goto label_330;
            }
            else if (id != Options.ID.FileTime)
            {
              if (id != Options.ID.Filter)
              {
                if (id != Options.ID.FilterLocalPathsOnly)
                  goto label_335;
              }
              else
                goto label_333;
            }
            else
              goto label_330;
          }
          else if (id <= Options.ID.FromDomain)
          {
            if (id <= Options.ID.Folders)
            {
              if (id != Options.ID.Flavor)
              {
                if (id != Options.ID.Folders)
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id != Options.ID.Force && (id == Options.ID.Format || id == Options.ID.FromDomain))
              goto label_330;
          }
          else if (id <= Options.ID.FromPool)
          {
            if (id == Options.ID.FromEmailAddress || id == Options.ID.FromPool)
              goto label_330;
            else
              goto label_335;
          }
          else if (id != Options.ID.FromSCE && id != Options.ID.FromSE && id != Options.ID.GetKeyFingerprint)
            goto label_335;
        }
        else if (id <= Options.ID.IPBlock)
        {
          if (id <= Options.ID.IgnoreSpace)
          {
            if (id <= Options.ID.HostGroup)
            {
              if (id <= Options.ID.Global)
              {
                if (id != Options.ID.GetOption)
                {
                  if (id != Options.ID.Global)
                    goto label_335;
                }
                else
                  goto label_330;
              }
              else if (id != Options.ID.Group)
              {
                if (id != Options.ID.HostGroup)
                  goto label_335;
              }
              else
                goto label_332;
            }
            else if (id <= Options.ID.IgnoreCase)
            {
              switch (id - 4300)
              {
                case Options.ID.UnknownOption:
                case (Options.ID) 2:
                  goto label_330;
                case (Options.ID) 1:
                  break;
                default:
                  if (id == Options.ID.IgnoreCase)
                    break;
                  goto label_335;
              }
            }
            else if ((uint) (id - 4500) > 1U)
            {
              if (id != Options.ID.IgnoreList)
              {
                if (id != Options.ID.IgnoreSpace)
                  goto label_335;
              }
              else
                goto label_330;
            }
          }
          else if (id <= Options.ID.IndexHistoryPeriod)
          {
            if (id <= Options.ID.Import)
            {
              if (id != Options.ID.Impersonate)
              {
                if (id != Options.ID.Import)
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id != Options.ID.Include && id == Options.ID.IndexHistoryPeriod)
              goto label_333;
          }
          else if (id <= Options.ID.Inputs)
          {
            if (id != Options.ID.IndexingStatus && (uint) (id - 4700) <= 1U)
              goto label_330;
          }
          else if (id != Options.ID.Install && (id == Options.ID.IP || id == Options.ID.IPBlock))
            goto label_330;
        }
        else if (id <= Options.ID.Latest)
        {
          if (id <= Options.ID.LabEnvironment)
          {
            if (id <= Options.ID.KeepMergeHistory)
            {
              if (id != Options.ID.ItemMode && (uint) (id - 4800) > 1U)
                goto label_335;
            }
            else if (id != Options.ID.Keywords)
            {
              if (id == Options.ID.LabEnvironment)
                goto label_330;
              else
                goto label_335;
            }
            else
              goto label_332;
          }
          else if (id <= Options.ID.LabEnvironmentPlacementPolicy)
          {
            if (id != Options.ID.LabEnvironmentIds)
            {
              if (id == Options.ID.LabEnvironmentPlacementPolicy)
                goto label_330;
              else
                goto label_335;
            }
            else
              goto label_332;
          }
          else if (id != Options.ID.LabTemplate)
          {
            if (id != Options.ID.LabUrl)
            {
              if (id != Options.ID.Latest)
                goto label_335;
            }
            else
              goto label_333;
          }
          else
            goto label_330;
        }
        else if (id <= Options.ID.ListSCVMMHostGroups)
        {
          if (id <= Options.ID.LibraryShare)
          {
            if (id != Options.ID.LCID)
            {
              if (id != Options.ID.LibraryShare)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id != Options.ID.List && id != Options.ID.ListLargeFiles && id != Options.ID.ListSCVMMHostGroups)
            goto label_335;
        }
        else if (id <= Options.ID.LogFile)
        {
          switch (id - 4995)
          {
            case Options.ID.UnknownOption:
            case (Options.ID) 3:
              break;
            case (Options.ID) 1:
            case (Options.ID) 2:
              goto label_335;
            case (Options.ID) 4:
            case (Options.ID) 5:
              goto label_330;
            default:
              if (id == Options.ID.LogFile)
                goto label_330;
              else
                goto label_335;
          }
        }
        else
        {
          if (id == Options.ID.Login)
            return Options.Style.TwoValues;
          if (id != Options.ID.LogToConsole && id != Options.ID.Machine)
            goto label_335;
        }
      }
      else if (id <= Options.ID.RepoUri)
      {
        if (id <= Options.ID.OverrideField)
        {
          if (id <= Options.ID.NewWITName)
          {
            if (id <= Options.ID.MinSize)
            {
              if (id <= Options.ID.MappingFile)
              {
                if (id <= Options.ID.LoginType)
                {
                  if (id != Options.ID.MakeDefault && id == Options.ID.LoginType)
                    goto label_330;
                }
                else if (id != Options.ID.Map && id == Options.ID.MappingFile)
                  goto label_330;
              }
              else if (id <= Options.ID.MaxPriority)
              {
                if (id == Options.ID.MaxBuilds || id == Options.ID.MaxPriority)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id == Options.ID.MaxProcesses || id == Options.ID.MinPriority || id == Options.ID.MinSize)
                goto label_330;
              else
                goto label_335;
            }
            else if (id <= Options.ID.Name)
            {
              if (id <= Options.ID.MSBuildArguments)
              {
                if (id != Options.ID.Move && id == Options.ID.MSBuildArguments)
                  goto label_333;
              }
              else if (id == Options.ID.MsiPatchRemovalList || id == Options.ID.Name)
                goto label_330;
              else
                goto label_335;
            }
            else if (id <= Options.ID.New)
            {
              if (id != Options.ID.NetworkLocation)
              {
                if (id != Options.ID.New)
                  goto label_335;
              }
              else
                goto label_333;
            }
            else if (id == Options.ID.NewName || id == Options.ID.NewOwner || id == Options.ID.NewWITName)
              goto label_330;
            else
              goto label_335;
          }
          else if (id <= Options.ID.NoRegex)
          {
            if (id <= Options.ID.NoGet)
            {
              if (id <= Options.ID.NoConflictsCheckForGated)
              {
                if (id != Options.ID.NoAutoResolve && id != Options.ID.NoConflictsCheckForGated)
                  goto label_335;
              }
              else if (id != Options.ID.NoDetect && id != Options.ID.NoGet)
                goto label_335;
            }
            else if (id <= Options.ID.NoImplicitBaseless)
            {
              if (id != Options.ID.NoIgnore && id != Options.ID.NoImplicitBaseless)
                goto label_335;
            }
            else if (id != Options.ID.NoMerge && id != Options.ID.NoPrompt && id != Options.ID.NoRegex)
              goto label_335;
          }
          else
          {
            if (id <= Options.ID.Options)
            {
              if (id <= Options.ID.Notes)
              {
                if (id == Options.ID.NoSummary || id != Options.ID.Notes)
                  goto label_329;
                else
                  goto label_333;
              }
              else if (id != Options.ID.Number)
              {
                if (id != Options.ID.Oid)
                {
                  if (id == Options.ID.Options)
                    goto label_330;
                  else
                    goto label_335;
                }
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.Override)
            {
              if (id != Options.ID.Output)
              {
                if (id == Options.ID.Override)
                  goto label_333;
                else
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id != Options.ID.OverrideFieldName && id != Options.ID.OverrideFieldValue)
            {
              if (id != Options.ID.OverrideField)
                goto label_335;
            }
            else
              goto label_330;
            return Options.Style.NameValuePair;
          }
        }
        else if (id <= Options.ID.Property)
        {
          if (id <= Options.ID.Postpone)
          {
            if (id <= Options.ID.PlanName)
            {
              if (id <= Options.ID.OverrideType)
              {
                if (id == Options.ID.OverrideMailAddress || id == Options.ID.OverrideType)
                  goto label_330;
                else
                  goto label_335;
              }
              else if (id != Options.ID.Owner)
              {
                switch (id - 7100)
                {
                  case Options.ID.UnknownOption:
                    break;
                  case (Options.ID) 1:
                  case (Options.ID) 2:
                  case (Options.ID) 3:
                  case (Options.ID) 4:
                  case (Options.ID) 5:
                  case (Options.ID) 7:
                  case (Options.ID) 10:
                  case (Options.ID) 12:
                  case (Options.ID) 13:
                    goto label_330;
                  default:
                    goto label_335;
                }
              }
              else
                goto label_330;
            }
            else if (id <= Options.ID.PersonalAccessTokenFile)
            {
              if (id == Options.ID.Permission || id == Options.ID.PersonalAccessTokenFile)
                goto label_330;
              else
                goto label_335;
            }
            else if (id != Options.ID.Platform && id != Options.ID.Port)
            {
              if (id != Options.ID.Postpone)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id <= Options.ID.ProductKey)
          {
            if (id <= Options.ID.Priority)
            {
              if (id != Options.ID.Preview && id == Options.ID.Priority)
                goto label_330;
            }
            else if (id == Options.ID.ProcessModel || id == Options.ID.ProductKey)
              goto label_330;
            else
              goto label_335;
          }
          else if (id <= Options.ID.Promote)
          {
            if (id != Options.ID.ProjectCollectionsOnly && id != Options.ID.Promote)
              goto label_335;
          }
          else if (id != Options.ID.Prompt && (id == Options.ID.Properties || id == Options.ID.Property))
            goto label_333;
        }
        else if (id <= Options.ID.ReApply)
        {
          if (id <= Options.ID.PublicUrl)
          {
            if (id <= Options.ID.Provider)
            {
              if (id != Options.ID.PropertyNames)
              {
                if (id == Options.ID.Provider)
                  goto label_330;
                else
                  goto label_335;
              }
              else
                goto label_332;
            }
            else if (id != Options.ID.Proxy && id == Options.ID.PublicUrl)
              goto label_330;
          }
          else if (id <= Options.ID.Quiet)
          {
            if (id != Options.ID.Publish && (uint) (id - 7700) > 1U)
              goto label_335;
          }
          else if (id != Options.ID.QueryText)
          {
            if (id != Options.ID.AnalysisServices && id != Options.ID.ReApply)
              goto label_335;
          }
          else
            goto label_330;
        }
        else if (id <= Options.ID.Remove)
        {
          if (id <= Options.ID.Relationships)
          {
            switch (id - 7800)
            {
              case Options.ID.UnknownOption:
              case (Options.ID) 6:
                break;
              case (Options.ID) 1:
                goto label_333;
              case (Options.ID) 2:
                goto label_330;
              case (Options.ID) 3:
              case (Options.ID) 4:
              case (Options.ID) 5:
                goto label_335;
              default:
                if (id == Options.ID.Relationships)
                  break;
                goto label_335;
            }
          }
          else if (id != Options.ID.RemoteDbServerName)
          {
            if (id != Options.ID.Remap && id == Options.ID.Remove)
              goto label_332;
          }
          else
            goto label_330;
        }
        else if (id <= Options.ID.RepairEnvironments)
        {
          if (id != Options.ID.RemoveComponents && id != Options.ID.RepairEnvironments)
            goto label_335;
        }
        else if (id != Options.ID.Replace && (id == Options.ID.Repository || id == Options.ID.RepoUri))
          goto label_330;
      }
      else if (id <= Options.ID.TeamProjects)
      {
        if (id <= Options.ID.Silent)
        {
          if (id <= Options.ID.Scope)
          {
            if (id <= Options.ID.ResultOwner)
            {
              if (id <= Options.ID.ResourceFile)
              {
                switch (id - 8100)
                {
                  case Options.ID.UnknownOption:
                  case (Options.ID) 4:
                  case (Options.ID) 5:
                  case (Options.ID) 10:
                    goto label_330;
                  case (Options.ID) 1:
                  case (Options.ID) 6:
                  case (Options.ID) 7:
                  case (Options.ID) 8:
                  case (Options.ID) 9:
                    goto label_335;
                  case (Options.ID) 2:
                  case (Options.ID) 3:
                    break;
                  default:
                    if (id == Options.ID.ResourceFile)
                      goto label_330;
                    else
                      goto label_335;
                }
              }
              else if (id != Options.ID.Resume && (uint) (id - 8201) <= 1U)
                goto label_330;
            }
            else if (id <= Options.ID.Retry)
            {
              if ((uint) (id - 8300) > 1U)
              {
                if (id != Options.ID.Retry)
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id != Options.ID.RunOwner)
            {
              if (id != Options.ID.Saved && id == Options.ID.Scope)
                goto label_332;
            }
            else
              goto label_330;
          }
          else if (id <= Options.ID.Set)
          {
            if (id <= Options.ID.SCVMMLibraryShare)
            {
              if (id == Options.ID.SCVMMHostGroup || id == Options.ID.SCVMMLibraryShare)
                goto label_330;
              else
                goto label_335;
            }
            else if (id != Options.ID.SCVMMServerName)
            {
              switch (id - 8490)
              {
                case Options.ID.UnknownOption:
                case (Options.ID) 5:
                case (Options.ID) 10:
                case (Options.ID) 11:
                case (Options.ID) 12:
                case (Options.ID) 13:
                case (Options.ID) 14:
                case (Options.ID) 15:
                case (Options.ID) 16:
                  goto label_330;
                case (Options.ID) 17:
                  break;
                default:
                  goto label_335;
              }
            }
            else
              goto label_333;
          }
          else if (id <= Options.ID.Shelveset)
          {
            if (id != Options.ID.SetValues)
            {
              if (id == Options.ID.Shelveset)
                goto label_330;
              else
                goto label_335;
            }
            else
              goto label_333;
          }
          else if (id != Options.ID.ShowAll && id != Options.ID.ShowLabels && id != Options.ID.Silent)
            goto label_335;
        }
        else if (id <= Options.ID.Status)
        {
          if (id <= Options.ID.SqlRsHostName)
          {
            if (id <= Options.ID.SiteName)
            {
              if (id != Options.ID.Site)
              {
                if (id == Options.ID.SiteName)
                  goto label_330;
                else
                  goto label_335;
              }
              else
                goto label_333;
            }
            else if (id != Options.ID.SiteType)
            {
              switch (id - 8900)
              {
                case Options.ID.UnknownOption:
                  break;
                case (Options.ID) 5:
                case (Options.ID) 6:
                case (Options.ID) 7:
                case (Options.ID) 10:
                  goto label_330;
                case (Options.ID) 8:
                  goto label_332;
                default:
                  goto label_335;
              }
            }
            else
              goto label_330;
          }
          else if (id <= Options.ID.StartBranch)
          {
            if (id != Options.ID.Start && id == Options.ID.StartBranch)
              goto label_330;
          }
          else if (id != Options.ID.StartCleanup && (id == Options.ID.State || id == Options.ID.Status))
            goto label_330;
        }
        else if (id <= Options.ID.SwitchUser)
        {
          if (id <= Options.ID.Stop)
          {
            if (id != Options.ID.StepsField)
            {
              if (id != Options.ID.Stop)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id != Options.ID.StopAfter)
          {
            switch (id - 9300)
            {
              case Options.ID.UnknownOption:
              case (Options.ID) 3:
              case (Options.ID) 4:
              case (Options.ID) 6:
              case (Options.ID) 8:
              case (Options.ID) 9:
                goto label_330;
              case (Options.ID) 1:
              case (Options.ID) 2:
              case (Options.ID) 5:
                goto label_335;
              case (Options.ID) 7:
                goto label_332;
              default:
                if (id == Options.ID.SwitchUser)
                  break;
                goto label_335;
            }
          }
          else
            goto label_330;
        }
        else if (id <= Options.ID.Tag)
        {
          if (id == Options.ID.SyncSuite || id == Options.ID.Tag)
            goto label_330;
          else
            goto label_335;
        }
        else if (id != Options.ID.Tags)
        {
          if (id == Options.ID.TargetBranch || (uint) (id - 9400) <= 1U)
            goto label_330;
          else
            goto label_335;
        }
        else
          goto label_332;
      }
      else if (id <= Options.ID.Unmapped)
      {
        if (id <= Options.ID.Title)
        {
          if (id <= Options.ID.TestEnvironment)
          {
            if (id <= Options.ID.TemporaryDataSizeLimit)
            {
              if ((uint) (id - 9450) > 3U)
              {
                if (id == Options.ID.TemporaryDataSizeLimit)
                  goto label_333;
                else
                  goto label_335;
              }
              else
                goto label_330;
            }
            else if (id == Options.ID.Test || id == Options.ID.TestEnvironment)
              goto label_330;
            else
              goto label_335;
          }
          else if (id <= Options.ID.SettingsFile)
          {
            if (id == Options.ID.Template || (uint) (id - 9504) <= 1U)
              goto label_330;
            else
              goto label_335;
          }
          else if (id != Options.ID.Thumbprints)
          {
            if (id == Options.ID.Timeout || id == Options.ID.Title)
              goto label_330;
            else
              goto label_335;
          }
          else
            goto label_332;
        }
        else if (id <= Options.ID.ToVersion)
        {
          if (id <= Options.ID.ToDomain)
          {
            if (id == Options.ID.ToAccount || id == Options.ID.ToDomain)
              goto label_330;
            else
              goto label_335;
          }
          else if (id == Options.ID.ToPool || id == Options.ID.ToVersion)
            goto label_330;
          else
            goto label_335;
        }
        else if (id <= Options.ID.UILevel)
        {
          if (id == Options.ID.Type || id == Options.ID.UILevel)
            goto label_330;
          else
            goto label_335;
        }
        else
        {
          switch (id - 9700)
          {
            case Options.ID.UnknownOption:
              break;
            case (Options.ID) 1:
              goto label_330;
            case (Options.ID) 2:
              goto label_332;
            default:
              if (id == Options.ID.Unload || id == Options.ID.Unmapped)
                break;
              goto label_335;
          }
        }
      }
      else if (id <= Options.ID.UseSqlAlwaysOn)
      {
        if (id <= Options.ID.UpdatePassword)
        {
          if (id <= Options.ID.UpdateFeatureFlags)
          {
            if (id != Options.ID.UpdateComputerName)
            {
              if (id != Options.ID.UpdateFeatureFlags)
                goto label_335;
            }
            else
              goto label_330;
          }
          else if (id != Options.ID.UpdateUserName)
          {
            if (id != Options.ID.UpdatePassword)
              goto label_335;
          }
          else
            goto label_330;
        }
        else if (id <= Options.ID.UpgradeSCVMM)
        {
          if (id != Options.ID.Upgrade)
          {
            if (id != Options.ID.UpgradeSCVMM)
              goto label_335;
          }
          else
            goto label_330;
        }
        else if (id != Options.ID.Url)
        {
          if (id != Options.ID.User)
          {
            if (id != Options.ID.UseSqlAlwaysOn)
              goto label_335;
          }
          else
            goto label_332;
        }
        else
          goto label_330;
      }
      else if (id <= Options.ID.VsixFilePath)
      {
        if (id <= Options.ID.Verify)
        {
          if (id != Options.ID.Validate && id != Options.ID.Verify)
            goto label_335;
        }
        else if (id != Options.ID.Version)
        {
          switch (id - 10400)
          {
            case Options.ID.UnknownOption:
              goto label_332;
            case (Options.ID) 1:
              break;
            case (Options.ID) 2:
              goto label_330;
            default:
              if (id == Options.ID.VsixFilePath)
                goto label_330;
              else
                goto label_335;
          }
        }
        else
          goto label_330;
      }
      else if (id <= Options.ID.Workspace)
      {
        if (id != Options.ID.WholeWord && id == Options.ID.Workspace)
          goto label_330;
      }
      else if (id != Options.ID.Settings)
      {
        if (id == Options.ID.SetIndexing)
          goto label_330;
      }
label_329:
      return Options.Style.NoValue;
label_330:
      return Options.Style.OneValue;
label_332:
      return Options.Style.MultipleValues;
label_333:
      return Options.Style.String;
label_335:
      return Options.Style.NoValue;
    }

    public static Options.Occurrences GetOccurrencesFromID(Options.ID id) => id == Options.ID.Oid || id == Options.ID.OverrideField ? Options.Occurrences.Multiple : Options.Occurrences.Once;

    public enum Value
    {
      AcceptMerge = 100, // 0x00000064
      AcceptTheirs = 200, // 0x000000C8
      AcceptYours = 300, // 0x0000012C
      AcceptYoursRenameTheirs = 400, // 0x00000190
      Active = 500, // 0x000001F4
      AdminConsole = 550, // 0x00000226
      All = 600, // 0x00000258
      AnalysisAdministratorRole = 605, // 0x0000025D
      ApplicationTier = 610, // 0x00000262
      Ascending = 630, // 0x00000276
      ATOnly = 640, // 0x00000280
      AutoMerge = 650, // 0x0000028A
      AutoMergeForced = 655, // 0x0000028F
      Azure = 662, // 0x00000296
      Basic = 675, // 0x000002A3
      Brief = 700, // 0x000002BC
      Build = 750, // 0x000002EE
      Checkin = 800, // 0x00000320
      Checkout = 900, // 0x00000384
      Context = 1000, // 0x000003E8
      Current = 1050, // 0x0000041A
      DeleteConflict = 1100, // 0x0000044C
      Descending = 1150, // 0x0000047E
      Detailed = 1200, // 0x000004B0
      DevFabric = 1250, // 0x000004E2
      Differences = 1300, // 0x00000514
      Different = 1400, // 0x00000578
      Get = 1500, // 0x000005DC
      Global = 1525, // 0x000005F5
      KeepYours = 1550, // 0x0000060E
      KeepYoursRenameTheirs = 1575, // 0x00000627
      Local = 1585, // 0x00000631
      Merge = 1600, // 0x00000640
      Negotiate = 1650, // 0x00000672
      NewServerEssential = 1675, // 0x0000068B
      No = 1700, // 0x000006A4
      None = 1800, // 0x00000708
      Ntlm = 1850, // 0x0000073A
      OAuth = 1863, // 0x00000747
      OI = 1875, // 0x00000753
      OverwriteWritable = 1900, // 0x0000076C
      OverwriteLocal = 2000, // 0x000007D0
      OverwriteDirectory = 2100, // 0x00000834
      Proxy = 2110, // 0x0000083E
      Rcs = 2200, // 0x00000898
      Replace = 2300, // 0x000008FC
      ReportingDataSource = 2310, // 0x00000906
      ReportingTfsContentManagerRole = 2350, // 0x0000092E
      Same = 2400, // 0x00000960
      Server = 2440, // 0x00000988
      ServerDatabasesTfsExecRole = 2450, // 0x00000992
      ServiceIdentity = 2465, // 0x000009A1
      ServiceUsersGroup = 2475, // 0x000009AB
      Shelved = 2500, // 0x000009C4
      Site = 2550, // 0x000009F6
      SourceOnly = 2600, // 0x00000A28
      SPExtensions = 2625, // 0x00000A41
      SPInstall = 2650, // 0x00000A5A
      SS = 2700, // 0x00000A8C
      SS_SideBySide = 2800, // 0x00000AF0
      SS_Unix = 2900, // 0x00000B54
      Standard = 2912, // 0x00000B60
      SystemDatabasesTfsExecRole = 2925, // 0x00000B6D
      Table = 2935, // 0x00000B77
      TakeTheirs = 2950, // 0x00000B86
      TargetOnly = 3000, // 0x00000BB8
      Tfs = 3100, // 0x00000C1C
      Unified = 3200, // 0x00000C80
      Unix = 3300, // 0x00000CE4
      Unlock = 3400, // 0x00000D48
      Upgrade = 3450, // 0x00000D7A
      Visual = 3500, // 0x00000DAC
      Windows = 3600, // 0x00000E10
      Xml = 3650, // 0x00000E42
      Yes = 3700, // 0x00000E74
    }

    public enum Style
    {
      NoValue,
      OneValue,
      TwoValues,
      MultipleValues,
      String,
      NameValuePair,
    }

    public enum Occurrences
    {
      None,
      Once,
      Multiple,
    }

    public enum ProcessModel
    {
      Xml,
      Inheritance,
    }

    public enum ID
    {
      UnknownOption = 0,
      Abort = 25, // 0x00000019
      Adds = 30, // 0x0000001E
      Account = 50, // 0x00000032
      AccountType = 51, // 0x00000033
      Address = 52, // 0x00000034
      AgeInDays = 75, // 0x0000004B
      Add = 99, // 0x00000063
      All = 100, // 0x00000064
      Allow = 200, // 0x000000C8
      AppTierUri = 202, // 0x000000CA
      AreaPath = 210, // 0x000000D2
      ApplyFilters = 214, // 0x000000D6
      AssignFailuresToUser = 220, // 0x000000DC
      Attach = 228, // 0x000000E4
      Attachments = 230, // 0x000000E6
      Attachment = 231, // 0x000000E7
      Auto = 300, // 0x0000012C
      AutoProvision = 310, // 0x00000136
      AutoResolveAll = 315, // 0x0000013B
      AutoSelect = 320, // 0x00000140
      Author = 400, // 0x00000190
      Authority = 450, // 0x000001C2
      Baseless = 500, // 0x000001F4
      BlobId = 510, // 0x000001FE
      Branch = 525, // 0x0000020D
      Build = 550, // 0x00000226
      BuildAgent = 600, // 0x00000258
      BuildController = 601, // 0x00000259
      BuildDefinition = 700, // 0x000002BC
      BuildDefinitionUri = 800, // 0x00000320
      BuildDir = 801, // 0x00000321
      BuildProcess = 850, // 0x00000352
      BuildUri = 900, // 0x00000384
      BuiltInIdentity = 901, // 0x00000385
      BuildVerification = 925, // 0x0000039D
      Bypass = 945, // 0x000003B1
      Candidate = 1000, // 0x000003E8
      Category = 1025, // 0x00000401
      Change = 1050, // 0x0000041A
      Changeset = 1100, // 0x0000044C
      Changesets = 1101, // 0x0000044D
      Checkin = 1150, // 0x0000047E
      Child = 1200, // 0x000004B0
      ClientCertificate = 1201, // 0x000004B1
      Clean = 1275, // 0x000004FB
      Cloak = 1300, // 0x00000514
      Clone = 1305, // 0x00000519
      CloneChildren = 1310, // 0x0000051E
      CloneRequirements = 1315, // 0x00000523
      CollectionDb = 1325, // 0x0000052D
      Collection = 1333, // 0x00000535
      CollectionId = 1335, // 0x00000537
      CollectionName = 1366, // 0x00000556
      Comment = 1400, // 0x00000578
      CommitId = 1450, // 0x000005AA
      Computer = 1500, // 0x000005DC
      ConfigurationDb = 1550, // 0x0000060E
      ConfigDBOnly = 1580, // 0x0000062C
      ConfigurationFolder = 1600, // 0x00000640
      Configure = 1700, // 0x000006A4
      ConfigId = 1701, // 0x000006A5
      ConfigName = 1702, // 0x000006A6
      ConfigureConnections = 1703, // 0x000006A7
      Confirmed = 1707, // 0x000006AB
      ConnectionString = 1710, // 0x000006AE
      Conservative = 1755, // 0x000006DB
      Console = 1800, // 0x00000708
      Continue = 1822, // 0x0000071E
      ContinueOnError = 1825, // 0x00000721
      Controller = 1850, // 0x0000073A
      ConvertToType = 1900, // 0x0000076C
      Create = 1950, // 0x0000079E
      CustomGetVersion = 2000, // 0x000007D0
      DatabaseName = 2001, // 0x000007D1
      DatabasePrefix = 2002, // 0x000007D2
      DatabaseAction = 2005, // 0x000007D5
      Date = 2007, // 0x000007D7
      DateRange = 2010, // 0x000007DA
      Days = 2050, // 0x00000802
      Debug = 2100, // 0x00000834
      Decloak = 2200, // 0x00000898
      Default = 2250, // 0x000008CA
      DefaultCollection = 2260, // 0x000008D4
      DefaultMailAddress = 2280, // 0x000008E8
      Delete = 2300, // 0x000008FC
      DeleteAll = 2310, // 0x00000906
      Deleted = 2400, // 0x00000960
      DeliveryType = 2401, // 0x00000961
      DeleteOptions = 2402, // 0x00000962
      Deletes = 2405, // 0x00000965
      DeleteValues = 2415, // 0x0000096F
      Deny = 2500, // 0x000009C4
      Description = 2600, // 0x00000A28
      DestinationSuiteId = 2625, // 0x00000A41
      DestinationTeamProject = 2626, // 0x00000A42
      DestinationWorkItemType = 2627, // 0x00000A43
      DestinationPlanName = 2628, // 0x00000A44
      DestroyCodeIndex = 2640, // 0x00000A50
      Detach = 2650, // 0x00000A5A
      Detect = 2675, // 0x00000A73
      Diff = 2685, // 0x00000A7D
      Directory = 2700, // 0x00000A8C
      Disable = 2780, // 0x00000ADC
      Discard = 2800, // 0x00000AF0
      Display = 2900, // 0x00000B54
      Dns = 2910, // 0x00000B5E
      DnsSuffix = 2920, // 0x00000B68
      Downgrade = 2950, // 0x00000B86
      Download = 2955, // 0x00000B8B
      DropExistingDatabases = 2978, // 0x00000BA2
      DropLocation = 3000, // 0x00000BB8
      DumpLog = 3070, // 0x00000BFE
      Edit = 3100, // 0x00000C1C
      EmailAddress = 3101, // 0x00000C1D
      Enable = 3199, // 0x00000C7F
      Enabled = 3200, // 0x00000C80
      Execute = 3201, // 0x00000C81
      EventType = 3202, // 0x00000C82
      EventFilter = 3203, // 0x00000C83
      Encoding = 3210, // 0x00000C8A
      Exclude = 3215, // 0x00000C8F
      ExitCode = 3300, // 0x00000CE4
      ExpectedResultsField = 3315, // 0x00000CF3
      Export = 3325, // 0x00000CFD
      Extend = 3330, // 0x00000D02
      Extended = 3350, // 0x00000D16
      External = 3380, // 0x00000D34
      FeatureFlag = 3390, // 0x00000D3E
      File = 3400, // 0x00000D48
      FileCount = 3420, // 0x00000D5C
      FileTime = 3450, // 0x00000D7A
      Filter = 3500, // 0x00000DAC
      FilterLocalPathsOnly = 3600, // 0x00000E10
      Flavor = 3610, // 0x00000E1A
      Folders = 3700, // 0x00000E74
      Force = 3800, // 0x00000ED8
      Format = 3900, // 0x00000F3C
      FromDomain = 3930, // 0x00000F5A
      FromEmailAddress = 3940, // 0x00000F64
      FromPool = 3942, // 0x00000F66
      FromSCE = 3945, // 0x00000F69
      FromSE = 3950, // 0x00000F6E
      GetKeyFingerprint = 3975, // 0x00000F87
      GetOption = 4000, // 0x00000FA0
      Global = 4100, // 0x00001004
      Group = 4200, // 0x00001068
      HostGroup = 4290, // 0x000010C2
      Help = 4300, // 0x000010CC
      Ignore = 4301, // 0x000010CD
      Id = 4302, // 0x000010CE
      IgnoreCase = 4400, // 0x00001130
      IgnoreEol = 4500, // 0x00001194
      IgnoreExistingIISArtifacts = 4501, // 0x00001195
      IgnoreList = 4550, // 0x000011C6
      IgnoreSpace = 4600, // 0x000011F8
      Impersonate = 4625, // 0x00001211
      Import = 4650, // 0x0000122A
      Include = 4675, // 0x00001243
      IndexHistoryPeriod = 4680, // 0x00001248
      IndexingStatus = 4685, // 0x0000124D
      Inherit = 4700, // 0x0000125C
      Inputs = 4701, // 0x0000125D
      Install = 4702, // 0x0000125E
      InternalUrl = 4703, // 0x0000125F
      IP = 4715, // 0x0000126B
      IPBlock = 4720, // 0x00001270
      ItemMode = 4750, // 0x0000128E
      KeepHistory = 4800, // 0x000012C0
      KeepMergeHistory = 4801, // 0x000012C1
      Keywords = 4810, // 0x000012CA
      LabEnvironment = 4825, // 0x000012D9
      LabEnvironmentIds = 4827, // 0x000012DB
      LabEnvironmentPlacementPolicy = 4830, // 0x000012DE
      LabTemplate = 4860, // 0x000012FC
      LabUrl = 4868, // 0x00001304
      Latest = 4900, // 0x00001324
      LCID = 4925, // 0x0000133D
      LibraryShare = 4965, // 0x00001365
      List = 4975, // 0x0000136F
      ListLargeFiles = 4980, // 0x00001374
      ListSCVMMHostGroups = 4990, // 0x0000137E
      ListSCVMMLibraryShares = 4995, // 0x00001383
      Load = 4998, // 0x00001386
      Location = 4999, // 0x00001387
      Lock = 5000, // 0x00001388
      Log = 5025, // 0x000013A1
      LogFile = 5075, // 0x000013D3
      Login = 5100, // 0x000013EC
      LogToConsole = 5110, // 0x000013F6
      Machine = 5118, // 0x000013FE
      MakeDefault = 5125, // 0x00001405
      LoginType = 5130, // 0x0000140A
      Manifest = 5175, // 0x00001437
      Map = 5200, // 0x00001450
      MappingFile = 5225, // 0x00001469
      MaxBuilds = 5250, // 0x00001482
      MaxPriority = 5275, // 0x0000149B
      MaxProcesses = 5300, // 0x000014B4
      MigrateDeploymentGroups = 5325, // 0x000014CD
      MinPriority = 5350, // 0x000014E6
      MinSize = 5380, // 0x00001504
      Move = 5400, // 0x00001518
      MSBuildArguments = 5500, // 0x0000157C
      MsiPatchRemovalList = 5550, // 0x000015AE
      Name = 5600, // 0x000015E0
      NetworkLocation = 5650, // 0x00001612
      New = 5700, // 0x00001644
      NewName = 5800, // 0x000016A8
      NewOwner = 5815, // 0x000016B7
      NewWITName = 5850, // 0x000016DA
      NoAutoResolve = 5860, // 0x000016E4
      NoConflictsCheckForGated = 5867, // 0x000016EB
      NoDetect = 5875, // 0x000016F3
      NoGet = 5900, // 0x0000170C
      NoIgnore = 5950, // 0x0000173E
      NoImplicitBaseless = 6000, // 0x00001770
      NoMerge = 6050, // 0x000017A2
      NoPrompt = 6100, // 0x000017D4
      NoRegex = 6200, // 0x00001838
      NoSummary = 6300, // 0x0000189C
      Notes = 6400, // 0x00001900
      Number = 6500, // 0x00001964
      Offline = 6550, // 0x00001996
      Oid = 6575, // 0x000019AF
      Options = 6600, // 0x000019C8
      Output = 6700, // 0x00001A2C
      Override = 6800, // 0x00001A90
      OverrideFieldName = 6830, // 0x00001AAE
      OverrideFieldValue = 6870, // 0x00001AD6
      OverrideField = 6880, // 0x00001AE0
      OverrideMailAddress = 6885, // 0x00001AE5
      OverrideType = 6900, // 0x00001AF4
      Owner = 7000, // 0x00001B58
      Overwrite = 7100, // 0x00001BBC
      PassName = 7101, // 0x00001BBD
      Password = 7102, // 0x00001BBE
      PageCompression = 7103, // 0x00001BBF
      PassiveAuthEnabled = 7104, // 0x00001BC0
      Patch = 7105, // 0x00001BC1
      PatchClassesAssembly = 7107, // 0x00001BC3
      Path = 7110, // 0x00001BC6
      PlanId = 7112, // 0x00001BC8
      PlanName = 7113, // 0x00001BC9
      Permission = 7125, // 0x00001BD5
      PersonalAccessTokenFile = 7140, // 0x00001BE4
      Platform = 7150, // 0x00001BEE
      Port = 7200, // 0x00001C20
      Postpone = 7300, // 0x00001C84
      Preview = 7400, // 0x00001CE8
      Priority = 7500, // 0x00001D4C
      ProcessModel = 7515, // 0x00001D5B
      ProductKey = 7525, // 0x00001D65
      ProjectCollectionsOnly = 7540, // 0x00001D74
      Promote = 7545, // 0x00001D79
      Prompt = 7550, // 0x00001D7E
      Properties = 7572, // 0x00001D94
      Property = 7575, // 0x00001D97
      PropertyNames = 7580, // 0x00001D9C
      Provider = 7600, // 0x00001DB0
      Proxy = 7625, // 0x00001DC9
      PublicAppTierUri = 7640, // 0x00001DD8
      PublicUrl = 7645, // 0x00001DDD
      Publish = 7675, // 0x00001DFB
      Queue = 7700, // 0x00001E14
      Quiet = 7701, // 0x00001E15
      QueryText = 7750, // 0x00001E46
      AnalysisServices = 7790, // 0x00001E6E
      ReApply = 7795, // 0x00001E73
      RebuildNow = 7798, // 0x00001E76
      Recursive = 7800, // 0x00001E78
      RedirectDns = 7801, // 0x00001E79
      RedirectOther = 7802, // 0x00001E7A
      RegenerateKey = 7803, // 0x00001E7B
      ReindexAll = 7806, // 0x00001E7E
      Relationships = 7813, // 0x00001E85
      RemoteDbServerName = 7825, // 0x00001E91
      Remap = 7875, // 0x00001EC3
      Remove = 7900, // 0x00001EDC
      RemoveComponents = 7910, // 0x00001EE6
      RepairEnvironments = 7990, // 0x00001F36
      Replace = 8000, // 0x00001F40
      Repository = 8050, // 0x00001F72
      RepoUri = 8075, // 0x00001F8B
      RequestedFor = 8100, // 0x00001FA4
      Reset = 8102, // 0x00001FA6
      ResetOwner = 8103, // 0x00001FA7
      ResolveResolution = 8104, // 0x00001FA8
      Resource = 8105, // 0x00001FA9
      ResourceDirectory = 8110, // 0x00001FAE
      ResourceFile = 8120, // 0x00001FB8
      Resume = 8200, // 0x00002008
      ResultsFile = 8201, // 0x00002009
      ResultOwner = 8202, // 0x0000200A
      RetentionPolicies = 8300, // 0x0000206C
      RunId = 8301, // 0x0000206D
      Retry = 8315, // 0x0000207B
      RunOwner = 8350, // 0x0000209E
      Saved = 8400, // 0x000020D0
      Scope = 8440, // 0x000020F8
      SCVMMHostGroup = 8460, // 0x0000210C
      SCVMMLibraryShare = 8465, // 0x00002111
      SCVMMServerName = 8480, // 0x00002120
      ServerUrl = 8490, // 0x0000212A
      ServerId = 8495, // 0x0000212F
      Server = 8500, // 0x00002134
      Service = 8501, // 0x00002135
      SettingFile = 8502, // 0x00002136
      SharePointAdminUri = 8503, // 0x00002137
      SharePointSitesUri = 8504, // 0x00002138
      SharePointUnc = 8505, // 0x00002139
      SharePointUri = 8506, // 0x0000213A
      Set = 8507, // 0x0000213B
      SetValues = 8520, // 0x00002148
      Shelveset = 8600, // 0x00002198
      ShowAll = 8650, // 0x000021CA
      ShowLabels = 8700, // 0x000021FC
      Silent = 8800, // 0x00002260
      Site = 8850, // 0x00002292
      SiteName = 8855, // 0x00002297
      SiteType = 8870, // 0x000022A6
      SlotMode = 8900, // 0x000022C4
      SmtpCertThumbprint = 8902, // 0x000022C6
      SmtpEnableSsl = 8903, // 0x000022C7
      SmtpHost = 8905, // 0x000022C9
      Sort = 8906, // 0x000022CA
      SqlInstance = 8907, // 0x000022CB
      SqlInstances = 8908, // 0x000022CC
      SqlRsHostName = 8910, // 0x000022CE
      SmtpPassword = 8911, // 0x000022CF
      SmtpPort = 8912, // 0x000022D0
      SmtpUser = 8913, // 0x000022D1
      Start = 8940, // 0x000022EC
      StartBranch = 8950, // 0x000022F6
      StartCleanup = 9000, // 0x00002328
      State = 9050, // 0x0000235A
      Status = 9100, // 0x0000238C
      StepsField = 9150, // 0x000023BE
      Stop = 9190, // 0x000023E6
      StopAfter = 9200, // 0x000023F0
      StopAt = 9300, // 0x00002454
      Storage = 9303, // 0x00002457
      SubscriptionId = 9304, // 0x00002458
      SuiteId = 9306, // 0x0000245A
      SourceSuiteIds = 9307, // 0x0000245B
      SourcePlanId = 9308, // 0x0000245C
      SuiteName = 9309, // 0x0000245D
      SwitchUser = 9335, // 0x00002477
      SyncSuite = 9340, // 0x0000247C
      Tag = 9349, // 0x00002485
      Tags = 9350, // 0x00002486
      TargetBranch = 9375, // 0x0000249F
      TeamProject = 9400, // 0x000024B8
      TeamProjects = 9401, // 0x000024B9
      TeamProjectCollectionHostGroup = 9450, // 0x000024EA
      TeamProjectCollectionLibraryShare = 9451, // 0x000024EB
      TeamProjectHostGroup = 9452, // 0x000024EC
      TeamProjectLibraryShare = 9453, // 0x000024ED
      TemporaryDataSizeLimit = 9461, // 0x000024F5
      Test = 9470, // 0x000024FE
      TestEnvironment = 9490, // 0x00002512
      Template = 9500, // 0x0000251C
      SettingsName = 9504, // 0x00002520
      SettingsFile = 9505, // 0x00002521
      Thumbprints = 9520, // 0x00002530
      Timeout = 9540, // 0x00002544
      Title = 9560, // 0x00002558
      ToAccount = 9570, // 0x00002562
      ToDomain = 9574, // 0x00002566
      ToPool = 9577, // 0x00002569
      ToVersion = 9580, // 0x0000256C
      Type = 9600, // 0x00002580
      UILevel = 9670, // 0x000025C6
      Unmap = 9700, // 0x000025E4
      UnattendFile = 9701, // 0x000025E5
      Uninstall = 9702, // 0x000025E6
      Unload = 9750, // 0x00002616
      Unmapped = 9755, // 0x0000261B
      UpdateComputerName = 9800, // 0x00002648
      Update = 9850, // 0x0000267A
      UpdateFeatureFlags = 9875, // 0x00002693
      UpdateLocal = 9900, // 0x000026AC
      UpdateUserName = 10000, // 0x00002710
      UpdatePassword = 10005, // 0x00002715
      Upgrade = 10010, // 0x0000271A
      UpgradeSCVMM = 10015, // 0x0000271F
      Upload = 10017, // 0x00002721
      UpgradeTestPlans = 10018, // 0x00002722
      UpgradeStatus = 10019, // 0x00002723
      Url = 10020, // 0x00002724
      User = 10100, // 0x00002774
      UseSqlAlwaysOn = 10150, // 0x000027A6
      Validate = 10200, // 0x000027D8
      Verify = 10285, // 0x0000282D
      Version = 10300, // 0x0000283C
      View = 10400, // 0x000028A0
      ViewAll = 10401, // 0x000028A1
      VirtualDirectory = 10402, // 0x000028A2
      VsixFilePath = 10450, // 0x000028D2
      WholeWord = 10500, // 0x00002904
      Workspace = 10600, // 0x00002968
      Settings = 10800, // 0x00002A30
      SetIndexing = 10850, // 0x00002A62
      Shallow = 10900, // 0x00002A94
    }
  }
}
