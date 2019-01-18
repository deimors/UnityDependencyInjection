using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		public Button IncrementButton;

		public void Initialize(Counter counter)
			=> IncrementButton.onClick.AddListener(counter.Increment);
	}
}
