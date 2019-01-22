# Dependency Injection with Zenject

How does your Unity project grow? Does it unfold as beautiful layers of abstraction, each class and function with just the right purpose, each new feature obvious and simple to implement, each addition revealing a greater understanding of the full picture? Or does it lurch along, a tangled mess of cross-referenced GameObjects, methods with ever-growing signatures, MonoBehaviours that need to be wired up "just right", and Manager classes that you just can't seem to stop adding variables to? While I can't guarantee a way to achieve the former, the latter can generally be avoided by implementing the _[Dependency Injection](https://en.wikipedia.org/wiki/Dependency_injection)_ pattern.

Simply put, dependency injection involves a _Container_ object being used to satisfy the dependencies of other _Client_ objects, either by passing the dependencies in through the client's constructor, properties, or methods. The simplest method of dependency injection is to manually instantiate and compose together classes into an object graph within a single class in the system (the _composition root_), however working within an existing framework can often pose a problem for this approach. Within Unity, the classes of most interest derive from `MonoBehaviour` and are attached to GameObjects in the scene hierarchy, however their instantiation is out of the developer's hands. To overcome this difficulty, the Zenject dependency injection library for Unity provides automatic discovery and injection of properties and methods attributed with `[Inject]` in MonoBehaviours in the scene, and the following is an example of its use.

## Driving from the Domain with Events

An important benefit of delegating the responsibility of object composition and dependency injection to a specific part of the system is that different layers of abstraction can be easily maintained without classes in either layer having to "own" a reference to the other. This eliminates a common use for "Manager" classes, which are often just convenient places for references to collect. In order to illustrate this benefit, this example will involve a simple architecture which establishes clear boundaries between the model, presentation, and infrastructure concerns. This architecture will be a simple example of _[Domain-driven Design](https://en.wikipedia.org/wiki/Domain-driven_design)_

# The Model Layer

The model layer defines the domain of the system as high-level concepts and interactions, independent of how these concepts might be presented to the user, saved to disk, transmitted over a network, etc.. For this system we will use a simple event-based model that accepts commands and emits events, but does not otherwise expose state (ie: no readable or mutable properties, no query methods).

## Counter Model

Create a new Unity project with the name `Counting`, and then create a `Code` folder in the project's `Assets` folder.

In the `Code` folder create a new C# Script with the name `Counter`

![Create Counter Model](https://deimors.github.io/UnityDependencyInjection/Images/Create%20Counter%20Model.png)

Open `Counter` and rewrite it to be a simple class

```c#
namespace Assets.Code
{
	public class Counter
	{
	
	}
}
```

This is going to be our domain model. It's a simple domain model; all it does is increment a counter. For now, we'll just expect it to emit an `Incremented` event containing the current count as an integer.

```c#
using System;

namespace Assets.Code
{
	public class Counter
	{
		public event Action<int> Incremented;
	}
}
```

We'll now need a way to trigger the `Incremented` event. Working back from the past tense to the imperative, an `Increment` command can be added to the model. The model also needs to define state, so that each time the `Increment` command is called the next integer in sequence is emitted with the event.

```c#
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
```

Note that the `_currentCount` state of the model isn't exposed publicly; this is intentional. Instead, any other class interested in the state of the model needs to observe the `Incremented` events, thereby allowing the data model to change without affecting the dependents of this model.

# The Presentation Layer

The presentation layer is the closest to the user, and is concerned with handling user input and output. In this system, the presentation layer is comprised both of content defined in the Unity editor and scripts which form the "glue" between Unity and the Model layer (ie: _Presenters_).

## Increment Button

Add a `UI > Canvas` to the scene, and a `UI > Button` to the canvas. Name that button `Increment Button` and update the text to read "Increment".

![Add Increment Button](https://deimors.github.io/UnityDependencyInjection/Images/Add%20Increment%20Button.png)

## Current Count Text

Add a `UI > Text` to the canvas and name it `Current Counter Text`. Adjust the Y position to 100 (off the center), the height to 120, the font size to 86 and center the text vertically and horizontally.

![Add Current Count Text](https://deimors.github.io/UnityDependencyInjection/Images/Add%20Current%20Count%20Text.png)

## Increment Button Presenter

Create a new C# Script `IncrementButtonPresenter` and rewrite it to be:

```c#
using UnityEngine;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		
	}
}
```

This presenter class will be the glue that binds together the view (the `Increment Button` created above) and the model (the `Counter` class). To accomplish this, it will need a reference to both objects.

Because the presenter is a `MonoBehaviour` which will be attached to the button in the Unity editor, the simplest way to reference the `Increment Button` is to assign it through a public field. While fields assigned in the editor can lead to a brittle dependencies when they cross large distances in the hierarchy, they are relatively stable when assigned to other components of the same `GameObject`.

```c#
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
	public class IncrementButtonPresenter : MonoBehaviour
	{
		public Button IncrementButton;
	}
}
```

We will also need the `Counter` model reference injected into the presenter. Since the presenter is a `MonoBehaviour`, constructor injected isn't possible, and so instead we will make use of method injection in order to both inject the model dependency and initialize the binding between the button and the model.

```c#
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
```

The `IncrementButtonPresenter` can now be added to the `Increment Button` in the editor, and the `IncrementButton` field can be assigned to the `Button` component.

![Add IncrementButtonPresenter](https://deimors.github.io/UnityDependencyInjection/Images/Add%20IncrementButtonPresenter.png)

## Current Count Text Presenter

Similarly, create a `CurrentCountTextPresenter` C# Script with a public property reference to the current count `Text` component, and a method injected with the `Counter` model which updates the text when an `Incremented` event occurs.

```c#
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
```

This presenter can now be added to `Current Count Text` in the editor, and the `CurrentCountText` field can be assigned to the `Text` component.

![Add CurrentCountTextPresenter](https://deimors.github.io/UnityDependencyInjection/Images/Add%20CurrentCountPresenter.png)

# The Infrastructure Layer

## Zenject Package

Now it's time to use the Zenject dependency injection container to satisfy the `Counter` model reference shared between the `IncrementButtonPresenter` and the `CurrentCountTextPresenter`. First, download and import the Zenject Dependency Injection IOC package from the Unity Asset Store.

![Import Zenject](https://deimors.github.io/UnityDependencyInjection/Images/Import%20Zenject.png)

## Scene Installer

Next, create a new C# Script named `SceneInstaller`. This class will be used to configure the Zenject container by inheriting from the `Zenject.MonoInstaller` base class and overriding the `InstallBindings()` method.

```c#
using Zenject;

namespace Assets.Code
{
	public class SceneInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			
		}
	}
}
```

The only binding which we need to be configured in the container is the `Counter` model binding. The same instance of this model needs to be injected into both the `IncrementButtonPresenter` and the `CurrentCountTextPresenter`, and so the model will be bound as a singleton.

```c#
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
```

Next, add a `Zenject > Scene Context` to the scene.

![Add SceneContext](https://deimors.github.io/UnityDependencyInjection/Images/Add%20Scene%20Context.png)

Then add the `SceneInstaller` component to the newly created `SceneContext`, and add a reference to the installer component to the list of Mono Installers.

![Add SceneInstaller to Scene Context](https://deimors.github.io/UnityDependencyInjection/Images/Add%20SceneInstaller%20to%20Scene%20Context.png)

Finally, it is necessary to mark the `Initialize(...)` methods in `IncrementButtonPresenter` and `CurrentCountTextPresenter` with the `[Inject]` attribute provided by Zenject. When the scene is started, the `SceneContext` will satisfy the dependencies of all public methods marked with the `[Inject]` attribute in scripts attached to GameObjects in the scene.

```c#
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
```

```c#
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
```

## Play Scene

Return to the Unity editor and play the scene.


![Play Scene](https://deimors.github.io/UnityDependencyInjection/Images/Play%20Scene.png)

Clicking the `Increment Button` fires the `onClick` event, on which the `IncrementButtonPresenter` registered the `Counter.Increment()` method as a listener. The model's `Increment()` method then updates its state and emits the `Incremented` event, which has a listener registered by the `CurrentCountTextPresenter` that updates the current value of the `Current Count Text`.

# Architecture

Architecture is concerned with the organization of a system in order to guide the changes that are made to the system over time. Packages are an organizational tool which can be used to draw boundaries and establish directions of dependency within a system. Unity has recently introduced the ability to create Assembly Definitions, which can be used to designate a folder as belonging to a specific assembly (the .Net term for "package"). By encapsulating the code we've written so far into separate assemblies and defining the dependencies between these assemblies we can prevent model code from being accidentally coupled to presentation or infrastructure code, thereby creating clear layers within the system.

## Arrange Folders

Create a new folder named `Model` and move `Counter` into it, then move `IncrementButtonPresenter` and `CurrentCountPresenter` into a new folder named `Presentation`, and finally move `SceneInstaller` into a new `Infrastructure` folder.

![Arrange Folders](https://deimors.github.io/UnityDependencyInjection/Images/Arrange%20Folders.png)

## Infrastructure Assembly

Under the `Infrastructure` code folder, create an Assembly Definition also named `Infrastructure`.

![Infrastructure Assembly Definition](https://deimors.github.io/UnityDependencyInjection/Images/Infrastructure%20Assembly%20Definition.png)

This assembly definition causes two errors occur for the `SceneInstaller` class because the newly defined assembly does not reference the Zenject assembly. The Zenject plugin folder also contains an assembly definition, named `zenject`, and so a reference can be added from the `Infrastructure` project to the `zenject` project in order to handle these errors.

![Reference Zenject from Infrastructure](https://deimors.github.io/UnityDependencyInjection/Images/Reference%20Zenject%20from%20Infrastructure.png)

The addition of the `zenject` reference will uncover a new error, namely that `Infrastructure` assembly does not have a reference to the `Content` model. This is because the code in an assembly does not have access to code not belonging to an assembly, even though code not in an assembly has access to code placed in assemblies (such as when `SceneInstaller` had yet to be placed in an assembly, yet was able to access code from the `zenject` assembly).

## Model Assembly

To solve this error, create a new Assembly Definition named `Model` in the `Model` code folder and reference this new assembly from the `Infrastructure` assembly.

![Reference Model from Infrastructure](https://deimors.github.io/UnityDependencyInjection/Images/Reference%20Model%20from%20Infrastructure.png)

## Presentation Assembly

Finally, create a `Presentation` Assembly Definition under the `Presentation` folder, and add references to both the `Model` and `zenject` assemblies.

![Presentation Assembly Definition](https://deimors.github.io/UnityDependencyInjection/Images/Presentation%20Assembly%20Definition.png)
