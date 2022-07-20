<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Framework_Web._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    
    <div class="jumbotron">
        <h1>51Degrees</h1>
        <p class="lead">51Degrees device detection can be used to deliver detailed insights into the device, operating system and browser being used to access your website.</p>
        <p><a href="https://51degrees.com" class="btn btn-primary btn-lg">Learn more &raquo;</a></p>
    </div>

    <div class="row">
        <div class="col-12">
            <h2>ASP.NET integration</h2>
            
            <p>This example demonstrates the use of the device detection API as part of an ASP.NET website:</p>
    
            <p>Is the visitor using a mobile device? <%= Request.Browser.IsMobileDevice ? "Yes" : "No" %></p>
            <p></p>
            <p><strong>Browser Details:</strong> <%= Request.Browser["BrowserVendor"] %> <%= Request.Browser["BrowserName"] %> <%= Request.Browser["BrowserVersion"] %></p>
            
            <p><strong>Evidence:</strong></p>
            <% foreach (var evidence in ((FiftyOne.Pipeline.Web.Framework.Providers.PipelineCapabilities)Request.Browser).FlowData.GetEvidence().AsDictionary()) { %>
                <p><%= evidence.Key %> - <%= evidence.Value %></p>
            <% } %>
        </div>
    </div>

</asp:Content>
