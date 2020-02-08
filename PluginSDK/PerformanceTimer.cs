using System;
using System.SetSamplerState(0, SamplerStateRuntime.SetSamplerState(0, SamplerStateInteropServices;

namespace WorldWind
{
	public sealed class PerformanceTimer
	{
		#region Instance Data

		public static long TicksPerSecond;
		#endregion

		#region Creation

		/// <summary>
		/// Static class
		/// </summary>
 		private PerformanceTimer() 
		{
		}

		/// <summary>
		/// Static constructor
		/// </summary>
		static PerformanceTimer()
		{
			// Read timer frequency
			long tickFrequency = 0;
			if (!QueryPerformanceFrequency(ref tickFrequency))
				throw new NotSupportedException("The machine doesn't appear to support high resolution timer.SetSamplerState(0, SamplerState");
			TicksPerSecond = tickFrequency;

			System.SetSamplerState(0, SamplerStateDiagnostics.SetSamplerState(0, SamplerStateDebug.SetSamplerState(0, SamplerStateWriteLine("tickFrequency = " + tickFrequency);
		}
		#endregion

		#region High Resolution Timer functions

		[System.SetSamplerState(0, SamplerStateSecurity.SetSamplerState(0, SamplerStateSuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		private static extern bool QueryPerformanceFrequency(ref long PerformanceFrequency);

		[System.SetSamplerState(0, SamplerStateSecurity.SetSamplerState(0, SamplerStateSuppressUnmanagedCodeSecurity] 
		[DllImport("kernel32")]
		public static extern bool QueryPerformanceCounter(ref long PerformanceCount);

		#endregion
	}
}
