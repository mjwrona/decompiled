<%@ Page Title="" Language="C#" Inherits="System.Web.Mvc.ViewPage<Microsoft.TeamFoundation.Server.WebAccess.DataImportStatusModel>" %>

<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Controls" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Html" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Platform" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Presentation" %>
<%@ Import Namespace="Microsoft.TeamFoundation.Server.WebAccess.Routing" %>

<!DOCTYPE html>
<html>
<head>
    <title><%:Model.Title %></title>
    <style type="text/css">
        body {
            font-family: "-apple-system",BlinkMacSystemFont,"Segoe UI",Roboto,"Helvetica Neue",Helvetica,Ubuntu,Arial,sans-serif,"Apple Color Emoji","Segoe UI Emoji","Segoe UI Symbol";
            font-size: 14px;
            height: 100%;
            margin: 0;
        }

        .header {
            height: 48px;
            font-size: 14px;
            display: flex;
            flex-direction: row;
            align-items: center;
        }
        
        .header .logo {
            width: 48px;
            text-align: center;
        }

        .header .logo img {
            height: 22px;
            margin-top: 4px;
        }

        .header .product-name {
            font-size: 14px;
            font-weight: bold;
            color: #0078d4;
        }

        h1 {
            font-weight: 300;
            font-size: 48px;
        }

        b {
            font-weight: 600;
        }

        .content {
            text-align: center;
            margin-top: 40px;
        }

        .step, table {
            width: 500px;
            margin: auto;
        }

        table {
            margin-top: 40px;
            margin-bottom: 40px;
        }

        th, td {
            padding: 4px;
        }

        .step h2 {
            font-weight: 300;
            font-size: 24px;
        }

        .step p {
            font-weight: 400;
            font-size: 14px;
        }

        .update-time {
            font-weight: 600;
            font-size: 14px;
        }

        .progress-bar {
            width: 100%;
            height: 14px;
            border: 1px solid #4A76C6;
        }

        .progress-bar-loading {
            height: 100%;
            background: repeating-linear-gradient(
                -45deg,
                #4A76C6,
                #4A76C6 2px,
                transparent 2px,
                transparent 6px
           );
        }

        .progress-bar-complete {
            height: 100%;
            background-color: #70AD47;
        }

        .no-wrap {
            white-space: nowrap;
        }
    </style>
