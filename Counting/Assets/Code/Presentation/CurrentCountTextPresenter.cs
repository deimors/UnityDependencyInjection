using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Code
{
	public class CurrentCountTextPresenter : MonoBehaviour
	{
		public Text CurrentCountText;

		[Inject]
		public void Initialize(Counter counter)
			=> counter.Incremented += newCount => CurrentCountText.text = newCount.ToString();
	}
}
