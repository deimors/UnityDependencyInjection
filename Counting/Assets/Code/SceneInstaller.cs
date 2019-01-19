using Zenject;

namespace Assets.Code
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			Container.Bind<Counter>().AsSingle();
		}
	}
}
