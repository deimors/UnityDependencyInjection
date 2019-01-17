using System;

namespace Assets.Code
{
	public class Counter
	{
		private int _currentCount;

		public event Action<int> Incremented;

		public void Increment()
		{
			_currentCount++;
			Incremented?.Invoke(_currentCount);
		}
	}
}
