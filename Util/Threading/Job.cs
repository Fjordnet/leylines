using System;
using System.Collections;
using System.Threading;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A convenience wrapper around a thread.
	/// </summary>
	public class Job
	{
		/// <summary>
		/// The thread performing the action.
		/// </summary>
		private readonly Thread thread;

		public bool IsRunning { get; set; }

		/// <summary>
		/// Creates a new job that executes the specified action.
		/// </summary>
		/// <param name="action">The action to execute.</param>
		public Job(Action action)
		{
			thread = new Thread(new ThreadStart(() =>
			{
				IsRunning = true;
				action();
				IsRunning = false;
			}));
		}

		/// <summary>
		/// Starts the job.
		/// </summary>
		/// <returns>This job, for chaining.</returns>
		public Job Start()
		{
			var masked = thread.ThreadState & ThreadState.Unstarted;
			if (masked == ThreadState.Unstarted)
			{
				thread.Start();
			}
			return this;
		}

		/// <summary>
		/// Aborts the job.
		/// </summary>
		/// <returns>This job, for chaining.</returns>
		public Job Abort()
		{
			thread.Abort();
			return this;
		}

		/// <summary>
		/// Returns an IEnumerator that terminates when the job has finished.
		/// </summary>
		/// <returns>
		/// An IEnumerator that terminates when the job has finished.
		/// </returns>
		public IEnumerator WaitFor()
		{
			var masked = thread.ThreadState & ThreadState.Stopped;
			while (masked != ThreadState.Stopped)
			{
				yield return null;
				masked = thread.ThreadState & ThreadState.Stopped;
			}
		}
	}
}