using System;

namespace WorldWind
{
	/// <summary>
	/// 
	/// </summary>
	interface IRenderable : IDisposable
	{
		void Initialize(DrawArgs drawArgs);
		void Update(DrawArgs drawArgs);
		void Render(DrawArgs drawArgs);
	}
}
