using Microsoft.AspNetCore.Components;

namespace BlazorApp.BlazorExtensions
{
	class CallbackManager
	{
		private Dictionary<string, object> Callbacks { get; set; }
		public void ProcessParameterView(ref ParameterView parameters)
		{
			var parametersMap = parameters.ToDictionary();
			var callbacks = parameters
				.ToDictionary()
				.Where(x => x.Key.EndsWith("Changed") && x.Value?.GetType().Name.StartsWith(nameof(EventCallback)) == true)
				.ToDictionary(x => x.Key, x => x.Value);

			if (callbacks.Count > 0)
			{
				Callbacks = callbacks;
				var parametersWithoutCallbacks = parametersMap.Except(callbacks).ToDictionary(static x => x.Key, static x => x.Value);
				parameters = ParameterView.FromDictionary(parametersWithoutCallbacks);
			}
		}
		public bool TryInvokeCallback<T>(T newValue, string propertyName)
		{
			if (Callbacks?.TryGetValue($"{propertyName}Changed", out var callback) == true)
			{
				((EventCallback<T>)callback).InvokeAsync(newValue);
				return true;
			}
			return false;
		}
	}
}
