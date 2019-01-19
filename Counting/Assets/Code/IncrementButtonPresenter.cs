using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		public Button IncrementButton;

		[Inject]
		public void Initialize(Counter counter)
			=> IncrementButton.onClick.AddListener(counter.Increment);
	}
}
