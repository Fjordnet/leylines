/* Unity3D Node Graph Framework
Copyright (c) 2017 Ava Pek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Defines a yield that may be returned from a node to wait for an event
	/// to occur.
	/// </summary>
	public abstract class Yield
	{
		/// <summary>
		/// True if the yield has finished waiting.
		/// </summary>
		public virtual bool Finished { get; protected set; }

		/// <summary>
		/// The total amount of time spent on this yield.
		/// </summary>
		public float Elapsed { get; protected set; }

		/// <summary>
		/// Called when an update event occurs.
		/// </summary>
		/// <param name="delta">
		/// The amount of time in seconds since the last time this was called.
		/// </param>
		public virtual void OnUpdate()
		{
			Elapsed += UnityEngine.Time.deltaTime;
		}
	}

	/// <summary>
	/// Returns the Socket to signal from to continue execution.
	/// </summary>
	public class SignalSocket : Yield
	{
		public override bool Finished { get { return true; } }

		public Socket Socket
		{
			get { return socket; }
		}
		private Socket socket;

		public GraphScope Scope
		{
			get { return scope; }
		}
		private GraphScope scope;

		public SignalSocket(GraphScope scope, Socket socket)
		{
			this.scope = scope;
			this.socket = socket;
		}
	}

	/// <summary>
	/// Yields until a certain number of seconds has passed.
	/// </summary>
	public class WaitForSeconds : Yield
	{
		private float seconds;

		public WaitForSeconds(float seconds)
		{
			this.seconds = seconds;
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			Finished = seconds <= Elapsed;
		}
	}

	/// <summary>
	/// Yields until the next update loop
	/// </summary>
	public class WaitForUpdate : Yield
	{
		public WaitForUpdate()
		{
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
			Finished = true;
		}
	}
}