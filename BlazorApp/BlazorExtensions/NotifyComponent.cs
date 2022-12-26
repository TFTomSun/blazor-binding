using Microsoft.AspNetCore.Components;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace BlazorApp.BlazorExtensions
{
	public abstract class NotifyComponent : ComponentBase
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
			return (T?)PropertyMap.GetOrAdd(memberName, static (k, f) => f == null ? default : f(), getDefault);
		}
		public override async Task SetParametersAsync(ParameterView parameters)
		{
			CallbackManager.ProcessParameterView(ref parameters);
			await base.SetParametersAsync(parameters);
		}
	}

}
