using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
	public class CurrentCountTextPresenter : MonoBehaviour
	{
		public Text CurrentCountText;

		public void Initialize(Counter counter)
			=> counter.Incremented += newCount => CurrentCountText.text = newCount.ToString();
	}
}
