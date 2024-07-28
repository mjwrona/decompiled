<div class="queue-build">
    <table class="filter">
        <tr>
            <td colspan="2">
                <label for="definition">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildDefinitionTitle %></label>
                <select id="definition" class="definition">
                </select>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <label for="source">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuild %></label>
                <select id="source" class="source">
                    <option value="latest" selected="selected"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuildLatest %></option>
                    <option value="latest-with-shelveset"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildWhatToBuildLatestWithShelveset %></option>
                </select>
            </td>
        </tr>
        <tr class="shelveset-picker-container">
            <td>
                <label for="source">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildShelvesetTitle %></label>
                <input type="text" id="shelveset" class="shelveset" />
            </td>
            <td style="width:1%"><button class="browse">...</button></td>
        </tr>
        <tr class="check-in-container">
            <td colspan="2">
                <input type="checkbox" id="check-in" class="check-in" />
                <label for="check-in" class="check-in">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildCheckinChanges %></label>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <label for="controller">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildControllerTitle %></label>
                <select id="controller" class="controller">
                </select>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <label for="priority">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildPriorityTitle %></label>
                <select id="priority" class="priority">
                    <option value="high"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityHigh %></option>
                    <option value="abovenormal"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityAboveNormal %></option>
                    <option value="normal" selected="selected"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityNormal %></option>
                    <option value="belownormal"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityBelowNormal %></option>
                    <option value="low"><%: Microsoft.TeamFoundation.Server.WebAccess.Build.Common.BuildCommonResources.QueueBuildPriorityLow %></option>
                </select>
            </td>
        </tr>
         <tr>
            <td colspan="2">
                <label for="drop-folder">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildDropLocationTitle %></label>
                <input type="text" id="drop-folder" class="drop-folder"/>
            </td>
        </tr>
         <tr>
            <td colspan="2">
                <label for="msbuild-args">
                    <%: Microsoft.TeamFoundation.Server.WebAccess.Build.BuildServerResources.QueueBuildMSBuildArgsTitle %></label>
                <input type="text" id="msbuild-args" class="msbuild-args"/>
            </td>
        </tr>
    </table>
</div>