</head>
<body>
    <div class="header">
        <span class="logo">
            <img src="data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACwAAAAsCAIAAACR5s1WAAAABGdBTUEAALGPC/xhBQAAAAFzUkdCAK7OHOkAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAYdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjEuNWRHWFIAAAMgSURBVFjD7Zj5TxNREMf5fwTRqPGCxBgTPIKJ/mC8fjIhUSOJRk2EAkYuj0IIhoAxiigiWA61REnAI0FB5BCj2G63B223XVvosT3o0u6Bg2vK0bK73RbXHzqZn152O5++mTf7vpOxIMlsnkiV2uEn6YVUWIa016zuyIYibW4V+mKcYOWF4PxUk1mLkzJDgGcVayE7vhAtJwTnuZV/ssPKCsH5ySazxk6uL0QozAwigZIufC0ILjuVrxyE6OxkiD+T7Z8951qwLWUIT/jlnlOJ9owTDJscRIRmx0xzNX3O/DqjyMCxfrxxRXZohsXckWF98OmQB2r5bAuWpzTEgZgNUFBfl9ps22/oJMfmPLNYe6jWeE2Fv5wgCluxw3WmjYo4G7kEAdv2aNB9rGE6M7nAe2/pId79Dy74u36S4X68dcjD88oSBKRcWtSd5WjBQ2t9/8w7TcDpo+JmVhTEN0toU4nYittapoMuebPX0Tvps7jCYhqDMIQnSO+7beB5KFuBHL1rKu3GO0e90KEpOuF+JAAB5VrQbI096PuVhsvP7C0f3RPTITLCJPmJEoBoGJhdtQTl6Z1LzTdaLAScolVLUNULqTYBiNilNEQaIg2RhkhD/McQO2LucDJAwGe6Y8R7pN4kJ0T0ObhYwwUiuwSRE+LvVdtP9X33NQ+6u0a9b3/6Jy0h00zYFaBY9h9CcBorrtDYXYEeqDGevmeGy7SiE6954wTW7jFiYGqRdXp2kTVlEGDIr/nNpYi0yzcIr4O1RrgJF7baQC0qXy+ygopJGAJMNepNUvmIdz4ZeLXDvt7hYb/VXwk+iOA8Axu7fgR7qvU/MFJYlSP4iuLYVY5uu65LCcGJRnNUrgmPBlRflorj/GNsLsyAXD7zwJKlkE5QpMLDFJvYfOJKuz0KEV10+ChQvfl1poTCg5gDQSVlSALFAR1iFUTUpmxktdqRU4EKEkA2P6FB6ZMajZ2E4ogLwRnFsO81gYtttrUaDDRAqyuc7MyqbdjDAxE1IkQ/H/FCs1pOcOEJBtuZgsEZFFLPOJHQhA+Ebt4dQ33/DP/k6jf7AQX/v/oFqgAAAABJRU5ErkJggg==" />
        </span>
        <span class="product-name"><%: PlatformResources.ProductName %></span>
    </div>
    <div class="content">
        <img id='Image' src="<%: Model.Image %>" />
        <h1>Importing</h1>
        <img id='Loader' src="<%: Model.LoadingImage %>"/>
        <div class="step">
            <h2><b id='Step-Title'><%: Model.StepTitle %></b> <span id='Step-Counter'><%: Model.StepCounter %></span></h2>
            <p id='Step-Description'><%: Model.StepDescription %></p>
        </div>
        <% if(Model.FileTransferProgress != null && Model.FileTransferProgress.Count > 0) { %>
        <table>
            <colgroup>
                <col width="33%" />
                <col width="67%" />
                <col width="0%" />
                <col width="0%" />
            </colgroup>
            <tbody>
                <% foreach(var ftProgress in Model.FileTransferProgress) { %>
                <tr>
                    <td><%: ftProgress.ServiceName %></td>
                    <td>
                        <div class="progress-bar">
                            <% if (ftProgress.Progress >= 100) { %>
                            <div class="progress-bar-complete" style="width:<%: ftProgress.Progress %>%;"></div>
                            <% } else { %>
                            <div class="progress-bar-loading" style="width:<%: ftProgress.Progress %>%;"></div>
                            <% } %>
                        </div>
                    </td>
                    <td><%: System.Math.Round(ftProgress.Progress, ftProgress.Progress >= 100 ? 0 : 1) %>%</td>
                    <td class="no-wrap"><%: ftProgress.Transferred %>/<%: ftProgress.Total %></td>
                </tr>
                <% } %>
            </tbody>
        </table>
        <% } %>

        <div class="update-time" id="Last-Progress-Date-Container">
            <p><b><%: PlatformResources.DataImport_LastUpdatedMessage %> <span id="Last-Progress-Date"></span></b></p>
        </div>
    </div>

    <script type="text/javascript"<%= Html.GenerateNonce(true) %>>
        var updateDelay = 60 * 1000;

        var LastProgressDate = function () {
            var lastUpdateProgress = "<%: Model.LastUpdate %>";
            if (lastUpdateProgress) {
                var date = new Date(lastUpdateProgress);
                document.getElementById("Last-Progress-Date").innerHTML = date.toLocaleString();
            } else {
                document.getElementById("Last-Progress-Date-Container").remove();
            }
        }

        LastProgressDate();
        setTimeout(function() { location.replace(location.origin + location.pathname); }, updateDelay);
    </script>
</body>
</html>


