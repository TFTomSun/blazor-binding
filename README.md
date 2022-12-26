# Demo 
This app demonstrates how to automatically handle binding callbacks without having them to be explicitly defined. The [Counter](https://github.com/TFTomSun/blazor-binding/blob/master/BlazorApp/Pages/Counter.razor) 
component in the project derrives from [NotifyComponent](https://github.com/TFTomSun/blazor-binding/blob/master/BlazorApp/BlazorExtensions/NotifyComponent.cs) which uses the [CallbackManager](https://github.com/TFTomSun/blazor-binding/blob/master/BlazorApp/BlazorExtensions/CallbackManager.cs) which handles the callback parameter filtering and invocation underneath.
