using System;
using System.Timers;
using System.Windows.Forms;
using NumbersCore.Primitives;
using NumbersCore.Utils;
using Timer = System.Timers.Timer;

namespace NumbersAPI.Motion
{
	public class Runner
	{
		public static Runner CurrentRunner;
		public static Runner GetRunnerById(int id) => CurrentRunner;

		public IAgent Agent { get; set; }
		private readonly Control _display;

		private bool _isPaused;
		private static DateTime _pauseTime;
		private static TimeSpan _delayTime = new TimeSpan(0);
		public static DateTime StartTime { get; private set; }

		static Runner()
		{
			StartTime = DateTime.Now;
		}

		private Timer _sysTimer;
		private TimeSpan _lastTime;
		private TimeSpan _currentTime;
		public double CurrentMs => _currentTime.TotalMilliseconds;


		public Runner(Control display)
		{
			CurrentRunner = this;
			_display = display;
			Initialize();
		}

		private void Initialize()
		{
			_currentTime = DateTime.Now - StartTime;
			_lastTime = _currentTime;

			_sysTimer = new Timer();
			_sysTimer.Elapsed += Tick;
			_sysTimer.Interval = 8;
			_sysTimer.Enabled = true;
		}

        //private float t = 0;
        private bool _isBusy = false;
        private bool _needsUpdate = true;
        public bool HasUpdated { get; set; } = false;
        public bool NeedsUpdate() => _needsUpdate = true;

		private void Tick(object sender, ElapsedEventArgs e)
		{
			if (!_isPaused && !_isBusy && _needsUpdate)
			{
				_isBusy = true;
				_currentTime = e.SignalTime - (StartTime + _delayTime);
				double deltaTime = (_currentTime - _lastTime).TotalMilliseconds;
				//Composites.Update(CurrentMs, deltaTime);
				_display?.Invalidate();
				_lastTime = _currentTime;
				_needsUpdate = !HasUpdated;
			}

			_isBusy = false;
		}

		public void ActivateComposite(int id)
		{
			//var composite = Composites[id];
			//if (composite != null)
			//{
			//	if (composite is ITimeable anim)
			//	{
			//		anim.StartTime = (float) (DateTime.Now - Runner.StartTime).TotalMilliseconds;
			//	}

			//	Composites.ActivateElement(composite.Id);
			//}
		}

		public void DeactivateComposite(int id)
		{
			//Composites.DeactivateElement(id);
		}

		public void Clear()
		{
			//Composites.Clear();
		}

		public void Reset()
		{
			//Composites.Reset();
		}

		public void Pause()
		{
			if (!_isPaused)
			{
				OnPause(this, null);
			}
		}

		public void UnPause()
		{
			if (_isPaused)
			{
				OnPause(this, null);
			}
		}
		public void TogglePause()
		{
			OnPause(this, null);
		}

        private void OnPause(object sender, EventArgs e)
		{
			_isPaused = !_isPaused;
			if (_isPaused)
			{
				_pauseTime = DateTime.Now;
				//foreach (var id in Composites.ActiveIds)
				//{
				//	if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
				//	{
				//		((ITimeable) Composites[id]).Pause();
				//	}
				//}
			}
			else
			{
				_delayTime += DateTime.Now - _pauseTime;
				_lastTime = DateTime.Now - (StartTime + _delayTime);
				//foreach (var id in Composites.ActiveIds)
				//{
				//	if (Composites.ContainsKey(id) && (Composites[id] is ITimeable))
				//	{
				//		((ITimeable) Composites[id]).Resume();
				//	}
				//}
			}
		}

	}
}