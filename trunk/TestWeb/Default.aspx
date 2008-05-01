<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<%@ Register Assembly="YTech.WebControls.ContentRotator" Namespace="YTech.WebControls.ContentRotator" TagPrefix="CR" %>
	
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title>Content Rotator Tester</title>
</head>
<body>
	<form runat="server">
	<div>
		<h2>Random (Default)</h2>
		<CR:ServerContentRotator runat="server" Key="Rotator1">
			<CR:ContentPanel runat="server" Impressions="50" Key="Content1">
				Content 1
			</CR:ContentPanel>
			<CR:ContentPanel runat="server" Impressions="50" Key="Content2">
				Content 2
			</CR:ContentPanel>
		</CR:ServerContentRotator>
		
		<h2>Always Same</h2>
		<CR:ServerContentRotator runat="server" Key="Rotator2" RotationMode="AlwaysSame">
			<CR:ContentPanel runat="server" Impressions="50" Key="Content1">
				Content 1
			</CR:ContentPanel>
			<CR:ContentPanel runat="server" Impressions="50" Key="Content2">
				Content 2
			</CR:ContentPanel>
		</CR:ServerContentRotator>
		
		<h2>Always Different</h2>
		<CR:ServerContentRotator runat="server" Key="Rotator3" RotationMode="AlwaysDifferent">
			<CR:ContentPanel runat="server" Impressions="50" Key="Content1">
				Content 1
			</CR:ContentPanel>
			<CR:ContentPanel runat="server" Impressions="50" Key="Content2">
				Content 2
			</CR:ContentPanel>
		</CR:ServerContentRotator>
	</div>
	</form>
</body>
</html>
