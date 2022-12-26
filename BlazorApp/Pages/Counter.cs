using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BlazorApp.Pages
{
	class CallbackManager
	{
		private Dictionary<string, object> Callbacks { get; set; }
		public void ProcessParameterView(ref ParameterView parameters) {
			var parametersMap = parameters.ToDictionary();
			var callbacks = parameters
				.ToDictionary()
				.Where(x => x.Key.EndsWith("Changed") && x.Value?.GetType().Name.StartsWith(nameof(EventCallback)) == true)
				.ToDictionary(x => x.Key, x => x.Value);

			if (callbacks.Count > 0)
			{
				this.Callbacks = callbacks;
				var parametersWithoutCallbacks = parametersMap.Except(callbacks).ToDictionary(static x => x.Key, static x => x.Value);
			    parameters =ParameterView.FromDictionary(parametersWithoutCallbacks);
			}
		}
		public bool TryInvokeCallback<T>(T newValue, string propertyName)
		{
			if (this.Callbacks?.TryGetValue($"{propertyName}Changed", out var callback) == true)
			{
				((EventCallback<T>)callback).InvokeAsync(newValue);
				return true;
			}
			return false;
		}
	}
	partial class Counter
	{
		private ConcurrentDictionary<string, object?> PropertyMap { get; } = new();


		private CallbackManager CallbackManager { get; } = new();
		protected void Set<T>(T value, [CallerMemberName] string memberName = null!)
		{
			PropertyMap.AddOrUpdate(memberName,
				static (_, v) => v.NewValue,
				static (k, o, x) =>
				{
					var newValue = x.NewValue;
					if (o is not T oldValue || !EqualityComparer<T>.Default.Equals(oldValue, newValue))
					{
						x.Me.CallbackManager.TryInvokeCallback(newValue, k);
					}
					return newValue;


				},
				(Me: this, NewValue: value));
		} 
		protected T? Get<T>(Func<T?>? getDefault = null, [CallerMemberName] string memberName = null!)
		{
			return (T?)PropertyMap.GetOrAdd(memberName, static (k,  f) => f == null ? default: f() , getDefault);
		}
		public override async Task SetParametersAsync(ParameterView parameters)
		{
			this.CallbackManager.ProcessParameterView(ref parameters);
			await base.SetParametersAsync(parameters);
		}
		protected override void OnParametersSet()
		{
			base.OnParametersSet();
		}

		protected override Task OnParametersSetAsync()
		{
			return base.OnParametersSetAsync();
		}
	}
}
