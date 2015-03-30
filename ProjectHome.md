A simple ASP.NET content rotator control that can be declared in your ASP.NET HTML markup.

```
<%@ Register Assembly="YTech.WebControls.ContentRotator"
Namespace="YTech.WebControls.ContentRotator" TagPrefix="CR" %>

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
```