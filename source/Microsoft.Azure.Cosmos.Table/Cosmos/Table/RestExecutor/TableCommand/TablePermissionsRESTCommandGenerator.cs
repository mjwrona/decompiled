// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand.TablePermissionsRESTCommandGenerator
// Assembly: Microsoft.Azure.Cosmos.Table, Version=1.0.7.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 461D0B3A-0B96-4D42-B330-3A8E714FC39A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Table.dll

using Microsoft.Azure.Cosmos.Table.RestExecutor.Common;
using Microsoft.Azure.Cosmos.Table.RestExecutor.Common.Auth;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Table.RestExecutor.TableCommand
{
  internal static class TablePermissionsRESTCommandGenerator
  {
    internal static RESTCommand<TablePermissions> GetAclImpl(
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<TablePermissions> cmd1 = new RESTCommand<TablePermissions>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<TablePermissions>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.CommandLocationMode = CommandLocationMode.PrimaryOrSecondary;
      cmd1.BuildRequest = (Func<RESTCommand<TablePermissions>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, uriBuilder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTablePermissions(NavigationHelper.AppendPathToSingleUri(uri, table.Name), uriBuilder, timeout, HttpMethod.Get, (TablePermissions) null, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.PreProcessResponse = (Func<RESTCommand<TablePermissions>, HttpResponseMessage, Exception, OperationContext, TablePermissions>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<TablePermissions>(HttpStatusCode.OK, resp, (TablePermissions) null, cmd, ex));
      cmd1.PostProcessResponseAsync = (Func<RESTCommand<TablePermissions>, HttpResponseMessage, OperationContext, CancellationToken, Task<TablePermissions>>) ((cmd, resp, ctx, token) => TableOperationHttpResponseParsers.ParseGetAclAsync(cmd, resp, ctx));
      return cmd1;
    }

    internal static RESTCommand<NullType> SetAclImpl(
      TablePermissions permissions,
      CloudTableClient client,
      CloudTable table,
      TableRequestOptions requestOptions)
    {
      RESTCommand<NullType> cmd1 = new RESTCommand<NullType>(client.Credentials, client.StorageUri);
      RESTCommandGeneratorUtils.ApplyTableRequestOptionsToStorageCommand<NullType>(requestOptions, cmd1);
      cmd1.HttpClient = client.HttpClient;
      cmd1.BuildRequest = (Func<RESTCommand<NullType>, Uri, UriQueryBuilder, HttpContent, int?, OperationContext, StorageRequestMessage>) ((cmd, uri, uriBuilder, httpContent, timeout, ctx) => TableRequestMessageFactory.BuildStorageRequestMessageForTablePermissions(NavigationHelper.AppendPathToSingleUri(uri, table.Name), uriBuilder, timeout, HttpMethod.Put, permissions, (ICanonicalizer) SharedKeyCanonicalizer.Instance, client.Credentials, ctx, requestOptions));
      cmd1.ParseErrorAsync = new Func<Stream, HttpResponseMessage, string, CancellationToken, Task<StorageExtendedErrorInformation>>(StorageExtendedErrorInformationRestHelper.ReadExtendedErrorInfoFromStreamAsync);
      cmd1.PreProcessResponse = (Func<RESTCommand<NullType>, HttpResponseMessage, Exception, OperationContext, NullType>) ((cmd, resp, ex, ctx) => HttpResponseParsers.ProcessExpectedStatusCodeNoException<NullType>(HttpStatusCode.NoContent, resp, NullType.Value, cmd, ex));
      return cmd1;
    }
  }
}
